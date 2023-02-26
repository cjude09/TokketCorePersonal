
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Tokket.Core;

namespace Tokket.Shared.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public const bool TokkepediaApp = true; //If false, code is Class Toks App only

        #region Setting Constants

        private const string SettingsKey = "T0KK3P3D!@";
        private static readonly string SettingsDefault = string.Empty;

        #endregion

        #region SavedData
        public static string UserId
        {
            get => AppSettings.GetValueOrDefault(nameof(UserId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserId), value);
        }
        public static string UserAccount
        {
            get => AppSettings.GetValueOrDefault(nameof(UserAccount), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserAccount), value);
        }
        public static string TokketUser
        {
            get => AppSettings.GetValueOrDefault(nameof(TokketUser), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TokketUser), value);
        }
        public static string TokketSubAccount
        {
            get => AppSettings.GetValueOrDefault(nameof(TokketSubAccount), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TokketSubAccount), value);
        }
        public static long UserCoins
        {
            get => AppSettings.GetValueOrDefault(nameof(UserCoins), 0L);
            set => AppSettings.AddOrUpdateValue(nameof(UserCoins), value);
        }
        public static int FilterTag
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterTag), (int)FilterType.All);
            set => AppSettings.AddOrUpdateValue(nameof(FilterTag), value);
        }

        public static int ClassHomeFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(ClassHomeFilter), (int)FilterType.All);
            set => AppSettings.AddOrUpdateValue(nameof(ClassHomeFilter), value);
        }

        public static int ClassSearchFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(ClassSearchFilter), (int)FilterType.All);
            set => AppSettings.AddOrUpdateValue(nameof(ClassSearchFilter), value);
        }

        public static int FilterByTypeHome
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeHome), (int)FilterBy.None);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeHome), value);
        }
        public static string FilterByItemsHome
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByItemsHome), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByItemsHome), value);
        }
        public static string FilterByTypeSelectedHome
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeSelectedHome), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeSelectedHome), value);
        }

        public static int FilterByTypeSearch
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeSearch), (int)FilterBy.None);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeSearch), value);
        }
        public static string FilterByItemsSearch
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByItemsSearch), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByItemsSearch), value);
        }
        public static string FilterByTypeSelectedSearch
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeSelectedSearch), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeSelectedSearch), value);
        }
        public static int FilterByTypeProfile
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeProfile), (int)FilterBy.None);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeProfile), value);
        }
        public static string FilterByItemsProfile
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByItemsProfile), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByItemsProfile), value);
        }
        public static string FilterByTypeSelectedProfile
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterByTypeSelectedProfile), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FilterByTypeSelectedProfile), value);
        }
        public static int FilterFeed
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterFeed), (int)FilterType.All);
            set => AppSettings.AddOrUpdateValue(nameof(FilterFeed), value);
        }
        public static int FilterToksHome
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterToksHome), 0);
            set => AppSettings.AddOrUpdateValue(nameof(FilterToksHome), value);
        }
        public static int FilterToksSearch
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterToksSearch), 0);
            set => AppSettings.AddOrUpdateValue(nameof(FilterToksSearch), value);
        }
        public static int FilterToksProfile
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterToksProfile), 0);
            set => AppSettings.AddOrUpdateValue(nameof(FilterToksProfile), value);
        }
        public static int FilterImage
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterImage), 2); //2=Both
            set => AppSettings.AddOrUpdateValue(nameof(FilterImage), value);
        }
        public static int FilterGroup
        {
            get => AppSettings.GetValueOrDefault(nameof(FilterGroup), 3); //3= My Groups
            set => AppSettings.AddOrUpdateValue(nameof(FilterGroup), value);
        }
        public static int BrowsedImgTag
        {
            get => AppSettings.GetValueOrDefault(nameof(BrowsedImgTag), 0);
            set => AppSettings.AddOrUpdateValue(nameof(BrowsedImgTag), value);
        }
        public static string ContinuationToken
        {
            get => AppSettings.GetValueOrDefault(nameof(ContinuationToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ContinuationToken), value);
        }
        public static string HomeContinuationToken
        {
            get => AppSettings.GetValueOrDefault(nameof(HomeContinuationToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(HomeContinuationToken), value);
        }
        public static string SearchToksContinuationToken
        {
            get => AppSettings.GetValueOrDefault(nameof(SearchToksContinuationToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SearchToksContinuationToken), value);
        }
        public static string ProfileTabContinuationToken
        {
            get => AppSettings.GetValueOrDefault(nameof(ProfileTabContinuationToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ProfileTabContinuationToken), value);
        }
        public static int SortByFilterTag
        {
            get => AppSettings.GetValueOrDefault(nameof(SortByFilterTag), (int)FilterType.Standard);
            set => AppSettings.AddOrUpdateValue(nameof(SortByFilterTag), value);
        }
        public static string SortByFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(SortByFilter), "standard");
            set => AppSettings.AddOrUpdateValue(nameof(SortByFilter), value);
        }
        public static string ImageBrowseCrop
        {
            get => AppSettings.GetValueOrDefault(nameof(ImageBrowseCrop), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ImageBrowseCrop), value);
        }
        public static int ActivityInt
        {
            get => AppSettings.GetValueOrDefault(nameof(ActivityInt), 0);
            set => AppSettings.AddOrUpdateValue(nameof(ActivityInt), value);
        }
        public static string byteImageViewer
        {
            get => AppSettings.GetValueOrDefault(nameof(byteImageViewer), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(byteImageViewer), value);
        }
        public static int MaintTabInt
        {
            get => AppSettings.GetValueOrDefault(nameof(MaintTabInt), 0);
            set => AppSettings.AddOrUpdateValue(nameof(MaintTabInt), value);
        }
        public static int CurrentTheme
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentTheme), 0);
            set => AppSettings.AddOrUpdateValue(nameof(CurrentTheme), value);
        }

        public static string classFilterByClass
        {
            get => AppSettings.GetValueOrDefault(nameof(classFilterByClass), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(classFilterByClass), value);
        }
        public static string classFilterByCategory
        {
            get => AppSettings.GetValueOrDefault(nameof(classFilterByCategory), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(classFilterByCategory), value);
        }
        public static Models.AuthorizationTokenModel GetUserModel() => JsonConvert.DeserializeObject<Models.AuthorizationTokenModel>(UserAccount);
        public static TokketUser GetTokketUser() => JsonConvert.DeserializeObject<TokketUser>(TokketUser);
        public static TokketSubaccount GetTokketSubaccount() => JsonConvert.DeserializeObject<TokketSubaccount>(TokketSubAccount);
        public static List<int> GetAddTokDtlOrig() => JsonConvert.DeserializeObject<List<int>>(AddTokDtlOrigString);
        public static void SetAddTokDtlOrig(IList<int> newAddTokDtlOrigString) => AddTokDtlOrigString = JsonConvert.SerializeObject(newAddTokDtlOrigString);
        public static string AddTokDtlOrigString
        {
            get => AppSettings.GetValueOrDefault(nameof(AddTokDtlOrigString), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AddTokDtlOrigString), value);
        }

        public static List<int> GetAddTokDtlEng() => JsonConvert.DeserializeObject<List<int>>(AddTokDtlEngString);
        public static void SetAddTokDtlEng(IList<int> newAddTokDtlEngString) => AddTokDtlEngString = JsonConvert.SerializeObject(newAddTokDtlEngString);
        public static string AddTokDtlEngString
        {
            get => AppSettings.GetValueOrDefault(nameof(AddTokDtlEngString), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AddTokDtlEngString), value);
        }


        //Sort Settings

        public static string SortByReferenceAscending {
            get => AppSettings.GetValueOrDefault(nameof(SortByReferenceAscending), "");
            set => AppSettings.AddOrUpdateValue(nameof(SortByReferenceAscending), value);
        }

        public static bool SortByTitleAscending {
            get => AppSettings.GetValueOrDefault(nameof(SortByTitleAscending), true);
            set => AppSettings.AddOrUpdateValue(nameof(SortByTitleAscending), value);
        }

        //public static List<int> GetIgnoredDrivers() => JsonConvert.DeserializeObject<List<int>>(IgnoredDriverString);
        //public static void SetIgnoredDrivers(IList<int> newIgnoredDrivers) => IgnoredDriverString = JsonConvert.SerializeObject(newIgnoredDrivers);
        //public static string IgnoredDriverString
        //{
        //    get => AppSettings.GetValueOrDefault(nameof(IgnoredDriverString), string.Empty);
        //    set => AppSettings.AddOrUpdateValue(nameof(IgnoredDriverString), value);
        //}
        #endregion
    }
}
