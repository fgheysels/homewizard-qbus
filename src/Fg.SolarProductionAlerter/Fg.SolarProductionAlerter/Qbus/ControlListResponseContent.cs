using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fg.SolarProductionAlerter.Qbus
{
    internal class ControlListResponseContent
    {
        [JsonPropertyName("Groups")]
        public ControlListGroup[] Groups { get; set; }
    }

    internal class ControlListGroup
    {
        [JsonPropertyName("Nme")]
        public string Name { get; set; }

        [JsonPropertyName("Itms")]
        public ControlItem[] Items { get; set; }
    }

    internal class ControlItem
    {
        [JsonPropertyName("Chnl")]
        public int Channel { get; set; }

        [JsonPropertyName("Nme")]
        public string Name { get; set; }

        [JsonPropertyName("Val")]
        public int[] Value { get; set; }
    }
}
