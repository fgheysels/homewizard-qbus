using Fg.SolarProductionAlerter.HomeWizard;
using Fg.SolarProductionAlerter.HomeWizard.Models;
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

            var sut = new PowerUsageDeterminator(homeWizardService.Object);

            var result = await sut.CalculatePowerUsageStateAsync();

            Assert.Equal(PowerUsageState.Unknown, result);
        }

        //[Theory]
        //public void CanDeterminePowerUsage()
        //{

        //}
    }
}