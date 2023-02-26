//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Favorite item.</summary>
    public class TokkepediaFavorite : BaseModel
    {
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}usersettings";

        /// <summary>Kind of favorite: category, avatar, etc</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; } = $"category";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [MaxLength(100)]
        [JsonRequired]
        [JsonProperty(PropertyName = "favorite_display", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteDisplay { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "favorite_id", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteId { get; set; }

        [MaxLength(200)]
        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
    }
}
