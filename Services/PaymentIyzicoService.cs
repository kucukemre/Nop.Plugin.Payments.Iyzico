using System;
using Armut.Iyzipay.Model;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public class PaymentIyzicoService : IPaymentIyzicoService
    {
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;

        public PaymentIyzicoService(ICustomerService customerService, IyzicoPaymentSettings iyzicoPaymentSettings, IGenericAttributeService genericAttributeService)
        {
            this._customerService = customerService;
            this._iyzicoPaymentSettings = iyzicoPaymentSettings;
            this._genericAttributeService = genericAttributeService;
        }

        public virtual Buyer PrepareBuyer(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);

            var customerName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            var customerSurName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            var customerIdentityNumber = _genericAttributeService.GetAttribute<string>(customer, "IdentityNumber");
            var customerGsmNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);

            var buyer = new Buyer
            {
                Id = customer.CustomerGuid.ToString(),
                Name = customerName,
                Surname = customerSurName,
                Email = customer.Email,
                IdentityNumber = customerIdentityNumber,
                RegistrationAddress = customer.BillingAddress.Address1,
                Ip = customer.LastIpAddress,
                City = customer.BillingAddress.City,
                Country = customer.BillingAddress.Country.Name,
                ZipCode = customer.BillingAddress.ZipPostalCode,
                GsmNumber = customerGsmNumber,
            };

            return buyer;
        }

        public virtual Address PrepareAddress(Core.Domain.Common.Address address)
        {
            return new Address
            {
                ContactName = String.Format("{0} {1}", address.FirstName, address.LastName),
                City = address.City,
                Country = address.Country?.Name,
                Description = address.Address1,
                ZipCode = address.ZipPostalCode
            };
        }
    }
}