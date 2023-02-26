using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.AlphaToks;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Tokket.Core.Tools;
using Tokket.Shared.Extensions.Http;
using System.Linq;

namespace Tokket.Shared.Services
{
    public class AlphaTokService : IAlphaToksService
    {
        private static AlphaTokService _instance = new AlphaTokService();
        public static AlphaTokService Instance { get { return _instance; } }

        public static string ContinuationToken { get; set; }
        public async Task<ResultModel> AddReaction(TokkepediaReaction item)
        {


            #region Service id and device platform
            HttpClient client = new HttpClient();

            HttpExtensions.RequestHeaders.Clear();
            //client.ClearHeaders();

            //client.Instance.DefaultRequestHeaders.Remove("userid");
            //client.Instance.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);
            //client.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            //client.Instance.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);
            //client.Instance.DefaultRequestHeaders.Add("itemid", item.ItemId);
            //client.Instance.DefaultRequestHeaders.Add("userid", item.UserId);
           

             HttpExtensions.RequestHeaders.Remove("userid");
             HttpExtensions.RequestHeaders.Add("deviceplatform", devicePlatform);
             HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
             HttpExtensions.RequestHeaders.Add("token", Settings.GetUserModel().IdToken);
             HttpExtensions.RequestHeaders.Add("itemid", item.ItemId);
             HttpExtensions.RequestHeaders.Add("userid", item.UserId);
            #endregion

            var content = string.Empty;
            try
            {
                var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/reaction{codePrefix}{Config.Configurations.ApiKey}";
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, item);
                content = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error add react: " + ex.ToString());
            }

            return JsonConvert.DeserializeObject<ResultModel>(content);
        }

        public async Task<ResultModel> CreateTokAsync(Models.AlphaToks.Tok tok)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Creating invite failed." };
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok{codePrefix}{Config.Configurations.ApiKey}";

            #region Service id and device platform
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();


            client.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            client.DefaultRequestHeaders.Add("userid", tok.UserId);
            client.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            client.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            client.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);
            client.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);

            //client.ClearHeaders();
            //client.Instance.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            //client.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            //client.Instance.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            //client.Instance.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            //client.Instance.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);
            //client.Instance.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);
            #endregion
            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, tok);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Create Successful!";
                    //  result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<Tok>(content) : null);
                }
            }
            return result;
        }


        public async Task<ResultModel> CreateTokAsync(TokModel tok)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Creating invite failed." };
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok{codePrefix}{Config.Configurations.ApiKey}";

            #region Service id and device platform
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();


            client.DefaultRequestHeaders.Add("serviceid", serviceId);
            client.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);
            client.DefaultRequestHeaders.Add("userid", tok.UserId);
            client.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            client.DefaultRequestHeaders.Add("userid", tok.UserId);
            client.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            client.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            client.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);
            client.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);

            //client.ClearHeaders();
            //client.Instance.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            //client.Instance.DefaultRequestHeaders.Add("userid", tok.UserId);
            //client.Instance.DefaultRequestHeaders.Add("tokgroupid", tok.TokGroup.ToIdFormat());
            //client.Instance.DefaultRequestHeaders.Add("toktypeid", tok.TokTypeId);
            //client.Instance.DefaultRequestHeaders.Add("categoryid", tok.CategoryId);
            //client.Instance.DefaultRequestHeaders.Add("token", Settings.GetUserModel().IdToken);
            #endregion
            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, tok);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Create Successful!";
                    //  result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<Tok>(content) : null);
                }
            }
            return result;
        }

        public async Task<bool> DeleteTokAsync(string id, string pk)
        {
            var client = new HttpClient();

            //client.ClearHeaders();

            //#region Service id and device platform


            //client.Instance.DefaultRequestHeaders.Clear();

            ////if (App._apiClient.client == null)
            ////    App._apiClient.client = new HttpClient();

            //client.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            //client.Instance.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);
            //client.Instance.DefaultRequestHeaders.Add("userid",Settings.GetUserModel().UserId);
            ////Invites
            //client.Instance.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            //client.Instance.DefaultRequestHeaders.Add("pk", pk);
            //#endregion

            #region Service id and device platform


            HttpExtensions.RequestHeaders.Clear();

            //if (App._apiClient.client == null)
            //    App._apiClient.client = new HttpClient();

             HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
             HttpExtensions.RequestHeaders.Add("deviceplatform", devicePlatform);
             HttpExtensions.RequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            //Invites
             HttpExtensions.RequestHeaders.Add("itemsbase", "abbreviations");
             HttpExtensions.RequestHeaders.Add("pk", pk);
            #endregion

            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok/{id}{codePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await client.DeleteRequestAsync(apiUrl);
            return response.IsSuccessStatusCode;

            throw new NotImplementedException();
        }

        public async Task<ResultData<TokkepediaReaction>> GetReactionsAsync(ReactionQueryValues values = null)
        {

            var client = new HttpClient();

          
            if (values == null)
                values = new ReactionQueryValues();

            #region Service id and device platform

            //client.Instance.DefaultRequestHeaders.Clear();



            //client.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            //client.Instance.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);
            //#endregion

            //client.Instance.DefaultRequestHeaders.Add("limit", values?.limit.ToString());
            //client.Instance.DefaultRequestHeaders.Add("kind", values?.kind);
            //client.Instance.DefaultRequestHeaders.Add("item_id", values?.item_id);
            //client.Instance.DefaultRequestHeaders.Add("activity_id", values?.activity_id);
            //client.Instance.DefaultRequestHeaders.Add("user_id", values?.user_id);
            //client.Instance.DefaultRequestHeaders.Add("reaction_id", values?.reaction_id);
            //client.Instance.DefaultRequestHeaders.Add("pagination_id", values?.pagination_id);
            //client.Instance.DefaultRequestHeaders.Add("reaction_total", values?.reaction_total.ToString() ?? "0");
            //client.Instance.DefaultRequestHeaders.Add("detail_number", values?.detail_number.ToString() ?? "0");

            HttpExtensions.RequestHeaders.Clear();
             HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
             HttpExtensions.RequestHeaders.Add("deviceplatform", devicePlatform);
            #endregion

             HttpExtensions.RequestHeaders.Add("limit", values?.limit.ToString());
             HttpExtensions.RequestHeaders.Add("kind", values?.kind);
             HttpExtensions.RequestHeaders.Add("item_id", values?.item_id);
             HttpExtensions.RequestHeaders.Add("activity_id", values?.activity_id);
             HttpExtensions.RequestHeaders.Add("user_id", values?.user_id);
             HttpExtensions.RequestHeaders.Add("reaction_id", values?.reaction_id);
             HttpExtensions.RequestHeaders.Add("pagination_id", values?.pagination_id);
             HttpExtensions.RequestHeaders.Add("reaction_total", values?.reaction_total.ToString() ?? "0");
             HttpExtensions.RequestHeaders.Add("detail_number", values?.detail_number.ToString() ?? "0");
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/reactions{codePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = await response.Content.ReadAsAsync<ResultData<TokkepediaReaction>>();

                return data;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                return null;
            }
            throw new NotImplementedException();
        }

        public async Task<Models.AlphaToks.Tok> GetTokAsync(string id)
        {
            var client = new HttpClient();

            //client.ClearHeaders();
            //#region Service id and device platform


            //client.Instance.DefaultRequestHeaders.Clear();


            //client.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            //client.Instance.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);

            ////Invites
            //client.Instance.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            //client.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            //#endregion
            HttpExtensions.RequestHeaders.Clear();
            #region Service id and device platform


             HttpExtensions.RequestHeaders.Clear();


             HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
             HttpExtensions.RequestHeaders.Add("deviceplatform", devicePlatform);

            //Invites
             HttpExtensions.RequestHeaders.Add("itemsbase", "abbreviations");
             HttpExtensions.RequestHeaders.Add("userid", Settings.GetUserModel().UserId);
            #endregion

            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok/{id}{codePrefix}{Config.Configurations.ApiKey}";
            //var apiUrl = $"{_apiSettings.ApiPrefix}/tok/{id}{_apiSettings.CodePrefix}{_apiSettings.ApiKey}";

            HttpResponseMessage response = await client.GetRequestAsync(apiUrl);
            return await response.Content.ReadAsAsync<Models.AlphaToks.Tok>();
            throw new NotImplementedException();
        }

        public async Task<ResultData<Models.AlphaToks.Tok>> GetToksAsync(Models.AlphaToks.TokQueryValues values = null)
        {
            var client = new HttpClient();

         
            if (values == null)
                values = new Models.AlphaToks.TokQueryValues();

            var converted = JsonConvert.SerializeObject(values);
            client.DefaultRequestHeaders.Remove("userid"); // Remove default
            client.DefaultRequestHeaders.Remove("token"); // Remove default
            client.DefaultRequestHeaders.Remove("streamtoken"); // Remove default

            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Add("offset", values?.offset);
            client.DefaultRequestHeaders.Add("order", values?.order);
            client.DefaultRequestHeaders.Add("country", values?.country);
            client.DefaultRequestHeaders.Add("category", values?.category);
            client.DefaultRequestHeaders.Add("tokgroup", values?.tokgroup);
            client.DefaultRequestHeaders.Add("toktype", values?.toktype);
            client.DefaultRequestHeaders.Add("userid", values?.userid);
            client.DefaultRequestHeaders.Add("text", values?.text);
            client.DefaultRequestHeaders.Add("loadmore", values?.loadmore);
            client.DefaultRequestHeaders.Add("token", values?.token);
            client.DefaultRequestHeaders.Add("streamtoken", values?.streamtoken);
            client.DefaultRequestHeaders.Add("image", (values?.image).ToString());
            client.DefaultRequestHeaders.Add("tagid", values.tagid);


            //Invites
            client.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
            client.DefaultRequestHeaders.Add("serviceid", serviceId);

            //HttpExtensions.RequestHeaders.Clear();

            // HttpExtensions.RequestHeaders.Add("offset", values?.offset);
            // HttpExtensions.RequestHeaders.Add("order", values?.order);
            // HttpExtensions.RequestHeaders.Add("country", values?.country);
            // HttpExtensions.RequestHeaders.Add("category", values?.category);
            // HttpExtensions.RequestHeaders.Add("tokgroup", values?.tokgroup);
            // HttpExtensions.RequestHeaders.Add("toktype", values?.toktype);
            // HttpExtensions.RequestHeaders.Add("userid", values?.userid);
            // HttpExtensions.RequestHeaders.Add("text", values?.text);
            // HttpExtensions.RequestHeaders.Add("loadmore", values?.loadmore);
            // HttpExtensions.RequestHeaders.Add("token", values?.token);
            // HttpExtensions.RequestHeaders.Add("streamtoken", values?.streamtoken);
            // HttpExtensions.RequestHeaders.Add("image", (values?.image).ToString());
            // HttpExtensions.RequestHeaders.Add("tagid", values.tagid);


            ////Invites
            // HttpExtensions.RequestHeaders.Add("itemsbase", "abbreviations");
            // HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
            //var apiUrl = new Uri($"{_apiSettings.BaseUrl}{_apiSettings.ApiPrefix}/toks{_apiSettings.CodePrefix}{_apiSettings.ApiKey}");
            var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/toks{codePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await client.GetRequestAsync(apiUrl);
            ResultData<Models.AlphaToks.Tok> data = new ResultData<Models.AlphaToks.Tok>(); 
            try
            {
                 data = await response.Content.ReadAsAsync<ResultData<Models.AlphaToks.Tok>>();
                var convert = data.Results.ToList();
                for (int i = 0; i < convert.Count(); ++i)
                {
                    if (convert[i].UserId == "tokket")
                    {
                        convert[i].UserPhoto = "/images/tokket.png";
                    }
                }

              
            }
            catch (Exception ex)
            {
                return null;
            }

            return data;
        }

        public async Task<ResultModel> UpdateTokAsync(Models.AlphaToks.Tok tok)
        {
            var client = new HttpClient();

            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed, ResultMessage = "Updating invite failed." };
            try
            {
                var apiUrl = $"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/tok/{tok.Id}{codePrefix}{Config.Configurations.ApiKey}";
                //#region Service id and device platform


                //client.Instance.DefaultRequestHeaders.Clear();


                //client.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
                //client.Instance.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);
                //#endregion
                //client.Instance.DefaultRequestHeaders.Add("userid",Settings.GetUserModel().UserId);
                ////Invites
                //client.Instance.DefaultRequestHeaders.Add("itemsbase", "abbreviations");
                //client.Instance.DefaultRequestHeaders.Add("pk", tok.PartitionKey);
                #region Service id and device platform



                HttpExtensions.RequestHeaders.Clear();


                 HttpExtensions.RequestHeaders.Add("serviceid", serviceId);
                 HttpExtensions.RequestHeaders.Add("deviceplatform", devicePlatform);
                #endregion
                 HttpExtensions.RequestHeaders.Add("userid", Settings.GetUserModel().UserId);
                //Invites
                 HttpExtensions.RequestHeaders.Add("itemsbase", "abbreviations");
                 HttpExtensions.RequestHeaders.Add("pk", tok.PartitionKey);
                var json = JsonConvert.SerializeObject(tok);
                HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, tok);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.ResultEnum = Helpers.Result.Success;
                    result.ResultMessage = "Update Successful!";
                    result.ResultObject = (!string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<Models.AlphaToks.Tok>(content) : null);
                }
            }
            catch (Exception ex)
            {

            }
           // return result;
            throw new NotImplementedException();
        }

#region Properties
        string devicePlatform = "android";
        private const string codePrefix = "?code=";

        //private const TokketId tokketId = TokketId.TokBlitz;
        private const string serviceId = "alphaguess", websiteUrl = "tokblitz.com";
        private static string _apiKey = "";

        private HttpClient _client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };
#endregion
    }
}
