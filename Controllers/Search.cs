using Microsoft.AspNetCore.Mvc;
using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;

namespace CollectionsPortal.Controllers
{
    public class Search : Controller
    {
        private readonly ApplicationDbContext _context;

        public Search(ApplicationDbContext db)
        {
            _context = db;
        }

        public IActionResult Index(int tag)
        {
            var itemsTags = _context.TagsToItems.Where(p => p.TagId == tag).ToList();


            if(itemsTags != null)
            {
                foreach (var pair in itemsTags)
                {
                    var item = _context.Items.Where(p => p.Id == pair.ItemId).ToList();
                    if(item != null)
                    {
                        ViewBag.items = item;
                    }
                }
            }

            return View();
        }

        public IActionResult textSearch(string text)
        {
            //full text search of db implementation here

            return View();
        }
    }
}
