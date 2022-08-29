using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CollectionsPortal.Controllers
{

    public class Item : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> CreateItem(Item item)
        {
            if(ViewBag.collection != null)
            {
              
            }
            return View();
        }

        [Authorize(Policy = "RequireUser")]
        public IActionResult Create(int collection)
        {
            ViewBag.collection = collection;
            return View();
        }
        [Authorize(Policy = "RequireUser")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
