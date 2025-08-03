using app.DTOs.Auth;
using EP.Application.Commands.Auth;
using EP.Application.Commands.User;
using EP.Application.Common.DTOs.Auth;
using EP.Application.Queries.Auth;
using EP.Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
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
            if (result == 0)
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

        //private string CreateRememberLoginToken(string emailOrUsername, string password)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //        new Claim("emailOrUsername", emailOrUsername),
        //        }),
        //        Issuer = _configuration.GetSection("JWTConfig:Issuer").Value!,
        //        Audience = _configuration.GetSection("JWTConfig:Audience").Value!,
        //        Expires = DateTime.UtcNow.AddDays(30),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTConfig:Key").Value!)),
        //            SecurityAlgorithms.HmacSha256)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    string jwt = tokenHandler.WriteToken(token);
        //    return jwt;
        //}

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

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordForm data)
        {
            var (result, email) = await _mediator.Send(new ResetPasswordCommand
            {
                Token = data.Token,
                Password = data.Password,
                ConfirmPassword = data.ConfirmPassword
            });
            if(result > 0)
            {
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

            return new JsonResult(new
            {
                EC = -1,
                EM = "Đặt lại mật khẩu thất bại!. Vui lòng thử lại sau",
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
            var query = new GetAccountQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_profile")]
        public async Task<IActionResult> EditProfile([FromBody] UserProfileForm data)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var command = new UpdateProfileCommand
                {
                    UserId = userId,
                    UserFullname = data.UserFullname,
                    Gender = data.Gender,
                    Dob = data.Dob,
                    Phone = data.Phone,
                    Address = data.Address,
                    DescriptionMarkdown = data.DescriptionMarkdown,
                    DescriptionHTML = data.DescriptionHTML
                };

                string accessToken = await _mediator.Send(command);

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
            catch (Exception ex)
            {
                throw new Exception("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!", ex);
            }
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
                var command = new UpdateAvatarCommand { UserId = userId, FileName = data.image.FileName, FileStream = stream };

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
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm data)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new ChangePasswordCommand { UserId = userId, OldPassword = data.OldPassword, Password = data.Password, ConfirmPassword = data.ConfirmPassword };
            var result = await _mediator.Send(command);
            if(result < 1)
            {
                return new JsonResult(new
                {
                    EC = 0,
                    EM = "Đổi mật khẩu thất bại. Vui lòng thử lại sau!",
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