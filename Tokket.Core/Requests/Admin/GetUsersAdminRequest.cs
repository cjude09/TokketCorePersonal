using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class GetUsersAdminRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level0; }
        public string UserId { get; set; }

        public string ContinuationToken { get; set; }

        public string RoleId
        {
            get => RoleId;
            set
            {
                if (Roles.ValidRoles.Contains(value))
                    RoleId = value;
                else
                    throw new InvalidOperationException($"Invalid role: {value}");
            }
        }
    }
}
