using app.DTOs.Auth;
using app.DTOs.User;
using app.Interface;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly EasyPublishingContext _context;

        public UserRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task addNewUser(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await _context.Wallets.AddAsync(new Wallet
                {
                    UserId = getUserByUsernameOrEmail(user.Username).UserId,
                    Fund = 0,
                    Refund = 0
                });
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public AccountDto getAccountById(int id)
        {
            var user = _context.Users.Where(u => u.UserId == id)
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

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await _context.Users.Where(u => u.RoleId != 1)
               .Include(u => u.Wallets)
               .Include(u => u.Stories)
               .Select(u => new UserDTO
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
            return users;
        }

        public User? getUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public User? getUserByUsernameOrEmail(string usernameOrEmail)
        {
            return _context.Users.Where(u => u.Username.Equals(usernameOrEmail) || u.Email.Equals(usernameOrEmail)).FirstOrDefault();
        }

        public void resetPassword(int userId, string newHashedPassword)
        {
            var user = _context.Users.Find(userId);
            user.Password = newHashedPassword;
            _context.SaveChanges();
        }

        public async Task<string> SwitchStatus(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            string msg = "Kích hoạt tài khoản thành công!";
            try
            {
                if (user.Status == false || user.Status == null)
                {
                    user.Status = true;
                }
                else
                {
                    msg = "Khóa tài khoản thành công!";
                    user.Status = false;
                }
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return msg;
        }

        public string? updateAvatar(int userId, AvatarForm data)
        {
            string fileUploaded = "";
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (data.image.Length > 0)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/images/avatar");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var ext = Path.GetExtension(data.image.FileName);
                var name = Path.GetFileNameWithoutExtension(data.image.FileName);
                var fileName = name + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ext;
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    data.image.CopyTo(stream);
                }
                user.UserImage = fileName;
                fileUploaded = user.UserImage;
                _context.SaveChanges();
            }
            else
            {
                return null;
            }
            return fileUploaded;
        }
    }
}
