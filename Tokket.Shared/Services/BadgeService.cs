using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class BadgeService : IBadgeService
    {
        public static IBadgeService Instance = new BadgeService();
        private HttpClientHelper _httpClientHelper;
        public BadgeService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        /// <summary>Gets all avatar available for purchase. Used for the Avatar Shop</summary>
        public async Task<ResultData<BadgeOwned>> GetUserBadgesAsync(string userId, string paginationId = null)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/badgesuser/{userId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            if (!string.IsNullOrEmpty(paginationId))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", paginationId);

            var response = await _httpClientHelper.Instance.GetAsync(apiUrl).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();

            var result = new ResultData<BadgeOwned>();
            try
            {
                var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(content);
                var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
                result = JsonConvert.DeserializeObject<ResultData<BadgeOwned>>(resource);
            }
            catch (Exception)
            {
                result.Results = new List<BadgeOwned>();
            }
            return result;

        }
        // Selects badge for use as profile picture
        public async Task<bool> SelectBadgeAsync(string badgeId)
        {
            var result = new ResultModel() { ResultEnum = Tokket.Shared.Helpers.Result.Failed, ResultMessage = "" }; 
            var apiUrl = $"{Config.Configurations.ApiPrefix}/badgeuserselect{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(badgeId), Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateUserBadgeColor(string badgeId, string id, string color = "black")
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("badgeid", badgeId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/userbadgecolor{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(color), Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateUserPointsSymbolColorAsync(string color, string UserId)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", UserId);

            string[] colors = new string[] { "black", "gold", "blue", "green", "orange", "purple", "pink" };
            if (!colors.Contains(color))
                return false;
            var apiUrl = $"{Config.Configurations.BaseUrl}/v1/userpointssymbolcolor{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(color), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
