using API.Controllers;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITests
{
    public class PasswordControllerTests
    {
        private readonly PasswordController _passwordController;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        private readonly UserRepository _repository;

        public PasswordControllerTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            var _dbContext = GetDbContext().Result;
            _aesService = new AESService();
            _repository = new UserRepository(_dbContext, _securityService);
            _passwordController = new PasswordController(_repository, _securityService, _aesService);
        }

        public async Task<IronGymContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IronGymContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new IronGymContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Users.CountAsync() < 4)
            {
                _securityService.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Users.Add(
                      new User()
                      {
                          Email = $"user{i}@example.com",
                          Name = "User",
                          PasswordHash = passwordHash,
                          PasswordSalt = passwordSalt,
                          NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                          VerificationCode = "123456",
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }


        [Fact]
        public void GetVerificationCode_ShouldReturnOk_WithValidEncryptedEmail()
        {
            string encryptedEmail = _aesService.EncryptAES("user1@example.com");

            var result = _passwordController.GetVerificationCode(encryptedEmail);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void GetVerificationCode_ShouldReturnNotFound_WithInvalidEncryptedEmail()
        {
            string encryptedEmail = _aesService.EncryptAES("invalidEncryptedEmail.com");

            var result = _passwordController.GetVerificationCode(encryptedEmail);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void CheckResetingPasswordCode_ShouldReturnOk_WithValidVerificationCode()
        {
            var verificationCodeModel = new VerificationCodeModel
            {
                Email = _aesService.EncryptAES("user1@example.com"),
                VerificationCode = "123456"
            };

            var result = _passwordController.CheckResetingPasswordCode(verificationCodeModel);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void CheckResetingPasswordCode_ShouldReturnBadRequest_WithInvalidVerificationCode()
        {
            var verificationCodeModel = new VerificationCodeModel
            {
                Email = _aesService.EncryptAES("user1@example.com"),
                VerificationCode = "654321"
            };

            var result = _passwordController.CheckResetingPasswordCode(verificationCodeModel);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void ChangePassword_ShouldReturnOk_WithValidNewPassword()
        {
            var newPasswordModel = new NewPasswordModel
            {
                Email = _aesService.EncryptAES("user1@example.com"),
                NewPassword = "newPassword123"
            };

            var result = _passwordController.ChangePassword(newPasswordModel);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
        }

        [Fact]
        public void ChangePassword_ShouldReturnStatusCode500_WhenErrorOccurs()
        {
            var newPasswordModel = new NewPasswordModel
            {
                Email = _aesService.EncryptAES("invalidEncryptedEmail.com"),
                NewPassword = "newPassword123"
            };

            var result = _passwordController.ChangePassword(newPasswordModel);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}