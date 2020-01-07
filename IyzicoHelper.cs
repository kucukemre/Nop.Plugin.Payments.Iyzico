using Armut.Iyzipay;

namespace Nop.Plugin.Payments.Iyzico
{
    public class IyzicoHelper
    {
        public static Options GetIyzicoOptions(IyzicoPaymentSettings iyzicoPaymentSettings)
        {
            var options = new Options
            {
                ApiKey = iyzicoPaymentSettings.ApiKey,
                SecretKey = iyzicoPaymentSettings.SecretKey,
                BaseUrl = iyzicoPaymentSettings.BaseUrl
            };

            return options;
        }
    }
}
