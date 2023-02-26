//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Badge.</summary>
    public class Badge : BaseModel
    {
        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "accessory";

        /// <summary>Kind of accessory.</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "badge";

        /// <summary>Name to display in shop catalog</summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = null;

        /// <summary>Description of the item</summary>
        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        /// <summary>Image URL</summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; } = "";

        /// <summary>Image Thumbnail</summary>
        [JsonProperty(PropertyName = "image_thumbnail")]
        public string ImageThumbnail { get; set; } = "";

        /// <summary>Achievement Id</summary>
        [JsonProperty(PropertyName = "achievement_id")]
        public string AchievementId { get; set; } = "";

        /// <summary>Null if no colors. Otherwise the colors will go here.</summary>
        [JsonProperty(PropertyName = "badges_colors", NullValueHandling = NullValueHandling.Ignore)]
        public string[] BadgeColors { get; set; } = null;

        /// <summary>Royalty badge</summary>
        [JsonProperty(PropertyName = "is_royalty", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRoyalty { get; set; } = null;

        /// <summary>Avatars have a place of origin, most of the time it will be a serviceid like 
        /// Example: tokblitz.</summary>
        [JsonProperty(PropertyName = "origin_id")]
        public string OriginId { get; set; } = "";

        /// <summary>Field in case sets of avatars are released as series. Example: 1, ...2, ...3, ...4</summary>
        [JsonProperty(PropertyName = "series_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SeriesId { get; set; } = null;

        /// <summary>Service ids where avatar cannot be used. Example: tokblitz, tokkepedia. Starts off with 1 item so that it is never null.</summary>
        [JsonProperty(PropertyName = "excluded_services", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ExcludedServices { get; set; } = new string[] { "ticktock" };

        /// <summary>Price coins</summary>
        [JsonProperty(PropertyName = "price_coins", NullValueHandling = NullValueHandling.Ignore)]
        public int PriceCoins { get; set; }

        /// <summary>Price USD</summary>
        [JsonProperty(PropertyName = "price_usd", NullValueHandling = NullValueHandling.Ignore)]
        public double? PriceUSD { get; set; }
    }


    public class BadgeOwned : Badge
    {
        //Default constructor
        public BadgeOwned() { }

        public BadgeOwned(Badge item)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Image = item.Image;
            OriginId = item.OriginId;
            SeriesId = item.SeriesId;
            ExcludedServices = item.ExcludedServices;
            PriceCoins = item.PriceCoins;
            PriceUSD = item.PriceUSD;
        }

        /// <summary>Kind of accessory.</summary>
        [JsonProperty(PropertyName = "kind")]
        public new string Kind { get; set; } = "badgeowned";

        /// <summary>User that made the purchase</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>How purchase was made: 'coins' or 'money'</summary>
        [JsonProperty(PropertyName = "purchase_method")]
        public string PurchaseMethod { get; set; }

        /// <summary>Amount of currency used</summary>
        [JsonProperty(PropertyName = "purchase_amount")]
        public double PurchaseAmount { get; set; }

        /// <summary>If true this item will not be shown when querying. This may help handle many items in the future.</summary>
        [JsonProperty(PropertyName = "unlisted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Unlisted { get; set; } = null;

        /// <summary>Badge color if there's multiple variants. Optional</summary>
        [JsonProperty(PropertyName = "badge_color", NullValueHandling = NullValueHandling.Ignore)]
        public string BadgeColor { get; set; } = null;
    }
}
