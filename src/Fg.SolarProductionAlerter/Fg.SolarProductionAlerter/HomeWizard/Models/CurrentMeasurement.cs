using System.Text.Json.Serialization;

namespace Fg.SolarProductionAlerter.HomeWizard.Models
{
    public class CurrentMeasurement
    {
        [JsonPropertyName("total_power_import_kwh")]
        public double TotalPowerImport { get; set; }

        [JsonPropertyName("total_power_export_kwh")]
        public double TotalPowerExport { get; set; }
    }
}
