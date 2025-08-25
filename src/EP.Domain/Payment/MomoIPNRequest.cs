using System.Text.Json.Serialization;

namespace EP.Application.Common.DTOs.Payment
{
    public class MomoIPNRequest
    {
        /// <summary>
        /// Mã đối tác (Partner Code)
        /// </summary>
        [JsonPropertyName("partnerCode")]
        public string? PartnerCode { get; set; }

        /// <summary>
        /// Mã đơn hàng (Order ID)
        /// </summary>
        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        /// <summary>
        /// Mã yêu cầu (Request ID)
        /// </summary>
        [JsonPropertyName("requestId")]
        public string? RequestId { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// Thời gian thanh toán (UNIX timestamp)
        /// </summary>
        [JsonPropertyName("transId")]
        public long TransId { get; set; }

        /// <summary>
        /// Mã giao dịch Momo
        /// </summary>
        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        /// <summary>
        /// Thông báo từ Momo
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Thời gian thanh toán (format: yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [JsonPropertyName("responseTime")]
        public string? ResponseTime { get; set; }

        /// <summary>
        /// Thông tin mô tả đơn hàng
        /// </summary>
        [JsonPropertyName("orderInfo")]
        public string? OrderInfo { get; set; }

        /// <summary>
        /// Mã ngân hàng (nếu có)
        /// </summary>
        [JsonPropertyName("payType")]
        public string? PayType { get; set; }

        /// <summary>
        /// Chữ ký xác thực (HMAC SHA256)
        /// </summary>
        [JsonPropertyName("signature")]
        public string? Signature { get; set; }

        /// <summary>
        /// Dữ liệu bổ sung (nếu có)
        /// </summary>
        [JsonPropertyName("extraData")]
        public string? ExtraData { get; set; }
    }
}
