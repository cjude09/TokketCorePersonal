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

namespace Tokket.Shared.Services
{
    public class PatchService : IPatchService
    {
        public static IPatchService Instance = new PatchService();
        private HttpClientHelper _httpClientHelper;
        public PatchService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        public async Task<bool> UpdateUserPointsSymbolEnabledAsync(bool isEnabled, TokketUser User)
        {
            _httpClientHelper.ClearHeaders();

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", User.Id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            if (!string.IsNullOrEmpty(User.SubaccountId))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("subaccount", User.SubaccountId);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/userpointssymbolenabled{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(isEnabled), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}