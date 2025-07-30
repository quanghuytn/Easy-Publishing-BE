using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Author
{
    public class StoryRelateAuthorDto
    {
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImage { get; set; }
        public int AuthorStories { get; set; }
        public int Like { get; set; }
        public int Read { get; set; }
        public StoryAuthorDto? AuthorNewestStory { get; set; }

    }
}
