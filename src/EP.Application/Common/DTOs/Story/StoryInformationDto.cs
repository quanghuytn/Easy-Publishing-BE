using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Shelves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryInformationDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string StoryDescription { get; set; }
        public string StoryDescriptionMarkdown { get; set; }
        public string StoryDescriptionHtml { get; set; }
        public List<CategoryShelfDto> StoryCategories { get; set; }
        public string StoryImage { get; set; }
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public int? Status { get; set; }
        public bool Reviewed { get; set; }
    }
}
