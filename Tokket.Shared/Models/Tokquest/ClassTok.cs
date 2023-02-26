using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models.AlphaToks;

namespace Tokket.Shared.Models.Tokquest
{
    public class ClassTok : TokModel
    {
        //[JsonProperty(PropertyName = "pk")]
        //public string PartitionKey { get; set; }

        #region Privacy
        /// <summary>Determines if tok is private.</summary>
        [JsonProperty(PropertyName = "is_private", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPrivate { get; set; } = true;
        /// <summary>Determines if tok is public.</summary>
        public bool IsPublic { get; set; } = false;
        /// <summary>Determines if tok is group.</summary>
        public bool IsGroup { get; set; } = false;
        #endregion

        public string BorderColor { get; set; } = "#808080";

        #region Tok Links

        /// <summary>Tok Link - for Basic toks only. Should contain the tok ID - no urls allowed.</summary>
        [JsonProperty(PropertyName = "tok_link", NullValueHandling = NullValueHandling.Ignore)]
        public string TokLink { get; set; } = null;

        /// <summary>Tok Link.</summary>
        /// <summary>Tok Link - for Detailed toks only. Should contain the tok ID - no urls allowed.</summary>
        [JsonProperty(PropertyName = "detail_tok_links", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DetailTokLinks { get; set; } = null;
        #endregion
    }
}
