using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
    public class ClassSet : Set
    {
        #region Set Extension    
        [JsonProperty(PropertyName = "tokpks")]
        public List<string> TokPartitionKeys { get; set; }
        #endregion
        #region Privacy
        /// <summary>Determines if tok is private.</summary>
        [JsonProperty(PropertyName = "is_private", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPrivate { get; set; } = true;
        #endregion                   

        #region Group
        /// <summary>Only add if this content is part of a group. </summary>
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; } = null;

        [JsonProperty(PropertyName = "group", NullValueHandling = NullValueHandling.Ignore)]
        public ClassGroup Group { get; set; }

        public int count { get; set; }

        #endregion

        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; } = "individual";
        public string RandomBorderColor { get; set; } = "";
    }
}
