using EP.Application.Common.DTOs.User;
using EP.Domain.Models;
using System;

namespace EP.Application.Common.Interfaces.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <returns>Generated JWT token as string</returns>
        string GenerateToken(UserDto user);
    }
}
