using Newtonsoft.Json;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Core;
using System.Globalization;
using Xamarin.Essentials;
using Tokket.Core.Tools;
using System.Net.Http.Headers;

namespace Tokket.Shared.Services
{
    public class AccountService : Interfaces.IAccountService
    {
        public static Interfaces.IAccountService Instance = new AccountService();

        private HttpClientHelper _httpClientHelper;
        public AccountService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        
        public static async Task RefreshAccount() {
            TokketUser tokketUser = await AccountService.Instance.GetUserAsync(Settings.GetUserModel().UserId);
            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
            Settings.UserCoins = tokketUser.Coins.Value;
            long longcoins = Settings.UserCoins;

        }
        public async Task<ResultModel> Login(LoginModel model)
        {
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokkepedia");
            TokketUser user = new TokketUser() { Email = model.Username, PasswordHash = model.Password};
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };
         
            var apiUrl = $"{Config.Configurations.ApiPrefix}/login"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API

            //var content = await _httpClientHelper.PostAsync(apiUrl, user);
            var content = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            var resContent = await content.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<ResultModel>(resContent);
            return result;
        }
        // Verifies user token
        public async Task<ResultModel> VerifyTokenAsync(string token, string refreshToken)
        {
            Models.AuthorizationTokenModel model = new Models.AuthorizationTokenModel();
            model.IdToken = token;
            model.RefreshToken = refreshToken;
            model.HashKey = HashGenerator.GenerateCustomHash("T0KK3T" + Settings.GetUserModel().UserId, 512, 24);
            model.UserId = Settings.GetUserModel().UserId;

            var apiUrl = $"{Config.Configurations.ApiPrefix}/verifytoken{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            //var content = await _httpClientHelper.PostAsync(apiUrl, model);
            var content = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var item = await content.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultModel>(item);
        }

        public ResultModel VerifyToken(string token, string refreshToken)
        {
            var userid = Settings.GetUserModel().UserId;
            Models.AuthorizationTokenModel model = new Models.AuthorizationTokenModel();
            model.IdToken = token;
            model.RefreshToken = refreshToken;
            model.HashKey = HashGenerator.GenerateCustomHash("T0KK3T" + userid, 512, 24);
            model.UserId = Settings.GetUserModel().UserId;

            var apiUrl = $"{Config.Configurations.ApiPrefix}/verifytoken{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var content = _httpClientHelper.Post(apiUrl, model);
            return JsonConvert.DeserializeObject<ResultModel>(content);
        }

        public async Task<ResultModel> RefreshTokenAsync(string refreshToken)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/verifytoken{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            var content = await _httpClientHelper.PostAsync(apiUrl, refreshToken);
            return JsonConvert.DeserializeObject<ResultModel>(content);
        }

        public async Task<TokketUser> GetUserAsync(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/user/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Api Method to Call with values
            var content = await _httpClientHelper.GetAsync(apiUrl);
            if (content == "Object reference not set to an instance of an object.") {
                return null;
            }
            else
            return JsonConvert.DeserializeObject<TokketUser>(content);
        }

        public async Task<ResultData<TokketUser>> GetUsersAsync(string serviceId, UserQueryValues values)
        {
            var _client = new HttpClient();
            if (values == null)
            {
                values = new UserQueryValues();
            }
            values.displaynameexact = true;
            
            //var devicePlatform = "android";
            //_client.DefaultRequestHeaders.Add("order", values?.order);
            //_client.DefaultRequestHeaders.Add("accounttype", values?.accounttype);
            //_client.DefaultRequestHeaders.Add("displayname", values?.displayname);

            //_client.DefaultRequestHeaders.Add("serviceid", serviceId);
            //_client.DefaultRequestHeaders.Add("deviceplatform", devicePlatform);

            //_client.DefaultRequestHeaders.Add("displaynameexact", values?.displaynameexact.ToString());

            var apiUrl = new Uri($"{Config.Configurations.BaseUrl}/v1/users{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");
            HttpResponseMessage response = await _client.GetAsync(apiUrl);

            try
            {
                var convert = await response.Content.ReadAsStringAsync();
                ResultData<TokketUser> data = JsonConvert.DeserializeObject<ResultData<TokketUser>>(convert);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TokketUser GetUser(string id)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/user/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Api Method to Call with values
            var content = _httpClientHelper.GetData(apiUrl);

            TokketUser tokketUser = new TokketUser();
            try
            {
                tokketUser = JsonConvert.DeserializeObject<TokketUser>(content);
            }
            catch (Exception ex)
            {
                tokketUser = new TokketUser();
            }
            return tokketUser;
        }

        public async Task<TokketUser> GetUserByEmailAsync(string email)
        {
            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("email", email);
            var apiUrl = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/user{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"); // Api Method to Call with values
            var item = await _httpClient.GetAsync(apiUrl);
            string content = await item.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokketUser>(content);
        }

        public async Task<ResultData<TokketUser>> GetUsers(string displayname, string accountype)
        {
            // TokkepediaApiClient client = new TokkepediaApiClient(_apiSettings.ApiKey, _apiSettings.BaseUrl + _apiSettings.ApiPrefix, _apiSettings.CodePrefix, TokketEnvironment.Dev, TokketDevicePlatform.Web);
            var client = new HttpClient();
            UserQueryValues test = new UserQueryValues();
            test.accounttype = accountype.ToLower();
            test.displayname = displayname;

            ResultData<TokketUser> holder = await GetUsersAsync("tokket", test);
            return holder;
        }

        //public async Task<ResultModel> SignUpAsync(string email, string password, string displayName, string country, DateTime date, string userPhoto)
        //{
        //    TokketUser user = new TokketUser() { Email = email, PasswordHash = password, DisplayName = displayName, Country = country, Birthday = date, UserPhoto = userPhoto };
        //    user.BirthDate = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month)} {date.Day}";
        //    var apiUrl = $"{Config.Configurations.ApiPrefix}/signup"; // Api Method to Call with values
        //    apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API
        //    var content = await _httpClientHelper.PostAsync(apiUrl, user);
        //    return JsonConvert.DeserializeObject<ResultModel>(content);
        //}
        public async Task<ResultModel> SignUpAsync(string email, string password, string displayName, string country, DateTime date, string userPhoto, string accountType, string groupType, string ownerName)
        {
            TokketUser user = new TokketUser();
            user.Email = email;
            user.PasswordHash = password;
            user.DisplayName = displayName;
            user.Country = country;
            user.Birthday = date;
            if (accountType == "individual")
            {
                user.UserPhoto = userPhoto;
            }
            else
            {
                user.SubaccountPhoto = userPhoto;
                user.AccountType = accountType;
                user.GroupAccountType = groupType;
                user.SubaccountName = ownerName;
            }
            user.BirthDate = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month)} {date.Day}";
            _httpClientHelper.ClearHeaders();
            var apiUrl = $"{Config.Configurations.ApiPrefix}/signup"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API
            var res = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            var content = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultModel>(content);
        }
        public async Task<bool> UpdateFollowAsync(TokkepediaFollow tok)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/follow/{tok.Id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PutAsync(apiUrl, tok);
            //return response.IsSuccessStatusCode;
            return true;
        }
        public async Task<bool> UpdateUserBioAsync(string bio)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/userbio{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, bio);
            //return response.IsSuccessStatusCode;
            return true;
        }
        public async Task<bool> UpdateUserWebsiteAsync(string website)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/userwebsite{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, website);
            //return response.IsSuccessStatusCode;
            return true;
        }

        public async Task<ResultModel> UpdateUserCountryStateAsync(string country, string state = null)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            //Make sure "userid" and "token" are included as headers
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/usercountrystate"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API
            string[] cs = null;
            cs = string.IsNullOrEmpty(state) ? new string[] { country } : new string[] { country, state };
            var response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(cs), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultModel>(content);
        }

        #region Image
        public async Task<bool> UploadProfilePictureAsync(string base64)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokket");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "ios");

            var apiUrl = $"{Config.Configurations.ApiPrefix}/profilepicture{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";


            var dataAsString = JsonConvert.SerializeObject(base64);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

           

            if (content.Headers.Contains("token")) content.Headers.Remove("token");
            if (content.Headers.Contains("userid")) content.Headers.Remove("userid");
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("userid");

            try
            {
                var token = idtoken;
                content.Headers.Add("token", token);
                content.Headers.Add("userid", userid);
            }
            catch (Exception e) { }

            var holder = await _httpClientHelper.Instance.PostAsync(apiUrl, content);

            return  holder.IsSuccessStatusCode;

        }
        public async Task<string> UploadProfileCoverAsync(string base64)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/profilecover{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, base64);
            return response;
            //return await response.Content.ReadAsAsync<string>();
        }
        public async Task<bool> UpdateUserDisplayNameAsync(string displayName)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/userdisplayname{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            var response = await _httpClientHelper.PostAsync(apiUrl, displayName);
            return true;
            //return response.IsSuccessStatusCode;
        }
        #endregion
        public async Task<ResultModel> SendPasswordResetAsync(string email)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("email", email);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/sendpasswordreset"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API
            var response = await _httpClientHelper.PostAsync(apiUrl, email);

            ResultModel resultModel = new ResultModel();
            if (response.Contains("error"))
            {
                resultModel.ResultEnum = Helpers.Result.Failed;
                resultModel.ResultMessage = "Email not found.";
            }
            else
            {
                resultModel.ResultEnum = Helpers.Result.Success;
            }

            return resultModel;
            //return response.IsSuccessStatusCode;
        }
        public async Task<ResultData<TokketSubaccount>> GetSubaccountsAsync(string userId, string continuation = null)
        {
            _httpClientHelper.ClearHeaders();
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            //Make sure "userid" and "token" are included as headers
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userid);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/subaccounts/{userId}?serviceid={Config.Configurations.ServiceId}"; // Api Method to Call with values
            apiUrl += $"&code={Config.Configurations.ApiKey}"; // Add Suffix for API
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var item = await response.Content.ReadAsStringAsync();

            var dataresource = JsonConvert.DeserializeObject<ResultModel>(item);
            var getter = JsonConvert.DeserializeObject<ResultData<TokketSubaccount>>(dataresource.ResultObject.ToString());
            return JsonConvert.DeserializeObject<ResultData<TokketSubaccount>>(dataresource.ResultObject.ToString());
        
        }
        public async Task<TokketSubaccount> GetSubaccountAsync(string id, string userId)
        {
            _httpClientHelper.ClearHeaders();
            var apiUrl = $"{Config.Configurations.ApiPrefix}/subaccount/{id}?userid={userId}&serviceid={Config.Configurations.ServiceId}"; // Api Method to Call with values
            apiUrl += $"&code={Config.Configurations.ApiKey}"; // Add Suffix for API
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var item = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<ResultModel>(item);
            return JsonConvert.DeserializeObject<TokketSubaccount>(dataresource.ResultObject.ToString());
        }
        public async Task<ResultData<TokketTitle>> GetGenericTitlesAsync(string paginationId)
        {
            var apiUrl = $"{Config.Configurations.ApiPrefix}/titlesgeneric{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            _httpClientHelper.ClearHeaders();
            if (!string.IsNullOrEmpty(paginationId))
                _httpClientHelper.Instance.DefaultRequestHeaders.Add("pagination_id", paginationId);

            HttpResponseMessage response = await _httpClientHelper.Instance.GetAsync(apiUrl).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultData<TokketTitle>>(content);
            return result;
        }
        public async Task<ResultData<TokketTitle>> GetTitlesAsync(string userId, string kind)
        {
            _httpClientHelper.ClearHeaders();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("kind", kind);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/titles/{userId}?serviceid={Config.Configurations.ServiceId}"; // Api Method to Call with values
            apiUrl += $"&code={Config.Configurations.ApiKey}"; // Add Suffix for API
            //_client.BaseAddress = new Uri($"{baseUrl}/subaccount/{id}?userid={userId}&serviceid={serviceId}&code={_apiKey}");
            var response = await _httpClientHelper.Instance.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var item =  JsonConvert.DeserializeObject<ResultModel>(content);
            if (item.ResultObject == null)
                return null;
            return JsonConvert.DeserializeObject<ResultData<TokketTitle>>(item.ResultObject.ToString());
        }

        public async Task<bool> SelectTitleAsync(string id)
        {
            _httpClientHelper.ClearHeaders();
            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetUserModel().UserId);

            var apiUrl = $"{Config.Configurations.ApiPrefix}/titleselect/{id}"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginSubaccountAsync(string userId, string subaccountId, string subaccountKey)
        {
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokkepedia"); //Valid: "tokket", "tokblitz", "tokkepedia"
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", userId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("subaccountid", subaccountId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("subaccountkey", subaccountKey);
            
            var apiUrl = $"{Config.Configurations.ApiPrefix}/subaccountlogin/"; // Api Method to Call with values
            apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(Settings.GetTokketUser()), Encoding.UTF8, "application/json"));

            var item = await response.Content.ReadAsStringAsync();
            var dataresource = JsonConvert.DeserializeObject<ResultModel>(item);
            return dataresource.ResultEnum == Helpers.Result.Success ? true : false;
        }

        public async Task<ResultModel> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("oldPassword", oldPassword);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("newPassword", newPassword);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/changepassword/{userId}"; // Api Method to Call with values
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultModel>(content);
        }


        public async Task<bool> DeleteUserAsync(string id)
        {
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", id);
            var apiUrl = $"{Config.Configurations.ApiPrefix}/user/{id}"; // Api Method to Call with values     
            HttpResponseMessage response = await _httpClientHelper.Instance.DeleteAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendEmailVerificationAsync(string email, string idToken)
        {
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("email", email);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idToken);
            var uri = $"{Config.Configurations.ApiPrefix}/sendemailverification{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}";
            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsync(uri, new StringContent(email, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
