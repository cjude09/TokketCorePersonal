using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models.Purchase;
using Tokket.Core;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResult> PurchaseGroupAsync(GoogleReceipt receipt, TokketUser groupAccount);
        Task<PurchaseResult> PurchaseTitleAsync(GoogleReceipt receipt, string titleId, bool isUnique = false);

        Task<PurchaseResult> PurchaseSubaccountAsync(GoogleReceipt receipt, string subaccountName, string subaccountKey);
        Task<bool> BillingStart(string productId, PurchaseModel product, TokketUser tokket = null, string subaccountName = "", string subaccountKey = "", string titleId = "", bool isUnique = false);
    }
}
