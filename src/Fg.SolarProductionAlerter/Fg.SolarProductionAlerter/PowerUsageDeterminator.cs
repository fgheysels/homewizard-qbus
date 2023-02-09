using Fg.SolarProductionAlerter.HomeWizard;

namespace Fg.SolarProductionAlerter
{
    internal class PowerUsageDeterminator
    {
        private readonly IHomeWizardService _homeWizard;

        private double _previousPowerImportValue = double.MinValue;
        private double _previousPowerExportValue = double.MinValue;

        public PowerUsageDeterminator(IHomeWizardService homeWizardService)
        {
            _homeWizard = homeWizardService;
        }

        public async Task<PowerUsageState> CalculatePowerUsageStateAsync()
        {
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

            _previousPowerExportValue = currentPowerUsage.TotalPowerExport;
            _previousPowerImportValue = currentPowerUsage.TotalPowerImport;

            var delta = activePowerImport - activePowerExport;

            if (delta < -3500)
            {
                return PowerUsageState.ExtremeOverProduction;
            }

            if (delta < -1500)
            {
                return PowerUsageState.OverProduction;
            }

            if (delta > 100)
            {
                return PowerUsageState.NotEnoughProduction;
            }

            return PowerUsageState.BreakEven;
        }
    }

    internal enum PowerUsageState
    {
        Unknown = 0,

        NotEnoughProduction = 1,

        BreakEven = 2,

        OverProduction = 3,

        ExtremeOverProduction = 4
    }
}
