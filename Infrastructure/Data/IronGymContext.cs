using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class IronGymContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public IronGymContext(DbContextOptions<IronGymContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SecurityService securityService = new SecurityService("randomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKeyrandomKey");
            securityService.CreatePasswordHash("Password", out byte[] passwordHash, out byte[] passwordSalt);

            modelBuilder.Entity<User>().HasData(new List<User>()
            {
                new User
                {
                    Id = 1,
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
                    Name = "John Doe",
                    Role = "User",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationCode = "123456",
                    IsEmailVerified = true,
                    Age = 30,
                    Height = 175,
                    Weight = 75,
                    ChestCircumference = 100,
                    ForearmCircumference = 30,
                    ArmCircumference = 35,
                    HipCircumference = 95,
                    ThighCircumference = 60,
                    CalfCircumference = 40
                },
                new User
                {
                    Id = 2,
                    Email = "employee@example.com",
                    NormalizedEmail = "EMPLOYEE@EXAMPLE.COM",
                    Name = "Jane Smith",
                    Role = "Employee",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationCode = "123456",
                    IsEmailVerified = true,
                    Age = 28,
                    Height = 163,
                    Weight = 60,
                    ChestCircumference = 90,
                    ForearmCircumference = 28,
                    ArmCircumference = 32,
                    HipCircumference = 88,
                    ThighCircumference = 55,
                    CalfCircumference = 38
                },
                new User
                {
                    Id = 3,
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    Name = "Admin Smith",
                    Role = "Admin",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationCode = "123456",
                    IsEmailVerified = true,
                    Age = 35,
                    Height = 180,
                    Weight = 85,
                    ChestCircumference = 105,
                    ForearmCircumference = 32,
                    ArmCircumference = 37,
                    HipCircumference = 100,
                    ThighCircumference = 62,
                    CalfCircumference = 42
                }
            });
        }
    }
}
