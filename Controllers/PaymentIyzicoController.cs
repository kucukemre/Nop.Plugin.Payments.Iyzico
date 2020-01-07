using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Iyzico.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PaymentIyzicoController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;

        public PaymentIyzicoController(ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            INotificationService notificationService,
            IyzicoPaymentSettings iyzicoPaymentSettings)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _iyzicoPaymentSettings = iyzicoPaymentSettings;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel()
            {
                ApiKey = _iyzicoPaymentSettings.ApiKey,
                SecretKey = _iyzicoPaymentSettings.SecretKey,
                BaseUrl = _iyzicoPaymentSettings.BaseUrl,
                PaymentMethodDescription = _iyzicoPaymentSettings.PaymentMethodDescription
            };
            return View(@"~/Plugins/Payments.Iyzico/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            _iyzicoPaymentSettings.ApiKey = model.ApiKey;
            _iyzicoPaymentSettings.SecretKey = model.SecretKey;
            _iyzicoPaymentSettings.BaseUrl = model.BaseUrl;
            _iyzicoPaymentSettings.PaymentMethodDescription = model.PaymentMethodDescription;
            _settingService.SaveSetting(_iyzicoPaymentSettings);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return View(@"~/Plugins/Payments.Iyzico/Views/Configure.cshtml", model);
        }
    }
}