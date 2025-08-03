using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Comment
{
    public class SendCommentDto
    {
        public int? StoryId { get; set; }
        public long? ChapterId { get; set; }
        public string CommentContent { get; set; } = null!;
    }
}
