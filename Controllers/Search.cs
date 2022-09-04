using CollectionsPortal.Data;
using CollectionsPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var itemsTags = _context.TagsToItems.Where(p => p.Tag.Id == tag).Include(p => p.Item).Include(p => p.Tag).ToList();
            var collectionTags = _context.TagsToCollections.Where(p => p.Tag.Id == tag).Include(p => p.Collection).Include(p => p.Tag).ToList();

            List<Models.Item> items = new List<Models.Item>();
            List<Collection> collections = new List<Collection>();

            if (itemsTags != null)
            {
                foreach (var item in itemsTags)
                {
                    items.Add(item.Item);
                }
            }
            if (collectionTags != null)
            {
                foreach (var col in collectionTags)
                {
                    collections.Add(col.Collection);
                }
            }

            ViewBag.items = items;
            ViewBag.collections = collections;

            return View();
        }

        public IActionResult textSearch(string text)
        {
            //full text search of db implementation here

            return View();
        }
    }
}
