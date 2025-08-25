using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.DTOs.User;
using EP.Application.Common.DTOs.Wallet;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context)
        {
        }

        public async Task<bool> CheckPurchase(int? userId, long chapterNumber, int storyId)
        {
            if (userId == 0)
            {
                return false;
            }
            
            var user = await _dbSet.Where(u => u.UserId == userId).Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                Stories = u.StoriesNavigation.Select(sn => new { StoryId = sn.StoryId }).ToList(),
                Chapters = u.Chapters.Select(c => new { chapterId = c.ChapterId, ChapterNumber = c.ChapterNumber, StoryId = c.StoryId }).ToList()
            }).FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            if (user.RoleId == 1)
            {
                return true;
            }

            if (user.Chapters.Any(c => c.ChapterNumber == chapterNumber && c.StoryId == storyId) || user.Stories.Any(s => s.StoryId == storyId))
            {
                return true;
            }

            return false;
        }

        public async Task<AccountDto?> getAccountById(int id)
        {
            var user = await _dbSet.Where(u => u.UserId == id)
                .AsNoTracking()
                .Select(u => new AccountDto
                {
                    UserId = u.UserId,
                    Role = u.Role.RoleName,
                    Email = u.Email,
                    Username = u.Username,
                    UserFullname = u.UserFullname,
                    Gender = u.Gender == true ? "Male" : "Female",
                    Dob = u.Dob,
                    Address = u.Address,
                    Phone = u.Phone,
                    Status = u.Status == true ? "Active" : "Inactive",
                    UserImage = u.UserImage,
                    DescriptionMarkdown = u.DescriptionMarkdown,
                    DescriptionHTML = u.DescriptionHtml,
                    TLT = u.Wallets.Select(w => w.Fund).FirstOrDefault()
                }).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<UserDto2>> GetAllUsers()
        {
            return await _dbSet.Where(u => u.RoleId != 1)
               .Select(u => new UserDto2
               {
                   Id = u.UserId,
                   FullName = u.UserFullname,
                   Email = u.Email,
                   Phone = u.Phone,
                   Username = u.Username,
                   Password = u.Password,
                   Dob = u.Dob.ToString(),
                   UserImage = u.UserImage,
                   Status = (u.Status == true ? "Active" : "Inactive"),
                   Address = u.Address,
               })
               .OrderBy(s => s.Id)
               .ToListAsync();
        }

        public async Task<UserPurchaseInfoDto?> GetPurchaseInfoInStory(int userId, int storyId)
        {
            return await _dbSet
                .Where(u => u.UserId == userId)
                .Select(u => new UserPurchaseInfoDto
                {
                    UserId = u.UserId,
                    OwnedStoryIds = u.StoriesNavigation.Select(s => s.StoryId).ToList(),
                    OwnedChapterIds = u.Chapters.Where(ch => ch.StoryId == storyId).Select(ch => ch.ChapterId).ToList(),
                    Wallet = u.Wallets.Select(w =>
                        new UserWalletDto
                        {
                            WalletId = w.WalletId,
                            UserId = w.UserId,
                            Fund = w.Fund,
                            Refund = w.Refund
                    }).FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserDto?> GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                .Select(u => new UserDto
                {
                    Id = u.UserId,
                    Email = u.Email,
                    Username = u.Username,
                    Password = u.Password,
                    Role = u.Role.RoleName,
                    Status = u.Status,
                })
                .FirstOrDefaultAsync();
        }

        public void ResetPassword(int userId, string newHashedPassword)
        {
            var user = _dbSet.Find(userId);
            if (user != null)
            {
                user.Password = newHashedPassword;
            }
        }
    }
}
