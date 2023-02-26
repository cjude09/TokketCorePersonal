using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
  
        public class TokquestPlayer
        {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string Pk { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "classgroup-tokquest";
        [JsonProperty(PropertyName = "group_id")]
        public string GroupId { get; set; }
        [JsonProperty(PropertyName = "fullname")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "user_photo")]
        public string user_photo { get; set; }

        [JsonProperty(PropertyName = "round")]
        public int Round { get; set; } = 0;
        [JsonProperty(PropertyName = "total_point")]
        public int total_point { get; set; } = 0;
        [JsonProperty(PropertyName = "correct_answers")]
        public int correct_answers { get; set; } = 0;

        [JsonProperty(PropertyName = "wrong_answers", NullValueHandling = NullValueHandling.Ignore)]
        public int wrong_answers { get; set; } = 0;

        [JsonProperty(PropertyName = "percent_correct")]
        public Decimal percent_correct { get; set; } = 0;
       
        [JsonProperty(PropertyName = "is_finished")]
        public bool is_finished { get; set; } = false;

        [JsonProperty(PropertyName = "is_teacher")]
        public bool is_teacher { get; set; } = false;

        [JsonProperty(PropertyName = "is_active")]
        public bool is_active { get; set; } = true;

        [JsonProperty(PropertyName = "created_time")]
        public DateTime created_time { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "team_name")]
        public string team_name { get; set; } = "";

        [JsonProperty(PropertyName = "player_number")]
        public int player_number { get; set; } = 0;
    }




}
