using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Core;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface ITokService
    {
        Task<List<TokModel>> GetAllToks(TokQueryValues values);
        Task<ResultData<TokSection>> GetTokSectionsAsync(string tokId, int count = 0, string continuationToken = null);
        Task<bool> CreateTokSectionAsync(TokSection tokSection, string tokId, int partitionNumber);
        Task<bool> UpdateTokSectionAsync(TokSection newTokSection);
        Task<bool> DeleteTokSectionAsync(TokSection tokSection);
        Task<ResultData<TokSection>> GetQnATokSectionsAsync(string tokId, int count = 0, string continuationToken = null);
        Task<bool> CreateQnaTokSectionAsync(TokSection tokSection, string tokId, int partitionNumber);
        Task<bool> UpdateQnATokSectionAsync(TokSection newTokSection);
        Task<bool> DeleteQnATokSectionAsync(TokSection tokSection);
        Task<List<TokModel>> GetAllFeaturedToks();
        Task<List<TokModel>> GetAllFeaturedToksAsync();
        Task<ResultModel> CreateTokAsync(TokModel tok, CancellationToken cancellationToken = default(CancellationToken), string item = "");
        Task<ResultModel> CreateTokAsync(Shared.Models.AlphaToks.Tok tok, string item = "");
        Task<ResultModel> DeleteTokAsync(string id, string pk);
        Task<ResultModel> UpdateTokAsync(TokModel tok, CancellationToken cancellationToken = default(CancellationToken));
        Task<TokModel> GetTokIdAsync(string id);
        Task<List<TokModel>> GetToksAsync(TokQueryValues values = null, string item = null, string serviceid = null);
        Task<List<TokModel>> GetToksByIdsAsync(List<string> ids);
        Task<ResultData<TokkepediaReaction>> UserReactionsGet(string item_id, string fromCaller = "");
        List<TokModel> AlternateToks(List<TokModel> resultData);

        Task<ResultData<YearbookTok>> GetYearbooksAsync(TokQueryValues values = null, string serviceid = null);

        Task<ResultModel> CreateYearbookAsync(YearbookTok tok);

        Task<YearbookTok> GetYearbookAsync(string id, string pk);

        Task<ResultModel> UpdateYearbookAsync(YearbookTok tok);

        Task<ResultModel> CreateOpportunityAsync(OpportunityTok tok);

        Task<OpportunityTok> GetOpportunityAsync(string id, string pk);

        Task<ResultModel> UpdateOpportunityAsync(OpportunityTok tok);

        Task<ResultData<OpportunityTok>> GetOpportunitiesAsync(TokQueryValues values = null, string serviceid = null);
        void SetUserReactionsCache(string fromCaller, ResultData<TokkepediaReaction> data);

        ResultData<TokkepediaReaction> GetUserReactionsCache(string fromCaller);

    }
}
