using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.ViewModels
{
    public class MySetsViewModel
    {
        //public Tok Tok { get; set; }
        public IEnumerable<Set> Sets { get; set; } = new List<Set>();
        public string Token { get; set; } = "";
        public IEnumerable<Set> GameSets { get; set; } = new List<Set>();
        public string GameSetToken { get; set; } = "";
    }
}
