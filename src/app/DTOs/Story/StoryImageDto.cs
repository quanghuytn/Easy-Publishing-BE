namespace app.DTOs.Story
{
    public class StoryImageDto
    {
        public int StoryId { get; set; }
        public IFormFile image { get; set; }
    }
}
