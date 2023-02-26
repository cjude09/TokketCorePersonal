using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models.Tokquest
{
    public class Set : BaseModel
    {
        public Set()
        {
            Id = Guid.NewGuid().ToString("n");
            PartitionKey = Id;
            CreatedTime = DateTime.Now;
            Timestamp = DateTime.Now;
        }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "set";

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        //Retrieve separately in the client in case changes are made
        [JsonProperty(PropertyName = "user_display_name")]
        public string UserDisplayName { get; set; } = "User Name";

        [JsonProperty(PropertyName = "user_photo")]
        public string UserPhoto { get; set; }

        [JsonProperty(PropertyName = "user_country")]
        public string UserCountry { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        [JsonProperty(PropertyName = "tok_type")]
        public string TokType { get; set; }

        [JsonProperty(PropertyName = "tok_type_id")]
        public string TokTypeId { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "ids")]
        public List<string> TokIds { get; set; } = new List<string>();

        /// <summary>Values: private, public</summary>
        [JsonProperty(PropertyName = "privacy")]
        public string Privacy { get; set; }

        [JsonProperty(PropertyName = "views")]
        public int Views { get; set; } = 1;

        [JsonProperty(PropertyName = "likes")]
        public int Likes { get; set; }

        [JsonProperty(PropertyName = "shares")]
        public int Shares { get; set; }

        [JsonProperty(PropertyName = "is_edited")]
        public bool IsEdited { get; set; } = false;

        //
        // Summary:
        //     If the user's account is disabled (content no longer accessible by the user,
        //     and they cannot login)
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; }

        [JsonIgnore]
        public string ColorHex { get; set; }

        //
        // Summary:
        //     Id of the tokmojis. They need to be in order from first to last used.
        [JsonProperty(PropertyName = "tokmoji_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiIds { get; set; }
        //
        // Summary:
        //     Each tokmoji needs to be connected to a valid purchase.
        [JsonProperty(PropertyName = "tokmoji_purchase_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIds { get; set; }

        #region Color
        /// <summary>Main color in hex format (if null, then automatically or randomly selected). For toks this is the tok tile color.</summary>
        [MaxLength(7)]
        [JsonProperty(PropertyName = "color_main_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorMainHex { get; set; } = "#FFFFFF";

        /// <summary>Color object.</summary>
        #endregion
    }
}
