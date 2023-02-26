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
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Android.ViewModels;
using Tokket.Core;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android
{
    [Activity(Label = "My Sets", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MySetsActivity : BaseActivity
    {
        internal static MySetsActivity Instance { get; private set; }
        public ViewPager viewpager; public TabLayout tabLayout;
        public MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        string toktypeid = "", tabheader = "Tok Sets",UserId = "";
        TextView txtMySetsPageTitle; Set setList; bool isAddToSet; //0 is Add to set
        TokModel tokModel;
        public TextView txtTotalToksSelected;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.mysets_page);

            viewpager = this.FindViewById<ViewPager>(Resource.Id.viewpagerMySets);

            txtMySetsPageTitle = this.FindViewById<TextView>(Resource.Id.txtMySetsPageTitle);
            tabLayout = this.FindViewById<TabLayout>(Resource.Id.tabMySets);
            txtTotalToksSelected = this.FindViewById<TextView>(Resource.Id.txtTotalToksSelected);

            Instance = this;
            MySetsVm.Instance = Instance;

            isAddToSet = Intent.GetBooleanExtra("isAddToSet",true);
            MySetsVm.IsAddToksToSet = isAddToSet;
            toktypeid = Intent.GetStringExtra("TokTypeId").ToString();
            MySetsVm.TokTypeID = toktypeid;

            UserId = Intent.GetStringExtra("userId");

            MySetsVm.LinearProgress = LinearProgress;
            MySetsVm.ProgressCircle = ProgressCircle;
            MySetsVm.ProgressText = ProgressText;

            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
            {
                //If opened from MySetsViewActivity
                AddSet.Click += async (s, e) =>
                {
                    if (isAddToSet)
                    {
                        await MySetsVm.AddSet();
                    }
                    else
                    {
                        await MySetsVm.RemoveToksFromSet();
                    }
                };

                if (isAddToSet)
                {
                    AddSet.Text = "Add to Set";
                }
                else
                {
                    AddSet.Text = "Remove from Set";
                }

                tabheader = "Select a Set:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                setList = JsonConvert.DeserializeObject<Set>(Intent.GetStringExtra("setModel"));
                MySetsVm.SetModel = setList;
                txtMySetsPageTitle.Text = setList.Name;
            }
            else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets))
            {
                if (viewpager.CurrentItem == 0)
                {
                    AddSet.Click += async (s, e) =>
                    {
                        await MySetsVm.AddSet();
                    };
                }
                else if(viewpager.CurrentItem == 1)
                {

                }
            }
            else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo))
            {
                tokModel = JsonConvert.DeserializeObject<TokModel>(Intent.GetStringExtra("tokModel"));
                MySetsVm.tokList = tokModel;
                txtMySetsPageTitle.Text = tokModel.PrimaryFieldText;
                tabheader = "Select a set:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                AddSet.Text = "Add to Set";
                AddSet.Click += async (s, e) =>
                {
                    await MySetsVm.AddSet();
                };
            }
            setupViewPager(viewpager);
            tabLayout.SetupWithViewPager(viewpager);
            CancelSet.Click += delegate
            {
                MySetsVm.CancelSet();
            };

            viewpager.PageSelected += Viewpager_PageSelected;
        }
        void setupViewPager(ViewPager viewPager)
        {
            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();


            if (UserId == Settings.GetTokketUser().Id)
                tabheader = "My Class Sets";
            else
                tabheader = "Class Sets";

            fragments.Add(new MyClassTokSetsFragment("",UserId));//mytoksets_fragment
            fragmentTitles.Add(tabheader);

            

            if (tabheader.ToLower() == "tok sets")
            {
                fragments.Add(new GameSetsFragment());
                fragmentTitles.Add("Tokket Game Sets");
            }            

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments,fragmentTitles);
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
        }
        private void Viewpager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    AddSet.Text = "+ Add Set";
                    AddSet.Visibility = ViewStates.Visible;
                    break;
                case 1:
                    AddSet.Text = "+ Add Game Set";
                    TextNoSetsInfo.Visibility = ViewStates.Gone;
                    AddSet.Visibility = ViewStates.Gone;
                    break;
            }
        }

        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            MySetsVm.PopUpMenuClick(v);
        }
        public TextView AddSet => FindViewById<TextView>(Resource.Id.btnMySetsAdd);
        public TextView CancelSet => FindViewById<TextView>(Resource.Id.btnMySetsCancel);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_mysetsprogress);
        public ProgressBar ProgressCircle => FindViewById<ProgressBar>(Resource.Id.progressbarMySets);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextMySets); 
        public TextView TextNoSetsInfo => FindViewById<TextView>(Resource.Id.txtMySetsPageNoSets);
    }
}