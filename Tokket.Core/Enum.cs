//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    //public enum CounterType { UserCounter, TokkepediaCounter, Category, TokType, TokTypeListCounter, Country, TokCounter, None };
    //public enum ItemType { Tok, Set, TokkepediaReaction, TokkepediaFollower, TokkepediaFollowing, None }; // TokkepediaFollow
    //public enum TimeframeType { Daily, Weekly, Monthly, Yearly, Overall, None };

    /// <summary>Kinds or reactions.</summary>
    public enum ReactionKind { GemA, GemB, GemC, Treasure, Accurate, Inaccurate, Comment, Report, None };

    public enum PaymentOptions
    {
        Unknown = 0,
        Stripe = 1,
        CreditCard,
        Paypal,
        Bank,
        WiredTransfer,
        OtherPaymentService,

        All = 99999
    }

    public enum TransactionType
    {
        Online = 1,
        Offline,

        All = 99999
    }

    public enum TransactionStatus
    {
        Unpaid = 1,
        Paid,
        Cancelled,
        Deleted,
        Partial,

        All = 99999
    }

    public enum HandlePosition
    {
        None,
        OptionA,
        OptionB,
        OptionC,
        OptionD
    }
}