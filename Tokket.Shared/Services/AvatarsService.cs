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
    public class AvatarsService : IAvatarsService
    {
        public static IAvatarsService Instance = new AvatarsService();
        private HttpClientHelper _httpClientHelper;
        public AvatarsService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        /// <summary>Gets all avatar available for purchase. Used for the Avatar Shop</summary>
        public async Task<ResultData<Avatar>> GetAvatarsAsync(string paginationId = null)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/avatars{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            if (!string.IsNullOrEmpty(paginationId))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", paginationId);

            var response = await _httpClientHelper.Instance.GetAsync(apiUrl).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultData<Avatar>>(content);
            return result;
        }
        public async Task<bool> SelectAvatarAsync(string avatarid)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "" };
            var apiUrl = $"{Config.Configurations.ApiPrefix}/avataruserselect/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(avatarid), Encoding.UTF8, "application/json"));
            var data = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UseAvatarAsProfilePictureAsync(bool isAvatarProfilePicture)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/avataruseasprofilepicture{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(isAvatarProfilePicture), Encoding.UTF8, "application/json"));
            var data = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UserSelectAvatarAsync(string avatarId)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/avataruserselect{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(avatarId), Encoding.UTF8, "application/json"));
            var data = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ResultData<Avatar>> AvatarsByIdsAsync(List<string> ids)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/avatarsbyids{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultData<Avatar>>(content);
            return result;
        }
    }
}