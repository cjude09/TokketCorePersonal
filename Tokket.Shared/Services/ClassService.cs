using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Extensions.Http;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services.Interfaces;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Tokket.Shared.Models.Tokquest;
using Newtonsoft.Json.Linq;

namespace Tokket.Shared.Services
{
    public class ClassService : IClassService
    {
        public static IClassService Instance = new ClassService();
        private HttpClientHelper _httpClientHelper;
        public ClassService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        #region  Class Group
        public async Task<ClassGroupModel> AddClassGroupAsync(ClassGroupModel item)
        {
            ClassGroupModel result = null;
            item.Label = "classgroup";
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroup{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(item);
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var data = await response.Content.ReadAsStringAsync();
            try
            {
                var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(data);
                var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);

                //Add trapping in case the return user_id is null, set the new value using the user's id
                var parseResponse = JToken.Parse(resource);
                var selectedPath = parseResponse.SelectToken("$.user_id");
                if (selectedPath != null)
                {
                    selectedPath.Replace(item.UserId);
                }

                var newSource = JsonConvert.SerializeObject(parseResponse, Formatting.Indented);

                result = JsonConvert.DeserializeObject<ClassGroupModel>(newSource);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public async Task<bool> UpdateClassGroupAsync(ClassGroupModel item)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroup/{item.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(item);
            var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClassGroupAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroup/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var test = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public void SetCacheGroupGroupAsync(string fromCaller, List<ClassGroupModel> list)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var resultData = new ResultData<ClassGroupModel>();
                resultData.Limit = list.Count;
                resultData.ContinuationToken = null;
                resultData.Results = list;

                var content = JsonConvert.SerializeObject(resultData);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task<ClassGroupModel> GetClassGroupAsync(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroup/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var data = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(data);
            var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<ClassGroupModel>(resource);
            return result;
        }
        public ResultData<ClassGroupModel> GetCachedClassGroupAsync(string fromCaller)
        {
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<ClassGroupModel>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<ClassGroupModel>>(getCacheData);
            }
            return result;
        }
        public async Task<ResultData<ClassGroupModel>> GetClassGroupAsync(ClassGroupQueryValues queryValues, string fromCaller = "")
        {
         
            //var idToken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("text");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("joined");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("paginationid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("showimage");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("isdescending");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("group_kind");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", queryValues.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", queryValues.text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("joined", queryValues.joined.ToString().ToLower() == "true" ? "true" : "");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("paginationid", queryValues.paginationid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("showimage", queryValues.showImage?.ToString() ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("isdescending", queryValues.isDescending?.ToString() ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("group_kind", queryValues.groupkind?.ToString() ?? string.Empty);


            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroups{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            try
            {
                var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
                var data = await response.Content.ReadAsStringAsync();
                //var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(data);
                //var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
                var result = JsonConvert.DeserializeObject<ResultData<ClassGroupModel>>(data);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(fromCaller))
                    {
                        _httpClientHelper.SetCachedAsync<string>(fromCaller, data);
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
                var data = await response.Content.ReadAsStringAsync();
                //var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(data);
                //var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
                var result = JsonConvert.DeserializeObject<ResultData<ClassGroupModel>>(data);
                return result;
            }

        }


        public async Task<ResultData<CommonModel>> GetMoreFilterOptions(ClassTokQueryValues queryValues, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resultData = new ResultData<CommonModel>();
            _httpClientHelper.ClearHeaders();
            try {
                if (queryValues.FilterBy == FilterBy.Type)
                {
                    resultData.Limit = 3;
                    resultData.Results = new List<CommonModel>() {
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Basic", Description = "Basic" },
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Detailed", Description = "Detailed" },
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "List", Description = "List" },
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Mega", Description = "Mega" },
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Pic", Description = "Pic" },
                    new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Q&A", Description = "Q&A" },};


                    /* return new List<CommonModel>() {
                         new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Basic", Description = "Basic" },
                         new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Detailed", Description = "Detailed" },
                         new CommonModel() { LabelIdentifier = "classtokgroup", Title = "Mega", Description = "Mega" }
                     };*/
                }
                else
                {
                    var userid = Settings.GetUserModel().UserId;
                    var idtoken = await SecureStorage.GetAsync("idtoken");
                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", queryValues?.paginationid);
                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("filterby", queryValues?.FilterBy.ToString());
                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("recent", queryValues?.RecentOnly.ToString().ToLower());
                    var apiUrl = $"{Config.Configurations.ApiPrefix}/filterby/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                    HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl, cancellationToken);
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResultModel>(content);

                    //TODO Use temporarily until continuationtoken is ready in api
                    var rData = result.ResultObject != null ? JsonConvert.DeserializeObject<ResultData<CommonModel>>(result.ResultObject.ToString()) : new ResultData<CommonModel>();

                    resultData.Results = rData.Results;
                    resultData.ContinuationToken = rData.ContinuationToken;

                    /*var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(content);
                    var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);
                    var result = JsonConvert.DeserializeObject<ResultData<CommonModel>>(resource);*/
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
            }
       
        

            return resultData;
        }
        #endregion


        #region Class Group Request

        public async Task<ClassGroupRequestModel> GetClassGroupRequestAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequest/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);

            var data = await response.Content.ReadAsStringAsync();

            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(data);
            var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<ClassGroupRequestModel>(resource);
            return result;
        }

        public async Task<ClassGroupRequestModel> RequestClassGroupAsync(ClassGroupRequestModel item)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequest{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var modelConvert = JsonConvert.SerializeObject(item);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(modelConvert, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            try {
                var data = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(content);
                var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);
                var result = JsonConvert.DeserializeObject<ClassGroupRequestModel>(resource);
                return result;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
         
           
        }

        public async Task<ResultData<ClassGroupRequestModel>> GetClassGroupJoinRequests(string continuationtoken, string groupid, RequestStatus requestStatus = RequestStatus.Pending)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("continuationtoken", continuationtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", "classgrouprequests");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupid", groupid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("status", ((int)requestStatus).ToString());
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequests/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<Dictionary<string, object>>>(content);
            var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<ResultData<ClassGroupRequestModel>>(resource);
            return result;
        }

        public async Task<ResultData<ClassGroupRequestModel>> GetClassGroupRequestAsync(ClassGroupRequestQueryValues queryValues)
        {
       

            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("receiverid", queryValues.receiverid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("status", queryValues.status.ToString());

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequests/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(content);
            var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<ResultData<ClassGroupRequestModel>>(resource);
            return result;

        }

        public async Task<ResultData<ClassGroupRequestModel>> GetClassGroupInvites(ClassGroupRequestQueryValues queryValues)
        {
            var idToken = await SecureStorage.GetAsync("idtoken");
            var userid = Settings.GetUserModel().UserId;
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idToken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("status", queryValues.status.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("receiverid", queryValues.receiverid);


            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequests/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(content);
            var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);

            var result = JsonConvert.DeserializeObject<ResultData<ClassGroupRequestModel>>(resource);
            return result;
        }

        public async Task<bool> AcceptRequest(string id, string pk, ClassGroupModel model)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            var userid = Settings.GetUserModel().UserId;
            var idtoken = Settings.GetUserModel().IdToken; 
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequestaccept/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var modelConvert = JsonConvert.SerializeObject(model);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(modelConvert, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeclineRequest(string id, string pk, ClassGroupModel model)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgrouprequestdecline/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var modelConvert = JsonConvert.SerializeObject(model);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(modelConvert, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveClassGroupAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroupleave/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, id);
            if (response.Contains("200"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RemoveMemberClassGroupAsync(string id, string pk, ClassGroupModel groupModel)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroupleave/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var group = JsonConvert.SerializeObject(groupModel);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(group, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<ResultData<TokketUser>> GetGroupMembers(string groupid, string paginationid = "")
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", paginationid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", groupid + "-classgroupmembers");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classgroupmembers/{groupid}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            try
            {
                var response = await _httpClientHelper.Instance.GetAsync(apiUrl);

                var data = await response.Content.ReadAsStringAsync();
                var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(data);
                var resource = JsonConvert.SerializeObject(dataresource.Resource, Formatting.Indented);
                var result = JsonConvert.DeserializeObject<ResultData<TokketUser>>(resource);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed GetGroupMembers" + ex.ToString());
                var list = new List<TokketUser>();
                var resultData = new ResultData<TokketUser>();
                resultData.Results = list;
                return resultData;
            }
        }



        #endregion


        #region Class Set
        public async Task<bool> AddClassSetAsync(ClassSetModel item, CancellationToken cancellationToken = default(CancellationToken))
        {
            bool isSuccess = true;
            if(!item.Label.Equals("superset"))
                item.Label = "classset";
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("group");

            if (!string.IsNullOrEmpty(item.GroupId))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("group", "true");
            }

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classset{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(item);
            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"), cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                isSuccess = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return isSuccess;
        }

        public async Task<bool> UpdateClassSetAsync(ClassSetModel item, CancellationToken cancellationToken = default(CancellationToken))
        {
            bool isSuccess = true;
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classset/{item.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(item);
            try
            {
                var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
                isSuccess = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        public async Task<bool> DeleteClassSetAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classset/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var test = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<ClassSetModel> GetClassSetAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classset/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);

            var data = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<TokkepediaResponse<ClassSetModel>>(data);
            return dataresource.Resource;
        }

        public ResultData<ClassSetModel> GetCacheClassSetAsync(string fromCaller)
        {
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<ClassSetModel>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<ClassSetModel>>(getCacheData);
            }
            return result;
        }

        public async Task<ResultData<ClassSetModel>> GetClassSetAsync(ClassSetQueryValues queryValues, CancellationToken cancellationToken = default(CancellationToken), string fromCaller = "")
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", queryValues.userid ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupid", queryValues.groupid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("paginationid", queryValues.paginationid ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("label", queryValues.label);

            if (!string.IsNullOrEmpty(queryValues.name))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("name", queryValues.name ?? string.Empty);

            if (!string.IsNullOrEmpty(queryValues.category))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("category", queryValues.category ?? string.Empty);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classsets/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            ResultData<ClassSetModel> result = new ResultData<ClassSetModel>();
            try
            {
                var response = await _httpClientHelper.Instance.GetAsync(apiUrl, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(content);
                var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);

                result = JsonConvert.DeserializeObject<ResultData<ClassSetModel>>(resource);

                if (!string.IsNullOrEmpty(fromCaller))
                {
                    _httpClientHelper.SetCachedAsync<string>(fromCaller, resource);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ContinuationToken = "cancelled";
            }

            return result;
        }

        public void SetCacheClassSetAsync(string fromCaller, List<ClassSetModel> classSetList)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var resultData = new ResultData<ClassSetModel>();
                resultData.Limit = classSetList.Count;
                resultData.ContinuationToken = null;
                resultData.Results = classSetList;

                var content = JsonConvert.SerializeObject(resultData);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion


        #region Class Tok

        public async Task<ResultModel> AddClassToksAsync(ClassTokModel item, CancellationToken cancellationToken = default(CancellationToken))
        {
            //item.Label = "classtok";
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Creating tok failed." };
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtok{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            _httpClientHelper.ClearHeaders();

            //Check if categoryId is classtoksCategory
            string categoryId = item.CategoryId;
            var categorySplit = item.CategoryId.Split('-');
            if (categorySplit.Length > 0)
            {
                if (categorySplit[0].ToLower() != "classtokscategory")
                {
                    categoryId = "classtokscategory-" + item.Category?.ToIdFormat();
                }
            }

            var idtoken = await SecureStorage.GetAsync("idtoken") ?? "test";
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokgroupid", item.TokGroup.ToIdFormat());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", item.TokTypeId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", categoryId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("private", item.IsPrivate.ToString().ToLower());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("group", item.IsGroup.ToString().ToLower());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("public", item.IsPublic.ToString().ToLower());
            var model = JsonConvert.SerializeObject(item);
            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"), cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                var resultContent = JsonConvert.DeserializeObject<ResultModel>(content);
                result.ResultEnum = resultContent.ResultEnum;
                result.ResultMessage = resultContent.ResultMessage;
                result.ResultObject = resultContent.ResultObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                result.ResultEnum = Helpers.Result.Failed;
                result.ResultMessage = "cancelled";
            }

            return result;

            ////var model = JsonConvert.SerializeObject(item);
            ////var secondaryVal = new StringContent(model, Encoding.UTF8, "application/json");
            //var response = await _httpClientHelper.PostAsync(apiUrl, item);
            //result = JsonConvert.DeserializeObject<ResultModel>(response);
            //if (result == null)
            //{
            //    result = new ResultModel();
            //    result.ResultEnum = Helpers.Result.Success;
            //    result.ResultMessage = "Created tok successfully!";
            //}
            ////var content = await response.Content.ReadAsStringAsync();

            ////if (response.IsSuccessStatusCode)
            ////{
            ////    if (!string.IsNullOrEmpty(content))
            ////    {
            ////        result.ResultEnum = Helpers.Result.Success;
            ////        result.ResultMessage = "Create Successful!";
            ////        result.ResultObject = JsonConvert.DeserializeObject<ClassTokModel>(content);
            ////    }
            ////}
            //return result;
        }

        public async Task<ResultModel> AddClassSetsToGroupAsync(string groupId, string pk, List<string> classsetIds)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Unable to Add Class Set(s) to the Group!" };
            var idtoken = await SecureStorage.GetAsync("idtoken") ?? "test";
            var userid = await SecureStorage.GetAsync("userid");

            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            if (classsetIds.Count > 0)
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("setids", JsonConvert.SerializeObject(classsetIds)); // Pk is included = Id*pk = Where * is a separator
                var apiUrl = $"{Config.Configurations.ApiPrefix}/classsetstogroup/{groupId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                //HttpResponseMessage response = await PostAsJsonAsync(apiUrl, "");
                var response = await _httpClientHelper.Instance.PostAsJsonAsync(apiUrl, "");
                var content = await response.Content.ReadAsStringAsync();
                var resultContent = JsonConvert.DeserializeObject<ResultModel>(content);
                result.ResultEnum = resultContent.ResultEnum;
                result.ResultMessage = resultContent.ResultMessage;
                result.ResultObject = resultContent.ResultObject;

                //var content = await response.Content.ReadAsStringAsync();
                //result = JsonConvert.DeserializeObject<ResultViewModel>(content);
                return result;

            }
            else {
                result.ResultMessage = "No Class Set(s) to add.";
                return result;

            }
        }

        public async Task<ResultModel> UpdateClassToksAsync(ClassTokModel item, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Class Tok Update Failed." };

            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", item.PartitionKey);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtok/{item.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(item);

            try
            {
                var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"), cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                var resultContent = JsonConvert.DeserializeObject<ResultModel>(content);
                if (resultContent != null)
                {
                    result.ResultEnum = resultContent.ResultEnum;
                    result.ResultMessage = resultContent.ResultMessage;
                    result.ResultObject = resultContent.ResultObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                result.ResultEnum = Helpers.Result.Failed;
                result.ResultMessage = "cancelled";
            }

            return result;
        }

        public async Task<bool> DeleteClassToksAsync(string id, string pk)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            var userid = Settings.GetTokketUser().Id;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            // _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var test = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<ClassTokModel> GetClassTokAsync(string id, string pk)
        {
            #region HTTP GetClassTok code
            //_httpClientHelper.ClearHeaders();
            //_httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            //_httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
            //var userid = Settings.GetTokketUser().Id;
            //var idtoken = await SecureStorage.GetAsync("idtoken");
            //// _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            //var apiUrl = $"{Config.Configurations.ApiPrefix}/classtok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            //var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            //var content = await response.Content.ReadAsStringAsync();

            //var data = JsonConvert.DeserializeObject<TokkepediaResponse<ClassTokModel>>(content);
            #endregion
            ClassTokModel data = new ClassTokModel();
            try
            {
                //ClasstokserviceDB GetClasstoks function
                //Add try catch due to error 400 crash
                //GetClassToksRequest request = new ServiceAccount.ServicesDB.GetClassToksRequest()
                //{
                //    QueryValues = queryValues
                //};
                var getclasstoksDB = await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.Interfaces.IClassTokService>().GetClassTokAsync<ClassTokModel>(id,pk);
                data = JsonConvert.DeserializeObject<ClassTokModel>(JsonConvert.SerializeObject(getclasstoksDB.Result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return data;
        }

        public async Task<ResultData<ClassTokModel>> GetClassToksAsync(ClassTokQueryValues queryValues, CancellationToken token = default(CancellationToken), string fromCaller = "")
        {
            _httpClientHelper.ClearHeaders();


            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("groupid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("level1");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("level2");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("level3");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");

            //var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");

            //var checkToken = await AccountService.Instance.VerifyTokenAsync(idtoken,);
            // _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("level1", queryValues?.level1 ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("level2", queryValues?.level2 ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("level3", queryValues?.level3 ?? string.Empty);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues?.partitionkeybase ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupid", queryValues?.groupid ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("public", queryValues?.publicfeed.ToString().ToLower());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("paginationid", queryValues?.paginationid ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("classtokmode", queryValues?.classtokmode.ToString().ToLower());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("classsetid", queryValues?.classsetid ?? string.Empty);
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("category", queryValues?.category ?? string.Empty);

            if (!string.IsNullOrEmpty(queryValues?.userid))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", queryValues?.userid.ToString() ?? string.Empty);
            }

            if (!string.IsNullOrEmpty(queryValues?.searchvalue))
            {
                queryValues.searchkey = "primary_text"; // Currently the Value is always primary_text. To be changed in the future. 
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("searchkey", queryValues?.searchkey ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("searchvalue", queryValues?.searchvalue ?? string.Empty);
            }
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("filterby", queryValues?.FilterBy.ToString() ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("filteritems", JsonConvert.SerializeObject(queryValues?.FilterItems ?? new List<string>()));
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", queryValues?.text?.ToString() ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("orderby", queryValues?.orderby ?? string.Empty);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("descending", queryValues?.descending.ToString().ToLower());

            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            ResultData<ClassTokModel> result = new ResultData<ClassTokModel>();

            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl, token);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokkepediaResponse<ResultData<Dictionary<string, object>>>>(content);
            var resource = JsonConvert.SerializeObject(data.Resource, Formatting.Indented);

            result = JsonConvert.DeserializeObject<ResultData<ClassTokModel>>(resource);

            if (!string.IsNullOrEmpty(fromCaller))
            {
                _httpClientHelper.SetCachedAsync<string>(fromCaller, resource);
            }

            //try
            //{
               
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    result = new ResultData<ClassTokModel>();
            //    result.ContinuationToken = "cancelled";
            //}

            return result;
        }

        public string AddNewToksFoundCache(string fromCaller, string resource)
        {
            _httpClientHelper.SetCachedAsync<string>(fromCaller, resource);
            return "";
        }

        public ResultData<ClassTokModel> GetCacheClassToksAsync(string fromCaller)
        {
            //var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            //use fromCaller (name of activity of fragment where this is called) and not apiUrl in case this is called in different activities
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<ClassTokModel>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<ClassTokModel>>(getCacheData);
            }
            return result;
        }
        
        public void SetCacheClassToksAsync(string fromCaller, List<ClassTokModel> list)
        {
            try
            {
                Barrel.Current.Empty(fromCaller);

                var resultData = new ResultData<ClassTokModel>();
                resultData.Limit = list.Count;
                resultData.ContinuationToken = null;
                resultData.Results = list;

                var content = JsonConvert.SerializeObject(resultData);

                _httpClientHelper.SetCachedAsync<string>(fromCaller, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<bool> AddClassToksToClassSetAsync(string classsetId, string pk, List<string> classtokIds, List<string> classtokPks)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokids", JsonConvert.SerializeObject(classtokIds));
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokpks", JsonConvert.SerializeObject(classtokPks));
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtokstoset/{classsetId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(apiUrl,"");
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClassToksFromClassSetAsync(ClassSetModel classset, List<string> classtokIds)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", classset.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokids", JsonConvert.SerializeObject(classtokIds));
            var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoksfromset/{classset.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TokModel>> GetClassGroupDocs(ClassGroupModel classGroup, string paginationId = null)
        {
            var idtoken = await SecureStorage.GetAsync("idtoken");
            TokQueryValues queryValues = new TokQueryValues();
            queryValues.tokgroup = "Basic";
            queryValues.category = "Document";
            queryValues.token = "";
            queryValues.toktype = classGroup.Name;
            queryValues.groupid = classGroup.Id;
            queryValues.loadmore = "yes";
            queryValues.token = paginationId;

            var docs = await TokService.Instance.GetToksAsync(queryValues, "tokdocs");
            if (docs == null)
            {
                docs = new List<TokModel>();
            }
            return docs;
        }

        public async Task UploadDocsToGroup(ClassGroupModel classGroup, List<FileModel> fileList)
        {
            var user = Settings.GetTokketUser();
            var uploadTask = new List<Task>();
            foreach (var file in fileList)
            {
                TokModel tok = new TokModel();
                tok.PrimaryFieldText = file.FileName;
                tok.SecondaryFieldText = user.DisplayName;
                tok.TokGroup = "Basic";
                tok.IsDetailBased = false;
                tok.IsMegaTok = false;
                tok.Category = "Document";
                tok.TokType = classGroup.Name;
                tok.GroupId = classGroup.Id;
                tok.UserPhoto = user.UserPhoto;
                tok.UserId = user.Id;
                tok.UserDisplayName = user.DisplayName;
                tok.TokTypeId = $"toktype-{tok.TokGroup.ToIdFormat()}-{tok.TokType.ToIdFormat()}";
                tok.UserCountry = user.Country;
                tok.UserState = user.State;
                tok.Image = file.FileUrl;
                tok.SourceLink = file.FileUrl;
                await TokService.Instance.CreateTokAsync(tok, item: "tokdocs");
                // var result = await TokService.Instance.CreateTokAsync(tok,item:"tokdocs");
            }
        }

        public async Task<ResultData<ClassGroup>> GetCommunitiesAsync(ClassGroupQueryValues queryValues)
        {
            try
            {
                _httpClientHelper.ClearHeaders();
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("text");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("joined");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("paginationid");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("showimage");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("isdescending");
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("groupkind");

                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", queryValues.userid);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", queryValues.partitionkeybase);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", queryValues.text);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("joined", queryValues.StringJoined);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("paginationid", queryValues?.paginationid ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("showimage", queryValues.showImage?.ToString() ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("isdescending", queryValues.isDescending?.ToString() ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupkind", queryValues.groupkind?.ToString() ?? string.Empty);

                _httpClientHelper.Instance.DefaultRequestHeaders.Add("level0", queryValues?.level0 ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("level1", queryValues?.level1 ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("level2", queryValues?.level2 ?? string.Empty);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("level3", queryValues?.level3 ?? string.Empty);

                var apiUrl = $"{Config.Configurations.ApiPrefix}/communities/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
                //var apiUrl = $"http://localhost:7071/api/classgroups";
                //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ResultData<Dictionary<string, object>>>(content);
                var resource = JsonConvert.SerializeObject(data);

                ResultData<ClassGroup> result = JsonConvert.DeserializeObject<ResultData<ClassGroup>>(resource);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultData<ClassGroup>();
            }
        }
        #endregion

        #region ClassTokServiceDB Functions
        public async Task<ResultData<ClassTokModel>> GetClassToksAsync(GetClassToksRequest request) {


            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();

            try
            {
                //ClasstokserviceDB GetClasstoks function
                //Add try catch due to error 400 crash
                //GetClassToksRequest request = new ServiceAccount.ServicesDB.GetClassToksRequest()
                //{
                //    QueryValues = queryValues
                //};
                var getclasstoksDB = await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.Interfaces.IClassTokService>().GetClassToksAsync<ClassTokModel>(request);
                tokResult.ContinuationToken = getclasstoksDB.ContinuationToken;
                var resultString = JsonConvert.SerializeObject(getclasstoksDB.Results);
                tokResult.Results = JsonConvert.DeserializeObject<List<ClassTokModel>>(resultString);

                if (tokResult.Results == null)
                    tokResult.Results = new List<ClassTokModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                tokResult = new ResultData<ClassTokModel>();
                tokResult.Results = new List<ClassTokModel>();
            }

            return tokResult;
        }
        #endregion
    }
}
