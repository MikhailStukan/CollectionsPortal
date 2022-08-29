using CollectionsPortal.Data;
using CollectionsPortal.Models;
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
            Collection collection = _context.Collections.Find(collectionId);

            if (collection == null)
                return NotFound();

            ViewBag.Collection = collection;

            var collOwner = _context.Users.Where(p => p.Id == collection.UserId).ToList();

            foreach (var user in collOwner)
            {
                ViewBag.Owner = user.UserName;
            }

            var items = _context.Items.Where(p => p.CollectionId == collectionId);
            ViewBag.Items = items;

            return View();
        }

        
        public async Task<IActionResult> Delete(int collection)
        {
            var coll = _context.Collections.OrderBy(p => p.Id).Include(p => p.Items).First();

            if (coll == null)
                return NotFound();

            var collOwner = _context.Users.Where(p => p.Id == coll.UserId).ToList();

            foreach(var user in collOwner)
            {
                if(User.Identity.Name != null && (User.Identity.Name == user.UserName || User.IsInRole("Administrator")))
                {
                    _context.Collections.Remove(coll);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Profile");
        }
    }
}
