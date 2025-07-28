namespace app.DTOs.Story
{
    public class UpdateStoryDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public int Status { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
