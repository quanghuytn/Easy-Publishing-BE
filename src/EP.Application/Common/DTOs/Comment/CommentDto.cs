using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public UserComment? UserComment { get; set; }
        public string CommentContent { get; set; } = null!;
        public DateTime CommentDate { get; set; }
        public bool CommentWriter { get; set; }
    }
}
