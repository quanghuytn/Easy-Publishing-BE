using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Report
{
    public class SendReportDto
    {
        public int ReportTypeId { get; set; }
        public int? StoryId { get; set; }
        public long? ChapterId { get; set; }
        public int? CommentId { get; set; }
        public string? ReportContent { get; set; } = null!;
    }
}
