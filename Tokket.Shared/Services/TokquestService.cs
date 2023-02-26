using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Extensions.Http;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Tokket.Core;

namespace Tokket.Shared.Services
{
    public class TokquestService : ITokquestServices
    {
        public static ITokquestServices Instance = new TokquestService();
        private HttpClientHelper _httpClientHelper;
     
        public TokquestService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        public async Task<ResultModel> CreateGameset(gameObject gameObject)
        {
            var result = new ResultModel();
            result.ResultEnum = Helpers.Result.Failed;
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemtotal", "-1");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("private", "true");
           _httpClientHelper.Instance.DefaultRequestHeaders.Add("group", gameObject.IsGroup.ToString().ToLower());
           _httpClientHelper.Instance.DefaultRequestHeaders.Add("public", gameObject.IsPublic.ToString().ToLower());
           _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", gameObject.UserId);

            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/gameset{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(gameObject), Encoding.UTF8, "application/json"));

            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var resultContent = JsonConvert.DeserializeObject<ResultModel>(content);
                result.ResultEnum = resultContent.ResultEnum;
                result.ResultMessage = resultContent.ResultMessage;
                result.ResultObject = resultContent.ResultObject;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                result.ResultEnum = Helpers.Result.Failed;
                result.ResultMessage = "cancelled";
            }

            return result;
        }

        public Task<bool> DeleteGamesets(string id, string pk)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> EditGamesets(gameObject gameObject)
        {
            throw new NotImplementedException();
        }

        public async Task<gameObject> GetGameset(string id, string classGroup)
        {
            HttpClient _httpClient = new HttpClient();

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClient.DefaultRequestHeaders.Add("pk", classGroup);
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform", "web");

            _httpClient.BaseAddress = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/gameset/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");
            HttpResponseMessage responseGet = await _httpClient.GetAsync(_httpClient.BaseAddress);
            if (responseGet.IsSuccessStatusCode)
            {
                var data = await responseGet.Content.ReadAsAsync<gameObject>();
                return data;
            }
            else
            {

                return null;
            }
        }

        public async Task<ResultData<gameObject>> GetGamesets(string classGroup)
        {

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("classgroup", classGroup);
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform","web");
            _httpClient.BaseAddress = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/gamesets{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");
            HttpResponseMessage responseGet = await _httpClient.GetAsync(_httpClient.BaseAddress);


            ResultData<gameObject> data;
            if (responseGet.IsSuccessStatusCode)
            {
                data = await responseGet.Content.ReadAsAsync<ResultData<gameObject>>();
                return data;
            }
            else {

                data = null;
                return data;
            
            
            }

        }

        public async Task<ClassSet> GetClassSetAsyncTokquest(string id, string pk)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/classset/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClient.GetAsync(apiUrl);
            var data = await response.Content.ReadAsAsync<TokkepediaResponse<ClassSet>>();
            return data.Resource;

        }

        public async Task<ClassTok> GetClassTokAsyncTokques(string id, string pk)
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/classtok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClient.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<ClassTok>>(content);
            return data.Resource;
        }
    }
}
