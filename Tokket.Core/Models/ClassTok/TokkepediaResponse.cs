using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tokket.Core
{
    public class TokkepediaResponse<T>
    {
        /// <summary>The resource.</summary>
        [JsonProperty("resource", NullValueHandling = NullValueHandling.Ignore)]
        public T Resource { get; set; }

        /// <summary>Status Code</summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>Request Charge</summary>
        public double? RequestCharge { get; set; }

        /// <summary>Request Charge Breakdown</summary>
        [JsonProperty("RequestChargeBreakdown", NullValueHandling = NullValueHandling.Ignore)]
        public List<(string, double?)> RequestChargeBreakdown { get; set; } = null;

        /// <summary>Used for concurrency.</summary>
        public string Etag { get; set; } = null;

        /// <summary>Request Charge Breakdown</summary>
        [JsonProperty("ContinuationToken", NullValueHandling = NullValueHandling.Ignore)]
        public string ContinuationToken { get; set; } = null;

        /// <summary></summary>
        [JsonProperty("Count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; } = null;

        /// <summary>Message</summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = null;
    }
}
