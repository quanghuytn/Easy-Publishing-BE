namespace EP.Application.Common.DTOs.RefundRequest
{
    public class RefundExportDto
    {
        public string UserFullname { get; set; }
        public string BankId { get; set; }
        public string BankAccount { get; set; }
        public string Amount { get; set; }
        public string RequestTime { get; set; }
        public string ResponseTime { get; set; }
    }
}
