using Fg.HomeWizard.EnergyApi.Client;
using Fg.SolarProductionAlerter.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Fg.SolarProductionAlerter.Tests
{
    public class PowerUsageDeterminatorTests
    {
        [Theory]
        [MemberData(nameof(GenerateDeterminePowerUsageTestDataset))]
        public async Task CanDeterminePowerUsage(Measurement currentMeasurement, PowerUsageState expectedState)
        {
            var homeWizardService = new Mock<IHomeWizardService>();

            homeWizardService.SetupSequence(s => s.GetCurrentMeasurementsAsync())
                             .ReturnsAsync(currentMeasurement);

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            var thresholds = new PowerUsageThresholdSettings
            {
                NotEnoughProductionThreshold = 200,
                OverProductionThreshold = -800,
                ExtremeOverProductionThreshold = -2500
            };

            var result = await sut.CalculatePowerUsageStateAsync(thresholds);

            Assert.Equal(expectedState, result);
        }

        public static IEnumerable<object[]> GenerateDeterminePowerUsageTestDataset()
        {
            yield return new object[]
            {
                new Measurement
                {
                    ActivePowerInWatt = -1500,
                    TotalPowerImportInKwh = 0.05,
                    TotalPowerExportInKwh = 1.5
                },

                PowerUsageState.OverProduction
            };

            yield return new object[]
            {
                new Measurement
                {
                    ActivePowerInWatt = -3000,
                    TotalPowerImportInKwh = 0.05,
                    TotalPowerExportInKwh = 3.05
                },

                PowerUsageState.ExtremeOverProduction
            };

            yield return new object[]
            {
                new Measurement
                {
                    ActivePowerInWatt = 1695,
                    TotalPowerImportInKwh = 1695.0,
                    TotalPowerExportInKwh = 0
                },

                PowerUsageState.NotEnoughProduction
            };

            yield return new object[]
            {
                new Measurement
                {
                    ActivePowerInWatt = 1694.709,
                    TotalPowerImportInKwh = 1694.709,
                    TotalPowerExportInKwh = 0
                },

                PowerUsageState.NotEnoughProduction
            };
        }
    }
}