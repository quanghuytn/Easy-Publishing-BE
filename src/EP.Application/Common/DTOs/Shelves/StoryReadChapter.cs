namespace EP.Application.Common.DTOs.Shelves
{
    public class StoryReadChapter
    {
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public DateTime? CreateTime { get; set; }
        public DateTime? ReadTime { get; set; }

    }
}
