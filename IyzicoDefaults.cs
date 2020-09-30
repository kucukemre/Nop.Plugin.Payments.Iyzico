namespace Nop.Plugin.Payments.Iyzico
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class IyzicoDefaults
    {
        #region Payments Iyzico

        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.Iyzico";

        #endregion

        #region Routing

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.Iyzico.Configure";

        /// <summary>
        /// Gets the get installment route name
        /// </summary>
        public static string GetInstallmentRouteName => "Plugin.Payments.Iyzico.GetInstallment";

        #endregion
    }
}
