using CollectionsPortal.Data;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionsPortal.Controllers
{

    public class Item : Controller
    {
        private readonly ApplicationDbContext _context;

        public Item(ApplicationDbContext db)
        {
            _context = db;
        }
        public async Task<IActionResult> Index(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Collection).Include(p => p.Comments).Include(p => p.Likes).FirstOrDefaultAsync();

            ViewBag.Item = item;
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create(int collectionId)
        {
            var collection = _context.Collections.Include(p => p.User).Include(p => p.Items).Include(p => p.FieldTemplates).Where(c => c.Id == collectionId).FirstOrDefault();

            if (collection == null)
            {
                return NotFound();
            }

            if (User.Identity.Name == collection.User.UserName || User.IsInRole("Administrator"))
            {
                ViewBag.Collection = collection;
            }
            else
            {
                return RedirectToAction("Index", "HomeController");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            var collection = _context.Collections.FirstOrDefault(p => p.Id == model.collectionId);

            foreach (var field in model.Fields)
            {
                field.FieldTemplates = _context.FieldTemplates.FirstOrDefault(p => p.Id == field.FieldTemplates.Id);
            }

            var item = new Models.Item()
            {
                Collection = collection,
                Name = model.Name,
                Description = model.Description,
                Fields = model.Fields,
                CreatedAt = DateTime.Now
            };

            collection.UpdatedAt = DateTime.Now;

            await _context.AddAsync(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Collections", new { collectionId = model.collectionId });
        }


        [Authorize(Policy = "RequireUser")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
