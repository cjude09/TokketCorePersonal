using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user settings for the service. Values here change infrequently or have a limited range (true|false, 1-50, etc), and they are needed by the user.</summary>
    public class AlphaGuessUserSettings : BaseModel
    {
        private const string _serviceId = "alphaguess";

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

        //[JsonProperty("room_purchased_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        //public bool? IsRoomPurchasedAlphaGuess { get; set; } = null;

        #region General counts
        /// <summary>True if user has purchased the no ads feature for the service.</summary>
        [JsonProperty("no_ads_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoAdsAlphaGuess { get; set; } = false;

        /// <summary>Total number of saved game slots owned.</summary>
        [JsonProperty("saved_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? SavedAlphaGuess { get; set; } = null;
        #endregion
    }
}
