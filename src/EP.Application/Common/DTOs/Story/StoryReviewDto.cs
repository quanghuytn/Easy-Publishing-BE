using EP.Application.Common.DTOs.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryReviewDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public DateTime StoryCreateTime { get; set; }
        public int? StoryStatus { get; set; }
        public MinimalInteractionDto StoryInteraction { get; set; }
        public int UserPurchaseStory { get; set; }
        public int UserPurchaseChapter { get; set; }
    }
}
