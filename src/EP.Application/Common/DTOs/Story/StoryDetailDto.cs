using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Shelves;
using EP.Domain.Models;
using System;
namespace EP.Application.Common.DTOs.Story
{
    public class StoryDetailDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public DateTime CreateTime { get; set; }
        public List<CategoryShelfDto> StoryCategories { get; set; }
        public MinimalAuthorDto StoryAuthor { get; set; }
        public int StoryChapterNumber { get; set; }
        public List<ChapterShelfDto> StoryChapters { get; set; }
        public int UserPurchaseStory { get; set; }
        public MinimalInteractionDto StoryInteraction { get; set; }
        public bool AuthorOwned { get; set; }
        public bool UserOwned { get; set; }
        public long LastReadChapter { get; set; }
        public bool UserFollow { get; set; }
        public bool UserLike { get; set; }
    }
}
