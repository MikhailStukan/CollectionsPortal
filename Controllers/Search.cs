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

        public async Task<IActionResult> Index(int tag)
        {
            try
            {
                var itemsTags = await _context.TagsToItems.Where(p => p.Tag.Id == tag).Include(p => p.Item).Include(p => p.Tag).Include(p => p.Item.Collection).Include(p => p.Item.Collection.User).ToListAsync();
                var collectionTags = await _context.TagsToCollections.Where(p => p.Tag.Id == tag).Include(p => p.Collection).Include(p => p.Tag).ToListAsync();

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
            catch (Exception e)
            {
                return View("Error", e.Message);
            }

        }

        public async Task<IActionResult> TextSearch(string text)
        {
            try
            {
                var resultsItems = await _context.Items.Include(p => p.Collection).Include(p => p.Collection.User).Where(c => (c.Name + c.Collection.Name + c.Collection.Description).Contains(text)).ToListAsync();
                var resultComments = await _context.Comments.Include(p => p.Item).Include(p => p.Item.Collection).Include(p => p.User).Where(p => p.Content.Contains(text)).ToListAsync();
                var resultFields = await _context.Fields.Include(p => p.Item).Include(p => p.Item.Collection.User).Where(p => p.Value.Contains(text)).ToListAsync();

                if (resultFields.Any())
                {
                    foreach (var fields in resultFields)
                    {
                        if (!resultsItems.Contains(fields.Item))
                        {
                            resultsItems.Add(fields.Item);
                        }
                    }
                }

                if (resultComments.Any())
                {
                    foreach (var comment in resultComments)
                    {
                        if (!resultsItems.Contains(comment.Item))
                        {
                            resultsItems.Add(comment.Item);
                        }

                    }
                }

                ViewBag.items = resultsItems;

                return View();
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
    }
}
