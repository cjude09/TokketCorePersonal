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
using Tokket.Shared.Helpers.Interfaces;
using System.Globalization;
using Xamarin.Essentials;
using Tokket.Core.Tools;

namespace Tokket.Shared.Services
{
    public class AccessoriesService : IAccessoriesService
    {
        public static IAccessoriesService Instance = new AccessoriesService();
        private HttpClientHelper _httpClientHelper;
        public AccessoriesService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        //Gets or updates (depending on the method) the default color for a certain class (i.e ("key", "tok_type"), ("value", "GLG101"))
        public async Task<DefaultColor> DefaultColorAsync(string colorHex, string userId, string keyvalue, string key = "tok_type", string method = "get")
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("method", method); //use "post" to update
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("key", key); //i.e. "tok_type"
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("value", keyvalue); //i.e. "GLG101", case sensitive
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/defaultcolor{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(colorHex, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<ResultModel>(content);
            if (item != null && item.ResultObject != null )
                return JsonConvert.DeserializeObject<DefaultColor>(item.ResultObject.ToString());
            else
                return null;
        }

        public async Task<bool> UpdateColorAsync(string colorHex, string id)
        {
//            client.DefaultRequestHeaders.Add("userid", id);
//            client.DefaultRequestHeaders.Add("token", token);
//            var apiUrl = $"{_apiSettings.BaseUrl}{_apiSettings.ApiPrefix}/color/{id}{_apiSettings.CodePrefix}{_apiSettings.ApiKey}";
//            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, colorHex);
//​
//            return response.IsSuccessStatusCode;
            throw new NotImplementedException();
        }
    }
}
