using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Helpers;
using NetUri = Android.Net.Uri;

namespace Tokket.Android.Fragments
{
    public class SharedUrisFragment : AndroidX.Fragment.App.Fragment
    {
        private SharedUrisActivityResultCallback _activityResultCallback;
        private ActivityResultLauncher _activityResultLauncher;

        View v;
        NetUri imageUri;
        string imageBase64;
        public SharedUrisFragment(NetUri imageUri, string imageBase64)
        {
            this.imageUri = imageUri;
            this.imageBase64 = imageBase64;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.shared_uris_fragment, container, false);

            _activityResultCallback = new SharedUrisActivityResultCallback(SharedUrisActivity.Instance);
            _activityResultLauncher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), _activityResultCallback);

            UpdateImageDisplay(imageBase64);

            btnCrop.Click += delegate
            {
                Settings.ImageBrowseCrop = (string)imageUri;
                OpenCropActivity();
            };

            return v;
        }

        public void UpdateImageDisplay(string newImage)
        {
            this.imageBase64 = newImage;
            byte[] imageByte = Convert.FromBase64String(newImage);
            imageView.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
        }
        private void OpenCropActivity()
        {
            Intent nextActivity = new Intent(RequireContext(), typeof(CropImageActivity));
            _activityResultLauncher.Launch(nextActivity);
        }
        public AppCompatImageView imageView => v.FindViewById<AppCompatImageView>(Resource.Id.imageView);
        public Button btnCrop => v.FindViewById<Button>(Resource.Id.btnCrop);
    }
}