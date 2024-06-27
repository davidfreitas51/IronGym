using FakeItEasy;
using FluentAssertions;
using IronGym.Shared.Entities;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MVC.Controllers;
using MVC.Services;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class HomeControllerTests
{
    private readonly IRequestSenderService _fakeRequestSenderService;
    private readonly IAESService _fakeAESService;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _fakeRequestSenderService = A.Fake<IRequestSenderService>();
        _fakeAESService = A.Fake<IAESService>(); // Mock IAESService
        _controller = new HomeController(_fakeAESService, _fakeRequestSenderService);

        // Set up TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(), A.Fake<ITempDataProvider>());
    }

    [Fact]
    public async Task GetStarted_Post_ValidModel_RedirectsToEmailVerification()
    {
        // Arrange
        var newAcc = new NewAccountViewModel { Email = "test@example.com", Password = "password123" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.PostRequest(newAcc, A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await _controller.GetStarted(newAcc) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("EmailVerification");
    }

    [Fact]
    public async Task EmailVerification_Get_Success_ReturnsView()
    {
        // Arrange
        var userEmail = "test@example.com";
        _controller.TempData["Email"] = userEmail; // Set TempData for the test
        var encryptedEmail = "mockedEncryptedString"; // Mock encrypted string
        A.CallTo(() => _fakeAESService.EncryptAES(userEmail))
            .Returns(encryptedEmail);

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.GetRequest(A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await _controller.EmailVerification() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull();
    }

    // Add more tests for other actions such as EmailVerification_Post, Login, etc.
}
