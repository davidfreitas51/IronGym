using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MVC.Services;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AESService _aesService;
        private readonly IRequestSenderService _requestSenderService;
        public HomeController(AESService aesService, IRequestSenderService requestSenderService)
        {
            _aesService = aesService;
            _requestSenderService = requestSenderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetStarted()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStarted([FromForm] NewAccountViewModel newAcc)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response = await _requestSenderService.PostRequest(newAcc, "https://localhost:7175/api/User/RegisterUser");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Errors = "This email is already in use";
                return View();
            }

            TempData["Email"] = newAcc.Email;

            return RedirectToAction("EmailVerification");
        }


        [HttpGet]
        public async Task<IActionResult> EmailVerification()
        {
            string userEmail = TempData.Peek("Email") as string;
            string encryptedEmail = _aesService.EncryptAES(userEmail);

            var response = await _requestSenderService.GetRequest("https://localhost:7175/api/User/GetVerificationCode/" + encryptedEmail);

            if (response.IsSuccessStatusCode)
            {
                return View();
            }

            ViewBag.Errors = "Error sending the verification e-mail";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailVerification([FromForm] VerificationCodeModel verCodeModel)
        {
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View();
            }

            string userEmail = TempData.Peek("Email") as string;
            verCodeModel.Email = _aesService.EncryptAES(userEmail);
            var response = await _requestSenderService.PostRequest(verCodeModel, "https://localhost:7175/api/User/CheckVerificationCode");

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Account successfully verified";
                return RedirectToAction("Login");
            }

            ViewBag.Errors = "Invalid code";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response = await _requestSenderService.PostRequest(login, "https://localhost:7175/api/User/Login");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("EmailVerification");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ViewBag.Errors = "Invalid credentials";
                return View();
            }
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(3)
                };
                Response.Cookies.Append("JWToken", token, cookieOptions);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, login.Email),
                    new Claim(ClaimTypes.Role, "User")
                };
                var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Vault");
            }
            return View();
        }
    }
}
