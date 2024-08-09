using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        bool CheckIfEmailIsAlreadyRegistered(string email);
        string GetEmailVerificationCode(string email);
        bool ValidateCode(string email, string codeInput);
        bool VerifyEmail(string email);
        bool ChangePassword(string email, string newPassword);
        bool AddUser(User user, string password);
        User GetUserByEmail(string email);
        List<User> GetAllUsers();
        bool UpdateUser(User user);
        bool DeleteUser(int userId);
    }
}