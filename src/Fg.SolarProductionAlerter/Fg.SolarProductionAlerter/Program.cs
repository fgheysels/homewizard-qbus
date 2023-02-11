using System.Configuration;
using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.Qbus;
using Fg.SolarProductionAlerter.Qbus.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fg.SolarProductionAlerter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            var loggerFactory = CreateLoggerFactory();
            var configuration = BuildConfiguration();

            var homeWizard = await GetHomeWizardDevice(configuration, loggerFactory);
            var qbusIndicators = GetConfiguredSolarIndicatorItems(configuration);

            var logger = loggerFactory.CreateLogger<Program>();

            var pud = new PowerUsageDeterminator(new HomeWizardService(homeWizard), loggerFactory.CreateLogger<PowerUsageDeterminator>());

            while (cts.IsCancellationRequested == false)
            {
                var powerUsage = await pud.CalculatePowerUsageStateAsync();

                logger.LogInformation($"Power State: {powerUsage}");

                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
            }

            //var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

            //var eqoWebSession = await EqoWebSession.CreateSessionAsync(qbusSettings.IpAddress, qbusSettings.Port, qbusSettings.Username, qbusSettings.Password);

            //var controlLists = await eqoWebSession.GetControlLists();

            //var solarIndicators = GetSolarIndicatorControlItems(qbusSettings.SolarIndicators, controlLists.First());

            //foreach (var solarIndicator in solarIndicators)
            //{
            //    await eqoWebSession.SetControlItemValueAsync(solarIndicator.Channel, 1);
            //}


            //var x = new HomeWizardService(new HomeWizardDevice("test", "192.168.1.101")); //devices.First());

            //var result = await x.GetCurrentMeasurements();

            //Console.WriteLine($"Import: {result.TotalPowerImportInKwh}");
            //Console.WriteLine($"Import: {result.TotalPowerExportInKwh}");
        }

        private static async Task<HomeWizardDevice> GetHomeWizardDevice(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var settings = configuration.GetSection("HomeWizard").Get<HomeWizardConfigurationSettings>();

            if (String.IsNullOrWhiteSpace(settings?.P1HostName))
            {
                throw new ConfigurationErrorsException("HomeWizard P1 Meter not configured.  HomeWizard__P1HostName setting not found.");
            }

            var device = await HomeWizardDeviceResolver.FindHomeWizardDeviceAsync(settings.P1HostName, loggerFactory.CreateLogger(nameof(HomeWizardDeviceResolver)));

            if (device == null)
            {
                throw new Exception("No HomeWizard device found in network with name " + settings.P1HostName);
            }

            return device;
        }

        private static IEnumerable<string> GetConfiguredSolarIndicatorItems(IConfiguration configuration)
        {
            var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

            if (String.IsNullOrWhiteSpace(qbusSettings?.SolarIndicators))
            {
                throw new ConfigurationErrorsException("No QBus alerters configured.  QBus__SolarIndicators setting not found.");
            }

            return qbusSettings.SolarIndicators.Split(",");
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

        private static ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder => builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
                options.UseUtcTimestamp = false;
            }));
        }
    }
}