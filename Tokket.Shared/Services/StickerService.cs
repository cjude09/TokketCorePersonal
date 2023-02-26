using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Xamarin.Essentials;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Purchase;

namespace Tokket.Shared.Services
{
    public class StickerService : IStickerService
    {
        public static IStickerService Instance = new StickerService();
        private HttpClientHelper _httpClientHelper;
        public StickerService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        /// <summary>Gets all stickers available for purchase. Used for the Stickers Shop</summary>
        public async Task<List<Sticker>> GetStickersAsync()
        {
            return StickerHelper.Stickers;

            //var apiUrl = $"{Config.Configurations.ApiPrefix}/stickers{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);
            //var content = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<ResultData<Sticker>>(content);
            //return result.Results;
        }

        /// <summary>Gets all stickers purchased by a user. Used in add/change/remove sticker in a tok, and also the user sticker inventory page</summary>
        public async Task<List<PurchasedSticker>> GetStickersUserAsync(string userId)
        {
            return JsonConvert.DeserializeObject<List<PurchasedSticker>>(JsonConvert.SerializeObject(StickerHelper.Stickers));

            //var apiUrl = $"{Config.Configurations.ApiPrefix}/stickersuser/{userId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);
            //var content = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<ResultData<PurchasedSticker>>(content);
            //return result.Results;
        }

        /// <summary>Adds a tok sticker or changes the existing one. Works as both an add and update function</summary>
        public async Task<bool> AddStickerToTokAsync(string tokId, string newStickerId)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/stickertokupdate/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(newStickerId, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        /// <summary>Removes a tok sticker</summary>
        public async Task<bool> RemoveStickerFromAsync(string tokId, string stickerId)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/stickertokupdate/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(stickerId, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        /// <summary>Purchases a sticker for a user</summary>

        public async Task<ResultModel> PurchaseStickerAsync(string stickerId, string tokId)
        {
            var result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "" };

            _httpClientHelper.ClearHeaders();

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemid", tokId);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/purchasecoins/{stickerId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(stickerId, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(content))
            {
                var purchaseResult = JsonConvert.DeserializeObject<PurchaseResultModel>(content);
                result.ResultEnum = purchaseResult.IsSuccess ? Helpers.Result.Success : Helpers.Result.Failed;
                result.ResultMessage = purchaseResult.Message;
                result.ResultObject = purchaseResult;
            }
            return result;
        }
    }
}