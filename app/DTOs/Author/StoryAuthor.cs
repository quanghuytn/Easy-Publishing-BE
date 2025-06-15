namespace app.DTOs.Author
{
    public class StoryAuthor
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
