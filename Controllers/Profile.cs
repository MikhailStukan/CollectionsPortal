using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var collections = _context.Collections.Where(p => p.User.Id.Equals(user.Id));
            ViewBag.Collections = collections;

            return View();
        }

        


    }
}
