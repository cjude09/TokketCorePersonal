using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.Widget;
using AndroidX.ViewPager2.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using NetUri = Android.Net.Uri;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SharedUrisActivity : BaseActivity
    {
        List<XFragment> fragments = new List<XFragment>();
        List<string> fragmentTitles = new List<string>();
        ViewPagerAdapter adapterFragment;
        internal static SharedUrisActivity Instance { get; private set; }
        List<string> imageBase64List;
        System.Collections.IList imageUriList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.activity_shared_uris);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Settings.ActivityInt = Convert.ToInt16(ActivityType.SharedUrisActivity);

            Instance = this;

            imageBase64List = AddClassTokActivity.Instance.GetImageBase64();

            imageUriList = AddClassTokActivity.Instance.GetImageUris();

            for (int i = 0; i < imageUriList.Count; i++)
            {
                fragmentTitles.Add("");
                fragments.Add(new SharedUrisFragment((NetUri)imageUriList[i], imageBase64List[i]));
            }

            setupViewPager();

            btnArrowLeft.Click += delegate
            {
                viewPager2CropAll.CurrentItem = viewPager2CropAll.CurrentItem - 1;
            };

            btnArrowRight.Click += delegate
            {
                viewPager2CropAll.CurrentItem = viewPager2CropAll.CurrentItem + 1;
            };
        }

        void setupViewPager()
        {
            adapterFragment = new ViewPagerAdapter(SupportFragmentManager, fragments, fragmentTitles, Lifecycle);
            viewPager2CropAll.Adapter = adapterFragment;
            viewPager2CropAll.Adapter.NotifyDataSetChanged();
            adapterFragment.NotifyDataSetChanged();

            var callback = new OnPageChangeSharedUriCallback(this);
            viewPager2CropAll.RegisterOnPageChangeCallback(callback);
            viewPager2CropAll.Orientation = ViewPager2.OrientationHorizontal;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.crop_result, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.crop_result_proceed:
                    AddClassTokActivity.Instance.SetAllSharedImages();
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void SharedUrisResultReceived(int resultCode, Intent data)
        {
            if (resultCode == (int)AppResult.Ok)
            {
                NetUri uri = data.Data;
                Settings.ImageBrowseCrop = (string)uri;
            }
        }

        public void displayImageBrowse()
        {
            imageBase64List[viewPager2CropAll.CurrentItem] = Settings.ImageBrowseCrop;
            
            var page = SupportFragmentManager.FindFragmentByTag("f" + adapterFragment.GetItemId(viewPager2CropAll.CurrentItem));
            if (page != null)
            {
                //Always update AddClassTokActivity with the new changes
                AddClassTokActivity.Instance.UpdateImageBase64List(imageBase64List);

                ((SharedUrisFragment)page).UpdateImageDisplay(imageBase64List[viewPager2CropAll.CurrentItem]);
            }
        }

        public ViewPager2 viewPager2CropAll => FindViewById<ViewPager2>(Resource.Id.viewPager2CropAll);
        public AppCompatImageButton btnArrowLeft => FindViewById<AppCompatImageButton>(Resource.Id.btnArrowLeft);
        public AppCompatImageButton btnArrowRight => FindViewById<AppCompatImageButton>(Resource.Id.btnArrowRight);
        class OnPageChangeSharedUriCallback : ViewPager2.OnPageChangeCallback
        {
            // Don't really need this as we are not using it.
            private readonly SharedUrisActivity activity;

            public OnPageChangeSharedUriCallback(SharedUrisActivity context)
            {
                this.activity = context;
            }

            public override void OnPageSelected(int position)
            {
                if (position == 0)
                {
                    activity.btnArrowLeft.Visibility = ViewStates.Invisible;
                    activity.btnArrowRight.Visibility = ViewStates.Visible;
                }
                else if (position == activity.fragments.Count - 1)
                {
                    activity.btnArrowLeft.Visibility = ViewStates.Visible;
                    activity.btnArrowRight.Visibility = ViewStates.Invisible;
                }
                else
                {
                    activity.btnArrowLeft.Visibility = ViewStates.Visible;
                    activity.btnArrowRight.Visibility = ViewStates.Visible;
                }
            }
        }
    }

    class SharedUrisActivityResultCallback : Java.Lang.Object, IActivityResultCallback
    {
        SharedUrisActivity _myActivity;
        public SharedUrisActivityResultCallback(SharedUrisActivity myActivity)
        {
            _myActivity = myActivity; //initialise the parent activity/fragment here
        }

        public void OnActivityResult(Java.Lang.Object result)
        {
            var activityResult = result as ActivityResult;
            _myActivity.SharedUrisResultReceived(activityResult.ResultCode, activityResult.Data); //pass the OnActivityResult data to parent class
        }
    }
}