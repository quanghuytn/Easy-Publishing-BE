using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Shelves
{
    public class ChapterShelfDto
    {
        public long ChapterId { get; set; }
        public long ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
