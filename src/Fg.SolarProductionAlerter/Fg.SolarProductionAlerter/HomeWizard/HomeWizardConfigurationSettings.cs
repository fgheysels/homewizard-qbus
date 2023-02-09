using System.Configuration;

namespace Fg.SolarProductionAlerter.HomeWizard
{
    public class HomeWizardConfigurationSettings
    {
        [ConfigurationProperty("P1HostName")]
        public string P1HostName { get; set; }
    }
}
