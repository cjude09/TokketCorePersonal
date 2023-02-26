using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.ViewModels;
using Tokket.Core;
using Tokket.Core.Tools;
using Tokket.Shared.Helpers;
using Tokket.Shared.Helpers.Interfaces;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Shared.Services.Interfaces;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class SetService : ISetService
    {
        public static ISetService Instance = new SetService();
        private HttpClientHelper _httpClientHelper;
        public SetService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        public async Task<ResultModel> CreateSetAsync(Set set)
        {
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/set{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", set.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var content = await _httpClientHelper.PostAsync(apiUrl, set);
            var result = JsonConvert.DeserializeObject<ResultModel>(content);
            return result;
        }

        public async Task<Set> GetSetAsync(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/set/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.GetAsync(apiUrl);
            return JsonConvert.DeserializeObject<Set>(content);
        }

        public async Task<ResultModel> UpdateSetAsync(Set set)
        {
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/set/{set.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", set.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var content = await _httpClientHelper.PutAsync(apiUrl, set);

            ResultModel result = new ResultModel();
            result.ResultEnum = Helpers.Result.Success;
            return result;
        }

        public async Task<ResultModel> DeleteSetAsync(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/set/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.DeleteAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<ResultModel>(content);
            return result;
        }

        public async Task<ResultData<Set>> GetSetsAsync(SetQueryValues values = null)
        {
            if (values == null)
                values = new SetQueryValues();
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values.offset.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/sets{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.GetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<ResultData<Set>>(content);
            return result;

        }

        public async Task<bool> AddTokToSetAsync(string setId, string setUserId, string tokId)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/setaddtok/{setId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", setUserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var content = await _httpClientHelper.PutAsync(apiUrl, tokId);
            //var result = JsonConvert.DeserializeObject<bool>(content);
            return true;
        }

        public async Task<bool> DeleteTokFromSetAsync(string setId, string setUserId, string tokId)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/setdeletetok/{setId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.PutAsync(apiUrl, tokId);
            //var result = JsonConvert.DeserializeObject<bool>(content);
            return true;
        }

        public async Task<bool> AddToksToSetAsync(string setId, string setUserId, string[] tokIds)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/setaddtoks/{setId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", setUserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var content = await _httpClientHelper.PutAsync(apiUrl, tokIds);
            //var result = JsonConvert.DeserializeObject<bool>(content);
            return true;
        }

        public async Task<bool> DeleteToksFromSetAsync(Set set, string[] tokIds)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/setdeletetoks/{set.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", set.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var content = await _httpClientHelper.PutAsync(apiUrl, tokIds);
            //var result = JsonConvert.DeserializeObject<bool>(content);
            return true;
        }
        public async Task<TokModel> GetTokAsync(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.GetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<TokModel>(content);
            return result;
        }

        #region Game Sets 
        public async Task<ResultModel> CreateGameSetAsync(GameSet set)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", set.UserId);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/gameset{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var json = JsonConvert.SerializeObject(set);
            var content = await _httpClientHelper.PostAsync(apiUrl, set);
            var result = JsonConvert.DeserializeObject<ResultModel>(content);
            return result;
        }

        //public async Task<ResultData<Set>> GetGameSetsAsync(SetQueryValues values = null)
        //{
        //    if (values == null)
        //        values = new SetQueryValues();

        //    _httpClientHelper.ClearHeaders();

        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values.offset.ToString());
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
        //    _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
        //    var apiUrl = $"{Config.Configurations.ApiPrefix}/gamesets{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
        //    var content = await _httpClientHelper.GetAsync(apiUrl);
        //    var result = JsonConvert.DeserializeObject<ResultData<Set>>(content);
        //    return result;
        //}

        public async Task<ResultData<GameSet>> GetGameSetsAsync(SetQueryValues values = null)
        {
            if (values == null)
                values = new SetQueryValues();

            _httpClientHelper.ClearHeaders();

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values.offset.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("gamename", values?.gamename);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/gamesets{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.GetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<ResultModel>(content);
            return JsonConvert.DeserializeObject<ResultData<GameSet>>(result.ResultObject.ToString()) ?? new ResultData<GameSet>();
        }

        public async Task<GameSet> GetGameSetAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();

            var apiUrl = $"{Config.Configurations.ApiPrefix}/gamesets{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var content = await _httpClientHelper.GetAsync(apiUrl);
            var result = JsonConvert.DeserializeObject<GameSet>(content);
            return result;
        }
        #endregion
    }
}
