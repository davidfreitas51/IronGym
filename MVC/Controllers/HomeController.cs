using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AESService _aesService;
        public HomeController(AESService aesService)
        {
            _aesService = aesService;
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

            var response = await PostRequest(newAcc, "https://localhost:7175/RegisterUser");

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

            var response = await GetRequest("https://localhost:7175/GetVerificationCode/" + encryptedEmail);

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
            var response = await PostRequest(verCodeModel, "https://localhost:7175/CheckVerificationCode");

            if (response.IsSuccessStatusCode)
            {
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

            var response = await PostRequest(login, "https://localhost:7175/Login");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("EmailVerification");
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
                    // Propriedades adicionais podem ser configuradas aqui
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Vault");
            }
            return View();
        }

        public async Task<HttpResponseMessage> GetRequest(string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            return await httpClient.GetAsync(apiUrl);
        }

        public async Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(apiUrl, content);
        }

        public async Task<HttpResponseMessage> PatchRequest<T>(T obj, string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl)
            {
                Content = content
            };

            return await httpClient.SendAsync(request);
        }
    }
}
