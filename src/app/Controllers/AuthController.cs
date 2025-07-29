using app.DTOs.Auth;
using app.DTOs.User;
using app.Interface;
using app.Service;
using EP.Application.Commands.Auth;
using EP.Application.Commands.User;
using EP.Application.Queries.Auth;
using EP.Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IMediator _mediator;

        public AuthController( IConfiguration configuration, IUserRepository userRepo, IMediator mediator)
        {
            _configuration = configuration;
            _userRepo = userRepo;
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var loginResponse = await _mediator.Send(command);
            return new JsonResult(new
            {
                EC = 0,
                EM = "Đăng nhập thành công",
                DT = new
                {
                    user = loginResponse.User,
                    access_token = loginResponse.AccessToken,
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
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            if(result == 0)
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Đăng ký tài khoản thất bại, vui lòng thử lại sau!"
                });
            }

            return new JsonResult(new
            {
                EC = 0,
                EM = "Đăng ký tài khoản thành công!",
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
        public async Task<IActionResult> SendMailConfirm([FromBody] SendMailConfirmQuery query)
        {
            await _mediator.Send(query);

            return new JsonResult(new
            {
                EC = 0,
                EM = "Chúng tôi đã gửi mail đến tài khoản email đã đăng ký của bạn, vui lòng làm theo hướng dẫn để đặt lại mật khẩu",
            });
        }

        [Authorize]
        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordForm data)
        {
            string email = User.FindFirstValue(JwtRegisteredClaimNames.Email);

            await _mediator.Send(new ResetPasswordCommand
            {
                Email = email,
                Token = data.Token,
                Password = data.Password,
                ConfirmPassword = data.ConfirmPassword
            });

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
        public async Task<IActionResult> GetAccount()
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetAccountQuery{ UserId = userId};
            var result = await _mediator.Send(query);

            return Ok(result);
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
        public async Task<IActionResult> ChangeAvatar([FromForm] AvatarForm data)
        {
            string fileUploaded = "";
            try
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                using var stream = data.image.OpenReadStream();
                var command = new UpdateAvatarCommand {UserId = userId, FileName = data.image.FileName, FileStream = stream };

                fileUploaded = await _mediator.Send(command);
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

        //[HttpPost("verify_token")]
        //public IActionResult VerifyToken([FromBody] VerifyTokenForm data)
        //{
        //    string token = data.Token;
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return new JsonResult(new
        //        {
        //            EC = 1,
        //            EM = "Token không hợp lệ"
        //        });
        //    }
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var jwtToken = tokenHandler.ReadJwtToken(token);
        //    DateTime expirationDate = jwtToken.ValidTo;
        //    if (DateTime.UtcNow < expirationDate)
        //    {
        //        return new JsonResult(new
        //        {
        //            EC = 0,
        //            EM = "Token hợp lệ",
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            EC = 2,
        //            EM = "Token đã hết hạn",
        //        });
        //    }
        //}
    }
}