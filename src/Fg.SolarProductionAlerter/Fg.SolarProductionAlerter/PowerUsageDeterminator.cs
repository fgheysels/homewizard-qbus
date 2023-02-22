using Fg.SolarProductionAlerter.HomeWizard;
using Microsoft.Extensions.Logging;

namespace Fg.SolarProductionAlerter
{
    internal class PowerUsageDeterminator
    {
        private readonly IHomeWizardService _homeWizard;
        private readonly ILogger<PowerUsageDeterminator> _logger;

        public PowerUsageDeterminator(IHomeWizardService homeWizardService, ILogger<PowerUsageDeterminator> logger)
        {
            _homeWizard = homeWizardService;
            _logger = logger;
        }

        public async Task<PowerUsageState> CalculatePowerUsageStateAsync()
        {
            var currentPowerUsage = await _homeWizard.GetCurrentMeasurements();

            _logger.LogInformation($"Power Usage: {currentPowerUsage.ActivePowerInWatt} watt");

            if (currentPowerUsage.ActivePowerInWatt <= -3000)
            {
                return PowerUsageState.ExtremeOverProduction;
            }

            if (currentPowerUsage.ActivePowerInWatt <= -800)
            {
                return PowerUsageState.OverProduction;
            }

            if (currentPowerUsage.ActivePowerInWatt > 200)
            {
                return PowerUsageState.NotEnoughProduction;
            }

            return PowerUsageState.BreakEven;
        }
    }

    public enum PowerUsageState
    {
        Unknown = 0,

        NotEnoughProduction = 1,

        BreakEven = 2,

        OverProduction = 3,

        ExtremeOverProduction = 4
    }
}
