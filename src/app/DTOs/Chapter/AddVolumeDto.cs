namespace app.DTOs.Chapter
{
    public class AddVolumeDto
    {
        public int StoryId { get; set; }
        public string VolumeTitle { get; set; } = null!;
    }
}
