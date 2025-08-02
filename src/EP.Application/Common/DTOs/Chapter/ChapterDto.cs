using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class ChapterDto
    {
        public long ChapterId { get; set; }
        public int StoryId { get; set; }
        public int VolumeId { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public string StoryTitle { get; set; } = null!;
        public string? ChapterContentMarkdown { get; set; }
        public string? ChapterContentHtml { get; set; }
        public long ChapterNumber { get; set; }
        public decimal? ChapterPrice { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
