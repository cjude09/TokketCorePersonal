using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Core;
using SharedAccount = Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using AndroidX.ViewPager.Widget;
using Android.Widget;
using Google.Android.Material.Tabs;
using Android.Content.PM;
using Android.Runtime;
using Android.Content;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Title", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Title", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class ProfileTitleActivity : BaseActivity
    {
        internal static ProfileTitleActivity Instance { get; private set; }
        ViewPager ViewPagerTitles; List<TokketTitle> ListMyTitles, ListUniqueTitles, ListGenericTitles, ListRoyalTitles;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.profiletitle_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;

            ListMyTitles = new List<TokketTitle>();
            ListUniqueTitles = new List<TokketTitle>();
            ListGenericTitles = new List<TokketTitle>();
            ListRoyalTitles = new List<TokketTitle>();

            TextCurrentTitle.Text = Settings.GetTokketUser().TitleId;
            TextTitleType.Text = "Unique";
 
            this.RunOnUiThread(async () => await InitializeTitle());

            BtnSelect.Click += async (sender, e) =>
            {
                if (BtnSelect != null)
                {
                    TokketTitle tokketTitleSelected = null;
                    int position = 0;

                    txtProgress.Text = "Changing Title...";
                    LinearProgress.Visibility = ViewStates.Visible;
                    this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                    try { position = (int)BtnSelect.Tag; } catch { position = int.Parse((string)BtnSelect.Tag); }

                    switch (TabLayoutTitle.SelectedTabPosition)
                    {
                        case 0:
                            tokketTitleSelected = ListUniqueTitles[position];
                            break;
                        case 1:
                            tokketTitleSelected = ListGenericTitles[position];
                            break;
                        case 2:
                            tokketTitleSelected = ListRoyalTitles[position];
                            break;
                    }


                    var result = await SharedAccount.AccountService.Instance.SelectTitleAsync(tokketTitleSelected.Id);

                    this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                    LinearProgress.Visibility = ViewStates.Gone;

                    string message = "";
                    if (result)
                    {
                        if (Settings.GetTokketUser().AccountType == "individual")
                        {
                            MainActivity.Instance.txtGroup.Text = tokketTitleSelected.Id;
                            ProfileFragment.Instance.UserTitle.Text = tokketTitleSelected.Id;
                            if (Settings.ActivityInt == (int)ActivityType.ProfileActivity)
                            {
                                ProfileUserActivity.Instance.UserTitle.Text = tokketTitleSelected.Id;
                            }
                        }

                        message = "Successfully selected a title.";
                    }
                    else
                    {
                        message = "Failed to select a title.";
                    }

                    var dialognetwork = new AlertDialog.Builder(this);
                    var alertnetwork = dialognetwork.Create();
                    alertnetwork.SetTitle("");
                    alertnetwork.SetMessage(message);
                    alertnetwork.SetButton("OK", (d, fv) => { });
                    alertnetwork.Show();
                    alertnetwork.SetCanceledOnTouchOutside(false);

                }
            };
        }
        private async Task InitializeTitle()
        {
            txtProgress.Text = "Loading...";
            LinearProgress.Visibility = ViewStates.Visible;

            var resUniqueTitles = await SharedAccount.AccountService.Instance.GetTitlesAsync(Settings.GetUserModel().UserId,"unique");

            try {
                if (resUniqueTitles.Results != null)
                {
                    foreach (var item in resUniqueTitles.Results.ToList())
                    {
                        if (!string.IsNullOrEmpty(item.TitleDisplay))
                        {
                            ListUniqueTitles.Add(item);
                        }
                    }
                }
            } catch (Exception ex) { 
            
            }
         

            var resGenericTitles = await SharedAccount.AccountService.Instance.GetTitlesAsync(Settings.GetUserModel().UserId, "generic");
            try {
                if (resGenericTitles.Results != null)
                {
                    foreach (var item in resGenericTitles.Results.ToList())
                    {
                        if (!string.IsNullOrEmpty(item.TitleDisplay))
                        {
                            ListGenericTitles.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
           
           

            var resRoyaltyTitles = await SharedAccount.AccountService.Instance.GetTitlesAsync(Settings.GetUserModel().UserId, "royalty");
            try {
                if (resRoyaltyTitles.Results != null)
                {
                    foreach (var item in resRoyaltyTitles.Results.ToList())
                    {
                        if (!string.IsNullOrEmpty(item.TitleDisplay))
                        {
                            ListRoyalTitles.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

         
           

            LinearProgress.Visibility = ViewStates.Gone;

            ViewPagerTitles = FindViewById<ViewPager>(Resource.Id.ViewPagerTitles);
            setupViewPager(ViewPagerTitles);
            TabLayoutTitle.SetupWithViewPager(ViewPagerTitles);
        }
        void setupViewPager(ViewPager viewPager)
        {
            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();
            fragments.Add(new ProfileTitleFragment("My Title", ListUniqueTitles));
            fragments.Add(new ProfileTitleFragment("Unique Title", ListUniqueTitles));
            fragments.Add(new ProfileTitleFragment("Generic Title", ListGenericTitles));
            fragments.Add(new ProfileTitleFragment("Royal Title", ListRoyalTitles));

            fragmentTitles.Add("My Title");
            fragmentTitles.Add("Unique Title");
            fragmentTitles.Add("Generic Title");
            fragmentTitles.Add("Royal Title");

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            ViewPagerTitles.Adapter.NotifyDataSetChanged();
        }

      

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public TextView TextTitleSelected => FindViewById<TextView>(Resource.Id.TextTitleSelected);
        public TextView TextCurrentTitle => FindViewById<TextView>(Resource.Id.TextCurrentTitle);
        public TextView TextTitleType => FindViewById<TextView>(Resource.Id.TextTitleType);
        public Button BtnSelect => FindViewById<Button>(Resource.Id.btnSelectTitle);
        public TabLayout TabLayoutTitle => FindViewById<TabLayout>(Resource.Id.TabLayoutTitle);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView txtProgress => FindViewById<TextView>(Resource.Id.txtProgress);
    }
}