using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CollectionsPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
