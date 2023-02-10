using Fg.SolarProductionAlerter.HomeWizard;
using Microsoft.Extensions.Logging;

namespace Fg.SolarProductionAlerter
{
    internal class PowerUsageDeterminator
    {
        private readonly IHomeWizardService _homeWizard;
        private readonly ILogger<PowerUsageDeterminator> _logger;

        private double _previousPowerImportValue = double.MinValue;
        private double _previousPowerExportValue = double.MinValue;

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

            if (_previousPowerExportValue == double.MinValue &&
                _previousPowerImportValue == double.MinValue)
            {
                _previousPowerExportValue = currentPowerUsage.TotalPowerExport;
                _previousPowerImportValue = currentPowerUsage.TotalPowerImport;

                return PowerUsageState.Unknown;
            }

            var activePowerExport = currentPowerUsage.TotalPowerExport - _previousPowerExportValue;
            var activePowerImport = currentPowerUsage.TotalPowerImport - _previousPowerImportValue;

            _logger.LogInformation($"Power import - current: {currentPowerUsage.TotalPowerImport} - previous: {_previousPowerImportValue}");
            _logger.LogInformation($"Power export - current: {currentPowerUsage.TotalPowerExport} - previous: {_previousPowerExportValue}");

            _logger.LogInformation($"Active Power import: {activePowerImport} - export: {activePowerExport}");

            _previousPowerExportValue = currentPowerUsage.TotalPowerExport;
            _previousPowerImportValue = currentPowerUsage.TotalPowerImport;

            var powerUsageInWatt = (activePowerImport - activePowerExport) * 1000;

            _logger.LogInformation($"Power Usage: {powerUsageInWatt} watt");

            if (powerUsageInWatt <= -3500)
            {
                return PowerUsageState.ExtremeOverProduction;
            }

            if (powerUsageInWatt < -1500)
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
