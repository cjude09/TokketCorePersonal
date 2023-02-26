using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class ClassGroupModel : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "classgroup";

        /// <summary>Uniquely identifies the user who posted.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";
        /// <summary>User's display name.</summary>
        [JsonProperty(PropertyName = "user_display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string UserDisplayName { get; set; } = null;

        [JsonProperty(PropertyName = "user_country", NullValueHandling = NullValueHandling.Ignore)]
        public string UserCountry { get; set; } = null;

        /// <summary>User's profile image.</summary>
        [JsonProperty(PropertyName = "user_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPhoto { get; set; } = null;
        /// <summary>User's header image.</summary>
        [JsonProperty(PropertyName = "cover_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPhoto { get; set; } = null;

        #region Color
        /// <summary>Main color in hex format (if null, then automatically or randomly selected). For toks this is the tok tile color.</summary>
        [JsonProperty(PropertyName = "color_main_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorMainHex { get; set; } = null;
        #endregion

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
        [JsonProperty(PropertyName = "school", NullValueHandling = NullValueHandling.Ignore)]
        public string School { get; set; } = null;
        [JsonProperty(PropertyName = "members")]
        public int Members { get; set; }
        // Used when adding to list of users joined. This allows any item in -classgroupsjoined to change/access the original item (located in {userid}-classgroups
        [JsonProperty(PropertyName = "ownerpk", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerPartitionKey { get; set; } = null;
        // Used when adding to list of users joined. This allows any item in -classgroupsjoined to change/access -classgroupmembers
        [JsonProperty(PropertyName = "memberpk", NullValueHandling = NullValueHandling.Ignore)]
        public string MemberPartitionKey { get; set; } = null;
        [JsonProperty(PropertyName = "ismember")]
        public bool IsMember { get; set; }
        [JsonProperty(PropertyName = "haspendingrequest")]
        public bool HasPendingRequest { get; set; }
        //Example: pk: "userid-classgroups0", ownerpk = "userid-classgroups0", memberpk: "userid-classgroupmembers0"
        [JsonProperty(PropertyName = "schedule")]
        public string[] Schedule { get; set; }
        [JsonProperty(PropertyName = "notifications")]
        public string[] Notifications { get; set; }
        /// <summary>User's account type (group_account_type). Options: "class" / null: Class, "club": Club, "team": Team</summary>
        [JsonProperty(PropertyName = "group_kind", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupKind { get; set; } = null;
        [JsonProperty(PropertyName = "user_info", NullValueHandling = NullValueHandling.Ignore)]
        public TokketUser UserInfo { get; set; } = null;
        [JsonIgnore]
        public string ThumbnailImage => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "md-image-") : "";

        [JsonIgnore]
        public string ThumbnailImageSmall => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "sm-image-") : "";

        #region User
        /// <summary>If the user's account is disabled (content no longer accessible by the user, and they cannot login)</summary>
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; } = null;

        /// <summary>User's biography.</summary>
        [JsonProperty(PropertyName = "user_bio", NullValueHandling = NullValueHandling.Ignore)]
        public string UserBio { get; set; } = null;

        /// <summary>User's website.</summary>
        [JsonProperty(PropertyName = "user_website", NullValueHandling = NullValueHandling.Ignore)]
        public string UserWebsite { get; set; } = null;

        /// <summary>User's state abbreviation. Only required if <see cref="UserCountry"/> is United States.</summary>
        [JsonProperty(PropertyName = "user_state", NullValueHandling = NullValueHandling.Ignore)]
        public string UserState { get; set; } = null;

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

        #region Patches (Referred to as "Points Symbol")
        /// <summary>True if the user wants to use a patch instead of a flag for toks.</summary>
        [JsonProperty(PropertyName = "is_pointssymbol_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPointsSymbolEnabled { get; set; } = null;

        /// <summary>Color to display</summary>
        [JsonProperty(PropertyName = "pointssymbol_color", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolColor { get; set; } = null;

        /// <summary>Id of the symbol</summary>
        [JsonProperty(PropertyName = "pointssymbol_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolId { get; set; } = null;

        /// <summary>Name of the symbol</summary>
        [JsonProperty(PropertyName = "pointssymbol_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolName { get; set; } = null;

        /// <summary>Image to display (only fill this in the client)</summary>
        [JsonProperty(PropertyName = "pointssymbol_image", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolImage { get; set; } = null;
        #endregion

        #region Community
        [JsonProperty(PropertyName = "level0", NullValueHandling = NullValueHandling.Ignore)]
        public string Level0 { get; set; } = null;
        [JsonProperty(PropertyName = "level1", NullValueHandling = NullValueHandling.Ignore)]
        public string Level1 { get; set; } = null;
        [JsonProperty(PropertyName = "level2", NullValueHandling = NullValueHandling.Ignore)]
        public string Level2 { get; set; } = null;
        [JsonProperty(PropertyName = "level3", NullValueHandling = NullValueHandling.Ignore)]
        public string Level3 { get; set; } = null;
        [JsonProperty(PropertyName = "level4", NullValueHandling = NullValueHandling.Ignore)]
        public string Level4 { get; set; } = null;
        [JsonProperty(PropertyName = "is_community", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsCommunity { get; set; } = false;
        [JsonProperty(PropertyName = "community_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CommunityId { get; set; } = null;
        #endregion

        public string MiddleText { get; set; } = "";
        public bool isCheck { get; set; } = false;

    }
}