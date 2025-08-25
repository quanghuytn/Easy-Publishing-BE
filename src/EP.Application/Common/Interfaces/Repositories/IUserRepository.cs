using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.DTOs.User;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<UserDto?> GetUserByUsernameOrEmail(string usernameOrEmail);
        void ResetPassword(int userId, string newHashedPassword);
        Task<AccountDto?> getAccountById(int id);
        //string? updateAvatar(int userId, AvatarForm data);
        Task<List<UserDto2>> GetAllUsers();
        Task<UserPurchaseInfoDto?> GetPurchaseInfoInStory(int userId, int storyId);
        public Task<bool> CheckPurchase(int? userId, long chapterNumber, int storyId);
    }
}
