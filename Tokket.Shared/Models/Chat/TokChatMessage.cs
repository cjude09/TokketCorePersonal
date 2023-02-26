using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;
namespace Tokket.Shared.Models.Chat
{
    public class TokChatMessage
    {
      
        [JsonProperty(PropertyName = "group_id")]
        public string group_id { get; set; }


        [JsonProperty(PropertyName = "date_time")]
        public string DateTime { get; set; }


        [JsonProperty(PropertyName = "group_pk")]
        public string group_pk { get; set; }

        [JsonProperty(PropertyName = "sender_id")]
        public string sender_id { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string message { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string label { get; set; } = "tokchatmessage";

        [JsonProperty(PropertyName = "image")]
        public string image { get; set; } = "/img/tokquest-images/tokquesticon.png";

        [JsonProperty(PropertyName = "display_name")]
        public string display_name { get; set; } = "";

        [JsonProperty(PropertyName = "files")]
        public List<string> files { get; set; }

        [JsonProperty(PropertyName = "filesextension")]
        public List<string> filesextension { get; set; }

        [JsonProperty(PropertyName = "filesname")]
        public List<string> filesname { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string pk { get; set; }

        [JsonProperty(PropertyName = "created_time")]
        public DateTime created_time { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime timestamp { get; set; }

        ////
        //// Summary:
        ////     Points earned.
        //[JsonProperty(PropertyName = "points", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Points { get; set; }
        ////
        //// Summary:
        ////     Coins spent.
        //[JsonProperty(PropertyName = "coins_deleted", NullValueHandling = NullValueHandling.Ignore)]
        //public long? CoinsDeleted { get; set; }
        ////
        //// Summary:
        ////     Coins earned.
        //[JsonProperty(PropertyName = "coins", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Coins { get; set; }
        ////
        //// Summary:
        ////     Property used to ensure document has not changed.
        //public string _etag { get; set; }
        ////
        //// Summary:
        ////     The recent update datetime in Unix time format
        //[JsonProperty(PropertyName = "_ts")]
        //public long _Timestamp { get; set; }
        ////
        //// Summary:
        ////     The recent update datetime in DateTime time format
        //[JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime Timestamp { get; set; }
        ////
        //// Summary:
        ////     Coin count before action was applied.
        //[JsonProperty(PropertyName = "coins_previous", NullValueHandling = NullValueHandling.Ignore)]
        //public long? CoinsPrevious { get; set; }
        ////
        //// Summary:
        ////     Date and time when the object was created. Should only be modified at object
        ////     creation.
        //[JsonProperty(PropertyName = "created_time")]
        //public DateTime CreatedTime { get; set; }
        ////
        //// Summary:
        ////     Document duplicate locations. Use .Find(), .Contains() or .Where() with "category-",
        ////     "toktype-",
        //[JsonProperty(PropertyName = "duplicates", NullValueHandling = NullValueHandling.Ignore)]
        //public List<string> Duplicates { get; set; }
        ////
        //// Summary:
        ////     Determines how the document is stored
        //[JsonProperty(PropertyName = "pk", NullValueHandling = NullValueHandling.Ignore)]
        //public string PartitionKey { get; set; }
        ////
        //// Summary:
        ////     Device platform the document was created (if any).
        //[JsonProperty(PropertyName = "device_platform", NullValueHandling = NullValueHandling.Ignore)]
        //public string DevicePlatform { get; set; }
        ////
        //// Summary:
        ////     Origin of the document.
        //[JsonProperty(PropertyName = "service_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string ServiceId { get; set; }
        ////
        //// Summary:
        ////     Id of the document
        //[JsonProperty(PropertyName = "id")]
        //public string Id { get; set; }
        ////
        //// Summary:
        ////     Stores the type of database action: create, update, delete, duplicate
        //[JsonProperty(PropertyName = "soft_marker", NullValueHandling = NullValueHandling.Ignore)]
        //public string SoftMarker { get; set; }
        ////
        //// Summary:
        ////     Points count before action was applied.
        //[JsonProperty(PropertyName = "points_previous", NullValueHandling = NullValueHandling.Ignore)]
        //public long? PointsPrevious { get; set; }

    }
}