//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    public class Sticker : BaseModel
    {
        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "accessory";

        /// <summary>Kind of accessory: Avatar, Tokmoji, or Sticker</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "sticker";

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

        /// <summary>Price USD</summary>
        [JsonProperty(PropertyName = "price_usd", NullValueHandling = NullValueHandling.Ignore)]
        public double? PriceUSD { get; set; }
    }

    public class PurchasedSticker : Sticker
    {
        //Default constructor
        public PurchasedSticker() { }

        public PurchasedSticker(Sticker sticker)
        {
            Id = sticker.Id;
            Name = sticker.Name;
            Text = sticker.Text;
            Image = sticker.Image;
            PriceCoins = sticker.PriceCoins;
            PriceUSD = sticker.PriceUSD;
        }

        /// <summary>Kind of accessory: Avatar, Tokmoji, or Sticker</summary>
        [JsonProperty(PropertyName = "kind")]
        public new string Kind { get; set; } = "purchasedsticker";

        /// <summary>User that made the purchase</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>How purchase was made: 'coins' or 'money'</summary>
        [JsonProperty(PropertyName = "purchase_method")]
        public string PurchaseMethod { get; set; }

        /// <summary>Amount of currency used</summary>
        [JsonProperty(PropertyName = "purchase_amount")]
        public double PurchaseAmount { get; set; }

        /// <summary>Platform purchase was made on: web, mobile</summary>
        [JsonProperty(PropertyName = "device_platform")]
        public string DevicePlatform { get; set; }
    }

}
