using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryListDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public string? CreateTime { get; set; }
        public string? StoryCategories { get; set; }
        public string? StoryAuthor { get; set; }
        public int StoryChapterNumber { get; set; }
        public int StoryChapters { get; set; }
        public int StoryReads { get; set; }
        public int Volumes { get; set; }
        public int UserOwned { get; set; }
        public int? Status { get; set; }
        public int UserFollow { get; set; }
        public int UserLike { get; set; }
    }
}
