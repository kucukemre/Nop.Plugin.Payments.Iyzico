using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Iyzico.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public PaymentInfoModel()
        {
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.CardHolderName")]
        public string CardholderName { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.CardNumber")]
        public string CardNumber { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.ExpirationDate")]
        public string ExpireMonth { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Iyzico.ExpirationDate")]
        public string ExpireYear { get; set; }
        public IList<SelectListItem> ExpireMonths { get; set; }
        public IList<SelectListItem> ExpireYears { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.CardCode")]
        public string CardCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Installment")]
        public string Installment { get; set; }
    }

    public class Installment
    {
        public string DisplayName { get; set; }

        public int InstallmentNumber { get; set; }

        public string Price { get; set; }

        public string TotalPrice { get; set; }
    }
}
