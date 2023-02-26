using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Android.ViewModels;
using Result = Android.App.Result;

namespace Tokket.Android
{
    /*#if (_TOKKEPEDIA)
        [Activity(Label = "Filter", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif

    #if (_CLASSTOKS)
        [Activity(Label = "Filter", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif*/
    [Activity(Label = "Filter", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    public class FilterActivity : BaseActivity
    {
        string activitycaller = "", SubTitle = "";
        Color primaryColor;
        public List<TokModel> ListTokModel;
        int filterToks = (int)FilterToks.Toks;
        internal static FilterActivity Instance { get; private set; }
        private int REQUEST_CLASS_FILTER = 1001;
        List<int> originalSettingList;
        List<int> newSettingList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.filter_page);
            SetSupportActionBar(toolBar);

            Instance = this;

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            originalSettingList = new List<int>();
            newSettingList = new List<int>();

            activitycaller = Intent.GetStringExtra("activitycaller");
            if (activitycaller == null)
                activitycaller = "";

            SubTitle = Intent.GetStringExtra("SubTitle");
            setActivityTitle("Filter", SubTitle);

            if (Settings.ActivityInt == (int)ActivityType.TokSearch)
            {
                linearImage.Visibility = ViewStates.Gone;
                linearPlay.Visibility = ViewStates.Gone;
                linearSortBy.Visibility = ViewStates.Gone;

#if (_TOKKEPEDIA)
                LinearFeed.Visibility = ViewStates.Gone;
                primaryColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary_dark));
#endif

#if (_CLASSTOKS)
                LinearFeed.Visibility = ViewStates.Visible;
#endif

                linearGroup.Visibility = ViewStates.Gone;
            }
            else if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
            {
                LinearFeed.Visibility = ViewStates.Gone;
            }
            else
            {
                linearImage.Visibility = ViewStates.Visible;
                linearPlay.Visibility = ViewStates.Visible;
                linearSortBy.Visibility = ViewStates.Visible;
                LinearFeed.Visibility = ViewStates.Visible;
                linearGroup.Visibility = ViewStates.Visible;
            }


            if (activitycaller.ToUpper() == "TOKSACTIVITY")
            {
                LinearFeed.Visibility = ViewStates.Gone;
            }


            if (Settings.ActivityInt == (int)ActivityType.ClassGroupActivity || Settings.ActivityInt == (int)ActivityType.GameCategoryMain)
            {
                linearGroup.Visibility = ViewStates.Visible;
            }
            else
            {
                linearGroup.Visibility = ViewStates.Gone;
            }

#if (_CLASSTOKS)
            LinearFilterBy.Visibility = ViewStates.Visible;
            btnGlobalToks.Text = "Public";
            btnFeaturedToks.Text = "MY CLASS TOKS";
            primaryColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent));
            setColorBtn();
#endif

#if (_CLASSTOKS)
            if (activitycaller.ToUpper() == "HOME")
            {
                if (Settings.ClassHomeFilter == (int)FilterType.All)
                {
                    btnGlobalToks.Checked = true;
                    originalSettingList.Add(btnGlobalToks.Id);
                }
                else
                {
                    btnFeaturedToks.Checked = true;
                    originalSettingList.Add(btnFeaturedToks.Id);
                }
            }
            else if (activitycaller.ToUpper() == "SEARCH")
            {
                if (Settings.ClassSearchFilter == (int)FilterType.All)
                {
                    btnGlobalToks.Checked = true;
                    originalSettingList.Add(btnGlobalToks.Id);
                }
                else
                {
                    btnFeaturedToks.Checked = true;
                    originalSettingList.Add(btnFeaturedToks.Id);
                }
            }
#endif

                //View As
            if (Settings.MaintTabInt == (int)MainTab.Home)
            {
                filterToks = Settings.FilterToksHome;
            }
            else if (Settings.MaintTabInt == (int)MainTab.Search)
            {
                filterToks = Settings.FilterToksSearch;
            }
            else if (Settings.MaintTabInt == (int)MainTab.Profile)
            {
                filterToks = Settings.FilterToksProfile;
            }

            if (filterToks == (int)FilterToks.Toks)
            {
                btnFilterToks.Checked = true;
                originalSettingList.Add(btnFilterToks.Id);
            }
            else if (filterToks == (int)FilterToks.Cards)
            {
                btnFilterCards.Checked = true;
                originalSettingList.Add(btnFilterCards.Id);
            }
            
            //Sort By
            if (Settings.SortByFilter == "standard")
            {
                Settings.SortByFilterTag = (int)FilterType.Standard;
                btnStandard.Checked = true;
                originalSettingList.Add(btnStandard.Id);
            }
            else if (Settings.SortByFilter == "recent")
            {
                Settings.SortByFilterTag = (int)FilterType.Recent;
                btnRecent.Checked = true;
                originalSettingList.Add(btnRecent.Id);
            }
            else if (Settings.SortByFilter == "toptoks")
            {
                btnTopToks.Checked = true;
                originalSettingList.Add(btnTopToks.Id);
            }

            //Image
            if (Settings.FilterImage == (int)ImageType.Image)
            {
                btnFilterImage.Checked = true;
                originalSettingList.Add(btnFilterImage.Id);
            }
            else if (Settings.FilterImage == (int)ImageType.NonImage)
            {
                btnFilterNoImage.Checked = true;
                originalSettingList.Add(btnFilterNoImage.Id);
            }
            else if (Settings.FilterImage == (int)ImageType.Both)
            {
                btnFilterBoth.Checked = true;
                originalSettingList.Add(btnFilterBoth.Id);
            }

            //Group
            if (Settings.FilterGroup == (int)GroupFilter.OwnGroup)
            {
                btnFilterOwnedGroup.Checked = true;
                originalSettingList.Add(btnFilterOwnedGroup.Id);
            }
            else if (Settings.FilterGroup == (int)GroupFilter.JoinedGroup)
            {
                btnFilterJoinedGroup.Checked = true;
                originalSettingList.Add(btnFilterJoinedGroup.Id);
            }
            else if (Settings.FilterGroup == (int)GroupFilter.MyGroup)
            {
                btnFilterMyGroup.Checked = true;
                originalSettingList.Add(btnFilterMyGroup.Id);
            }

            //FilterBy
            setBtnFilterByClick(false);

            //Feed
            btnGlobalToks.Click += btnFilterClick;
            btnFeaturedToks.Click += btnFilterClick;

            //View As
            //Finish this activity once view as has been changed so that data will just changed the adapter in ClassToksFragment
            /*btnFilterToks.Click += btnFilterClick;
            btnFilterCards.Click += btnFilterClick;*/
            btnFilterToks.Click += delegate
            {
                setFilterToks((int)FilterToks.Toks);

                Intent = new Intent();
                Intent.PutExtra("viewAsChanged", true);
                SetResult(Result.Ok, Intent);
                Finish();
            };
            btnFilterCards.Click += delegate
            {
                setFilterToks((int)FilterToks.Cards);

                Intent = new Intent();
                Intent.PutExtra("viewAsChanged", true);
                SetResult(Result.Ok, Intent);
                Finish();
            };

            //Sort
            btnStandard.Click += btnFilterClick;
            btnTopToks.Click += btnFilterClick;
            btnRecent.Click += btnFilterClick;

            //Image
            btnFilterImage.Click += btnFilterClick;
            btnFilterNoImage.Click += btnFilterClick;
            btnFilterBoth.Click += btnFilterClick;

            //Group
            btnFilterMyGroup.Click += btnFilterClick;
            btnFilterJoinedGroup.Click += btnFilterClick;
            btnFilterOwnedGroup.Click += btnFilterClick;

            //Play
            btnFilterTokCards.Click -= btnFilterTokCardsClick;
            btnFilterTokCards.Click += btnFilterTokCardsClick;

            //ReferenceID
            btnRefAscending.Click += btnRefAscendingClick;
            btnRefDescending.Click += btnRefDescendingClick;

            //Title Id
            btnTitleAscending.Click += btnTitleAscendingClick;
            btnTitleDescending.Click += btnTitleDescendingClick;

            btnFilterByAll.Click += delegate
            {
                checkBtnFilterAll(btnFilterByAll);
            };

            btnClass.ContentDescription = "Class";
            btnClass.Click += btnFilterBy;

            btnCategory.ContentDescription = "Category";
            btnCategory.Click += btnFilterBy;

            btnType.ContentDescription = "Type";
            btnType.Click += btnFilterBy;

            btnFilterTokMatch.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(TokMatchActivity));
                nextActivity.PutExtra("isSet", false);
                nextActivity.PutExtra("SubTitle", SubTitle);
                this.StartActivity(nextActivity);
            };

            BtnTokChoice.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(TokChoiceActivity));
                nextActivity.PutExtra("isSet", false);
                nextActivity.PutExtra("SubTitle", SubTitle);
                nextActivity.PutExtra("TokModelList", JsonConvert.SerializeObject(ClassToksFragment.Instance.ClassTokCollection));
                this.StartActivity(nextActivity);
            };

            btnFilterSets.Click += (object sender, EventArgs e) =>
            {
                Intent nextActivity = new Intent(this, typeof(MySetsActivity));
                nextActivity.PutExtra("isAddToSet", false);
                nextActivity.PutExtra("TokTypeId", "");
                this.StartActivity(nextActivity);
            };

            linearImage.Visibility = ViewStates.Gone;
            linearPlay.Visibility = ViewStates.Gone;
            linearSortBy.Visibility = ViewStates.Gone;
        }

        private void btnTitleDescendingClick(object sender, EventArgs e)
        {
            Settings.SortByTitleAscending = false;
            setBtnFilterByClick(true);
        }

        private void btnTitleAscendingClick(object sender, EventArgs e)
        {
            Settings.SortByTitleAscending = true;
            setBtnFilterByClick(true);
        }

        private void btnRefDescendingClick(object sender, EventArgs e)
        {
            Settings.SortByReferenceAscending = "reference_id";
            setBtnFilterByClick(true);
        }

        private void btnRefAscendingClick(object sender, EventArgs e)
        {
            Settings.SortByReferenceAscending = "";
            setBtnFilterByClick(true);
        }

        private void checkBtnFilterAll(View v)
        {
            if (activitycaller.ToUpper() == "HOME")
            {
                Settings.FilterByTypeHome = (int)FilterBy.None;
                Settings.FilterByItemsHome = "";
            }
            else if (activitycaller.ToUpper() == "SEARCH")
            {
                Settings.FilterByTypeSearch = (int)FilterBy.None;
                Settings.FilterByItemsSearch = "";
            }
            else if (activitycaller.ToUpper() == "PROFILE")
            {
                Settings.FilterByTypeProfile = (int)FilterBy.None;
                Settings.FilterByItemsProfile = "";
            }

            setBtnFilterByClick(true);
        }

        private void btnFilterBy(object sender, EventArgs e)
        {
            Intent nextActivity = new Intent(this, typeof(ClassFilterbyActivity));
            nextActivity.PutExtra("filterby", (sender as Button).ContentDescription);
            nextActivity.PutExtra("caller", activitycaller.ToLower());
            this.StartActivityForResult(nextActivity, REQUEST_CLASS_FILTER);
        }
        
        private void btnFilterTokCardsClick(object sender, EventArgs e)
        {
            Intent nextActivity = new Intent(this, typeof(TokCardsMiniGameActivity));
            nextActivity.PutExtra("isSet", false);
            nextActivity.PutExtra("SubTitle", SubTitle);
            this.StartActivity(nextActivity);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //...

                //Handle sandwich menu icon click
                case HomeId:
                    if (newSettingList.Count > 0)
                    {
                        ApplyFilter();
                    }
                    
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void setBtnFilterByClick(bool isClick)
        {
            int selectedId = 0;
            if (activitycaller.ToUpper() == "HOME")
            {
                if (Settings.FilterByTypeHome == (int)FilterBy.None)
                {
                    btnFilterByAll.Checked = true;
                    selectedId = Resource.Id.btnFilterByAll;
                }
                else if (Settings.FilterByTypeHome == (int)FilterBy.Class)
                {
                    btnClass.Checked = true;
                    selectedId = Resource.Id.btnClass;
                }
                else if (Settings.FilterByTypeHome == (int)FilterBy.Category)
                {
                    btnCategory.Checked = true;
                    selectedId = Resource.Id.btnCategory;
                }
                else if (Settings.FilterByTypeHome == (int)FilterBy.Type)
                {
                    btnType.Checked = true;
                    selectedId = Resource.Id.btnType;
                }
            }
            else if (activitycaller.ToUpper() == "SEARCH")
            {
                if (Settings.FilterByTypeSearch == (int)FilterBy.None)
                {
                    btnFilterByAll.Checked = true;
                    selectedId = Resource.Id.btnFilterByAll;
                }
                else if (Settings.FilterByTypeSearch == (int)FilterBy.Class)
                {
                    btnClass.Checked = true;
                    selectedId = Resource.Id.btnClass;
                }
                else if (Settings.FilterByTypeSearch == (int)FilterBy.Category)
                {
                    btnCategory.Checked = true;
                    selectedId = Resource.Id.btnCategory;
                }
                else if (Settings.FilterByTypeSearch == (int)FilterBy.Type)
                {
                    btnType.Checked = true;
                    selectedId = Resource.Id.btnType;
                }
            }
            else if (activitycaller.ToUpper() == "PROFILE")
            {
                if (Settings.FilterByTypeProfile == (int)FilterBy.None)
                {
                    btnFilterByAll.Checked = true;
                    selectedId = Resource.Id.btnFilterByAll;
                }
                else if (Settings.FilterByTypeProfile == (int)FilterBy.Class)
                {
                    btnClass.Checked = true;
                    selectedId = Resource.Id.btnClass;
                }
                else if (Settings.FilterByTypeProfile == (int)FilterBy.Category)
                {
                    btnCategory.Checked = true;
                    selectedId = Resource.Id.btnCategory;
                }
                else if (Settings.FilterByTypeProfile == (int)FilterBy.Type)
                {
                    btnType.Checked = true;
                    selectedId = Resource.Id.btnType;
                }
            }

            if (isClick)
            {
                if (!originalSettingList.Any(n => n == selectedId))
                {
                    newSettingList.Add(selectedId);
                }
            }
        }

        private void btnFilterClick(object sender, EventArgs e)
        {
            View v = (sender as View);

            if (!originalSettingList.Any(n => n == v.Id))
            {
                newSettingList.Add(v.Id);
            }

            switch (v.Id)
            {
                case Resource.Id.btnGlobalToks:
                    Settings.FilterFeed = (int)FilterType.All;
                    Settings.FilterTag = (int)FilterType.All;

#if (_CLASSTOKS)
                    if (activitycaller.ToUpper() == "HOME")
                    {
                        MainActivity.Instance.txtFeedOptionType.Text = "Public Class Toks";
                        Settings.FilterByTypeHome = (int)FilterBy.None;
                        Settings.FilterByItemsHome = "";
                        Settings.ClassHomeFilter = (int)FilterType.All;
                    }
                    else if (activitycaller.ToUpper() == "SEARCH")
                    {
                        Settings.FilterByTypeSearch = (int)FilterBy.None;
                        Settings.FilterByItemsSearch = "";
                        Settings.ClassSearchFilter = (int)FilterType.All;
                    }
#endif
                    break;
                case Resource.Id.btnFeaturedToks:
                    Settings.FilterFeed = (int)FilterType.Featured;
                    Settings.FilterTag = (int)FilterType.Featured;

#if (_CLASSTOKS)
                    if (activitycaller.ToUpper() == "HOME")
                    {
                        MainActivity.Instance.txtFeedOptionType.Text = "MY CLASS TOKS";
                        Settings.ClassHomeFilter = (int)FilterType.Featured;
                        Settings.FilterByTypeHome = (int)FilterBy.None;
                        Settings.FilterByItemsHome = "";
                    }
                    else if (activitycaller.ToUpper() == "SEARCH")
                    {
                        Settings.ClassSearchFilter = (int)FilterType.Featured;
                        Settings.FilterByTypeSearch = (int)FilterBy.None;
                        Settings.FilterByItemsSearch = "";
                    }
#endif
                    break;
                case Resource.Id.btnFilterToks:
                    setFilterToks((int)FilterToks.Toks);
                    break;
                case Resource.Id.btnFilterCards:
                    setFilterToks((int)FilterToks.Cards);
                    break;
                case Resource.Id.btnFilterMyGroup:
                    Settings.FilterGroup = (int)GroupFilter.MyGroup;

                    break;
                case Resource.Id.btnFilterJoinedGroup:
                    Settings.FilterGroup = (int)GroupFilter.JoinedGroup;

                    break;
                case Resource.Id.btnFilterOwnedGroup:
                    Settings.FilterGroup = (int)GroupFilter.OwnGroup;

                    break;
                case Resource.Id.btnFilterStandard:
                    Settings.FilterTag = (int)FilterType.Standard;
                    Settings.SortByFilter = "standard";
                    break;
                case Resource.Id.btnRecent:
                    Settings.FilterTag = (int)FilterType.Recent;
                    Settings.SortByFilter = "recent";

                    break;
                case Resource.Id.btnTopToks:
                    break;
                case Resource.Id.btnFilterImage:
                    Settings.FilterImage = (int)ImageType.Image;
                    break;
                case Resource.Id.btnFilterNoImage:
                    Settings.FilterImage = (int)ImageType.NonImage;
                    break;
                case Resource.Id.btnFilterBoth:
                    Settings.FilterImage = (int)ImageType.Both;

                    break;
                default:
                    break;
            }
        }

        private void ApplyFilter()
        {
            var isChanged = true;
            if (newSettingList.Count == 1)
            {
                if (newSettingList[0] == Resource.Id.btnFilterToks || newSettingList[0] == Resource.Id.btnFilterCards)
                {
                    isChanged = false;
                }
            }

            Intent = new Intent();
            Intent.PutExtra("isChanged", isChanged);
            SetResult(Result.Ok, Intent);
            Finish();
        }
        private void setFilterToks(int ftoks)
        {
            if (Settings.MaintTabInt == (int)MainTab.Home)
            {
                Settings.FilterToksHome = ftoks;
            }
            else if (Settings.MaintTabInt == (int)MainTab.Search)
            {
                Settings.FilterToksSearch = ftoks;
            }
            else if (Settings.MaintTabInt == (int)MainTab.Profile)
            {
                Settings.FilterToksProfile = ftoks;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == REQUEST_CLASS_FILTER) && (resultCode == Result.Ok) && (data != null))
            {
                if (activitycaller.ToUpper() == "HOME")
                {
                    Settings.FilterByTypeHome = data.GetIntExtra("filterby", 0);
                    Settings.FilterByItemsHome = data.GetStringExtra("filterByList");

                    if (!string.IsNullOrEmpty(Settings.FilterByItemsHome))
                    {
                        var listResult = JsonConvert.DeserializeObject<List<string>>(data.GetStringExtra("filterByList"));
                        if (listResult.Count > 0)
                        {
                            if ((FilterBy)Settings.FilterByTypeHome == FilterBy.Type)
                            {
                                var items = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsHome);
                                if (items.Count == 1)
                                {
                                    Settings.FilterByTypeSelectedHome = items[0];
                                }
                                else
                                {
                                    Settings.FilterByTypeSelectedHome = "";
                                }
                            }
                        }
                        else
                        {
                            checkBtnFilterAll(btnFilterByAll);
                        }
                    }
                }
                else if (activitycaller.ToUpper() == "SEARCH")
                {
                    Settings.FilterByTypeSearch = data.GetIntExtra("filterby", 0);
                    Settings.FilterByItemsSearch = data.GetStringExtra("filterByList");

                    if (!string.IsNullOrEmpty(Settings.FilterByItemsSearch))
                    {
                        var listResult = JsonConvert.DeserializeObject<List<string>>(data.GetStringExtra("filterByList"));
                        if (listResult.Count > 0)
                        {
                            if ((FilterBy)Settings.FilterByTypeSearch == FilterBy.Type)
                            {
                                var items = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsSearch);
                                if (items.Count == 1)
                                {
                                    Settings.FilterByTypeSelectedSearch = items[0];
                                }
                                else
                                {
                                    Settings.FilterByTypeSelectedSearch = "";
                                }
                            }
                        }
                        else
                        {
                            checkBtnFilterAll(btnFilterByAll);
                        }
                    }
                }
                else if (activitycaller.ToUpper() == "PROFILE")
                {
                    Settings.FilterByTypeProfile = data.GetIntExtra("filterby", 0);
                    Settings.FilterByItemsProfile = data.GetStringExtra("filterByList");

                    if (!string.IsNullOrEmpty(Settings.FilterByItemsProfile))
                    {
                        var listResult = JsonConvert.DeserializeObject<List<string>>(data.GetStringExtra("filterByList"));
                        if (listResult.Count > 0)
                        {
                            if ((FilterBy)Settings.FilterByTypeProfile == FilterBy.Type)
                            {
                                var items = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsProfile);
                                if (items.Count == 1)
                                {
                                    Settings.FilterByTypeSelectedProfile = items[0];
                                }
                                else
                                {
                                    Settings.FilterByTypeSelectedProfile = "";
                                }
                            }
                        }
                        else
                        {
                            checkBtnFilterAll(btnFilterByAll);
                        }
                    }
                }

                setBtnFilterByClick(true);
            }
        }

        private void setColorBtn()
        {
            btnGlobalToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_orange);
            btnFeaturedToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_orange);
            btnFilterToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_green);
            btnFilterCards.SetBackgroundResource(Resource.Drawable.selector_btn_filter_green);
            btnFilterMyGroup.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterJoinedGroup.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterOwnedGroup.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnStandard.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnRecent.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnTopToks.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterImage.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterNoImage.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterBoth.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterSets.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterTokCards.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            btnFilterTokMatch.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
            BtnTokChoice.SetBackgroundResource(Resource.Drawable.selector_btn_filter_classtok);
        }

        public RadioButton btnGlobalToks => FindViewById<RadioButton>(Resource.Id.btnGlobalToks);
        public RadioButton btnFeaturedToks => FindViewById<RadioButton>(Resource.Id.btnFeaturedToks);
        public RadioButton btnTopToks => FindViewById<RadioButton>(Resource.Id.btnTopToks);
        public RadioButton btnStandard => FindViewById<RadioButton>(Resource.Id.btnFilterStandard);
        public RadioButton btnRecent => FindViewById<RadioButton>(Resource.Id.btnRecent);
        public RadioButton btnFilterToks => FindViewById<RadioButton>(Resource.Id.btnFilterToks);
        public Button btnFilterSets => FindViewById<Button>(Resource.Id.btnFilterSets);
        public RadioButton btnFilterCards => FindViewById<RadioButton>(Resource.Id.btnFilterCards);
        public Button btnFilterTokCards => FindViewById<Button>(Resource.Id.btnFilterTokCards);
        public Button btnFilterTokMatch => FindViewById<Button>(Resource.Id.btnFilterTokMatch);
        public Button BtnTokChoice => FindViewById<Button>(Resource.Id.btnFilterTokChoice);
        public RadioButton btnFilterImage => FindViewById<RadioButton>(Resource.Id.btnFilterImage);
        public RadioButton btnFilterNoImage => FindViewById<RadioButton>(Resource.Id.btnFilterNoImage);
        public RadioButton btnFilterBoth => FindViewById<RadioButton>(Resource.Id.btnFilterBoth);
        public RadioGroup LinearFeed => FindViewById<RadioGroup>(Resource.Id.LinearFeed);

        public RadioButton btnFilterMyGroup => FindViewById<RadioButton>(Resource.Id.btnFilterMyGroup);
        public RadioButton btnFilterJoinedGroup => FindViewById<RadioButton>(Resource.Id.btnFilterJoinedGroup);
        public RadioButton btnFilterOwnedGroup => FindViewById<RadioButton>(Resource.Id.btnFilterOwnedGroup);
        public RadioGroup linearGroup => FindViewById<RadioGroup>(Resource.Id.linearGroup);
        public RadioGroup radioGroupViewAs => FindViewById<RadioGroup>(Resource.Id.radioGroupViewAs);
        public RadioGroup linearSortBy => FindViewById<RadioGroup>(Resource.Id.linearSortBy);
        public RadioGroup linearImage => FindViewById<RadioGroup>(Resource.Id.linearImage);
        public LinearLayout linearPlay => FindViewById<LinearLayout>(Resource.Id.linearPlay);
        public RadioGroup LinearFilterBy => FindViewById<RadioGroup>(Resource.Id.LinearFilterBy);
        public RadioButton btnFilterByAll => FindViewById<RadioButton>(Resource.Id.btnFilterByAll);
        public RadioButton btnClass => FindViewById<RadioButton>(Resource.Id.btnClass);
        public RadioButton btnCategory => FindViewById<RadioButton>(Resource.Id.btnCategory);
        public RadioButton btnType => FindViewById<RadioButton>(Resource.Id.btnType);

        public RadioButton btnRefAscending => FindViewById<RadioButton>(Resource.Id.btnRefAscending);
        public RadioButton btnRefDescending => FindViewById<RadioButton>(Resource.Id.btnRefDescending);
        public RadioButton btnTitleAscending => FindViewById<RadioButton>(Resource.Id.btnTitleAscending);
        public RadioButton btnTitleDescending => FindViewById<RadioButton>(Resource.Id.btnTitleDescending);
    }
}