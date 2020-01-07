using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.Iyzico.Controllers;
using Nop.Plugin.Payments.Iyzico.Services;

namespace Nop.Plugin.Payments.Iyzico.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            builder.RegisterType<PaymentIyzicoController>().AsSelf();
            builder.RegisterType<PaymentIyzicoService>().As<IPaymentIyzicoService>().InstancePerDependency();
        }

        public int Order => 2;
    }
}
