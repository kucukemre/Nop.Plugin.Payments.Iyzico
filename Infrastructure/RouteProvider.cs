using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Iyzico.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(IyzicoDefaults.ConfigurationRouteName, "Plugins/Iyzico/Configure",
                new { controller = "PaymentIyzico", action = "Configure", area = AreaNames.Admin });

            endpointRouteBuilder.MapControllerRoute(IyzicoDefaults.GetInstallmentRouteName, "Plugins/Iyzico/GetInstallment",
                new { controller = "PaymentIyzico", action = "GetInstallment" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1;

        #endregion
    }
}
