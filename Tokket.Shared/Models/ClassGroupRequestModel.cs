using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    /// <summary> Handles both a Group requesting a user and a user requesting a group. </summary>
    public class ClassGroupRequestModel : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "classgrouprequest";
        [JsonProperty(PropertyName = "receiver_id")]
        public string ReceiverId { get; set; } = "";
        [JsonProperty(PropertyName = "receiver_label")]
        public string ReceiverLabel { get; set; } = "user"; // Receiver can only be a user. It can be the owner of the class group.
        [JsonProperty(PropertyName = "sender_id")]
        public string SenderId { get; set; } = "";
        [JsonProperty(PropertyName = "sender_displayname")]
        public string SenderDisplayName { get; set; }
        [JsonProperty(PropertyName = "sender_image")]
        public string SenderImage { get; set; }
        [JsonProperty(PropertyName = "sender_label")]
        public string SenderLabel { get; set; } = "user"; // Same with receiver
        [JsonProperty(PropertyName = "group_id")]
        public string GroupId { get; set; }  // Id of the class group to identify where this request to be put up or came from
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
        [JsonProperty(PropertyName = "school", NullValueHandling = NullValueHandling.Ignore)]
        public string School { get; set; } = null;
        [JsonProperty(PropertyName = "members")]
        public int Members {get; set;}
        /// <summary>
        ///     Message of the request
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message {get; set;}
        /// <summary>
        ///     Remarks of the request. Can be a status string e.g "Approved", "Declined"
        /// </summary>
        [JsonProperty(PropertyName = "remarks")]
        public string Remarks {get; set;}
        /// <summary>
        ///     Status of the request
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "group_pk")]
        public string GroupPartitionKey { get; set; }
    }
}