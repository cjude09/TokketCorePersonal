//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>Contains user favorites for the service.</summary>
    public class TokkepediaUserFavorites : BaseModel
    {
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}userfavorites";

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

        [JsonProperty(PropertyName = "favorite_categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<TokkepediaFavorite> FavoriteCategories { get; set; } = null;

        [JsonProperty(PropertyName = "favorite_avatars", NullValueHandling = NullValueHandling.Ignore)]
        public List<TokkepediaFavorite> FavoriteAvatars { get; set; } = null;
    }
}
