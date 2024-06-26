using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class ForgotYourPassword : Controller
    {
        private readonly AESService _aesService;
        public ForgotYourPassword(AESService aesService)
        {
            _aesService = aesService;
        }

        [HttpGet]
        public IActionResult AddEmail()
        {
            return View();
        }
    }
}
