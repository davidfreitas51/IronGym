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
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        public UserController(UserRepository userRepository, ISecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser(NewAccountViewModel newAccount)
        {
            if (_userRepository.CheckIfEmailIsAlreadyRegistered(newAccount.Email))
            {
                return BadRequest();
            }
            User user = new User
            {
                Name = newAccount.Name,
                Email = newAccount.Email,
            };
            _userRepository.AddUser(user, newAccount.Password);
            return Ok(user);
        }

        [HttpGet("GetVerificationCode/{encryptedEmail}")]
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

        [HttpPost("CheckVerificationCode")]
        public IActionResult CheckVerificationCode(VerificationCodeModel verificationCodeModel)
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
            _userRepository.VerifyEmail(email);
            return Ok();
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel login)
        {
            User user = _userRepository.GetUserByEmail(login.Email);

            if (user == null)
            {
                return NotFound();
            }
            if (user.IsEmailVerified == false)
            {
                return Unauthorized();
            }
            if (!_securityService.ComparePasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }

            string token = _securityService.CreateToken(user);
            return Ok(token);
        }
    }
}
