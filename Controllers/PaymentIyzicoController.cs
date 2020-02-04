using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Armut.Iyzipay.Model;
using Armut.Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Plugin.Payments.Iyzico.Validators;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Iyzico.Controllers
{
    public class PaymentIyzicoController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly TaxSettings _taxSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        public PaymentIyzicoController(ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            INotificationService notificationService,
            IyzicoPaymentSettings iyzicoPaymentSettings, IWorkContext workContext, ICustomerService customerService, IShoppingCartService shoppingCartService, TaxSettings taxSettings, ICurrencyService currencyService, IPriceFormatter priceFormatter, IPriceCalculationService priceCalculationService, IOrderTotalCalculationService orderTotalCalculationService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _iyzicoPaymentSettings = iyzicoPaymentSettings;
            _workContext = workContext;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _taxSettings = taxSettings;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _priceCalculationService = priceCalculationService;
            _orderTotalCalculationService = orderTotalCalculationService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
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
        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //validate configuration (custom validation)
            var validationResult = new ConfigurationValidator(_localizationService).Validate(model);
            if (!validationResult.IsValid)
                return Configure();

            _iyzicoPaymentSettings.ApiKey = model.ApiKey;
            _iyzicoPaymentSettings.SecretKey = model.SecretKey;
            _iyzicoPaymentSettings.BaseUrl = model.BaseUrl;
            _iyzicoPaymentSettings.PaymentMethodDescription = model.PaymentMethodDescription;

            _settingService.SaveSetting(_iyzicoPaymentSettings);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return View(@"~/Plugins/Payments.Iyzico/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult GetInstallment(string binNumber)
        {
            if (String.IsNullOrEmpty(binNumber))
                return Json(String.Empty);

            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            var shoppingCart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart);
            var shoppingCartTotal = _orderTotalCalculationService.GetShoppingCartTotal(shoppingCart, out var orderDiscountAmount, out var orderAppliedDiscounts, out var appliedGiftCards, out var redeemedRewardPoints, out var redeemedRewardPointsAmount);

            var options = IyzicoHelper.GetIyzicoOptions(_iyzicoPaymentSettings);
            var retrieveInstallmentInfoRequest = new RetrieveInstallmentInfoRequest()
            {
                BinNumber = binNumber.ToString(),
                Locale = Locale.TR.ToString(),
                Price = _priceCalculationService.RoundPrice(shoppingCartTotal.Value).ToString("f8", CultureInfo.InvariantCulture),
                ConversationId = string.Empty
            };
            var installmentInfo = InstallmentInfo.Retrieve(retrieveInstallmentInfoRequest, options);

            var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
            var subtotal = decimal.Zero;
            var list = new List<Installment>();

            if (installmentInfo.Status == "success" && installmentInfo.InstallmentDetails.Count > 0)
            {
                foreach (var installmentDetail in installmentInfo.InstallmentDetails.FirstOrDefault().InstallmentPrices)
                {
                    var installment = new Installment();

                    installment.DisplayName = _localizationService.GetResource("Plugins.Payments.Iyzico.Installment" + installmentDetail.InstallmentNumber);
                    installment.InstallmentNumber = installmentDetail.InstallmentNumber ?? 0;
                    decimal.TryParse(installmentDetail.Price.Replace(".", ","), out decimal price);
                    installment.Price = _priceFormatter.FormatPrice(price, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                    decimal.TryParse(installmentDetail.TotalPrice.Replace(".",","), out decimal totalPrice);
                    installment.TotalPrice = _priceFormatter.FormatPrice(totalPrice, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                    list.Add(installment);
                }
            }
            else
            {
                subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotal ?? 0, _workContext.WorkingCurrency);

                list.Add(new Installment()
                {
                    DisplayName = _localizationService.GetResource("Plugins.Payments.Iyzico.Installment1"),
                    InstallmentNumber = 1,
                    Price = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax),
                    TotalPrice = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax)
                });
            }

            return Json(list);
        }
    }
}