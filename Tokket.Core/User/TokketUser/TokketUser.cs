//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System;

    public class TokketUser : BaseModel
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "user";

        [JsonIgnore]
        public string IdToken { get; set; }

        [JsonIgnore]
        public string StreamToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public string Email { get; set; } = "";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PasswordHash { get; set; } = null;

        /// <summary>Email verification status. Needs to be true in order to use Tokket services.</summary>
        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified { get; set; } = false;

        /// <summary>If the user's account is disabled (content no longer accessible by the user, and they cannot login)</summary>
        [JsonProperty(PropertyName = "disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; } = null;

        /// <summary>User's display name.</summary>
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        /// <summary>User's photo.</summary>
        [JsonProperty(PropertyName = "user_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPhoto { get; set; } = null;

        /// <summary>User's header photo.</summary>
        [JsonProperty(PropertyName = "cover_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPhoto { get; set; } = null;

        /// <summary>User's biography.</summary>
        [JsonProperty(PropertyName = "bio", NullValueHandling = NullValueHandling.Ignore)]
        public string Bio { get; set; } = null;

        /// <summary>User's website.</summary>
        [JsonProperty(PropertyName = "website", NullValueHandling = NullValueHandling.Ignore)]
        public string Website { get; set; } = null;

        /// <summary>User's birthday in DateTime format.</summary>
        [JsonProperty(PropertyName = "birthday")]
        public DateTime Birthday { get; set; }

        /// <summary>User's birthday in string format.</summary>
        [JsonProperty(PropertyName = "birthdate")]
        public string BirthDate { get; set; }

        #region Birth numbers
        /// <summary>Day of birth. Example: The '2000' in 1-2-2000 (January 2nd)</summary>
        //[JsonProperty(PropertyName = "birth_year", NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public long BirthYear => Birthday.Year;

        /// <summary>Day of birth. Example: The '1' in 1-2-2000 (January 2nd)</summary>
        [JsonIgnore]
        public long BirthMonth => Birthday.Month;

        /// <summary>Day of birth. Example: The '2' in 1-2-2000 (January 2nd)</summary>
        [JsonIgnore]
        public long BirthDay => Birthday.Day;
        #endregion

        /// <summary>Date and time the user created their account.</summary>
        [JsonProperty(PropertyName = "joined")]
        public DateTime Joined { get; set; } = DateTime.Now;

        /// <summary>User's country.</summary>
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>User's state abbreviated if they live in the United States.</summary>
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; } = null;

        /// <summary>is_locked_out</summary>
        [JsonProperty(PropertyName = "is_locked_out", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsLockedOut { get; set; } = null;

        /// <summary>User's account type (account_type). Can only be "individual" or "group"</summary>
        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; } = "individual";

        /// <summary>Id of the purchase record if this is a group account</summary>
        [JsonProperty(PropertyName = "purchase_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PurchaseId { get; set; } = null;

        /// <summary>Is the item 'not safe for work'</summary>
        [JsonProperty(PropertyName = "nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NSFW { get; set; } = null;

        #region Referral Code

        //FullCode
        /// <summary>Code that was used, if any, during sign up. Example: AB12 </summary>
        [JsonProperty(PropertyName = "referralcode_value", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferralCodeValue { get; set; } = null;

        /// <summary>Source of the code that was used, if any, during sign up. Values: "sms" | "email" | "fb" , etc.</summary>
        [JsonProperty(PropertyName = "referralcode_source", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferralCodeSource { get; set; } = null;

        /// <summary>Code for admins who can send referrals. </summary>
        [JsonProperty(PropertyName = "admin_referralcode_value", NullValueHandling = NullValueHandling.Ignore)]
        public string AdminReferralCodeValue { get; set; } = null;

        ///<summary>Can only be "Q1", "Q2", "Q3", "Q4"</summary>
        //[JsonProperty(PropertyName = "referralcode_quarter", NullValueHandling = NullValueHandling.Ignore)]
        //public string ReferralCodeQuarter { get; set; } = null;

        ///<summary>Year i.e. 2022 </summary>
        //[JsonProperty(PropertyName = "referralcode_year", NullValueHandling = NullValueHandling.Ignore)]
        //public string ReferralCodeYear { get; set; } = null;

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

        /// <summary>User defined passcode that controls read/write access. In the TokketUser class this is field is filled out during Sign Up.</summary>
        [JsonProperty(PropertyName = "subaccount_key", NullValueHandling = NullValueHandling.Ignore)]
        public string SubaccountKey { get; set; } = null;

        /// <summary>Optional field that can control whether a key is required. False/null by default.</summary>
        [JsonProperty(PropertyName = "is_subaccount_key_disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSubaccountKeyDisabled { get; set; } = null;
        #endregion

        #region Membership
        /// <summary>Stores the date the royalty membership was purchased or renewed.</summary>
        [JsonProperty(PropertyName = "membership_last_updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? MembershipLastUpdated { get; set; } = null;

        /// <summary>True if the royalty membership has been purchase in the past year.</summary>
        [JsonIgnore]
        public bool MembershipEnabled
        {
            get
            {
                //Must be within the year of purchase
                if (MembershipLastUpdated == null || (DateTime)(MembershipLastUpdated).Value.AddYears(1) < DateTime.Now)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>Color for NORMAL membership badge, not royalty</summary>
        //[JsonProperty(PropertyName = "membershipbadge_color", NullValueHandling = NullValueHandling.Ignore)]
        //public string MembershipBadgeColor { get; set; } = null;
        #endregion

        #region Accessories

        #region Avatars
        /// <summary>Id of the user's selected avatar</summary>
        [JsonProperty(PropertyName = "selected_avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedAvatar { get; set; } = null;

        /// <summary>Id of the user's favorite avatar</summary>
        [JsonProperty(PropertyName = "favorite_avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteAvatar { get; set; } = null;

        /// <summary>True if the user is using an Avatar as their profile picture, false if</summary>
        [JsonProperty(PropertyName = "is_avatar_profile_picture", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAvatarProfilePicture { get; set; } = null;
        #endregion

        #region Badge
        /// <summary>Id of the user's selected badge</summary>
        [JsonProperty(PropertyName = "selected_badge", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedBadge { get; set; } = null;

        /// <summary>Id of the user's favorite badge</summary>
        [JsonProperty(PropertyName = "favorite_badge", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteBadge { get; set; } = null;

        /// <summary>True if the user is using an Badge as their profile picture, false if</summary>
        [JsonProperty(PropertyName = "is_badge_profile_picture", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsBadgeProfilePicture { get; set; } = null;
        #endregion

        #region Tokmoji
        /// <summary>Id of the user's favorite tokmoji</summary>
        [JsonProperty(PropertyName = "favorite_tokmoji", NullValueHandling = NullValueHandling.Ignore)]
        public string FavoriteTokmoji { get; set; } = null;

        /// <summary>Replaces all Tokmoji on the site with ♦ {text here} ♦</summary>
        [JsonProperty(PropertyName = "is_tokmoji_disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTokmojiDisabled { get; set; } = null;
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

        #endregion

        #region General Counts
        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; } = null;

        [JsonProperty("coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Coins { get; set; } = null;

        [JsonProperty("deleted_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedPoints { get; set; } = null;

        [JsonProperty("deleted_coins", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedCoins { get; set; } = null;
        #endregion

        #region Personal Counter: Counts only the user can change
        [JsonProperty("toks", NullValueHandling = NullValueHandling.Ignore)]
        public long? Toks { get; set; } = null;

        [JsonProperty("sets", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sets { get; set; } = null;

        [JsonProperty("deleted_toks", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedToks { get; set; } = null;

        [JsonProperty("deleted_strikes", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedStrikes { get; set; } = null;

        [JsonProperty("deleted_sets", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedSets { get; set; } = null;

        #region Reactions
        [JsonProperty(PropertyName = "reaction_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReactionScore { get; set; } = null;

        [JsonProperty(PropertyName = "reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reactions { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_reactions", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedReactions { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gema", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemA { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemb", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemB { get; set; } = null;

        /// <summary>Number of gems.</summary>
        [JsonProperty(PropertyName = "gemc", NullValueHandling = NullValueHandling.Ignore)]
        public long? GemC { get; set; } = null;

        [JsonProperty(PropertyName = "accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Accurates { get; set; } = null;

        [JsonProperty(PropertyName = "inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? Inaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? Comments { get; set; } = null;

        /// <summary>Number of treasures.</summary>
        [JsonProperty(PropertyName = "treasure", NullValueHandling = NullValueHandling.Ignore)]
        public long? Treasures { get; set; } = null;

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public long? Reports { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_accurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedAccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_inaccurates", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedInaccurates { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedComments { get; set; } = null;

        [JsonProperty(PropertyName = "following", NullValueHandling = NullValueHandling.Ignore)]
        public long? Following { get; set; } = null;

        [JsonProperty(PropertyName = "deleted_following", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedFollowing { get; set; } = null;
        #endregion


        #endregion

        #region Public Counts
        /// <summary>Number of followers.</summary>
        [JsonProperty(PropertyName = "followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? Followers { get; set; } = null;

        /// <summary>Number of followers.</summary>
        [JsonProperty(PropertyName = "deleted_followers", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeletedFollowers { get; set; } = null;
        #endregion


        #region Games

        #region Tok Blitz

        //---TOK BLITZ---
        #region Tok Blitz Analytics
        #region General counts
        /// <summary>Total number of games played.</summary>
        [JsonProperty("games_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesTokBlitz { get; set; } = null;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsTokBlitz { get; set; } = null;
        #endregion

        #region Difficulty Statistics

        #region Number of games played
        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL1TokBlitz { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL2TokBlitz { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL3TokBlitz { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL4TokBlitz { get; set; } = null;
        #endregion

        #region Total Points earned
        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL1TokBlitz { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL2TokBlitz { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL3TokBlitz { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL4TokBlitz { get; set; } = null;
        #endregion

        #region Total Eliminators used
        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL1TokBlitz { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL2TokBlitz { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL3TokBlitz { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL4TokBlitz { get; set; } = null;
        #endregion

        #region Total seconds elasped across all games
        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l1_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL1TokBlitz { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l2_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL2TokBlitz { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l3_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL3TokBlitz { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l4_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL4TokBlitz { get; set; } = null;
        #endregion

        #endregion
        #endregion

        #region Tok Blitz Counter
        /// <summary>Total number of eliminators owned.</summary>
        [JsonProperty("eliminators_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitz { get; set; } = null;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_tokblitz_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlitzDeleted { get; set; } = null;
        #endregion

        #region Tok Blitz Settings
        /// <summary>True if user has purchased the room feature for the service.</summary>
        [JsonProperty("room_purchased_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRoomPurchasedTokBlitz { get; set; } = null;

        /// <summary>True if user has purchased the no ads feature for the service.</summary>
        [JsonProperty("no_ads_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoAdsTokBlitz { get; set; } = null;

        /// <summary>Total number of saved game slots owned for the service. Max 50.</summary>
        [JsonProperty("saved_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? SavedTokBlitz { get; set; } = null;

        /// <summary>Total number of teams owned for the service. Max 15.</summary>
        [JsonProperty("teams_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamsTokBlitz { get; set; } = null;
        #endregion

        #region Daily Bonus
        /// <summary>Record id: id of the DailyBonusRecord</summary>
        [JsonProperty(PropertyName = "record_id_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public string RecordIdTokBlitz { get; set; } = null;

        /// <summary>The first day and time the user used the service. In other words, the date the user checks the service for the first time. Same as CreatedTime</summary>
        [JsonProperty(PropertyName = "first_date_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? FirstDateTokBlitz { get; set; } = null;

        /// <summary>The most recent day and time the user used the service</summary>
        [JsonProperty(PropertyName = "recent_date_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RecentDateTokBlitz { get; set; } = null;

        /// <summary>Days in a row</summary>
        [JsonProperty(PropertyName = "days_consecutive_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveDaysTokBlitz { get; set; } = null;

        /// <summary>Days total</summary>
        [JsonProperty(PropertyName = "days_cumulative_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? CumulativeDaysTokBlitz { get; set; } = null;

        /// <summary>Longest daily bonus streak</summary>
        [JsonProperty(PropertyName = "longest_consecutive_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveLongestTokBlitz { get; set; } = null;

        /// <summary>True if more than 24 hours has passed</summary>
        [JsonProperty(PropertyName = "is_rewarded_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRewardedTokBlitz { get; set; } = null;

        /// <summary>True if the check was successful (no code errors)</summary>
        [JsonProperty(PropertyName = "is_checked_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCheckedTokBlitz { get; set; } = null;

        /// <summary>Message to display</summary>
        [JsonProperty(PropertyName = "message_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageTokBlitz { get; set; } = null;

        /// <summary>DateTime where daily bonus is applied</summary>
        [JsonProperty(PropertyName = "daily_reset_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DailyResetTokBlitz { get; set; } = null;

        /// <summary>Hours left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "hours_left_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public int? HoursLeftTokBlitz { get; set; } = null;

        /// <summary>Minutes left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "minutes_left_tokblitz", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinutesLeftTokBlitz { get; set; } = null;
        #endregion

        //---TOK BLITZ---
        #endregion

        #region Tok Blast
        //---TOK BLAST---
        #region Tok Blast Analytics
        #region General counts
        /// <summary>Total number of games played.</summary>
        [JsonProperty("games_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesTokBlast { get; set; } = null;

        /// <summary>Total number of points earned.</summary>
        [JsonProperty("points_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsTokBlast { get; set; } = null;
        #endregion

        #region Difficulty Statistics

        #region Number of games played
        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL1TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL2TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL3TokBlast { get; set; } = null;

        /// <summary>Total number of games played in the level.</summary>
        [JsonProperty("games_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesL4TokBlast { get; set; } = null;
        #endregion

        #region Total Points earned
        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL1TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL2TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL3TokBlast { get; set; } = null;

        /// <summary>Total number of points earned in the level.</summary>
        [JsonProperty("points_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsL4TokBlast { get; set; } = null;
        #endregion

        #region Total Eliminators used
        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL1TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL2TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL3TokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used in the level.</summary>
        [JsonProperty("eliminators_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsL4TokBlast { get; set; } = null;
        #endregion

        #region Total seconds elasped across all games
        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l1_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL1TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l2_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL2TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l3_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL3TokBlast { get; set; } = null;

        /// <summary>Total number of seconds elasped when playing in the level.</summary>
        [JsonProperty("seconds_l4_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SecondsL4TokBlast { get; set; } = null;
        #endregion

        #endregion
        #endregion

        #region Tok Blast Counter
        /// <summary>Total number of eliminators owned.</summary>
        [JsonProperty("eliminators_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlast { get; set; } = null;

        /// <summary>Total number of eliminators used.</summary>
        [JsonProperty("eliminators_tokblast_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? EliminatorsTokBlastDeleted { get; set; } = null;

        /// <summary>Total number of revealers owned.</summary>
        [JsonProperty("revealers_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? RevealersTokBlast { get; set; } = null;

        /// <summary>Total number of revealers used.</summary>
        [JsonProperty("revealers_tokblast_deleted", NullValueHandling = NullValueHandling.Ignore)]
        public long? RevealersTokBlastDeleted { get; set; } = null;
        #endregion

        #region Tok Blast Settings
        /// <summary>True if user has purchased the room feature for the service.</summary>
        [JsonProperty("room_purchased_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRoomPurchasedTokBlast { get; set; } = null;

        /// <summary>True if user has purchased the no ads feature for the service.</summary>
        [JsonProperty("no_ads_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoAdsTokBlast { get; set; } = null;

        /// <summary>Total number of saved game slots owned for the service.</summary>
        [JsonProperty("saved_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? SavedTokBlast { get; set; } = null;
        #endregion

        #region Tok Blast Daily Bonus
        /// <summary>Record id: id of the DailyBonusRecord</summary>
        [JsonProperty(PropertyName = "record_id_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public string RecordIdTokBlast { get; set; } = null;

        /// <summary>The first day and time the user used the service. In other words, the date the user checks the service for the first time. Same as CreatedTime</summary>
        [JsonProperty(PropertyName = "first_date_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? FirstDateTokBlast { get; set; } = null;

        /// <summary>The most recent day and time the user used the service</summary>
        [JsonProperty(PropertyName = "recent_date_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RecentDateTokBlast { get; set; } = null;

        /// <summary>Days in a row</summary>
        [JsonProperty(PropertyName = "days_consecutive_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveDaysTokBlast { get; set; } = null;

        /// <summary>Days total</summary>
        [JsonProperty(PropertyName = "days_cumulative_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? CumulativeDaysTokBlast { get; set; } = null;

        /// <summary>Longest daily bonus streak</summary>
        [JsonProperty(PropertyName = "longest_consecutive_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveLongestTokBlast { get; set; } = null;

        /// <summary>True if more than 24 hours has passed</summary>
        [JsonProperty(PropertyName = "is_rewarded_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRewardedTokBlast { get; set; } = null;

        /// <summary>True if the check was successful (no code errors)</summary>
        [JsonProperty(PropertyName = "is_checked_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCheckedTokBlast { get; set; } = null;

        /// <summary>Message to display</summary>
        [JsonProperty(PropertyName = "message_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageTokBlast { get; set; } = null;

        /// <summary>DateTime where daily bonus is applied</summary>
        [JsonProperty(PropertyName = "daily_reset_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DailyResetTokBlast { get; set; } = null;

        /// <summary>Hours left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "hours_left_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public int? HoursLeftTokBlast { get; set; } = null;

        /// <summary>Minutes left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "minutes_left_tokblast", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinutesLeftTokBlast { get; set; } = null;
        #endregion

        //---TOK BLAST---
        #endregion

        #region Alpha Guess
        //---ALPHA GUESS---

        #region Alpha Guess Daily Bonus
        /// <summary>Record id: id of the DailyBonusRecord</summary>
        [JsonProperty(PropertyName = "record_id_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public string RecordIdAlphaGuess { get; set; } = null;

        /// <summary>The first day and time the user used the service. In other words, the date the user checks the service for the first time. Same as CreatedTime</summary>
        [JsonProperty(PropertyName = "first_date_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? FirstDateAlphaGuess { get; set; } = null;

        /// <summary>The most recent day and time the user used the service</summary>
        [JsonProperty(PropertyName = "recent_date_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RecentDateAlphaGuess { get; set; } = null;

        /// <summary>Days in a row</summary>
        [JsonProperty(PropertyName = "days_consecutive_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveDaysAlphaGuess { get; set; } = null;

        /// <summary>Days total</summary>
        [JsonProperty(PropertyName = "days_cumulative_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? CumulativeDaysAlphaGuess { get; set; } = null;

        /// <summary>Longest daily bonus streak</summary>
        [JsonProperty(PropertyName = "longest_consecutive_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public long? ConsecutiveLongestAlphaGuess { get; set; } = null;

        /// <summary>True if more than 24 hours has passed</summary>
        [JsonProperty(PropertyName = "is_rewarded_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRewardedAlphaGuess { get; set; } = null;

        /// <summary>True if the check was successful (no code errors)</summary>
        [JsonProperty(PropertyName = "is_checked_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCheckedAlphaGuess { get; set; } = null;

        /// <summary>Message to display</summary>
        [JsonProperty(PropertyName = "message_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageAlphaGuess { get; set; } = null;

        /// <summary>DateTime where daily bonus is applied</summary>
        [JsonProperty(PropertyName = "daily_reset_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DailyResetAlphaGuess { get; set; } = null;

        /// <summary>Hours left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "hours_left_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public int? HoursLeftAlphaGuess { get; set; } = null;

        /// <summary>Minutes left before daily bonus. Set to null if IsRewarded is true</summary>
        [JsonProperty(PropertyName = "minutes_left_alphaguess", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinutesLeftAlphaGuess { get; set; } = null;
        #endregion

        //---ALPHA GUESS---
        #endregion

        #endregion

        #region Categorical (internal or potentially used in the future)
        /// <summary>Id of the collection this item is stored in. Collections are currently for internal/corporate/curated items such as quote of the hour.</summary>
        [JsonProperty(PropertyName = "collection_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionId { get; set; } = null;

        /// <summary>Tags are currently for internal/corporate/curated items.</summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Tags { get; set; } = null;
        #endregion

        #region Tok Handle
        [JsonProperty(PropertyName = "current_handle")]
        public string CurrentHandle { get; set; } = null;
        [JsonProperty(PropertyName = "handle_image")]
        public string HandleImage { get; set; } = null;
        [JsonProperty(PropertyName = "handle_color")]
        public string HandleColor { get; set; } = null;
        [JsonProperty(PropertyName = "handle_position")]
        public HandlePosition HandlePosition { get; set; } = HandlePosition.None;
        [JsonProperty(PropertyName = "is_user_display_handle_enabled")]
        public bool IsUserDisplayHandleEnabled
        {
            get
            {
                if (String.IsNullOrEmpty(HandleColor) && String.IsNullOrEmpty(HandleImage))
                    return false;
                else
                    return true;
            }
        }
        #endregion
    }
}
