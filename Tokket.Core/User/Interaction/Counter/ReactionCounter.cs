//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;

    /// <summary>Base class for Reaction counters. Counts the number of reactions in the tok.</summary>
    public class ReactionCounter : BaseModel
    {
        /// <summary>Doc type (label): subaccount</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "reactioncounter";

        /// <summary>User id of the tok owner.</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

        /// <summary>Tok id.</summary>
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        ///// <summary>Id in stream.io</summary>
        //[JsonProperty(PropertyName = "activity_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string ActivityId { get; set; } = null;
    }

    /// <summary>Number of views on a tok. Tile Tap views should be added asynchronously AFTER the tile is tapped, page visit views are added before page is loaded. Any views should be a separate document so that the list of unique viewers can be seen.
    /// Partition Key: {tokid}-views
    /// </summary>
    public class ViewCounter : ReactionCounter
    {
        /// <summary>Counter kind: viewcounter, gemcounter, commentcounter, replycounter</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "viewcounter";

        /// <summary>Number of tile tap views by other people. Excludes tile tap views from the logged in user.</summary>
        [JsonProperty(PropertyName = "tiletap_views", NullValueHandling = NullValueHandling.Ignore)]
        public long? TileTapViews { get; set; } = null;

        /// <summary>Number of page visit views by other people, such as /tok. Excludes page visit views from the logged in user.</summary>
        [JsonProperty(PropertyName = "pagevisit_views", NullValueHandling = NullValueHandling.Ignore)]
        public long? PageVisitViews { get; set; } = null;

        /// <summary>Number of tile tap views by the logged in user.</summary>
        [JsonProperty(PropertyName = "tiletap_views_personal", NullValueHandling = NullValueHandling.Ignore)]
        public long? TileTapViewsPersonal { get; set; } = null;

        /// <summary>Number of page visit views by the logged in user.</summary>
        [JsonProperty(PropertyName = "pagevisit_views_personal", NullValueHandling = NullValueHandling.Ignore)]
        public long? PageVisitViewsPersonal { get; set; } = null;
    }

    //From now on replies and likes will be stored directly (to improve GetComments api)
    ///// <summary>Use for comments only. Reply reactions are likes and replies on a comment. Partition Key: {commentid}-counter</summary>
    //public class ReplyCounter : ReactionCounter
    //{
    //    /// <summary>Counter kind:  replycounter</summary>
    //    [JsonProperty(PropertyName = "kind")]
    //    public string Kind { get; set; } = "replycounter";

    //    /// <summary>User id of the comment owner.</summary>
    //    [JsonProperty(PropertyName = "parent_user", NullValueHandling = NullValueHandling.Ignore)]
    //    public string ParentUser { get; set; } = null;

    //    /// <summary>Comment id.</summary>
    //    [JsonProperty(PropertyName = "parent_item", NullValueHandling = NullValueHandling.Ignore)]
    //    public string ParentItem { get; set; } = null;

    //    /// <summary>JSON/Dictionary field name: "like"</summary>
    //    [JsonProperty(PropertyName = "like", NullValueHandling = NullValueHandling.Ignore)]
    //    public long? Likes { get; set; } = null;

    //    /// <summary>JSON field name: "comment"</summary>
    //    [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
    //    public long? Comments { get; set; } = null;

    //    /// <summary>JSON field name: "deleted_likes"</summary>
    //    [JsonProperty(PropertyName = "deleted_like", NullValueHandling = NullValueHandling.Ignore)]
    //    public long? DeletedLikes { get; set; } = null;

    //    /// <summary>JSON field name: "deleted_comment"</summary>
    //    [JsonProperty(PropertyName = "deleted_comment", NullValueHandling = NullValueHandling.Ignore)]
    //    public long? DeletedComments { get; set; } = null;
    //}

    /// <summary>Number of gems given to a tok. GemA = Valuables, GemB = Brilliants, GemC = Preciouses
    /// Partition Key: {tokid}-gems</summary>
    public class GemCounter : ReactionCounter
    {
        /// <summary>Counter kind: viewcounter, gemcounter, commentcounter, replycounter</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "gemcounter";

        #region Old field names. Will be using gemA for future flexibility if the name ever changes.
        //[JsonProperty(PropertyName = "valuables", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Valuables { get; set; } = null;

        //[JsonProperty(PropertyName = "brilliants", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Brilliants { get; set; } = null;

        //[JsonProperty(PropertyName = "preciouses", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Preciouses { get; set; } = null;

        //[JsonProperty(PropertyName = "deleted_valuables", NullValueHandling = NullValueHandling.Ignore)]
        //public long? DeletedValuables { get; set; } = null;

        //[JsonProperty(PropertyName = "deleted_brilliants", NullValueHandling = NullValueHandling.Ignore)]
        //public long? DeletedBrilliants { get; set; } = null;

        //[JsonProperty(PropertyName = "deleted_preciouses", NullValueHandling = NullValueHandling.Ignore)]
        //public long? DeletedPreciouses { get; set; } = null;
        #endregion

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
    }

    /// <summary>Use for toks only. Number of comments given to a tok.
    /// Partition Key: {tokid}-comments</summary>
    public class CommentCounter : ReactionCounter
    {
        /// <summary>Counter kind: viewcounter, gemcounter, commentcounter, replycounter</summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; } = "commentcounter";

        #region Tok level
        /// <summary>JSON/Dictionary field name: "accurate"</summary>
        [JsonProperty(PropertyName = "accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "inaccurate"</summary>
        [JsonProperty(PropertyName = "inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "comment"</summary>
        [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        /// <summary>Total number of ratings. JSON/Dictionary field name: "star_ratings"</summary>
        [JsonProperty(PropertyName = "star_ratings", NullValueHandling = NullValueHandling.Ignore)]
        public long? StarRatings { get; set; } = null;

        /// <summary>Average of all ratings. JSON/Dictionary field name: "star_ratings"</summary>
        [JsonProperty(PropertyName = "star_rating", NullValueHandling = NullValueHandling.Ignore)]
        public double? StarRating { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "deleted_accurate"</summary>
        [JsonProperty(PropertyName = "deleted_accurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "deleted_inaccurate"</summary>
        [JsonProperty(PropertyName = "deleted_inaccurate", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "deleted_comment"</summary>
        [JsonProperty(PropertyName = "deleted_comment", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        /// <summary>Total number of ratings. JSON/Dictionary field name: "star_ratings"</summary>
        [JsonProperty(PropertyName = "deleted_star_ratings", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedStarRatings { get; set; } = null;
        #endregion


        #region Detail
        /// <summary>JSON/Dictionary field name: "accurate1"</summary>
        [JsonProperty(PropertyName = "accurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail1 { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "inaccurate1"</summary>
        [JsonProperty(PropertyName = "inaccurate1", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail1 { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "comment1"</summary>
        [JsonProperty(PropertyName = "comment1", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommentsDetail1 { get; set; } = null;

        //Detail 2
        /// <summary>JSON/Dictionary field name: "accurate2"</summary>
        [JsonProperty(PropertyName = "accurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccuratesDetail2 { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "inaccurate2"</summary>
        [JsonProperty(PropertyName = "inaccurate2", NullValueHandling = NullValueHandling.Ignore)]
        public long? InaccuratesDetail2 { get; set; } = null;

        /// <summary>JSON/Dictionary field name: "comment2"</summary>
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
    }
}
