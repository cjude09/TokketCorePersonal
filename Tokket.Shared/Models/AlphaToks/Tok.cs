using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using Tokket.Core;

namespace Tokket.Shared.Models.AlphaToks
{
    /// <summary>Contains all fields of Tok. Toks are the user generated posts of Tokkepedia.</summary>
    public class Tok : BaseModel
    {
        /// <summary>Type of item.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tok";

        /// <summary>A unique for each Activity in getstream.io. Different from <see cref="BaseModel.Id"/>, which is stored in foreign_id.</summary>
        [JsonProperty(PropertyName = "activity_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityId { get; set; } = null;

        /// <summary>Uniquely identifies the user who posted.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";

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

        #region Location
        /// <summary>Tok's country id (ISO code).</summary>
        [MaxLength(5)]
        [JsonProperty(PropertyName = "country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; } = null;

        /// <summary>Tok's state abbreviation. Only required if <see cref="UserCountry"/> is United States.</summary>
        [MaxLength(5)]
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; } = null;
        #endregion

        #region Color
        /// <summary>Main color in hex format (if null, then automatically or randomly selected). For toks this is the tok tile color.</summary>
        [MaxLength(7)]
        [JsonProperty(PropertyName = "color_main_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorMainHex { get; set; } = null;

        /// <summary>Color object.</summary>
        [JsonProperty(PropertyName = "color_main", NullValueHandling = NullValueHandling.Ignore)]
        public Color ColorMain { get; set; }
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

        #region Sticker
        /// <summary>Id of the tok's tile sticker</summary>
        [JsonProperty(PropertyName = "sticker", NullValueHandling = NullValueHandling.Ignore)]
        public string Sticker { get; set; } = null;

        /// <summary>Url of the tok's tile sticker image</summary>
        [JsonProperty(PropertyName = "sticker_image", NullValueHandling = NullValueHandling.Ignore)]
        public string StickerImage { get; set; } = null;
        #endregion

        #region Tokmoji
        /// <summary>Id of the tokmojis. They need to be in order from first to last used.</summary>
        [JsonProperty(PropertyName = "tokmoji_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiIds { get; set; } = null;

        /// <summary>Each tokmoji needs to be connected to a valid purchase. (Primary)</summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIds { get; set; } = null;

        /// <summary>Each tokmoji needs to be connected to a valid purchase. (Secondary)</summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids_secondary", NullValueHandling = NullValueHandling.Ignore)]
        public string[] TokmojiPurchaseIdsSecondary { get; set; }

        /// <summary>
        ///     Description:
        ///         Each tokmoji needs to be connected to a valid purchase. (Details)
        ///     Example:
        ///         1, { "purchase1", "purchase2" }
        ///         2, { "purchase3" }             
        ///         3, { "purchase4", "purchase5", "purchase6", "purchase7" }
        ///         ...
        ///         10, { "purchase8" }
        ///         
        ///         - Whereas 1 is the KEY and detail index of the detailed tok and the VALUE is a string of purchase ids.
        /// </summary>
        [JsonProperty(PropertyName = "tokmoji_purchase_ids_details", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<int, string[]> TokmojiPurchaseIdsDetails { get; set; }

        /// <summary>Tokmoji disabled if null or false. Tokmoji can only show if this field is true, otherwise any tokmoji are not shown.</summary>
        [JsonProperty(PropertyName = "tokmoji_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TokmojiEnabled { get; set; } = null;
        #endregion

        #region Patches (Referred to as "Points Symbol")
        /// <summary>True if the user wants to use a patch instead of a flag for toks.</summary>
        [JsonProperty(PropertyName = "is_pointssymbol_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPointsSymbolEnabled { get; set; } = null;

        /// <summary>Color to display</summary>
        [JsonProperty(PropertyName = "pointssymbol_color", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolColor { get; set; } = null;

        /// <summary>Id of the symbol</summary>
        [JsonProperty(PropertyName = "pointssymbol_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolId { get; set; } = null;

        /// <summary>Name of the symbol</summary>
        [JsonProperty(PropertyName = "pointssymbol_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolName { get; set; } = null;

        /// <summary>Image to display (only fill this in the client)</summary>
        [JsonProperty(PropertyName = "pointssymbol_image", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsSymbolImage { get; set; } = null;
        #endregion

        #region Categorical
        /// <summary>Tok Groups are a grouping that ensures necessary character limits, field names, and number of details are applied - it is a post format. See <see cref="TokTypeList"/> for more info.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }

        /// <summary>Tok types divide a tok group into more practical and specific groupings. Most of the time the tok type is still broad enough to have a wide variety of <see cref="Category"/></summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "tok_type")]
        public string TokType { get; set; }

        /// <summary>Uniquely identifies tok types and must include the tok group. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.TokType"/></summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "tok_type_id")]
        public string TokTypeId { get; set; }

        /// <summary>Categorizes the tok into a specific topic. It should be more specific than the <see cref="TokType"/>, and it should not contain anything in <see cref="PrimaryFieldText"/> or <see cref="Details"/>.</summary>
        [MaxLength(100)]
        [JsonRequired]
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        /// <summary>Uniquely identifies categories. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.Category"/></summary>
        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; }
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

        #region Standard Content
        /// <summary>Primary text field name. If possible use <see cref="Tokket.Core.Tools.TokGroupTool"/> to generate based on <see cref="TokGroup"/></summary>
        [JsonProperty(PropertyName = "primary_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryFieldName { get; set; }

        /// <summary>Primary field value. Maximum is 600 characters.</summary>
        [MaxLength(600)]
        [JsonRequired]
        [JsonProperty(PropertyName = "primary_text")]
        public string PrimaryFieldText { get; set; }

        /// <summary>Secondary field name.</summary>
        [JsonProperty(PropertyName = "secondary_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryFieldName { get; set; }

        /// <summary>Secondary field value.</summary>
        [JsonProperty(PropertyName = "secondary_text", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryFieldText { get; set; }

        /// <summary>Details for the tok. Leave null if tok is not detailed.</summary>
        [JsonProperty(PropertyName = "details", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Details { get; set; } = null;

        /// <summary>Images for each detail. Leave null if tok is not detailed.</summary>
        [JsonProperty(PropertyName = "detail_images", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DetailImages { get; set; } = null;
        #endregion

        #region English Translation fields
        /// <summary>Language of the tok. Default is "english".</summary>
        [JsonRequired]
        [JsonProperty("language")]
        public string Language { get; set; } = "english";

        /// <summary>Checks if the tok is in English.</summary>
        [JsonProperty(PropertyName = "is_english")]
        public bool IsEnglish { get; set; } = true;

        /// <summary>English Translation for the Primary field value. </summary>
        [JsonProperty(PropertyName = "english_primary_text", NullValueHandling = NullValueHandling.Ignore)]
        public string EnglishPrimaryFieldText { get; set; } = null;

        /// <summary>English Translation for the Secondary field value. </summary>
        [JsonProperty(PropertyName = "english_secondary_text", NullValueHandling = NullValueHandling.Ignore)]
        public string EnglishSecondaryFieldText { get; set; } = null;

        /// <summary>English Translation for the details. </summary>
        [JsonProperty(PropertyName = "english_details", NullValueHandling = NullValueHandling.Ignore)]
        public string[] EnglishDetails { get; set; } = null;
        #endregion

        #region Mega Tok​
        //All are null by default, and NullValueHandling.Ignore will make sure it's not written to database if null​

        /// <summary>Is the tok a mega tok</summary>
        [JsonProperty(PropertyName = "is_mega_tok", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMegaTok { get; set; } = null;

        /// <summary>Number of sections, at this moment there is no limit.</summary>
        [JsonProperty(PropertyName = "section_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? SectionCount​ { get; set; } = null;

        /// <summary>Largest section length, max is 150,000.</summary>
        [JsonProperty(PropertyName = "section_length_total", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionLengthTotal​ { get; set; } = null;

        /// <summary>Largest section length, max is 150,000.</summary>
        [JsonProperty(PropertyName = "section_length_largest", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionLength​Largest { get; set; } = null;

        /// <summary>Number of partitions needed for all sections. Maximum 5,000 sections per partition (10 GB / up to 200 KB)​. Only increase when new partition needed (over 5,000) and never decrease.</summary>
        [JsonProperty(PropertyName = "section_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionPartitions { get; set; } = null;

        /// <summary>The first 5 section titles.</summary>
        [JsonProperty(PropertyName = "section_titles", NullValueHandling = NullValueHandling.Ignore)]
        public string[] SectionTitles { get; set; } = null;

        //Will never be populated when querying for toks. Missing parts need to be lazy loaded)
        ///// <summary>Store all loaded sections here. </summary>
        //[JsonProperty(PropertyName = "sections", NullValueHandling = NullValueHandling.Ignore)]
        //public TokSection[] Sections { get; set; } = null;
        #endregion

        /// <summary>Required field values for the tok. Field names should be generated from the TokGroupTool</summary>
        [JsonProperty(PropertyName = "required_field_values", NullValueHandling = NullValueHandling.Ignore)]
        public string[] RequiredFieldValues { get; set; } = null;

        /// <summary>Optional field values for the tok. Field names should be generated from the TokGroupTool</summary>
        [JsonProperty(PropertyName = "optional_field_values", NullValueHandling = NullValueHandling.Ignore)]
        public string[] OptionalFieldValues { get; set; } = null;

        /// <summary>Additional notes for the tok.</summary>
        [JsonProperty(PropertyName = "notes", NullValueHandling = NullValueHandling.Ignore)]
        public string Notes { get; set; } = null;

        #region Media
        /// <summary>Image Id</summary>
        [JsonProperty(PropertyName = "image_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; } = null;

        /// <summary>Main image for the tok.</summary>
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;

        /// <summary>Image Height (in pixels)</summary>
        [JsonProperty(PropertyName = "image_height", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageHeight { get; set; } = null;

        /// <summary>Image Width (in pixels)</summary>
        [JsonProperty(PropertyName = "image_width", NullValueHandling = NullValueHandling.Ignore)]
        public int? ImageWidth { get; set; } = null;

        /// <summary>Image Extension (.jpg, .png, the period needs to be included)</summary>
        [JsonProperty(PropertyName = "image_extension", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageExtension { get; set; } = null;

        /// <summary>Video Id</summary>
        [JsonProperty(PropertyName = "video_id", NullValueHandling = NullValueHandling.Ignore)]
        public string VideoId { get; set; } = null;

        /// <summary>Main video for the tok.</summary>
        [JsonProperty(PropertyName = "video", NullValueHandling = NullValueHandling.Ignore)]
        public string Video { get; set; } = null;

        /// <summary>Image Height (in pixels)</summary>
        [JsonProperty(PropertyName = "video_height", NullValueHandling = NullValueHandling.Ignore)]
        public int? VideoHeight { get; set; } = null;

        /// <summary>Video Width (in pixels)</summary>
        [JsonProperty(PropertyName = "video_width", NullValueHandling = NullValueHandling.Ignore)]
        public int? VideoWidth { get; set; } = null;

        /// <summary>Video Duration (in seconds)</summary>
        [JsonProperty(PropertyName = "video_duration", NullValueHandling = NullValueHandling.Ignore)]
        public int? VideoDuration { get; set; } = null;

        /// <summary>Video Extension (.mp4, the period needs to be included)</summary>
        [JsonProperty(PropertyName = "video_extension", NullValueHandling = NullValueHandling.Ignore)]
        public string VideoExtension { get; set; } = null;

        /// <summary>Thumbnail for the tok.</summary>
        [JsonProperty(PropertyName = "video_thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string VideoThumbnail { get; set; } = null;

        /// <summary>Image Extension (.jpg, .png, the period needs to be included)</summary>
        [JsonProperty(PropertyName = "video_thumbnail_extension", NullValueHandling = NullValueHandling.Ignore)]
        public string VideoThumbnailExtension { get; set; } = null;

        /// <summary>Date and time when the event associated with the tok start. For Tok Blitz this refers to the invite 'day of the week'.</summary>
        [JsonProperty(PropertyName = "event_start_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EventStartTime { get; set; } = null;

        /// <summary>Date and time when the event associated with the tok ends.</summary>
        [JsonProperty(PropertyName = "event_end_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EventEndTime { get; set; } = null;
        #endregion

        /// <summary>Is the tok a detailed tok</summary>
        [JsonProperty(PropertyName = "is_detail_based")]
        public bool IsDetailBased { get; set; }

        /// <summary>Is the tok a replicated tok</summary>
        [JsonProperty(PropertyName = "is_replicated")]
        public bool IsReplicated { get; set; } = false;

        /// <summary>Is the tok a edited tok</summary>
        [JsonProperty(PropertyName = "is_edited")]
        public bool IsEdited { get; set; } = false;

        /// <summary>Is the tok a global tok</summary>
        [JsonProperty(PropertyName = "is_global")]
        public bool IsGlobal { get; set; } = true;

        /// <summary>Is the tok verified</summary>
        [JsonProperty(PropertyName = "verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVerified { get; set; } = null;


        #region Answer field (Only for Test)
        [JsonProperty(PropertyName = "has_answer_field", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasAnswerField { get; set; }

        [JsonProperty(PropertyName = "answer_field_number", NullValueHandling = NullValueHandling.Ignore)]
        public int? AnswerFieldNumber { get; set; }
        #endregion

        /// <summary>Indicates whether there's a gem reaction or not.</summary>
        [JsonProperty(PropertyName = "has_gem_reaction", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasGemReaction { get; set; } = null;

        #region Statistics
        //Statistics

        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "users_reacted", NullValueHandling = NullValueHandling.Ignore)]
        public long? UsersReacted { get; set; } = null;

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reports { get; set; } = null;

        [JsonProperty(PropertyName = "shares", NullValueHandling = NullValueHandling.Ignore)]
        public long? Shares { get; set; } = null;

        [JsonProperty(PropertyName = "views", NullValueHandling = NullValueHandling.Ignore)]
        public long? Views { get; set; } = null;

        //Cosmos DB Helpers
        [JsonProperty(PropertyName = "reaction_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionPartitions { get; set; } = null;

        #endregion

        #region Gem Reactions
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

        /// <summary>Number of gems deleted.</summary>
        [JsonProperty(PropertyName = "deleted_gema", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedGemA { get; set; } = null;

        /// <summary>Number of gems deleted.</summary>
        [JsonProperty(PropertyName = "deleted_gemb", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedGemB { get; set; } = null;

        /// <summary>Number of gems deleted.</summary>
        [JsonProperty(PropertyName = "deleted_gemc", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedGemC { get; set; } = null;
        #endregion

        #region Comment reactions
        [JsonProperty(PropertyName = "accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        //Details
        [JsonProperty(PropertyName = "accurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] AccuratesDetails { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] InaccuratesDetails { get; set; } = null;

        [JsonProperty(PropertyName = "comments_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] CommentsDetails { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] DeletedAccuratesDetails { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] DeletedInaccuratesDetails { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comments_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] DeletedCommentsDetails { get; set; } = null;
        #endregion

        #region Views
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
        #endregion

        [JsonProperty("latest_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<Tokket.Core.TokkepediaReaction>> LatestReactions { get; set; } = null;

        [JsonProperty("own_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<Tokket.Core.TokkepediaReaction>> OwnReactions { get; set; } = null;

        [JsonProperty("reaction_counts", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, int> ReactionCounts { get; set; } = null;
    }
    /// <summary>
    /// Values for querying toks.
    /// </summary>
    public class TokQueryValues
    {
        public string order { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string category { get; set; }
        public string tokgroup { get; set; }
        public string toktype { get; set; }
        public string userid { get; set; }
        public string itemid { get; set; }
        public string loadmore { get; set; }
        public string token { get; set; }
        public string streamtoken { get; set; }
        public string detailnumber { get; set; } = "-1";
        public string offset { get; set; }
        public string toktotal { get; set; }
        public bool? image { get; set; }
        public bool? video { get; set; }
        public string tagid { get; set; }
        public string eventstarttimedayofweek { get; set; } = "0"; //Sunday
        public string sortby { get; set; } = "standard";
        #region Search
        public bool? startswith { get; set; }
        public string text { get; set; }
        #endregion
        public string serviceid = "alphaguess";
        public string itemsbase = "toks";
        public string itemssuffix { get; set; }
    }
}
