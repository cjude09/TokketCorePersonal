using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IReactionService
    {
        Task<ResultModel> AddReaction(ReactionModel item);
        Task<bool> UpdateReaction(TokkepediaReaction item);
        Task<bool> DeleteReaction(string id);
        Task<ResultData<ReactionModel>> GetReactionsAsync(ReactionQueryValues values = null, string fromCaller = "");
        Task<ReactionValueModel> GetReactionsValueAsync(string id, string fromCaller = "");
        Task<List<TokketUserReaction>> GetReactionsUsersAsync(ReactionQueryValues reactionQueryValues);
        Task<ResultData<ReactionModel>> GetCommentReplyAsync(ReactionQueryValues values = null);
        Task<TokkepediaReaction> GetCommentAsync(string id);
        Task<ResultData<TokkepediaReaction>> UserReactionsGet(string item_id, string userid);
        Task<ResultModel> AddReport(Report item);
        ResultData<ReactionModel> GetReactionsCache(string fromCaller);
        void SetReactionsCache(string fromCaller, ResultData<ReactionModel> data);
        ReactionValueModel GetReactionsValueCache(string fromCaller);
        void SetReactionsValueCache(string fromCaller, ReactionValueModel data);
    }
}
