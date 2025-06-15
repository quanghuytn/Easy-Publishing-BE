namespace app.DTOs.Review
{
    public class ChapterReviewDto
    {
        public long ChapterId { get; set; }
        public int StoryId { get; set; }
        public int VolumeId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public string VolumeTitle { get; set; } = null!;
        public string ChapterTitle { get; set; } = null!;
        public int VolumeNumber { get; set; }
        public long ChapterNumber { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
