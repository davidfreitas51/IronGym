using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;

namespace MVC.Controllers
{
    public class ForgotYourPasswordController : Controller
    {
        private readonly AESService _aesService;
        private readonly RequestSenderService _requestSenderService;
        public ForgotYourPasswordController(AESService aesService, RequestSenderService requestSenderService)
        {
            _aesService = aesService;
            _requestSenderService = requestSenderService;
        }

        [HttpGet]
        public IActionResult AddEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmail([FromForm] ForgotYourPasswordEmail userEmail)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string encryptedEmail = _aesService.EncryptAES(userEmail.Email);
            var response = await _requestSenderService.GetRequest($"https://localhost:7175/GetResetingPasswordCode/{encryptedEmail}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("VerificationCode", "ForgotYourPassword");
            }
            ViewBag.Error = "An error occurred while processing your request.";
            return View();
        }

        [HttpGet]
        public IActionResult VerificationCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerificationCode([FromForm] ForgotYourPasswordEmail email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return RedirectToAction("ChangePassword");
        }
    }
}
