using app.DTOs.Auth;
using app.DTOs.User;
using app.Models;

namespace app.Interface
{
    public interface IUserRepository
    {
        User? getUserByUsernameOrEmail(string usernameOrEmail);
        Task addNewUser(User user);
        void resetPassword(int userId, string newHashedPassword);
        AccountDto getAccountById(int id);
        User? getUserById(int id);
        string? updateAvatar(int userId, AvatarForm data);
        Task<string> SwitchStatus(string email);
        Task<List<UserDTO>> GetAllUsers();
    }
}
