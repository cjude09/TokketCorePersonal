using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Preference;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Tabs;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Permissions;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Extensions;
using Tokket.Shared.Models;
using Tokket.Android.ViewModels;
using Tokket.Core;
using Xamarin.Essentials;
using ServiceAccount = Tokket.Shared.Services;
using Tokket.Android.TokQuest;
using XFragment = AndroidX.Fragment.App.Fragment;
using Tokket.Shared.Services;
using AndroidX.AppCompat.Widget;
using System.Linq;
using System.IO;
using Tokket.Android.Custom;
using Android.Text;
using Android.Webkit;
using Tokket.Android.Views;
using AlertDialog = Android.App.AlertDialog;
using Result = Android.App.Result;
using Tokket.Android.Helpers;
using Bumptech.Glide;
using Bumptech.Glide.Request;

namespace Tokket.Android
{
    [Activity(Label = "Main Activity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity, View.IOnTouchListener
    {
        private int REQUEST_PROFILE_MEMBERSHIP = 10001;
        private bool bulb_outlined = true;
        internal static MainActivity Instance { get; private set; }
        public DrawerLayout drawerLayout;
        NavigationView m_navigationView;
        private ActionBarDrawerToggle mDrawerToggle;
        AdapterStateFragmentX adapter;
        public Tokket.Android.LockableViewPager MainViewPager;
        List<XFragment> fragments = new List<XFragment>();
        List<string> fragmentTitles = new List<string>();
        public TabLayout tabLayout; Intent nextActivity;
        public ProfilePageViewModel ProfileVm => App.Locator.ProfilePageVM;
        public HomePageViewModel HomeVm => App.Locator.HomePageVM;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        Color defaultPrimaryColor, defaultPrimaryDarkerColor;
        ExpandableListAdapter mMenuClassTokAdapter;
        TokketUser tokketUser;
        public View headerView;
        public ImageView userphoto;
        public string cacheUserPhoto = "";
        public Bitmap bitmapUserPhoto;
        public bool isOnProfile { get; private set; } = false;
        private enum ActivityName
        {
            ActivityHome = 500,
            ActivityProfile = 501
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Shared.Extensions.Screen.SetScreenSize(
                 (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density),
                 (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density)
                );
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }

            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //Close login
            if (LoginActivity.Instance != null)
            {
                LoginActivity.Instance.CloseLogin();
            }

            Instance = this;

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            m_navigationView = FindViewById<NavigationView>(Resource.Id.main_navigation_view);

            drawerLayout.DrawerClosed -= DrawerLayout_DrawerClosed;
            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;

            //setting the header layout values
            headerView = m_navigationView.GetHeaderView(0);
            userphoto = headerView.FindViewById<ImageView>(Resource.Id.img_profileUserPhoto);
            var user = headerView.FindViewById<TextView>(Resource.Id.lblUser);
            var groupSelection = headerView.FindViewById<ImageView>(Resource.Id.lblGroupSelect);

            MainViewPager = FindViewById<LockableViewPager>(Resource.Id.viewpager);
            tabLayout = this.FindViewById<TabLayout>(Resource.Id.tabLayout);

            MainViewPager.SwipeLocked = true;
#if (_TOKKEPEDIA)
            MainViewPager.OffscreenPageLimit = 4; //This is used in order to avoid the pages to keep on refreshing
#endif
#if (_CLASSTOKS)
            MainViewPager.OffscreenPageLimit = 3;
#endif
            setupAppBar();

#if (_CLASSTOKS)
            imageViewLogoText.SetImageResource(Resource.Drawable.classtok_text);
#endif

            setMainColors();

            AppCenter.Start("f55f6052-d41d-4625-9dac-56bbeec98599",
                           typeof(Analytics), typeof(Crashes));

            Settings.ImageBrowseCrop = null;

#if (_CLASSTOKS)
            txtFeedOptionType.Text = Settings.FilterFeed == (int)FilterType.All ? "PUBLIC CLASS TOKS" : "MY CLASS TOKS  ";
            txtFeedOptionType.Visibility = ViewStates.Visible;
#endif
            
            tokketUser = Settings.GetTokketUser();
            cacheUserPhoto = GetCachedAsync<string>(tokketUser.Id);
            if (!string.IsNullOrEmpty(cacheUserPhoto))
            {
                var userPhotoByte = Convert.FromBase64String(cacheUserPhoto);
                var bitmap = BitmapFactory.DecodeByteArray(userPhotoByte, 0, userPhotoByte.Length);
                //Resize image
                bitmapUserPhoto = bitmap;

                userphoto.SetImageBitmap(bitmapUserPhoto);
            }
            else
            {
                Glide.With(this).Load(tokketUser.UserPhoto).Into(userphoto);
            }

            var displayName = tokketUser.DisplayName;
            if (displayName.Length > 25)
            {
                user.Text = displayName.Substring(0, 25) + "...";
            }
            else
            {
                user.Text = displayName;
            }

            if (tokketUser.AccountType.ToLower() == "group")
            {
                var subaccount = Settings.GetTokketSubaccount();
                if (subaccount?.SubaccountName.Length > 25)
                {
                    txtGroup.Text = subaccount.SubaccountName.Substring(0, 25) + "...";
                }
                else
                {
                    txtGroup.Text = subaccount.SubaccountName;
                }
            }

            if (tokketUser.AccountType == "individual")
            {
                txtGroup.Text = tokketUser.CurrentHandle ?? "";

                //If user have title/s
                if (!string.IsNullOrEmpty(txtGroup.Text))
                {
                    groupSelection.Visibility = ViewStates.Visible;
                }
                else
                {
                    groupSelection.Visibility = ViewStates.Gone;
                }
            }

            var GifCoinIcon = headerView.FindViewById<ImageView>(Resource.Id.gif_profileCoins);
            Glide.With(this)
                .Load(Resource.Drawable.gold)
                .Into(GifCoinIcon);

            long longcoins = Settings.UserCoins;

            TextView TextProfileCoins = headerView.FindViewById<TextView>(Resource.Id.TextProfileCoins);
            TextProfileCoins.Text = longcoins.ToKMB();

            if (mDrawerToggle == null)
            {
                setupDrawerLayout();
            }

            m_navigationView.NavigationItemSelected -= M_navigationView_NavigationItemSelected;
            m_navigationView.NavigationItemSelected += M_navigationView_NavigationItemSelected;


            fab_AddTok.Click += (object sender, EventArgs e) =>
            {
#if (_TOKKEPEDIA)
                nextActivity = new Intent(this, typeof(AddTokActivity));
                nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                nextActivity.SetFlags(ActivityFlags.NewTask);
                this.StartActivity(nextActivity);
#endif
#if (_CLASSTOKS)
                nextActivity = new Intent(this, typeof(AddClassTokActivity));
                nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                nextActivity.SetFlags(ActivityFlags.NewTask);
                this.StartActivity(nextActivity);
#endif
            };

            loadHomeFragments();

            setupViewPager(MainViewPager);
            tabLayout.SetupWithViewPager(MainViewPager);
            setupHomeTabIcons();

#if (_CLASSTOKS)
            loadClassTokNavigation();

            NavigationClassToks.Visibility = ViewStates.Visible;
            ScrollNormalToks.Visibility = ViewStates.Gone;
#endif

            MainViewPager.PageSelected -= Viewpager_PageSelected;
            MainViewPager.PageSelected += Viewpager_PageSelected;

#if (_TOKKEPEDIA)
                    //setting the value of the quote
                    GetQuoteOfTheHour(m_navigationView);
#endif
        }

        private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            collapseGroup();
        }

        private void collapseGroup()
        {
            for (int i = 0; i < NavigationClassToks.Count; i++)
            {
                NavigationClassToks.CollapseGroup(i);
            }
        }

        private void loadClassTokNavigation()
        {
            var ListDataHeader = LeftNavView.LoadClassTokHeaderNavigation(this);

            var ListDataChild = LeftNavView.LoadClassToChildrenNavigation();

            mMenuClassTokAdapter = new ExpandableListAdapter(this, ListDataHeader, ListDataChild, NavigationClassToks);
            NavigationClassToks.SetAdapter(mMenuClassTokAdapter);
            NavigationClassToks.GroupClick += NavigationClassToks_GroupClick;
            NavigationClassToks.ChildClick -= NavigationClassToks_ChildClick;
            NavigationClassToks.ChildClick += NavigationClassToks_ChildClick;
        }

        private void NavigationClassToks_GroupClick(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            var groupList = sender as ListView;

            var mainMenu = mMenuClassTokAdapter.GetGroup(e.GroupPosition);
            switch (mainMenu.ToString())
            {
                case "Tok Channels":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Tok Paks":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Class Sets":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Class Toks":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Study Games":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Tok Groups":
                    Settings.FilterGroup = (int)GroupFilter.OwnGroup;
                    nextActivity = new Intent(this, typeof(ClassGroupListActivity));
                    this.StartActivity(nextActivity);

                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Notifications":
                    nextActivity = new Intent(this, typeof(InvitesActivity));
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Guides":
                    if (NavigationClassToks.IsGroupExpanded(e.GroupPosition))
                    {
                        NavigationClassToks.CollapseGroup(e.GroupPosition);
                    }
                    else
                    {
                        //Collapse other groups before expanding
                        collapseGroup();
                        NavigationClassToks.ExpandGroup(e.GroupPosition);
                    }
                    break;
                case "Settings":
                    nextActivity = new Intent(this, typeof(SettingsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    this.StartActivity(nextActivity);

                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Tok Quest":
                    nextActivity = new Intent(this, typeof(TokQuestMain));
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Alpha Toks":
                    nextActivity = new Intent(this, typeof(AlphaToksMain)); ;
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Peerbook":
                    nextActivity = new Intent(this, typeof(YearbookActivity));
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;
                case "Opportunity":
                    nextActivity = new Intent(this, typeof(OpportunityListActivity));
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;

                case "Training":
                    nextActivity = new Intent(this, typeof(TrainingListActivity));
                    nextActivity.PutExtra("mode","training");
                    StartActivity(nextActivity);
                    drawerLayout.CloseDrawer(GravityCompat.Start);
                    break;

                default:
                    Toast.MakeText(this, mainMenu.ToString(), ToastLength.Long).Show();
                    break;
            }
        }

        private async Task goToGuidePage()
        {
            //await Browser.OpenAsync("https://tokket.com/faq", new BrowserLaunchOptions
            //{
            //    LaunchMode = BrowserLaunchMode.SystemPreferred,
            //    TitleMode = BrowserTitleMode.Show,
            //    PreferredToolbarColor = System.Drawing.Color.AliceBlue,
            //    PreferredControlColor = System.Drawing.Color.Violet
            //});
            var intent = new Intent(this, typeof(GuideActivity));
            StartActivity(intent);
        }

        private void NavigationClassToks_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            var childList = sender as ListView;

            Intent nextActivity;
            var subMenu = mMenuClassTokAdapter.GetChild(e.GroupPosition, e.ChildPosition);
            var mainMenu = mMenuClassTokAdapter.GetGroup(e.GroupPosition);
            switch (mainMenu.ToString())
            {
                case "Tok Channels":
                    nextActivity = new Intent(this, typeof(TokChannelActivity));
                    nextActivity.PutExtra("level1", subMenu.ToString());
                    this.StartActivity(nextActivity);
                    break;
                case "Tok Paks":
                    if (subMenu.ToString() == "Add Tok Paks")
                    {
                        nextActivity = new Intent(this, typeof(AddTokPakActivity));
                        nextActivity.PutExtra("fromGroupModel", false);
                        this.StartActivity(nextActivity);
                    }
                    else if (subMenu.ToString() == "View My Tok Paks")
                    {
                        nextActivity = new Intent(this, typeof(ViewTokPakActivity));
                        nextActivity.PutExtra("isPublic", false);
                        this.StartActivity(nextActivity);
                    }
                    else if (subMenu.ToString() == "View Public Tok Paks")
                    {
                        nextActivity = new Intent(this, typeof(ViewTokPakActivity));
                        nextActivity.PutExtra("isPublic", true);
                        this.StartActivity(nextActivity);
                    }
                    break;
                case "Class Sets":
                    if (subMenu.ToString() == "My Class Sets")
                    {
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                        nextActivity = new Intent(this, typeof(MyClassSetsActivity));
                        nextActivity.PutExtra("isAddToSet", false);
                        nextActivity.PutExtra("TokTypeId", "");
                        nextActivity.SetFlags(ActivityFlags.NewTask);
                        this.StartActivity(nextActivity);
                    }
                    else if (subMenu.ToString() == "Public Class Sets")
                    {
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                        nextActivity = new Intent(this, typeof(MyClassSetsActivity));
                        nextActivity.PutExtra("isAddToSet", false);
                        nextActivity.PutExtra("isPublicClassSets", true);
                        nextActivity.PutExtra("TokTypeId", "");
                        nextActivity.SetFlags(ActivityFlags.NewTask);
                        this.StartActivity(nextActivity);
                    }
                    break;
                case "Class Toks":
                    if (subMenu.ToString() == "My Class Toks")
                    {
                        Settings.FilterFeed = (int)FilterType.Featured;
                        Settings.FilterTag = (int)FilterType.Featured;

                        MainActivity.Instance.txtFeedOptionType.Text = "MY CLASS TOKS";
                        Settings.ClassHomeFilter = (int)FilterType.Featured;
                        Settings.FilterByTypeHome = (int)FilterBy.None;
                        Settings.FilterByItemsHome = "";

                        RunOnUiThread(async () => await ClassToksFragment.Instance.LoadPublicMyClassToks());
                    }
                    else if (subMenu.ToString() == "Public Class Toks")
                    {
                        Settings.FilterFeed = (int)FilterType.All;
                        Settings.FilterTag = (int)FilterType.All;

                        MainActivity.Instance.txtFeedOptionType.Text = "Public Class Toks";
                        Settings.FilterByTypeHome = (int)FilterBy.None;
                        Settings.FilterByItemsHome = "";
                        Settings.ClassHomeFilter = (int)FilterType.All;
                        RunOnUiThread(async () => await ClassToksFragment.Instance.LoadPublicMyClassToks());
                    }

                    MainViewPager.CurrentItem = 0; //CurrentItem of Home is 0
                    break;
                case "Study Games":
                    var subMenux = subMenu.ToString().ToLower();
                    var classSetDialog = new StudyGamesClassSetDialog(this, subMenux);
                    MainActivity.Instance.showDialog(classSetDialog);
                    break;
                case "Guides":
                    var intent = new Intent(this, typeof(GuidesActivity));
                    var subText = subMenu.ToString().Replace("•", "").Trim();
                    switch (subText) {
                        case "How to Add Class Toks":
                            var intentAddClassTokGuide = new Intent(this, typeof(GuideAddClassTokActivity));
                            StartActivity(intentAddClassTokGuide);
                            break;
                        case "How to Create a Tok Group":
                            var intentAddTokGroupGuide = new Intent(this, typeof(GuideAddTokGroupActivity));
                            StartActivity(intentAddTokGroupGuide);
                            break;
                        case "How to Create Class Toks":
                            RunOnUiThread(async () => await goToGuidePage());
                            break;
                        case "How to Create Presentation":
                            intent.PutExtra("InstructionSetup", "1");
                            intent.PutExtra("GuideTitle", subText);
                            StartActivity(intent);
                            break;
                        case "How to Create a Family Account":
                            intent.PutExtra("InstructionSetup", "2");
                            intent.PutExtra("GuideTitle", subText);
                            StartActivity(intent);
                            break;
                        case "How to Create an Organization Account":
                            intent.PutExtra("InstructionSetup", "3");
                            intent.PutExtra("GuideTitle", subText);
                            StartActivity(intent);
                            break;
                    }
                    break;
                default:
                    break;
            }

            drawerLayout.CloseDrawer(GravityCompat.Start);

            /*switch (subMenu.ToString())
            {
                default:
                   
                    break;
            }*/
        }

        public void loadStudyGamesOptions(string model, string subMenux)
        {
            if (!string.IsNullOrEmpty(model))
            {
                var classSetModel = JsonConvert.DeserializeObject<ClassSetModel>(model);
                var subTitle = "User: " + Settings.GetTokketUser().DisplayName;
                if (subMenux == "tok cards")
                {
                 
                    if (classSetModel.TokIds.Count > 2) {
                        nextActivity = new Intent(this, typeof(TokCardsMiniGameActivity));
                        nextActivity.PutExtra("classsetModel", model);
                        this.StartActivity(nextActivity);
                    }
                    else
                    {
                        var mssgDialog = new AlertDialog.Builder(Instance);
                        var alertMssg = mssgDialog.Create();
                        alertMssg.SetTitle("");
                        alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertMssg.SetMessage("Tok cards requires at least 2 toks in the set. Add more toks to play.");
                        alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                        alertMssg.Show();
                    }

                }
                else if (subMenux == "tok choice")
                {
                     classSetModel = JsonConvert.DeserializeObject<ClassSetModel>(model);
                    if (classSetModel.TokIds.Count > 3)
                    {
                        var alertDiag = new AlertDialog.Builder(Instance);
                        alertDiag.SetTitle("Tok Choice");
                        alertDiag.SetMessage("Continue to Play Set?");
                        alertDiag.SetPositiveButton(Html.FromHtml("<font color='#dc3545'>Return</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                        {
                            alertDiag.Dispose();
                        });
                        alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Play</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                        {
                            nextActivity = new Intent(this, typeof(TokChoiceActivity));
                            nextActivity.PutExtra("classsetModel", model);
                            this.StartActivity(nextActivity);
                        });
                        var diag = alertDiag.Create();
                        diag.Show();
                    }
                    else
                    {
                        var mssgDialog = new AlertDialog.Builder(Instance);
                        var alertMssg = mssgDialog.Create();
                        alertMssg.SetTitle("");
                        alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertMssg.SetMessage("Tok Choice requires at least 3 toks in the set. Add more toks to play.");
                        alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                        alertMssg.Show();
                    }
                }
                else if (subMenux == "tok match")
                {
                    if (classSetModel.TokIds.Count > 2)
                    {
                        nextActivity = new Intent(this, typeof(TokMatchActivity));
                        nextActivity.PutExtra("classsetModel", model);
                        nextActivity.PutExtra("isSet", true);
                        this.StartActivity(nextActivity);
                    }
                    else {
                        var mssgDialog = new AlertDialog.Builder(Instance);
                        var alertMssg = mssgDialog.Create();
                        alertMssg.SetTitle("");
                        alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertMssg.SetMessage("Tok Match requires at least 4 toks in the set. Add more toks to play.");
                        alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                        alertMssg.Show();
                    }
                }
            }
        }

        private void setMainColors()
        {
            var headerlayout = m_navigationView.GetHeaderView(0);
#if (_TOKKEPEDIA)
            var tokgroup_b = (ImageView)FindViewById(Resource.Id.tokgroup_b);
            var play_icon = (ImageView)FindViewById(Resource.Id.play_icon);
            var class_b = (ImageView)FindViewById(Resource.Id.class_b);
            var settings_black_36dp = (ImageView)FindViewById(Resource.Id.settings_black_36dp);
            var tokgroup_bQuote = (ImageView)FindViewById(Resource.Id.tokgroup_bQuote);
            // Black is Settings.CurrentTheme = 0          
            // White is Settings.CurrentTheme = 1
            tokgroup_b.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            play_icon.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            class_b.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            settings_black_36dp.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            tokgroup_bQuote.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
#endif

#if (_CLASSTOKS)
            /*var imageViewClassSets = (ImageView)FindViewById(Resource.Id.imageViewClassSets);
            var imageViewClassGroup = (ImageView)FindViewById(Resource.Id.imageViewClassGroup);
            var imageViewClassNotification = (ImageView)FindViewById(Resource.Id.imageViewClassNotification);
            var imageViewClassGuide = (ImageView)FindViewById(Resource.Id.imageViewClassGuide);
            var imageViewClassFolder = (ImageView)FindViewById(Resource.Id.imageViewClassFolder);
            var imageViewClassSettings = (ImageView)FindViewById(Resource.Id.imageViewClassSettings);
            // Black is Settings.CurrentTheme = 0          
            // White is Settings.CurrentTheme = 1
            imageViewClassSets.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            imageViewClassGroup.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            imageViewClassNotification.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            imageViewClassGuide.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            imageViewClassFolder.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));
            imageViewClassSettings.SetColorFilter(Settings.CurrentTheme == 0 ?
                Android.Graphics.Color.ParseColor("#000000") :
                Android.Graphics.Color.ParseColor("#ffffff"));*/
#endif

#if (_TOKKEPEDIA)
            defaultPrimaryColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary));
            defaultPrimaryDarkerColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary_darker));
            fab_AddTok.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#8a00d9"));
#endif

#if (_CLASSTOKS)
            defaultPrimaryColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent));
            defaultPrimaryDarkerColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent_darker));
#endif

            headerlayout.FindViewById(Resource.Id.relativeLayoutHeader).SetBackgroundColor(Settings.CurrentTheme == (int)ThemeStyle.Light ?
                defaultPrimaryColor :
                defaultPrimaryDarkerColor);

            MainToolbar.SetBackgroundColor(Settings.CurrentTheme == (int)ThemeStyle.Light ?
                defaultPrimaryColor :
                defaultPrimaryDarkerColor);

            tabLayout.SetBackgroundColor(Settings.CurrentTheme == (int)ThemeStyle.Light ?
                defaultPrimaryColor :
                defaultPrimaryDarkerColor);
            
            BulbImg.SetImageResource(Settings.CurrentTheme == (int)ThemeStyle.Light ?
                Resource.Drawable.dark_lightbulb :
                Resource.Drawable.lightbulb);
            bulb_outlined = Settings.CurrentTheme == (int)ThemeStyle.Light ? true : false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                if (v.ContentDescription == "coins")
                {
                    LinearCoinsToast.Visibility = ViewStates.Gone;
                }
            }
            else if (e.Action == MotionEventActions.Down)
            {
                if (v.ContentDescription == "coins")
                {
                    LinearCoinsToast.Visibility = ViewStates.Visible;
                    TextCoinsToast.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);
                }
            }
            return true;
        }

        /*private byte[] ConvertByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }*/
        private void loadHomeFragments()
        {
            fragments.Clear();
            fragmentTitles.Clear();
#if (_TOKKEPEDIA)
            fragments.Add(new home_fragment());
            fragmentTitles.Add("Home");
#endif
#if (_CLASSTOKS)
            fragments.Add(new ClassToksFragment("", _isLoadTab: true));
            fragmentTitles.Add("Class Toks");
#endif
            fragments.Add(new SearchFragment());

#if (_TOKKEPEDIA)
            fragments.Add(new notification_fragment());
#endif
            fragments.Add(new ProfileFragment());

            fragmentTitles.Add("Search");
#if (_TOKKEPEDIA)
            fragmentTitles.Add("Notifications");
#endif
            fragmentTitles.Add("Profile");
        }
        private void setupHomeTabIcons()
        {
#if (_TOKKEPEDIA)
            tabLayout.GetTabAt(0).SetIcon(Resource.Drawable.ic_home48_dp);
            tabLayout.GetTabAt(1).SetIcon(Resource.Drawable.ic_search_white_24dp);
            tabLayout.GetTabAt(2).SetIcon(Resource.Drawable.ic_notification_white48);
            tabLayout.GetTabAt(3).SetIcon(Resource.Drawable.ic_profile_white48);
#endif
#if (_CLASSTOKS)
            setupClassTokTabIcons();
#endif
        }
        private void setupClassTokTabIcons()
        {
            tabLayout.GetTabAt(0).SetIcon(Resource.Drawable.classtok_white_48);
            tabLayout.GetTabAt(1).SetIcon(Resource.Drawable.ic_search_white_24dp);
            //tabLayout.GetTabAt(2).SetIcon(Resource.Drawable.ic_notification_white48);
            tabLayout.GetTabAt(2).SetIcon(Resource.Drawable.ic_profile_white48);
        }
        void setupViewPager(ViewPager viewPager)
        {
            adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            MainViewPager.Adapter.NotifyDataSetChanged();
            adapter.NotifyDataSetChanged();
            MainViewPager.PageSelected -= MainPager_PageSelected;
            MainViewPager.PageSelected += MainPager_PageSelected;
            //mainviewpager.SetPageTransformer(true, new FadeTransformerViewPager());
        }

        private void MainPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            switch (e.Position) {
                case 0: //classtoks fragment
                    isOnProfile = false;
                    fab_AddTok.Visibility = ViewStates.Visible;
                    break;
                case 1: //search fragment
                    isOnProfile = false;
                    fab_AddTok.Visibility = ViewStates.Gone;
                    break;
                case 2: //profile fragment
                    isOnProfile = true;
                    fab_AddTok.Visibility = ViewStates.Visible;
                    break;
            }
        }

        private void setupAppBar()
        {
            SetSupportActionBar(MainToolbar);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Clear();

#if (_TOKKEPEDIA)
             if (tabLayout.SelectedTabPosition == 3) //Profile Tab
#endif
#if (_CLASSTOKS)
            if (tabLayout.SelectedTabPosition == 2) //Profile Tab
#endif
            {
                MenuInflater.Inflate(Resource.Menu.profile_menu, menu);

                //IMenuItem Android.Resource.Id
                var itemReport = menu.FindItem(Resource.Id.item_Report);

                itemReport.SetVisible(false);
            }
#if (_TOKKEPEDIA)
            else if (tabLayout.SelectedTabPosition == 2) //notification
            {

            }
#endif
            else
            {
                MenuInflater.Inflate(Resource.Menu.main, menu);
            }
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.Clear();
#if (_TOKKEPEDIA)
            if (tabLayout.SelectedTabPosition == 3) //Profile
#endif
#if (_CLASSTOKS)
            if (tabLayout.SelectedTabPosition == 2) //Profile
#endif
            {
                MenuInflater.Inflate(Resource.Menu.profile_menu, menu);

                //IMenuItem Android.Resource.Id
                var itemReport = menu.FindItem(Resource.Id.item_Report);

                itemReport.SetVisible(false);
            }
#if (_TOKKEPEDIA)
            else if (tabLayout.SelectedTabPosition == 2) //Notification
            {

            }
#endif
            else
            {
                MenuInflater.Inflate(Resource.Menu.main, menu);
            }

            return base.OnPrepareOptionsMenu(menu);
        }
        public override void OnBackPressed()
        {
            Settings.ActivityInt = (int)ActivityType.HomePage;
            base.OnBackPressed();
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
        //When This is Clicked from the profileTab
        [Java.Interop.Export("OnClickProfileImage")]
        public void OnClickProfileImage(View v)
        {
            if (ProfileVm.tokketUser.Id == Settings.UserId)
            {
                Settings.ActivityInt = (int)ActivityType.ProfileTabActivity;
                BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this, ProfileFragment.Instance.ProfileUserPhoto);
                bottomsheet.Show(this.SupportFragmentManager, "tag");
                /*Intent myIntent = new Intent();
                myIntent.SetType("image/*");
                myIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(myIntent, "Select Picture"), (int)ActivityType.ProfileTabActivity);*/
            }
        }

        [Java.Interop.Export("OnClickCoverPhoto")]
        public void OnClickCoverPhoto(View v)
        {
            if (ProfileVm.tokketUser.Id == Settings.UserId)
            {
                Settings.BrowsedImgTag = Convert.ToInt32(v.ContentDescription);
                BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this, ProfileFragment.Instance.ProfileCoverPhoto);
                bottomsheet.Show(this.SupportFragmentManager, "tag");

                /*Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.ProfileActivity);*/
            }
        }


        [Java.Interop.Export("onClickGroupSelect")]
        public void onClickGroupSelect(View v)
        {
            if (tokketUser.AccountType == "individual")
            {
                Intent nextActivity = new Intent(this, typeof(TokHandleActivity));
                StartActivity(nextActivity);
                /*Intent nextActivity = new Intent(this, typeof(ProfileTitleActivity));
                StartActivity(nextActivity);*/
            }
            else
            {
                Intent nextActivity = new Intent(this, typeof(SubAccountActivity));
                StartActivity(nextActivity);
            }
        }


        [Java.Interop.Export("onClickBulb")]
        public void onClickBulb(View v)
        {

            if (bulb_outlined)
            {
                //Dark
                bulb_outlined = false;
                Settings.CurrentTheme = (int)ThemeStyle.Dark;
            }
            else
            {
                bulb_outlined = true;
                Settings.CurrentTheme = (int)ThemeStyle.Light;
            }

            Intent nextActivity = new Intent(this, typeof(MainActivity));
            Finish();
            this.OverridePendingTransition(0, 0);
            StartActivity(nextActivity);
        }

        [Java.Interop.Export("onRedirectProfile")]
        public void onRedirectProfile(View v)
        {
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", tokketUser.Id);
            MainActivity.Instance.StartActivity(nextActivity);
        }

        [Java.Interop.Export("OnTapSearchTip")]
        public void OnTapSearchTip(View v)
        {
            SearchFragment.Instance.framTip.Visibility = ViewStates.Gone;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent nextActivity;
            switch (item.ItemId)
            {
                //...

                //Handle sandwich menu icon click
                case HomeId:
                    //If menu is open, close it. Else, open it.
                    if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
                        drawerLayout.CloseDrawers();
                    else
                        drawerLayout.OpenDrawer(GravityCompat.Start);
                    break;
                case Resource.Id.btnFilter: //Accessed from Home
                    int tabIndx = tabLayout.SelectedTabPosition;
                    switch (tabIndx)
                    {
                        case 0: //Home
                            Settings.MaintTabInt = (int)MainTab.Home;
                            break;
                        case 1: //Search
                            Settings.MaintTabInt = (int)MainTab.Search;
                            break;
#if (_TOKKEPEDIA)
                            case 2: //Notification
                            Settings.MaintTabInt = (int)MainTab.Notification;
                            break;
                        case 3: //Profile
                            break;
#endif
#if (_CLASSTOKS)
                        case 2: //Profile
                            break;
#endif
                        default:
                            Settings.MaintTabInt = (int)MainTab.Home;
                            break;
                    }
                    nextActivity = new Intent(this, typeof(FilterActivity));
                    if (Settings.MaintTabInt == (int)MainTab.Home)
                    {
                        nextActivity.PutExtra("activitycaller", "Home");
                    }
                    else if (Settings.MaintTabInt == (int)MainTab.Search)
                    {
                        nextActivity.PutExtra("activitycaller", "Search");
                    }
#if (_TOKKEPEDIA)
                    editor.PutString("TokModelList", JsonConvert.SerializeObject(HomeVm.TokDataList));
                    editor.Apply();
#endif
                    StartActivityForResult(nextActivity, (int)ActivityName.ActivityHome);

                    break;
                case Resource.Id.item_filter: //Accessed from Profile Tab
                    editor.PutString("TokModelList", JsonConvert.SerializeObject(ProfileFragment.Instance.TokDataList));
                    editor.Apply();

                    Settings.MaintTabInt = (int)MainTab.Profile;
                    nextActivity = new Intent(this, typeof(FilterActivity));
                    nextActivity.PutExtra("activitycaller", "Profile");
                    nextActivity.PutExtra("SubTitle", "User: " + Settings.GetTokketUser().DisplayName);
                    StartActivityForResult(nextActivity, (int)ActivityName.ActivityProfile);
                    break;
                case Resource.Id.item_avatar:
                    nextActivity = new Intent(this, typeof(AvatarsActivity));
                    this.StartActivityForResult(nextActivity, (int)ActivityType.AvatarsActivity);
                    break;
                case Resource.Id.item_sets:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.ProfileSetsView);
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    StartActivity(nextActivity);
                    break;
                case Resource.Id.item_Share:
                    RunOnUiThread(async () => await Share.RequestAsync(new ShareTextRequest
                    {
                        Uri = Shared.Config.Configurations.Url + "user/" + ProfileVm.UserId,
                        Title = ProfileVm.UserDisplayName
                    }));

                    break;
                /*case Resource.Id.item_titles:
                    nextActivity = new Intent(this, typeof(ProfileTitleActivity));
                    StartActivity(nextActivity);
                    break;*/
                case Resource.Id.item_badges:
                    nextActivity = new Intent(this, typeof(BadgesActivity));
                    this.StartActivityForResult(nextActivity, 40011);
                    break;
                case Resource.Id.item_Membership:
                    nextActivity = new Intent(this, typeof(ProfileMembershipActivity));
                    StartActivityForResult(nextActivity, REQUEST_PROFILE_MEMBERSHIP);
                    break;
                case Resource.Id.item_tokHandle:
                    nextActivity = new Intent(this, typeof(TokHandleActivity));
                    StartActivity(nextActivity);
                        
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void M_navigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.leftmenu_TokGroup:
                    nextActivity = new Intent(this, typeof(TokGroupsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    this.StartActivity(nextActivity);
                    break;

                case Resource.Id.leftmenu_Set:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    this.StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_ClassToks:
                    MainToolbar.SetBackgroundColor(Color.ParseColor("#3498db"));
                    tabLayout.SetBackgroundColor(Color.ParseColor("#3498db"));
                    ScrollNormalToks.Visibility = ViewStates.Gone;
                    NavigationClassToks.Visibility = ViewStates.Visible;
                    fragments.RemoveAt(0);
                    fragmentTitles.RemoveAt(0);
                    fragments.Insert(0, new ClassToksFragment(Settings.UserId));
                    fragmentTitles.Insert(0, "Class Toks");

                    adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
                    setupViewPager(MainViewPager);
                    setupClassTokTabIcons();
                    break;
                //case Resource.Id.leftmenu_Privacy:
                //    break;

                //case Resource.Id.leftmenu_Theme:
                //    break;
                case Resource.Id.leftmenu_Settings:
                    nextActivity = new Intent(this, typeof(SettingsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    MainActivity.Instance.StartActivity(nextActivity);
                    break;

                //Class Toks Menu Below
                case Resource.Id.leftmenu_classtok_ClassSets:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    nextActivity = new Intent(this, typeof(MyClassSetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    this.StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_classtok_ClassGroups:
                    nextActivity = new Intent(this, typeof(ClassGroupListActivity));
                    StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_classtok_Invites:
                    nextActivity = new Intent(this, typeof(InvitesActivity));
                    StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_classtok_Guide:
                    break;
                case Resource.Id.leftmenu_classtok_Folders:
                    break;
                case Resource.Id.leftmenu_classtok_Tokkepedia:
                    MainToolbar.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    tabLayout.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    m_navigationView.Menu.Clear();
                    ScrollNormalToks.Visibility = ViewStates.Visible;
                    NavigationClassToks.Visibility = ViewStates.Gone;
                    //m_navigationView.InflateMenu(Resource.Menu.left_menus);

                    GetQuoteOfTheHour(m_navigationView);

                    fragments.RemoveAt(0);
                    fragmentTitles.RemoveAt(0);
                    fragments.Insert(0, new HomeFragment());
                    fragmentTitles.Insert(0, "Home");
                    fragments.Insert(1, new SearchFragment());
                    fragmentTitles.Insert(1, "Search");
                    fragments.Insert(2, new NotificationFragment());
                    fragmentTitles.Insert(2, "Notifications");
                    fragments.Insert(3, new ProfileFragment());
                    fragmentTitles.Insert(3, "Profile");


                    adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
                    setupViewPager(MainViewPager);
                    setupHomeTabIcons();


                    //change header layout bg color for tokkepedia
                    var headerlayout = FindViewById(Resource.Id.relativeLayoutHeader);
                    headerlayout.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.navy_blue)));
                    break;
            }

            e.MenuItem.SetChecked(false);
            drawerLayout.CloseDrawer(GravityCompat.Start);
        }

        [Java.Interop.Export("OnClickClassMenu")]
        public void OnClickClassMenu(View v)
        {
            switch (v.Id)
            {
                //Class Toks Menu Below
                case Resource.Id.leftmenu_classtok_ClassSets:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    nextActivity = new Intent(this, typeof(MyClassSetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    this.StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_classtok_ClassGroups:
                    nextActivity = new Intent(this, typeof(ClassGroupListActivity));
                    MainActivity.Instance.StartActivity(nextActivity);
                    break;
                case Resource.Id.leftmenu_classtok_Invites:
                    break;
                case Resource.Id.leftmenu_classtok_Guide:
                    break;
                case Resource.Id.leftmenu_classtok_Folders:
                    break;
                case Resource.Id.leftmenu_classtok_Tokkepedia:
                    MainToolbar.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    tabLayout.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    m_navigationView.Menu.Clear();
                    ScrollNormalToks.Visibility = ViewStates.Visible;
                    NavigationClassToks.Visibility = ViewStates.Gone;
                    //m_navigationView.InflateMenu(Resource.Menu.left_menus);

                    GetQuoteOfTheHour(m_navigationView);

                    fragments.RemoveAt(0);
                    fragmentTitles.RemoveAt(0);
                    fragments.Insert(0, new HomeFragment());
                    fragmentTitles.Insert(0, "Home");
                    fragments.Insert(1, new SearchFragment());
                    fragmentTitles.Insert(1, "Search");
                    fragments.Insert(2, new NotificationFragment());
                    fragmentTitles.Insert(2, "Notifications");
                    fragments.Insert(3, new ProfileFragment());
                    fragmentTitles.Insert(3, "Profile");

                    adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
                    setupViewPager(MainViewPager);
                    setupHomeTabIcons();


                    //change header layout bg color for tokkepedia
                    var headerlayout = FindViewById(Resource.Id.relativeLayoutHeader);
                    headerlayout.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    break;
            }
            drawerLayout.CloseDrawers();
        }

        [Java.Interop.Export("OnClickCustomMenu")]
        public void OnClickCustomMenu(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.LinearTokGroups:
                    nextActivity = new Intent(this, typeof(TokGroupsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    this.StartActivity(nextActivity);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.LinearSet:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    this.StartActivity(nextActivity);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.LinearClassToks:
                    MainToolbar.SetBackgroundColor(Color.ParseColor("#3498db"));
                    tabLayout.SetBackgroundColor(Color.ParseColor("#3498db"));
                    ScrollNormalToks.Visibility = ViewStates.Gone;
                    NavigationClassToks.Visibility = ViewStates.Visible;

                    fragments.RemoveAt(0);
                    fragmentTitles.RemoveAt(0);
                    fragments.Insert(0, new ClassToksFragment(Settings.UserId));
                    fragmentTitles.Insert(0, "Class Toks");

                    if (fragmentTitles.Count == 4)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            fragments.RemoveAt(1);
                            fragmentTitles.RemoveAt(1);
                        }
                    }


                    adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
                    setupViewPager(MainViewPager);
                    setupClassTokTabIcons();

                    drawerLayout.CloseDrawers();


                    //change header layout bg color for classtok
                    var headerlayout = FindViewById(Resource.Id.relativeLayoutHeader);
                    headerlayout.SetBackgroundColor(Color.ParseColor("#3498db"));

                    break;
                case Resource.Id.LinearSettings:
                    nextActivity = new Intent(this, typeof(SettingsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    this.StartActivity(nextActivity);
                    drawerLayout.CloseDrawers();
                    break;
                /*case Resource.Id.LinearClassTokSettings:
                    nextActivity = new Intent(this, typeof(SettingsActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    this.StartActivity(nextActivity);
                    drawerLayout.CloseDrawers();
                    break;*/
                case Resource.Id.LinearQuoteTitle:
                    break;
                case Resource.Id.BtnMenuAddTok:
                    nextActivity = new Intent(this, typeof(AddTokActivity));
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    this.StartActivity(nextActivity);
                    break;
            }
        }

        [Java.Interop.Export("OnClickQuotePage")]
        public void OnClickQuotePage(View v)
        {
            Intent nextActivity = new Intent(this, typeof(QuoteActivity));
            this.StartActivity(nextActivity);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.ProfileTabActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);

            }
            else if ((requestCode == 40011) && (resultCode == Result.Ok)) //Badge accessed from Profile Tab
            {
                var badgeString = data.GetStringExtra("Badge");
                var badgeModel = JsonConvert.DeserializeObject<BadgeOwned>(badgeString);
                Glide.With(this).Load(badgeModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileFragment.Instance.ProfileUserPhoto);
                loadToks(badgeModel.Image);
            }
            else if (requestCode == (int)ActivityName.ActivityHome && resultCode == Result.Ok) //Filter
            {
#if (_TOKKEPEDIA)
                this.RunOnUiThread(async () => await home_fragment.HFInstance.InitAsync((FilterType)Settings.FilterTag, Settings.SortByFilter));
#endif

#if (_CLASSTOKS)
                if (Settings.MaintTabInt == (int)MainTab.Search)
                {
                    if (Settings.ClassSearchFilter == (int)FilterType.All)
                    {
                        SearchFragment.Instance.SearchText.Hint = "Search in Public Class Toks";
                    }
                    else if (Settings.ClassSearchFilter == (int)FilterType.Featured)
                    {
                        SearchFragment.Instance.SearchText.Hint = "Search in My Class Toks";
                    }
                    ClassToksFragment.Instance.isSearchFragment = true;
                }
                else
                {
                    ClassToksFragment.Instance.isSearchFragment = false;
                }


                /*fragments.RemoveAt(0);
                fragmentTitles.RemoveAt(0);
                fragments.Insert(0, new classtoks_fragment(Settings.GetUserModel().UserId));
                fragmentTitles.Insert(0, "Class Toks");

                adapter = new AdapterStateFragmentX(SupportFragmentManager, fragments, fragmentTitles);
                setupViewPager(MainViewPager);
                setupClassTokTabIcons();*/
                var viewAsChanged = data.GetBooleanExtra("viewAsChanged", false);
                var isChanged = data.GetBooleanExtra("isChanged", false);
                if (viewAsChanged)
                {
                    ClassToksFragment.Instance.setDefaultAdapter(false);
                } else
                {
                    this.RunOnUiThread(async () => await ClassToksFragment.Instance.InitializeData(isChanged));
                }
#endif
            }
            else if (requestCode == (int)ActivityName.ActivityProfile && resultCode == Result.Ok) //Filter
            {
                this.RunOnUiThread(async () => await ProfileFragment.Instance.LoadToks());
            }
            else if ((requestCode == (int)ActivityType.AvatarsActivity) && (resultCode == Result.Ok))
            {
                var avatarString = data.GetStringExtra("Avatar");
                var avatarModel = JsonConvert.DeserializeObject<Avatar>(avatarString);
                Glide.With(this).Load(avatarModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileFragment.Instance.ProfileUserPhoto);
            }
            else if ((requestCode == REQUEST_PROFILE_MEMBERSHIP) && (resultCode == Result.Ok))
            {
                long longcoins = Settings.UserCoins;
                ProfileFragment.Instance.TextProfileCoins.Text = longcoins.ToKMB();
            }
            else if ((requestCode == (int)ActivityType.HomePage) && (resultCode == Result.Ok)) {
                ClassToksFragment.Instance.ClassTokDataAdapter.NotifyDataSetChanged();
            }
        }
        public void displayImageBrowse()
        {
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                var tokketUser = Settings.GetTokketUser();
                //Refresh image
                //Save image in a cache
                if (Barrel.Current.Exists(tokketUser.Id))
                {
                    Barrel.Current.Empty(tokketUser.Id);
                }

                cacheUserPhoto = Settings.ImageBrowseCrop;
                SetCachedAsync<string>(tokketUser.Id, cacheUserPhoto, 365);

                var bitmap = BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length);
                //Resize image
                bitmapUserPhoto = SpannableHelper.scaleDown(bitmap, 300, true);
                ProfileFragment.Instance.ProfileUserPhoto.SetImageBitmap(bitmapUserPhoto);
                userphoto.SetImageBitmap(bitmapUserPhoto);

                ProfileFragment.Instance.classtokDataAdapter.NotifyDataSetChanged();
                ClassToksFragment.Instance.ClassTokDataAdapter.NotifyDataSetChanged();

                //Recreate activity due to issue that image not showing
                Finish();
                OverridePendingTransition(0, 0);
                StartActivity(this.Intent);
                OverridePendingTransition(0, 0);
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                ProfileFragment.Instance.ProfileCoverPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            }
            
            this.RunOnUiThread(async () => await SaveUserCoverPhoto(Settings.ImageBrowseCrop));
            Settings.ImageBrowseCrop = null;
        }
        public async Task SaveUserCoverPhoto(string base64img)
        {
            var saveBase64img = "data:image/jpeg;base64," + base64img;
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                var tokketUser = Settings.GetTokketUser();
                                
                var uri = await ServiceAccount.AccountService.Instance.UploadProfilePictureAsync(saveBase64img);

                //Refresh image
                if (uri)
                {
                    tokketUser = await ServiceAccount.AccountService.Instance.GetUserAsync(Settings.UserId);
                    Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                }

                //loadToks(uri);
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                await ServiceAccount.AccountService.Instance.UploadProfileCoverAsync(saveBase64img);
            }
        }
        private void Viewpager_PageSelected(object sender, AndroidX.ViewPager.Widget.ViewPager.PageSelectedEventArgs e)
        {
            //set color status bar
            this.Window.SetStatusBarColor(Color.Transparent);

            if (Settings.CurrentTheme == (int)ThemeStyle.Light)
            {
                //set color action bar
                this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(defaultPrimaryColor));
                this.tabLayout.SetBackgroundColor(defaultPrimaryColor);

            }
            else
            {
                //set color action bar
                this.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(defaultPrimaryDarkerColor));
                this.tabLayout.SetBackgroundColor(defaultPrimaryDarkerColor);
            }

            //Display appbar if hidden
            MainAppBar.SetExpanded(true, true);
            //InvalidateOptionsMenu and call OnPrepareOptionMenu to update Menu
            switch (e.Position)
            {
                case 0:
                    ViewCompat.RequestApplyInsets(MainAppBar);
                    Settings.ActivityInt = (int)ActivityType.HomePage;
                    InvalidateOptionsMenu();
                    break;
                case 1://Search
                    Settings.ActivityInt = (int)ActivityType.TokSearch;
                    InvalidateOptionsMenu();
                    break;
#if (_TOKKEPEDIA)
                    case 2: //Notification
                    InvalidateOptionsMenu();
                    break;
                case 3:
                    //When profile tab is selected
                    Settings.ActivityInt = (int)ActivityType.ProfileTabActivity;
                    InvalidateOptionsMenu();
                    break;
#endif
#if (_CLASSTOKS)
                case 2:
                    //When profile tab is selected
                    Settings.ActivityInt = (int)ActivityType.ProfileTabActivity;
                    InvalidateOptionsMenu();
                    break;
#endif
            }
        }
        private async void GetQuoteOfTheHour(NavigationView m_navigationView)
        {
            var QuoteTitle = (TextView)m_navigationView.FindViewById(Resource.Id.lblQuote);
            var QuoteAuthor = (TextView)m_navigationView.FindViewById(Resource.Id.lblQuoteAuthor);
            var QuoteCategory = (TextView)m_navigationView.FindViewById(Resource.Id.lblQuoteCategory);
            var QuoteProgress = (ProgressBar)m_navigationView.FindViewById(Resource.Id.progressQuote);

            OggClass sendItem = await ServiceAccount.CommonService.Instance.GetQuoteAsync();
            QuoteTitle.Text = sendItem.PrimaryText;
            QuoteAuthor.Text = sendItem.SecondaryText;
            QuoteCategory.Text = "Category: " + sendItem.Category;
            QuoteProgress.Visibility = ViewStates.Gone;

        }

        public void loadToks(string image = "")
        {
            showProgress();
#if (_CLASSTOKS)
            if (ProfileFragment.Instance.classtokDataAdapter != null)  //if classtokadapter used
            {
                var itemIndex = ProfileFragment.Instance.classtokDataAdapter.items.Where(x => x.UserId == tokketUser.Id).ToList();
                if (itemIndex.Count != -1)
                {
                    for (int i = 0; i < ProfileFragment.Instance.ClassTokList.Count; i++)
                    {
                        if (ProfileFragment.Instance.ClassTokList[i].UserId == tokketUser.Id)
                        {
                            ProfileFragment.Instance.ClassTokList[i].UserPhoto = image;
                        }
                    }
                    //   profile_fragment.Instance.ClassTokList[itemIndex].UserPhoto = image;
                }

                ProfileFragment.Instance.SetClassRecyclerAdapter(ProfileFragment.Instance.ClassTokList);
            }

            if (ClassToksFragment.Instance.ClassTokDataAdapter != null) //if classtokadapter used
            {
                var itemIndex = ClassToksFragment.Instance.ClassTokDataAdapter.items.Where(x => x.UserId == tokketUser.Id).ToList();
                if (itemIndex.Count != -1)
                {
                    for (int i = 0; i < ClassToksFragment.Instance.ClassTokDataAdapter.items.Count; i++)
                    {
                        if (ClassToksFragment.Instance.ClassTokDataAdapter.items[i].UserId == tokketUser.Id)
                        {
                            ClassToksFragment.Instance.ClassTokDataAdapter.items[i].UserPhoto = image;
                        }
                        // classtoks_fragment.Instance.ClassTokDataAdapter.items[itemIndex].UserPhoto = image;
                    }
                    ClassToksFragment.Instance.setDefaultAdapter();
                }
            }


            if (ProfileUserActivity.Instance != null)
            {
                if (ProfileUserActivity.Instance.classtokDataAdapter != null)  //if classtokadapter used
                {
                    var itemIndex = ProfileUserActivity.Instance.classtokDataAdapter.items.Where(x => x.UserId == tokketUser.Id).ToList();
                    if (itemIndex.Count != -1)
                    {

                        for (int i = 0; i < ProfileUserActivity.Instance.ClassTokList.Count; i++)
                        {
                            if (ProfileUserActivity.Instance.ClassTokList[i].UserId == tokketUser.Id)
                            {
                                ProfileUserActivity.Instance.ClassTokList[i].UserPhoto = image;
                            }
                        }
                        //    ProfileUserActivity.Instance.ClassTokList[itemIndex].UserPhoto = image;
                    }
                    ProfileUserActivity.Instance.SetClassRecyclerAdapter();
                }
            }

            Linearprogress.Visibility = ViewStates.Gone;
#endif
            hideProgress();
        }
        public void showProgress()
        {
            Linearprogress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
       //     ProgressBarCircle.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)), Android.Graphics.PorterDuff.Mode.Multiply);
        }
        public void hideProgress()
        {
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            Linearprogress.Visibility = ViewStates.Gone;
        }

        public async Task RefreshAccount()
        {
            TokketUser tokketUser = await Tokket.Shared.Services.AccountService.Instance.GetUserAsync(Settings.UserId);
            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
            Settings.UserCoins = tokketUser.Coins.Value;
            long longcoins = Settings.UserCoins;

            TextView TextProfileCoins = headerView.FindViewById<TextView>(Resource.Id.TextProfileCoins);
            TextProfileCoins.Text = longcoins.ToKMB();
            ProfileFragment.Instance.TextProfileCoins.Text = longcoins.ToKMB();
        }

        //UI properties
        public AndroidX.AppCompat.Widget.Toolbar MainToolbar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.main_toolbar);
        public FloatingActionButton fab_AddTok => FindViewById<FloatingActionButton>(Resource.Id.fab_AddTok);
        public CollapsingToolbarLayout CollapsingToolBarMain => FindViewById<CollapsingToolbarLayout>(Resource.Id.CollapsingToolBarMain);
        public ScrollView ScrollNormalToks => FindViewById<ScrollView>(Resource.Id.ScrollNormalToks);
        public ExpandableListView NavigationClassToks => FindViewById<ExpandableListView>(Resource.Id.navigationClassToks);
        public LinearLayout LinearCoinsToast => m_navigationView.GetHeaderView(0).FindViewById<LinearLayout>(Resource.Id.LinearCoinsToast);
        public TextView TextCoinsToast => m_navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TextCoinsToast);
        public TextView txtGroup => m_navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.lblGroup);
        public TextView txtFeedOptionType => m_navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.txtFeedOptionType);
        public ImageView BulbImg => FindViewById<ImageView>(Resource.Id.imageBulb);
        public ImageView imageViewLogoText => FindViewById<ImageView>(Resource.Id.imageViewTokkepediaLogoText);
        public AppBarLayout MainAppBar => FindViewById<AppBarLayout>(Resource.Id.appbarMain);

        public LinearLayout Linearprogress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
    }
}