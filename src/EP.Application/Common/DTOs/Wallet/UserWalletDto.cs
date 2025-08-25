namespace EP.Application.Common.DTOs.Wallet
{
    public class UserWalletDto
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public decimal Fund { get; set; }
        public decimal Refund { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal AmountTopUp { get; set; }
        public decimal AmountWithdrawn { get; set; }
    }
}
