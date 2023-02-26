using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core
{
    public interface IAccountService
    {
        bool IsLoggedIn();

        AuthenticatedUser GetAuthenticatedUser();

        PostUserLoginResponse PostUserLogin(PostUserLoginRequest request);

        Task PostUserLogoutAsync();
        Task<PostUserLoginResponse> PostUserLoginAsync(PostUserLoginRequest request);
    }
}
