using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionsPortal.Controllers
{
    public class Collections : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public Collections(ApplicationDbContext db, UserManager<User> userManager)
        {
            _context = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int collectionId)
        {
            var collection = _context.Collections.Where(p => p.Id == collectionId).Include(p => p.User).Include(p => p.Items).Include(p => p.Topic).Include(p => p.FieldTemplates).FirstOrDefault();

            if (collection == null)
                return NotFound();

            ViewBag.Collection = collection;

            ViewBag.Owner = collection.User;

            ViewBag.Items = collection.Items;

            ViewBag.Fields = collection.FieldTemplates;

            ViewBag.Topic = collection.Topic;

            return View();
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Topics = await _context.Topics.ToListAsync();

            return View();
        }

        [Authorize(Policy = "RequireUser")]
        [HttpPost]
        public async Task<IActionResult> CreateCollection(CreateCollectionViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var topic = await _context.Topics.FirstOrDefaultAsync(p => p.Id == model.Topic.Id);
            var tags = model.Tags.Split(",");

            Collection collection = new Collection()
            {
                User = user,
                Name = model.Name,
                Description = model.Description,
                FieldTemplates = model.Fields,
                Topic = topic,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            foreach(var tag in tags)
            {
                Tag t = new Tag()
                {
                    Name = tag
                };

                await _context.Tags.AddAsync(t);

                TagsToCollection tagsTo = new TagsToCollection()
                {
                    Collection = collection,
                    Tag = t
                };

                await _context.TagsToCollections.AddAsync(tagsTo);
            }


            await _context.AddAsync(collection);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });

        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int collectionId)
        {
            var coll = _context.Collections.OrderBy(p => p.Id).Include(p => p.Items).Include(p => p.User).First();

            if (coll == null)
                return NotFound();

            if (User.Identity.Name != null && (User.Identity.Name == coll.User.UserName || User.IsInRole("Administrator")))
            {
                _context.Collections.Remove(coll);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Profile");
        }

    }
}

