using System.ComponentModel.DataAnnotations;

namespace IronGym.Shared.Entities
{
    public class ForgotYourPasswordEmail
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
