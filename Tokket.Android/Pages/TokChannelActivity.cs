using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Newtonsoft.Json;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.ViewModels;
using Tokket.Shared.Extensions;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Result = Android.App.Result;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using AndroidX.ViewPager2.Widget;
using Tokket.Android.Views.Fragments;
using XFragment = AndroidX.Fragment.App.Fragment;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.BottomNavigation;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Navigation;
using Tokket.Android.Views;
using AndroidX.Core.View;
using Bumptech.Glide;
using AndroidX.Core.Content;
using static Android.Icu.Text.ListFormatter;
using static Android.App.ActionBar;
using Tokket.Shared.Models.Tokquest;

namespace Tokket.Android
{
    [Activity(Label = "TokChannel", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokChannelActivity : BaseActivity
    {
        private ActionBarDrawerToggle mDrawerToggle;
        private AdapterFragmentX adapterFragment;
        Helpers.FeatureFlags FeatureTabFlags = new Helpers.FeatureFlags();
        private int REQUEST_CODE_ADD_TOK = 1001, REQUEST_CODE_ADD_CLASS_GROUP = 1002;
        internal static TokChannelActivity Instance { get; private set; }
        ClassTokModel classtokModel;
        ClassTokDataAdapter ClassTokDataAdapter; ClassTokCardDataAdapter classtokcardDataAdapter;
        public List<ClassTokModel> ClassTokCollection; List<Tokmoji> ListTokmojiModel;
        List<LevelViewModel> level2List, level3List;
        TokChannelsLevelAdapter level2Adapter, level3Adapter;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        string level2Selected, level3Selected;
        List<string> randomcolors = new List<string>();
        List<string> mathHighSchoolList, scienceHighSchoolList, electivesHighSchoolList, englishHighSchoolList, socialStudiesHighSchoolList,
                    businessCollegeList, scienceCollegeList, humanitiesCollegeList, engineeringCollegeList, characterList, bibleVerseList, holidaysList, specialDaysList, 
                    successList, spiritualList;

        List<XFragment> fragments = new List<XFragment>();
        List<string> fragmentTitles = new List<string>();
        public DrawerLayout drawerLayout;
        TokketUser tokketUser;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_tok_channel);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            if (mDrawerToggle == null)
            {
                setupDrawerLayout();
            }
            setupAppBar();
            drawerLayout.DrawerClosed -= DrawerLayout_DrawerClosed;
            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;

            tokketUser = Settings.GetTokketUser();

            Instance = this;
            ClassTokCollection = new List<ClassTokModel>();
            mathHighSchoolList = new List<string>(new string[] { "Algebra", "Geometry", "Trigonometry", "Calculus", "Statistics", "Financial Literacy" });
            scienceHighSchoolList = new List<string>(new string[] { "Biology", "Physics", "Chemistry", "Physical Science", "Astronomy", "Anatomy" });
            electivesHighSchoolList = new List<string>(new string[] { "Jrotc", "Pschology", "Culinary", "Automotive", "Career Prep", "Sociology" });
            englishHighSchoolList = new List<string>(new string[] { "Literature", "Language" });
            //List<string> foreignLanguageHighSchoolList = new List<string>(new string[] { "spanish", "german", "french", "mandarin" });
            socialStudiesHighSchoolList = new List<string>(new string[] {"World History", "Us History", "Government", "Economics", "Geography" });

            businessCollegeList = new List<string>(new string[] { "Accounting", "Finance", "Economics" });
            scienceCollegeList = new List<string>(new string[] { "Chemistry", "Biology", "Earth Sciences", "Physics", "Astronomy", "Agriculture", "Sport Science", "Computer Science" });
            humanitiesCollegeList = new List<string>(new string[] {"History", "Philosopy" });
            engineeringCollegeList = new List<string>(new string[] { "Civil Engineering", "Mechanical Engineering", "Electrical Engineering", "General Engineering", "Mechanical Engineering", "Aerospace Engineering" });
            //List<string> artsCollegeList = new List<string>(new string[] { "arts", "performing arts", "design", "architecture", "archaeology" });

            characterList = new List<string>(new string[] { "Attitude", "Character", "Courage", "Commitment", "Faith", "God", "Hope", "Humor", "Kindness", "Love", "Patience", "Perseverance", "Peace", "Service" });
            bibleVerseList = new List<string>(new string[] { "Peace", "Love", "Hope", "Money", "Faith", "Salvation", "Strength", "Perseverance" });
            holidaysList = new List<string>(new string[] { "Christmas", "Easter", "Fourth of July", "Labor Day", "Memorial Day", "MLK Jr. Day", "New Year", "Thanksgiving", "Veteran’s Day" });
            specialDaysList = new List<string>(new string[] { "Anniversary", "Baby Birth", "Birthday", "Graduation", "Father’s Day", "Mother’s Day", "Retirement" });
            successList = new List<string>(new string[] { "Ability", "Adversity", "Creativity", "Education", "Leadership", "Money", "Opportunity", "Quality", "Success", "Time", "Wisdom", "Work" });

            spiritualList = new List<string>(new string[] { "Buddhism", "Christianity", "Hinduism", "Islam", "Judaism", "New Age" });

            fragments = fragments = new List<XFragment>();

            var layoutManager2 = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            RecyclerLevel2.SetLayoutManager(layoutManager2);

            var layoutManager3 = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            RecyclerLevel3.SetLayoutManager(layoutManager3);

            this.Title = Intent.GetStringExtra("level1");
            LoadLevel2List();

            fab_AddTok.Click += (object sender, EventArgs e) =>
            {
                Bundle bundle = new Bundle();
                bundle.PutString("level1", this.Title);
                bundle.PutString("level2", level2Selected == "All" ? "" : level2Selected);
                bundle.PutString("level3", level3Selected == "All" ? "" : level3Selected);
                Intent nextActivity;
#if (_TOKKEPEDIA)
                nextActivity = new Intent(this, typeof(AddTokActivity));
                this.StartActivityForResult(nextActivity, REQUEST_CODE_ADD_TOK);
#endif
#if (_CLASSTOKS)
                if (FeatureTabFlags.TokChannelsIsCommunitiesTabEnabled && ViewpagerChannel.CurrentItem != 0)
                {
                    bundle.PutBoolean("isCommunity", FeatureTabFlags.TokChannelsIsCommunitiesTabEnabled);
                    nextActivity = new Intent(this, typeof(AddClassGroupActivity));
                    nextActivity.PutExtras(bundle);
                    this.StartActivityForResult(nextActivity, REQUEST_CODE_ADD_CLASS_GROUP);
                }
                else
                {
                    nextActivity = new Intent(this, typeof(AddClassTokActivity));
                    nextActivity.PutExtras(bundle);
                    this.StartActivityForResult(nextActivity, REQUEST_CODE_ADD_TOK);
                }
#endif
            };
            
            btnTab1.Click += delegate
            {
                TabChannel.GetTabAt(0).Select();
            };

            btnTab2.Click += delegate
            {
                TabChannel.GetTabAt(1).Select();
            };

            BottomNav.ItemSelected += BottomNav_ItemSelected;

            LoadLeftListNavigation();
            SetLeftNavHeader();
        }
        private void SetLeftNavHeader()
        {
            //setting the header layout values
            var headerView = LeftNavigation.GetHeaderView(0);
            var userphoto = headerView.FindViewById<ImageView>(Resource.Id.img_profileUserPhoto);
            var user = headerView.FindViewById<TextView>(Resource.Id.lblUser);
            var groupSelection = headerView.FindViewById<ImageView>(Resource.Id.lblGroupSelect);
            user.Text = tokketUser.Email;
            Glide.With(this).Load(tokketUser.UserPhoto).Into(userphoto);

            LinearCoinsToast.Visibility = ViewStates.Gone;
            TextCoinsToast.Visibility = ViewStates.Gone;
            txtGroup.Visibility = ViewStates.Gone;
            txtFeedOptionType.Visibility = ViewStates.Gone;
            LeftNavArrowDown.Visibility = ViewStates.Gone;

            var defaultPrimaryColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent));
            var defaultPrimaryDarkerColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent_darker));

            headerView.FindViewById(Resource.Id.relativeLayoutHeader).SetBackgroundColor(Settings.CurrentTheme == (int)ThemeStyle.Light ?
                defaultPrimaryColor :
                defaultPrimaryDarkerColor);
        }
        private void BottomNav_ItemSelected(object sender, BottomNavigationView.ItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.toks)
            {
                
            }
            else if (e.Item.ItemId == Resource.Id.search)
            {
                ShowSearchView();
            }
            else if (e.Item.ItemId == Resource.Id.tokhandles)
            {
                Toast.MakeText(this, "Coming Soon!", ToastLength.Long).Show();
            }
        }
        private void LoadLevel2List()
        {
            level2List = new List<LevelViewModel>();
            switch (this.Title.ToLower())
            {
                case "career":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#E64A19")));
                   // RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#E64A19"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#E64A19"));
                    careerItems();
                    break;
                case "college":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#512DA8")));
                  //  RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#512DA8"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#512DA8"));
                    collegeItems();
                    break;
                case "junior school":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#d32f2f")));
                  //  RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#d32f2f"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#d32f2f"));
                    break;
                case "health":
                    FeatureTabFlags.TokChannelsIsCommunitiesTabEnabled = true;
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#ffacc4")));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#ffacc4"));
                    healthItems();
                    break;
                case "high school":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#1976D2")));
                  //  RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#1976D2"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#1976D2"));
                    highSchoolItems();
                    break;
                case "recreation":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#689F38")));
                //    RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#689F38"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#689F38"));
                    break;
                case "religion":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#E64A19")));
                //    RecyclerLevel2.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor("#E64A19"));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#E64A19"));
                    religionItems();
                    break;
                case "wisdom":
                    this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#ffb6c1")));
                    fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#ffb6c1"));
                    wisdomItems();
                    break;
                default:
                    break;
            }

            if (!FeatureTabFlags.TokChannelsIsCommunitiesTabEnabled)
            {
                MidTabSpace.Visibility = ViewStates.Gone;
                btnTab2.Visibility = ViewStates.Gone;
            }

            if (level2List.Count > 0)
            {
                var itemLevel = new LevelViewModel();
                itemLevel.levelName = "All";
                level2List.Insert(0, itemLevel);
            }

            for (int i = 0; i < level2List.Count; i++)
            {
                int ndx = i % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                level2List[i].colorBackground = randomcolors[ndx];
            }
            level2Adapter = new TokChannelsLevelAdapter(this, level2List);
            RecyclerLevel2.SetAdapter(level2Adapter);
        }

        void SetupViewPager(ViewPager viewPager)
        {
            if (fragments.Count() == 0)
            {
                fragments = new List<XFragment>();
                fragmentTitles = new List<string>();

                fragments.Add(new TokChannelToksFragment(this.Title, level2Selected, level3Selected));

                fragmentTitles.Add("Toks");

                if (FeatureTabFlags.TokChannelsIsCommunitiesTabEnabled)
                {
                    fragments.Add(new TokChannelCommunitiesFragment());
                    fragmentTitles.Add("Communities");
                }

                SetupViewPagerFragment(viewPager);
            }
            else
            {
                var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                if (CurrentPage != null)
                {
                    if (ViewpagerChannel.CurrentItem == 0)
                    {
                        RunOnUiThread(async () => await ((TokChannelToksFragment)CurrentPage).LoadToks(this.Title, level2Selected, level3Selected));
                    }
                }
            }
        }

        private void SetupViewPagerFragment(ViewPager viewPager)
        {
            adapterFragment = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapterFragment;
            ViewpagerChannel.Adapter.NotifyDataSetChanged();

            ViewpagerChannel.PageSelected += (obj, e) => {
                var position = e.Position;
                /*if (SearchV != null)
                {
                    if (position == 0)
                    {
                        SearchV.QueryHint = "Search for Toks...";
                    }
                    else
                    {
                        SearchV.QueryHint = "Search for Communities...";
                    }
                }*/
            };
        }

        private void careerItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Agricultural";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Business";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Computing";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Construction";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Creative";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Culinary";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Healthcare";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Human Services";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Mechanical";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Project Management";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Technical";
            level2List.Add(level2);
        }

        private void collegeItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Business";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Science";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Humanities";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Engineering";
            level2List.Add(level2);
        }

        private void healthItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Medical";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Mental";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Nutritional";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Physical";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Spiritual";
            level2List.Add(level2);
        }

        private void highSchoolItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Math";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "English";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Science";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Social Studies";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Elective";
            level2List.Add(level2);
        }

        private void religionItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Christianity";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Islam";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Buddhism";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Hinduism";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Other";
            level2List.Add(level2);
        }

        private void wisdomItems()
        {
            var level2 = new LevelViewModel();
            level2.levelName = "Character";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Special Days";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Holidays";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Success";
            level2List.Add(level2);

            level2 = new LevelViewModel();
            level2.levelName = "Bible Verse";
            level2List.Add(level2);
        }

        public void LoadLevel3List(string parentName, string color, int level)
        {
            ClassTokCollection.Clear();
            if (level == 2)
            {
                level2Selected = parentName;
                level3Selected = "All";
              //  RecyclerLevel3.HorizontalScrollbarThumbDrawable = new ColorDrawable(Color.ParseColor(color));
            }
            else if (level == 3)
            {
                level3Selected = parentName;
            }

            if (parentName.ToLower() == "all" && level == 3)
            {
                btnTab1.Text = level2Selected;
            }
            else if (parentName.ToLower() == "all" && level == 2)
            {
                btnTab1.Text = "Toks";
            }
            else
            {
                btnTab1.Text = parentName;
            }

            level3List = new List<LevelViewModel>();
            switch (parentName.ToLower())
            {
                case "math":
                    for (int i = 0; i < mathHighSchoolList.Count; i++)
                    {
                        var item = new LevelViewModel();
                        item.levelName = mathHighSchoolList[i];
                        level3List.Add(item);
                    }
                    break;
                case "science":
                    if (this.Title == "High School")
                    {
                        for (int i = 0; i < scienceHighSchoolList.Count; i++)
                        {
                            var item = new LevelViewModel();
                            item.levelName = scienceCollegeList[i];
                            level3List.Add(item);
                        }
                    }
                    else if (this.Title == "College")
                    {
                        for (int i = 0; i < scienceCollegeList.Count; i++)
                        {
                            var item = new LevelViewModel();
                            item.levelName = scienceCollegeList[i];
                            level3List.Add(item);
                        }
                    }
                    break;
                case "elective":
                    for (int i = 0; i < electivesHighSchoolList.Count; i++)
                    {
                        var item = new LevelViewModel();
                        item.levelName = electivesHighSchoolList[i];
                        level3List.Add(item);
                    }
                    break;
                case "english":
                    foreach (var item in englishHighSchoolList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "social studies":
                    foreach (var item in socialStudiesHighSchoolList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "business":
                    foreach (var item in businessCollegeList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "humanities":
                    foreach (var item in humanitiesCollegeList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "engineering":
                    foreach (var item in engineeringCollegeList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "character":
                    foreach (var item in characterList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "bible verse":
                    foreach (var item in bibleVerseList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "holidays":
                    foreach (var item in holidaysList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "special days":
                    foreach (var item in specialDaysList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "success":
                    foreach (var item in successList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                case "spiritual":
                    foreach (var item in spiritualList)
                    {
                        var itemLevel = new LevelViewModel();
                        itemLevel.levelName = item;
                        level3List.Add(itemLevel);
                    }
                    break;
                default:
                    break;
            }

            if (level3List.Count > 0)
            {
                RecyclerLevel3.Visibility = ViewStates.Visible;
                var itemLevel = new LevelViewModel();
                itemLevel.levelName = "All";
                level3List.Insert(0, itemLevel);

                for (int i = 0; i < level3List.Count; i++)
                {
                    int ndx = i % Colors.Count;
                    if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                    level3List[i].colorBackground = randomcolors[ndx];
                }

                level3Adapter = new TokChannelsLevelAdapter(this, level3List, 3);
                RecyclerLevel3.SetAdapter(level3Adapter);
            }
            else
            {
                if (level == 2 || level2Selected == "All")
                {
                    RecyclerLevel3.Visibility = ViewStates.Gone;
                }

                level2Selected = level2Selected == "All" ? "" : level2Selected;
                level3Selected = level3Selected == "All" ? "" : level3Selected;
                SetupViewPager(ViewpagerChannel);
                TabChannel.SetupWithViewPager(ViewpagerChannel);
            }
        }
        private void setupAppBar()
        {
            SetSupportActionBar(MainToolbar);
        }
        private void setupDrawerLayout()
        {
            drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            mDrawerToggle = new ActionBarDrawerToggle(this, drawerLayout, MainToolbar, Resource.String.opened, Resource.String.closed);
            drawerLayout.AddDrawerListener(mDrawerToggle);
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            /* This little thing will display our actual hamburger icon*/
            if (mDrawerToggle == null)
            {
                setupDrawerLayout();
            }
            mDrawerToggle.SyncState();
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }
        private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            collapseGroup();
        }

        private void collapseGroup()
        {
            for (int i = 0; i < ExpandableList.Count; i++)
            {
                ExpandableList.CollapseGroup(i);
            }
        }

        private void ShowSearchView()
        {
            double widthD = getLayoutWidth();
            var searchDialog = new Dialog(this);
            searchDialog.SetContentView(Resource.Layout.custom_searchview);
            searchDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            searchDialog.Show();
            searchDialog.SetCanceledOnTouchOutside(true);
            searchDialog.SetCancelable(true);

            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            searchDialog.Window.SetLayout(int.Parse((widthD * 0.80).ToString()), LayoutParams.WrapContent);
            searchDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            // Access Popup layout fields like below  
            var SearchV = searchDialog.FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.searchView);
            SearchV.Background = ContextCompat.GetDrawable(this, Resource.Drawable.rounded_white_background);
            SearchV.BackgroundTintList = fab_AddTok.BackgroundTintList;

            // Events for that popup layout  
            SearchManager searchManager = (SearchManager)GetSystemService(Context.SearchService);
            SearchV.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            if (ViewpagerChannel.CurrentItem == 0)
            {
                SearchV.QueryHint = "Search for Toks...";
            }
            else
            {
                SearchV.QueryHint = "Search for Community...";
            }
            SearchV.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                if (CurrentPage != null)
                {
                    if (ViewpagerChannel.CurrentItem == 0)
                    {
                        RunOnUiThread(async () => await ((TokChannelToksFragment)CurrentPage).LoadToks(this.Title, level2Selected, level3Selected, true, e.NewText));
                    }
                    else
                    {
                        RunOnUiThread(async () => await ((TokChannelCommunitiesFragment)CurrentPage).GetCommunities(this.Title, level2Selected, level3Selected, "", true, e.NewText));
                    }
                }

                searchDialog.Dismiss();
                e.Handled = true;
            };

            SearchV.Close += delegate
            {
                //Currently not working
                //TODO When searchview is cancel, should hide the text.
                var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                if (CurrentPage != null)
                {
                    if (ViewpagerChannel.CurrentItem == 0)
                    {
                        ((TokChannelToksFragment)CurrentPage).HideSearchResult();
                    }
                    else
                    {
                        ((TokChannelCommunitiesFragment)CurrentPage).HideSearchResult();
                    }
                }

                searchDialog.Dismiss();
            };
        }
        
        public void AddClassGroupCollection(ClassGroup item, bool isSave = true)
        {
            var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
            if (CurrentPage != null)
            {
                if (ViewpagerChannel.CurrentItem == 1)
                {
                    ((TokChannelCommunitiesFragment)CurrentPage).AddClassGroupCollection(item, isSave);
                }
            }
        }

        /*public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_tokchannel, menu);

            var searchItem = menu.FindItem(Resource.Id.item_search);
            var view = LayoutInflater.Inflate(Resource.Layout.custom_searchview, null); ;
            SearchV = view.FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.searchView);

            searchItem.SetActionView(SearchV);

            SearchManager searchManager = (SearchManager)GetSystemService(Context.SearchService);

            SearchV.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            SearchV.QueryHint = "Search for Toks...";
            SearchV.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                if (CurrentPage != null)
                {
                    if (ViewpagerChannel.CurrentItem == 0)
                    {
                        RunOnUiThread(async () => await ((TokChannelToksFragment)CurrentPage).LoadToks(this.Title, level2Selected, level3Selected, true, e.NewText));
                    }
                    else
                    {
                        RunOnUiThread(async () => await ((TokChannelCommunitiesFragment)CurrentPage).GetCommunities(this.Title, level2Selected, level3Selected, "", true, e.NewText));
                    }
                }

                e.Handled = true;
            };

            SearchV.Close += delegate
            {
                //Currently not working
                //TODO When searchview is cancel, should hide the text.
                var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                if (CurrentPage != null)
                {
                    if (ViewpagerChannel.CurrentItem == 0)
                    {
                        ((TokChannelToksFragment)CurrentPage).HideSearchResult();
                    }
                    else
                    {
                        ((TokChannelCommunitiesFragment)CurrentPage).HideSearchResult();
                    }
                }
            };

            // change hint color
            TextView searchText = (TextView)SearchV.FindViewById(Resource.Id.search_src_text);
            searchText.SetTextColor(Color.White);
            searchText.SetHintTextColor(Color.LightGray);

            return base.OnCreateOptionsMenu(menu);
        }*/

        private void LoadLeftListNavigation()
        {
            var ListDataHeader = LeftNavView.LoadTokChannelNavigation(this);

            var ListDataChild = new Dictionary<string, List<string>>();

            var ExpandableListAdapter  = new ExpandableListAdapter(this, ListDataHeader, ListDataChild, ExpandableList);
            ExpandableList.SetAdapter(ExpandableListAdapter);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    //If menu is open, close it. Else, open it.
                    if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
                        drawerLayout.CloseDrawers();
                    else
                        drawerLayout.OpenDrawer(GravityCompat.Start);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_CODE_ADD_TOK && resultCode == Result.Ok)
            {
                var tokModel = JsonConvert.DeserializeObject<ClassTokModel>(data.GetStringExtra("tokModel"));
                if (tokModel != null)
                {
                    ClassTokCollection.Insert(0, tokModel);

                    var CurrentPage = SupportFragmentManager.FindFragmentByTag("android:switcher:" + ViewpagerChannel.Id + ":" + ViewpagerChannel.CurrentItem);
                    if (CurrentPage != null)
                    {
                        if (ViewpagerChannel.CurrentItem == 0)
                        {
                            ((TokChannelToksFragment)CurrentPage).SetClassTokRecyclerAdapter();
                        }
                    }
                }
            }
        }
        //AndroidX.AppCompat.Widget.SearchView SearchV;
        public RecyclerView RecyclerLevel2 => FindViewById<RecyclerView>(Resource.Id.recycyclerLevel2);
        public RecyclerView RecyclerLevel3 => FindViewById<RecyclerView>(Resource.Id.recycyclerLevel3);
        public FloatingActionButton fab_AddTok => FindViewById<FloatingActionButton>(Resource.Id.fab_AddTok);
        public ViewPager ViewpagerChannel => FindViewById<ViewPager>(Resource.Id.viewpagerChannel);
        public TabLayout TabChannel => FindViewById<TabLayout>(Resource.Id.tabChannel);
        public Button btnTab1 => FindViewById<Button>(Resource.Id.btnTab1);
        public Button btnTab2 => FindViewById<Button>(Resource.Id.btnTab2);
        public View MidTabSpace => FindViewById<View>(Resource.Id.viewMidTab);
        public BottomNavigationView BottomNav => FindViewById<BottomNavigationView>(Resource.Id.bottomNav);
        public AndroidX.AppCompat.Widget.Toolbar MainToolbar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.main_toolbar);
        public ExpandableListView ExpandableList => FindViewById<ExpandableListView>(Resource.Id.navigation_left);
        public NavigationView LeftNavigation => FindViewById<NavigationView>(Resource.Id.main_navigation_view);
        public LinearLayout LinearCoinsToast => LeftNavigation.GetHeaderView(0).FindViewById<LinearLayout>(Resource.Id.LinearCoinsToast);
        public TextView TextCoinsToast => LeftNavigation.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TextCoinsToast);
        public TextView txtGroup => LeftNavigation.GetHeaderView(0).FindViewById<TextView>(Resource.Id.lblGroup);
        public TextView txtFeedOptionType => LeftNavigation.GetHeaderView(0).FindViewById<TextView>(Resource.Id.txtFeedOptionType);
        public ImageView LeftNavArrowDown => LeftNavigation.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.lblGroupSelect);
    }
}