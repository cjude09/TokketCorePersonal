//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Counts shared between all platforms. </summary>
    public class AllUserCounter : BaseModel
    {
        private const string _serviceId = "all";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}usercounter";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>"user", "subaccount"</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; } = null;

        [JsonProperty("coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; } = null;

        [JsonProperty("deleted_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedPoints { get; set; } = null;

        [JsonProperty("deleted_coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCoins { get; set; } = null;
    }
}
