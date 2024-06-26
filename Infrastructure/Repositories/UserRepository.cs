using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using IronGym.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly IronGymContext _context;
        private readonly ISecurityService _securityService;
        private readonly EmailService _email;

        public UserRepository(IronGymContext context, ISecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
            _email = new EmailService();
        }

        public bool CheckIfEmailIsAlreadyRegistered(string email)
        {
            return _context.Users.Any(u => u.NormalizedEmail == email.ToUpper());
        }

        public string GetEmailVerificationCode(string email)
        {
            User user = GetUserByEmail(email);
            user.VerificationCode = _email.SendVerificationEmail(email);
            _context.SaveChanges();
            return user.VerificationCode;
        }

        public bool ValidateCode(string email, string codeInput)
        {
            User user = GetUserByEmail(email);
            if (user != null && user.VerificationCode == codeInput)
            {
                return true;
            }
            return false;
        }

        public bool VerifyEmail(string email)
        {
            User user = GetUserByEmail(email);
            user.IsEmailVerified = true;
            _context.SaveChanges();
            return true;
        }

        public bool AddUser(User user, string password)
        {
            try
            {
                _securityService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.NormalizedEmail = user.Email.ToUpper();
                user.Role = "User";

                _context.Users.Add(user);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.NormalizedEmail == email.ToUpper());
        }

        public List<User> GetAllUsers()
        {
            return _context.Set<User>().ToList();
        }

        public bool UpdateUser(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var user = _context.Set<User>().Find(userId);
                if (user == null)
                {
                    return false;
                }
                _context.Set<User>().Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
