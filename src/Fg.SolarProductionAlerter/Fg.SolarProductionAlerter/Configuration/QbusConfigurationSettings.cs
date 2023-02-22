namespace Fg.SolarProductionAlerter.Configuration
{
    public class QbusConfigurationSettings
    {
        /// <summary>
        /// The IP Address of the QBus controller
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The port at which the QBus EqoWeb API is listening.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// QBus account username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// QBus account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// A comma separated list of QBus devices that must be notified on power-usage state changes.
        /// </summary>
        public string SolarIndicators { get; set; }
    }
}
