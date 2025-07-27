using app.DTOs.Auth;
using app.DTOs.User;
using app.Interface;
using app.Models;
using app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace app.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;
        private HashService hashService = new HashService();
        private MailService mailService = new MailService();

        public AuthController( IConfiguration configuration, IUserRepository userRepo)
        {
            _configuration = configuration;
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginForm data)
        {
            if (string.IsNullOrEmpty(data.EmailOrUsername) || string.IsNullOrEmpty(data.Password))
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Vui lòng nhập đủ thông tin yêu cầu",
                });
            }
            var user = _userRepo.getUserByUsernameOrEmail(data.EmailOrUsername);
            if (user == null)
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Thông tin đăng nhập không đúng",
                });
            };
            string password = user.Password;
            var userResponse = _userRepo.getAccountById(user.UserId);
            if (!hashService.Verify(password, data.Password))
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Thông tin đăng nhập không đúng",
                });
            }
            if (user.Status == false)
            {
                return new JsonResult(new
                {
                    EC = 3,
                    EM = "Tài khoản không khả dụng",
                });
            }
            UserDTO userDTO = new UserDTO
            {
                Id = userResponse.UserId,
                Email = userResponse.Email,
                Username = userResponse.Username,
                Role = userResponse.Role
            };
            var accessToken = CreateToken(userDTO);
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            cookieOptions.HttpOnly = true;
            Response.Cookies.Append("access_token", accessToken, cookieOptions);
            if (data.Remember)
            {
                var rememberToken = CreateRememberLoginToken(data.EmailOrUsername, data.Password);
                cookieOptions.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("remember_token", rememberToken, cookieOptions);
            }
            else
            {
                Response.Cookies.Delete("remember_token");
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đăng nhập thành công",
                DT = new
                {
                    user = userResponse,
                    access_token = accessToken,
                },
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đăng xuất thành công"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterForm data)
        {
            if (string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password) || string.IsNullOrEmpty(data.Username))
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Vui lòng nhập đủ thông tin yêu cầu",
                });
            }
            var user = _userRepo.getUserByUsernameOrEmail(data.Email);
            if (user != null)
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Email đã được đăng ký bởi tài khoản khác",
                });
            }
            user = _userRepo.getUserByUsernameOrEmail(data.Username);
            if (user != null)
            {
                return new JsonResult(new
                {
                    EC = 3,
                    EM = "Username đã được đăng ký bởi tài khoản khác",
                });
            }
            if (!data.Password.Equals(data.ConfirmPassword))
            {
                return new JsonResult(new
                {
                    EC = 4,
                    EM = "Xác nhận mật khẩu không khớp với mật khẩu đã nhập"
                });
            }
            string passwordHash = hashService.Hash(data.Password);
            try
            {
                await _userRepo.addNewUser(new User
                {
                    Email = data.Email,
                    Password = passwordHash,
                    Username = data.Username,
                    Gender = true
                });
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đăng ký tài khoản thành công",
            });
        }

        private string CreateToken(UserDTO user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTConfig:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTConfig:Issuer"],
                audience: _configuration["JWTConfig:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Int32.Parse(_configuration.GetSection("JWTConfig:Time").Value!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //private string CreateToken(UserDTO user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //        new Claim("userId", user.Id.ToString()),
        //        new Claim("email", user.Email),
        //        new Claim("username", user.Username),
        //        new Claim("Role", user.Role),
        //        }),
        //        Issuer = _configuration.GetSection("JWTConfig:Issuer").Value!,
        //        Audience = _configuration.GetSection("JWTConfig:Audience").Value!,
        //        Expires = DateTime.UtcNow.AddDays(Int32.Parse(_configuration.GetSection("JWTConfig:Time").Value!)),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTConfig:Key").Value!)),
        //            SecurityAlgorithms.HmacSha256)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    // Serialize token to string
        //    string jwt = tokenHandler.WriteToken(token);
        //    return jwt;
        //}

        private string CreateForgotPasswordToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim("email", email)
                }),
                Issuer = _configuration.GetSection("JWTConfig:Issuer").Value!,
                Audience = _configuration.GetSection("JWTConfig:Audience").Value!,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTConfig:Key").Value!)),
                    SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            string jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        private string CreateRememberLoginToken(string emailOrUsername, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim("emailOrUsername", emailOrUsername),
                new Claim("password", password)
                }),
                Issuer = _configuration.GetSection("JWTConfig:Issuer").Value!,
                Audience = _configuration.GetSection("JWTConfig:Audience").Value!,
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTConfig:Key").Value!)),
                    SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            string jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        [HttpPost("forgot_password")]
        public IActionResult SendMailConfirm([FromBody] ForgotPasswordForm data)
        {
            var user = _userRepo.getUserByUsernameOrEmail(data.Email);
            if (user == null)
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Email chưa được đăng ký",
                });
            }
            try
            {
                string token = CreateForgotPasswordToken(data.Email);
                mailService.Send(data.Email,
                        "Easy Publishing: Đặt lại mật khẩu",
                        "<b>Xin chào " + user.Username + ",</b>" +
                        "<p>Chúng tôi đã nhận được một yêu cầu đặt lại mật khẩu! </p> " +
                        "<p>Vui lòng bỏ qua mail này nếu bạn không phải người thực hiện.</p> " +
                        "<p>Nếu bạn là người thực hiện yêu cầu, vui lòng click vào đường dẫn dưới đây để đặt lại mật khẩu:</p> " +
                        "<a href =\"https://genesis-easy-publishing.vercel.app//auth/reset-password?token=" + token + "\">Đặt lại mật khẩu</a>");
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Error: " + ex.Message,
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Chúng tôi đã gửi mail đến tài khoản email đã đăng ký của bạn, vui lòng làm theo hướng dẫn để đặt lại mật khẩu",
            });
        }

        [Authorize]
        [HttpPost("reset_password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordForm data)
        {
            string email = User.FindFirstValue(JwtRegisteredClaimNames.Email);

            var user = _userRepo.getUserByUsernameOrEmail(email);
            if (user == null)
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Người dùng không tồn tại"
                });
            }
            if (!data.Password.Equals(data.ConfirmPassword))
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Xác nhận mật khẩu không khớp với mật khẩu đã nhập"
                });
            }

            try
            {
                _userRepo.resetPassword(user.UserId, hashService.Hash(data.Password));
                mailService.Send(email,
                    "Easy Publishing: Đặt lại mật khẩu",
                    "<b>Xin chào " + user.Username + ",</b>" +
                    "<p>Mật khẩu của bạn đã được đặt lại thành công!</p> " +
                    "<p>Mật khẩu mới: <b>" + data.Password + "</b></p>");
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    EC = 3,
                    EM = "Error: " + ex.Message,
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đặt lại mật khẩu thành công",
                DT = new
                {
                    email = email
                }
            });
        }

        [Authorize]                
        [HttpGet("account")]
        public IActionResult GetAccount()
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _userRepo.getAccountById(userId);
            return new JsonResult(new
            {
                EC = 0,
                EM = "Thông tin tài khoản",
                DT = new
                {
                    user = user
                },
            });


        }

        [Authorize]
        [HttpPut("update_profile")]
        public async Task<IActionResult> EditProfile([FromBody] UserProfileForm data)
        {
            string accessToken = null;
            try
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userRepo.updateUser(userId, data);

                UserDTO userDTO = new UserDTO
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Username = user.Username
                };
                accessToken = CreateToken(userDTO);
                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(1);
                cookieOptions.HttpOnly = true;
                Response.Cookies.Append("access_token", accessToken, cookieOptions);
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật hồ sơ thành công",
                DT = new
                {
                    access_token = accessToken
                }
            });
        }

        [Authorize]
        [HttpPut("update_avatar")]
        public IActionResult ChangeAvatar([FromForm] AvatarForm data)
        {
            string fileUploaded = "";
            try
            {
                
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                fileUploaded = _userRepo.updateAvatar(userId, data);
                if (fileUploaded == null)
                {
                    return new JsonResult(new
                    {
                        EC = 1,
                        EM = "File không tồn tại"
                    });
                }
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật ảnh đại diện thành công",
                DT = new
                {
                    fileUploaded = fileUploaded
                }
            });
        }

        [Authorize]
        [HttpPost("change_password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordForm data)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = _userRepo.getUserById(userId);
                if (!hashService.Verify(user.Password, data.OldPassword))
                {
                    return new JsonResult(new
                    {
                        EC = 1,
                        EM = "Mật khẩu không đúng"
                    });
                }
                if (!data.Password.Equals(data.ConfirmPassword))
                {
                    return new JsonResult(new
                    {
                        EC = 2,
                        EM = "Xác nhận mật khẩu không khớp với mật khẩu đã nhập"
                    });
                }
                _userRepo.resetPassword(userId, hashService.Hash(data.Password));
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đổi mật khẩu thành công",
            });
        }

        [HttpPost("verify_token")]
        public IActionResult VerifyToken([FromBody] VerifyTokenForm data)
        {
            string token = data.Token;
            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Token không hợp lệ"
                });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            DateTime expirationDate = jwtToken.ValidTo;
            if (DateTime.UtcNow < expirationDate)
            {
                return new JsonResult(new
                {
                    EC = 0,
                    EM = "Token hợp lệ",
                });
            }
            else
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Token đã hết hạn",
                });
            }
        }
    }
}