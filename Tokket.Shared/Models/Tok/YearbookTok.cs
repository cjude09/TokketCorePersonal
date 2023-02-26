using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tok
{
    public class YearbookTok : TokModel
    {
        [JsonProperty(PropertyName = "yearbook_type", NullValueHandling = NullValueHandling.Ignore)]
        public string YearbookType { get; set; } = "";
        [JsonProperty(PropertyName = "yearbook_schoolname", NullValueHandling = NullValueHandling.Ignore)]
        public string YearbookSchoolname { get; set; } = "";
        [JsonProperty(PropertyName = "yearbook_grouptype", NullValueHandling = NullValueHandling.Ignore)]
        public string YearbookGroupType { get; set; } = "";
        [JsonProperty(PropertyName = "yearbook_timing", NullValueHandling = NullValueHandling.Ignore)]
        public string YearbookTiming { get; set; } = "";
        [JsonProperty(PropertyName = "yearbook_graduationmonthyear", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime YearbookGraduationMonthYear { get; set; }
        [JsonProperty(PropertyName = "yearbook_tiletype", NullValueHandling = NullValueHandling.Ignore)]
        public string YearbookTileType { get; set; } = "";
        [JsonProperty(PropertyName = "is_pic", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPic { get; set; }
        [JsonProperty(PropertyName = "not_pic", NullValueHandling = NullValueHandling.Ignore)]
        public bool NotPic { get; set; }
        [JsonProperty(PropertyName = "has_image", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasImage { get; set; }
        [JsonProperty(PropertyName = "school_group_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SchoolGroupName { get; set; } = "";
    }

}
