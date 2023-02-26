using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class OggClass
    {

        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "primary_text")]
        public string PrimaryText { get; set; }

        [JsonProperty(PropertyName = "secondary_text")]
        public string SecondaryText { get; set; }

        public string Title { get; set; } = "Quote of the Hour - ";
        public string Desc { get; set; }
        public string Desc2 { get; set; }

        public string address { get; set; } //= "https://" + QuoteTool.BaseUrlGet() + "/quoteofthehour/";
        public string BaseAddress { get; set; } //= "https://" + QuoteTool.BaseUrlGet();
        //public string secondary { get; set; }
        //public string primary { get; set; }

        //public string category { get; set; }
        //public int tokID { get; set; }
    }
}
