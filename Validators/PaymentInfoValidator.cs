using FluentValidation;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Iyzico.Validators
{
    /// <summary>
    /// Represents payment info model validator
    /// </summary>
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            //set validation rules
            RuleFor(x => x.CardholderName).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardholderName.Required"));
            RuleFor(x => x.CardNumber).IsCreditCard().WithMessage(localizationService.GetResource("Payment.CardNumber.Wrong"));
            RuleFor(x => x.CardCode).Matches(@"^[0-9]{3,4}$").WithMessage(localizationService.GetResource("Payment.CardCode.Wrong"));
            RuleFor(x => x.ExpireMonth).NotEmpty().WithMessage(localizationService.GetResource("Payment.ExpireMonth.Required"));
            RuleFor(x => x.ExpireYear).NotEmpty().WithMessage(localizationService.GetResource("Payment.ExpireYear.Required"));
            RuleFor(x => x.Installment).Matches(@"^[0-9]*$").WithMessage(localizationService.GetResource("Plugins.Payments.Iyzico.Installment.Wrong"));
        }
    }
}