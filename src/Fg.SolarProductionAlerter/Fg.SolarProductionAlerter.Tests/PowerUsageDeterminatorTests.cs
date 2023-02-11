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

            homeWizardService.Setup(s => s.GetCurrentMeasurements()).ReturnsAsync(new Measurement());

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(PowerUsageState.Unknown, result);
        }

        [Theory]
        [MemberData(nameof(GenerateDeterminePowerUsageTestDataset))]
        public async Task CanDeterminePowerUsage(Measurement previousMeasurement, Measurement currentMeasurement, PowerUsageState expectedState)
        {
            var homeWizardService = new Mock<IHomeWizardService>();

            homeWizardService.SetupSequence(s => s.GetCurrentMeasurements())
                .ReturnsAsync(previousMeasurement)
                .ReturnsAsync(currentMeasurement);

            var sut = new PowerUsageDeterminator(homeWizardService.Object, NullLogger<PowerUsageDeterminator>.Instance);

            // We need two calls, since the first call will always return 'Unknown', as at this time, we have no clue
            // of the power-usage delta.
            _ = await sut.CalculatePowerUsageStateAsync();

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(expectedState, result);
        }

        public static IEnumerable<object[]> GenerateDeterminePowerUsageTestDataset()
        {
            yield return new object[]
            {
                new Measurement(new DateTimeOffset(2023, 02, 11, 16, 0, 0, TimeSpan.Zero),
                    0.0, 0),

                new Measurement(new DateTimeOffset(2023, 02, 11, 17, 0, 0, TimeSpan.Zero),
                    0.05, 1.5),

                PowerUsageState.OverProduction
            };

            yield return new object[]
            {
                new Measurement(new DateTimeOffset(2023, 02, 11, 16, 0, 0, TimeSpan.Zero),
                    0.0, 0),

                new Measurement(new DateTimeOffset(2023, 02, 11, 17, 0, 0, TimeSpan.Zero),
                    0.05, 3.05),

                PowerUsageState.ExtremeOverProduction
            };

            yield return new object[]
            {
                new Measurement(new DateTimeOffset(2023, 02, 11, 16, 0, 0, TimeSpan.Zero),
                    1694.0, 0),

                new Measurement(new DateTimeOffset(2023, 02, 11, 17, 0, 30, TimeSpan.Zero),
                    1695.0, 0),

                PowerUsageState.NotEnoughProduction
            };

            yield return new object[]
            {
                new Measurement(new DateTimeOffset(2023, 02, 11, 16, 16, 0, TimeSpan.Zero),
                                1694.657, 0),

                new Measurement(new DateTimeOffset(2023, 02, 11, 16, 16, 30, TimeSpan.Zero),
                                1694.709, 0),

                PowerUsageState.NotEnoughProduction
            };
        }
    }
}