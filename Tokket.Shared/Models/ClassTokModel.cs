using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class ClassTokModel : TokModel
    {
        #region Privacy
        /// <summary>Determines if tok is private.</summary>
        [JsonProperty(PropertyName = "is_private", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPrivate { get; set; } = true;
        /// <summary>Determines if tok is public.</summary>
        
        public bool IsPublic { get; set; } = false;
        /// <summary>Determines if tok is group.</summary>
        public bool IsGroup { get; set; } = false;
        #endregion

       #region Group
        //Below code already exist in TokModel
        /*
        /// <summary>Only add if this content is part of a group. </summary>
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; } = null;*/

        /// <summary>Name of the group belong </summary>
        [JsonProperty(PropertyName = "group_name", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupName { get; set; } = null;
        #endregion

        #region Tok Links
        /// <summary>Tok Link - for Basic toks only. Should contain the tok ID - no urls allowed.</summary>
        [JsonProperty(PropertyName = "tok_link", NullValueHandling = NullValueHandling.Ignore)]
        public string TokLink { get; set; } = null;
        /// <summary>Tok Link.</summary>
        /// <summary>Tok Link - for Detailed toks only. Should contain the tok ID - no urls allowed.</summary>
        [JsonProperty(PropertyName = "detail_tok_links", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DetailTokLinks { get; set; } = null;
        #endregion

        #region TokPak
        [JsonProperty(PropertyName = "images_istokpak_visible", NullValueHandling = NullValueHandling.Ignore)]
        public List<bool> ImagesIsTokPakVisible { get; set; } = null;
        #endregion
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ImagesIsTokPakVisibleInitialize
        {
            get
            {
                //Must be within the year of purchase
                if (ImagesIsTokPakVisible == null)
                {
                    ImagesIsTokPakVisible = new List<bool> { true, true, true, true, true, true, true, true, true, true };
                    return false;
                }
                else
                    return true;
            }

          private  set { }
           
        }

        public ViewsModel ViewsModel { get; set; }
        #region Tok Share
        //temporary
        [JsonProperty(PropertyName = "tok_share_pk", NullValueHandling = NullValueHandling.Ignore)]
        public string TokSharePk { get; set; } = null;

        /// <summary>Tok Share - contains the id of the shared tok. If this field isn't null or empty then there should only be a PrimaryFieldText filled out in this item.</summary>
        [JsonProperty(PropertyName = "tok_share", NullValueHandling = NullValueHandling.Ignore)]
        public string TokShare { get; set; } = null;

        [JsonProperty(PropertyName = "shared_tok", NullValueHandling = NullValueHandling.Ignore)]
        public string SharedTok { get; set; } = null;
        #endregion

        #region Xamarin Stuffs
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DetailSummaryText
        {
            get
            {
                var bulletStrings = string.Empty;
                if (Details == null) return bulletStrings;
                if (Details.Length == 0) return bulletStrings;

                foreach (var item in Details)
                {
                    if(!string.IsNullOrEmpty(item))
                        bulletStrings += "> " + item + "\n";
                }

                return bulletStrings;
            }
            private set { }

        }
        #endregion

        [JsonProperty(PropertyName = "group", NullValueHandling = NullValueHandling.Ignore)]
        public ClassGroupModel Group { get; set; } = null;

        public bool IsMasterCopy { get; set; } = false;
    }
}
