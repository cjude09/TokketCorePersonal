//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core.Tools
{
    public static class Validators
    {
        public static bool IsValidTok(this Tok tok)
        {
            if (string.IsNullOrEmpty(tok.TokGroup))
                return false;

            if (string.IsNullOrEmpty(tok.TokType))
                return false;

            if (string.IsNullOrEmpty(tok.Category))
                return false;

            if (string.IsNullOrEmpty(tok.PrimaryFieldText))
                return false;

            return true;
        }

        public static bool IsValidUser(this TokketUser tok)
        {
            if (string.IsNullOrEmpty(tok.DisplayName))
                return false;

            if (tok.Label != "user")
                return false;

            return true;
        }

        public static bool IsValidComment(this Comment item)
        {
            if (string.IsNullOrEmpty(item.CommentText))
                return false;

            return true;
        }

        public static bool IsValidSet(this Set item)
        {
            if (string.IsNullOrEmpty(item.Name))
                return false;

            return true;
        }

        public static bool IsValidFollow(this TokkepediaFollow item)
        {
            if (string.IsNullOrEmpty(item.FollowId))
                return false;

            return true;
        }

        public static bool IsValidReaction(this TokkepediaReaction item)
        {
            if (string.IsNullOrEmpty(item.ItemId) || string.IsNullOrEmpty(item.UserId) || string.IsNullOrEmpty(item.OwnerId))
                return false;

            return true;
        }

        public static bool IsValidReactionUpdate(this TokkepediaReaction newItem, TokkepediaReaction oldItem)
        {
            if (string.IsNullOrEmpty(newItem.Id) || string.IsNullOrEmpty(newItem.ItemId) || string.IsNullOrEmpty(newItem.UserId) || string.IsNullOrEmpty(newItem.OwnerId))
                return false;

            if (newItem.Id != oldItem.Id || newItem.UserId != oldItem.UserId || newItem.ItemId != oldItem.ItemId || newItem.OwnerId != oldItem.OwnerId)
                return false;

            return true;
        }

        //public static bool IsValidSubscriptionRecord(this SubscriptionRecord item)
        //{
        //    if (string.IsNullOrEmpty(item.SubscriptionId))
        //        return false;

        //    return true;
        //}

        public static bool IsValidTokUpdate(this Tok tok, Tok currentTok)
        {
            if (tok.TokGroup != currentTok.TokGroup)
                return false;

            if (tok.TokType != currentTok.TokType)
                return false;

            if (tok.Id != currentTok.Id)
                return false;

            if (tok.ActivityId != currentTok.ActivityId)
                return false;

            if (string.IsNullOrEmpty(tok.PrimaryFieldText))
                return false;

            return true;
        }
    }

    /// <summary>
    /// Validates labels, kinds, and actions
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// List of actions
        /// </summary>
        public static string[] ValidActions = new string[]
        {
            "create", "read", "update", "delete"
        };

        /// <summary>
        /// List of item kinds
        /// </summary>
        public static string[] ValidKinds = new string[]
        {
            "tok", "set", "user", "like", "dislike", "accurate", "inaccurate", "comment"
        };
    }
}
