using System.Text.Json.Serialization;

namespace Fg.SolarProductionAlerter.Qbus.Models
{
    internal class SetControlItemValueContent
    {
        [JsonPropertyName("Chnl")] public int Channel { get; set; }

        [JsonPropertyName("Val")]
        public int[] Value { get; set; }
    }
}
