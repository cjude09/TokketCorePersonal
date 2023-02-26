using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.SignalR
{
    public class SignalRService
    {
    }
    public class SignalRConnectionInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}
