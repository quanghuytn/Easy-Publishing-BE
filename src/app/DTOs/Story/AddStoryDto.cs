namespace app.DTOs.Story
{
    public class AddStoryDto
    {
        public string StoryTitle { get; set; } = null!;
        public int AuthorId { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public string? StoryImage { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
