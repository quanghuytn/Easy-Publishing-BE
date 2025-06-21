namespace app.DTOs.Shelves
{
    public class ChapterShelfDto
    {
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
