using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core
{
    public interface IAuthService
    {
        Task<PostAuthorizationLoginResponse> PostAuthorizationLoginAsync(PostAuthorizationLoginRequest request);
    }
}
