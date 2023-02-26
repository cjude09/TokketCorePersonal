using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;
namespace Tokket.Shared.Models.Chat
{
    public class TokChatReplies :BaseModel
    {
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "sender_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SenderId { get; set; }

        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string message { get; set; }

        [JsonProperty(PropertyName = "likes", NullValueHandling = NullValueHandling.Ignore)]
        public int Likes { get; set; }

        [JsonProperty(PropertyName = "label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; } = "tokchatreply";
    }
}
