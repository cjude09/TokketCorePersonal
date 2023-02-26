using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Tokquest
{
   public class TokquestGameFuntionService
   {
        private static bool Checker(string answer, GamePlayHolder gamePlayHolder) {
            GamePlayHolder _gamplayerHolder = gamePlayHolder;
            var test =    _gamplayerHolder.CurrentGameObject.GameListObject[0].answer.Contains(answer);

            if (test)
            {
                return true;
            }
            else {

                return false;
            }

        }

   }
}
