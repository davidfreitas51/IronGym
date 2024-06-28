using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        public EmployeeController(UserRepository userRepository, ISecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel login)
        {
            User user = _userRepository.GetUserByEmail(login.Email);

            if (user == null)
            {
                return NotFound();
            }
            if (user.Role == "User")
            {
                return Unauthorized();
            }
            if (!_securityService.ComparePasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }

            EmployeeLoginModel employeeLogin = new EmployeeLoginModel
            {
                Token = _securityService.CreateToken(user),
                Role = user.Role
            };
            string employeeLoginJSON = JsonConvert.SerializeObject(employeeLogin);
            return Ok(employeeLoginJSON);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<ShowUsersModel> users = _userRepository.GetAllUsers();
            string jsonUsers = JsonConvert.SerializeObject(users);
            return Ok(jsonUsers);
        }

        [HttpPost("RegisterEmployee")]
        public IActionResult RegisterEmployee(NewAccountViewModel newAccount)
        {
            if (_userRepository.CheckIfEmailIsAlreadyRegistered(newAccount.Email))
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
    }
}
