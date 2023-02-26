//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core.Tools
{
    using System.Collections.Generic;

    public static class Converters
    {
        /// <summary>
        /// Obsolete: Do not use.
        /// </summary>
        public static TokketUser SetCounts(this TokketUser user, UserCounter counter)
        {
            if (counter == null)
            {
                user.Toks = 0;
                user.Points = 0;
                user.Coins = 0;
                user.Sets = 0;
                //user.StrikesTokBlitz = 0;
                user.EliminatorsTokBlitz = 0;
                user.Reactions = 0;
                user.Accurates = 0;
                user.Inaccurates = 0;
                user.Comments = 0;
                user.Reports = 0;
                user.Followers = 0;
                user.Following = 0;
            }
            else
            {
                user.Toks = (counter?.Toks != null) ? (long)(counter?.Toks) : 0;
                user.Points = (counter?.Points != null) ? (long)(counter?.Points) : 0;
                user.Coins = (counter?.Coins != null) ? (long)(counter?.Coins) : 0;
                user.Sets = (counter?.Sets != null) ? (long)(counter?.Sets) : 0;
                user.EliminatorsTokBlitz = (counter?.EliminatorsTokBlitz != null) ? (long)(counter?.EliminatorsTokBlitz) : 0;
                user.Reactions = (counter?.Reactions != null) ? (long)(counter?.Reactions) : 0;
                user.Accurates = (counter?.Accurates != null) ? (long)(counter?.Accurates) : 0;
                user.Inaccurates = (counter?.Inaccurates != null) ? (long)(counter?.Inaccurates) : 0;
                user.Comments = (counter?.Comments != null) ? (long)(counter?.Comments) : 0;
                user.Reports = (counter?.Reports != null) ? (long)(counter?.Reports) : 0;
                user.Followers = (counter?.Followers != null) ? (long)(counter?.Followers) : 0;
                user.Following = (counter?.Following != null) ? (long)(counter?.Following) : 0;
            }

            return user;
        }

        public static TokkepediaFollowing ToTokkepediaFollowing(this TokkepediaFollow follow)
        {
            TokkepediaFollowing following = new TokkepediaFollowing()
            {
                CategoryId = follow.CategoryId,
                FeedLabel = follow.FeedLabel,
                FollowDisplayName = follow.FollowDisplayName,
                FollowId = follow.FollowId,
                FollowPhoto = follow.FollowPhoto,
                Id = follow.Id,
                Label = follow.Label,
                PartitionKey = follow.PartitionKey,
                Timestamp = follow.Timestamp,
                UserDisplayName = follow.UserDisplayName,
                UserId = follow.UserId,
                UserPhoto = follow.UserPhoto,
                _etag = follow._etag,
                _Timestamp = follow._Timestamp
            };

            return following;
        }

        public static TokkepediaFollower ToTokkepediaFollower(this TokkepediaFollow follow)
        {
            TokkepediaFollower follower = new TokkepediaFollower()
            {
                CategoryId = follow.CategoryId,
                FeedLabel = follow.FeedLabel,
                FollowDisplayName = follow.FollowDisplayName,
                FollowId = follow.FollowId,
                FollowPhoto = follow.FollowPhoto,
                Id = follow.Id,
                Label = follow.Label,
                PartitionKey = follow.PartitionKey,
                Timestamp = follow.Timestamp,
                UserDisplayName = follow.UserDisplayName,
                UserId = follow.UserId,
                UserPhoto = follow.UserPhoto,
                _etag = follow._etag,
                _Timestamp = follow._Timestamp
            };

            return follower;
        }

        public static TokketUser ToValidNewUser(this TokketUser item)
        {
            item.NoAdsTokBlitz = false;
            item.EliminatorsTokBlitz = 0;

            return item;
        }
    }
}
