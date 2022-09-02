using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollectionsPortal.CloudStorage;
using Google.Cloud.Storage.V1;
using Google;
using Microsoft.Extensions.Options;

namespace CollectionsPortal.Controllers
{
    public class Collections : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICloudStorage _cloudStorage;

        public Collections(ApplicationDbContext db, UserManager<User> userManager, ICloudStorage cloudStorage)
        {
            _context = db;
            _userManager = userManager;
            _cloudStorage = cloudStorage;
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

        [HttpGet("getTags")]
        [Route("api/getTags")]
        public IActionResult GetTags()
        {
            string term = HttpContext.Request.Query["term"].ToString();
            var tags = _context.Tags.Where(p => p.Name.StartsWith(term)).Select(u => u.Name).ToList();
            return Ok(tags);
        }


        [Authorize(Policy = "RequireUser")]
        [HttpPost]
        public async Task<IActionResult> CreateCollection(CreateCollectionViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var topic = await _context.Topics.FirstOrDefaultAsync(p => p.Id == model.Topic.Id);
            var existingTags = _context.Tags.ToList().Select(u => u.Name);

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
            
            if(model.ImageFile != null)
            {
                string fileNameForStorage = $"{collection.Name}{collection.Id}{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(model.ImageFile.FileName)}";
                collection.imageUrl = await _cloudStorage.UploadFileAsync(model.ImageFile, fileNameForStorage);
            }


            foreach (var tag in tags)
            {
                if (!existingTags.Contains(tag))
                {
                    Tag newTag = new Tag()
                    {
                        Name = tag
                    };
                    TagsToCollection tagsTo = new TagsToCollection()
                    {
                        Collection = collection,
                        Tag = newTag
                    };
                    await _context.Tags.AddAsync(newTag);
                    await _context.TagsToCollections.AddAsync(tagsTo);
                }
                else
                {
                    TagsToCollection tagsTo = new TagsToCollection()
                    {
                        Collection = collection,
                        Tag = _context.Tags.Where(p => p.Name == tag).FirstOrDefault()
                    };
                    await _context.TagsToCollections.AddAsync(tagsTo);
                }
            }
                

            await _context.AddAsync(collection);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });

        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int collectionId)
        {
            var coll = _context.Collections.OrderBy(p => p.Id).Include(p => p.User).First();

            if (coll == null)
                return NotFound();

            if (User.Identity.Name != null && (User.Identity.Name == coll.User.UserName || User.IsInRole("Administrator")))
            {
                if (coll.imageUrl != null)
                {
                    Uri uri = new Uri(coll.imageUrl);
                    string fileName = uri.Segments.LastOrDefault();
                    await _cloudStorage.DeleteFileAsync(fileName);
                }

                _context.Collections.Remove(coll);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Profile");
        }

    }
}

