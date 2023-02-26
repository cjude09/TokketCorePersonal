using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Chat;
using Tokket.Shared.Models.SignalR;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IChatService
    {
        Task<SignalRConnectionInfo> GetSignalRConnectionInfoGroupChat(string id);
        Task<bool> AddToClassGroupRoomChat(string id, string classgroupid);

        Task<bool> SendMessageTokChat(string classgroupid, TokChatMessage item);

        Task InitChatHub(string id);

        Task InitChatHub(string id,Action<TokChat> action1, Action<TokChat> action2);
        Task InitChatHub(string id, ObservableCollection<TokModel> toks, List<string> commentlist);
        Task DeleteMessage(string id,TokChatMessage message);
        Task UpdateMessage(string id, TokChatMessage message);
        void ChatOnFirstLoad(Action<TokChat> action);

        void ChatOnRecieve(Action<TokChat> action);
    }
}
