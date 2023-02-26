using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class CommonService : ICommonService
    {
        public static ICommonService Instance = new CommonService();
        private HttpClientHelper _httpClientHelper;
        public CommonService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        public async Task<ResultData<TokketUser>> SearchUsersAsync(string text, string token = "")
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("displayname", text);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/users{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ResultData<TokketUser>>(content);
            return data;
        }
        public async Task<ResultData<CategoryModel>> SearchCategoriesAsync(string text, string pageToken = "")
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", pageToken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("name", text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("classtokmode", "true");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/categories{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ResultData<CategoryModel>>(content);
            return data;
        }
        //public async Task<bool> AddRecentSearchAsync(string text)
        //{
        //    var userid = Settings.GetUserModel().UserId;
        //    var idtoken = await SecureStorage.GetAsync("idtoken");
        //    _httpClientHelper.ClearHeaders();
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", text);
        //    var apiUrl = $"{Config.Configurations.ApiPrefix}/searchesaddrecent/{userid}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
        //    var response = await _httpClientHelper.PostAsync(apiUrl, new UserSearches());
        //    bool result = true;
        //    if (response.Contains("error"))
        //    {
        //        result = false;
        //    }

        //    return result;
        //}
        //public async Task<UserSearches> GetRecentSearchesAsync()
        //{
        //    var userid = Settings.GetUserModel().UserId;
        //    var idtoken = await SecureStorage.GetAsync("idtoken");
        //    _httpClientHelper.ClearHeaders();
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
        //    var apiUrl = $"{Config.Configurations.ApiPrefix}/searchesgetrecent/{userid}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
        //    var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
        //    var content = await response.Content.ReadAsStringAsync();
        //    var data = JsonConvert.DeserializeObject<UserSearches>(content);
        //    return data;
        //}
        public async Task<string> UploadImageAsync(string base64)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/image{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(base64), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<string>(content);
            return data;
        }
        public async Task<OggClass> GetQuoteAsync()
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/quoteofthehourtokkepediatok{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<OggClass>(content);
            return data;
        }
    }
}