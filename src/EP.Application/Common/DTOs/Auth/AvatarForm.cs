using Microsoft.AspNetCore.Http;

namespace EP.Application.Common.DTOs.Auth
{
    public class AvatarForm
    {
        public IFormFile image { get; set; }
    }
}
