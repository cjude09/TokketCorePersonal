using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class PostUserLoginResponse : BaseResponse<AuthenticatedUser>
    {
        public override AuthenticatedUser Result { get; set; }
    }
}
