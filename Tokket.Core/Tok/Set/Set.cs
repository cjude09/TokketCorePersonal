using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tokket.Core
{
    public class Set : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "set";

        [JsonProperty(PropertyName = "activity_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityId { get; set; } = null;

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        //Retrieve separately in the client in case changes are made
        //[JsonProperty(PropertyName = "user_display_name")]
        //public string UserDisplayName { get; set; } = "User Name";

        //[JsonProperty(PropertyName = "user_photo")]
        //public string UserPhoto { get; set; }

        [JsonProperty(PropertyName = "user_country")]
        public string UserCountry { get; set; }

        [JsonProperty(PropertyName = "user_state", NullValueHandling = NullValueHandling.Ignore)]
        public string UserState { get; set; } = null;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        [JsonProperty(PropertyName = "tok_type")]
        public string TokType { get; set; }

        [JsonProperty(PropertyName = "tok_type_id")]
        public string TokTypeId { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; } = "";

        /// <summary>Uniquely identifies categories. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.Category"/></summary>
        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; } = "";

        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "ids")]
        public List<string> TokIds { get; set; } = new List<string>();

        /// <summary>Values: private, public</summary>
        [JsonProperty(PropertyName = "privacy")]
        public string Privacy { get; set; }

        /// <summary>If the set has been edited (adding/removing toks don't count)</summary>
        [JsonProperty(PropertyName = "is_edited")]
        public bool IsEdited { get; set; } = false;

        /// <summary>Is the item 'not safe for work'</summary>
        [JsonProperty(PropertyName = "nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NSFW { get; set; } = null;

        #region Group
        /// <summary>Only add if this content is part of a group. </summary>
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; } = null;

        /// <summary>Name of the group belong </summary>
        [JsonProperty(PropertyName = "group_name", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupName { get; set; } = null;
        #endregion

        #region Color
        /// <summary>Main color in hex format (if null, then automatically or randomly selected). For toks this is the tok tile color. Example: FFFFFF</summary>
        [MaxLength(7)]
        [JsonProperty(PropertyName = "color_main_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorMainHex { get; set; } = null;
        #endregion

        #region Tokmoji
        /// <summary>Id of the tokmojis. They need to be in order from first to last used.</summary>
        [JsonProperty(PropertyName = "tokmoji_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiIds { get; set; } = null;

        /// <summary>Each tokmoji needs to be connected to a valid purchase. (Primary)</summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIds { get; set; } = null;

        /// <summary>Each tokmoji needs to be connected to a valid purchase. (Secondary)</summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids_secondary", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIdsSecondary { get; set; }

        /// <summary>Tokmoji disabled if null or false. Tokmoji can only show if this field is true, otherwise any tokmoji are not shown.</summary>
        [JsonProperty(PropertyName = "tokmoji_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TokmojiEnabled { get; set; } = null;
        #endregion

        #region User
        /// <summary>If the user's account is disabled (content no longer accessible by the user, and they cannot login)</summary>
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; } = null;

        /// <summary>User's display name.</summary>
        [JsonProperty(PropertyName = "user_display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string UserDisplayName { get; set; } = null;

        /// <summary>User's profile image.</summary>
        [JsonProperty(PropertyName = "user_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPhoto { get; set; } = null;

        /// <summary>User's header image.</summary>
        [JsonProperty(PropertyName = "cover_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPhoto { get; set; } = null;

        /// <summary>User's biography.</summary>
        [MaxLength(160)]
        [JsonProperty(PropertyName = "user_bio", NullValueHandling = NullValueHandling.Ignore)]
        public string UserBio { get; set; } = null;

        /// <summary>User's website.</summary>
        [MaxLength(100)]
        [DataType(DataType.Url)]
        [JsonProperty(PropertyName = "user_website", NullValueHandling = NullValueHandling.Ignore)]
        public string UserWebsite { get; set; } = null;

        /// <summary>User's account type (account_type). Can only be "individual" or "group"</summary>
        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; } = "individual";
        #endregion

        #region Titles
        /// <summary>Id of the user's currently selected title. Not case sensitive and max of 25 characters</summary>
        [JsonProperty(PropertyName = "title_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleId { get; set; } = null;

        /// <summary>Display of the user's currently selected title. Case sensitive and max of 25 characters</summary>
        [JsonProperty(PropertyName = "title_display", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleDisplay { get; set; } = null;

        /// <summary>True if the user's currently selected title is unique.</summary>
        [JsonProperty(PropertyName = "title_unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleUnique { get; set; } = null;

        /// <summary>True if the user's currently selected title is enabled.</summary>
        [JsonProperty(PropertyName = "title_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleEnabled { get; set; } = null;
        #endregion

        #region Subaccount
        // The user's currently selected subaccount fields. These fields are retrieved through GetUserAsync. If not specified the owner's fields will be in there

        /// <summary>User's account type (group_account_type). Can only be "family" or "organization"</summary>
        [JsonProperty(PropertyName = "group_account_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupAccountType { get; set; } = null;

        /// <summary>Currently selected subaccount's id.</summary>
        [JsonProperty(PropertyName = "subaccount_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountId { get; set; } = null;

        /// <summary>Currently selected subaccount's display name.</summary>
        [JsonProperty(PropertyName = "subaccount_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountName { get; set; } = null;

        /// <summary>Currently selected subaccount's profile picture.</summary>
        [JsonProperty(PropertyName = "subaccount_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountPhoto { get; set; } = null;

        /// <summary>True if the currently selected subaccount is the owner.</summary>
        [JsonProperty(PropertyName = "subaccount_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SubaccountOwner { get; set; } = null;
        #endregion

        #region Categorical (internal or potentially used in the future)
        /// <summary>Id of the collection this item is stored in. Collections are currently for internal/corporate/curated items such as quote of the hour.</summary>
        [JsonProperty(PropertyName = "collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; } = null;

        /// <summary>Tags are currently for internal/corporate/curated items.</summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; } = null;
        #endregion

        [JsonProperty(PropertyName = "reference_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceId { get; set; } = null;

        #region Local Declaration
        //This is used for local declaration when this model is selected/check based on the UI design
        public bool isCheck { get; set; } = false;
        #endregion
    }

    public class GameSet : Set
    {
        [JsonProperty(PropertyName = "toks")]
        public List<Tok> Toks { get; set; }

        /// <summary>Game names: "tokboom", "tokblast"</summary>
        [JsonProperty(PropertyName = "game_name")]
        public string GameName { get; set; }

        [JsonIgnore]
        private List<string> tokIds { get; set; } = null;

        [JsonProperty(PropertyName = "ids", NullValueHandling = NullValueHandling.Ignore)]
        public new List<string> TokIds
        {
            get
            {
                return null;
            }
            set
            {
                tokIds = null;
            }
        }
    }
}
