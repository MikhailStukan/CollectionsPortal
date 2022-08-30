using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CollectionsPortal.ViewModels;
using CollectionsPortal.Models;

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
            var collection = _context.Collections.Where(p => p.Id == collectionId).Include(p => p.User).Include(p => p.Items).First();

            if (collection == null)
                return NotFound();

            ViewBag.Collection = collection;

            ViewBag.Owner = collection.User;

            ViewBag.Items = collection.Items;

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

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            Collection collection = new Collection()
            {
                User = user,
                Name = model.Name,
                Description = model.Description,
                FieldTemplates = model.Fields,
                Topic = topic
            };

            await _context.AddAsync(collection);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });

        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int collection)
        {
            var coll = _context.Collections.OrderBy(p => p.Id).Include(p => p.Items).Include(p => p.User).First();

            if (coll == null)
                return NotFound();

                if(User.Identity.Name != null && (User.Identity.Name == coll.User.UserName || User.IsInRole("Administrator")))
                {
                    _context.Collections.Remove(coll);
                    await _context.SaveChangesAsync();
                }

            return RedirectToAction("Index", "Profile");
        }

        }
    }

