using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class PointsSymbolModel
    {
        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "accessory";
        /// <summary>Kind of accessory: Patches</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "pointssymbol";
        /// <summary>Name to display</summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";
        /// <summary>Description of the item</summary>
        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;
        /// <summary>Image URL</summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; } = "";
        /// <summary>Points required to earn</summary>
        [JsonProperty(PropertyName = "points_required")]
        public long PointsRequired { get; set; } = 0;
        /// <summary>Degree</summary>
        [JsonProperty(PropertyName = "degree")]
        public int Degree { get; set; } = 0;
        public string Level { get; set; }
        public int index { get; set; }
        public bool IsGrayedOut { get; set; }
    }
}