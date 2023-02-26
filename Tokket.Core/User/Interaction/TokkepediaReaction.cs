//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Reaction for Tokkepedia.</summary>
    public class TokkepediaReaction : BaseModel
    {
        /// <summary>Type of item. Will always be "reaction", use .Kind to specify the type of reaction</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "reaction";

        /// <summary>Kind of reaction. Kinds: valuable (5), brilliant (10), precious (15), accurate (5), inaccurate (-10), comment (0), treasure (250), like (comments only)
        /// Also includes tiletap_views, pagevisit_views, tiletap_views_personal, pagevisit_views_personal</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>User's id.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        /// <summary>Tok id.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        ///// <summary>Id in stream.io</summary>
        //[JsonProperty(PropertyName = "activity_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string ActivityId { get; set; } = null;

        /// <summary>category_id of the tok. Used to filter a user's reactions by category.</summary>
        [JsonProperty(PropertyName = "category_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryId { get; set; } = null;

        /// <summary>tok_type_id of the tok. Used to filter a user's reactions by tok type.</summary>
        [JsonProperty(PropertyName = "tok_type_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TokTypeId { get; set; } = null;

        /// <summary>Id of the user who originally posted the tok.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "owner_id")]
        public string OwnerId { get; set; } = "";

        /// <summary>Id of the purchase record.</summary>
        [JsonProperty(PropertyName = "purchase_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PurchaseId { get; set; } = null;

        /// <summary>If true reaction is a comment. If false it is something else.</summary>
        [JsonProperty(PropertyName = "is_comment")]
        public bool IsComment { get; set; } = false;

        /// <summary>If true reaction is either a like on a comment or a reply to a comment.</summary>
        [JsonProperty(PropertyName = "is_child")]
        public bool IsChild { get; set; } = false;

        /// <summary>If true, the reaction has a child.</summary>
        [JsonProperty(PropertyName = "has_children")]
        public bool HasChildren { get; set; } = false;

        /// <summary>All/some childrens (mostly comments/replies)</summary>
        [JsonProperty(PropertyName = "children")]
        public List<TokkepediaReaction> Children { get; set; }

        /// <summary>True if the logged in user liked the comment. (Makes it so Children doesn't have to be searched.</summary>
        [JsonProperty(PropertyName = "user_liked")]
        public bool UserLiked { get; set; } = false;

        /// <summary>Contains the user's like if any. This makes it easier to delete (unlike) later.</summary>
        [JsonProperty(PropertyName = "user_like")]
        public List<TokkepediaReaction> UserLike { get; set; }

        #region Tokmoji
        /// <summary>Id of the tokmojis. They need to be in order from first to last used.</summary>
        [JsonProperty(PropertyName = "tokmoji_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiIds { get; set; } = null;

        /// <summary>Each tokmoji needs to be connected to a valid purchase.</summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIds { get; set; } = null;

        /// <summary>Tokmoji disabled if null or false. Tokmoji can only show if this field is true, otherwise any tokmoji are not shown.</summary>
        [JsonProperty(PropertyName = "tokmoji_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TokmojiEnabled { get; set; } = null;
        #endregion

        #region Detail fields
        /// <summary>If true reaction is a detailed reaction.</summary>
        [JsonProperty(PropertyName = "is_detail_reaction")]
        public bool IsDetailReaction { get; set; } = false;

        [JsonProperty(PropertyName = "detail_num")]
        public long DetailNum { get; set; } = 0;
        #endregion

        #region Comment fields
        /// <summary>Comment/reply text. Leave null if not a comment</summary>
        [JsonProperty(PropertyName = "text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; } = null;

        /// <summary>If the comment has been edited at least once.</summary>
        [JsonProperty(PropertyName = "is_edited")]
        public bool IsEdited { get; set; } = false;
        #endregion

        #region Child fields
        /// <summary>Id of the original comment's user this is a reply to.</summary>
        [JsonProperty(PropertyName = "parent_user", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentUser { get; set; } = null;

        /// <summary>Id of the original comment this is a reply to.</summary>
        [JsonProperty(PropertyName = "parent_item", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentItem { get; set; } = null;
        #endregion


        #region Counter fields
        /// <summary>Number of replies.</summary>
        [JsonProperty("children_counts", NullValueHandling = NullValueHandling.Ignore)]
        public int ChildrenCounts { get; set; } = 0;

        [JsonProperty("deleted_children_counts", NullValueHandling = NullValueHandling.Ignore)]
        public int DeletedChildrenCounts { get; set; } = 0;

        /// <summary>Number of likes. (Comments only)</summary>
        [JsonProperty(PropertyName = "likes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Likes { get; set; } = null;

        /// <summary>Number of replies.</summary>
        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        /// <summary>Number of deleted likes. (Comments only)</summary>
        [JsonProperty(PropertyName = "deleted_likes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedLikes { get; set; } = null;

        /// <summary>Number of deleted replies.</summary>
        [JsonProperty(PropertyName = "deleted_comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        /// <summary>Number of star ratings.</summary>
        [JsonProperty(PropertyName = "star_ratings_count", NullValueHandling = NullValueHandling.Ignore)]
        public double? StarRatingsCount { get; set; } = null;

        /// <summary>Star rating. Max is 5.0, min is 1.0</summary>
        [JsonProperty(PropertyName = "star_rating", NullValueHandling = NullValueHandling.Ignore)]
        public double? StarRating { get; set; } = null;

        ///// <summary>Number of likes. (Comments only)</summary>
        //[JsonProperty(PropertyName = "likes", NullValueHandling = NullValueHandling.Ignore)]
        //public long Likes { get; set; } = 0;
        #endregion

        #region Update or Retrieve separately or update in case changes are made

        #region User
        /// <summary>If the user's account is disabled (content no longer accessible by the user, and they cannot login)</summary>
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; } = null;

        /// <summary>User's display name.</summary>
        [JsonProperty(PropertyName = "user_display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string UserDisplayName { get; set; } = null;

        /// <summary>User's profile image.</summary>
        [JsonProperty(PropertyName = "user_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPhoto { get; set; } = null;

        /// <summary>User's header image.</summary>
        [JsonProperty(PropertyName = "cover_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPhoto { get; set; } = null;

        /// <summary>User's biography.</summary>
        [MaxLength(160)]
        [JsonProperty(PropertyName = "user_bio", NullValueHandling = NullValueHandling.Ignore)]
        public string UserBio { get; set; } = null;

        /// <summary>User's website.</summary>
        [MaxLength(100)]
        [DataType(DataType.Url)]
        [JsonProperty(PropertyName = "user_website", NullValueHandling = NullValueHandling.Ignore)]
        public string UserWebsite { get; set; } = null;

        /// <summary>User's country id (ISO code).</summary>
        [MaxLength(5)]
        [JsonProperty(PropertyName = "user_country", NullValueHandling = NullValueHandling.Ignore)]
        public string UserCountry { get; set; } = null;

        /// <summary>User's state abbreviation. Only required if <see cref="UserCountry"/> is United States.</summary>
        [MaxLength(5)]
        [JsonProperty(PropertyName = "user_state", NullValueHandling = NullValueHandling.Ignore)]
        public string UserState { get; set; } = null;

        /// <summary>User's account type (account_type). Can only be "individual" or "group"</summary>
        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; } = "individual";
        #endregion

        #region Titles
        /// <summary>Id of the user's currently selected title. Not case sensitive and max of 25 characters</summary>
        [JsonProperty(PropertyName = "title_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleId { get; set; } = null;

        /// <summary>Display of the user's currently selected title. Case sensitive and max of 25 characters</summary>
        [JsonProperty(PropertyName = "title_display", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleDisplay { get; set; } = null;

        /// <summary>True if the user's currently selected title is unique.</summary>
        [JsonProperty(PropertyName = "title_unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleUnique { get; set; } = null;

        /// <summary>True if the user's currently selected title is enabled.</summary>
        [JsonProperty(PropertyName = "title_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleEnabled { get; set; } = null;
        #endregion

        #region Subaccount
        // The user's currently selected subaccount fields. These fields are retrieved through GetUserAsync. If not specified the owner's fields will be in there

        /// <summary>User's account type (group_account_type). Can only be "family" or "organization"</summary>
        [JsonProperty(PropertyName = "group_account_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupAccountType { get; set; } = null;

        /// <summary>Currently selected subaccount's id.</summary>
        [JsonProperty(PropertyName = "subaccount_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountId { get; set; } = null;

        /// <summary>Currently selected subaccount's display name.</summary>
        [JsonProperty(PropertyName = "subaccount_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountName { get; set; } = null;

        /// <summary>Currently selected subaccount's profile picture.</summary>
        [JsonProperty(PropertyName = "subaccount_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountPhoto { get; set; } = null;

        /// <summary>True if the currently selected subaccount is the owner.</summary>
        [JsonProperty(PropertyName = "subaccount_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SubaccountOwner { get; set; } = null;
        #endregion

        #endregion

        #region Categorical (internal or potentially used in the future)
        /// <summary>Id of the collection this item is stored in. Collections are currently for internal/corporate/curated items such as quote of the hour.</summary>
        [JsonProperty(PropertyName = "collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; } = null;

        /// <summary>Tags are currently for internal/corporate/curated items.</summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; } = null;

        /// <summary>Is the item 'not safe for work'</summary>
        [JsonProperty(PropertyName = "nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NSFW { get; set; } = null;
        #endregion
    }


    public class ReactionData
    {
        [JsonProperty(PropertyName = "owner")]
        public string OwnerId { get; set; } = "";

        [JsonProperty(PropertyName = "is_detail_reaction")]
        public bool IsDetailReaction { get; set; } = false;

        [JsonProperty(PropertyName = "is_comment")]
        public bool IsComment { get; set; } = false;

        [JsonProperty(PropertyName = "is_child")]
        public bool IsChild { get; set; } = false;

        #region Detail fields
        [JsonProperty(PropertyName = "detail_num")]
        public long DetailNum { get; set; } = 0;
        #endregion

        #region Comment fields
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "is_edited")]
        public bool IsEdited { get; set; }
        #endregion

        #region Child fields
        [JsonProperty(PropertyName = "parent_user")]
        public string ParentUser { get; set; } = "";

        [JsonProperty(PropertyName = "parent_item")]
        public string ParentItem { get; set; } = "";
        #endregion
    }

    /// <summary>Stores reaction counts.</summary>
    public class ReactionCounts : ReactionCounter
    {
        #region Tok level
        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gema", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemA { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemb", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemB { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemc", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemC { get; set; } = null;

        /// <summary>Number of treasure.</summary>
        [JsonProperty(PropertyName = "treasure", NullValueHandling = NullValueHandling.Ignore)]
        public long? Treasure { get; set; } = null;
        #endregion

        #region Comment reactions
        [JsonProperty(PropertyName = "accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        #region Detail
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail1 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail1 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment1", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail1 { get; set; } = null;

        //Detail 2
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail2 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail2 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment2", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail2 { get; set; } = null;

        //Detail 3
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail3 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail3 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment3", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail3 { get; set; } = null;

        //Detail 4
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail4 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail4 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment4", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail4 { get; set; } = null;

        //Detail 5
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail5 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail5 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment5", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail5 { get; set; } = null;

        //Detail 6
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail6 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail6 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment6", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail6 { get; set; } = null;

        //Detail 7
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail7 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail7 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment7", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail7 { get; set; } = null;

        //Detail 8
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail8 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail8 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment8", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail8 { get; set; } = null;

        //Detail 9
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail9 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail9 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment9", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail9 { get; set; } = null;

        //Detail 10
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "accurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail10 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "inaccurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail10 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "comment10", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail10 { get; set; } = null;
        #endregion

        #region Detail Deleted 
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail1 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail1 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment1", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail1 { get; set; } = null;

        //Detail 2
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail2 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail2 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment2", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail2 { get; set; } = null;

        //Detail 3
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail3 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate3", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail3 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment3", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail3 { get; set; } = null;

        //Detail 4
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail4 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate4", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail4 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment4", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail4 { get; set; } = null;

        //Detail 5
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail5 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate5", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail5 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment5", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail5 { get; set; } = null;

        //Detail 6
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail6 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate6", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail6 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment6", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail6 { get; set; } = null;

        //Detail 7
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail7 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate7", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail7 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment7", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail7 { get; set; } = null;

        //Detail 8
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail8 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate8", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail8 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment8", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail8 { get; set; } = null;

        //Detail 9
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail9 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate9", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail9 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment9", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail9 { get; set; } = null;

        //Detail 10
        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_accurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccuratesDetail10 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate10", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccuratesDetail10 { get; set; } = null;

        /// <summary>Number of comments.</summary>
        [JsonProperty(PropertyName = "deleted_comment10", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCommentsDetail10 { get; set; } = null;
        #endregion

        #endregion
    }
}
