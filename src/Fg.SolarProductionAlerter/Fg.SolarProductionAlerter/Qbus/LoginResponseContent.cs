using System.Text.Json.Serialization;

namespace Fg.SolarProductionAlerter.Qbus
{
    internal class LoginResponseContent
    {
        [JsonPropertyName("rsp")]
        public bool Rsp { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
