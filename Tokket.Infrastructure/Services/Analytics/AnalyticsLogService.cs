using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Core.Interfaces.Analytics;

namespace Tokket.Infrastructure.Services.Analytics
{
    public class AnalyticsLogService : IAnalyticsLogService
    {
        public async Task<TrackAnalyticsResponse<T>> TrackAnalyticsEvent<T>(TrackAnalyticsRequest<T> request)
        {
            return new TrackAnalyticsResponse<T>();
        }
    }
}
