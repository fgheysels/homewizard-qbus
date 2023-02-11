using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.HomeWizard.Models;
using Microsoft.Extensions.Logging;

namespace Fg.SolarProductionAlerter
{
    internal class PowerUsageDeterminator
    {
        private readonly IHomeWizardService _homeWizard;
        private readonly ILogger<PowerUsageDeterminator> _logger;

        private Measurement? _previousMeasurement = null;

        public PowerUsageDeterminator(IHomeWizardService homeWizardService, ILogger<PowerUsageDeterminator> logger)
        {
            _homeWizard = homeWizardService;
            _logger = logger;
        }

        public async Task<PowerUsageState> CalculatePowerUsageStateAsync()
        {
            // TODO: possible to improve: if the active_power_average_w takes into account solar panel production
            //       (is a delta between import & export), we can simplify this implementation and just take that
            //       value into account.

            var currentPowerUsage = await _homeWizard.GetCurrentMeasurements();

            if (_previousMeasurement == null )
            {
                _previousMeasurement = currentPowerUsage;

                return PowerUsageState.Unknown;
            }

            var activePowerExport = currentPowerUsage.TotalPowerExportInKwh - _previousMeasurement.TotalPowerExportInKwh;
            var activePowerImport = currentPowerUsage.TotalPowerImportInKwh - _previousMeasurement.TotalPowerImportInKwh;

            _logger.LogInformation($"Power import - current: {currentPowerUsage.TotalPowerImportInKwh} - previous: {_previousMeasurement.TotalPowerImportInKwh}");
            _logger.LogInformation($"Power export - current: {currentPowerUsage.TotalPowerExportInKwh} - previous: {_previousMeasurement.TotalPowerExportInKwh}");

            _logger.LogInformation($"Active Power import: {activePowerImport} - export: {activePowerExport}");

            var powerUsageInWatt = (activePowerImport - activePowerExport) / (currentPowerUsage.Timestamp - _previousMeasurement.Timestamp).TotalHours * 1000;

            _logger.LogInformation($"Power Usage: {powerUsageInWatt} watt");

            if (powerUsageInWatt <= -3000)
            {
                return PowerUsageState.ExtremeOverProduction;
            }

            if (powerUsageInWatt <= -800)
            {
                return PowerUsageState.OverProduction;
            }

            if (powerUsageInWatt > 100)
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
