namespace app.DTOs.Interaction
{
    public class ChapterInteractionDto
    {
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public int PurchaseChapter { get; set; }
        public int CommentChapter { get; set; }
        public int ReportChapter { get; set; }

    }
}
