//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class Tokmoji : BaseModel
    {
        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "accessory";

        /// <summary>Kind of accessory: Avatar, Tokmoji, or Sticker</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "tokmoji";

        /// <summary>Name to display in shop catalog</summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";

        /// <summary>Text of the item</summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; } = "";

        /// <summary>Image URL</summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; } = "";

        /// <summary>Price coins</summary>
        [JsonProperty(PropertyName = "price_coins", NullValueHandling = NullValueHandling.Ignore)]
        public int PriceCoins { get; set; }

        ///// <summary>Price USD</summary>
        //[JsonProperty(PropertyName = "price_usd", NullValueHandling = NullValueHandling.Ignore)]
        //public int? PriceUSD { get; set; } = null;
    }

    /// <summary>Create this whenever a tokmoji is purchased for a Tok or a Reaction. Id should be in the format of {user_id}-{tokmoji_id}-{GUID}</summary>
    public class PurchasedTokmoji : Tokmoji
    {
        //Default constructor
        public PurchasedTokmoji() { }

        public PurchasedTokmoji(Tokmoji tokmoji)
        {
            Id = tokmoji.Id;
            Name = tokmoji.Name;
            Text = tokmoji.Text;
            Image = tokmoji.Image;
            PriceCoins = tokmoji.PriceCoins;
            //PriceUSD = tokmoji.PriceUSD;
        }

        /// <summary>Kind of accessory: Avatar, Tokmoji, or Sticker</summary>
        [JsonProperty(PropertyName = "kind")]
        public new string Kind { get; set; } = "purchasedtokmoji";

        /// <summary>User that made the purchase</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>Id of the item</summary>
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        /// <summary>Type of the item: tok, set</summary>
        [JsonProperty(PropertyName = "item_label")]
        public string ItemLabel { get; set; }

        /// <summary>How purchase was made: 'coins' or 'money'</summary>
        [JsonProperty(PropertyName = "purchase_method")]
        public string PurchaseMethod => "coins";

        /// <summary>Amount of currency used</summary>
        [JsonProperty(PropertyName = "purchase_amount")]
        public double PurchaseAmount { get; set; }

        /// <summary>Platform purchase was made on: web, mobile</summary>
        [JsonProperty(PropertyName = "device_platform")]
        public string DevicePlatform { get; set; }
    }

}
