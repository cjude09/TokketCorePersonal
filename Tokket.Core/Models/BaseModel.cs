using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class BaseModel
    {
        /// <summary>
        ///     Base Model Constructor to initialize default values
        /// </summary>
        public BaseModel()
        {
            PartitionKey = id;
        }

        #region ID
        [JsonIgnore]
        string id = Guid.NewGuid().ToString("n");

        /// <summary>Id of the document</summary>
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set
            {
                //Set the id and partition key to be the same by default
                //PartitionKey = value;  // This causes the edits not working for other data that don't have the same value with id [Cris]
                id = value;
            }
        }
        #endregion

        /// <summary>Origin of the document.</summary>
        [JsonProperty(PropertyName = "service_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceId { get; set; } = null;

        /// <summary>Device platform the document was created (if any).</summary>
        [JsonProperty(PropertyName = "device_platform", NullValueHandling = NullValueHandling.Ignore)]
        public string DevicePlatform { get; set; } = null;

        /// <summary>Determines how the document is stored</summary>
        [JsonProperty(PropertyName = "pk", NullValueHandling = NullValueHandling.Ignore)]
        public string PartitionKey { get; set; }

        /// <summary>Document duplicate locations. Use .Find(), .Contains() or .Where() with "category-", "toktype-", </summary>
        [JsonProperty(PropertyName = "duplicates", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Duplicates { get; set; } = null;

        /// <summary>Stores the type of database action: create, update, delete, duplicate</summary>
        [JsonProperty(PropertyName = "soft_marker", NullValueHandling = NullValueHandling.Ignore)]
        public string SoftMarker { get; set; } = null;

        #region Created Time and Most Recent Update
        /// <summary>Date and time when the object was created. Should only be modified at object creation.</summary>
        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        [JsonIgnore]
        private DateTime timestamp = DateTime.Now;

        /// <summary> The recent update datetime in DateTime time format </summary>
        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = DateTime.Now;
                _Timestamp = ToUnixTime(DateTime.Now);
            }
        }

        /// <summary> The recent update datetime in Unix time format </summary>
        [JsonProperty(PropertyName = "_ts")]
        public long _Timestamp { get; set; }

        /// <summary> Converts DateTime to Integer </summary>
        public long ToUnixTime(DateTime dateTime)
        {
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)Math.Round((dateTime - d).TotalSeconds);
        }

        /// <summary> Property used to ensure document has not changed.</summary>
        public string _etag { get; set; }
        #endregion

        #region Coins/Points
        /// <summary>Coins earned.</summary>
        [JsonProperty(PropertyName = "coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; } = null;

        /// <summary>Coins spent.</summary>
        [JsonProperty(PropertyName = "coins_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? CoinsDeleted { get; set; } = null;

        /// <summary>Points earned.</summary>
        [JsonProperty(PropertyName = "points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; } = null;

        /// <summary>Coin count before action was applied.</summary>
        [JsonProperty(PropertyName = "coins_previous", NullValueHandling = NullValueHandling.Ignore)]
        public long? CoinsPrevious { get; set; } = null;

        /// <summary>Points count before action was applied.</summary>
        [JsonProperty(PropertyName = "points_previous", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsPrevious { get; set; } = null;
        #endregion
    }
}
