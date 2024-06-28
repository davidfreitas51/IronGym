using IronGym.Shared.Entities;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using System.Security.Claims;
using System.Text.Json;

namespace MVC.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class EmployeeController : Controller
    {
        private readonly IAESService _aesService;
        private readonly IRequestSenderService _requestSenderService;
        private readonly HttpClient _httpClient;
        public EmployeeController(IAESService aesService, IRequestSenderService requestSenderService)
        {
            _aesService = aesService;
            _requestSenderService = requestSenderService;
            _httpClient = new HttpClient();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7175/api/employee/GetAll");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var userList = JsonSerializer.Deserialize<List<ShowUsersModel>>(responseData);
                return View(userList);
            }
            return View(new List<ShowUsersModel>());
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response = await _requestSenderService.PostRequest(login, "https://localhost:7175/api/employee/Login");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ViewBag.Errors = "Invalid credentials";
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.Errors = "This employee was not found";
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ViewBag.Errors = "You don't have permission to access this resource";
                return View();
            }
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var employeeLogin = JsonSerializer.Deserialize<EmployeeLoginModel>(responseString);

                var token = employeeLogin.Token;
                var role = employeeLogin.Role;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(3)
                };
                Response.Cookies.Append("JWToken", token, cookieOptions);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, login.Email),
                    new Claim(ClaimTypes.Role, role)
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

                return RedirectToAction("Index");
            }
            ViewBag.Errors = "An error has occured";
            return View();
        }


        private string GetJwtTokenFromCookie()
        {
            if (Request.Cookies.TryGetValue("JWToken", out var token))
            {
                return token;
            }
            return null;
        }

    }
}
