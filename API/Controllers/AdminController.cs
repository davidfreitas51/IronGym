using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using IronGym.Application.Services;
using IronGym.Shared.Entities;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly AESService _aesService;
        private readonly EmailService _emailService;

        public AdminController(UserRepository userRepository, ISecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
            _emailService = new EmailService();
        }

        [HttpPost("RegisterEmployee")]
        public IActionResult RegisterEmployee(EmployeeModel employee)
        {
            User user = _userRepository.GetUserByEmail(employee.Email);
            if (user != null)
            {
                return BadRequest("User already exists.");
            }

            string password = _emailService.SendPasswordEmail(employee.Email);
            User newUser = new User
            {
                Name = employee.Name,
                Email = employee.Email,
            };

            if (_userRepository.AddEmployee(newUser, password))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            List<ShowUsersModel> employees = _userRepository.GetAllEmployees();
            string jsonEmployees = JsonConvert.SerializeObject(employees);
            return Ok(jsonEmployees);
        }

        [HttpGet("GetEmployeeByEmail")]
        public IActionResult GetEmployeeByEmail(string email)
        {
            User employee = _userRepository.GetUserByEmail(email);
            if (employee == null)
            {
                return NotFound();
            }
            string jsonEmployee = JsonConvert.SerializeObject(employee);
            return Ok(jsonEmployee);
        }

        [HttpPost("EditEmployee")]
        public IActionResult EditEmployee(int id)
        {
            return Ok();
        }

        [HttpPost("DeleteEmployee")]
        public IActionResult DeleteEmployee(int id)
        {
            return Ok();
        }
    }
}
