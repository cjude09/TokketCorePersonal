//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core.Tools
{
    using System.Collections.Generic;

    public class ResultData<T>
    {
        public long Limit { get; set; }
        public string ContinuationToken { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
