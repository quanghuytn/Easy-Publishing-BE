namespace EP.Application.Common.DTOs.Transaction
{
    public class AdminTransactionDto
    {
        public string Username { get; set; } = string.Empty;
        public long TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal RefundBefore { get; set; }
        public decimal RefundAfter { get; set; }
        public string TransactionTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
