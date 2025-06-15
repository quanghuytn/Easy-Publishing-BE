namespace app.DTOs.Chapter
{
    public class AddChapterDto
    {
        public int StoryId { get; set; }
        public int VolumeId { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public string? ChapterContentMarkdown { get; set; }
        public string? ChapterContentHtml { get; set; }
        public decimal? ChapterPrice { get; set; }
    }
}
