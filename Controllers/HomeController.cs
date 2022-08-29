using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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
            ViewData["tags"] = GetTags();
            ViewData["collections"] = BiggestCollections();
            ViewData["items"] = LastItems();
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
            List<Tag> tags = new List<Tag>();

            tags = _context.Tags.ToList();

            return tags;
        }

        private List<Collection> BiggestCollections()
        {
            List<Collection> collections = new List<Collection>();

            return collections;
        }

        private List<Item> LastItems()
        {
            List<Item> items = new List<Item>();

            return items;
        }

    }
}