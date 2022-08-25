using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectionsPortal.Controllers
{
    [Authorize(Policy = "RequireUser")]
    public class Profile : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public Profile(ApplicationDbContext db, UserManager<User> userManager)
        {
            _context = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);


            ViewBag.User = user;

            var collections = _context.Collections.Where(p => p.UserId.Equals(user.Id));
            ViewBag.Collections = collections;

            return View();
        }

        public async Task<IActionResult> Create()
        {
            var topics = _context.Topics.ToList();
            ViewBag.Topics = topics;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollection(Collection collection)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            collection.UserId = await _userManager.GetUserIdAsync(user);
            collection.CreatedAt = DateTime.Now;
            collection.UpdatedAt = DateTime.Now;

            _context.Collections.Add(collection);
            _context.SaveChanges();

            return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });
        }
    }
}
