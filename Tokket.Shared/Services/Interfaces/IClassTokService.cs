using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Services.ServicesDB;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IClassTokService
    {
        Task<GetClassToksResponse<T>> GetClassToksAsync<T>(GetClassToksRequest request);
        Task<ClassToksItemResponse<T>> GetClassTokAsync<T>(string id, string pk);
        Task<ClassToksItemResponse<T>> AddClassTokAsync<T>(ClassTokModel tok);

        Task<ClassToksItemResponse<T>> UpdateClassTokAsync<T>(ClassTokModel classtok);

        Task<ClassToksItemResponse<T>> DeleteClassTokAsync<T>(string id, string pk);

        Task<FilterByResponse<T>> FilterBy<T>(ClassTokQueryValues queryValues);
    }
}
