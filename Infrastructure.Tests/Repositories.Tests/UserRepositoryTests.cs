using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
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

            result.Should().BeAssignableTo<List<User>>();
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
    }
}
