using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models.Base;


namespace Tokket.Shared.Models
{
    public class LoginModel : BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
