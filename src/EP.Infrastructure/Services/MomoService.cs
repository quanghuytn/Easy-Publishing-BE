using Azure.Core;
using EP.Application.Common.DTOs.Payment;
using EP.Application.Common.Interfaces.Services;
using EP.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace EP.Infrastructure.Services
{
    public class MomoService : IMomoService
    {
        private readonly MomoPaySetting _momoPaySetting;
        private readonly IHashService _hashService;
        private readonly HttpClient _httpClient;

        public MomoService(IOptions<MomoPaySetting> momoPaySetting, IHashService hashService, HttpClient httpClient)
        {
            _momoPaySetting = momoPaySetting.Value;
            _hashService = hashService;
            _httpClient = httpClient;
        }
        public async Task<(bool Success, string PaymentUrl, string Message)> CreatePaymentLinkAsync(
            decimal amount, string orderInfo, int userId, string? extraData = null)
        {
            try
            {
                var partnerCode = _momoPaySetting.PartnerCode;
                var returnUrl = _momoPaySetting.ReturnUrl;
                var ipnUrl = _momoPaySetting.IpnUrl;
                var paymentUrl = _momoPaySetting.PaymentUrl;
                var accessKey = _momoPaySetting.AccessKey;
                var secretKey = _momoPaySetting.SecretKey;

                var orderId = $"{userId}_{DateTime.Now.Ticks}";

                var momoRequest = new MomoRequest(
                    partnerCode: partnerCode,
                    requestId: GenerateRequestId(),
                    amount: (long)amount,
                    orderId: orderId,
                    orderInfo: orderInfo,
                    redirectUrl: returnUrl,
                    ipnUrl: ipnUrl,
                    requestType: "captureWallet",
                    extraData: extraData ?? string.Empty
                );

                var rawSignature = momoRequest.MakeRawSignature(accessKey);
                momoRequest.Signature = _hashService.HmacSHA256(secretKey, rawSignature);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var requestData = JsonSerializer.Serialize(momoRequest, options);

                var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(paymentUrl, requestContent);


                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<MomoCreateLinkResponse>(responseContent, options);

                    if (responseData?.ResultCode == "0")
                    {
                        return (true, responseData.PayUrl, "Tạo link thanh toán Momo thành công");
                    }
                    else
                    {
                        return (false, string.Empty, responseData?.Message ?? "Lỗi không xác định từ Momo");
                    }
                }
                else
                {
                    return (false, string.Empty, $"HTTP Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo link thanh toán!", ex);
            }
        }

        private string GenerateRequestId() => $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
    }
}
