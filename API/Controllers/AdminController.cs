﻿using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
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

        public AdminController(UserRepository userRepository, ISecurityService securityService, AESService aesService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _aesService = aesService;
        }

        [HttpPost("NewEmployee")]
        public IActionResult NewEmployee(int id)
        {
            // Implement the logic for creating a new employee
            return Ok();
        }

        [HttpGet("GetEmployee")]
        public IActionResult GetAllEmployees()
        {
            // List<User> employees = _userRepository.GetAllEmployees(); // Assuming a method to get all employees exists
            // string jsonEmployees = JsonConvert.SerializeObject(employees);
            // return Ok(jsonEmployees);
            return Ok();
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
            // Implement the logic for editing an employee
            return Ok();
        }

        [HttpPost("DeleteEmployee")]
        public IActionResult DeleteEmployee(int id)
        {
            // Implement the logic for deleting an employee
            return Ok();
        }
    }
}