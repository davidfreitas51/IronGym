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
        _fakeAESService = A.Fake<IAESService>();
        _controller = new HomeController(_fakeAESService, _fakeRequestSenderService);

        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(), A.Fake<ITempDataProvider>());
    }

    [Fact]
    public async Task GetStarted_Post_ValidModel_RedirectsToEmailVerification()
    {
        var newAcc = new NewAccountViewModel { Email = "test@example.com", Password = "password123" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.PostRequest(newAcc, A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        var result = await _controller.GetStarted(newAcc) as RedirectToActionResult;

        result.Should().NotBeNull();
        result.ActionName.Should().Be("EmailVerification");
    }

    [Fact]
    public async Task EmailVerification_Get_Success_ReturnsView()
    {
        var userEmail = "test@example.com";
        _controller.TempData["Email"] = userEmail;
        var encryptedEmail = "mockedEncryptedString";
        A.CallTo(() => _fakeAESService.EncryptAES(userEmail))
            .Returns(encryptedEmail);

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.GetRequest(A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        var result = await _controller.EmailVerification() as ViewResult;

        result.Should().NotBeNull();
        result.ViewName.Should().BeNull();
    }
}
