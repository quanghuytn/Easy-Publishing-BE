namespace app.DTOs.Review
{
    public class ChapterVolumeReviewDto
    {
        public long ChapterId { get; set; }
        public int? Status { get; set; }
        public long ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public decimal? ChapterPrice { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
