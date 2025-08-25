namespace EP.Domain.Settings
{
    public class VNPaySetting
    {
        public string Version { get; set; }
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string PaymentUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}
