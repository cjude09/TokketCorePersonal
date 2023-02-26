using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public abstract class BaseAuthenticatedRequest : BaseRequest
    {
        protected string UserId { get; }

        protected string AuthToken { get; }
    }
}
