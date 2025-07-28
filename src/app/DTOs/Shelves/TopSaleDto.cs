namespace app.DTOs.Shelves
{
    public class TopSaleDto
    {
        public StorySaleInforDto? Story { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StorySaleInforDto
    {
        public int StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string? StoryImage { get; set; }
        public string? AuthorName { get; set; }
    }
}
