//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Values to query for sections in a Mega Tok.</summary>
    public class TokSectionQueryValues
    {
        /// <summary>Tok id.</summary>
        public string tokId​ { get; set; }

        /// <summary>Number of toks to get. Do not change when loading more.</summary>
        public int count​ { get; set; }

        /// <summary>Used for loading more sections.</summary>
        public string continuationToken​ { get; set; }

        /// <summary>Only is "number" is supported right now.</summary>
        public string order { get; set; } = "number";
    }
}
