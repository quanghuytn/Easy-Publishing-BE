namespace app.DTOs.Story
{
    public class GetStoryImageDto
    {
        public IFormFile image { get; set; }
        public string? previousImage { get; set; }
    }
}
