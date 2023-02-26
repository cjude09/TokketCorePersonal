using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Core;

namespace Tokket.Shared.Models.Notification
{
    public class TokkepediaNotificationNew : BaseModel
    {
        //[JsonProperty(PropertyName = "id")]
        //public string Id { get; set; }

        //Id of the tok they reacted to
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; } = "";

        //Id of the tok they reacted to
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "";

        //Id of the user who sent it
        [JsonProperty(PropertyName = "sender_id")]
        public string SenderId { get; set; } = "";

        [JsonProperty(PropertyName = "sender_photo")]
        public string SenderPhoto { get; set; } = "";

        [JsonProperty(PropertyName = "sender_display_name")]
        public string SenderDisplayName { get; set; } = "";

        //
        // Summary:
        //     User's country id (ISO code).
        [JsonProperty(PropertyName = "user_country", NullValueHandling = NullValueHandling.Ignore)]
        public string UserCountry { get; set; }
        //
        // Summary:
        //     User's state abbreviation. Only required if Tokket.Core.TokkepediaReaction.UserCountry
        //     is United States.
        [JsonProperty(PropertyName = "user_state", NullValueHandling = NullValueHandling.Ignore)]
        public string UserState { get; set; }
        //
        // Summary:
        //     User's account type (account_type). Can only be "individual" or "group"
        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; }
        //
        // Summary:
        //     Id of the user's currently selected title. Not case sensitive and max of 25 characters
        [JsonProperty(PropertyName = "title_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleId { get; set; }
        //
        // Summary:
        //     Display of the user's currently selected title. Case sensitive and max of 25
        //     characters
        [JsonProperty(PropertyName = "title_display", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleDisplay { get; set; }
        //
        // Summary:
        //     True if the user's currently selected title is unique.
        [JsonProperty(PropertyName = "title_unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleUnique { get; set; }
        //
        // Summary:
        //     True if the user's currently selected title is enabled.
        [JsonProperty(PropertyName = "title_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleEnabled { get; set; }
        //
        // Summary:
        //     User's account type (group_account_type). Can only be "family" or "organization"
        [JsonProperty(PropertyName = "group_account_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupAccountType { get; set; }
        //
        // Summary:
        //     Currently selected subaccount's id.
        [JsonProperty(PropertyName = "subaccount_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountId { get; set; }
        //
        // Summary:
        //     Currently selected subaccount's display name.
        [JsonProperty(PropertyName = "subaccount_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountName { get; set; }


        //Kind: Put the item.Kind of reaction here: "gema", "inaccurate"
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "";


        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "notification";

        [JsonProperty(PropertyName = "notification_text", NullValueHandling = NullValueHandling.Ignore)]
        public string NotificationText { get; set; } = "";

        [JsonProperty(PropertyName = "is_read")]
        public bool IsRead { get; set; }
        [JsonProperty(PropertyName = "is_seen")]
        public bool IsSeen { get; set; }
    }
}
