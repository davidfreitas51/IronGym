using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        public PasswordController(UserRepository userRepository, ISecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
        }

        [HttpGet("GetResetingPasswordCode/{encryptedEmail}")]
        public IActionResult GetVerificationCode(string encryptedEmail)
        {
            string email = _aesService.DecryptAES(encryptedEmail);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            _userRepository.GetEmailVerificationCode(email);
            return Ok();
        }

        [HttpPost("CheckResetingPasswordCode")]
        public IActionResult CheckResetingPasswordCode(VerificationCodeModel verificationCodeModel)
        {
            string email = _aesService.DecryptAES(verificationCodeModel.Email);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }
            if (user.VerificationCode != verificationCodeModel.VerificationCode)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(NewPasswordModel newPasswordModel)
        {
            string email = _aesService.DecryptAES(newPasswordModel.Email);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            bool passwordChanged = _userRepository.ChangePassword(email, newPasswordModel.NewPassword);

            if (passwordChanged)
            {
                return Ok(new { message = "Password changed successfully." });
            }
            else
            {
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }
    }
}
