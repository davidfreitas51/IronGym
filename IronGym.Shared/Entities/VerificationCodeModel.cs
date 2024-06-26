namespace IronGym.Shared.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class VerificationCodeModel
    {
        [Required(ErrorMessage = "Verification code is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits")]
        public string VerificationCode { get; set; }

        public string Email {  get; set; }
    }

}
