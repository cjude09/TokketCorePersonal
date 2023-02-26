using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core.Tools;
using static Tokket.Android.ProfileUserActivity;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Tok Groups", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Tok Groups", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class TokGroupsActivity : BaseActivity
    {
        public AdapterFragmentX fragment { get; private set; }
        public ViewPager viewpager; TabLayout tabLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.toktgroups_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            viewpager = FindViewById<ViewPager>(Resource.Id.viewpagerTokTypesGroup);
            setupViewPager(viewpager);
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayoutTokTypesGroup);
            tabLayout.SetupWithViewPager(viewpager);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.info_tokgroup, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.btnMenuInfo:
                    string message = "Tok Types organize Tokkepedia information. They are structured to easily review knowledge that is shared and also to make the toks playable in our Tokket games.";
                    Toast toast = Toast.MakeText(ApplicationContext, message, ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, -200);
                    toast.Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        void setupViewPager(ViewPager viewPager)
        {
            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();

            fragments.Add(new TokTypeBasicFragment());
            fragments.Add(new TokTypeDetailedFragment());
            fragments.Add(new TokTypeMegaFragment());

            fragmentTitles.Add("BASIC");
            fragmentTitles.Add("DETAILED");
            fragmentTitles.Add("MEGA");

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments,fragmentTitles);
            //adapter.AddFragment(new toktypebasic_fragment(), "BASIC");
            //adapter.AddFragment(new toktypedetailed_fragment(), "DETAILED");
            //adapter.AddFragment(new toktypemega_fragment(), "MEGA");
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
            fragment = adapter;
        }
    }
}