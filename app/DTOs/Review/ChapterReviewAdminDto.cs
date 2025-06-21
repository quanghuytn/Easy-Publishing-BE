namespace app.DTOs.Review
{
    public class ChapterReviewAdminDto
    {
        public double Tt_key { get; set; }
        public double Tt_parent { get; set; }
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string? Title { get; set; }
        public string? CreateTime { get; set; }
    }
}
