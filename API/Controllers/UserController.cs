using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly SecurityService _securityService;
        private readonly AESService _aesService;
        public UserController(UserRepository userRepository, SecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
        }

        [HttpPost("/RegisterUser")]
        public IActionResult RegisterUser(NewAccountViewModel newAccount)
        {
            if (!_userRepository.CheckIfEmailIsAlreadyRegistered(newAccount.Email))
            {
                return BadRequest();
            }
            User user = new User
            {
                Email = newAccount.Email,
            };
            _userRepository.AddUser(user, newAccount.Password);
            return Ok(user);
        }

        [HttpGet("/GetVerificationCode/{encryptedEmail}")]
        public IActionResult GetVerificationCode(string encryptedEmail)
        {
            string email = _aesService.DecryptAES(encryptedEmail);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            string code = _userRepository.GetEmailVerificationCode(email);
            return Ok(code);
        }

        [HttpGet("/CheckVerificationCode/{encryptedEmail}/{code}")]
        public IActionResult CheckVerificationCode(string encryptedEmail, string code)
        {
            string email = _aesService.DecryptAES(encryptedEmail);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(code);
        }

        [HttpPatch("/VerifyEmail/{encryptedEmail}")]
        public IActionResult VerifyEmail(string encryptedEmail)
        {
            string email = _aesService.DecryptAES(encryptedEmail);
            User user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            _userRepository.VerifyEmail(email);
            return Ok();
        }

        [HttpPost("/Login/")]
        public IActionResult VerifyEmail(LoginViewModel login)
        {
            if (!_userRepository.CheckIfEmailIsAlreadyRegistered(login.Email))
            {
                return BadRequest();
            }
            User user = _userRepository.GetUserByEmail(login.Email);

            if (user == null)
            {
                return NotFound();
            }
            if (user.IsEmailVerified == false)
            {
                return Unauthorized();
            }

            string token = _securityService.CreateToken(user);
            return Ok(token);
        }
    }
}
