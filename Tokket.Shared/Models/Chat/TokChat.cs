using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;
namespace Tokket.Shared.Models.Chat
{
    public class TokChat : BaseModel
    {

        //[JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string GroupId { get; set; }

        //[JsonProperty(PropertyName = "label")]
        //public string Label { get; set; } = "tokchat";

        ////[JsonProperty(PropertyName = "owner_id", NullValueHandling = NullValueHandling.Ignore)]
        ////public string OwnerId { get; set; }

        //[JsonProperty(PropertyName = "itembase")]
        //public string ItemBase { get; set; }

        //[JsonProperty(PropertyName = "tokchat_message", NullValueHandling = NullValueHandling.Ignore)]
        //public List<TokChatMessage> tokchat_message { get; set; }

        //[JsonProperty(PropertyName = "id")]
        //public string Id { get; set; }

        //[JsonProperty(PropertyName = "pk")]
        //public string Pk { get; set; }

        //[JsonProperty(PropertyName = "created_time" , NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime CreatedTime { get; set; }
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tokchat";

        //[JsonProperty(PropertyName = "owner_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string OwnerId { get; set; }

        [JsonProperty(PropertyName = "itembase")]
        public string ItemBase { get; set; }

        [JsonProperty(PropertyName = "tokchat_message")]
        public List<TokChatMessage> TokChatMessage { get; set; } = new List<TokChatMessage>();
    }
    //
    // Summary:
    //     Base class for all classes to derive from.
    public class BaseModel1
    {
        //
        // Summary:
        //     Base Model Constructor to initialize default values
        //public BaseModel1();

        //
        // Summary:
        //     Points earned.
        [JsonProperty(PropertyName = "points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; }
        //
        // Summary:
        //     Coins spent.
        [JsonProperty(PropertyName = "coins_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? CoinsDeleted { get; set; }
        //
        // Summary:
        //     Coins earned.
        [JsonProperty(PropertyName = "coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; }
        //
        // Summary:
        //     Property used to ensure document has not changed.
        public string _etag { get; set; }
        //
        // Summary:
        //     The recent update datetime in Unix time format
        [JsonProperty(PropertyName = "_ts")]
        public long _Timestamp { get; set; }
        //
        // Summary:
        //     The recent update datetime in DateTime time format
        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp { get; set; }
        //
        // Summary:
        //     Coin count before action was applied.
        [JsonProperty(PropertyName = "coins_previous", NullValueHandling = NullValueHandling.Ignore)]
        public long? CoinsPrevious { get; set; }
        //
        // Summary:
        //     Date and time when the object was created. Should only be modified at object
        //     creation.
        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; }
        //
        // Summary:
        //     Document duplicate locations. Use .Find(), .Contains() or .Where() with "category-",
        //     "toktype-",
        [JsonProperty(PropertyName = "duplicates", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Duplicates { get; set; }
        //
        // Summary:
        //     Determines how the document is stored
        [JsonProperty(PropertyName = "pk", NullValueHandling = NullValueHandling.Ignore)]
        public string PartitionKey { get; set; }
        //
        // Summary:
        //     Device platform the document was created (if any).
        [JsonProperty(PropertyName = "device_platform", NullValueHandling = NullValueHandling.Ignore)]
        public string DevicePlatform { get; set; }
        //
        // Summary:
        //     Origin of the document.
        [JsonProperty(PropertyName = "service_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceId { get; set; }
        //
        // Summary:
        //     Id of the document
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //
        // Summary:
        //     Stores the type of database action: create, update, delete, duplicate
        [JsonProperty(PropertyName = "soft_marker", NullValueHandling = NullValueHandling.Ignore)]
        public string SoftMarker { get; set; }
        //
        // Summary:
        //     Points count before action was applied.
        [JsonProperty(PropertyName = "points_previous", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsPrevious { get; set; }

        //
        // Summary:
        //     Converts DateTime to Integer
        //public long ToUnixTime(DateTime dateTime);
    }
}

