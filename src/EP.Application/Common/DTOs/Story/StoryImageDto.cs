using Microsoft.AspNetCore.Http;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryImageDto
    {
        public int StoryId { get; set; }
        public IFormFile image { get; set; }
    }
}
