using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Iyzico.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Admin.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Admin.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl")]
        public string BaseUrl { get; set; }
        public bool BaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Admin.Fields.PaymentMethodDescription")]
        public string PaymentMethodDescription { get; set; }
    }
}
