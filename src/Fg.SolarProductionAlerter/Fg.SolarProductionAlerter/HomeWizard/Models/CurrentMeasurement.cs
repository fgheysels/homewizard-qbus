using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fg.SolarProductionAlerter.HomeWizard.Models
{
    internal class CurrentMeasurement
    {
        [JsonPropertyName("total_power_import_kwh")]
        public double TotalPowerImport { get; set; }

        [JsonPropertyName("total_power_export_kwh")]
        public double TotalPowerExport { get; set; }
    }
}
