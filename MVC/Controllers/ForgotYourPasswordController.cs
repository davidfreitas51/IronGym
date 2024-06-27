using IronGym.Shared.Entities;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;

namespace MVC.Controllers
{
    public class ForgotYourPasswordController : Controller
    {
        private readonly IAESService _aesService;
        private readonly IRequestSenderService _requestSenderService;
        public ForgotYourPasswordController(IAESService aesService, IRequestSenderService requestSenderService)
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
            var response = await _requestSenderService.GetRequest($"https://localhost:7175/api/Password/GetResetingPasswordCode/{encryptedEmail}");

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
        public async Task<IActionResult> VerificationCode([FromForm] VerificationCodeModel verificationCodeModel)
        {
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View();
            }

            string userEmail = TempData.Peek("Email") as string;
            verificationCodeModel.Email = _aesService.EncryptAES(userEmail);
            var response = await _requestSenderService.PostRequest(verificationCodeModel, "https://localhost:7175/api/Password/CheckResetingPasswordCode");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ChangePassword");
            }

            ViewBag.Errors = "Invalid code";
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] NewPasswordModel newPasswordModel)
        {
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View();
            }

            string userEmail = TempData.Peek("Email") as string;
            newPasswordModel.Email = _aesService.EncryptAES(userEmail);

            var result = await _requestSenderService.PostRequest(newPasswordModel, "https://localhost:7175/api/Password/ChangePassword");

            TempData["MSG_S"] = "Password changed successfully";
            return RedirectToAction("Login", "Home");
        }
    }
}
