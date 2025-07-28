namespace app.DTOs.Interaction
{
    public class StoryInteractionDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public int Like { get; set; }
        public int Follow { get; set; }
        public int View { get; set; }
        public int Read { get; set; }
        public int PurchaseStory { get; set; }
        public int PurchaseChapter { get; set; }
        public int CommentStory { get; set; }
        public int CommentChapter { get; set; }
        public int ReportStory { get; set; }
        public int ReportChapter { get; set; }

    }
}
