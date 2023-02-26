using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Services.Interfaces;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Newtonsoft.Json.Linq;
using Tokket.Infrastructure;
using db = Tokket.Infrastructure;
using Tokket.Shared.Helpers;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Net.Http;
using Tokket.Core;

namespace Tokket.Shared.Services
{
    public class TokHandleService  : ITokHandleService
    {
        //private readonly IApi _api;

        public static ITokHandleService Instance = new TokHandleService();
        private HttpClientHelper _httpClientHelper;
        #region Constructors
        public TokHandleService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
            // _api = api;
        }

        #endregion

        #region Methods and Functions

        public async Task<GetTokHandleResponse> GetTokHandleAsync(GetTokHandleRequest request)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokhandle/{request.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            return JsonConvert.DeserializeObject<GetTokHandleResponse>(response);
        }

        public async Task<GetTokHandlesByCategoryResponse> GetTokHandlesByCategoryAsync(GetTokHandlesByCategoryRequest request)
        {
            throw new NotImplementedException();
            //GetItemRequest itemRequest = new GetItemRequest()
            //{
            //    Id = request.Id,
            //    PartitionKey = request.PartitionKey
            //};
            //var response = await databaseService.GetItemsAsync<TokHandle>(x => x.Category == request.Category, x => x.CreatedTime, null,
            //    new Microsoft.Azure.Cosmos.QueryRequestOptions() { PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey($"{request.PartitionKeyItemsBase}-{request.Category}"), MaxItemCount = 24 }); //pk = tokhandles-category
            //return new GetTokHandlesByCategoryResponse() { Results = response.Results };
        }

        public async Task<GetTokHandlesByUserResponse> GetTokHandlesByUserAsync(GetTokHandlesByUserRequest request)
        {
            throw new NotImplementedException();
            //var response = await databaseService.GetPartitionedItemAsync<TokHandle>(x => x["user_id"].ToString() == request.UserId && x["label"].ToString() == "tokhandle", x => x["_ts"],
            //                $"{request.UserId}-{request.PartitionKeyItemsBase}"); //pk = userid-tokhandles    
            //return new GetTokHandlesByUserResponse() { Results = response.Resource.Results };
        }

        public async Task<SetTokHandlePriceResponse> SetTokHandlePriceAsync(SetTokHandlePriceRequest request)
        {
            throw new NotImplementedException();
            //var item = await _api.PatchItemAsync<TokHandle>(new PatchItemRequest()
            //{
            //    FieldsUpdate = new Dictionary<string, object>()
            //    {
            //        { "price_usd", request.NewPriceUSD }
            //    }
            //});

            //var response = new SetTokHandlePriceResponse() { Item = item.Result };
            //return response;
        }

        public async Task<List<TokkepediaResponse<Dictionary<string, object>>>> CreateTokHandleAsync(CreateTokHandleRequest request)
        {
            throw new NotImplementedException();
            //List<TokkepediaResponse<Dictionary<string, object>>> items = new List<TokkepediaResponse<Dictionary<string, object>>>();

            //TokHandle tokhandle = new TokHandle();
            //string pk = $"{request.UserId}-tokhandles";
            //tokhandle.Id = request.Id;
            //tokhandle.PartitionKey = request.UserId;
            //tokhandle.UserId = request.UserId;
            //Dictionary<string, object> item = JObject.FromObject(tokhandle).ToObject<Dictionary<string, object>>();

            //items.Add(await _api.CreatePartitionedItemAsync(item, request.Id));
            //items.Add(await _api.CreatePartitionedItemAsync(item, pk));
            //return items;
        }

        public async Task<GetTokHandlesByUserResponse> GetTokHandlesByUserAsync(string userid = "")
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokhandlesuser/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            string content = await response.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<GetTokHandlesByUserResponse>(content);
            return results;
        }

        public async Task<ResultModel> UpdateTokHandleAsync(TokHandle tokHandle)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", tokHandle.PartitionKey);
            var json = JsonConvert.SerializeObject(tokHandle); // For Debugging
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokhandle/{tokHandle.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
        }
        #endregion
    }
}
