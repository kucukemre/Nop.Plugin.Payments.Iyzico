using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Armut.Iyzipay.Model;
using Armut.Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Iyzico.Controllers;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Plugin.Payments.Iyzico.Services;
using Nop.Plugin.Payments.Iyzico.Validators;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Iyzico
{
    public class IyzicoPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentIyzicoService _iyzicoService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;


        public IyzicoPaymentProcessor(ISettingService settingService, IPaymentIyzicoService IyzicoService, IyzicoPaymentSettings IyzicoPaymentSettings, IWebHelper webHelper, ILocalizationService localizationService, IOrderService orderService, ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this._settingService = settingService;
            this._iyzicoService = IyzicoService;
            this._iyzicoPaymentSettings = IyzicoPaymentSettings;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
        }

        public override void Install()
        {
            //settings
            var settings = new IyzicoPaymentSettings
            {
                ApiKey = "",
                SecretKey = "",
                BaseUrl = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            #region locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.ApiKey", "Iyzico API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.ApiKey.Hint", "Enter Iyzico API Key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.SecretKey", "Iyzico Secret Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.SecretKey.Hint", "Enter Iyzico Secret Key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl", "Iyzico Base URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl.Hint", "Enter Iyzico Base URL.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.PaymentMethodDescription", "Pay by credit / debit card using Iyzico payment service");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.CardHolderName", "Card Holder Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.CardNumber", "Card Number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ExpirationDate", "Expiration Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.CardCode", "Card Code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Installment", "Installment");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.MessageCode", "Message Code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.Message", "Message");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.Deleted", "Deleted");
            #endregion

            //locales-error-messages-on-turkish
            #region locales-error-messages-on-turkish
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.8", "IdentityNumber gönderilmesi zorunludur!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.12", "Kart numarası geçersizdir!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.15", "Cvc geçersizdir!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.17", "ExpireYear ve expireMonth geçersizdir!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.5152", "Test kredi kartları ile ödeme yapılamaz!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10005", "İşlem onaylanmadı!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10012", "Geçersiz işlem!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10051", "Kart limiti yetersiz, yetersiz bakiye!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10054", "Son kullanma tarihi hatalı!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10057", "Kart sahibi bu işlemi yapamaz!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10093", "Kartınız internetten alışverişe kapalıdır. Açtırmak için ONAY yazıp kart son 6 haneyi 3340’a gönderebilirsiniz.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10215", "Geçersiz kart numarası!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10226", "İzin verilen PIN giriş sayısı aşılmış!");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Iyzico.Installment.Wrong", "Geçersiz taksit!");
            #endregion

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<IyzicoPaymentSettings>();

            //locales
            #region locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.ApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.ApiKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.SecretKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.SecretKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.BaseUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.Fields.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.CardHolderName");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.CardNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ExpirationDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.CardCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Installment");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.MessageCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.Message");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Admin.IyzicoMessage.Deleted");
            #endregion

            //locales-error-messages-on-turkish
            #region locales-error-messages-on-turkish
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.8");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.12");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.15");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.17");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.5152");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10005");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10012");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10051");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10054");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10057");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10093");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10215");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.ErrorMessage.10226");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Iyzico.Installment.Wrong");
            #endregion
            base.Uninstall();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            try
            {
                var options = IyzicoHelper.GetIyzicoOptions(_iyzicoPaymentSettings);
                var paymentCard = new PaymentCard()
                {
                    CardHolderName = processPaymentRequest.CreditCardName,
                    CardNumber = processPaymentRequest.CreditCardNumber,
                    ExpireMonth = processPaymentRequest.CreditCardExpireMonth.ToString(),
                    ExpireYear = processPaymentRequest.CreditCardExpireYear.ToString(),
                    Cvc = processPaymentRequest.CreditCardCvv2
                };

                var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                var billingAddress = _iyzicoService.PrepareAddress(_customerService.GetCustomerById(processPaymentRequest.CustomerId).BillingAddress);
                var shippingAddress = _customerService.GetCustomerById(processPaymentRequest.CustomerId).ShippingAddress != null ? _iyzicoService.PrepareAddress(_customerService.GetCustomerById(processPaymentRequest.CustomerId).ShippingAddress) : billingAddress;

                var paymentRequest = new CreatePaymentRequest
                {
                    Price = processPaymentRequest.OrderTotal.ToString("f8", CultureInfo.InvariantCulture),
                    PaidPrice = processPaymentRequest.OrderTotal.ToString("f8", CultureInfo.InvariantCulture),
                    Currency = Currency.TRY.ToString(),
                    Installment = Convert.ToInt32(processPaymentRequest.CustomValues.GetValueOrDefault(_localizationService.GetResource("Plugins.Payments.Iyzico.Installment"))),
                    BasketId = processPaymentRequest.OrderGuid.ToString(),
                    PaymentCard = paymentCard,
                    Buyer = _iyzicoService.PrepareBuyer(processPaymentRequest.CustomerId),
                    ShippingAddress = shippingAddress,
                    BillingAddress = billingAddress,
                    BasketItems = GetItems(customer, processPaymentRequest.StoreId)
                };

                paymentRequest.PaymentGroup = PaymentGroup.LISTING.ToString();

                var payment = Payment.Create(paymentRequest, options);
                if (payment.Status != "success")
                {
                    string errorMessage = _localizationService.GetResource(String.Format("Plugins.Payments.Iyzico.ErrorMessage.{0}", payment.ErrorCode)) ?? payment.ErrorMessage;
                    result.AddError(errorMessage);
                    return result;
                }

                result.NewPaymentStatus = PaymentStatus.Pending;
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Get transaction line items
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>List of transaction items</returns>
        private List<BasketItem> GetItems(Core.Domain.Customers.Customer customer, int storeId)
        {
            var items = new List<BasketItem>();

            //get current shopping cart            
            var shoppingCart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart, storeId);

            //define function to create item
            BasketItem createItem(decimal price, int productId, string productName, string categoryName, BasketItemType itemType = BasketItemType.PHYSICAL)
            {
                return new BasketItem
                {
                    Id = productId.ToString(),
                    Name = productName,
                    Category1 = categoryName,
                    ItemType = itemType.ToString(),
                    Price = Convert.ToDecimal(price, CultureInfo.InvariantCulture).ToString("f8", CultureInfo.InvariantCulture),
                };
            }

            items.AddRange(shoppingCart.Where(shoppingCartItem => shoppingCartItem.Product != null).Select(shoppingCartItem =>
            {
                return createItem(shoppingCartItem.Product.Price * shoppingCartItem.Quantity,
                    shoppingCartItem.Product.Id,
                    shoppingCartItem.Product.Name,
                    shoppingCartItem.Product.ProductCategories?.FirstOrDefault().Category.Name);
            }));

            return items;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        public decimal GetAdditionalHandlingFee(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => 0;

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest) => new CapturePaymentResult();

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest) => new RefundPaymentResult();

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest) => new VoidPaymentResult();

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest) => new ProcessPaymentResult();

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest) => new CancelRecurringPaymentResult();

        public bool CanRePostProcessPayment(Nop.Core.Domain.Orders.Order order) => false;

        public Type GetControllerType() => typeof(PaymentIyzicoController);

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool HidePaymentMethod(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => false;

        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            //validate payment info (custom validation)
            var validationResult = new PaymentInfoValidator(_localizationService).Validate(new PaymentInfoModel
            {
                CardholderName = form[nameof(PaymentInfoModel.CardholderName)],
                CardNumber = form[nameof(PaymentInfoModel.CardNumber)],
                ExpireMonth = form[nameof(PaymentInfoModel.ExpireMonth)],
                ExpireYear = form[nameof(PaymentInfoModel.ExpireYear)],
                CardCode = form[nameof(PaymentInfoModel.CardCode)],
                Installment = form[nameof(PaymentInfoModel.Installment)]
            });
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var paymentRequest = new ProcessPaymentRequest();

            //pass custom values to payment processor
            //if (_iyzicoPaymentSettings.UseInstallment)
            var installment = form[nameof(PaymentInfoModel.Installment)];
            if (!StringValues.IsNullOrEmpty(installment) && !installment.FirstOrDefault().Equals(Guid.Empty.ToString()))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Iyzico.Installment"), installment.FirstOrDefault());

            //set card details
            paymentRequest.CreditCardName = form[nameof(PaymentInfoModel.CardholderName)];
            paymentRequest.CreditCardNumber = form[nameof(PaymentInfoModel.CardNumber)];
            paymentRequest.CreditCardExpireMonth = int.Parse(form[nameof(PaymentInfoModel.ExpireMonth)]);
            paymentRequest.CreditCardExpireYear = int.Parse(form[nameof(PaymentInfoModel.ExpireYear)]);
            paymentRequest.CreditCardCvv2 = form[nameof(PaymentInfoModel.CardCode)];

            return paymentRequest;
        }

        public string PaymentMethodDescription => _iyzicoPaymentSettings.PaymentMethodDescription;

        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/PaymentIyzico/Configure";

        public string GetPublicViewComponentName() => "PaymentIyzico";
    }
}
