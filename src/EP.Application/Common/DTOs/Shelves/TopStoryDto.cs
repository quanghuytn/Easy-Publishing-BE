using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Shelves
{
    public class TopStoryDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public List<CategoryShelfDto> StoryCategories { get; set; } = new();
        public MinimalAuthorDto? StoryAuthor { get; set; }
        public DateTime StoryCreateTime { get; set; }
        public int StoryChapterNumber { get; set; }
        public ChapterShelfDto? StoryLatestChapter { get; set; }
        public MinimalInteractionDto? StoryInteraction { get; set; }
        public int UserPurchaseStory { get; set; }
        public int UserPurchaseChapter { get; set; }

    }
}
