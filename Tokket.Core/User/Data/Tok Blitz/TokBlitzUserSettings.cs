using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user settings for the service. Values here change infrequently or have a limited range (true|false, 1-50, etc), and they are needed by the user.</summary>
    public class TokBlitzUserSettings : BaseModel
    {
        private const string _serviceId = "tokblitz";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}usersettings";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId 
        {
            get { return _serviceId; }
        }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>True if user has purchased the room feature for the service.</summary>
        [JsonProperty("room_purchased_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRoomPurchasedTokBlitz { get; set; } = false;

        /// <summary>True if user has purchased the no ads feature for the service.</summary>
        [JsonProperty("no_ads_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoAdsTokBlitz { get; set; } = false;

        /// <summary>Total number of saved game slots owned for the service.</summary>
        [JsonProperty("saved_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SavedTokBlitz { get; set; } = 1;

        /// <summary>Total number of teams owned for the service.</summary>
        [JsonProperty("teams_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamsTokBlitz { get; set; } = 1;
    }
}