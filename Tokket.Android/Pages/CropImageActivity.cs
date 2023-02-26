using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Uri = Android.Net.Uri;
using TokketAppCrop;
using Tokket.Android.Fragments;
using Android.Content.PM;
using TheArtOfDev.Edmodo.Cropper;

namespace Tokket.Android
{
    [Activity(Label = "Crop Image", NoHistory = true, Theme = "@style/Theme.AppCompat")]
    public class CropImageActivity : BaseActivity
    {
        //region: Fields and Consts

        private CropFragment _currentFragment;

        private Uri _cropImageUri;

        private CropImageViewOptions _cropImageViewOptions = new CropImageViewOptions();
        //endregion
        public void SetCurrentFragment(CropFragment fragment)
        {
            _currentFragment = fragment;
        }
        public void SetCurrentOptions(CropImageViewOptions options)
        {
            _cropImageViewOptions = options;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.crop_activity_main);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (savedInstanceState == null)
            {
                SetMainFragmentByPreset(CropDemoPreset.Rect);
            }
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _currentFragment.UpdateCurrentCropViewOptions();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.crop_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }

            if (_currentFragment != null && _currentFragment.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == CropImage.PickImageChooserRequestCode && resultCode == Result.Ok)
            {
                var imageUri = CropImage.GetPickImageResultUri(this, data);
                if (CropImage.IsReadExternalStoragePermissionsRequired(this, imageUri))
                {
                    _cropImageUri = imageUri;

                    RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, CropImage.PickImagePermissionsRequestCode);
                }
                else
                {

                    _currentFragment.setImageUri(imageUri);
                }
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    CropImage.StartPickImageActivity(this);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Long).Show();
                }
            }
            if (requestCode == CropImage.PickImagePermissionsRequestCode)
            {
                if (_cropImageUri != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    _currentFragment.setImageUri(_cropImageUri);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Long).Show();
                }
            }
        }

        private void SetMainFragmentByPreset(CropDemoPreset demoPreset)
        {
            var fragmentManager = SupportFragmentManager;
            fragmentManager.BeginTransaction()
                    .Replace(Resource.Id.cropcontainer, CropFragment.NewInstance(demoPreset))
                    .Commit();
        }
    }
}