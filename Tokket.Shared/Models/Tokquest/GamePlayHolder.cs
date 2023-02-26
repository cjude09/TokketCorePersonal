using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
   public class GamePlayHolder
    {

        public gameObject CurrentGameObject { get; set; } = new gameObject();
        public int Score { get; set; }
        public int Round { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }

        public TokquestPlayer Player { get; set; } = new TokquestPlayer();



        //currentGameObject;
        //    score;
        //    round;
        //    Player;
        //    constructor(game,score,round,Player){
        //    this.currentGameObject = game;
        //    this.score = score;
        //    this.round = round;
        //    this.Player = Player;
    }

}
