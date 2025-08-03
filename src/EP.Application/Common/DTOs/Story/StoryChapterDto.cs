using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryChapterDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public decimal StoryPrice { get; set; }
    }
}
