namespace EP.Application.Common.Interfaces.Services.Payment
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
