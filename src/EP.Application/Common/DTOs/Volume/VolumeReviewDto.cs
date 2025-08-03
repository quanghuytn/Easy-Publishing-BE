using EP.Application.Common.DTOs.Chapter;

namespace EP.Application.Common.DTOs.Volume
{
    public class VolumeReviewDto
    {
        public int VolumeId { get; set; }
        public int VolumeNumber { get; set; }
        public int StoryId { get; set; }
        public string VolumeTitle { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public List<ChapterVolumeReviewDto> Chapters { get; set; }
    }
}
