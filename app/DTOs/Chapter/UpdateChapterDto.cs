namespace app.DTOs.Chapter
{
    public class UpdateChapterDto
    {
        public long ChapterId { get; set; }

        public string ChapterTitle { get; set; } = null!;

        public string? ChapterContentMarkdown { get; set; }

        public string? ChapterContentHtml { get; set; }

        public decimal? ChapterPrice { get; set; }
    }
}
