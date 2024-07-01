using API.Controllers;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace APITests
{
    public class EmployeeControllerTests
    {
        private readonly EmployeeController _employeeController;
        private readonly ISecurityService _securityService;
        private readonly UserRepository _repository;
        private readonly AESService _aesService;

        public EmployeeControllerTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            _aesService = new AESService();
            var _dbContext = GetDbContext().Result;
            _repository = new UserRepository(_dbContext, _securityService);
            _employeeController = new EmployeeController(_repository, _securityService, _aesService);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "user0@example.com"),
                new Claim(ClaimTypes.Role, "Employee")
            }, "mock"));

            _employeeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        public async Task<IronGymContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IronGymContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new IronGymContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Users.CountAsync() < 10)
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
                          ChestCircumference = 100 + i,
                          ForearmCircumference = 30 + i,
                          ArmCircumference = 35 + i,
                          HipCircumference = 95 + i,
                          ThighCircumference = 60 + i,
                          CalfCircumference = 40 + i,
                          Weight = 70 + i,
                          Height = 175 + i,
                          Age = 25 + i,
                          Role = i % 2 == 0 ? "Employee" : "User" 
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task Login_ShouldReturnOk()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user0@example.com",
                Password = "password123"
            };

            var result = _employeeController.Login(loginViewModel) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var employeeLoginJSON = result.Value as string;
            var employeeLogin = JsonConvert.DeserializeObject<EmployeeLoginModel>(employeeLoginJSON);

            employeeLogin.Should().NotBeNull();
            employeeLogin.Role.Should().Be("Employee");
            employeeLogin.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturnNotFound()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "nonexistentuser@example.com",
                Password = "password123"
            };

            var result = _employeeController.Login(loginViewModel) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user1@example.com", 
                Password = "password123"
            };

            var result = _employeeController.Login(loginViewModel) as UnauthorizedResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user0@example.com",
                Password = "wrongpassword"
            };

            var result = _employeeController.Login(loginViewModel) as BadRequestResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = _employeeController.GetAll() as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var jsonUsers = result.Value as string;
            var users = JsonConvert.DeserializeObject<List<ShowUsersModel>>(jsonUsers);

            users.Should().NotBeNull();
            users.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnOk()
        {
            var newAccount = new NewAccountViewModel
            {
                Email = "newemployee@example.com",
                Password = "password123"
            };

            var result = _employeeController.RegisterEmployee(newAccount) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var user = result.Value as User;
            user.Should().NotBeNull();
            user.Email.Should().Be(newAccount.Email);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnBadRequest()
        {
            var newAccount = new NewAccountViewModel
            {
                Email = "user0@example.com",
                Password = "password123"
            };

            var result = _employeeController.RegisterEmployee(newAccount) as BadRequestResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetUserInfo_ShouldReturnOk()
        {
            var result = _employeeController.GetUserInfo(1) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var userInfoJSON = result.Value as string;
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoJSON);

            userInfo.Should().NotBeNull();
            userInfo.Email.Should().Be("user@example.com");
        }

        [Fact]
        public async Task GetUserInfo_ShouldReturnNotFound()
        {
            var result = _employeeController.GetUserInfo(999) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOk()
        {
            var userInfo = new UserInfo
            {
                Email = "user0@example.com",
                Name = "Updated User",
                ChestCircumference = 105,
                ForearmCircumference = 32,
                ArmCircumference = 37,
                HipCircumference = 98,
                ThighCircumference = 63,
                CalfCircumference = 42,
                Weight = 72,
                Height = 178,
                Age = 26
            };

            var result = _employeeController.UpdateUser(userInfo) as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound()
        {
            var userInfo = new UserInfo
            {
                Email = "nonexistentuser@example.com",
                Name = "Updated User",
                ChestCircumference = 105,
                ForearmCircumference = 32,
                ArmCircumference = 37,
                HipCircumference = 98,
                ThighCircumference = 63,
                CalfCircumference = 42,
                Weight = 72,
                Height = 178,
                Age = 26
            };

            var result = _employeeController.UpdateUser(userInfo) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOk()
        {
            var result = _employeeController.DeleteUser(1) as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound()
        {
            var result = _employeeController.DeleteUser(999) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }
    }
}
