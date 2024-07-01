using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IronGym.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories.Tests
{
    public class UserRepositoryTests
    {
        private readonly SecurityService _securityService;
        private readonly IronGymContext _dbContext;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            _dbContext = GetDbContext().Result;
            _repository = new UserRepository(_dbContext, _securityService);
        }

        public async Task<IronGymContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IronGymContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new IronGymContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Users.CountAsync() < 1)
            {

                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Users.Add(
                      new User()
                      {
                          Email = "User@example.com",
                          Name = "User",
                          NormalizedEmail = "USER@EXAMPLE.COM",
                          VerificationCode = "123456"
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async void GetEmailVerificationCode_ShouldReturnVerificationCode()
        {
            var result = _repository.GetEmailVerificationCode("User@example.com");

            result.Should().NotBeNullOrEmpty(); 
            result.Length.Should().Be(6);
        }

        [Fact]
        public async void ValidateCode_ShouldBeTrue()
        {
            var result = _repository.ValidateCode("User@example.com", "123456");

            result.Should().BeTrue();
        }

        [Fact]
        public async void ValidateCode_ShouldBeFalseIncorrectCode()
        {
            var result = _repository.ValidateCode("User@example.com", "654321");

            result.Should().BeFalse();
        }

        [Fact]
        public async void ValidateCode_ShouldBeFalseEmailNotFound()
        {
            var result = _repository.ValidateCode("User@example.com", "654321");

            result.Should().BeFalse();
        }

        [Fact]
        public async void VerifyEmail_ShouldBeTrue()
        {
            var result = _repository.VerifyEmail("User@example.com");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfEmailIsAlreadyRegistered_ShouldBeTrue()
        {
            var result = _repository.CheckIfEmailIsAlreadyRegistered("UsEr@examPle.com");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfEmailIsAlreadyRegistered_ShouldBeFalse()
        {
            var result = _repository.CheckIfEmailIsAlreadyRegistered("invalidUser@example.com");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddUser_ShouldBeTrue()
        {
            User user = new User()
            {
                Email = "user@example.com",
            };

            bool result = _repository.AddUser(user, "password");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_ShouldBeFalse()
        {
            User user = new User();

            bool result = _repository.AddUser(user, "password");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNullForNonExistent()
        {
            var result = _repository.GetUserByEmail("NonExistentUser@example.com");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnList()
        {
            var result = _repository.GetAllUsers();

            result.Should().BeAssignableTo<List<ShowUsersModel>>();
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnTrue()
        {
            var result = _repository.ChangePassword("user@example.com", "newPassword");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnFalse()
        {
            var result = _repository.ChangePassword("invalidUser@example.com", "newPassword");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            var userId = 1;
            var result = _repository.GetUserById(userId);
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Assuming no user with Id 999 exists
            var nonExistentUserId = 999;
            var result = _repository.GetUserById(nonExistentUserId);
            result.Should().BeNull();
        }


        [Fact]
        public async Task GetAllEmployees_ShouldReturnAllEmployees()
        {
            var result = _repository.GetAllEmployees();

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.Should().Contain(e => e.Email == "employee@example.com");
        }

        [Fact]
        public async Task GetEmployeeInfo_ShouldReturnEmployeeInfo_WhenEmployeeExists()
        {
            var result = _repository.GetEmployeeInfo(2);

            result.Should().NotBeNull();
            result.Name.Should().Be("Jane Smith");
            result.Email.Should().Be("employee@example.com");
        }

        [Fact]
        public void UpdateEmployee_ShouldReturnTrue_WhenEmployeeExists()
        {
            var updateModel = new EmployeeModel
            {
                Email = "employee@example.com",
                Name = "Updated Name"
            };

            var result = _repository.UpdateEmployee(updateModel);

            result.Should().BeTrue();

            var updatedUser = _dbContext.Users.FirstOrDefault(u => u.Email == updateModel.Email);
            updatedUser.Should().NotBeNull();
            updatedUser.Name.Should().Be(updateModel.Name);
        }

        [Fact]
        public void UpdateEmployee_ShouldReturnFalse_WhenEmployeeDoesNotExist()
        {
            var updateModel = new EmployeeModel
            {
                Email = "nonexistent@example.com",
                Name = "NewName"
            };

            var result = _repository.UpdateEmployee(updateModel);

            result.Should().BeFalse();
        }

        [Fact]
        public void UpdateUser_ShouldReturnTrue_WhenUserExists()
        {
            var updateModel = new UserInfo
            {
                Email = "user@example.com",
                Name = "Updated User",
                ChestCircumference = 95,
                ForearmCircumference = 27,
                ArmCircumference = 32,
                HipCircumference = 100,
                ThighCircumference = 58,
                CalfCircumference = 38,
                Weight = 75,
                Height = 180,
                Age = 32
            };

            var result = _repository.UpdateUser(updateModel);

            result.Should().BeTrue();

            var updatedUser = _dbContext.Users.FirstOrDefault(u => u.Email == updateModel.Email);
            updatedUser.Should().NotBeNull();
            updatedUser.Name.Should().Be(updateModel.Name);
            updatedUser.ChestCircumference.Should().Be(updateModel.ChestCircumference);
            updatedUser.ForearmCircumference.Should().Be(updateModel.ForearmCircumference);
            updatedUser.ArmCircumference.Should().Be(updateModel.ArmCircumference);
            updatedUser.HipCircumference.Should().Be(updateModel.HipCircumference);
            updatedUser.ThighCircumference.Should().Be(updateModel.ThighCircumference);
            updatedUser.CalfCircumference.Should().Be(updateModel.CalfCircumference);
            updatedUser.Weight.Should().Be(updateModel.Weight);
            updatedUser.Height.Should().Be(updateModel.Height);
            updatedUser.Age.Should().Be(updateModel.Age);
        }

        [Fact]
        public void UpdateUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var updateModel = new UserInfo
            {
                Email = "nonexistent@example.com",
                Name = "Updated User",
                ChestCircumference = 95,
                ForearmCircumference = 27,
                ArmCircumference = 32,
                HipCircumference = 100,
                ThighCircumference = 58,
                CalfCircumference = 38,
                Weight = 75,
                Height = 180,
                Age = 32
            };

            var result = _repository.UpdateUser(updateModel);

            result.Should().BeFalse();
        }

        [Fact]
        public void DeleteUser_ShouldReturnTrue_WhenUserExists()
        {
            int userIdToDelete = 1;

            var result = _repository.DeleteUser(userIdToDelete);

            result.Should().BeTrue();

            var deletedUser = _dbContext.Users.FirstOrDefault(u => u.Id == userIdToDelete);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public void DeleteUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            int nonExistentUserId = 999;

            var result = _repository.DeleteUser(nonExistentUserId);

            result.Should().BeFalse();
        }

        [Fact]
        public void AddEmployee_ShouldReturnTrue_WhenEmployeeAddedSuccessfully()
        {
            var employee = new User
            {
                Email = "newemployee@example.com",
                Name = "New Employee"
            };
            string password = "password123";

            var result = _repository.AddEmployee(employee, password);

            result.Should().BeTrue();

            var addedEmployee = _dbContext.Users.FirstOrDefault(u => u.Email == employee.Email);
            addedEmployee.Should().NotBeNull();
            addedEmployee.Role.Should().Be("Employee");
            addedEmployee.IsEmailVerified.Should().BeTrue();
        }

        [Fact]
        public void AddEmployee_ShouldReturnFalse_WhenEmployeeEmailAlreadyExists()
        {
            var existingEmployee = new User
            {
                Email = "employee@example.com",
                Name = "Existing Employee"
            };
            string password = "password123";

            _dbContext.Users.Add(existingEmployee);
            _dbContext.SaveChanges();

            var result = _repository.AddEmployee(existingEmployee, password);

            result.Should().BeFalse();
        }
    }
}