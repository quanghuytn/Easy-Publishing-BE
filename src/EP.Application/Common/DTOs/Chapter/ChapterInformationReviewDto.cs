using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class ChapterInformationReviewDto
    {
        public long ChapterId { get; set; }
        public int? ChapterStatus { get; set; }
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? ChapterTitle { get; set; }
        public string? ChapterContentHtml { get; set; }
        public string? ChapterContentMarkdown { get; set; }
        public long ChapterNumber { get; set; }
        public int VolumeId { get; set; }
        public decimal? ChapterPrice { get; set; }
    }
}
