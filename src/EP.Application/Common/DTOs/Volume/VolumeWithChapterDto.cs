using EP.Application.Common.DTOs.Chapter;

namespace EP.Application.Common.DTOs.Volume
{
    public class VolumeWithChapterDto
    {
        public int VolumeNumber { get; set; }
        public string? VolumeTitle { get; set; }
        public List<ChapterDto> VolumeChapters { get; set; }
    }
}
