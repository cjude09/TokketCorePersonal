using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services.Interfaces
{
    public interface ICommonService
    {
        Task<ResultData<TokketUser>> SearchUsersAsync(string text, string token = "");
        Task<ResultData<CategoryModel>> SearchCategoriesAsync(string text, string pageToken = "");
        //Task<bool> AddRecentSearchAsync(string text);
        //Task<UserSearches> GetRecentSearchesAsync();
        Task<string> UploadImageAsync(string base64);
        Task<OggClass> GetQuoteAsync();
    }
}
