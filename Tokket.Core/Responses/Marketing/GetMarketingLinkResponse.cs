using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
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
