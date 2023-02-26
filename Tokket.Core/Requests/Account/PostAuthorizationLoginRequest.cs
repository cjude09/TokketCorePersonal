using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class PostAuthorizationLoginRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.None; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
