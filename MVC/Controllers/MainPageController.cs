using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    [Authorize(Roles = "User")]
    public class MainPageController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }
    }
}
