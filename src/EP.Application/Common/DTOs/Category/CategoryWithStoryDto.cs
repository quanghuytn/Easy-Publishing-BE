using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Interaction;

namespace EP.Application.Common.DTOs.Category
{
    public class CategoryWithStoryDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<StoryInCategoryShelfDto> Stories { get; set; }
    }

    public class StoryInCategoryShelfDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public MinimalInteractionDto? StoryInteraction { get; set; }
        public DateTime StoryCreateTime { get; set; }
    }
}
