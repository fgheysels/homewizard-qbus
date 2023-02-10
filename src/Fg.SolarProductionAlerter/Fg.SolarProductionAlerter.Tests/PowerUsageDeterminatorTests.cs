using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.HomeWizard.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Fg.SolarProductionAlerter.Tests
{
    public class PowerUsageDeterminatorTests
    {
        [Fact]
        public async Task ReturnsUnknown_OnFirstCall()
        {
            var homeWizardService = new Mock<IHomeWizardService>();

            homeWizardService.Setup(s => s.GetCurrentMeasurements()).ReturnsAsync(new CurrentMeasurement());

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(PowerUsageState.Unknown, result);
        }

        [Theory]
        [InlineData(3510, 10, PowerUsageState.ExtremeOverProduction)]
        [InlineData(3510, 1000, PowerUsageState.OverProduction)]
        [InlineData(200, 800, PowerUsageState.NotEnoughProduction)]
        public async Task CanDeterminePowerUsage(double currentPowerExport, double currentPowerImport, PowerUsageState expectedState)
        {
            var homeWizardService = new Mock<IHomeWizardService>();

            homeWizardService.SetupSequence(s => s.GetCurrentMeasurements())
                .ReturnsAsync(new CurrentMeasurement { TotalPowerExport = 0, TotalPowerImport = 0 })
                .ReturnsAsync(new CurrentMeasurement { TotalPowerExport = currentPowerExport, TotalPowerImport = currentPowerImport });

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            // We need two calls, since the first call will always return 'Unknown', as at this time, we have no clue
            // of the power-usage delta.
            _ = await sut.CalculatePowerUsageStateAsync();

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(expectedState, result);
        }
    }
}