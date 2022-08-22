using CollectionsPortal.Models;
using Microsoft.AspNetCore.Mvc;
using CollectionsPortal.Data;
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private List<string> GetTags()
        {
            List<string> tagNames = new List<string>();

            var tags = _context.Tags.ToList();
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    tagNames.Add(tag.Name);
                }
            }
            return tagNames;
        }
    }
}