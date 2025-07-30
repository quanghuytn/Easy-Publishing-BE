using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Author
{
    public class AuthorDto
    {
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImage { get; set; }
        public string? AuthorEmail { get; set; }
        public string? AuthorDescriptionHtml { get; set; }

        public string? AuthorDescriptionMarkdown { get; set; }
        public int AuthorStories { get; set; }
    }
}
