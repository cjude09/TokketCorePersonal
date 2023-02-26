using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;

namespace Tokket.Shared.Models
{
    public class ResultModel
    {
        public ResultModel()
        {
            ResultEnum = Helpers.Result.None;
        }
        public Helpers.Result ResultEnum { get; set; }
        public string ResultMessage { get; set; }
        public object ResultObject { get; set; }
    }
}
