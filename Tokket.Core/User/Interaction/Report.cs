//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class Report : BaseModel
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "report";

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        //Can be a tok, user, etc
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "activity_id")]
        public string ActivityId { get; set; }

        [JsonProperty(PropertyName = "item_label")]
        public string ItemLabel { get; set; }

        [JsonProperty(PropertyName = "owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty(PropertyName = "category_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryId { get; set; } = null;

        [JsonProperty(PropertyName = "tok_type_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TokTypeId { get; set; } = null;

        [JsonProperty(PropertyName = "reaction_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ReactionId { get; set; } = null;

        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
    }
}
