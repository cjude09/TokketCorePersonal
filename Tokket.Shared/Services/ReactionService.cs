using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ReactionService : IReactionService
    {
        public static IReactionService Instance = new ReactionService();
        private HttpClientHelper _httpClientHelper;
        public HttpClientHelper _HttpClientHelper
        {
            get { return _httpClientHelper; }
            set { _httpClientHelper = value; }
        }

        public ReactionService()
        {
            try 
            {
                _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
            } 
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif
            }
            }
            public async Task<ResultModel> AddReaction(ReactionModel item)
        {
            _httpClientHelper.ClearHeaders();

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemid", item.ItemId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/reaction{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(item);

            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };
            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
                var resultModel = JsonConvert.DeserializeObject<ResultModel>(content);
                
                result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<ResultModel>(content) : null);
                result.ResultEnum = resultModel.ResultEnum;
            }
            catch (Exception)
            {
                result.ResultMessage = "Failed!";
                result.ResultObject = null;
                result.ResultEnum = Helpers.Result.Failed;
            }
            
            return result;
        }

        public async Task<bool> UpdateReaction(TokkepediaReaction item)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };
            try
            {
                //Settings.GetUserModel().UserId
                _httpClientHelper.ClearHeaders();
                var idtoken = await SecureStorage.GetAsync("idtoken");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);
                var apiUrl = $"{Config.Configurations.ApiPrefix}/reaction/{item.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                var model = JsonConvert.SerializeObject(item);
                var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
                Debug.WriteLine("Response " + response.RequestMessage);
                Debug.WriteLine("Response " + response.IsSuccessStatusCode);
                var content = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResultModel>(content);
            }
            catch (Exception e)
            {
                result.ResultObject = null;
                result.ResultEnum = Helpers.Result.None;
            }

            return result.ResultEnum == Helpers.Result.Success;
        }

        public async Task<bool> DeleteReaction(string id)
        {
   
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/reaction/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var test = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<ResultData<ReactionModel>> GetReactionsAsync(ReactionQueryValues values = null, string fromCaller = "")
        {
            if (values == null)
                values = new ReactionQueryValues();
            _httpClientHelper.ClearHeaders();
            //var idtoken = await SecureStorage.GetAsync("idtoken"); //Did not work in TokInfoViewModel pull to refresh
            var idtoken = "test";
            if (!string.IsNullOrEmpty(idtoken)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            if (!string.IsNullOrEmpty(values?.limit.ToString())) _httpClientHelper.Instance.DefaultRequestHeaders.Add("limit", values?.limit.ToString());
            if (!string.IsNullOrEmpty(values?.kind)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("kind", values?.kind);
            if (!string.IsNullOrEmpty(values?.item_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("item_id", values?.item_id);
            if (!string.IsNullOrEmpty(values?.activity_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("activity_id", values?.activity_id);
            if (!string.IsNullOrEmpty(values?.user_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("user_id", values?.user_id);
            if (!string.IsNullOrEmpty(values?.reaction_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("reaction_id", values?.reaction_id);
            if (!string.IsNullOrEmpty(values?.pagination_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", values?.pagination_id);
            if (!string.IsNullOrEmpty(values?.reaction_total.ToString())) _httpClientHelper.Instance.DefaultRequestHeaders.Add("reaction_total", values?.reaction_total.ToString() ?? "0");
            if (!string.IsNullOrEmpty(values?.detail_number.ToString())) _httpClientHelper.Instance.DefaultRequestHeaders.Add("detail_number", values?.detail_number.ToString() ?? "0");
            if (!string.IsNullOrEmpty(values?.userid)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);

            //Added for getting all a user's likes in a list of comments:
            if (values?.user_likes ?? false) _httpClientHelper.Instance.DefaultRequestHeaders.Add("user_likes", values.user_likes.ToString());
            var apiUrl = $"{Config.Configurations.ApiPrefix}/reactions{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);

            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<ReactionModel>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                var list = data.Results.ToList();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].UserId == "tokket")
                    {
                        list[i].UserPhoto = "/images/tokket.png";
                    }
                }

                if (!string.IsNullOrEmpty(fromCaller))
                {
                    //Cache data
                    SetReactionsCache(fromCaller, data);
                }

                return data;// JsonConvert.DeserializeObject<List<ReactionModel>>(JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SetReactionsCache(string fromCaller, ResultData<ReactionModel> data)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var content = JsonConvert.SerializeObject(data);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public ResultData<ReactionModel> GetReactionsCache(string fromCaller)
        {
            //var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<ReactionModel>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<ReactionModel>>(getCacheData);
            }
            return result;
        }

        public async Task<ReactionValueModel> GetReactionsValueAsync(string id, string fromCaller)
        {
         
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("gemcounter", "true");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("commentcounter", "true");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("viewcounter", "true");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/reactioncounters/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}&gemcounter=true&commentcounter=true&viewcounter=true&serviceid={Config.Configurations.ServiceId}&deviceplatform={Config.Configurations.DevicePlatform}";
            var response = await _httpClientHelper.GetAsync(apiUrl);

            try
            {
                var data = JsonConvert.DeserializeObject<List<object>>(response);

                var gem = JsonConvert.DeserializeObject<GemsModel>(JsonConvert.SerializeObject(data[0]));
                var comments = JsonConvert.DeserializeObject<CommentsModel>(JsonConvert.SerializeObject(data[1]));
                var views = JsonConvert.DeserializeObject<ViewsModel>(JsonConvert.SerializeObject(data[2]));

                ReactionValueModel reactionValueModel = new ReactionValueModel();
                reactionValueModel.GemsModel = gem;
                reactionValueModel.CommentsModel = comments;
                reactionValueModel.ViewsModel = views;

                if (!string.IsNullOrEmpty(fromCaller))
                {
                    SetReactionsValueCache(fromCaller, reactionValueModel);
                }

                return reactionValueModel;
            }
            catch
            {
                return null;
            }
        }

        public void SetReactionsValueCache(string fromCaller, ReactionValueModel data)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var content = JsonConvert.SerializeObject(data);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public ReactionValueModel GetReactionsValueCache(string fromCaller)
        {
            //var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = JsonConvert.DeserializeObject<ReactionValueModel>(getCacheData);
            return result;
        }

        public async Task<List<TokketUserReaction>> GetReactionsUsersAsync(ReactionQueryValues reactionQueryValues)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("item_id", reactionQueryValues.item_id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("detail_number", reactionQueryValues.detail_number.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("kind", reactionQueryValues.kind);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", reactionQueryValues.pagination_id);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/reactionsusers{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);

            try
            {
                //var data = JsonConvert.DeserializeObject<List<TokketUserReaction>>(response);
                //return data;

                var data = JsonConvert.DeserializeObject<ResultData<TokketUserReaction>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                var list = data.Results.ToList();
                return JsonConvert.DeserializeObject<List<TokketUserReaction>>(JsonConvert.SerializeObject(list));
            }
            catch
            {
                return null;
            }
        }

        public async Task<ResultData<ReactionModel>> GetCommentReplyAsync(ReactionQueryValues values = null)
        {
            if (values == null)
                values = new ReactionQueryValues();

            _httpClientHelper.ClearHeaders();
            var idtoken = "test";
            if (!string.IsNullOrEmpty(idtoken)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            if (!string.IsNullOrEmpty(values?.limit.ToString())) _httpClientHelper.Instance.DefaultRequestHeaders.Add("limit", values?.limit.ToString());
            if (!string.IsNullOrEmpty(values?.kind)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("kind", values?.kind);
            if (!string.IsNullOrEmpty(values?.item_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("item_id", values?.item_id);
            if (!string.IsNullOrEmpty(values?.activity_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("activity_id", values?.activity_id);
            if (!string.IsNullOrEmpty(values?.user_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("user_id", values?.user_id);
            if (!string.IsNullOrEmpty(values?.reaction_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("reaction_id", values?.reaction_id);
            if (!string.IsNullOrEmpty(values?.pagination_id)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", values?.pagination_id);
            if (!string.IsNullOrEmpty(values?.detail_number.ToString())) _httpClientHelper.Instance.DefaultRequestHeaders.Add("detail_number", values?.detail_number.ToString() ?? "0");
            if (!string.IsNullOrEmpty(values?.userid)) _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            if (values?.user_likes ?? false) _httpClientHelper.Instance.DefaultRequestHeaders.Add("user_likes", values.user_likes.ToString());


            var apiUrl = $"{Config.Configurations.ApiPrefix}/reactionreplies{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}&serviceid={Config.Configurations.ServiceId}&deviceplatform={Config.Configurations.DevicePlatform}";
            var response = await _httpClientHelper.GetAsync(apiUrl);

            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<ReactionModel>>(response);
                return data;
                //Settings.ContinuationToken = data.ContinuationToken;
                //var list = data.Results.ToList();
                //for (int i = 0; i < list.Count; ++i)
                //{
                //    if (list[i].UserId == "tokket")
                //    {
                //        list[i].UserPhoto = "/images/tokket.png";
                //    }
                //}
                //return JsonConvert.DeserializeObject<List<TokkepediaReaction>>(JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<TokkepediaReaction> GetCommentAsync(string id)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/reactions/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}&serviceid={Config.Configurations.ServiceId}&deviceplatform={Config.Configurations.DevicePlatform}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaReaction>(content.Result);

            return data;
        }

        public async Task<ResultModel> AddReport(Report item)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemid", item.ItemId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/report{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(item);

            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };

            if (string.IsNullOrEmpty(item.OwnerId) || string.IsNullOrEmpty(item.ItemId) || string.IsNullOrEmpty(item.UserId) || string.IsNullOrEmpty(item.ItemLabel))
            {
                result.ResultObject = null;
                result.ResultEnum = Helpers.Result.None;
                result.ResultMessage = "Must contain an owner, item id, item label, and user id.";
                return result;
            }

            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
                var resultModel = JsonConvert.DeserializeObject<ResultModel>(content);

                result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<ResultModel>(content) : null);
                result.ResultEnum = resultModel.ResultEnum;
            }
            catch (Exception)
            {
                result.ResultObject = null;
                result.ResultEnum = Helpers.Result.None;
            }

            return result;
        }

        public async Task<ResultData<TokkepediaReaction>> UserReactionsGet(string item_id, string userid)
        {
            try
            {
                _httpClientHelper.ClearHeaders();
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("item_id", item_id);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
                var apiUrl = $"{Config.Configurations.ApiPrefix}/userreactions/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
                var content = await response.Content.ReadAsStringAsync();
                ResultData<TokkepediaReaction> result = new ResultData<TokkepediaReaction>();
                result = JsonConvert.DeserializeObject<ResultData<TokkepediaReaction>>(content);
                return result;
            }
            catch (Exception e)
            {
                ResultData<TokkepediaReaction> test = new ResultData<TokkepediaReaction>();
                return test;
            }

        }
    }
}