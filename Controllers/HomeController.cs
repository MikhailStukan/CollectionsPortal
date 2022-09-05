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

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.Tags = await GetTags();
                ViewBag.Collections = await BiggestCollections();
                ViewBag.Items = await LastItems();
            }
            catch
            {
                ViewBag.Tags = new List<Tag>();
                ViewBag.Collections = new List<Collection>();
                ViewBag.Items = new List<Models.Item>();
            }

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
        private async Task<List<Tag>> GetTags()
        {
            try
            {
                var tags = await _context.Tags.ToListAsync();
                return tags;
            }
            catch(Exception e)
            {
                return new List<Tag>();
            }
            
        }

        private async Task<List<Collection>> BiggestCollections()
        {
            try
            {
                var collections = await _context.Collections.Include(p => p.Items).OrderByDescending(p => p.Items.Count()).Include(p => p.User).ToListAsync();

                if (collections.Count() > 5)
                {
                    return collections.GetRange(0, 5);
                }
                else
                {
                    return collections;
                }
            }
            catch
            {
                return new List<Collection>();
            }  
        }

        private async Task<List<Models.Item>> LastItems()
        {
            try
            {
                var items = await _context.Items.OrderByDescending(p => p.CreatedAt).Include(p => p.Collection.User).ToListAsync();

                if (items.Count() > 5)
                {
                    return items.GetRange(0, 5);
                }
                else
                {
                    return items;
                }
            }
            catch
            {
                return new List<Models.Item>();
            }
            
        }

    }
}