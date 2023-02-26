//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    //using PayPalCheckoutSdk.Orders;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PurchaseResult : BaseModel
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "purchase";

        //App package id (i.e. com.companyname.MyApp1)
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "bundle_id")]
        public string BundleId { get; set; }

        [JsonProperty(PropertyName = "device_platform")]
        public string DevicePlatform { get; set; }

        //Product ID
        [JsonProperty(PropertyName = "product_id")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "purchase_type")]
        public string Type { get; set; } = "consumable";

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "Example: PlayerName's profile {id} updated with 10 more strikes.";

        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Content { get; set; } = null;

        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess { get; set; } = true;

        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        //Cost
        [JsonProperty(PropertyName = "price_usd", NullValueHandling = NullValueHandling.Ignore)]
        public double? PriceUSD { get; set; } = null;

        [JsonProperty(PropertyName = "price_coins", NullValueHandling = NullValueHandling.Ignore)]
        public int? PriceCoins { get; set; } = null;
    }
}
