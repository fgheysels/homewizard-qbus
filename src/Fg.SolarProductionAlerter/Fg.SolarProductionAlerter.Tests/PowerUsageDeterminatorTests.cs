using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.HomeWizard.Models;
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

            homeWizardService.SetupSequence(s => s.GetCurrentMeasurements())
                             .ReturnsAsync(currentMeasurement);

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(expectedState, result);
        }

        public static IEnumerable<object[]> GenerateDeterminePowerUsageTestDataset()
        {
            yield return new object[]
            {
                new Measurement(-1500, 0.05, 1.5),

                PowerUsageState.OverProduction
            };

            yield return new object[]
            {
                new Measurement(-3000, 0.05, 3.05),

                PowerUsageState.ExtremeOverProduction
            };

            yield return new object[]
            {
                new Measurement(1695, 1695.0, 0),

                PowerUsageState.NotEnoughProduction
            };

            yield return new object[]
            {
                new Measurement(1694.709, 1694.709, 0),

                PowerUsageState.NotEnoughProduction
            };
        }
    }
}