//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>Contains user analytics for the service. Values here change frequently but are rarely needed by the user.</summary>
    public class TokkepediaUserAnalyticsToks : BaseModel
    {
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"{_serviceId}useranalyticstoks";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }

        /// <summary>"user", "subaccount"</summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>Start time.</summary>
        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? StartTime { get; set; } = null;

        /// <summary>End time.</summary>
        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EndTime { get; set; } = null;

        #region Analytics

        /// <summary>Items go here.</summary>
        [JsonProperty("items_posted_category", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserAnalyticsTok> ToksPostedCategory { get; set; } = null;

        /// <summary>Items go here.</summary>
        [JsonProperty("items_posted_toktype", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserAnalyticsTok> ToksPostedTokType { get; set; } = null;

        /// <summary>Items go here.</summary>
        [JsonProperty("items_viewed_category", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserAnalyticsTok> ToksViewedCategory { get; set; } = null;

        /// <summary>Items go here.</summary>
        [JsonProperty("items_viewed_toktype", NullValueHandling = NullValueHandling.Ignore)]
        public List<UserAnalyticsTok> ToksViewedTokType { get; set; } = null;

        #endregion
    }

    /// <summary>User analytics item. Should not have the full details of the item, just what's needed.</summary>
    public class UserAnalyticsTok : BaseModel
    {
        #region Standard info
        private const string _serviceId = "tokkepedia";

        /// <summary>Type of document.</summary>
        [JsonProperty("label")]
        public string Label { get; set; } = $"tok";

        /// <summary>Service id.</summary>
        [JsonIgnore]
        public string ServiceId
        {
            get { return _serviceId; }
        }
        #endregion

        /// <summary>User's id.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>User's photo.</summary>
        [JsonProperty("user_photo")]
        public string UserPhoto { get; set; }

        /// <summary>Primary field value. Maximum is 600 characters.</summary>
        [JsonProperty(PropertyName = "primary_text")]
        public string PrimaryFieldText { get; set; }

        /// <summary>Item's category.</summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>Main image for the tok.</summary>
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
    }
}
