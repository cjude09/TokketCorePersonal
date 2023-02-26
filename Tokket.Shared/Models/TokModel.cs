using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Base;
using Tokket.Core;

namespace Tokket.Shared.Models
{
    public class TokModel : Base.BaseModel
    {
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("n");

        /// <summary>Type of item.</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "tok";
        /// <summary>A unique for each Activity in getstream.io. Different from <see cref="BaseModel.Id"/>, which is stored in foreign_id.</summary>
        [JsonProperty(PropertyName = "activity_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityId { get; set; } = "";
        /// <summary>Uniquely identifies the user who posted.</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; } = "user";
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
 
        [JsonProperty(PropertyName = "user_bio", NullValueHandling = NullValueHandling.Ignore)]
        public string UserBio { get; set; } = null;
        /// <summary>User's website.</summary>
 
        [JsonProperty(PropertyName = "user_website", NullValueHandling = NullValueHandling.Ignore)]
        public string UserWebsite { get; set; } = null;
        /// <summary>User's country id (ISO code).</summary>
   
        [JsonProperty(PropertyName = "user_country", NullValueHandling = NullValueHandling.Ignore)]
        public string UserCountry { get; set; } = null;
        /// <summary>User's state abbreviation. Only required if <see cref="UserCountry"/> is United States.</summary>
 
        [JsonProperty(PropertyName = "user_state", NullValueHandling = NullValueHandling.Ignore)]
        public string UserState { get; set; } = null;

        #region Color
        /// <summary>Main color in hex format (if null, then automatically or randomly selected). For toks this is the tok tile color.</summary>
        [JsonProperty(PropertyName = "color_main_hex", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorMainHex { get; set; } = "#FFFFFF";
        #endregion

        #region Accessories
        /// <summary>Id of the user's selected avatar</summary>
        [JsonProperty(PropertyName = "selected_avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedAvatar { get; set; } = null;
        /// <summary>True if the user is using an Avatar as their profile picture, false if not</summary>
        [JsonProperty(PropertyName = "is_avatar_profile_picture", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAvatarProfilePicture { get; set; } = null;
        /// <summary>Id of the tok's tile sticker</summary>
        [JsonProperty(PropertyName = "sticker", NullValueHandling = NullValueHandling.Ignore)]
        public string Sticker { get; set; } = null;
        /// <summary>Url of the tok's tile sticker image</summary>
        [JsonProperty(PropertyName = "sticker_image", NullValueHandling = NullValueHandling.Ignore)]
        public string StickerImage { get; set; } = null;
        #endregion
        #region Categorical
        /// <summary>Tok Groups are a grouping that ensures necessary character limits, field names, and number of details are applied - it is a post format. See <see cref="TokTypeList"/> for more info.</summary>
  
        [JsonProperty(PropertyName = "tok_group")]
        public string TokGroup { get; set; }
        /// <summary>Tok types divide a tok group into more practical and specific groupings. Most of the time the tok type is still broad enough to have a wide variety of <see cref="Category"/></summary>
   
        [JsonProperty(PropertyName = "tok_type")]
        public string TokType { get; set; }
        /// <summary>Uniquely identifies tok types and must include the tok group. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.TokType"/></summary>
 
        [JsonProperty(PropertyName = "tok_type_id")]
        public string TokTypeId { get; set; }
        /// <summary>Categorizes the tok into a specific topic. It should be more specific than the <see cref="TokType"/>, and it should not contain anything in <see cref="PrimaryFieldText"/> or <see cref="Details"/>.</summary>
    
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
        /// <summary>Uniquely identifies categories. It is the <see cref="BaseModel.Id"/> for <see cref="Tokket.Core.Category"/></summary>
  
        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; }
        #endregion
        #region Standard Content
        [JsonIgnore]
        public int PrimaryFieldLimit { get; set; }
        [JsonIgnore]
        public int SecondaryFieldLimit { get; set; }
        /// <summary>Primary text field name. If possible use <see cref="Tokket.Core.Tools.TokGroupTool"/> to generate based on <see cref="TokGroup"/></summary>
        [JsonProperty(PropertyName = "primary_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryFieldName { get; set; }
        /// <summary>Primary field value. Maximum is 600 characters.</summary>

        [JsonProperty(PropertyName = "primary_text")]
        public string PrimaryFieldText { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "secondary_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryFieldName { get; set; }
        [JsonProperty(PropertyName = "secondary_text", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryFieldText { get; set; }
        [JsonProperty(PropertyName = "details", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Details { get; set; } = null;
        [JsonProperty(PropertyName = "detail_images", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DetailImages { get; set; } = null;
        #endregion
        #region English Translation fields
        /// <summary>Language of the tok. Default is "english".</summary>
   
        [JsonProperty("language")]
        public string Language { get; set; } = "english";
        /// <summary>Checks if the tok is in English.</summary>
        [JsonProperty(PropertyName = "is_english")]
        public bool IsEnglish { get; set; } = true;
        [JsonProperty(PropertyName = "has_gem_reaction")]
        public bool HasGemReaction { get; set; } = false;
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
        public bool? IsMegaTok { get; set; }
        /// <summary>Number of sections, at this moment there is no limit.</summary>
        [JsonProperty(PropertyName = "section_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? SectionCount​ { get; set; }
        /// <summary>Largest section length, max is 150,000.</summary>
        [JsonProperty(PropertyName = "section_length_total", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionLengthTotal​ { get; set; }
        /// <summary>Largest section length, max is 150,000.</summary>
        [JsonProperty(PropertyName = "section_length_largest", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionLength​Largest { get; set; }
        /// <summary>The first 5 section titles.</summary>
        [JsonProperty(PropertyName = "section_titles", NullValueHandling = NullValueHandling.Ignore)]
        public string[] SectionTitles { get; set; }
        //Will never be populated when querying for toks. Missing parts need to be lazy loaded)
        /// <summary>Store all loaded sections here. </summary>
        [JsonProperty(PropertyName = "tok_section", NullValueHandling = NullValueHandling.Ignore)]
        public TokSection[] Sections { get; set; }
        //Only increase when new partition needed (over 5,000) and never decrease​
        /// <summary>Number of partitions needed for all sections. Maximum 5,000 sections per partition (10 GB / up to 200 KB)​</summary>
        [JsonProperty(PropertyName = "section_partitions", NullValueHandling = NullValueHandling.Ignore)]
        public int? SectionPartitions { get; set; }
        #endregion
        /// <summary>Required field values for the tok. Field names should be generated from the TokGroupTool</summary>
        [JsonProperty(PropertyName = "required_field_values", NullValueHandling = NullValueHandling.Ignore)]
        public string[] RequiredFieldValues { get; set; } = null;
        [JsonIgnore]
        public string[] OptionalFields { get; set; }
        [JsonIgnore]
        public string[] RequiredFields { get; set; }
        /// <summary>Optional field values for the tok. Field names should be generated from the TokGroupTool</summary>
        [JsonProperty(PropertyName = "optional_field_values", NullValueHandling = NullValueHandling.Ignore)]
        public string[] OptionalFieldValues { get; set; } = null;
        /// <summary>Additional notes for the tok.</summary>
        [JsonProperty(PropertyName = "notes", NullValueHandling = NullValueHandling.Ignore)]
        public string Notes { get; set; } = null;
        /// <summary>Main image for the tok.</summary>
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = null;
        [JsonProperty(PropertyName = "secondary_image", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryImage { get; set; } = null;
        /// <summary>Is the tok a detailed tok</summary>
        [JsonProperty(PropertyName = "is_detail_based")]
        public bool IsDetailBased { get; set; }
        /// <summary>Is the tok a mega tok</summary>
        [JsonProperty(PropertyName = "is_mega")]
        public bool IsMega { get; set; }
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
        /// <summary>Is the tok not safe for work</summary>
        [JsonProperty(PropertyName = "nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NSFW { get; set; } = null;
        #region Answer field (Only for Test)
        [JsonProperty(PropertyName = "has_answer_field", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasAnswerField { get; set; }
        [JsonProperty(PropertyName = "answer_field_number", NullValueHandling = NullValueHandling.Ignore)]
        public int AnswerFieldNumber { get; set; }
        #endregion
        #region Statistics
        //Statistics
        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;
        [JsonProperty(PropertyName = "users_reacted", NullValueHandling = NullValueHandling.Ignore)]
        public long? UsersReacted { get; set; } = null;
        [JsonProperty(PropertyName = "coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; } = null;
        [JsonProperty(PropertyName = "likes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Likes { get; set; } = null;
        //[JsonProperty(PropertyName = "dislikes", NullValueHandling = NullValueHandling.Ignore)]
        //public long? Dislikes { get; set; } = null;
        [JsonProperty(PropertyName = "accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;
        [JsonProperty(PropertyName = "inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;
        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;
        [JsonProperty(PropertyName = "accurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] AccuratesDetails { get; set; } = null;
        [JsonProperty(PropertyName = "inaccurates_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] InaccuratesDetails { get; set; } = null;
        [JsonProperty(PropertyName = "comments_details", NullValueHandling = NullValueHandling.Ignore)]
        public long[] CommentsDetails { get; set; } = null;
        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reports { get; set; } = null;
        [JsonProperty(PropertyName = "shares", NullValueHandling = NullValueHandling.Ignore)]
        public long? Shares { get; set; } = null;
        [JsonProperty(PropertyName = "views", NullValueHandling = NullValueHandling.Ignore)]
        public long? Views { get; set; } = null;
        #endregion
        [JsonIgnore] DateTime createdTime = DateTime.Now;
        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime
        {
            get { return createdTime; }
            set
            {
                RelativeTime = DateConvert.ConvertToRelative(value);
                createdTime = value;
            }
        }
        [JsonIgnore]
        public string RelativeTime { get; set; }
        //DataTime format
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        //Unix time format
        [JsonProperty(PropertyName = "_ts")]
        public int _Timestamp { get; set; }
        [JsonIgnore]
        public Set[] Sets { get; set; }
        [JsonProperty("latest_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<TokkepediaReaction>> LatestReactions { get; set; } = null;
        [JsonProperty("own_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<TokkepediaReaction>> OwnReactions { get; set; } = null;

        [JsonProperty(PropertyName = "is_indent", NullValueHandling = NullValueHandling.Ignore)]
        public List<bool> IsIndent { get; set; } = null;
        [JsonIgnore]
        public string ColorHex { get; set; }

        public int IndexCounter { get; set; }

        #region Source/Link URL
        /// <summary>Link (must be url)</summary>
        [JsonProperty(PropertyName = "source_link", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceLink { get; set; } = null;

        /// <summary>Additional notes for the source link.</summary>
        [JsonProperty(PropertyName = "source_notes", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceNotes { get; set; } = null;
        #endregion

        /// <summary>Type of bullets to show. Options: "bullet", "number", "letter", "none". Clients should consider the default (if null) as "bullet".</summary>
        [JsonProperty(PropertyName = "bullet_kind", NullValueHandling = NullValueHandling.Ignore)]
        public string BulletKind { get; set; } = null;

        #region SubAccount and Title
        //
        // Summary:
        //     True if the currently selected subaccount is the owner.
        [JsonProperty(PropertyName = "subaccount_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SubaccountOwner { get; set; }
        //
        // Summary:
        //     Currently selected subaccount's profile picture.
        [JsonProperty(PropertyName = "subaccount_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountPhoto { get; set; }
        //
        // Summary:
        //     Currently selected subaccount's display name.
        [JsonProperty(PropertyName = "subaccount_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountName { get; set; }
        //
        // Summary:
        //     Currently selected subaccount's id.
        [JsonProperty(PropertyName = "subaccount_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountId { get; set; }

        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupId { get; set; }
        //
        // Summary:
        //     User's account type (group_account_type). Can only be "family" or "organization"
        [JsonProperty(PropertyName = "group_account_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupAccountType { get; set; }
        //
        // Summary:
        //     True if the user's currently selected title is enabled.
        [JsonProperty(PropertyName = "title_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleEnabled { get; set; }
        //
        // Summary:
        //     True if the user's currently selected title is unique.
        [JsonProperty(PropertyName = "title_unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TitleUnique { get; set; }
        //
        // Summary:
        //     Display of the user's currently selected title. Case sensitive and max of 25
        //     characters
        [JsonProperty(PropertyName = "title_display", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleDisplay { get; set; }
        //
        // Summary:
        //     Id of the user's currently selected title. Not case sensitive and max of 25 characters
        [JsonProperty(PropertyName = "title_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleId { get; set; }
        //
        // Summary:
        //     User's account type (account_type). Can only be "individual" or "group"
        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; }
        #endregion

        #region Tok Channels
        /// <summary>Highest Level (i.e. Channel). May differ for each platform.</summary>
        [JsonProperty(PropertyName = "level1", NullValueHandling = NullValueHandling.Ignore)]
        public string Level1 { get; set; } = null;
        /// <summary>Next Level (i.e. Subchannel). May differ for each platform.</summary>
        [JsonProperty(PropertyName = "level2", NullValueHandling = NullValueHandling.Ignore)]
        public string Level2 { get; set; } = null;
        /// <summary>Next Level (i.e. Subchannel). May differ for each platform.</summary>
        [JsonProperty(PropertyName = "level3", NullValueHandling = NullValueHandling.Ignore)]
        public string Level3 { get; set; } = null;
        #endregion

        [JsonIgnore]
        public string ThumbnailImage => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "md-image-") : "";

        [JsonIgnore]
        public string ThumbnailImageSmall => !string.IsNullOrEmpty(Image) ? Image.Replace("image-", "sm-image-") : "";

        [JsonProperty(PropertyName = "reference_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceId { get; set; } = null;

        [JsonProperty(PropertyName = "current_handle", NullValueHandling = NullValueHandling.Ignore)]
        public string CurrentHandle { get; set; } = null;

        [JsonProperty(PropertyName = "handle_image", NullValueHandling = NullValueHandling.Ignore)]
        public string HandleImage { get; set; } = null;

        [JsonProperty(PropertyName = "handle_color", NullValueHandling = NullValueHandling.Ignore)]
        public string HandleColor { get; set; } = null;

        [JsonProperty(PropertyName = "handle_position", NullValueHandling = NullValueHandling.Ignore)]
        public HandlePosition HandlePosition { get; set; } = HandlePosition.None;
        [JsonProperty(PropertyName = "has_comments")]
        public bool HasComments { get; set; } = true;
        [JsonProperty(PropertyName = "has_reactions")]
        public bool HasReactions { get; set; } = true;


        [JsonProperty(PropertyName = "answer_enabled")]
        public bool AnswerEnabled { get; set; } = true;
 
        [JsonProperty(PropertyName = "master_copy")]
        public bool MasterCopy { get; set; } = false;
        [JsonProperty(PropertyName = "copy_of_master")]
        public bool CopyOfMaster { get; set; } = false;
        //Only use on Web, no need to apply at the API
        [JsonProperty(PropertyName = "feed_type")]
        public string FeedType { get; set; } = "";
        [JsonProperty(PropertyName = "community_id")]
        public string CommunityId { get; set; } = null;

    }
}
