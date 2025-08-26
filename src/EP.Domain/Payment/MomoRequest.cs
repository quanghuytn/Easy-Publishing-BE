namespace EP.Domain.Payment
{
    public class MomoRequest
    {
        public string PartnerCode { get; private set; }
        public string RequestId { get; private set; }
        public long Amount { get; private set; }
        public string OrderId { get; private set; }
        public string OrderInfo { get; private set; }
        public string RedirectUrl { get; private set; }
        public string IpnUrl { get; private set; }
        public string RequestType { get; private set; }
        public string ExtraData { get; private set; }
        public string Lang { get; private set; }
        public string Signature { get; set; }

        public MomoRequest(
            string partnerCode,
            string requestId,
            long amount,
            string orderId,
            string orderInfo,
            string redirectUrl,
            string ipnUrl,
            string requestType,
            string extraData = "",
            string lang = "vi")
        {
            PartnerCode = partnerCode;
            RequestId = requestId;
            Amount = amount;
            OrderId = orderId;
            OrderInfo = orderInfo;
            RedirectUrl = redirectUrl;
            IpnUrl = ipnUrl;
            RequestType = requestType;
            ExtraData = extraData;
            Lang = lang;
        }

        public string MakeRawSignature(string accessKey)
        {
            var rawHash = $"accessKey={accessKey}" +
                          $"&amount={Amount}" +
                          $"&extraData={ExtraData}" +
                          $"&ipnUrl={IpnUrl}" +
                          $"&orderId={OrderId}" +
                          $"&orderInfo={OrderInfo}" +
                          $"&partnerCode={PartnerCode}" +
                          $"&redirectUrl={RedirectUrl}" +
                          $"&requestId={RequestId}" +
                          $"&requestType={RequestType}";

            return rawHash;
        }
    }
}
