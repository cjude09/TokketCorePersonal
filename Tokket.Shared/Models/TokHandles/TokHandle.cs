using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    /// <summary>For tok handles, .</summary>
    public class TokHandle : Tokket.Core.BaseModel
    {
        //Note that Purchase

        /// <summary>Type of item. Tok handle</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tokhandle";

        /// <summary>Uniquely identifies the user the handle currently belongs to.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        /// <summary>Number of characters. i.e. @jack has 4 characters.</summary>
        [JsonProperty(PropertyName = "characters", NullValueHandling = NullValueHandling.Ignore)]
        public int Characters { get; set; } = 0;

        /// <summary>Price in USD. i.e. @jack has 4 characters.</summary>
        [JsonProperty(PropertyName = "price_usd", NullValueHandling = NullValueHandling.Ignore)]
        public double PriceUSD { get; set; } = 0;

        [JsonIgnore]
        public bool IsAvailable => !string.IsNullOrEmpty(UserId);

        /// <summary>Uniquely identifies the user the handle currently belongs to.</summary>
        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; } = null;

        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
        [JsonProperty(PropertyName = "color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; } = null;
        [JsonProperty(PropertyName = "position", NullValueHandling = NullValueHandling.Ignore)]
        public HandlePosition Position { get; set; } = HandlePosition.None;
    }
}
