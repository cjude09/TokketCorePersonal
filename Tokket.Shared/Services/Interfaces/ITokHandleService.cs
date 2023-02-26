using Tokket.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models;
using System.Threading.Tasks;
using Tokket.Core;

namespace Tokket.Shared.Services.Interfaces
{
    public interface ITokHandleService
    {
        Task<GetTokHandleResponse> GetTokHandleAsync(GetTokHandleRequest request);
        Task<GetTokHandlesByUserResponse> GetTokHandlesByUserAsync(GetTokHandlesByUserRequest request);
        Task<GetTokHandlesByCategoryResponse> GetTokHandlesByCategoryAsync(GetTokHandlesByCategoryRequest request);
        Task<SetTokHandlePriceResponse> SetTokHandlePriceAsync(SetTokHandlePriceRequest request);
        Task<List<TokkepediaResponse<Dictionary<string, object>>>> CreateTokHandleAsync(CreateTokHandleRequest request);
        Task<ResultModel> UpdateTokHandleAsync(TokHandle item);
        Task<GetTokHandlesByUserResponse> GetTokHandlesByUserAsync(string userid = "");
    }

    public class SetTokHandlePriceRequest
    {
        public string Id { get; set; }

        public double NewPriceUSD { get; set; }
    }

    public class SetTokHandlePriceResponse
    {
        public TokHandle Item { get; set; }
    }

    public class GetTokHandlesBaseRequest
    {
        public string PartitionKeyItemsBase { get; set; } = "tokhandles";
    }

    public class GetTokHandlesByCategoryRequest : GetTokHandlesBaseRequest
    {
        public GetTokHandlesByCategoryRequest(string category)
        {
            Category = category;
        }

        public string Category { get; set; }
    }

    public class GetTokHandlesByCategoryResponse
    {
        public IEnumerable<TokHandle> Results { get; set; }
    }

    public class GetTokHandlesByUserRequest : GetTokHandlesBaseRequest
    {
        public GetTokHandlesByUserRequest(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }

    public class GetTokHandlesByUserResponse
    {
        public IEnumerable<TokHandle> Results { get; set; }
    }

    public class GetTokHandleRequest
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
    }

    public class GetTokHandleResponse
    {
        public PartitionManager Result { get; set; }
    }
    public class CreateTokHandleRequest
    {
        public CreateTokHandleRequest(string userId, string id)
        {
            UserId = userId;
            Id = id;
        }
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}
