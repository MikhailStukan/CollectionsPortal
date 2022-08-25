﻿using CollectionsPortal.Models;
using Microsoft.AspNetCore.Mvc;
using CollectionsPortal.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;


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
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1)}
                );

            return LocalRedirect(returnUrl);
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