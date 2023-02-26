//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    /// <summary>
    /// Base class for receipts on all platforms
    /// </summary>
    public class Receipt
    {
        /// <summary>
        /// The purchase Id. order.ProductId SKU
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The bundle Id of the app
        /// </summary>
        public string BundleId { get; set; }

        /// <summary>
        /// The transaction Id/ order.OrderId. NOTE: if null, it is a test purchase
        /// </summary>
        public string TransactionId { get; set; }
    }

    /// <summary>
    /// A receipt for Google Play in-app purchases
    /// </summary>
    public class GoogleReceipt : Receipt
    {
        /// <summary>
        /// The "developer payload" used on the purchase
        /// </summary>
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// The receipt data for Google Play
        /// </summary>
        public string PurchaseToken { get; set; }
    }

    /// <summary>
    /// A receipt for web paypal
    /// </summary>
    public class WebReceipt : Receipt
    {

    }
}
