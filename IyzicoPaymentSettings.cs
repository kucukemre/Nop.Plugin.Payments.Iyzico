using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Iyzico
{
    public class IyzicoPaymentSettings : ISettings
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string BaseUrl { get; set; }
        public string PaymentMethodDescription { get; set; }
    }
}
