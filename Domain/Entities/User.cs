namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? VerificationCode { get; set; }
        public bool? IsEmailVerified { get; set; }
        public string? Role { get; set; }
    }
}
