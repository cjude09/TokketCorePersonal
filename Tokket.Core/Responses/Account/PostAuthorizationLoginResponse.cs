using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class PostAuthorizationLoginResponse : BaseResponse<AuthorizationTokenModel>
    {

        public override AuthorizationTokenModel Result { get; set; }
    }

    public class AuthorizationTokenModel
    {
        public string AuthToken { get; set; }

        public string RefreshToken { get; set; }

        public string Id { get; set; }

    }
}
