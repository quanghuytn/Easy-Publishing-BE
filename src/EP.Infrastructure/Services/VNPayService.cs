using EP.Application.Common.DTOs.Payment;
using EP.Application.Common.Interfaces.Services;
using EP.Application.Settings;
using EP.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace EP.Infrastructure.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VNPaySetting _vnPaySetting;
        private readonly IHashService _hashService;

        public VNPayService(
            IOptions<VNPaySetting> vnPaySetting,
            IHttpContextAccessor httpContextAccessor,
            IHashService hashService)
        {
            _vnPaySetting = vnPaySetting.Value;
            _httpContextAccessor = httpContextAccessor;
            _hashService = hashService;
        }

        public string CreatePaymentRequest(
            decimal amount,
            string currency,
            string content,
            DateTime paymentDate,
            DateTime expireDate)
        {
            string version = _vnPaySetting.Version;
            string tmnCode = _vnPaySetting.TmnCode;
            string hashSecret = _vnPaySetting.HashSecret;
            string paymentUrl = _vnPaySetting.PaymentUrl;
            string returnUrl = _vnPaySetting.ReturnUrl;

            var vnpayRequest = new VNPayRequest(
                version,
                tmnCode,
                paymentDate,
                expireDate,
                _httpContextAccessor.HttpContext?.Connection?.LocalIpAddress?.ToString() ?? string.Empty,
                amount,
                currency ?? string.Empty,
                "other",
                content ?? string.Empty,
                returnUrl,
                DateTime.Now.Ticks.ToString());

            return GetLink(paymentUrl, hashSecret, vnpayRequest);
        }

        private string GetLink(string baseUrl, string secretKey, VNPayRequest vnPayRequest)
        {
            var requestData = CreateRequestData(vnPayRequest);
            StringBuilder data = new StringBuilder();
            foreach (var kv in requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append($"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}&");
                }
            }

            string result = baseUrl + "?" + data.ToString();
            var secureHash = _hashService.HmacSHA512(secretKey, data.ToString().Remove(data.Length - 1, 1));
            return result += "vnp_SecureHash=" + secureHash;
        }

        private SortedList<string, string> CreateRequestData(VNPayRequest vnPayRequest)
        {
            var requestData = new SortedList<string, string>(new VNPayCompare())
            {
                { "vnp_Version", vnPayRequest.Version },
                { "vnp_Command", vnPayRequest.Command },
                { "vnp_TmnCode", vnPayRequest.TmnCode },
                { "vnp_Amount", ((int)(vnPayRequest.Amount * 100)).ToString() },
                { "vnp_CurrCode", vnPayRequest.CurrencyCode },
                { "vnp_CreateDate", vnPayRequest.CreateDate.ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", vnPayRequest.ExpireDate.ToString("yyyyMMddHHmmss") },
                { "vnp_IpAddr", vnPayRequest.IpAddress },
                { "vnp_Locale", vnPayRequest.Locale },
                { "vnp_OrderInfo", vnPayRequest.OrderInfo },
                { "vnp_OrderType", vnPayRequest.OrderType },
                { "vnp_ReturnUrl", vnPayRequest.ReturnUrl },
                { "vnp_TxnRef", vnPayRequest.TxnRef }
            };

            if (!string.IsNullOrEmpty(vnPayRequest.BankCode))
            {
                requestData.Add("vnp_BankCode", vnPayRequest.BankCode);
            }

            return requestData;
        }
    }
}
