using System.Collections.Generic;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Core;

namespace Tokket.Shared.ViewModels
{
    public class GameSetViewModel
    {
        public GameSetModel GameSet { get; set; }
        public List<TokTypeList> TokGroups { get; set; }
        public string TokGroupDataString { get; set; }
        public string Base64Image { get; set; }

        public TokTypeList TokGroup { get; set; }
        public GameScheme GameScheme { get; set; } = GameScheme.TokBlitz;
    }
}
