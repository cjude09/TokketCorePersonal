using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface ITokMojiService
    {
        ResultData<Tokmoji> GetCacheTokmojisAsync();
        Task<ResultData<Tokmoji>> GetTokmojisAsync(string paginationId = null);
        Task<ResultModel> PurchaseTokmojiAsync(string tokmojiid, string itemLabel);
    }
}
