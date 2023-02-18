using System.Text.Json.Serialization;

namespace Fg.SolarProductionAlerter.HomeWizard.Models
{
    public class Measurement
    {
        public Measurement()
        {
        }

        public Measurement(double activePowerInWatt, double totalPowerImportInKwh, double totalPowerExportInKwh)
        {
            ActivePowerInWatt = activePowerInWatt;
            TotalPowerExportInKwh = totalPowerExportInKwh;
            TotalPowerImportInKwh = totalPowerImportInKwh;
        }

        [JsonPropertyName("active_power_w")]
        public double ActivePowerInWatt { get; set; }

        [JsonPropertyName("total_power_import_kwh")]
        public double TotalPowerImportInKwh { get; set; }

        [JsonPropertyName("total_power_export_kwh")]
        public double TotalPowerExportInKwh { get; set; }
    }
}
