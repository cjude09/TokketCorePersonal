using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core.Interfaces.Analytics
{
    public interface IAnalyticsLogService
    {
        Task<TrackAnalyticsResponse<T>> TrackAnalyticsEvent<T>(TrackAnalyticsRequest<T> request);
    }

    public class TrackAnalyticsRequest<T>
    {
    }

    public class TrackAnalyticsResponse<T>
    {
    }
}
