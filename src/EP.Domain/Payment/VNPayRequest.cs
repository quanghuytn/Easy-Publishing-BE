namespace EP.Domain.Payment
{
    public class VNPayRequest
    {
        public string Version { get; private set; }
        public string TmnCode { get; private set; }
        public DateTime CreateDate { get; private set; }
        public DateTime ExpireDate { get; private set; }
        public string IpAddress { get; private set; }
        public decimal Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public string OrderType { get; private set; }
        public string OrderInfo { get; private set; }
        public string ReturnUrl { get; private set; }
        public string TxnRef { get; private set; }
        public string Locale { get; private set; } = "vn";
        public string? BankCode { get; private set; }
        public string Command { get; private set; } = "pay";

        public VNPayRequest(
            string version,
            string tmnCode,
            DateTime createDate,
            DateTime expireDate,
            string ipAddress,
            decimal amount,
            string currencyCode,
            string orderType,
            string orderInfo,
            string returnUrl,
            string txnRef,
            string? bankCode = null)
        {
            Version = version;
            TmnCode = tmnCode;
            CreateDate = createDate;
            ExpireDate = expireDate;
            IpAddress = ipAddress;
            Amount = amount;
            CurrencyCode = currencyCode;
            OrderType = orderType;
            OrderInfo = orderInfo;
            ReturnUrl = returnUrl;
            TxnRef = txnRef;
            BankCode = bankCode;
        }
    }
}
