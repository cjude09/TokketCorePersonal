//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class Comment : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "comment";

        [JsonProperty(PropertyName = "activity_id")]
        public string ActivityId { get; set; } = "";

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        //Retrieve separately in the client in case changes are made
        [JsonProperty(PropertyName = "user_display_name")]
        public string UserDisplayName { get; set; } = "User Name";

        [JsonProperty(PropertyName = "user_photo")]
        public string UserPhoto { get; set; }
        //---

        [JsonProperty(PropertyName = "user_country")]
        public string UserCountry { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string CommentText { get; set; }

        /// <summary>  
        /// Types: "comment" (normal), "accurate", "inaccurate"
        /// </summary>
        [JsonProperty(PropertyName = "comment_type")]
        public string CommentType { get; set; } = "comment";

        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "item_label")]
        public string ItemLabel { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public long Likes { get; set; }

        [JsonProperty(PropertyName = "dislikes")]
        public long Dislikes { get; set; }

        [JsonProperty(PropertyName = "is_detail_reaction")]
        public bool IsDetailReaction { get; set; } = false;

        [JsonProperty(PropertyName = "detail_num")]
        public long DetailNum { get; set; } = 0;

        //---THESE FIELDS ONLY APPLY TO COMMENT REPLIES (When a user replies to a comment)
        [JsonProperty(PropertyName = "is_reply")]
        public bool IsReply { get; set; } = false;

        [JsonProperty(PropertyName = "comment_id")]
        public string CommentId { get; set; }

        [JsonProperty(PropertyName = "replies")]
        public long Replies { get; set; }
        //---
    }
}
