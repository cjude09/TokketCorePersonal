using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public abstract class BaseRequest
    {
        protected abstract string RequiredRole { get; }
        public string ServiceId { get; set; }
    }
}
