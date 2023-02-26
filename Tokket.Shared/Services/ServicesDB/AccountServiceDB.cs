using System;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core;
using Tokket.Core.Tools;
using FirebaseAdmin.Auth;
using AuthorizationTokenModel = Tokket.Shared.Models.AuthorizationTokenModel;

namespace Tokket.Shared.Services
{
    public class AccountServiceDB : Tokket.Shared.Services.Interfaces.IAccountService
    {
        private Tokket.Infrastructure.IDatabaseService cosmosSetup;
        private Tokket.Infrastructure.FirebaseAuthService firebaseAuth;
        public static AccountServiceDB Instance = new AccountServiceDB();
        public static Tokket.Infrastructure.AccountService _accountService;
        public readonly FirebaseAdmin.Auth.FirebaseAuth FirebaseAppAdmin;

        public AccountServiceDB() {
            if (_accountService == null)
            {
                cosmosSetup = new Tokket.Infrastructure.CosmosDBService(new Tokket.Infrastructure.ApiOptions(isprod: Config.Configurations.isProd));
                firebaseAuth = new Tokket.Infrastructure.FirebaseAuthService(new Tokket.Infrastructure.ApiOptions(isprod: Config.Configurations.isProd));
                _accountService = new Tokket.Infrastructure.AccountService(cosmosSetup, firebaseAuth, firebaseAuth);
            }
            else {
                cosmosSetup = _accountService._dbService;
                firebaseAuth = _accountService._firebaseAuthService;
            }
             
           
        }

      
        public async Task<AuthenticatedUser> Login(LoginModel model)
        {
            //_httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            //_httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", "tokkepedia");
          //  TokketUser user = new TokketUser() { Email = model.Username, PasswordHash = model.Password };
           // ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Failed };

           // var apiUrl = $"{Config.Configurations.ApiPrefix}/login"; // Api Method to Call with values
           // apiUrl += $"{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}"; // Add Suffix for API
          
            var content = await _accountService.PostUserLoginAsync(new PostUserLoginRequest() { Email = model.Username,Password = model.Password, ServiceId ="tokkepedia", ChangeCurrentlyLoggedInUser = true });
            var result = content.Result;
            return result;
        }

        public bool IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public bool IsLevel0()
        {
            throw new NotImplementedException();
        }

     

        public AuthenticatedUser GetAuthrenticatedUser()
        {
            throw new NotImplementedException();
        }

        public PostUserLoginResponse PostUserLogin(PostUserLoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task PostUserLogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PostUserLoginResponse> PostUserLoginAsync(PostUserLoginRequest request)
        {
            throw new NotImplementedException();
        }

        async Task<ResultModel> Interfaces.IAccountService.Login(LoginModel model)
        {

            //var content = await _accountService.PostUserLoginAsync(new PostUserLoginRequest() { Email = model.Username, Password = model.Password, ServiceId = "tokkepedia", ChangeCurrentlyLoggedInUser = true });
            //var resultM = content.Result;
            //var config = new MapperConfiguration(cfg => { 
            //    cfg.CreateMap<>
            //});

            //return result;
            throw new NotImplementedException();
        }

        public async Task<ResultModel> VerifyTokenAsync(string token, string refreshtoken)
        {
            ResultModel result = new ResultModel() { ResultEnum = Helpers.Result.Forbidden, ResultMessage = "Invalid Token. " };
            var userid = Settings.GetUserModel().UserId;

            AuthorizationTokenModel model = new AuthorizationTokenModel();
            model.IdToken = token;
            model.RefreshToken = refreshtoken;
            model.HashKey = HashGenerator.GenerateCustomHash("T0KK3T" + userid, 512, 24);
            model.UserId = Settings.GetUserModel().UserId;

            //_logger.LogInformation("Verifying Token...");

            try
            {
                FirebaseToken decodedToken = await firebaseAuth.FirebaseAppAdmin.VerifyIdTokenAsync(token);
                if (decodedToken.Uid != userid)
                    result.ResultMessage += "Token Mismatch.";

                // If it goes here, the request is valid
                result.ResultObject = new AuthorizationTokenModel() { UserId = decodedToken.Uid };
                result.ResultEnum = Helpers.Result.Success;
                result.ResultMessage = "Valid Token.";
            }
            catch (Exception ex)
            {
                result.ResultEnum = Helpers.Result.Forbidden;
                result.ResultMessage += ex.Message;
            }

            return result;
        }

        public ResultModel VerifyToken(string token, string refreshtoken)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<TokketUser> GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public TokketUser GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> SignUpAsync(string email, string password, string displayName, string country, DateTime date, string userPhoto, string accountType, string groupType, string ownerName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserBioAsync(string bio)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserWebsiteAsync(string website)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateUserCountryStateAsync(string country, string state = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadProfilePictureAsync(string base64)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadProfileCoverAsync(string base64)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserDisplayNameAsync(string displayName)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> SendPasswordResetAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ResultData<TokketSubaccount>> GetSubaccountsAsync(string userId, string continuation = null)
        {
            throw new NotImplementedException();
        }

        public Task<TokketSubaccount> GetSubaccountAsync(string id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultData<TokketTitle>> GetGenericTitlesAsync(string paginationId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultData<TokketTitle>> GetTitlesAsync(string userId, string kind)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SelectTitleAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginSubaccountAsync(string userId, string subaccountId, string subaccountKey)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultData<TokketUser>> GetUsers(string displayname, string accounttype)
        {
            throw new NotImplementedException();
        }

        public Task<TokketUser> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailVerificationAsync(string email, string idToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResultData<TokketUser>> GetUsersAsync(string serviceId, UserQueryValues values)
        {
            throw new NotImplementedException();
        }

    
    }
}
