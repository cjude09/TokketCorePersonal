using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Tokket.Core;

namespace Tokket.Shared.Services
{
    public class TokPakService : ITokPakService
    {
        public static ITokPakService Instance = new TokPakService();
        private HttpClientHelper _httpClientHelper;
        public TokPakService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        public async Task<TokPak> AddTokPakAsync(TokPak item)
        {
            item.Label = "tokpak";
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token",idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("public", item.IncludeInPublic.ToString().ToLower() ?? "false");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokpak{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var json = JsonConvert.SerializeObject(item);
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<TokPak>>(content);

            return data.Resource;
        }

        public async Task<bool> DeleteTokPakAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetTokketUser().Id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokpak/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var test = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<TokPak> GetTokPakAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokpak/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();

            var data = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<TokPak>>(data);
            return dataresource.Resource;
        }

        public async Task<ResultData<TokPak>> GetTokPaksAsync(TokPakQueryValues queryValues, string fromCaller = "")
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("groupid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token",idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupid", queryValues.groupid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", queryValues.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("paginationid", queryValues?.paginationid ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokpaktype", queryValues?.tokpaktype ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("public", queryValues?.publicfeed.ToString().ToLower() ?? "false");

            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokpaks/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(content);
            if (data != null)
            {
                var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);

                if (!string.IsNullOrEmpty(fromCaller))
                {
                    _httpClientHelper.SetCachedAsync<string>(fromCaller, resource);
                }

                var result = JsonConvert.DeserializeObject<ResultData<TokPak>>(resource);
                return result;
            }
            else {
                return null;
            }
           
        }
        public ResultData<TokPak> GetCacheTokPaksAsync(string fromCaller)
        {
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<TokPak>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<TokPak>>(getCacheData);
            }
            return result;
        }
        public void SetCacheTokPaksAsync(string fromCaller, List<TokPak> tokPakList)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var resultData = new ResultData<TokPak>();
                resultData.Limit = tokPakList.Count;
                resultData.ContinuationToken = null;
                resultData.Results = tokPakList;

                var content = JsonConvert.SerializeObject(resultData);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task<TokPak> UpdateTokPakAsync(TokPak item)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
            var json = JsonConvert.SerializeObject(item);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tokpak/{item.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<TokPak>>(content);
            return dataresource.Resource;
        }
    }
}