using EP.Application.Common.DTOs.User;
using EP.Domain.Models;
using System;
using System.Security.Claims;

namespace EP.Application.Common.Interfaces.Services.Common
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <returns>Generated JWT token as string</returns>
        string GenerateToken(UserDto user);
        string CreateForgotPasswordToken(string email);
        public ClaimsPrincipal DecodeToken(string token);
    }
}
