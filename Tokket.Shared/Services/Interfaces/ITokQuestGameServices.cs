using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.SignalR;
using Tokket.Shared.Models.Tokquest;

namespace Tokket.Shared.Services.Interfaces
{
   public interface ITokQuestGameServices
    {
        Task<SignalRConnectionInfo> GetSignalRConnectionInfoGroup(string id, bool IsTokQuest);
        Task<bool> AddToClassGroupRoom(string getuserId, bool isteacher, TokQuestMultiplayer tokquestMultiplayer, TokquestPlayer tokquestPlayer);
        Task<bool> GameStart(string id, TokQuestMultiplayer tokquestMultiplayer);
        Task<bool> SendMessage(string id, GameMessage gameMessage);
    }
}
