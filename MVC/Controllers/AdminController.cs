using Domain.Entities;
using IronGym.Shared.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MVC.Services;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestSenderService _requestSenderService;
        public AdminController(IRequestSenderService requestSenderService)
        {
            _requestSenderService = requestSenderService;
            _httpClient = new HttpClient();
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7175/api/admin/GetAllEmployees");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var userList = JsonSerializer.Deserialize<List<ShowUsersModel>>(responseData);
                return View(userList);
            }
            return View(new List<ShowUsersModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] EmployeeModel employee)
        {
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                return View();
            }

            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            employee.Password = "XXXXXXXXXXX";
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string jsonClient = JsonSerializer.Serialize(employee);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7175/api/admin/RegisterEmployee", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); 
            }
            else
            {
                ViewBag.Error = "This e-mail is already in use.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://localhost:7175/api/admin/GetEmployeeInfo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<EmployeeModel>(responseData);
                return View(userInfo);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] EmployeeModel employeeModel)
        {
            ModelState.Remove("Password");
            if (!ModelState.IsValid)
            {
                return View();
            }

            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            employeeModel.Password = "XXXXXXXXXXX";
            string jsonClient = JsonSerializer.Serialize(employeeModel);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7175/api/admin/UpdateEmployee", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://localhost:7175/api/admin/DeleteEmployee/{id}");

            return RedirectToAction("Index");
        }
        private string GetJwtTokenFromCookie()
        {
            if (Request.Cookies.TryGetValue("JWToken", out var token))
            {
                return token;
            }
            return null;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginDefaultManager()
        {
            LoginViewModel login = new LoginViewModel
            {
                Email = "admin@example.com",
                Password = "Password"
            };
            var response = await _requestSenderService.PostRequest(login, "https://localhost:7175/api/employee/Login");

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
    }
}
