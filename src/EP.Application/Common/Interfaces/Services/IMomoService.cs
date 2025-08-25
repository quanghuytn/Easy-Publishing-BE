namespace EP.Application.Common.Interfaces.Services
{
    public interface IMomoService
    {
        Task<(bool Success, string PaymentUrl, string Message)> CreatePaymentLinkAsync(
            decimal amount,
            string orderInfo,
            int userId,
            string? extraData = null);
    }
}
