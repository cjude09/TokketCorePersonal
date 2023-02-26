using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class TokketUserReaction : TokketUser
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "kind")]
        public string Kind = null;
    }
}
