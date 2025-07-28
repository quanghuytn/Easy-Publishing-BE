using app.DTOs.Interaction;

namespace app.DTOs.Review
{
    public class StoryReviewDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public DateTime StoryCreateTime { get; set; }
        public int? StoryStatus { get; set; }
        public MinimalInteractionDto StoryInteraction { get; set; }
        public int UserPurchaseStory { get; set; }
        public int UserPurchaseChapter { get; set; }
    }
}
