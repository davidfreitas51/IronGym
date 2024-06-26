using FluentAssertions;
using IronGym.Shared.Services;

namespace ServicesTests
{
    public class AESServiceTests
    {
        private readonly AESService _aesService = new AESService();
        [Fact]
        public void EncryptAES_ShouldBeString()
        {
            string decriptedText = "exampleText";

            string encryptedText = _aesService.EncryptAES(decriptedText);

            encryptedText.Should().Be("4oqT1vVW48OfhP/cqPiM8g==");
        }

        [Fact]
        public void EncryptAES_ShouldNotBeString()
        {
            string decriptedText = "exampleText";

            string encryptedText = _aesService.EncryptAES(decriptedText);

            encryptedText.Should().NotBe("incorrectValue");
        }

        [Fact]
        public void DecriptAES_ShouldBeString()
        {
            string encryptedText = "4oqT1vVW48OfhP/cqPiM8g==";

            string decriptedText = _aesService.DecryptAES(encryptedText);

            decriptedText.Should().Be("exampleText");
        }

        [Fact]
        public void DecryptAES_ShouldNotBeString()
        {
            string encryptedText = "4oqT1vVW48OfhP/cqPiM8g==";

            string decriptedText = _aesService.DecryptAES(encryptedText);

            decriptedText.Should().NotBe("incorrectText");
        }
    }
}
