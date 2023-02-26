using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Android.Custom;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using Android.Provider;
using Settings = Tokket.Shared.Helpers.Settings;
using Result = Android.App.Result;
using NetUri = Android.Net.Uri;

namespace Tokket.Android
{
    [Activity(Label = "Graphic", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class NameplateSettingsUIActivity : BaseActivity
    {
        private int REQUEST_COLORS = 1001;
        internal static NameplateSettingsUIActivity Instance { get; private set; }
        HandlePosition handlePosition = HandlePosition.None;
        TokHandle tokhandle;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_nameplate_settings_ui);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            tokhandle = new TokHandle();
            var currenTokHandle = Intent.GetStringExtra("CurrentTokHandle");
            if (!string.IsNullOrEmpty(currenTokHandle))
            {
                tokhandle = JsonConvert.DeserializeObject<TokHandle>(currenTokHandle);
                SelectedPosition(tokhandle.Position, tokhandle.Label);
                if (!string.IsNullOrEmpty(tokhandle.Image))
                {
                    imagePreviewGraphic.SetBackgroundColor(Color.Transparent);
                    Glide.With(this).Load(tokhandle.Image).Into(imagePreviewGraphic);
                }
                else if (!string.IsNullOrEmpty(tokhandle.Color))
                {
                    txtTokHandle.SetBackgroundColor(Color.ParseColor(tokhandle.Color));
                    imagePreviewGraphic.SetBackgroundColor(Color.ParseColor(tokhandle.Color));
                }
            }

            Settings.ActivityInt = (int)ActivityType.NameplateSettingsUIActivity;
            Instance = this;


            btnChangeGraphic.Click += delegate
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.NameplateSettingsUIActivity);
            };

            btnChangeColor.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(ColorPickerDialog));
                this.StartActivityForResult(nextActivity, REQUEST_COLORS);
            };

            txtTokHandle.TextChanged += (s, e) =>
            {
                SelectedPosition(handlePosition, txtTokHandle.Text);
            };

            btnChangePosition.Click += delegate
            {
                new NameplatePositionDialog(this).Show();
            };

            btnSaveChanges.Click += async(s,e) =>
            {
                showBlueLoading(this);
                var response = await SaveNameplateSettings();
                hideBlueLoading(this);

                bool isSuccess = false;
                string message = response.ResultMessage;
                if (response.ResultEnum == Shared.Helpers.Result.Success)
                {
                    message = "Updated successfully!";
                    isSuccess = true;
                }

                ShowLottieMessageDialog(this, message, isSuccess, header: "Failed!");
            };
        }

        private async Task<ResultModel> SaveNameplateSettings()
        {
            tokhandle.Position = handlePosition;
            tokhandle.UserId = Settings.GetTokketUser().Id;
            tokhandle.Label = txtTokHandle.Text;
            return await TokHandleService.Instance.UpdateTokHandleAsync(tokhandle);
        }

        public void SelectedPosition(HandlePosition _handlePosition, string textToDisplay = "TOK HANDLE")
        {
            handlePosition = _handlePosition;

            if (handlePosition == HandlePosition.OptionA)
            {
                lbl_nameuser.Text = $"My Display Name \n {textToDisplay}";
                lbl_nameuser.Gravity = GravityFlags.Center | GravityFlags.Top;
            }
            else if (handlePosition == HandlePosition.OptionB)
            {
                lbl_nameuser.Text = textToDisplay;
                lbl_nameuser.Gravity = GravityFlags.Top;
            }
            else if (handlePosition == HandlePosition.OptionC)
            {
                lbl_nameuser.Text = textToDisplay;
                lbl_nameuser.Gravity = GravityFlags.Bottom;
            }
            else
            {
                lbl_nameuser.Text = textToDisplay;
                lbl_nameuser.Gravity = GravityFlags.Center;
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }
                    
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.NameplateSettingsUIActivity) && (resultCode == Result.Ok) && (data != null))
            {
                tokhandle.Color = null;

                txtTokHandle.SetBackgroundColor(Color.Transparent);
                imagePreviewGraphic.SetBackgroundColor(Color.Transparent);
                NetUri uri = data.Data;
                Settings.ImageBrowseCrop = (string)uri;

                onClickImage(uri, requestCode);
            }
            else if ((requestCode == REQUEST_COLORS) && (resultCode == Result.Ok) && (data != null))
            {
                imagePreviewGraphic.SetImageBitmap(null);

                var colorHex = data.GetStringExtra("color");
                tokhandle.Color = colorHex;
                tokhandle.Image = null;
                txtTokHandle.SetBackgroundColor(Color.ParseColor(colorHex));
                imagePreviewGraphic.SetBackgroundColor(Color.ParseColor(tokhandle.Color));
            }
        }

        private void onClickImage(NetUri uri, int requestCode)
        {
            var messageCropDialog = new MessageDialog(this, "Option", "", "Crop", "Save", (s, e) =>
            {
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                this.StartActivityForResult(nextActivity, requestCode);

            },
                (s, e) =>
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, uri);

                    MemoryStream outputStream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, outputStream);
                    byte[] byteArray = outputStream.ToArray();

                    //Use your Base64 String as you wish
                    Settings.ImageBrowseCrop = Base64.EncodeToString(byteArray, Base64Flags.Default);

                    displayImageBrowse();
                });
            messageCropDialog.Show();
        }

        public void displayImageBrowse()
        {
            tokhandle.Image = "data:image/jpeg;base64," + Settings.ImageBrowseCrop;
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            imagePreviewGraphic.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            Settings.ImageBrowseCrop = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public EditText txtTokHandle => FindViewById<EditText>(Resource.Id.txtTokHandle);
        public Button btnChangeColor => FindViewById<Button>(Resource.Id.btnChangeColor);
        public Button btnChangeGraphic => FindViewById<Button>(Resource.Id.btnChangeGraphic);
        public Button btnSaveChanges => FindViewById<Button>(Resource.Id.btnSaveChanges);
        public Button btnChangePosition => FindViewById<Button>(Resource.Id.btnChangePosition);
        public ImageView imagePreviewGraphic => FindViewById<ImageView>(Resource.Id.imagePreviewGraphic);

        public TextView lbl_nameuser => FindViewById<TextView>(Resource.Id.lbl_nameuser);
    }
}