using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class ReactionModel : TokkepediaReaction
    {
        [JsonProperty(PropertyName = "children_token")]
        public string ChildrenToken { get; set; }

        // Summary:
        //     Star rating. Max is 5.0, min is 1.0
        [JsonProperty(PropertyName = "star_rating", NullValueHandling = NullValueHandling.Ignore)]
        public double? StarRatingCount { get; set; }
    }
}
