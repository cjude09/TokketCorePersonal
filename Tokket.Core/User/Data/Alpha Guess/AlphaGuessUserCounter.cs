using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    /// <summary>Contains user counts for the service. Values here change frequently and are needed by the user.</summary>
    public class AlphaGuessUserCounter : BaseModel
    {
        private const string _serviceId = "alphaguess";

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

        //[JsonProperty("room_purchased_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        //public bool? IsRoomPurchasedAlphaGuess { get; set; } = null;

        #region General counts
        /// <summary>Total number of eliminators owned.</summary>
        [JsonProperty("eliminators_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsAlphaGuess { get; set; } = null;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_alphaguess_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsAlphaGuessDeleted { get; set; } = null;
        #endregion
    }
}
