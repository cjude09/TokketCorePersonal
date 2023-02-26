using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Core
{
    public class TokkepediaUserCounterPersonalExpanded : TokkepediaUserCounterPersonal
    {
        [JsonProperty("classtoks_private", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassToksPrivate { get; set; }
        [JsonProperty("classtoks_group", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassToksGroup { get; set; }
        [JsonProperty("classtoks_public", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClassToksPublic { get; set; }

    }
}
