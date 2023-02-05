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
        internal ControlListGroup[] Groups { get; set; }
    }

    internal class ControlListGroup
    {
        [JsonPropertyName("Nme")]
        internal string Name { get; set; }

        [JsonPropertyName("Itms")]
        internal ControlItem[] Items { get; set; }
    }

    internal class ControlItem
    {
        [JsonPropertyName("Chnl")]
        internal int Channel { get; set; }

        [JsonPropertyName("Nme")]
        internal string Name { get; set; }

        [JsonPropertyName("Val")]
        internal int[] Value { get; set; }
    }
}
