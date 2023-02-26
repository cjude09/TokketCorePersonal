//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class TokkepediaFollow : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "follow";

        //Type of feed the user is following
        [JsonProperty(PropertyName = "feed_label")]
        public string FeedLabel { get; set; } = "user";

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        //Retrieve separately in the client in case changes are made
        [JsonProperty(PropertyName = "user_display_name")]
        public string UserDisplayName { get; set; } = "User Name";

        [JsonProperty(PropertyName = "user_photo")]
        public string UserPhoto { get; set; }
        //---

        [JsonProperty(PropertyName = "follow_id")]
        public string FollowId { get; set; }

        [JsonProperty(PropertyName = "follow_display_name")]
        public string FollowDisplayName { get; set; }

        [JsonProperty(PropertyName = "follow_photo")]
        public string FollowPhoto { get; set; }

        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; } = "";

        [JsonProperty(PropertyName = "following")]
        public bool IsFollowing { get; set; }

        [JsonProperty(PropertyName = "update_count")]
        public int UpdateCount { get; set; }
    }

    public class TokkepediaFollowing : TokkepediaFollow
    {
    }

    public class TokkepediaFollower : TokkepediaFollow
    {
    }
}
