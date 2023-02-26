using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Tokket.Shared.ViewModels;
using Tokket.Shared.Extensions;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using AndroidX.ViewPager.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Tabs;
using AndroidX.Preference;
using Android.Content.PM;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Reaction Values", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Reaction Values", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class ReactionValuesActivity : BaseActivity
    {
        internal static ReactionValuesActivity Instance { get; private set; }
        TabLayout tabLayout; ViewPager viewpager;
        public AdapterFragmentX fragment { get; private set; }
        ReactionValueViewModel reactionValueVM; string tokId;
        bool isDetailed = false; int DetailNumber = -1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            Settings.ActivityInt = Convert.ToInt16(ActivityType.ReactionValuesActivity);
            SetContentView(Resource.Layout.reactionvalues_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            reactionValueVM = JsonConvert.DeserializeObject<ReactionValueViewModel>(Intent.GetStringExtra("reactionValueVM"));

            tokId = Intent.GetStringExtra("tokId");

            try
            {
                isDetailed =bool.Parse(Intent.GetStringExtra("isDetailed"));
                DetailNumber = int.Parse(Intent.GetStringExtra("detailNumber"));
            }
            catch (Exception ex) { 
            
            }

            viewpager = FindViewById<ViewPager>(Resource.Id.viewpagerReactionValues);
            setupViewPager(viewpager);
            tabLayout = this.FindViewById<TabLayout>(Resource.Id.tabLayoutReactionValues);
            tabLayout.SetupWithViewPager(viewpager);
            tabLayout.TabMode = TabLayout.ModeScrollable;
            setupTabIcons();

            viewpager.PageSelected -= Viewpager_PageSelected;
            viewpager.PageSelected += Viewpager_PageSelected;
        }

        private void Viewpager_PageSelected(object sender, AndroidX.ViewPager.Widget.ViewPager.PageSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    this.Title = "Reaction Values";
                    break;
                case 1:
                    this.Title = "Valuable";
                    break;
                case 2:
                    this.Title = "Brilliant";
                    break;
                case 3:
                    this.Title = "Precious";
                    break;
                case 4:
                    this.Title = "Accurate";
                    break;
                case 5:
                    this.Title = "Inaccurate";
                    break;
            }
        }

        private void setupTabIcons()
        {
            //tabLayout.GetTabAt(0).SetIcon(Resource.Drawable.ic_home24_dp);
            if (!isDetailed)
            {
                tabLayout.GetTabAt(1).SetIcon(Resource.Drawable.baseline_favorite_black_24);
                tabLayout.GetTabAt(2).SetIcon(Resource.Drawable.hundred_24px);
                tabLayout.GetTabAt(3).SetIcon(Resource.Drawable.purple_gem);
                tabLayout.GetTabAt(4).SetIcon(Resource.Drawable.check_black_48dp);
                tabLayout.GetTabAt(5).SetIcon(Resource.Drawable.clear_black_48dp);
            }
            else {
                tabLayout.GetTabAt(1).SetIcon(Resource.Drawable.check_black_48dp);
                tabLayout.GetTabAt(2).SetIcon(Resource.Drawable.clear_black_48dp);
            }
          
        }
        void setupViewPager(ViewPager viewPager)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("reactionValueVM", JsonConvert.SerializeObject(reactionValueVM));
            editor.PutString("tokId", tokId);
            editor.PutString("detailNumber",DetailNumber.ToString());
            editor.PutString("isDetailed", isDetailed.ToString());
            editor.Apply();

            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();
            if (!isDetailed)
            {
                fragments.Add(new ReactionValuesUserFragment("All"));
                fragments.Add(new ReactionValuesUserFragment("GemA"));
                fragments.Add(new ReactionValuesUserFragment("GemB"));
                fragments.Add(new ReactionValuesUserFragment("GemC"));
                fragments.Add(new ReactionValuesUserFragment("Accurate"));
                fragments.Add(new ReactionValuesUserFragment("Inaccurate"));

                if (reactionValueVM == null)
                {
                    fragmentTitles.Add("All ");
                    fragmentTitles.Add("0");
                    fragmentTitles.Add("0");
                    fragmentTitles.Add("0");
                    fragmentTitles.Add("0");
                    fragmentTitles.Add("0");
                }
                else
                {
                    fragmentTitles.Add("All " + reactionValueVM.All.ToKMB());
                    fragmentTitles.Add(reactionValueVM.GemA.ToKMB());
                    fragmentTitles.Add(reactionValueVM.GemB.ToKMB());
                    fragmentTitles.Add(reactionValueVM.GemC.ToKMB());
                    fragmentTitles.Add(reactionValueVM.Accurate.ToKMB());
                    fragmentTitles.Add(reactionValueVM.Inaccurate.ToKMB());
                }

            }
            else {
                fragments.Add(new ReactionValuesUserFragment("All"));
                fragments.Add(new ReactionValuesUserFragment("Accurate"));
                fragments.Add(new ReactionValuesUserFragment("Inaccurate"));


                if (reactionValueVM == null)
                {
                    fragmentTitles.Add("All ");
                    fragmentTitles.Add("0");
                    fragmentTitles.Add("0");
                }
                else
                {
                    fragmentTitles.Add("All " + reactionValueVM.All.ToKMB());
                    fragmentTitles.Add(reactionValueVM.Accurate.ToKMB());
                    fragmentTitles.Add(reactionValueVM.Inaccurate.ToKMB());
                }
            }

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments,fragmentTitles);
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
            fragment = adapter;
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
    }
}