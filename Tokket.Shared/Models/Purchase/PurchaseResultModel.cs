using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Purchase
{
    public class PurchaseResultModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; }
        [JsonProperty(PropertyName = "bundle_id")]
        public string BundleId { get; set; }
        [JsonProperty(PropertyName = "device_platform")]
        public string DevicePlatform { get; set; }
        [JsonProperty(PropertyName = "product_id")]
        public string ProductId { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "purchase_type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public object Content { get; set; }
        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess { get; set; }
        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; }
        public double PriceUSD { get; set; }
        public int PriceCoins { get; set; }
    }
}
