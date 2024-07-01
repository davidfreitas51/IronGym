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
    public class UserControllerTests
    {
        private readonly UserController _userController;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        private readonly UserRepository _repository;

        public UserControllerTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            var _dbContext = GetDbContext().Result;
            _aesService = new AESService();
            _repository = new UserRepository(_dbContext, _securityService);
            _userController = new UserController(_repository, _securityService, _aesService);
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
        public void RegisterUser_ShouldOk()
        {
            var newAccount = new NewAccountViewModel
            {
                Email = "newUser@example.com",
                Password = "password123"
            };

            var result = _userController.RegisterUser(newAccount);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void RegisterUser_ShouldReturnBadRequest_WhenEmailAlreadyRegistered()
        {
            var newAccount = new NewAccountViewModel
            {
                Email = "user1@example.com",
                Password = "password123"
            };

            var result = _userController.RegisterUser(newAccount);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void GetVerificationCode_ShouldReturnOk()
        {
            string encryptedEmail = _aesService.EncryptAES("user1@example.com");

            var result = _userController.GetVerificationCode(encryptedEmail);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void GetVerificationCode_ShouldReturnNotFound_WhenUserNotFound()
        {
            string encryptedEmail = _aesService.EncryptAES("nonexistent@example.com");

            var result = _userController.GetVerificationCode(encryptedEmail);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void CheckVerificationCode_ShouldReturnOk()
        {
            string encryptedEmail = _aesService.EncryptAES("user1@example.com");
            var verificationCodeModel = new VerificationCodeModel
            {
                Email = encryptedEmail,
                VerificationCode = "123456" 
            };

            var result = _userController.CheckVerificationCode(verificationCodeModel);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void CheckVerificationCode_ShouldReturnNotFound_WhenUserNotFound()
        {
            string encryptedEmail = _aesService.EncryptAES("nonexistent@example.com");
            var verificationCodeModel = new VerificationCodeModel
            {
                Email = encryptedEmail,
                VerificationCode = "123456"
            };

            var result = _userController.CheckVerificationCode(verificationCodeModel);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void CheckVerificationCode_ShouldReturnBadRequest_WhenVerificationCodeIncorrect()
        {
            string encryptedEmail = _aesService.EncryptAES("user1@example.com");
            var verificationCodeModel = new VerificationCodeModel
            {
                Email = encryptedEmail,
                VerificationCode = "654321" // Incorrect verification code
            };
            var result = _userController.CheckVerificationCode(verificationCodeModel);

            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public void Login_ShouldReturnOk_WithValidCredentials()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user1@example.com",
                Password = "password123"
            };

            var result = _userController.Login(loginViewModel);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Login_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            var result = _userController.Login(loginViewModel);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenEmailNotVerified()
        {
            var user = new User
            {
                Email = "unauthorizedUser@example.com",
            };
            _repository.AddUser(user, "password123");

            var loginViewModel = new LoginViewModel
            {
                Email = "unauthorizedUser@example.com",
                Password = "password123"
            };

            var result = _userController.Login(loginViewModel);

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenPasswordIncorrect()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user1@example.com",
                Password = "incorrectPassword"
            };

            var result = _userController.Login(loginViewModel);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
