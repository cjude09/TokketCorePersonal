//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokkepediaNotificationActivity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        [JsonProperty(PropertyName = "foreign_id")]
        public string ForeignId { get; set; }

        #region User data
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; } = null;

        [JsonProperty(PropertyName = "user_display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string UserDisplayName { get; set; } = null;

        [JsonProperty(PropertyName = "user_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPhoto { get; set; } = null;

        [JsonProperty(PropertyName = "cover_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPhoto { get; set; } = null;

        [JsonProperty(PropertyName = "user_bio", NullValueHandling = NullValueHandling.Ignore)]
        public string UserBio { get; set; } = null;
        #endregion

        #region Reaction Data
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public ReactionData ReactionData { get; set; } = null;
        #endregion
    }
    
}
