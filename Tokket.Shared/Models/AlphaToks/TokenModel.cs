using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.AlphaToks
{
    public class TokenModel
    {
        [JsonProperty(PropertyName = "token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "range", NullValueHandling = NullValueHandling.Ignore)]
        public TokenRange Range { get; set; }

     
    }

    public class TokenRange {
        [JsonProperty(PropertyName = "min", NullValueHandling = NullValueHandling.Ignore)]
        public string Min { get; set; }

        [JsonProperty(PropertyName = "max", NullValueHandling = NullValueHandling.Ignore)]
        public string Max { get; set; }
    }
}
