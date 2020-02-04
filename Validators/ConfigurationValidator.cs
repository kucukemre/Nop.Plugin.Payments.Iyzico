using FluentValidation;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Iyzico.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            //set validation rules
            RuleFor(x => x.ApiKey).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Iyzico.Admin.Fields.ApiKey.Required"));
            RuleFor(x => x.SecretKey).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Iyzico.Admin.Fields.SecretKey.Required"));
            RuleFor(x => x.BaseUrl).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl.Required"));
        }
    }
}