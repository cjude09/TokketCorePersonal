using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class ClassSetModel : Set
    {
        //#region Set Format
        //[JsonIgnore]
        //private string[] validFormats = new string[] { "Basic", "Detailed", "Mega" };
        //[JsonIgnore]
        //private string tokFormat = "Basic";
        ///// <summary>In a class tok, Tok Groups are Tok Formats.</summary>
        //[JsonProperty(PropertyName = "tok_group")]
        //public new string TokGroup
        //{
        //    get { return tokFormat; }
        //    set
        //    {
        //        if (validFormats.Contains(value))
        //            tokFormat = value;
        //        else
        //            tokFormat = "Detailed";
        //    }
        //}
        //#endregion
        #region Privacy
        /// <summary>Determines if tok is private.</summary>
        [JsonProperty(PropertyName = "is_private", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPrivate { get; set; } = true;
        #endregion
        #region Group
        /// <summary>Determines if tok is private.</summary>
        //[JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string GroupId { get; set; } = null;


        [JsonProperty(PropertyName = "group", NullValueHandling = NullValueHandling.Ignore)]
        public ClassGroupModel Group { get; set; }
        #endregion

        [JsonIgnore]
        public string ThumbnailImage => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "md-image-") : "";

        [JsonIgnore]
        public string ThumbnailImageSmall => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "sm-image-") : "";

        #region Supersets  
        /// <summary>
        ///     List of Set Ids
        /// </summary>
        [JsonProperty(PropertyName = "setids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SetIds { get; set; } = new List<string>();

        /// <summary>
        ///     List of Set Partition Keys
        /// </summary>
        [JsonProperty(PropertyName = "setpks", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SetPartitionKeys { get; set; } = new List<string>();

        /// <summary>
        ///     List of Sets in the Superset
        /// </summary>
        [JsonProperty(PropertyName = "sets", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClassSetModel> Sets { get; set; } = new List<ClassSetModel>();

        [JsonIgnore]
        private List<string> PlayableTypesList { get; set; } = new List<string>();
        
        [JsonIgnore] 
        public bool IsPlayable { 
            get {
                if (PlayableTypesList.Contains("non-playable"))
                {
                    return false;
                }
                else {
                    return true;    
                }
             }
            
            set { } }

        #endregion
    }
}