using Fg.SolarProductionAlerter.Qbus;
using Microsoft.Extensions.Configuration;

namespace Fg.SolarProductionAlerter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = BuildConfiguration();

            var x = configuration.GetSection("Qbus");

           var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

            var eqoWebSession = await EqoWebSession.CreateSessionAsync(qbusSettings.IpAddress, qbusSettings.Port, qbusSettings.Username, qbusSettings.Password);
        }

        private  static IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            return configuration;
        }
    }
}