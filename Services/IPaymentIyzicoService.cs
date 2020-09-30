using Iyzipay.Model;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public interface IPaymentIyzicoService
    {
        Buyer PrepareBuyer(int customerId);
        Address PrepareAddress(Core.Domain.Common.Address address);
    }
}
