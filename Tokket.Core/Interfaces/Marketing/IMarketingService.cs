using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core
{
    public interface IMarketingService
    {
        Task<GetReferralCodeResponse> GetReferralCodeAsync(GetReferralCodeRequest request);

        Task<GetMarketingCountersResponse> GetMarketingCountersAsync(GetMarketingCountersRequest request);

        Task<CreateReferralCodeResponse> CreateReferralCodeAsync(CreateReferralCodeRequest request);

        Task<UpdateMarketingCountersResponse> UpdateMarketingCountersAsync(UpdateMarketingCountersRequest request);
    }
}
