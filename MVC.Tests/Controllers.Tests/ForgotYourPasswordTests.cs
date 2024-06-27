using FakeItEasy;
using FluentAssertions;
using IronGym.Shared.Entities;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MVC.Controllers;
using MVC.Services;
using System.Net;

public class ForgotYourPasswordControllerTests
{
    private readonly IRequestSenderService _fakeRequestSenderService;
    private readonly IAESService _fakeAESService;
    private readonly ForgotYourPasswordController _controller;

    public ForgotYourPasswordControllerTests()
    {
        _fakeRequestSenderService = A.Fake<IRequestSenderService>();
        _fakeAESService = A.Fake<IAESService>();
        _controller = new ForgotYourPasswordController(_fakeAESService, _fakeRequestSenderService);

        // Set up TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(), A.Fake<ITempDataProvider>());
    }

    [Fact]
    public void AddEmail_Get_ReturnsView()
    {
        // Act
        var result = _controller.AddEmail() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull();
    }

    [Fact]
    public async Task AddEmail_Post_ValidModel_RedirectsToVerificationCode()
    {
        // Arrange
        var userEmail = new ForgotYourPasswordEmail { Email = "test@example.com" };
        var encryptedEmail = "mockedEncryptedString";
        A.CallTo(() => _fakeAESService.EncryptAES(userEmail.Email))
            .Returns(encryptedEmail);

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.GetRequest(A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await _controller.AddEmail(userEmail) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("VerificationCode");
        result.ControllerName.Should().Be("ForgotYourPassword");
    }

    [Fact]
    public void VerificationCode_Get_ReturnsView()
    {
        // Act
        var result = _controller.VerificationCode() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull();
    }

    [Fact]
    public async Task VerificationCode_Post_ValidModel_RedirectsToChangePassword()
    {
        // Arrange
        var verificationCodeModel = new VerificationCodeModel { VerificationCode = "123456" };
        var userEmail = "test@example.com";
        var encryptedEmail = "mockedEncryptedString";

        _controller.TempData["Email"] = userEmail;
        A.CallTo(() => _fakeAESService.EncryptAES(userEmail))
            .Returns(encryptedEmail);

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.PostRequest(verificationCodeModel, A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await _controller.VerificationCode(verificationCodeModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("ChangePassword");
    }

    [Fact]
    public void ChangePassword_Get_ReturnsView()
    {
        // Act
        var result = _controller.ChangePassword() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull();
    }

    [Fact]
    public async Task ChangePassword_Post_ValidModel_RedirectsToLogin()
    {
        // Arrange
        var newPasswordModel = new NewPasswordModel { NewPassword = "newPassword123", ConfirmPassword = "newPassword123" };
        var userEmail = "test@example.com";
        var encryptedEmail = "mockedEncryptedString";

        _controller.TempData["Email"] = userEmail;
        A.CallTo(() => _fakeAESService.EncryptAES(userEmail))
            .Returns(encryptedEmail);

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => _fakeRequestSenderService.PostRequest(newPasswordModel, A<string>._))
            .Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await _controller.ChangePassword(newPasswordModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Login");
        result.ControllerName.Should().Be("Home");
    }
}
