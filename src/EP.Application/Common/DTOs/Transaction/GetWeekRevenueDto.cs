namespace EP.Application.Common.DTOs.Transaction
{
    public class GetWeekRevenueDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Data { get; set; } = new();
    }
}
