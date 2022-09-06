using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionsPortal.Controllers
{

    public class Profile : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public Profile(ApplicationDbContext db, UserManager<User> userManager)
        {
            _context = db;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Index()
        {
            try
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.User = user;

                var collections = _context.Collections.Where(p => p.User.Id.Equals(user.Id)).Include(p => p.Items).Include(p => p.Topic).ToList();
                ViewBag.Collections = collections;

                return View();
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }

    }
}
