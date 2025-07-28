using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.DTOs.User;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context)
        {
        }
        public Task AddNewUser(User user)
        {
            throw new NotImplementedException();
        }

        public AccountDto? getAccountById(int id)
        {
            var user = _dbSet.Where(u => u.UserId == id)
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
                }).FirstOrDefault();
            return user;
        }

        public async Task<UserDto?> GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return await _dbSet
                .Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                .Select(u => new UserDto
                {
                    Id = u.UserId,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role.RoleName,
                    Password = u.Password,
                    Status = u.Status,
                })
                .FirstOrDefaultAsync();
        }

        public void ResetPassword(int userId, string newHashedPassword)
        {
            throw new NotImplementedException();
        }

        public Task<string> SwitchStatus(string email)
        {
            throw new NotImplementedException();
        }
    }
}
