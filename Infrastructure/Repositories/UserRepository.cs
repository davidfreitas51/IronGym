using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using IronGym.Application.Services;
using IronGym.Shared.Entities;
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

        public bool ChangePassword(string email, string newPassword)
        {
            User user = GetUserByEmail(email);
            if (user != null)
            {
                _securityService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            return false;
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
                user.IsEmailVerified = false;

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

        public List<ShowUsersModel> GetAllUsers()
        {
            List<User> users = _context.Users.Where(u => u.Role == "User").ToList();
            List<ShowUsersModel> showUsers = new List<ShowUsersModel>();
            foreach (User user in users)
            {
                ShowUsersModel userModel = new ShowUsersModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                };
                showUsers.Add(userModel);
            }
            return showUsers;
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
