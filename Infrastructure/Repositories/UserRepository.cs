using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly IronGymContext _context;
        private readonly ISecurityService _securityService;

        public UserRepository(IronGymContext context, ISecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
        }

        public bool CheckIfEmailIsAlreadyRegistered(string email)
        {
            return _context.Users.Any(u => u.Email.ToUpper() == email.ToUpper());
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
            catch (Exception ex)
            {
                return false;
            }
        }
        public User GetUserById(int userId)
        {
            return _context.Set<User>().Find(userId);
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
