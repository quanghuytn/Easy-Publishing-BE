using Newtonsoft.Json;

namespace app.Service.MomoService
{
    public class MomoIPNRequest
    {
        /// <summary>
        /// Mã đối tác (Partner Code)
        /// </summary>
        [JsonProperty("partnerCode")]
        public string? PartnerCode { get; set; }

        /// <summary>
        /// Mã đơn hàng (Order ID)
        /// </summary>
        [JsonProperty("orderId")]
        public string? OrderId { get; set; }

        /// <summary>
        /// Mã yêu cầu (Request ID)
        /// </summary>
        [JsonProperty("requestId")]
        public string? RequestId { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// Thời gian thanh toán (UNIX timestamp)
        /// </summary>
        [JsonProperty("transId")]
        public long TransId { get; set; }

        /// <summary>
        /// Mã giao dịch Momo
        /// </summary>
        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        /// <summary>
        /// Thông báo từ Momo
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Thời gian thanh toán (format: yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [JsonProperty("responseTime")]
        public string? ResponseTime { get; set; }

        /// <summary>
        /// Thông tin mô tả đơn hàng
        /// </summary>
        [JsonProperty("orderInfo")]
        public string? OrderInfo { get; set; }

        /// <summary>
        /// Mã ngân hàng (nếu có)
        /// </summary>
        [JsonProperty("payType")]
        public string? PayType { get; set; }

        /// <summary>
        /// Chữ ký xác thực (HMAC SHA256)
        /// </summary>
        [JsonProperty("signature")]
        public string? Signature { get; set; }

        /// <summary>
        /// Dữ liệu bổ sung (nếu có)
        /// </summary>
        [JsonProperty("extraData")]
        public string? ExtraData { get; set; }
    }
}
