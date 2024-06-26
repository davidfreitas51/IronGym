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
        public SecurityService _securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
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
                          NormalizedEmail = "USER@EXAMPLE.COM"
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task CheckIfEmailIsAlreadyRegistered_ShouldBeTrue()
        {
            var dbContext = await GetDbContext();
            var repository = new UserRepository(dbContext, _securityService);
            
            var result = repository.CheckIfEmailIsAlreadyRegistered("UsEr@examPle.com");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfEmailIsAlreadyRegistered_ShouldBeFalse()
        {
            var dbContext = await GetDbContext();
            var repository = new UserRepository(dbContext, _securityService);

            var result = repository.CheckIfEmailIsAlreadyRegistered("invalidUser@example.com");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddUser_ShouldBeTrue()
        {
            var dbContext = await GetDbContext();
            var repository = new UserRepository(dbContext, _securityService);
            User user = new User()
            {
                Email = "user@example.com",
            };

            bool result = repository.AddUser(user, "password");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_ShouldBeFalse()
        {
            var dbContext = await GetDbContext();
            var repository = new UserRepository(dbContext, _securityService);
            User user = new User();

            bool result = repository.AddUser(user, "password");

            result.Should().BeFalse();
        }
    }
}
