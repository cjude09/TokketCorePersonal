using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Extensions.Http;
using Tokket.Shared.Helpers;
using Tokket.Shared.Helpers.Interfaces;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class TokService : ITokService
    {
        public static ITokService Instance = new TokService();
        private HttpClientHelper _httpClientHelper;
        public TokService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }

        public async Task<List<TokModel>> GetAllFeaturedToks()
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/allfeaturedtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<Tok>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                var list = data.Results.ToList();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].UserId == "tokket")
                    {
                        list[i].UserPhoto = "/images/tokket.png";
                    }
                }
                return JsonConvert.DeserializeObject<List<TokModel>>(JsonConvert.SerializeObject(list));
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TokModel>> GetAllToks(TokQueryValues values)
        {
            if (values == null)
                values = new TokQueryValues();
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("country", values?.country);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("category", values?.category);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokgroup", values?.tokgroup);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktype", values?.toktype);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("streamtoken", values?.streamtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("sortby", values?.sortby);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("image", (values?.image).ToString());
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<Tok>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                var list = data.Results.ToList();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].UserId == "tokket")
                    {
                        list[i].UserPhoto = "/images/tokket.png";
                    }
                }
                var serialize = JsonConvert.SerializeObject(list);
                return JsonConvert.DeserializeObject<List<TokModel>>(serialize);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return null;
            }
        }
        public List<TokModel> AlternateToks(List<TokModel> resultData)
        {
            List<TokModel> image = new List<TokModel>(), nonImage = new List<TokModel>(), alternated = new List<TokModel>();
            image = resultData.Where(x => !(x.Image == null || x.Image.Equals(""))).ToList();
            nonImage = resultData.Where(x => (x.Image == null || x.Image.Equals(""))).ToList();
            //Image first
            while (image.Count > 0)
            {
                alternated.Add(image[image.Count - 1]);
                image.Remove(image[image.Count - 1]);
                //Non image next
                if (nonImage.Count > 0)
                {
                    alternated.Add(nonImage[nonImage.Count - 1]);
                    nonImage.Remove(nonImage[nonImage.Count - 1]);
                }
            }
            //Rest are non image
            if (nonImage.Count > 0)
                alternated.AddRange(nonImage);
            resultData = alternated;
            return resultData;
        }
        public async Task<List<TokModel>> GetToksAsync(TokQueryValues values = null, string item = null, string serviceid = null)
        {
            if (values == null)
                values = new TokQueryValues();

            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
            //if (!string.IsNullOrEmpty(item))
            //{
            //    if (item == "abbreviations")
            //    {
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid");
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", item);
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "alphaguess");
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Add("country", values?.country);
            //        _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
            //    }

            //}

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values?.offset);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupid", values?.groupid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("streamtoken", values?.streamtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("image", (values?.image).ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tagid", values.tagid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("category", values?.category);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("country", values?.country);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("state", values?.state);
            if (!string.IsNullOrEmpty(item))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", item);
            }
            if (!string.IsNullOrEmpty(serviceid))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceid);
            }

            var apiUrl = $"{Config.Configurations.ApiPrefix}/toks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<TokModel>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                if (item == "abbreviations")
                {
                    AlphaTokService.ContinuationToken = data.ContinuationToken;
                }
                var list = data.Results.ToList();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].UserId == "tokket")
                    {
                        list[i].UserPhoto = "/images/tokket.png";
                    }
                }
                var serialize = JsonConvert.SerializeObject(list);
                return JsonConvert.DeserializeObject<List<TokModel>>(serialize);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return null;
            }
        }
        public async Task<List<TokModel>> GetAllFeaturedToksAsync()
        {
            _httpClientHelper.ClearHeaders();
            var apiUrl = $"{Config.Configurations.ApiPrefix}/allfeaturedtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<TokModel>>(response);
                Settings.ContinuationToken = data.ContinuationToken;

                var list = data.Results.ToList();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].UserId == "tokket")
                    {
                        list[i].UserPhoto = "/images/tokket.png";
                    }
                }
                var serialize = JsonConvert.SerializeObject(list);
                return JsonConvert.DeserializeObject<List<TokModel>>(serialize);
            }
            catch
            {
                return null;
            }
        }
        public async Task<ResultModel> CreateTokAsync(TokModel tok, CancellationToken cancellationToken = default(CancellationToken), string item = "")
        {
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            _httpClientHelper.ClearHeaders();

            if (!string.IsNullOrEmpty(item))
            {
                if (item == "abbreviations" || item == "tokpics" || item == "tokdocs")
                {
                    _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
                    _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", item);
                }

            }
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);

            var model = JsonConvert.SerializeObject(tok);
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };

            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"), cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Created:
                        result.ResultEnum = Helpers.Result.Success;
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.Unauthorized:
                        result.ResultEnum = Helpers.Result.Failed;
                        break;
                }

                if (result.ResultEnum == Helpers.Result.Success)
                {
                    try
                    {
                        result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<TokModel>(content) : null);
                    }
                    catch (Exception) { }
                }
                else
                {
                    result.ResultObject = null;
                }
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
        public async Task<ResultModel> DeleteTokAsync(string id, string pk = "")
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", pk);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Accepted:
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                    result.ResultEnum = Helpers.Result.Success;
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                case System.Net.HttpStatusCode.Forbidden:
                case System.Net.HttpStatusCode.NotFound:
                case System.Net.HttpStatusCode.Unauthorized:
                    result.ResultEnum = Helpers.Result.Failed;
                    break;
            }

            return result;
        }
        public async Task<ResultModel> UpdateTokAsync(TokModel tok, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Updating tok failed." };
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tok/{tok.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(tok);

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", tok.PartitionKey);
            try
            {
                var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"), cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Update Successful!";
                    result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<TokModel>(content) : null);
                }
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
        public async Task<TokModel> GetTokIdAsync(string id)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tok/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);
            return JsonConvert.DeserializeObject<TokModel>(response);
        }
        public async Task<List<TokModel>> GetToksByIdsAsync(List<string> ids)
        {
            if (ids == null)
                return null;
            if (ids.Count > 100)
                return null;
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toks/ids{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, ids);
            try
            {
                var data = JsonConvert.DeserializeObject<ResultData<Tok>>(response);
                Settings.ContinuationToken = data.ContinuationToken;
                if (data.Results != null)
                {
                    var list = data.Results.ToList();
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i].UserId == "tokket")
                        {
                            list[i].UserPhoto = "/images/tokket.png";
                        }
                    }
                    var serialize = JsonConvert.SerializeObject(list);
                    return JsonConvert.DeserializeObject<List<TokModel>>(serialize);
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }

        }

        #region MegaTok
        public async Task<ResultData<TokSection>> GetTokSectionsAsync(string tokId, int count = 0, string continuationToken = null)
        {
            TokSectionQueryValues values = new TokSectionQueryValues()
            {
                tokId = tokId,
                continuationToken = continuationToken,
                count = count
            };
            Settings.ContinuationToken = values.continuationToken;
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksections/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(values);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            ResultData<TokSection> result = new ResultData<TokSection>();
            result = JsonConvert.DeserializeObject<ResultData<TokSection>>(content);
            if (result.Results == null)
            {
                return new ResultData<TokSection>() { Results = new List<TokSection>() };
            }
            else
            {
                return result; //.ToList()
            }

        }
        public async Task<bool> CreateTokSectionAsync(TokSection tokSection, string tokId, int partitionNumber)
        {
            string partitionKey = $"{tokId}-toksections{partitionNumber}";
            tokSection.PartitionKey = partitionKey;

            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tokSection.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", partitionKey);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/create/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(tokSection);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var content = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateTokSectionAsync(TokSection newTokSection)
        {
            if (string.IsNullOrEmpty(newTokSection.PartitionKey) || string.IsNullOrEmpty(newTokSection.TokId))
                return false;
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk"); // Avoids duplicate pk, get tok section will cause duplicate pk     
                                                                           //Required
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", newTokSection.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", newTokSection.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokid", newTokSection.TokId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/{newTokSection.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(newTokSection);
            var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteTokSectionAsync(TokSection tokSection)
        {
            if (string.IsNullOrEmpty(tokSection.Id) || string.IsNullOrEmpty(tokSection.TokId) || string.IsNullOrEmpty(tokSection.PartitionKey))
                return false;
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk"); // Avoids duplicate pk, get tok section will cause duplicate pk
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", tokSection.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokid", tokSection.TokId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/{tokSection.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }
        public async Task<ResultData<TokkepediaReaction>> UserReactionsGet(string item_id, string fromCaller = "")
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("item_id", item_id);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/userreactions/{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.GetAsync(apiUrl);

            var data = JsonConvert.DeserializeObject<ResultData<TokkepediaReaction>>(response);
            if (!string.IsNullOrEmpty(fromCaller))
            {
                SetUserReactionsCache(fromCaller, data);
            }
            return data;
        }

        public void SetUserReactionsCache(string fromCaller, ResultData<TokkepediaReaction> data)
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

        public ResultData<TokkepediaReaction> GetUserReactionsCache(string fromCaller)
        {
            //var apiUrl = $"{Config.Configurations.ApiPrefix}/classtoks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var getCacheData = _httpClientHelper.GetCachedAsync<string>(fromCaller);
            //If there's a cache data and forceRefresh == false
            var result = new ResultData<TokkepediaReaction>();
            result.Results = null;
            if (!string.IsNullOrEmpty(getCacheData))
            {
                result = JsonConvert.DeserializeObject<ResultData<TokkepediaReaction>>(getCacheData);
            }
            return result;
        }

        #endregion

        #region QnATok
        public async Task<ResultData<TokSection>> GetQnATokSectionsAsync(string tokId, int count = 0, string continuationToken = null)
        {
            TokSectionQueryValues values = new TokSectionQueryValues()
            {
                tokId = tokId,
                continuationToken = continuationToken,
                count = count
            };
            Settings.ContinuationToken = values.continuationToken;
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksections/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(values);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            ResultData<TokSection> result = new ResultData<TokSection>();
            result = JsonConvert.DeserializeObject<ResultData<TokSection>>(content);
            if (result.Results == null)
            {
                return new ResultData<TokSection>() { Results = new List<TokSection>() };
            }
            else
            {
                return result; //.ToList()
            }

        }
        public async Task<bool> CreateQnaTokSectionAsync(TokSection tokSection, string tokId, int partitionNumber)
        {
            string partitionKey = $"{tokId}-toksections{partitionNumber}";
            tokSection.PartitionKey = partitionKey;

            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tokSection.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", partitionKey);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/create/{tokId}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var model = JsonConvert.SerializeObject(tokSection);
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            var content = response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateQnATokSectionAsync(TokSection newTokSection)
        {
            if (string.IsNullOrEmpty(newTokSection.PartitionKey) || string.IsNullOrEmpty(newTokSection.TokId))
                return false;
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk"); // Avoids duplicate pk, get tok section will cause duplicate pk     
                                                                           //Required
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", newTokSection.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", newTokSection.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokid", newTokSection.TokId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/{newTokSection.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var model = JsonConvert.SerializeObject(newTokSection);
            var response = await _httpClientHelper.Instance.PutAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteQnATokSectionAsync(TokSection tokSection)
        {
            if (string.IsNullOrEmpty(tokSection.Id) || string.IsNullOrEmpty(tokSection.TokId) || string.IsNullOrEmpty(tokSection.PartitionKey))
                return false;
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("pk"); // Avoids duplicate pk, get tok section will cause duplicate pk
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("pk", tokSection.PartitionKey);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokid", tokSection.TokId);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/toksection/{tokSection.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }

        #endregion

        public async Task<ResultModel> CreateTokAsync(Models.AlphaToks.Tok tok, string item = "")
        {
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var apiUrl = $"{Config.Configurations.ApiPrefix}/tok{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            _httpClientHelper.ClearHeaders();

            if (!string.IsNullOrEmpty(item))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", item);
            }

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);

            var model = JsonConvert.SerializeObject(tok);
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };

            try
            {
                var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(model, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Created:
                        result.ResultEnum = Helpers.Result.Success;
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.Unauthorized:
                        result.ResultEnum = Helpers.Result.Failed;
                        break;
                }

                if (result.ResultEnum == Helpers.Result.Success)
                {
                    try
                    {
                        result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<TokModel>(content) : null);
                    }
                    catch (Exception) { }
                }
                else
                {
                    result.ResultObject = null;
                }
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

        public async Task<ResultData<YearbookTok>> GetYearbooksAsync(TokQueryValues values = null, string serviceid = null)
        {
            if (values == null)
                values = new TokQueryValues();
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default

            var idtoken = await SecureStorage.GetAsync("idtoken");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values?.offset);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("streamtoken", values?.streamtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("image", (values?.image).ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tagid", values.tagid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("country", values?.country);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("state", values?.state);

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("category", "Yearbook");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "yearbooktoks");

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("yearbook_tiletype", values?.yearbook_tiletype);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("yearbook_type", values?.yearbook_type);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("yearbook_schoolname", values?.yearbook_schoolname);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("yearbook_grouptype", values?.yearbook_grouptype);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("yearbook_timing", values?.yearbook_timing);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values?.text);
            if (!string.IsNullOrEmpty(serviceid))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceid);
            }

            var apiUrl = $"{Config.Configurations.ApiPrefix}/yearbooks{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);

            try
            {
                var data = await response.Content.ReadAsAsync<ResultData<YearbookTok>>();

                for (int i = 0; i < data.Results.Count(); ++i)
                {
                    if (data.Results.ToList()[i].UserId == "tokket")
                    {
                        data.Results.ToList()[i].UserPhoto = "/images/tokket.png";
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return null;
            }
        }

        public async Task<ResultModel> CreateYearbookAsync(YearbookTok tok) {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "" };
            var apiUrl = $"{Config.Configurations.ApiPrefix}/yearbook{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid"); // Remove default

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokket");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "yearbooktoks");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", "");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", "Yearbook");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid",tok.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var json = JsonConvert.SerializeObject(tok);
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(apiUrl, tok);
            var content = await response.Content.ReadAsAsync<ResultModel>();
            if (content != null) {
                result.ResultEnum = content.ResultEnum;
                result.ResultMessage = content.ResultMessage;
                result.ResultObject = content.ResultObject;
            }

            return result;
        }

        public async Task<YearbookTok> GetYearbookAsync(string id, string pk) {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/yearbook/{id}/{pk}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            return await response.Content.ReadAsAsync<YearbookTok>();
        }

        public async Task<ResultModel> UpdateYearbookAsync(YearbookTok tok) {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Updating tok failed." };

            try {
                var apiUrl = $"{Config.Configurations.ApiPrefix}/yearbook/{tok.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                var json = JsonConvert.SerializeObject(tok);

                var idtoken = await SecureStorage.GetAsync("idtoken");
                _httpClientHelper.ClearHeaders();
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid"); // Remove default

                _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokket");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "yearbooktoks");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", "");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", "Yearbook");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
                HttpResponseMessage response = await _httpClientHelper.Instance.PutAsJsonAsync(apiUrl, tok);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Update Successful!";
                    result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<Tok>(content) : null);
                }
            } catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
            }

            return result;
        }

        public async Task<ResultModel> CreateOpportunityAsync(OpportunityTok tok)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "" };
            var apiUrl = $"{Config.Configurations.ApiPrefix}/opportunity{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid"); // Remove default

            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokket");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "opportunitytok");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", "");
           // _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", "Opportunity");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var json = JsonConvert.SerializeObject(tok);
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(apiUrl, tok);
            var content = await response.Content.ReadAsAsync<ResultModel>();
            if (content != null)
            {
                result.ResultEnum = content.ResultEnum;
                result.ResultMessage = content.ResultMessage;
                result.ResultObject = content.ResultObject;
            }

            return result;
        }

        public async Task<OpportunityTok> GetOpportunityAsync(string id, string pk)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/opportunity/{id}/{pk}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            return await response.Content.ReadAsAsync<OpportunityTok>();
        }

        public async Task<ResultModel> UpdateOpportunityAsync(OpportunityTok tok)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Updating tok failed." };

            try
            {
                var apiUrl = $"{Config.Configurations.ApiPrefix}/opportunity/{tok.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
                var json = JsonConvert.SerializeObject(tok);

                var idtoken = await SecureStorage.GetAsync("idtoken");
                _httpClientHelper.ClearHeaders();
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid"); // Remove default

                _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokket");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "opportunitytok");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("toktypeid", "");
              //  _httpClientHelper.Instance.DefaultRequestHeaders.Add("categoryid", "Opportunity");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
                HttpResponseMessage response = await _httpClientHelper.Instance.PutAsJsonAsync(apiUrl, tok);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Update Successful!";
                    result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<Tok>(content) : null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
            }

            return result;
        }

        public async Task<ResultData<OpportunityTok>> GetOpportunitiesAsync(TokQueryValues values = null, string serviceid = null)
        {
            if (values == null)
                values = new TokQueryValues();
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("token"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("streamtoken"); // Remove default
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid"); // Remove default

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("offset", values?.offset);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("order", values?.order);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", values?.token);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("streamtoken", values?.streamtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("image", (values?.image).ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("tagid", values.tagid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("country", values?.country);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", values?.userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("state", values?.state);

           // _httpClientHelper.Instance.DefaultRequestHeaders.Add("category", "Opportunity");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("itemsbase", "opportunitytok");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("text", values.text);

            //Opportunity
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("opportunity_type", values?.opportunity_type);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("application_deadline", values?.application_deadline);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("awards_available", values?.awards_available);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("description", values?.description);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("amount", values?.amount);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("email_address", values?.email_address);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("address", values?.address);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("phone_number", values?.phone_number);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("about_company", values?.about_company);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("website", values?.website);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("requirements", values?.requirements);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("training_type", values?.training_type);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("training_tok", values?.training_tok);
            if (!string.IsNullOrEmpty(serviceid))
            {
                _httpClientHelper.Instance.DefaultRequestHeaders.Remove("serviceid");
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceid);
            }

            var apiUrl = $"{Config.Configurations.ApiPrefix}/opportunity{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl);

            try
            {
                var data = await response.Content.ReadAsAsync<ResultData<OpportunityTok>>();

                for (int i = 0; i < data.Results.Count(); ++i)
                {
                    if (data.Results.ToList()[i].UserId == "tokket")
                    {
                        data.Results.ToList()[i].UserPhoto = "/images/tokket.png";
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return null;
            }
        }
    }
}