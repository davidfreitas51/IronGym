using FluentAssertions;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Controllers;
using MVC.Services;

namespace MVCTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly IRequestSenderService _requestSenderService;
        private readonly IAESService _aesService;

        public HomeControllerTests()
        {
            _aesService = new MockAESService(); // Implement or mock IAESService as needed
            _requestSenderService = new MockRequestSenderService(); // Implement or mock IRequestSenderService as needed
            _homeController = new HomeController(_aesService, _requestSenderService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public void Index_ShouldReturnViewResult()
        {
            var result = _homeController.Index() as ViewResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void GetStarted_Get_ShouldReturnViewResult()
        {
            var result = _homeController.GetStarted() as ViewResult;

            result.Should().NotBeNull();
        }

        private class MockAESService : IAESService
        {
            public string DecryptAES(string cipherText)
            {
                throw new NotImplementedException();
            }

            public string EncryptAES(string plainText)
            {
                // Mock encryption method for testing
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));
            }
        }

        private class MockRequestSenderService : IRequestSenderService
        {
            public Task<HttpResponseMessage> GetRequest(string url)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PostRequest<T>(T content, string url)
            {
                // Mock post request for testing
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                return Task.FromResult(response);
            }
        }
    }
}
