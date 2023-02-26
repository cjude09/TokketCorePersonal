using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class GetUsersAdminResponse : BaseResponse<List<AuthenticatedUser>>
    {

        public override List<AuthenticatedUser> Result { get; set; }
    }
}
