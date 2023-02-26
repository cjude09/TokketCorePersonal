//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Contains user settings for the service. Values here change infrequently or have a limited range (true|false, 1-50, etc), and they are needed by the user.</summary>
    public class TokkepediaUserSettings : BaseModel
    {
        private const string _serviceId = "tokkepedia";

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

        /// <summary>"user", "subaccount"</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        #region Tokmoji
        /// <summary>Id of the user's favorite tokmoji</summary>
        [JsonProperty(PropertyName = "favorite_tokmoji", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteTokmoji { get; set; } = null;

        /// <summary>Replaces all Tokmoji on the site with :{text here}:</summary>
        [JsonProperty(PropertyName = "is_tokmoji_disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTokmojiDisabled { get; set; } = null;
        #endregion

        /// <summary>If true will show a user's PointsSymbol instead of a country/state flag on a tok tile.</summary>
        [JsonProperty("pointssymbol_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PointsSymbolEnabled { get; set; } = null;

        /// <summary>Selected Theme</summary>
        [JsonProperty(PropertyName = "theme", NullValueHandling = NullValueHandling.Ignore)]
        public string Theme { get; set; } = null;
    }
}
