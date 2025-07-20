namespace app.DTOs.Story
{
    public class StoryImageDto
    {
        public int storyId { get; set; }
        public IFormFile image { get; set; }
    }
}
