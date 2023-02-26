using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models;

namespace Tokket.Shared.ViewModels
{
    public class ReactionValueViewModel
    {
        public ReactionValueViewModel()
        {
            User = new List<TokketUserReaction>();
        }

        private long all;
        public long All
        {
            get
            {
                all = GemA + GemB + GemC + Accurate + Inaccurate;
                return all;
            }
            set
            {
                all = value;
            }
        }
        public long GemA { get; set; }
        public long GemB { get; set; }
        public long GemC { get; set; }
        public long Accurate { get; set; }
        public long Inaccurate { get; set; }
        public List<TokketUserReaction> User { get; set; }
    }
}