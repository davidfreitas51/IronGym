using Domain.Entities;
using IronGym.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminController()
        {
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
                var userInfo = JsonSerializer.Deserialize<UserInfo>(responseData);
                return View(userInfo);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string jsonClient = JsonSerializer.Serialize(userInfo);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7175/api/employee/UpdateUser", content);

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

            var response = await client.DeleteAsync($"https://localhost:7175/api/employee/DeleteUser/{id}");

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
    }
}
