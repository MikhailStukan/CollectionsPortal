using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CollectionsPortal.ViewModels;
using CollectionsPortal.Models;
using CollectionsPortal.Data;

namespace CollectionsPortal.Controllers
{
    [Authorize(Policy = "RequireAdmin")]
    public class Admin : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public Admin(ApplicationDbContext db)
        {
            _context = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users(int currentPageIndex = 1)
        {
            var usersView = new UserViewModel
            {
                UsersPerPage = 10,
                Users = _context.Users.OrderBy(u => u.Id),
                CurrentPage = currentPageIndex
            };

            return View(usersView);
        }

    }
}
