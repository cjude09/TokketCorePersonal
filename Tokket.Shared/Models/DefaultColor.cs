using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    //Defines a default color for a categorization of items
    public class DefaultColor : BaseModel
    {
        //id and pk: defaultcolor-{key}-{value}-{userid}
        public DefaultColor()
        {
            Id = "defaultcolor-tok_type-MAT101-user_id";
            PartitionKey = "defaultcolor-tok_type-MAT101-user_id";
        }
        
        [JsonProperty(PropertyName = "label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; } = "defaultcolor";
        
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; } = "";
        
        //I.e. "tok_type" because default colors are assigned to classes. If it were for a whole category, then it would be "category"
        [JsonProperty(PropertyName = "key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; } = "tok_type";
        
        //Value of the kind: in this case the specific class
        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; } = "MAT101";
        
        [JsonProperty(PropertyName = "color_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorHex { get; set; } = "#FFFFFF";
    }
}
