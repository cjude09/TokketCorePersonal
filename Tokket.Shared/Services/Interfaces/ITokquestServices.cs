using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
   public interface ITokquestServices
    {
        Task<ResultData<gameObject>> GetGamesets(string classGroup);
        Task<ResultModel> CreateGameset(gameObject gameObject);
        Task<bool> DeleteGamesets(string id, string pk);

        Task<gameObject> GetGameset(string id, string classGroup);
        Task<ResultModel> EditGamesets(gameObject gameObject);
        Task<ClassSet> GetClassSetAsyncTokquest(string id, string pk);
        Task<ClassTok> GetClassTokAsyncTokques(string id, string pk);
    }
}
