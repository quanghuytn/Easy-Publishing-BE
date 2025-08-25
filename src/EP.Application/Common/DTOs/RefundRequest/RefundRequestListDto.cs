namespace EP.Application.Common.DTOs.RefundRequest
{
    public class RefundRequestListDto
    {
        public long RequestId { get; set; }
        public string UserFullname { get; set; }
        public int WalletId { get; set; }
        public string BankId { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
        public string RequestTime { get; set; }
        public DateTime? ResponseTime { get; set; }
    }
}
