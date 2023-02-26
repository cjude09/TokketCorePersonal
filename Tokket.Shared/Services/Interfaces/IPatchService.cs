using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Core;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IPatchService
    {
        Task<bool> UpdateUserPointsSymbolEnabledAsync(bool isEnabled, TokketUser User);
    }
}
