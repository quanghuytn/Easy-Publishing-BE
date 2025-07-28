using EP.Application.Common.DTOs.User;
using EP.Application.Common.Interfaces.Services;
using EP.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;   
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EP.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtSettings;

        public TokenService(IOptions<JwtSetting> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string GenerateToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Int32.Parse(_jwtSettings.Time!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
