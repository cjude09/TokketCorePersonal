using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Android.Fragments;
using XFragment = AndroidX.Fragment.App.Fragment;
using Tokket.Shared.Models;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Android.Graphics;
using Android.Content.Res;

namespace Tokket.Android
{
    /*#if (_TOKKEPEDIA)
        [Activity(Label = "", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif

    #if (_CLASSTOKS)
        [Activity(Label = "", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif*/
    [Activity(Label = "", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class PatchesActivity : BaseActivity
    {
        List<PointsSymbolModel> ListPatchesColor;
        public TokketUser TokketUserCur;
        internal static PatchesActivity Instance { get; private set; }
        string UserId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.patchespage);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;

            if (!string.IsNullOrEmpty(Intent.GetStringExtra("tokketuser")))
                TokketUserCur = JsonConvert.DeserializeObject<TokketUser>(Intent.GetStringExtra("tokketuser"));
            else
                TokketUserCur = Settings.GetTokketUser();
            
            UserId = TokketUserCur.Id;

            string title = "";
            if (!string.IsNullOrEmpty(TokketUserCur.AccountType))
            {
                if (TokketUserCur.AccountType == "group")
                {
                    title = TokketUserCur.Points + " points";
                }
                else
                {
                    title = TokketUserCur.Points + " points";
                }
            }
            else
            {
                title = TokketUserCur.Points + " points";
            }
            setActivityTitle(title);
             
            setupViewPager(ViewPagerPatches);
            TabPatches.SetupWithViewPager(ViewPagerPatches);

            BtnChangePatchColor.Click += delegate
            {
                LinearPatchColor.Visibility = ViewStates.Visible;
                LinearPatchTabs.Enabled = false;
            };

            if (TokketUserCur.PointsSymbolColor != null)
            {
                TextCurrentColor.Text = "Current Patch Color: " + TokketUserCur.PointsSymbolColor.Substring(0, 1).ToUpper() + TokketUserCur.PointsSymbolColor.Substring(1, TokketUserCur.PointsSymbolColor.Length - 1);
            }
            
            ListPatchesColor = PointsSymbolsHelper.PatchesColors();
            BtnChangeColorCmd.Click += async(sender, e) =>
            {
                int position = 0;
                if (BtnChangeColorCmd == null)
                {
                    var dialog = new AlertDialog.Builder(this);
                    var alertDialog = dialog.Create();
                    alertDialog.SetTitle("");
                    alertDialog.SetMessage("No color selected!");
                    alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                    alertDialog.Show();
                    alertDialog.SetCanceledOnTouchOutside(false);
                }
                else
                {
                    try { position = (int)BtnChangeColorCmd.Tag; } catch { position = int.Parse((string)BtnChangeColorCmd.Tag); }
                    string color = ListPatchesColor[position].Name.ToLower();

                    showBlueLoading(this);
                    var result = await BadgeService.Instance.UpdateUserPointsSymbolColorAsync(ListPatchesColor[position].Name.ToLower(), UserId);
                    hideBlueLoading(this);

                    if (result)
                    {
                        var dialog = new AlertDialog.Builder(this);
                        var alertDialog = dialog.Create();
                        alertDialog.SetTitle("");
                        alertDialog.SetMessage("Color changed successfully!");
                        alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => 
                        {
                            TokketUserCur.PointsSymbolColor = color;
                            Settings.TokketUser = JsonConvert.SerializeObject(TokketUserCur);
                            LinearPatchTabs.Enabled = true;
                            LinearPatchColor.Visibility = ViewStates.Gone;
                            if (Settings.ActivityInt == (int)ActivityType.ProfileActivity)
                            {
                                ProfileUserActivity.Instance.ShowCurrentRank();
                            }
                            else if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
                            {
                                ProfileFragment.Instance.ShowCurrentRank();
                                ProfileFragment.Instance.classtokDataAdapter.NotifyDataSetChanged();
                            }
                            ClassToksFragment.Instance.ClassTokDataAdapter.NotifyDataSetChanged();
                            this.Finish();
                        });
                        alertDialog.Show();
                        alertDialog.SetCanceledOnTouchOutside(false);
                    }
                    else
                    {
                        ShowLottieMessageDialog(this, "Failed to change color!", false);
                    }
                }
            };
            if (TokketUserCur.Id != Settings.GetTokketUser().Id) {
                BtnChangePatchColor.Visibility = ViewStates.Gone;
            }
            BtnChangeColorCmd.Enabled = false;
            BtnChangeColorCmd.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));
            LoadData();
        }

        public void EnableChangeColorButton(bool pressed = false) {
            if(pressed)
                BtnChangeColorCmd.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));
            else
                BtnChangeColorCmd.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));
        }
        private void LoadData()
        {
            RecyclerColorPatches.SetLayoutManager(new GridLayoutManager(this, 1));
            var adapter = new PatchesAdapter(ListPatchesColor, PatchesTab.PatchColor, null);
            RecyclerColorPatches.SetAdapter(adapter);
        }
        void setupViewPager(ViewPager viewPager)
        {
            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();

            fragments.Add(new PatchesFragment("My Patches", TokketUserCur));
            fragments.Add(new PatchesFragment("Level Table", TokketUserCur));
            if(Settings.GetTokketUser().Id == TokketUserCur.Id)
                 fragments.Add(new PatchesSettingsFragment("Settings"));

            fragmentTitles.Add("My Patches");
            fragmentTitles.Add("Level Table");
            if (Settings.GetTokketUser().Id == TokketUserCur.Id)
                fragmentTitles.Add("Settings");

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            ViewPagerPatches.Adapter.NotifyDataSetChanged();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    if (LinearPatchColor.Visibility == ViewStates.Visible)
                    {
                        LinearPatchTabs.Enabled = true;
                        LinearPatchColor.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        Finish();
                    }
                    
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void OnBackPressed()
        {
            if (LinearPatchColor.Visibility == ViewStates.Visible)
            {
                LinearPatchTabs.Enabled = true;
                LinearPatchColor.Visibility = ViewStates.Gone;
            }
            else
            {
                base.OnBackPressed();
            }
        }
        public Button BtnChangePatchColor => FindViewById<Button>(Resource.Id.BtnChangePatchColor);
        public TabLayout TabPatches => FindViewById<TabLayout>(Resource.Id.tabLayoutPatches);
        public ViewPager ViewPagerPatches => FindViewById<ViewPager>(Resource.Id.viewpagerPatches);
        public LinearLayout LinearPatchTabs => FindViewById<LinearLayout>(Resource.Id.LinearPatchTabs);
        public LinearLayout LinearPatchColor => FindViewById<LinearLayout>(Resource.Id.LinearPatchColor);
        public TextView TextCurrentColor => FindViewById<TextView>(Resource.Id.TextCurrentColor);
        public RecyclerView RecyclerColorPatches => FindViewById<RecyclerView>(Resource.Id.RecyclerColorPatches);
        public Button BtnChangeColorCmd => FindViewById<Button>(Resource.Id.BtnChangeColorCmd);
    }
}