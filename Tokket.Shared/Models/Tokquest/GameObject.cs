using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
    public class gameObject : Set
    {

        public gameObject()
        {
            this.Label = "gameset";
        }

        [JsonProperty(PropertyName = "game_name")]
        public string GameName { get; set; }

        [JsonProperty(PropertyName = "game_list_object")]
        public List<GameDetails> GameListObject { get; set; } = new List<GameDetails>();

        [JsonProperty(PropertyName = "game_name_owner", NullValueHandling = NullValueHandling.Ignore)]
        public string game_name_owner { get; set; }

        [JsonProperty(PropertyName = "game_name_owner_title", NullValueHandling = NullValueHandling.Ignore)]
        public string game_name_owner_title { get; set; }

        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string category { get; set; }

        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> comments { get; set; }

        [JsonProperty(PropertyName = "flag", NullValueHandling = NullValueHandling.Ignore)]
        public string flag { get; set; }

        [JsonProperty(PropertyName = "is_score_type", NullValueHandling = NullValueHandling.Ignore)]
        public bool is_score_type { get; set; }

        [JsonProperty(PropertyName = "desc", NullValueHandling = NullValueHandling.Ignore)]
        public string desc { get; set; }

        [JsonProperty(PropertyName = "kind", NullValueHandling = NullValueHandling.Ignore)]
        public string kind { get; set; }

        [JsonProperty(PropertyName = "IsPublic", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPublic { get; set; } = false;

        // tells whether in private group associated
        [JsonProperty(PropertyName = "IsGroup", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsGroup { get; set; } = false;

        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string group_id { get; set; }
    }
}
