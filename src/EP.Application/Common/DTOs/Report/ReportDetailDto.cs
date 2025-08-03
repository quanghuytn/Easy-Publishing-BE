using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Report
{
    public class ReportDetailDto
    {
        public int ReportId { get; set; }
        public string? UserName { get; set; }
        public int? StoryId { get; set; }
        public int? CommentId { get; set; }
        public long? ChapterId { get; set; }
        public string? ReportTypeContent { get; set; }
        public string? ChapterTitle { get; set; }
        public string? Link { get; set; }
        public string? StoryTitle { get; set; }
        public string? CommentContent { get; set; }
        public string? ReportContent1 { get; set; }
        public string? ReportDate { get; set; }
        public string? Status { get; set; }
    }
}
