using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Interop;
using System;
using System.IO;
using Tokket.Android;
using Tokket.Android.Helpers;
using Tokket.Shared.Helpers;

namespace TokketAppCrop
{
#if (_TOKKEPEDIA)
    [Activity(MainLauncher = false,NoHistory =true, Theme = "@style/CustomAppTheme")]
#endif

#if (_CLASSTOKS)
    [Activity(MainLauncher = false, NoHistory = true, Theme = "@style/CustomAppThemeBlue")]
#endif
    public class CropResultActivity : AppCompatActivity
    {

        /**
     * The image to show in the activity.
     */
        public static Activity Instance { get; private set; }
        public static Bitmap Image;

        private ImageView _imageView;
        Intent intent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_crop_result);

            _imageView = ((ImageView)FindViewById(Resource.Id.resultImageView));
            _imageView.SetBackgroundResource(Resource.Drawable.backdrop);

            intent = Intent;
            if (Image != null)
            {
                _imageView.SetImageBitmap(Image);
                var sampleSize = intent.GetIntExtra("SAMPLE_SIZE", 1);
                var ratio = (int)(10 * Image.Width / (double)Image.Height) / 10d;
                var byteCount = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.HoneycombMr1)
                {
                    byteCount = Image.ByteCount / 1024;
                }
                var desc = $"({Image.Width}, {Image.Height}), Image Size: {sampleSize}, Ratio: {ratio}, Bytes: {byteCount}K";
                FindViewById<TextView>(Resource.Id.resultImageText).Text = desc;
            }
            else
            {
                var imageUri = intent.GetParcelableExtra("URI").JavaCast<Android.Net.Uri>();
                if (imageUri != null)
                {
                    _imageView.SetImageURI(imageUri);
                }
                else
                {
                    Toast.MakeText(this, "No image is set to show", ToastLength.Long).Show();
                }
            }
        }


        public override void OnBackPressed()
        {
            releaseBitmap();
            base.OnBackPressed();
        }

        [Export("onImageViewClicked")]
        public void onImageViewClicked(View view)
        {
            releaseBitmap();
            Finish();
        }

        private void releaseBitmap()
        {
            if (Image != null)
            {
                Image.Recycle();
                Image = null;
            }
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
                case Resource.Id.crop_result_proceed:
                    MemoryStream byteArrayOutputStream = new MemoryStream();
                    Image.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);

                    byte[] byteArray = byteArrayOutputStream.ToArray();
                    Settings.ImageBrowseCrop = Convert.ToBase64String(byteArray);//Base64.EncodeToString(byteArray, Base64.Default);

                    if ((ActivityType)Settings.ActivityInt == ActivityType.AddTokActivityType)
                    {
                        AddTokActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddSetActivityType)
                    {
                        AddSetActivity.Instance.displayImageBrowse(Image, Settings.ImageBrowseCrop);
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddSectionPage)
                    {
                        AddSectionPage.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.ProfileActivity)
                    {
                        ProfileUserActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.ProfileTabActivity)
                    {
                        MainActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddClassTokActivity)
                    {
                        AddClassTokActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddClassGroupActivity)
                    {
                        AddClassGroupActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddClassSetActivity)
                    {
                        AddClassSetActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.SignUpActivity)
                    {
                        SignupActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddGameSetActivity)
                    {
                        AddGameSetActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddPicClassGroup)
                    {
                        ClassGroupActivity.Instance.displayImageBrowse(Image, Settings.ImageBrowseCrop);
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddYearbookActivity) {
                        AddYearbookActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.TokQuestCreateGame)
                    {
                        CreateGameSetActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.AddOpportunityActivity)
                    {
                        AddOpportunityActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.SharedUrisActivity)
                    {
                        SharedUrisActivity.Instance.displayImageBrowse();
                    }
                    else if ((ActivityType)Settings.ActivityInt == ActivityType.NameplateSettingsUIActivity)
                    {
                        NameplateSettingsUIActivity.Instance.displayImageBrowse();
                    }
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}