using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class TokPak : Set
    {
        [JsonProperty(PropertyName = "label")]
        public new string Label { get; set; } = "tokpak";

        [JsonProperty(PropertyName = "bullet_kind")]
        public string BulletKind { get; set; } = "bullet";

        public BulletKind BulletKindEnum => BulletKind == "bullet" || string.IsNullOrEmpty(BulletKind) ? Helpers.BulletKind.Bullet : Helpers.BulletKind.Number;

        [JsonProperty("tokpak_type")]
        public TokPakType Type { get; set; } = TokPakType.Paper;

        //[JsonProperty("tokpak_toks")]
        //public List<Tok> Toks { get; set; } = new List<Tok>();

        /// <summary></summary>
        [JsonProperty(PropertyName = "classtoks", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClassTokModel> ClassToks { get; set; } = null;

        /// <summary>
        ///     Ids are not stored. Only getter if there's some data in Class Toks.
        /// </summary>
        [JsonProperty(PropertyName = "ids", NullValueHandling = NullValueHandling.Ignore)]
        public new List<string> TokIds => ClassToks != null ? ClassToks.Select(x => x.Id).ToList() : new List<string>();

        public bool IncludeInPublic { get; set; }
    }
}
