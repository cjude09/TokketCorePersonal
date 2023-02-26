using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class FirebaseAuthModel
    {
        public string FirebaseToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime Created { get; set; }
        public UserModel User { get; set; }
        public bool IsExpired { get; set; }
    }
}
