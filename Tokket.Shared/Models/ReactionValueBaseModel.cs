using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class ReactionValueBaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("n");

        [JsonProperty("pk")]
        public string Pk { get; set; } = "tokkepediacounter";

        [JsonProperty("label")]
        public string Label { get; set; } = "tokkepediacounter";

        /// <summary>Kind of accessory: Avatar, Tokmoji, or Sticker</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "sticker";

        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty(PropertyName = "service_id")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "device_platform")]
        public string DevicePlatform { get; set; }

        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }


        [JsonProperty(PropertyName = "_rid")]
        public string Rid { get; set; }
        [JsonProperty(PropertyName = "_self")]
        public string Self { get; set; }
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
        [JsonProperty(PropertyName = "_attachments")]
        public string Attachments { get; set; }
        [JsonProperty(PropertyName = "_ts")]
        public string Ts { get; set; }
    }

    public class ReactionValueModel
    {
        public GemsModel GemsModel { get; set; }
        public CommentsModel CommentsModel { get; set; }
        public ViewsModel ViewsModel { get; set; }

        public string Gem1 { get; set; }
        public string Gem2 { get; set; }
        public string Gem3 { get; set; }
        public string Gem4 { get; set; }
        public string Gem5 { get; set; }
        public string TokLevel { get; set; }

        public string GemA { get; set; }
        public string GemB { get; set; }
        public string GemC { get; set; }
        public string Accurate { get; set; }
        public string Inaccurate { get; set; }
        public string TokLevelTotal { get; set; }


        public string GemADetailTotal { get; set; }
        public string GemBDetailTotal { get; set; }
        public string GemCDetailTotal { get; set; }
        public string AccurateDetailTotal { get; set; }
        public string InaccurateDetailTotal { get; set; }
        public string DetailTotal { get; set; }


        public string GemATotal { get; set; }
        public string GemBTotal { get; set; }
        public string GemCTotal { get; set; }
        public string AccurateTotal { get; set; }
        public string InaccurateTotal { get; set; }
        public string Total { get; set; }
    }

    public class GemsModel : ReactionValueBaseModel
    {

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gema", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA { get; set; }

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemb", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB { get; set; }

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemc", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC { get; set; }

        [JsonProperty(PropertyName = "gema1", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA1 { get; set; }

        [JsonProperty(PropertyName = "gemb1", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB1 { get; set; }

        [JsonProperty(PropertyName = "gemc1", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC1 { get; set; }

        [JsonProperty(PropertyName = "gema2", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA2 { get; set; }

        [JsonProperty(PropertyName = "gemb2", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB2 { get; set; }

        [JsonProperty(PropertyName = "gemc2", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC2 { get; set; }

        [JsonProperty(PropertyName = "gema3", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA3 { get; set; }

        [JsonProperty(PropertyName = "gemb3", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB3 { get; set; }

        [JsonProperty(PropertyName = "gemc3", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC3 { get; set; }

        [JsonProperty(PropertyName = "gema4", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA4 { get; set; }

        [JsonProperty(PropertyName = "gemb4", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB4 { get; set; }

        [JsonProperty(PropertyName = "gemc4", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC4 { get; set; }

        [JsonProperty(PropertyName = "gema5", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA5 { get; set; }

        [JsonProperty(PropertyName = "gemb5", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB5 { get; set; }

        [JsonProperty(PropertyName = "gemc5", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC5 { get; set; }

        [JsonProperty(PropertyName = "gema6", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA6 { get; set; }

        [JsonProperty(PropertyName = "gemb6", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB6 { get; set; }

        [JsonProperty(PropertyName = "gemc6", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC6 { get; set; }

        [JsonProperty(PropertyName = "gema7", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA7 { get; set; }

        [JsonProperty(PropertyName = "gemb7", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB7 { get; set; }

        [JsonProperty(PropertyName = "gemc7", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC7 { get; set; }

        [JsonProperty(PropertyName = "gema8", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA8 { get; set; }

        [JsonProperty(PropertyName = "gemb8", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB8 { get; set; }

        [JsonProperty(PropertyName = "gemc8", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC8 { get; set; }

        [JsonProperty(PropertyName = "gema9", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA9 { get; set; }

        [JsonProperty(PropertyName = "gemb9", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB9 { get; set; }

        [JsonProperty(PropertyName = "gemc9", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC9 { get; set; }

        [JsonProperty(PropertyName = "gema10", NullValueHandling = NullValueHandling.Ignore)]
        public long GemA10 { get; set; }

        [JsonProperty(PropertyName = "gemb10", NullValueHandling = NullValueHandling.Ignore)]
        public long GemB10 { get; set; }

        [JsonProperty(PropertyName = "gemc10", NullValueHandling = NullValueHandling.Ignore)]
        public long GemC10 { get; set; }
    }

    public class CommentsModel : ReactionValueBaseModel
    {
        [JsonProperty(PropertyName = "accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate { get; set; }

        [JsonProperty(PropertyName = "inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate { get; set; }

        [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment { get; set; }

        [JsonProperty(PropertyName = "accurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate1 { get; set; }

        [JsonProperty(PropertyName = "inaccurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate1 { get; set; }

        [JsonProperty(PropertyName = "comment1", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment1 { get; set; }

        [JsonProperty(PropertyName = "accurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate2 { get; set; }

        [JsonProperty(PropertyName = "inaccurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate2 { get; set; }

        [JsonProperty(PropertyName = "comment2", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment2 { get; set; }

        [JsonProperty(PropertyName = "accurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate3 { get; set; }

        [JsonProperty(PropertyName = "inaccurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate3 { get; set; }

        [JsonProperty(PropertyName = "comment3", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment3 { get; set; }

        [JsonProperty(PropertyName = "accurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate4 { get; set; }

        [JsonProperty(PropertyName = "inaccurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate4 { get; set; }

        [JsonProperty(PropertyName = "comment4", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment4 { get; set; }

        [JsonProperty(PropertyName = "accurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate5 { get; set; }

        [JsonProperty(PropertyName = "inaccurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate5 { get; set; }

        [JsonProperty(PropertyName = "comment5", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment5 { get; set; }

        [JsonProperty(PropertyName = "accurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate6 { get; set; }

        [JsonProperty(PropertyName = "inaccurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate6 { get; set; }

        [JsonProperty(PropertyName = "comment6", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment6 { get; set; }

        [JsonProperty(PropertyName = "accurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate7 { get; set; }

        [JsonProperty(PropertyName = "inaccurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate7 { get; set; }

        [JsonProperty(PropertyName = "comment7", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment7 { get; set; }

        [JsonProperty(PropertyName = "accurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate8 { get; set; }

        [JsonProperty(PropertyName = "inaccurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate8 { get; set; }

        [JsonProperty(PropertyName = "comment8", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment8 { get; set; }

        [JsonProperty(PropertyName = "accurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate9 { get; set; }

        [JsonProperty(PropertyName = "inaccurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate9 { get; set; }

        [JsonProperty(PropertyName = "comment9", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment9 { get; set; }

        [JsonProperty(PropertyName = "accurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long Accurate10 { get; set; }

        [JsonProperty(PropertyName = "inaccurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long Inaccurate10 { get; set; }

        [JsonProperty(PropertyName = "comment10", NullValueHandling = NullValueHandling.Ignore)]
        public long Comment10 { get; set; }
    }

    public class ViewsModel : ReactionValueBaseModel
    {
        [JsonProperty(PropertyName = "tiletap_views", NullValueHandling = NullValueHandling.Ignore)]
        public long TileTapViews { get; set; }
        [JsonProperty(PropertyName = "pagevisit_views", NullValueHandling = NullValueHandling.Ignore)]
        public long PageVisitViews { get; set; }
        [JsonProperty(PropertyName = "tiletap_views_personal", NullValueHandling = NullValueHandling.Ignore)]
        public long TileTapViewsPersonal { get; set; }
        [JsonProperty(PropertyName = "page_views_personal", NullValueHandling = NullValueHandling.Ignore)]
        public long PageViewsPersonal { get; set; }

    }
}
