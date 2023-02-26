using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Tokket.Shared.Models;
using Newtonsoft.Json;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.Tabs;
using AndroidX.ViewPager.Widget;
using XFragment = AndroidX.Fragment.App.Fragment;
using Tokket.Android.Fragments;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "SuperSet Info", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SuperSetInfoActivity : BaseActivity
    {

        ClassSetModel ClassSetModel;
        internal static SuperSetInfoActivity Instance;
        const int ADD_SET_TO_SUPERSET = 10001, REMOVE_SET_FROM_SUPERSET = 10002, REQUEST_ADD_CLASS_SET = 10003;
        Intent nextActvity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.supersetinfo_page);

            SetSupportActionBar(toolBar);
            setActivityTitle("Super Set");
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            ClassSetModel = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("supersetModel"));
            
            if (ClassSetModel != null)
            {
                loadSetInfo();
                InitializeSuperSet();
            }
            else {
              
            }

            BtnAddToSuperSet.Click += delegate
            {
                nextActvity = new Intent(this,typeof(AddRemoveSuperSetActivity));
                nextActvity.PutExtra("isAddSets", true);
                nextActvity.PutExtra("supersetModel",Intent.GetStringExtra("supersetModel"));
                StartActivityForResult(nextActvity,ADD_SET_TO_SUPERSET);
            };

            BtnRemoveFromSuperSet.Click += delegate {
                nextActvity = new Intent(this, typeof(AddRemoveSuperSetActivity));
                nextActvity.PutExtra("supersetModel", Intent.GetStringExtra("supersetModel"));
                nextActvity.PutExtra("isAddSets", false);
                StartActivityForResult(nextActvity, REMOVE_SET_FROM_SUPERSET);
            };
            // Create your application here
        }

        private void loadSetInfo()
        {
            if (!string.IsNullOrEmpty(ClassSetModel.Image))
            {
                Glide.With(this).Load(ClassSetModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Animation.loader_animation).FitCenter()).Into(SuperSetImage);
            }

            LblSetName.Text = ClassSetModel.Name;
            LblSetCounts.Text = ClassSetModel.Sets?.Count.ToString();
        }
        void setupViewPager(ViewPager viewPager, string title = "MY CLASS TOKS SETS")
        {
            List<XFragment> fragments = new List<XFragment>();
            List<string> fragmentTitles = new List<string>();

            fragments.Add(new MyClassTokSetsFragment("", ispublicsets: false) { SuperSetSelected = ClassSetModel });
          //  fragments.Add(new myclasstoksets_fragment("", ispublicsets: false, issupersets: true));//Supersets
            //fragments.Add(new studysets_fragment(""));
            //fragments.Add(new convertible_fragment(""));

            fragmentTitles.Add(title);
          //  fragmentTitles.Add("Supersets");
            //fragmentTitles.Add("Study Sets");
            //fragmentTitles.Add("Convertible");

            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
            //viewpager.PageSelected += (obj, e) => {
            //    if (e.Position == 0)
            //    {
            //        BtnAddClassSet.Text = "+ Add Class Set";
            //    }
            //    else if (e.Position == 1)
            //    {
            //        BtnAddClassSet.Text = "+ Add Super Set";
            //    }
            //};


        }

        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }

            AndroidX.AppCompat.Widget.PopupMenu menu = new AndroidX.AppCompat.Widget.PopupMenu(this, v);
            menu.Inflate(Resource.Menu.myclasssets_popmenu);
            var itemViewTokInfo = menu.Menu.FindItem(Resource.Id.itemViewTokInfo);
            var itemAddClassSetToGroup = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
            var itemViewClassToks = menu.Menu.FindItem(Resource.Id.viewClassToks);
            var itemReplicate = menu.Menu.FindItem(Resource.Id.itemReplicate);
            var itemEdit = menu.Menu.FindItem(Resource.Id.itemEdit);
            var itemDelete = menu.Menu.FindItem(Resource.Id.itemDelete);
            var itemPlayTokCards = menu.Menu.FindItem(Resource.Id.itemPlayTokCards);
            var itemPlayTokChoice = menu.Menu.FindItem(Resource.Id.itemPlayTokChoice);
            var itemTokMatch = menu.Menu.FindItem(Resource.Id.itemTokMatch);
            var itemReport = menu.Menu.FindItem(Resource.Id.itemReport);
            var itemManageClassToks = menu.Menu.FindItem(Resource.Id.itemManageClassSetToGroup);
            itemReport.SetVisible(false);

            itemViewClassToks.SetVisible(true);
            itemViewTokInfo.SetVisible(false);
            itemAddClassSetToGroup.SetVisible(false);
            itemReplicate.SetVisible(false);
            itemEdit.SetVisible(false);
            itemDelete.SetVisible(false);
            itemPlayTokCards.SetVisible(false);
            itemPlayTokChoice.SetVisible(false);
            itemTokMatch.SetVisible(false);
            itemManageClassToks.SetVisible(false);

            var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);

            // A menu item was clicked:
            menu.MenuItemClick += (s1, arg1) =>
            {
                Intent nextActivity; string modelConvert;
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "view class toks":
                        nextActivity = new Intent(this, typeof(SuperSetInfoActivity));
                        modelConvert = JsonConvert.SerializeObject((page as MyClassTokSetsFragment).ListClassTokSets[position]);
                        nextActivity.PutExtra("supersetModel", modelConvert);
                        MainActivity.Instance.StartActivity(nextActivity);
                        break;
                }
            };

            // Menu was dismissed:
            menu.DismissEvent += (s2, arg2) =>
            {
                //Console.WriteLine("menu dismissed");
            };
            menu.Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //...

                //Handle sandwich menu icon click
                case HomeId:
                    //If menu is open, close it. Else, open it.
                    Finish();
                    break;
                case Resource.Id.itemEdit:
                    var nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
                    var modelConvert = JsonConvert.SerializeObject(ClassSetModel);
                    nextActivity.PutExtra("ClassTokSetsModel", modelConvert);
                    nextActivity.PutExtra("isSave", false);
                    this.StartActivityForResult(nextActivity, REQUEST_ADD_CLASS_SET);

                    break;
                case Resource.Id.itemDelete:
                    RunOnUiThread(async () => {
                        showBlueLoading(this);

                        bool? result = null;
                        result = await ClassService.Instance.DeleteClassSetAsync(ClassSetModel.Id, ClassSetModel.PartitionKey);

                        hideBlueLoading(this);

                        if (result != null)
                        {
                            MyClassSetsActivity.Instance.tokFragmentPassItemClassSetsFromAddClassSet(ClassSetModel, false, true);
                            this.Finish();
                        }
                    });
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.superset_info_menu, menu);

            var itemEdit = menu.FindItem(Resource.Id.itemEdit);
            var itemDelete = menu.FindItem(Resource.Id.itemDelete);
            var itemReport = menu.FindItem(Resource.Id.itemReport);

            if (ClassSetModel.UserId == Settings.GetTokketUser().Id)
            {
                itemEdit.SetVisible(true);
                itemDelete.SetVisible(true);
                itemReport.SetVisible(false);
            }
            else
            {
                itemEdit.SetVisible(false);
                itemDelete.SetVisible(false);
                itemReport.SetVisible(true);
            }

            return true;
        }

        private void InitializeSuperSet() {
            setupViewPager(viewpager, "Class Sets");
            tabLayout.SetupWithViewPager(viewpager);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ADD_SET_TO_SUPERSET && resultCode == Result.Ok)
            {
                var modelData = data.GetStringExtra("classSetData");
                var modelUpdae = data.GetStringExtra("updateSuperSet");
                var setList = JsonConvert.DeserializeObject<List<ClassSetModel>>(modelData);
                var update = JsonConvert.DeserializeObject<ClassSetModel>(modelUpdae);
                MyClassTokSetsFragment.Instance.UpdateSuperSetInfoItem(setList, true);
                LblSetCounts.Text = update.Sets?.Count.ToString();

            }
            else if (requestCode == REMOVE_SET_FROM_SUPERSET && resultCode == Result.Ok)
            {
                var modelData = data.GetStringExtra("classSetData");
                var setList = JsonConvert.DeserializeObject<List<ClassSetModel>>(modelData);
                var modelUpdae = data.GetStringExtra("updateSuperSet");
                var update = JsonConvert.DeserializeObject<ClassSetModel>(modelUpdae);
                MyClassTokSetsFragment.Instance.UpdateSuperSetInfoItem(setList, false);
                LblSetCounts.Text = update.Sets?.Count.ToString();
            }
            else if ((requestCode == REQUEST_ADD_CLASS_SET) && (resultCode == Result.Ok))
            {
                var dataValue = data.GetStringExtra("classsetModel");
                if (dataValue != null)
                {
                    ClassSetModel = JsonConvert.DeserializeObject<ClassSetModel>(dataValue);
                    loadSetInfo();

                    MyClassSetsActivity.Instance.tokFragmentPassItemClassSetsFromAddClassSet(ClassSetModel, false, false);
                }
            }
        }

        public ImageView SuperSetImage => FindViewById<ImageView>(Resource.Id.img_superset);

        public TextView LblSetName => FindViewById<TextView>(Resource.Id.lblSuperSetName);

        public TextView LblSetCounts => FindViewById<TextView>(Resource.Id.lblTotalSetsCount);

        public Button BtnAddToSuperSet => FindViewById<Button>(Resource.Id.btn_addtosuperset);

        public Button BtnRemoveFromSuperSet => FindViewById<Button>(Resource.Id.btn_removefromSuperset);

        // public RecyclerView SuperSetRecycler => FindViewById<RecyclerView>(Resource.Id.recycler_Supersets);
        public TabLayout tabLayout => this.FindViewById<TabLayout>(Resource.Id.tabMySets);
        //public TextView TextNoSetsInfo => FindViewById<TextView>(Resource.Id.txtMySetsPageNoSets);
        public ViewPager viewpager => this.FindViewById<ViewPager>(Resource.Id.viewpagerMySets);
    }
}