using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
    public class GameMessage
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("point")]
        public int point { get; set; }

        [JsonProperty("round")]
        public int round { get; set; }

        [JsonProperty("correctanswers")]
        public int correctanswers { get; set; }



    }
}
