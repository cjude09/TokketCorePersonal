using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Xamarin.Essentials;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android
{
    [Activity(Label = "", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MySetsViewActivity : BaseActivity
    {
        internal static MySetsViewActivity Instance { get; private set; }
        Set setList; TabLayout tabLayout; ViewPager viewpager;
        public AdapterFragmentX fragment { get; private set; }
        Intent nextActivity;
        ImageButton imgbtnSetsTokCards, imgbtnSetsTokMatch;
        Button btnMySetsViewAdd, btnMySetsViewRemove;
        bool isAddToSet;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mysetsview_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            setList = JsonConvert.DeserializeObject<Set>(Intent.GetStringExtra("setModel"));

            var lblMySetViewTitle = FindViewById<TextView>(Resource.Id.lblMySetViewTitle);
            var lblMySetViewTokGroup = FindViewById<TextView>(Resource.Id.lblMySetViewTokGroup);
            var lblMySetViewTokType = FindViewById<TextView>(Resource.Id.lblMySetViewTokType);
            var lblMySetViewDescription = FindViewById<TextView>(Resource.Id.lblMySetViewDescription);
            var lblMySetViewToksCnt = FindViewById<TextView>(Resource.Id.lblMySetViewToksCnt);
            btnMySetsViewAdd = this.FindViewById<Button>(Resource.Id.btnMySetsViewAdd);
            btnMySetsViewRemove = this.FindViewById<Button>(Resource.Id.btnMySetsViewRemove);

            imgbtnSetsTokCards = this.FindViewById<ImageButton>(Resource.Id.imgbtnSetsTokCards);
            imgbtnSetsTokMatch = this.FindViewById<ImageButton>(Resource.Id.imgbtnSetsTokMatch);

            //Add Toks
            btnMySetsViewAdd.Tag = 0;
            btnMySetsViewAdd.Click -= AddRemoveToks;
            btnMySetsViewAdd.Click += AddRemoveToks;

            //Remove Toks
            btnMySetsViewRemove.Tag = 1;
            btnMySetsViewRemove.Click -= AddRemoveToks;
            btnMySetsViewRemove.Click += AddRemoveToks;

            imgbtnSetsTokCards.Click -= OnPlayTokCards;
            imgbtnSetsTokCards.Click += OnPlayTokCards;
            imgbtnSetsTokMatch.Click -= OnPlayTokMatch;
            imgbtnSetsTokMatch.Click += OnPlayTokMatch;

            lblMySetViewTitle.Text = setList.Name;
            SupportActionBar.Title = lblMySetViewTitle.Text;
            lblMySetViewTokGroup.Text = "Tok Group: " + setList.TokGroup;
            lblMySetViewTokType.Text = "Tok Type: " + setList.TokType;
            lblMySetViewDescription.Text = setList.Description;
            lblMySetViewToksCnt.Text = setList.TokIds.Count().ToString();

            viewpager = this.FindViewById<ViewPager>(Resource.Id.viewpagerMySetsViewToks);
            setupViewPager(viewpager);
            tabLayout = this.FindViewById<TabLayout>(Resource.Id.tabLayoutMySetsViewToks);
            tabLayout.SetupWithViewPager(viewpager);
        }
        void setupViewPager(ViewPager viewPager)
        {
            Settings.FilterTag = (int)FilterType.Set;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("userid", SecureStorage.GetAsync("userid").GetAwaiter().GetResult());
            editor.PutString("setList", JsonConvert.SerializeObject(setList));
            editor.Apply();

            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();
            fragments.Add(new ToksFragment());
            fragments.Add(new MySetsTokCardsFragment());

            fragmentTitles.Add("Toks");
            fragmentTitles.Add("Tok Cards");

            var adapter = new AdapterFragmentX(this.SupportFragmentManager, fragments,fragmentTitles);
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
            fragment = adapter;
        }
        private void AddRemoveToks(object sender, EventArgs e)
        {
            isAddToSet = true;
            if ((int)(sender as Button).Tag == 1)
            {
                isAddToSet = false; // set false if button clicked is remove
            }
            Settings.ActivityInt = Convert.ToInt16(ActivityType.MySetsView);
            nextActivity = new Intent(MainActivity.Instance, typeof(MySetsActivity));
            nextActivity.PutExtra("isAddToSet", isAddToSet);
            nextActivity.PutExtra("TokTypeId", setList.TokTypeId);
            nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(setList));
            this.StartActivity(nextActivity);
        }
        private void OnPlayTokCards(object sender, EventArgs e)
        {
            nextActivity = new Intent(MainActivity.Instance, typeof(TokCardsMiniGameActivity));
            nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(setList));
            MainActivity.Instance.StartActivity(nextActivity);
        }
        private void OnPlayTokMatch(object sender, EventArgs e)
        {
            nextActivity = new Intent(MainActivity.Instance, typeof(TokMatchActivity));
            nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(setList));
            MainActivity.Instance.StartActivity(nextActivity);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    //Go back to where the Left Menu is calling from originally
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.SetFlags(ActivityFlags.ClearTop);
                    MainActivity.Instance.StartActivity(nextActivity);                    
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}