using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Helpers
{
    public enum GameScheme { TokBlitz, TokBlast, TokBoom, AlphaGuess }
    public enum ClassGroupTab { ClassTok, ExistingClassTok, ClassSet, Chat, TokPic, Members, TokPak, TokDoc, ExistingClassSet }
    public enum ClassGroupOption { All, Church, Class, Club, Team , Study}
    public enum ProfileOptions { Website, Bio, DisplayName }
    public enum ReactionType { Like, Dislike, GemA, GemB, GemC, Accurate, Inaccurate }

    public enum NavigationStack
    {
        Login = 0,
        Signup,
        Dashboard,
        Tutorial,
        Main,
        FirstScreen,
        ForgotPassword,
        ResetPassword,
        EmailSignUp,
    }

    public enum TokMatchMode
    {
        Normal = 0,
        Education
    }

    public enum EditMode
    {
        Save = 1,
        Update
    }
    public enum ImageType
    {
        Image = 0,
        NonImage,
        Both
    }
    public enum Result
    {
        None = 0,
        Success,
        Failed,
        Error,
        Forbidden,
        NoInternet
    }

    public enum GroupFilter { AllGroup, OwnGroup, JoinedGroup, MyGroup }


    public enum FilterType
    {
        None = 0,
        TokType,
        Text,
        Category,
        Country,
        User,
        Group,
        Featured,
        Set,
        All,
        ClassToks,
        Standard,
        Recent,
        Tag
    }
    public enum FilterToks
    {
        Toks,
        Cards
    }
    public enum Toks
    {
        Category,
        TokGroup,
        TokType
    }
    public enum  NumDisplay
    {
        mobile,
        tablet
    }
    public enum ActivityType
    {
        HomePage,
        AddTokActivityType,
        AddSetActivityType,
        LeftMenuSets,
        MySetsView,
        TokInfo,
        TokSearch,
        AddSectionPage,
        ToksFragment,
        AddStickerDialogActivity,
        ReactionValuesActivity,
        ProfileActivity,
        ProfileTabActivity,
        AddClassTokActivity,
        AddClassGroupActivity,
        AddClassSetActivity,
        SignUpActivity,
        ClassGroupActivity,
        GameCategoryMain,
        AddGameSetActivity,
        AvatarsActivity,
        AddPicClassGroup,
        TokquestGamesetsMain,
        TeacherAttendanceMain,
        StudentAttendanceMain,
        CreateGameSetClassGroup,
        CreateGameSetToks,
        AddYearbookActivity,
        YearbookActivity,
        TokQuestCreateGame,
        AddOpportunityActivity,
        OpportunityActivity,
        ProfileSetsView,
        SharedUrisActivity,
        NameplateSettingsUIActivity
    }
    public enum ThemeStyle
    {
        Light = 0,
        Dark = 1
    }

    public enum MainTab
    {
        Home,
        Search,
        Notification,
        Profile
    }
    public enum PatchesTab
    {
        MyPatches,
        LevelTable,
        PatchColor
    }

    /// <summary>
    ///     Summary:
    ///     Pending - The status is Pending if the user is requesting to join the group.
    ///     PendingInvite - The status is PendingInvite if the user is invited by the owner. This will show up in users invites/invitation list.
    ///     Accepted - The status is Accepted if the owner of the group or the invitee accepted the request.
    ///     Declined - The status is Declined if the owner of the group or the invitee declined the request.
    ///     All - This status is for filtering data. Not available for data.
    /// </summary>
    public enum RequestStatus { Pending, PendingInvite, Accepted, Declined, All }
    public enum Actions { None, OpenLink, OpenMedia, OpenHashTag, OpenUser }
    public enum FilterBy { None, Class, Category, Type }
    public enum TokPakType { Paper, Presentation, PracticeTest }
    public enum BulletKind { Bullet, Number }

    /// <summary>
	/// Current state of the item in the cache.
	/// </summary>
	[Flags]
    public enum CacheState
    {
        /// <summary>
        /// An unknown state for the cache item
        /// </summary>
        None = 0,
        /// <summary>
        /// Expired cache item
        /// </summary>
        Expired = 1,
        /// <summary>
        /// Active non-expired cache item
        /// </summary>
        Active = 2
    }
}
