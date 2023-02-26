using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using System;
using Tokket.Shared.Helpers;
using Android.Graphics;
using Java.IO;
using Tokket.Android.Helpers;
using Android.App;
using Newtonsoft.Json;
using Bumptech.Glide.Request;
using Bumptech.Glide;
using NetUri = Android.Net.Uri;
using Android.Provider;
using Environment = Android.OS.Environment;
using Settings = Tokket.Shared.Helpers.Settings;
using File = Java.IO.File;
using Tokket.Core;

namespace Tokket.Android.Fragments
{
    public class BottomSheetUserPhotoFragment : BottomSheetDialogFragment
    {
        View v;
        NetUri imageUri;
        ImageView PhotoImage;
        Activity activity;
        public BottomSheetUserPhotoFragment(Activity _activity, ImageView _photoImage)
        {
            this.activity = _activity;
            this.PhotoImage = _photoImage;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.row_userphoto_options, container, false);

            var btnTakePhoto = v.FindViewById<Button>(Resource.Id.btnTakePhoto);
            var btnAvatars = v.FindViewById<Button>(Resource.Id.btnAvatars);
            var btnChoosePhoto = v.FindViewById<Button>(Resource.Id.btnChoosePhoto);
            var btnBadges = v.FindViewById<Button>(Resource.Id.btnBadges);

            if (Settings.ActivityInt != (int)ActivityType.ProfileActivity && Settings.ActivityInt != (int)ActivityType.ProfileTabActivity)
            {
                btnAvatars.Visibility = ViewStates.Gone;
                btnBadges.Visibility = ViewStates.Gone;
            }

            btnTakePhoto.Click += delegate
            {
                StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();

                StrictMode.SetVmPolicy(builder.Build());

                Intent intent = new Intent(MediaStore.ActionImageCapture);

                var dir = Context.GetExternalFilesDir(Environment.DirectoryDcim);
                File _file = new File(dir, String.Format("InspectionPhoto_{0}.jpg", Guid.NewGuid()));
                imageUri = NetUri.FromFile(_file);
                intent.PutExtra(MediaStore.ExtraOutput, NetUri.FromFile(_file));
                StartActivityForResult(intent, 0);
            };

            btnAvatars.Click += delegate
            {
                Intent nextActivity = new Intent(this.Activity, typeof(AvatarsActivity));
                activity.StartActivityForResult(nextActivity, (int)ActivityType.AvatarsActivity);
                Dismiss();
            };

            btnChoosePhoto.Click += delegate
            {
                Intent nextActivity = new Intent();
                nextActivity.SetType("image/*");
                nextActivity.SetAction(Intent.ActionGetContent);
                activity.StartActivityForResult(Intent.CreateChooser(nextActivity, "Select Picture"), Settings.ActivityInt);
                Dismiss();
            };

            btnBadges.Click += delegate
            {
                Intent nextActivity = new Intent(this.Activity, typeof(BadgesActivity));
                activity.StartActivityForResult(nextActivity, 40011);
                Dismiss();
            };

            return v;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0 && resultCode == -1)
            {
                try
                {
                    ImageDecoder.Source source = ImageDecoder.CreateSource(this.Context.ContentResolver, imageUri);
                    Bitmap bitmap = ImageDecoder.DecodeBitmap(source); ; //(Bitmap)data.Extras.Get("data");
                    Bitmap scaledBitmap = SpannableHelper.scaleDown(bitmap, 300, true);

                    if (Settings.ActivityInt != (int)ActivityType.AddTokActivityType)
                        //show image
                        activity.FindViewById<ImageView>(PhotoImage.Id).SetImageBitmap(scaledBitmap);

                    //save image to database
                    string base64img = ImageConverter.BitmapToBase64(scaledBitmap);
                    Settings.ImageBrowseCrop = base64img;

                    if (Settings.ActivityInt == (int)ActivityType.ProfileActivity || Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
                    {
                        MainActivity.Instance.RunOnUiThread(async () => await MainActivity.Instance.SaveUserCoverPhoto(base64img));
                    }
                    else if (Settings.ActivityInt == (int)ActivityType.AddTokActivityType)
                    {
                        AddTokActivity.Instance.displayImageBrowse();
                    }
                    else if (Settings.ActivityInt == (int)ActivityType.AddSetActivityType)
                    {
                        AddSetActivity.Instance.displayImageBrowse(scaledBitmap, base64img);
                    }

                    Dismiss();

                    //todo: need to delete temporary image
                }
                catch (Exception)
                {

                }
            }
            else if ((requestCode == Settings.ActivityInt) && (resultCode == -1) && (data != null))
            {
                NetUri uri = data.Data;
                Intent nextActivity = new Intent(activity, typeof(CropImageActivity));

                ImageDecoder.Source source = ImageDecoder.CreateSource(this.Context.ContentResolver, uri);
                Bitmap bitmap = ImageDecoder.DecodeBitmap(source);
                Bitmap scaledBitmap = SpannableHelper.scaleDown(bitmap, 500, true); //Set image size maximum to 1200 only

                Settings.ImageBrowseCrop = ImageConverter.BitmapToBase64(scaledBitmap);
                this.StartActivityForResult(nextActivity, requestCode);
            }
            else if ((requestCode == 40011) && (resultCode == -1) && (data != null))
            {
                var badgeString = data.GetStringExtra("Badge");
                var badgeModel = JsonConvert.DeserializeObject<BadgeOwned>(badgeString);

                if (Settings.ActivityInt == (int)ActivityType.ProfileActivity)
                {
                    Glide.With(activity).Load(badgeModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileUserActivity.Instance.ProfileUserPhoto);
                }
                else if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
                {
                    Glide.With(activity).Load(badgeModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileFragment.Instance.ProfileUserPhoto);
                    MainActivity.Instance.loadToks(badgeModel.Image);
                }

                Dismiss();
            }
        }


        public override void Dismiss()
        {
            base.Dismiss();
        }

    }
}