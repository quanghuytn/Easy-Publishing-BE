using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.DTOs.User;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<UserDto?> GetUserByUsernameOrEmail(string usernameOrEmail);
        Task AddNewUser(User user);
        void ResetPassword(int userId, string newHashedPassword);
        Task<AccountDto?> getAccountById(int id);
        //string? updateAvatar(int userId, AvatarForm data);
        Task<string> SwitchStatus(string email);
        //Task<List<UserDTO>> GetAllUsers();
    }
}
