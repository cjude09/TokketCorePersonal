using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.AlphaToks;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IAlphaToksService
    {
        Task<ResultModel> AddReaction(Tokket.Core.TokkepediaReaction item);

        Task<ResultModel> CreateTokAsync(Tok tok);

        Task<ResultModel> CreateTokAsync(TokModel tok);

        Task<ResultModel> UpdateTokAsync(Tok tok);

        Task<bool> DeleteTokAsync(string id, string pk);

        Task<Tok> GetTokAsync(string id);

        Task<ResultData<Tok>> GetToksAsync(Models.AlphaToks.TokQueryValues values = null);

        Task<ResultData<Tokket.Core.TokkepediaReaction>> GetReactionsAsync(ReactionQueryValues values = null);
    }
}
