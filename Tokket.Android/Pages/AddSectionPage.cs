using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Tokket.Shared.Services;
using AlertDialog = Android.App.AlertDialog;
using Tokket.Shared.Models;
using Android.Webkit;
using ImageViews.Photo;
using Android.Animation;
using Tokket.Android.Helpers;
using Android.Content.PM;
using Android.Graphics.Drawables;
using System.IO;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "Add Section",Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddSectionPage : BaseActivity
    {
        internal static AddSectionPage Instance { get; private set; }
        TokSection tokSection = new TokSection();
        int isAddSection = 0; // Default is 0 for add; 1 for update; 2 for view
        TokModel tokModel = new TokModel();
        GlideImgListener GListener;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addsection_page);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.custom_toolbar);

            SetSupportActionBar(tokback_toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            TxtSectionNo.Text = Intent.GetIntExtra("SectionNo", 0).ToString();
            TxtSectionNoView.Text = TxtSectionNo.Text;
            isAddSection = Intent.GetIntExtra("isAddSection", 0);
            tokModel = JsonConvert.DeserializeObject<TokModel>(Intent.GetStringExtra("tokModel"));

            
            if (isAddSection == 0)
            {
                tokSection.TokId = tokModel.Id;
                tokSection.TokTypeId = tokModel.TokTypeId;
                tokSection.UserId = Settings.GetUserModel().UserId;
            }
            else
            {
                tokSection = JsonConvert.DeserializeObject<TokSection>(Intent.GetStringExtra("tokSection"));
                tokSection.UserId = Settings.GetUserModel().UserId;
                SectionTitle.Text = tokSection.Title;
                SectionContent.Text = tokSection.Content;
                TitleView.Text = tokSection.Title;
                ContentView.Text = tokSection.Content;

                if (isAddSection == 1) //If selected is edit
                {
                    this.Title = "Update Section";
                    SaveSection.Text = "Update Section";
                }
                else //if selected is view
                {
                    this.Title = "View Section";

                    ScrollAddSectionForEntry.Visibility = ViewStates.Gone;
                    ScrollAddSectionForView.Visibility = ViewStates.Visible;
                }
                
                if (tokSection.Image != null)
                {
                    if (URLUtil.IsValidUrl(tokSection.Image))
                    {
                        Glide.With(this).Load(tokSection.Image).Into(ImageDisplay);

                        GListener = new GlideImgListener();
                        GListener.ParentActivity = this;
                        Glide.With(this).Load(tokSection.Image).Listener(GListener).Into(ImageDisplayView);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(tokSection.Image);
                        ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                        ImageDisplayView.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }

                    RelaveAddSectionMegaImg.Visibility = ViewStates.Visible;
                    RelativeAddSectionView.Visibility = ViewStates.Visible;
                }
            }

            SaveSection.Click += async(object sender, EventArgs e) =>
            {
                tokSection.Title = SectionTitle.Text;
                tokSection.Content = SectionContent.Text;

                bool issuccess = false;
                string resultMssg = "";
                LinearProgress.Visibility = ViewStates.Visible;
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                if (isAddSection == 0) //Saving
                {
                    TextProgress.Text = "Saving...";
                    issuccess = await TokService.Instance.CreateTokSectionAsync(tokSection, tokModel.Id, 0);
                    if (issuccess == false)
                    {
                        resultMssg = "Failed to add section!";
                    }
                    else
                    {
                        resultMssg = "Successfully added new section!";
                    }
                }
                else if (isAddSection == 1) //Edit
                {
                    TextProgress.Text = "Updating...";
                    issuccess = await TokService.Instance.UpdateTokSectionAsync(tokSection);
                    if (issuccess == false)
                    {
                        resultMssg = "Failed to update section!";
                    }
                    else
                    {
                        resultMssg = "Successfully updated the section!";
                    }
                }
                LinearProgress.Visibility = ViewStates.Gone;

                //Requested by David Onquit to remove the popup message from the tokkepediaandroid channel.
                //var builder = new AlertDialog.Builder(this);
                //builder.SetMessage(resultMssg);
                //builder.SetTitle("");
                //var dialog = (AlertDialog)null;
                //builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                //{
                if (issuccess)
                {
                        var modelSerialized = JsonConvert.SerializeObject(tokSection);
                        Intent intent = new Intent();
                        intent.PutExtra("toksection", modelSerialized);
                        SetResult(Result.Ok, intent);
                        Finish();
                 }
                else
                {
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage(resultMssg);
                    builder.SetTitle("");
                    var dialog = (AlertDialog)null;
                    builder.SetPositiveButton("OK" ?? "OK", (d, index) =>{});
                    dialog = builder.Create();
                    dialog.Show();
                }

                //});
                //dialog = builder.Create();
                //dialog.Show();

                //if (issuccess)
                //{
                //    dialog.SetCanceledOnTouchOutside(false);
                //}
            };

            ImageDisplayView.Click += delegate
            {
                Bitmap imgBitmap = ((BitmapDrawable)ImageDisplayView.Drawable).Bitmap;
                MemoryStream byteArrayOutputStream = new MemoryStream();
                imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();

                Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
                Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
                this.StartActivity(nextActivity);
            };
        }
        [Java.Interop.Export("OnClickAddTokImgDetail")]
        public void OnClickAddTokImgDetail(View v)
        {
            Settings.ActivityInt = (int)ActivityType.AddSectionPage;
            Settings.BrowsedImgTag = (int)v.Tag;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddSectionPage);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddSectionPage) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }
        }
        public void displayImageBrowse()
        {
            byte[] imageDetailBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            tokSection.Image = Settings.ImageBrowseCrop;

            //ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
            Glide.With(this).AsBitmap().Load(imageDetailBytes).Into(ImageDisplay);

            Settings.ImageBrowseCrop = null;
            RelaveAddSectionMegaImg.Visibility = ViewStates.Visible;
        }
        [Java.Interop.Export("OnDeleteImageDtl")]
        public void OnDeleteImageDtl(View v)
        {
            int vtag = (int)v.Tag;
            RelaveAddSectionMegaImg.Visibility = ViewStates.Gone;
            tokSection.Image = null;
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
       
        #region UI Properties
        public ImageView ImageBrowse => FindViewById<ImageView>(Resource.Id.btnAddSection_img);
        public EditText SectionTitle => FindViewById<EditText>(Resource.Id.EdittxtAddSectionTitle);
        public EditText SectionContent => FindViewById<EditText>(Resource.Id.txtAddSectionContent);
        public ImageView ImageDisplay => FindViewById<ImageView>(Resource.Id.btnAddSection_displayimg);
        public ImageView ImageDelete => FindViewById<ImageView>(Resource.Id.btnAddSection_deleteImg);
        public Button SaveSection => FindViewById<Button>(Resource.Id.btnAddSectionSave);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_addsectionprogress);
        public TextView TextProgress => FindViewById<TextView>(Resource.Id.progressBarTextAddSection);
        public RelativeLayout RelaveAddSectionMegaImg => FindViewById<RelativeLayout>(Resource.Id.RelativeAddSection);
        public ScrollView ScrollAddSectionForEntry => FindViewById<ScrollView>(Resource.Id.ScrollAddSectionForEntry);

        //For Viewing Controls
        public RelativeLayout RelativeAddSectionView => FindViewById<RelativeLayout>(Resource.Id.RelativeAddSectionView);
        public ScrollView ScrollAddSectionForView => FindViewById<ScrollView>(Resource.Id.ScrollAddSectionForView);
        public TextView TitleView => FindViewById<TextView>(Resource.Id.TxtDisplayAddSectionTitle);
        public ImageView ImageDisplayView => FindViewById<ImageView>(Resource.Id.btnAddSection_displayimgView);
        public TextView ContentView => FindViewById<TextView>(Resource.Id.txtAddSectionContentView);
        public TextView TxtSectionNo => FindViewById<TextView>(Resource.Id.txtAddSectionMegaNumber);
        public TextView TxtSectionNoView => FindViewById<TextView>(Resource.Id.txtAddSectionViewMegaNumber);
        #endregion
    }
}