//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class TokkepediaNotificationSet
    {
        [JsonProperty(PropertyName = "unseen")]
        public long Unseen { get; set; }
        
        [JsonProperty(PropertyName = "unread")]
        public long Unread { get; set; }
        
        [JsonProperty(PropertyName = "results")]
        public List<TokkepediaNotification> Results { get; set; }

        [JsonProperty(PropertyName = "pagination_id")]
        public string PaginationId { get; set; }
    }

}
