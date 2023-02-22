using Microsoft.Extensions.Configuration;

namespace Fg.SolarProductionAlerter.Configuration
{
    public class PowerUsageThresholdSettings
    {
        [ConfigurationKeyName("ExtremeOverProduction")]
        public double ExtremeOverProductionThreshold { get; set; }

        [ConfigurationKeyName("OverProduction")]
        public double OverProductionThreshold { get; set; }

        [ConfigurationKeyName("NotEnoughProduction")]
        public double NotEnoughProductionThreshold { get; set; }

        public static readonly PowerUsageThresholdSettings Default = new PowerUsageThresholdSettings
        {
            NotEnoughProductionThreshold = 200,
            OverProductionThreshold = -800,
            ExtremeOverProductionThreshold = -2500
        };
    }
}
