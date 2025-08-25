namespace EP.Application.Common.Interfaces.Services
{
    public interface IVNPayService
    {
        string CreatePaymentRequest(
            decimal amount,
            string currency,
            string content,
            DateTime paymentDate,
            DateTime expireDate);
    }
}
