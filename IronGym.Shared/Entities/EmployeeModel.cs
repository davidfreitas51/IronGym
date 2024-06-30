using System.ComponentModel.DataAnnotations;

namespace IronGym.Shared.Entities
{
    public class EmployeeModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters", MinimumLength = 1)]
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
