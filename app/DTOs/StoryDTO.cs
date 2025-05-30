using Microsoft.EntityFrameworkCore;
namespace app.DTOs
{
    public class StoryDTO
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string StoryImage { get; set; }
        public string StoryDescription { get; set; }
        public List<StoryCategory> StoryCategories { get; set; }
        public StoryAuthor StoryAuthor { get; set; }
        public string StoryCreateTime { get; set; }
        public double StoryPrice { get; set; }
        public int Status { get; set; }
    }

    public class StoryCategory
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class StoryAuthor
    {
        public int UserId { get; set; }
        public string UserFullname { get; set; }
    }
}
