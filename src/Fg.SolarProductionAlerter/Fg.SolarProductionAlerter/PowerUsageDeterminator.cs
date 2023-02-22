﻿using Fg.SolarProductionAlerter.Configuration;
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

        public async Task<PowerUsageState> CalculatePowerUsageStateAsync(PowerUsageThresholdSettings thresholds)
        {
            var currentPowerUsage = await _homeWizard.GetCurrentMeasurements();

            _logger.LogInformation($"Power Usage: {currentPowerUsage.ActivePowerInWatt} watt");

            if (currentPowerUsage.ActivePowerInWatt <= thresholds.ExtremeOverProductionThreshold)
            {
                return PowerUsageState.ExtremeOverProduction;
            }

            if (currentPowerUsage.ActivePowerInWatt <= thresholds.OverProductionThreshold)
            {
                return PowerUsageState.OverProduction;
            }

            if (currentPowerUsage.ActivePowerInWatt > thresholds.NotEnoughProductionThreshold)
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
