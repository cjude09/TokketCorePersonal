using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models.Base;

namespace Tokket.Shared.Models
{
    public class UserModel:BaseModel
    {
        public string Id { get; set; }
        public string FederatedId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }

        public string PhotoUrl { get; set; }

        public string PhoneNumber { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string StreamToken { get; set; }
    }
}
