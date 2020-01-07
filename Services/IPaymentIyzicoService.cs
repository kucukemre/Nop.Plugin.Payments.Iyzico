using Armut.Iyzipay.Model;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public interface IPaymentIyzicoService
    {
        BinNumber RetrieveBinNumber(string binNumber);
        InstallmentInfo RetrieveInstallments(string binNumber, string price);

        Buyer PrepareBuyer(int customerId);
        Address PrepareAddress(Core.Domain.Common.Address address);
    }
}
