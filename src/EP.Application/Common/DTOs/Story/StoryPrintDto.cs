using EP.Application.Common.DTOs.Volume;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryPrintDto
    {
        public string StoryTitle { get; set; }
        public string StoryImage { get; set; }
        public string StoryDescription { get; set; }
        public string StoryDescriptionHtml { get; set; }
        public string StoryDescriptionMarkdown { get; set; }
        public decimal StoryPrice { get; set; }
        public List<VolumeWithChapterDto> StoryVolumes { get; set; }
    }
}
