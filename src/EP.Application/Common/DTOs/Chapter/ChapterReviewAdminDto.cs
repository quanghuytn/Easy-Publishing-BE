using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class ChapterReviewAdminDto
    {
        public double Tt_key { get; set; }
        public double Tt_parent { get; set; }
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string? Title { get; set; }
        public string? CreateTime { get; set; }
    }
}
