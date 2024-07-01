using API.Controllers;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace APITests
{
    public class MainPageControllerTests
    {
        private readonly MainPageController _mainPageController;
        private readonly ISecurityService _securityService;
        private readonly UserRepository _repository;

        public MainPageControllerTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            var _dbContext = GetDbContext().Result;
            _repository = new UserRepository(_dbContext, _securityService);
            _mainPageController = new MainPageController(_repository);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "user0@example.com")
            }, "mock"));

            _mainPageController.ControllerContext = new ControllerContext()
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
                          Age = 25 + i
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task GetInfo_ShouldReturnOk()
        {
            var result = _mainPageController.GetInfo("user0@example.com") as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var userInfoJSON = result.Value as string;
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoJSON);

            userInfo.Should().NotBeNull();
            userInfo.Name.Should().Be("User");
            userInfo.Email.Should().Be("user0@example.com");
            userInfo.ChestCircumference.Should().Be(100);
            userInfo.ForearmCircumference.Should().Be(30);
            userInfo.ArmCircumference.Should().Be(35);
            userInfo.HipCircumference.Should().Be(95);
            userInfo.ThighCircumference.Should().Be(60);
            userInfo.CalfCircumference.Should().Be(40);
            userInfo.Weight.Should().Be(70);
            userInfo.Height.Should().Be(175);
            userInfo.Age.Should().Be(25);
        }

        [Fact]
        public async Task GetInfo_ShouldReturnNotFound()
        {
            var result = _mainPageController.GetInfo("nonexistentuser@example.com") as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }
    }
}
