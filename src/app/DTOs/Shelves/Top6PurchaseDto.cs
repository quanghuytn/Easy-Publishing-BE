using app.DTOs.Author;
using app.DTOs.Interaction;

namespace app.DTOs.Shelves
{
    public class Top6PurchaseDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string StoryImage { get; set; }
        public string StoryDescription { get; set; }
        public MinimalAuthorDto StoryAuthor { get; set; }
        public DateTime StoryCreateTime { get; set; }
        public int StoryChapterNumber { get; set; }
        public MinimalInteractionDto StoryInteraction { get; set; }
        public int UserCount { get; set; }
        public int UserPurchaseChapter { get; set; }
    }
}
