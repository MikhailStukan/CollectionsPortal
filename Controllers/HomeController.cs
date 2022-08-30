using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace CollectionsPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _context = db;
        }

        public IActionResult Index()
        {
            ViewBag.Tags = GetTags();
            ViewBag.Collections = BiggestCollections();
            ViewBag.Items = LastItems();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

            return LocalRedirect(returnUrl);
        }
        private List<Tag> GetTags()
        {
            var tags = _context.Tags.ToList();

            return tags;
        }

        private List<Collection> BiggestCollections()
        {
            var collections = _context.Collections.Include(p => p.Items).OrderByDescending(p => p.Items.Count()).ToList();

            if (collections.Count() > 5)
            {
                return collections.GetRange(0, 5);
            }
            else
            {
                return collections;
            }
        }

        private List<Models.Item> LastItems()
        {
            var items = _context.Items.OrderByDescending(p => p.CreatedAt).ToList();

            if (items.Count() > 5)
            {
                return items.GetRange(0, 5);
            }
            else
            {
                return items;
            }
        }

    }
}