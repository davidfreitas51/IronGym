using FluentAssertions;
using IronGym.Application.Services;

namespace Application.Tests.Services.Tests
{
    public class EmailServiceTests
    {
        EmailService _emailService;
        public EmailServiceTests()
        {
             _emailService = new EmailService();
        }

        [Fact]
        public void SendVerificationEmail_ShouldReturnVerificationCode()
        {
            string email = "user@example.com";

            var result = _emailService.SendVerificationEmail(email);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(6, "because the verification code should be a six-character string");
        }

        [Fact]
        public void SendPasswordEmail_ShouldReturnPassword()
        {
            string email = "user@example.com";

            var result = _emailService.SendPasswordEmail(email);

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(8, "because the verification code should be a eight-character string");
        }
    }
}
