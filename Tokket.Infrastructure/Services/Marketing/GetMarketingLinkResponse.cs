using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Core;

namespace Tokket.Infrastructure
{
    public class GetReferralCodeResponse : BaseResponse<ReferralCode>
    {
        public override ReferralCode Result { get; set; }
    }

    public class GetMarketingCountersResponse : BaseResponse<List<MarketingUserCounter>>
    {
        public override List<MarketingUserCounter> Result { get; set; }
    }

    public class UpdateMarketingCountersResponse : BaseResponse<List<MarketingUserCounter>>
    {
        public override List<MarketingUserCounter> Result { get; set; }
    }

    public class CreateReferralCodeResponse : BaseResponse<ReferralCode>
    {
        public override ReferralCode Result { get; set; }
    }
}
