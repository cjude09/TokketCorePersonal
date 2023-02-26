using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Notification;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class NotificationService : INotificationService
    {
        public static INotificationService Instance = new NotificationService();
        private HttpClientHelper _httpClientHelper;
        public NotificationService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        public async Task<ResultData<TokkepediaNotificationNew>> GetNotif(string id)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/notifications/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var data = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(data);
            var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<ResultData<TokkepediaNotificationNew>>(resource);

            return result;
        }
        public async Task<TokkepediaNotificationSet> GetNotificationsAsync(string id, NotficationQueryValues values = null)
        {
            if (values == null)
                values = new NotficationQueryValues();

            _httpClientHelper.ClearHeaders();

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", values?.pagination_id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("limit", values.limit.ToString());
            var apiUrl = $"{Config.Configurations.ApiPrefix}/notifications/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var data = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<TokkepediaNotificationSet>(data);
            return dataresource;

        }
        //Use the id from TokkepediaNotification
        public async Task<bool> MarkAsReadAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/markasread/{id}/{pk}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }
        //Use the ids from TokkepediaNotification
        public async Task<bool> RemoveNotificationsAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/deletenotification/{id}/{pk}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }
    }
}
