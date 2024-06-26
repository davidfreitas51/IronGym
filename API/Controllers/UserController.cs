using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
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

        [HttpGet("/GetVerificationEmail/{id}")]
        public IActionResult GetVerificationEmail(string email)
        {
            User user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            string code = _userRepository.GetEmailVerificationCode(email);

            return Ok(user);
        }
    }
}
