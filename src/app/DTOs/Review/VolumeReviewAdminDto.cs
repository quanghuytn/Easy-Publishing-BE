namespace app.DTOs.Review
{
    public class VolumeReviewAdminDto
    {
        public double Tt_key { get; set; }
        public double Tt_parent { get; set; }
        public int VolumeId { get; set; }
        public int VolumeNumber { get; set; }
        public string Title { get; set; }
        public string CreateTime { get; set; }
        public List<ChapterReviewAdminDto> Chapters { get; set; }
    }
}
