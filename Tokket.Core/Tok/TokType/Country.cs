//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class Country : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "country";

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty("toks")]
        public long Toks { get; set; }

        [JsonProperty("points")]
        public long Points { get; set; }

        [JsonProperty("coins")]
        public long Coins { get; set; }

        [JsonProperty("deleted_toks")]
        public long DeletedToks { get; set; }

        [JsonProperty("deleted_points")]
        public long DeletedPoints { get; set; }

        [JsonProperty("deleted_coins")]
        public long DeletedCoins { get; set; }
    }
}
