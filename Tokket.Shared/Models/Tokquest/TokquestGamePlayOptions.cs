using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
   public class TokquestGamePlayOptions
    {
        public string GroupId { get; set; }
        public string GameId { get; set; }
        public string OwnerId { get; set; }
        public string GameType { get; set; }
        public int GameLent { get; set; }

    }
}
