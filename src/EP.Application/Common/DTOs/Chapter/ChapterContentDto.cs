using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Story;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Chapter
{
    public class ChapterContentDto
    {
        public long ChapterId { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public long ChapterNumber { get; set; }
        public decimal? ChapterPrice { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Comment { get; set; }
        public int UserPurchaseChapter { get; set; }
        public long PreviousChapterNumber { get; set; }
        public long NextChapterNumber { get; set; }
        public bool Owned { get; set; }
        public bool UserLike { get; set; }
        public StoryChapterDto? Story { get; set; }
        public MinimalAuthorDto? Author { get; set; }

    }
}
