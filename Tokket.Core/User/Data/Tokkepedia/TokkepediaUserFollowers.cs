//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Stores the user's follower counts. Needs a separate document to keep database costs down (no partial updates supported).</summary>
    public class TokkepediaUserFollowers : BaseModel
    {
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}userfollowers";

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

        #region Counts
        [JsonProperty(PropertyName = "followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? Followers { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedFollowers { get; set; } = null;
        #endregion
    }
}
