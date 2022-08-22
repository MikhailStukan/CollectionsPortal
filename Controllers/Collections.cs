using Microsoft.AspNetCore.Mvc;
using CollectionsPortal.Data;
using CollectionsPortal.Models;

namespace CollectionsPortal.Controllers
{
    public class Collections : Controller
    {
        private readonly ApplicationDbContext _context;

        public Collections(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpGet]
        public ActionResult Index(int collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            return View();
        }
    }
}
