using System.Text.Json.Serialization;

namespace Fg.SolarProductionAlerter.HomeWizard.Models
{
    public class Measurement
    {

        public Measurement()
        {
        }

        public Measurement(DateTimeOffset timestamp, double totalPowerImportInKwh, double totalPowerExportInKwh)
        {
            Timestamp = timestamp;
            TotalPowerExportInKwh = totalPowerExportInKwh;
            TotalPowerImportInKwh = totalPowerImportInKwh;
        }

        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("total_power_import_kwh")]
        public double TotalPowerImportInKwh { get; set; }

        [JsonPropertyName("total_power_export_kwh")]
        public double TotalPowerExportInKwh { get; set; }
    }
}
