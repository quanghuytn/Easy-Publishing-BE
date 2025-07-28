namespace app.DTOs.Story
{
    public class StoryChapterDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public decimal StoryPrice { get; set; }
    }
}
