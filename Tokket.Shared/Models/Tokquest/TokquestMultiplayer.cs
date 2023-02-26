using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models.Tokquest
{
    public class TokQuestMultiplayer
    {

        // classgroupid + leader id
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        [JsonProperty(PropertyName = "leaderId")]
        public string leaderId { get; set; }

        [JsonProperty(PropertyName = "players", NullValueHandling = NullValueHandling.Ignore)]
        public object[] players { get; set; } = new object[10];

        [JsonProperty(PropertyName = "isActive")]
        public bool isActive { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string label { get; set; } = "tokquestmultiplayer";

        [JsonProperty(PropertyName = "pk")]
        public string pk { get; set; }

        [JsonProperty(PropertyName = "gameId")]
        public string gameId { get; set; }
        [JsonProperty(PropertyName = "gameLength")]
        public int gameLength { get; set; }
        [JsonProperty(PropertyName = "gameType")]
        public string gameType { get; set; }
       
        [JsonProperty(PropertyName = "WhosTurn")]
        public string WhosTurn { get; set; }

        [JsonProperty(PropertyName = "NextTurn")]
        public string NextTurn { get; set; }

        [JsonProperty(PropertyName = "tokquestPlayers", NullValueHandling = NullValueHandling.Ignore)]
        public List<TokquestPlayer> tokquestPlayers { get; set; } = new List<TokquestPlayer>();

        //[JsonProperty(PropertyName = "gauntletExtras", NullValueHandling = NullValueHandling.Ignore)]
        //public List<GauntletExtras> gauntletExtras { get; set; } = new List<GauntletExtras>();

        [JsonProperty(PropertyName = "gameObject",NullValueHandling = NullValueHandling.Ignore)]
        public gameObject gameObject { get; set; }

    }
}
