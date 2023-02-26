using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{
    public class GetReferralCodeRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level1; }

        public string? ReferralCode { get; set; }
    }

    public class GetMarketingCountersRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level1; }

        public string? OwnerId { get; set; }
    }

    public class CreateReferralCodeRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level1; }

        public string? ReferralCode { get; set; }

        public string? OwnerId { get; set; }
    }

    public class UpdateMarketingCountersRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level1; }

        public string? OwnerId { get; set; }

        public MarketingUserCounter Item { get; set; }
    }
}
