using Iyzipay.Model;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using System;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public class PaymentIyzicoService : IPaymentIyzicoService
    {
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;

        public PaymentIyzicoService(ICustomerService customerService, IyzicoPaymentSettings iyzicoPaymentSettings, IGenericAttributeService genericAttributeService, IAddressService addressService, ICountryService countryService)
        {
            this._customerService = customerService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._iyzicoPaymentSettings = iyzicoPaymentSettings;
            this._genericAttributeService = genericAttributeService;
        }

        public virtual Buyer PrepareBuyer(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);

            var customerName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            var customerSurName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            var customerIdentityNumber = _genericAttributeService.GetAttribute<string>(customer, "IdentityNumber");
            if (string.IsNullOrEmpty(customerIdentityNumber))
                customerIdentityNumber = "11111111111";
            var customerGsmNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);

            var billingAddress = _addressService.GetAddressById(customer.BillingAddressId ?? 0);
            if (billingAddress == null)
                throw new NopException("Customer billing address not set");

            var country = _countryService.GetCountryById(billingAddress.CountryId ?? 0);
            if (country == null)
                throw new NopException("Billing address country not set");

            var buyer = new Buyer
            {
                Id = customer.CustomerGuid.ToString(),
                Name = customerName,
                Surname = customerSurName,
                Email = customer.Email,
                IdentityNumber = customerIdentityNumber,
                RegistrationAddress = billingAddress.Address1,
                Ip = customer.LastIpAddress,
                City = billingAddress.City,
                Country = country.Name,
                ZipCode = billingAddress.ZipPostalCode,
                GsmNumber = customerGsmNumber,
            };

            return buyer;
        }

        public virtual Address PrepareAddress(Core.Domain.Common.Address address)
        {
            var country = _countryService.GetCountryById(address.CountryId ?? 0);
            if (country == null)
                throw new NopException("Billing address country not set");

            return new Address
            {
                ContactName = String.Format("{0} {1}", address.FirstName, address.LastName),
                City = address.City,
                Country = country.Name,
                Description = address.Address1,
                ZipCode = address.ZipPostalCode
            };
        }
    }
}