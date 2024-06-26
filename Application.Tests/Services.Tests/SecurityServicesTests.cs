using Application.Services;
using Domain.Entities;
using FluentAssertions;

namespace Application.Tests.Services.Tests
{
    public class SecurityServicesTests
    {
        public SecurityService _securityService = new SecurityService("exampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleStringexampleString");

        [Fact]
        public void CreatePasswordHash_ShouldReturnHashedString()
        {
            string password = "examplePassword";

            _securityService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            passwordHash.Should().NotBeEmpty();
            passwordSalt.Should().NotBeEmpty();
            passwordHash.Length.Should().BeGreaterThan(0);
            passwordSalt.Length.Should().BeGreaterThan(0);
        }


        [Fact]
        public void CreatePasswordHash_ShouldReturnTrue()
        {
            string password = "examplePassword";
            _securityService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            bool result = _securityService.ComparePasswordHash("examplePassword", passwordHash, passwordSalt);

            result.Should().BeTrue();
        }

        [Fact]
        public void CreatePasswordHash_ShouldReturnFalse()
        {
            string password = "examplePassword";
            _securityService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            bool result = _securityService.ComparePasswordHash("wrongPassword", passwordHash, passwordSalt);

            result.Should().BeFalse();
        }


        [Fact]
        public void CreateToken_ShouldBeString()
        {
            User user = new User
            {
                Email = "test",
                Role = "test"
            };

            string token = _securityService.CreateToken(user);

            token.Length.Should().NotBe(0);
        }

        [Fact]
        public void CreateToken_ShouldBeNull()
        {
            User user = new User();

            string token = _securityService.CreateToken(user);

            token.Should().BeNull();
        }
    }
}
