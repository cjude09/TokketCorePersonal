using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface ITokPakService
    {
        Task<TokPak> AddTokPakAsync(TokPak item);
        Task<TokPak> UpdateTokPakAsync(TokPak item);
        Task<bool> DeleteTokPakAsync(string id, string pk);
        Task<TokPak> GetTokPakAsync(string id, string pk);
        Task<ResultData<TokPak>> GetTokPaksAsync(TokPakQueryValues queryValues, string fromCaller = "");
        ResultData<TokPak> GetCacheTokPaksAsync(string fromCaller);
        void SetCacheTokPaksAsync(string fromCaller, List<TokPak> tokPakList);
    }
}
