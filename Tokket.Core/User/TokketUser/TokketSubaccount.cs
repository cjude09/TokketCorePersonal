//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    /// <summary>A Tokket subaccount.</summary>
    public class TokketSubaccount : BaseModel
    {
        /// <summary>Doc type (label): subaccount</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "subaccount";

        /// <summary>Group account id that owns the subaccount.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        /// <summary>Id of the purchase record.</summary>
        [JsonProperty(PropertyName = "purchase_id")]
        public string PurchaseId { get; set; }

        /// <summary>If the user's account is disabled (content no longer accessible by the user, and they cannot login)</summary>
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; } = null;

        #region Subaccount
        // The user's currently selected subaccount fields. These fields are retrieved through GetUserAsync. If not specified the owner's fields will be in there
        // Currently selected subaccount's id is the ID field

        /// <summary>Currently selected subaccount's display name.</summary>
        [JsonProperty(PropertyName = "subaccount_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountName { get; set; } = null;

        /// <summary>Currently selected subaccount's profile picture.</summary>
        [JsonProperty(PropertyName = "subaccount_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountPhoto { get; set; } = null;

        /// <summary>True if the currently selected subaccount is the owner.</summary>
        [JsonProperty(PropertyName = "is_subaccount_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSubaccountOwner { get; set; } = null;

        /// <summary>User defined passcode that controls read/write access. In the TokketUser class this is field is filled out during Sign Up.</summary>
        [JsonProperty(PropertyName = "subaccount_key", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountKey { get; set; } = null;

        /// <summary>Optional field that can control whether a key is required. False/null by default.</summary>
        [JsonProperty(PropertyName = "is_subaccount_key_disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSubaccountKeyDisabled { get; set; } = null;
        #endregion

        /// <summary>Color for NORMAL membership badge, not royalty</summary>
        //[JsonProperty(PropertyName = "membershipbadge_color", NullValueHandling = NullValueHandling.Ignore)]
        //public string MembershipBadgeColor { get; set; } = null;

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
    }
}
