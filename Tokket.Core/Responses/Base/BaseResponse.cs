using System;
using System.Collections.Generic;
using System.Net;

namespace Tokket.Core
{
    public abstract class BaseResponse<T>
    {
        public int StatusCode { get; set; } = 200;

        public HttpStatusCode StatusCodeEnum { get; set; }

        public string? Message { get; set; } = null;

        public abstract T Result { get; set; }
    }
}
