using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.Qbus;
using Fg.SolarProductionAlerter.Qbus.Models;
using Microsoft.Extensions.Configuration;

namespace Fg.SolarProductionAlerter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("start");

            var configuration = BuildConfiguration();

            //var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

            //var eqoWebSession = await EqoWebSession.CreateSessionAsync(qbusSettings.IpAddress, qbusSettings.Port, qbusSettings.Username, qbusSettings.Password);

            //var controlLists = await eqoWebSession.GetControlLists();

            //var solarIndicators = GetSolarIndicatorControlItems(qbusSettings.SolarIndicators, controlLists.First());

            //foreach (var solarIndicator in solarIndicators)
            //{
            //    await eqoWebSession.SetControlItemValueAsync(solarIndicator.Channel, 1);
            //}

            var devices = await HomeWizardDeviceResolver.FindHomeWizardDevicesAsync();
            Console.WriteLine("Devices:");
            foreach (var d in devices)
            {
                Console.WriteLine(d.Name + " " + d.IPAddress);
            }

            var x = new HomeWizardService(new HomeWizardDevice("test", "192.168.1.101") ); //devices.First());

            var result = await x.GetCurrentMeasurements();

            Console.WriteLine($"Import: {result.TotalPowerImport}");
            Console.WriteLine($"Import: {result.TotalPowerExport}");
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