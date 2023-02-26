using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class GameSetModel
    {
        public int IdImageGame { get; set; }
        //
        // Summary:
        //     Game names: "tokboom", "tokblast"
        public string GameTitle { get; set; }
        public string GameDescription { get; set; }

        public string Category { get; set; }
        public string CategoryId { get; set; }
        public List<TokModel> Toks { get; set; }


        public string QuestionType { get; set; }
        public string Question { get; set; }
        public int AnswerPosition { get; set; } = 0;
        public string AnswerTrueFalse { get; set; }
        public ObservableCollection<AddTokDetailModel> AnswerList { get; set; }

    }
}
