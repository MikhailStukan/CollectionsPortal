using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CollectionsPortal.Controllers
{
    [Authorize(Policy = "RequireUser")]
    public class Collection : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
