using app.DTOs.Author;
using app.DTOs.Category;
using app.DTOs.Chapter;

namespace app.DTOs.Shelves
{
    public class TopPriceStoryDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public List<CategoryShelfDto> StoryCategories { get; set; } = new();
        public MinimalAuthorDto StoryAuthor { get; set; }
        public DateTime StoryCreateTime { get; set; }
        public int StoryChapterNumber { get; set; }
        public ChapterShelfDto? StoryLatestChapter { get; set; }
        public decimal StoryPrice { get; set; }
        public decimal? ChaptersPrice { get; set; }
    }
}
