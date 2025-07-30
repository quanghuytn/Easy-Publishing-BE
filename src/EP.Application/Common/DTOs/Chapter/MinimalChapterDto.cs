using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class MinimalChapterDto
    {
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public decimal? ChapterPrice { get; set; }
        public DateTime CreateTime { get; set; }
        public int Comment { get; set; }
        public int UserPurchaseChapter { get; set; }
        public int? Status { get; set; }
    }
}
