using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace MVC.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        private readonly HttpClient _httpClient;
        public MainPageController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string email = string.Empty;    
            if (emailClaim != null)
            {
                email = emailClaim.Value;
            }

            var response = await client.GetAsync($"https://localhost:7175/api/MainPage/GetInfo/{email}" );
            var responseData = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(responseData);
            return View(userInfo);
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
