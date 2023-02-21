﻿using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.Qbus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Configuration;

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

            var logger = loggerFactory.CreateLogger<Program>();

            var pud = new PowerUsageDeterminator(new HomeWizardService(homeWizard), loggerFactory.CreateLogger<PowerUsageDeterminator>());

            PowerUsageState previousPowerUsage = PowerUsageState.Unknown;

            while (cts.IsCancellationRequested == false)
            {
                var powerUsage = await pud.CalculatePowerUsageStateAsync();

                logger.LogInformation($"Power State: {powerUsage}");

                if (powerUsage != previousPowerUsage && powerUsage != PowerUsageState.Unknown)
                {
                    var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

                    if (qbusSettings == null)
                    {
                        throw new InvalidOperationException("Unable to retrieve QBus settings");
                    }

                    await ModifyQbusSolarIndicatorAsync(powerUsage, qbusSettings);
                }

                previousPowerUsage = powerUsage;

                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
            }
        }

        private static async Task ModifyQbusSolarIndicatorAsync(PowerUsageState state, QbusConfigurationSettings settings)
        {
            var eqoWebSession = await EqoWebSession.CreateSessionAsync(settings.IpAddress, settings.Port, settings.Username, settings.Password);

            var controlItems = await eqoWebSession.GetSolarIndicatorControlItems(settings);

            List<Task> tasks = new List<Task>();

            foreach (var controlItem in controlItems)
            {
                tasks.Add(eqoWebSession.SetSolarIndicatorAsync(controlItem, state));
            }

            await Task.WhenAll(tasks);
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