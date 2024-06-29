using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainPageController : ControllerBase
    {

        private readonly UserRepository _userRepository;
        public MainPageController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet("GetInfo/{email}")]
        [Authorize]
        public IActionResult GetInfo(string email)
        {
            User user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }

            UserInfo userInfo = new UserInfo
            {
                Name = user.Name,
                Email = user.Email,
                ChestCircumference = user.ChestCircumference,
                ForearmCircumference = user.ForearmCircumference,
                ArmCircumference = user.ArmCircumference,
                HipCircumference = user.HipCircumference,
                ThighCircumference = user.ThighCircumference,
                CalfCircumference = user.CalfCircumference,
                Weight = user.Weight,
                Height = user.Height,
                Age = user.Age
            };

            string userInfoJSON = JsonConvert.SerializeObject(userInfo);
            return Ok(userInfoJSON);
        }
    }
}
