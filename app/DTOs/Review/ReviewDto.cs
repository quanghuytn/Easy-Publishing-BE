using app.DTOs.Chapter;

namespace app.DTOs.Review
{
    public class ReviewDto
    {
        public bool SpellingError { get; set; }

        public bool LengthError { get; set; }

        public string? ReviewContent { get; set; }

        public bool PoliticalContentError { get; set; }

        public bool DistortHistoryError { get; set; }

        public bool SecretContentError { get; set; }

        public bool OffensiveContentError { get; set; }

        public bool UnhealthyContentError { get; set; }
        public DateTime ReviewDate { get; set; }
        public ChapterDto? Chapters { get; set;}
        public ReviewerDto? Reviewer { get; set; }

    }
}
