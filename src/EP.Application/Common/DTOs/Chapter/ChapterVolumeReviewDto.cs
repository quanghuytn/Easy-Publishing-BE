using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class ChapterVolumeReviewDto
    {
        public long ChapterId { get; set; }
        public int? Status { get; set; }
        public long ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public decimal? ChapterPrice { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
