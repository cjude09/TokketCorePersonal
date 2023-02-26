using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using static Google.Android.Material.Tabs.TabLayoutMediator;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Fragments
{
    public class SetsSearchFragment : AndroidX.Fragment.App.Fragment
    {
        internal static SetsSearchFragment Instance { get; private set; }
        View v;
        string UserId, groupId;
        public bool isPublicSets { get; set; }
        List<XFragment> fragments = new List<XFragment>();
        public List<string> fragmentTitles = new List<string>();
        public int selectedPosition { get; set; }
        public bool isSuperSets { get; set; }
        ViewPagerAdapter adapterFragment;
        public SetsSearchFragment(string _groupId, string _userId = "", bool ispublicsets = false, bool issupersets = false)
        {
            this.groupId = _groupId;
            this.UserId = _userId;
            this.isPublicSets = ispublicsets;
            this.isSuperSets = issupersets;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.fragment_search_sets, container, false);
            Instance = this;
            Initialize();

            btnTab1.Click += delegate
            {
                tabSearchSet.GetTabAt(0).Select();
            };

            btnTab2.Click += delegate
            {
                tabSearchSet.GetTabAt(1).Select();
            };

            return v;
        }

        private void Initialize()
        {
            fragments = new List<XFragment>();
            fragmentTitles = new List<string>();

            fragments.Add(new MyClassTokSetsFragment("", ispublicsets: isPublicSets, issupersets: isSuperSets) {
                searchText = MyClassSetsActivity.Instance.txtSearch.Text, searchType = "name"
            });
            fragments.Add(new MyClassTokSetsFragment("", ispublicsets: false, issupersets: isSuperSets) {
                searchText = MyClassSetsActivity.Instance.txtSearch.Text,
                searchType = "category"
            });

            fragmentTitles.Add("Sets Name");
            fragmentTitles.Add("Category");

            btnTab1.Text = "Sets Name";
            btnTab2.Text = "Category";

            setupViewPager();

            TabLayoutMediator tabMediator = new TabLayoutMediator(tabSearchSet, viewpagerSearchSets, new TabConfigurationStrategy(Activity));
            tabMediator.Attach();
            //tabSearchSet.SetupWithViewPager(viewpagerSearchSets);
            tabSearchSet.GetTabAt(selectedPosition).Select();
        }
        void setupViewPager()
        {
            adapterFragment = new ViewPagerAdapter(ChildFragmentManager, fragments, fragmentTitles, ViewLifecycleOwner.Lifecycle);
            viewpagerSearchSets.Adapter = adapterFragment;
            viewpagerSearchSets.Adapter.NotifyDataSetChanged();
            adapterFragment.NotifyDataSetChanged();

            var callback = new OnPageChangeCallback(Activity);
            viewpagerSearchSets.RegisterOnPageChangeCallback(callback);
            viewpagerSearchSets.Orientation = ViewPager2.OrientationHorizontal;
        }

        public TabLayout tabSearchSet => v.FindViewById<TabLayout>(Resource.Id.tabSearchSet);
        public ViewPager2 viewpagerSearchSets => v.FindViewById<ViewPager2>(Resource.Id.viewpagerSearchSets);
        public Button btnTab1 => v.FindViewById<Button>(Resource.Id.btnTab1);
        public Button btnTab2 => v.FindViewById<Button>(Resource.Id.btnTab2);
    }
    class TabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
    {
        // using this for the tab titles
        private readonly Context context;

        public TabConfigurationStrategy(Context context)
        {
            this.context = context;
        }

        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(SetsSearchFragment.Instance.fragmentTitles[position]);
        }
    }

    class OnPageChangeCallback : ViewPager2.OnPageChangeCallback
    {
        // Don't really need this as we are not using it.
        private readonly Context context;

        public OnPageChangeCallback(Context context)
        {
            this.context = context;
        }

        public override void OnPageSelected(int position)
        {
            SetsSearchFragment.Instance.selectedPosition = position;
        }
    }
}