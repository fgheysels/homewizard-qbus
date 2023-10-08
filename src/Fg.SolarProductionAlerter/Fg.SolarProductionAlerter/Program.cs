using Fg.SolarProductionAlerter.Configuration;
using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.Qbus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Drawing;

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

            var configuration = BuildConfiguration();
            var loggerFactory = CreateLoggerFactory(configuration);

            var homeWizard = await GetHomeWizardDevice(configuration, loggerFactory);

            var logger = loggerFactory.CreateLogger<Program>();

            var pud = new PowerUsageDeterminator(new HomeWizardService(homeWizard), loggerFactory.CreateLogger<PowerUsageDeterminator>());

            PowerUsageState previousPowerUsage = PowerUsageState.Unknown;

            while (cts.IsCancellationRequested == false)
            {
                try
                {
                    var thresholdSettings = configuration.GetSection("PowerUsageThresholds").Get<PowerUsageThresholdSettings>();

                    var powerUsage = await pud.CalculatePowerUsageStateAsync(thresholdSettings ?? PowerUsageThresholdSettings.Default);

                    logger.LogDebug($"Power State: {powerUsage}");

                    if (powerUsage != previousPowerUsage && powerUsage != PowerUsageState.Unknown)
                    {
                        logger.LogInformation($"Power Usage state changed from {previousPowerUsage} to {powerUsage}");

                        var qbusSettings = configuration.GetSection("Qbus").Get<QbusConfigurationSettings>();

                        if (qbusSettings == null)
                        {
                            throw new InvalidOperationException("Unable to retrieve QBus settings");
                        }

                        var qbusSession = await EqoWebSessionRegistry.GetSessionAsync(qbusSettings, logger);

                        await ModifyQbusSolarIndicatorAsync(powerUsage, qbusSession, qbusSettings);
                    }

                    previousPowerUsage = powerUsage;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An unexpected error occurred");
                }

                await Task.Delay(DetermineWaitTime(logger), cts.Token);
            }
        }

        private static async Task ModifyQbusSolarIndicatorAsync(PowerUsageState state, EqoWebSession eqoWebSession, QbusConfigurationSettings settings)
        {
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

        private static readonly TimeSpan HalfPastTen = new TimeSpan(22, 30, 0);
        private static readonly TimeSpan FiveInTheMorning = new TimeSpan(5, 0, 0);

        private static TimeSpan DetermineWaitTime(ILogger logger)
        {
            if (DateTime.Now.TimeOfDay > HalfPastTen)
            {
                var waitTime = DateTime.Now.Date.AddDays(1).Add(FiveInTheMorning) - DateTime.Now;

                logger.LogDebug($"It's evening - waiting until next morning before checking status again.  That is {waitTime} of sleep");
                
                return waitTime;
            }

            return TimeSpan.FromSeconds(30);
        }

        private static IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            return configuration;
        }

        private static ILoggerFactory CreateLoggerFactory(IConfiguration configuration)
        {
            return LoggerFactory.Create(builder =>
            {
                // Passing in 'Configuration' in itself has no effect on the logger
                // regarding the configured log-levels.  Need to explicitly pass in
                // the Logging section.
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
                    options.UseUtcTimestamp = false;
                });
            });
        }
    }
}