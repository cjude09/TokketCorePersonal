using Tokket.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Tokket.Infrastructure
{
    public class AccountService : IAccountService
    {
        public  IAuthService _authService;
        public  IDatabaseService _dbService;
        public FirebaseAuthService _firebaseAuthService;

        public AccountService(IDatabaseService dbService, IAuthService authService,FirebaseAuthService firebaseAuth)
        {
            _dbService = dbService;
            _authService = authService;
            _firebaseAuthService = firebaseAuth;
        }


        bool _isLoggedIn = false;
        //false by default. Set to true to make it easier to edit UIs

        private AuthenticatedUser _authenticatedUser = new AuthenticatedUser() { Role = Roles.Level1 };

        public bool IsLevel0() => _authenticatedUser?.Role == Roles.Level0;
        public bool IsLevel1() => _authenticatedUser?.Role == Roles.Level1 || _authenticatedUser?.Role == Roles.Level0;

        public bool IsLevel2() => _authenticatedUser?.Role == Roles.Level1 || _authenticatedUser?.Role == Roles.Level0;

        public bool IsLevel3() => _authenticatedUser?.Role == Roles.Level1 || _authenticatedUser?.Role == Roles.Level0;
        public bool IsNone() => _authenticatedUser?.Role == Roles.None;

        public bool IsLoggedIn()
        {
            return _isLoggedIn;
        }

        public AuthenticatedUser GetAuthenticatedUser()
        {
            if (IsLevel0() || IsLevel1())
                return _authenticatedUser;
            else
                return _authenticatedUser;
        }

        public PostUserLoginResponse PostUserLogin(PostUserLoginRequest request)
        {
            throw new NotImplementedException();
            return null;
        }

        public async Task<PostUserLoginResponse> PostUserLoginAsync(PostUserLoginRequest request)
        {
            //Login to token provider
            var response = await _authService.PostAuthorizationLoginAsync(new PostAuthorizationLoginRequest() { Email = request.Email, Password = request.Password });

            //Get user from db
            var dbResponse = await _dbService.GetItemAsync<AuthenticatedUser>(new GetItemRequest() { Id = response.Result.Id, PartitionKey = response.Result.Id });

            if (request.ChangeCurrentlyLoggedInUser)
            {
                _authenticatedUser = dbResponse.Result;
                _isLoggedIn = true;
            }

            return new PostUserLoginResponse() { Result = dbResponse.Result };
        }

        public async Task PostUserLogoutAsync()
        {
            _isLoggedIn = false;
            _authenticatedUser = null;
        }
    }
}
