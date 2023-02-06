using Fg.SolarProductionAlerter.Qbus;
using Fg.SolarProductionAlerter.Qbus.Models;
using Microsoft.Extensions.Configuration;

namespace Fg.SolarProductionAlerter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = BuildConfiguration();

            var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

            var eqoWebSession = await EqoWebSession.CreateSessionAsync(qbusSettings.IpAddress, qbusSettings.Port, qbusSettings.Username, qbusSettings.Password);

            var controlLists = await eqoWebSession.GetControlLists();

            var solarIndicators = GetSolarIndicatorControlItems(qbusSettings.SolarIndicators, controlLists.First());

            foreach (var solarIndicator in solarIndicators)
            {
                await eqoWebSession.SetControlItemValueAsync(solarIndicator.Channel, 1);
            }
        }

        private static IEnumerable<ControlItem> GetSolarIndicatorControlItems(string solarIndicators, ControlListGroup controlListGroup)
        {
            string[] configuredSolarIndicators = solarIndicators.Split(",");

            return controlListGroup.Items
                .Where(c => configuredSolarIndicators.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
        }

        private static IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            return configuration;
        }
    }
}