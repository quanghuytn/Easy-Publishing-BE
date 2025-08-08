namespace EP.Application.Common.DTOs.Shelves
{
    public class TopAuthorRevenueDto
    {
        public TopAuthorDto? Author { get; set; }
        public decimal Revenue { get; set; }
    }
    public class TopAuthorDto
    {
        public string? AuthorFullname { get; set; }
        public string? AuthorEmail { get; set; }
        public string? AuthorImage { get; set; }
    }
}
