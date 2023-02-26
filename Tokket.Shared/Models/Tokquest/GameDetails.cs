using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
      public class GameDetails
    {
        [JsonProperty(PropertyName = "question_id", NullValueHandling = NullValueHandling.Ignore)]
        public string QuestionId { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "question_kind")]
        public string QuestionKind { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public List<string> answer { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "question")]
        public string question { get; set; }

        [JsonProperty(PropertyName = "choices")]
        public List<string> choices { get; set; } = new List<string>();

    }
}
