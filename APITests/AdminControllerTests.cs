using API.Controllers;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IronGym.Application.Services;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Xunit;

namespace APITests
{
    public class AdminControllerTests
    {
        private readonly AdminController _adminController;
        private readonly ISecurityService _securityService;
        private readonly UserRepository _repository;
        private readonly AESService _aesService;
        private readonly EmailService _emailService;

        public AdminControllerTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            _aesService = new AESService();
            _emailService = new EmailService();
            var _dbContext = GetDbContext().Result;
            _repository = new UserRepository(_dbContext, _securityService);
            _adminController = new AdminController(_repository, _securityService, _aesService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "adminuser"),
                            new Claim(ClaimTypes.Email, "admin@example.com"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "mock"))
                    }
                }
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
                          Role = i % 2 == 0 ? "Employee" : "User"  // Half are employees
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnOk()
        {
            var employeeModel = new EmployeeModel
            {
                Name = "New Employee",
                Email = "newemployee@example.com"
            };

            var result = _adminController.RegisterEmployee(employeeModel) as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnBadRequest()
        {
            var employeeModel = new EmployeeModel
            {
                Name = "Existing Employee",
                Email = "user0@example.com"
            };

            var result = _adminController.RegisterEmployee(employeeModel) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("User already exists.");
        }

        [Fact]
        public async Task GetAllEmployees_ShouldReturnOk()
        {
            var result = _adminController.GetAllEmployees() as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var jsonEmployees = result.Value as string;
            var employees = JsonConvert.DeserializeObject<List<ShowUsersModel>>(jsonEmployees);

            employees.Should().NotBeNull();
            employees.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetEmployeeByEmail_ShouldReturnOk()
        {
            var result = _adminController.GetEmployeeByEmail("user0@example.com") as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var jsonEmployee = result.Value as string;
            var employee = JsonConvert.DeserializeObject<User>(jsonEmployee);

            employee.Should().NotBeNull();
            employee.Email.Should().Be("user0@example.com");
        }

        [Fact]
        public async Task GetEmployeeByEmail_ShouldReturnNotFound()
        {
            var result = _adminController.GetEmployeeByEmail("nonexistentuser@example.com") as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetEmployeeInfo_ShouldReturnOk()
        {
            var result = _adminController.GetEmployeeInfo(1) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var jsonEmployee = result.Value as string;
            var employee = JsonConvert.DeserializeObject<User>(jsonEmployee);

            employee.Should().NotBeNull();
            employee.Email.Should().Be("user@example.com");
        }

        [Fact]
        public async Task EditEmployee_ShouldReturnOk()
        {
            var employeeModel = new EmployeeModel
            {
                Name = "Updated Employee",
                Email = "user0@example.com"
            };

            var result = _adminController.EditEmployee(employeeModel) as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task EditEmployee_ShouldReturnNotFound()
        {
            var employeeModel = new EmployeeModel
            {
                Name = "Nonexistent Employee",
                Email = "nonexistentuser@example.com"
            };

            var result = _adminController.EditEmployee(employeeModel) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnOk()
        {
            var result = _adminController.DeleteEmployee(1) as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnNotFound()
        {
            var result = _adminController.DeleteEmployee(999) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }
    }
}
