namespace EP.Application.Common.DTOs.Transaction
{
    public class TransactionDto
    {
        public long TransactionId { get; set; }
        public int Amount { get; set; }
        public string? StoryTitle { get; set; }
        public string? ChapterTitle { get; set; }
        public int FundBefore { get; set; }
        public int FundAfter { get; set; }
        public int RefundBefore { get; set; }
        public int RefundAfter { get; set; }
        public DateTime TransactionTime { get; set; }
        public bool? Status { get; set; }
        public string? Description { get; set; }
    }
}
