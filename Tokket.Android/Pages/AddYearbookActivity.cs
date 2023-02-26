using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Bumptech.Glide;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Helpers;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "AddYearbookActivity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddYearbookActivity : BaseActivity
    {
        #region Spinners Item
        string[] SchoolTypeItem = new string[] { "School Type", "Junior High", "Senior High","College", "Medical", "Technical","Professional","Vocational","Faith Based", "Bussiness","Miltary" };
        string[] GroupTypeItem = new string[] { "Group Type", "School", "Batch","Class", "Team","Club","Cohort", "Graduating Class", "Session", "Semester", "Quarter" };
        string[] TimingTypeItem = new string[] { "Timing Type", "In progress", "After class" };
        string[] TileTypeItem = new string[] { "Tile Type","Tok","Pic" };

        #endregion
        internal static AddYearbookActivity Instance;
        YearbookTok Yearbook = new YearbookTok();
        bool isUpdate = false; GlideImgListener GListener;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addyearbook_page);
            LoadSpinnerAdapters();
            CancelButton.Click += (obj,_event) => { Finish(); };
            SaveYearbookButton.Click += SaveYearbook_Click;
            Instance = this;
            if (!string.IsNullOrEmpty(Intent.GetStringExtra("isUpdate"))) {
                isUpdate = Intent.GetStringExtra("isUpdate") == "true";
                Yearbook = JsonConvert.DeserializeObject<YearbookTok>(Intent.GetStringExtra("yearbookModel")) ;
                SaveYearbookButton.Text = "Edit Peerbook";

                GroupType.SetSelection(GroupTypeItem.ToList().IndexOf(Yearbook.YearbookGroupType));
                SchoolName.Text = Yearbook.YearbookSchoolname;
                TileType.SetSelection(TileTypeItem.ToList().IndexOf(Yearbook.YearbookTileType));
                SchoolType.SetSelection(SchoolTypeItem.ToList().IndexOf(Yearbook.YearbookType));
                TimingType.SetSelection(TimingTypeItem.ToList().IndexOf(Yearbook.YearbookTiming));
                TitleText.Text = Yearbook.PrimaryFieldText;
                DescriptionText.Text = Yearbook.SecondaryFieldText;

                if (!string.IsNullOrEmpty(Yearbook.Image)) {
                    ImageDisplay.ContentDescription = Yearbook.Image;
                    byte[] imageMainBytes = Convert.FromBase64String(Yearbook.Image);
                    ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                    BrowseImgButton.Visibility = ViewStates.Gone;
                    RemoveImgButton.Visibility = ViewStates.Visible;
                }
            }
               
            // Create your application here
        }

        private async void SaveYearbook_Click(object sender, EventArgs e)
        {
            linearLayoutProgress.Visibility = ViewStates.Visible;

            var user = Settings.GetTokketUser();
            if (!isUpdate) {
                Yearbook = new YearbookTok();
                Yearbook.UserPhoto = user.UserPhoto;
                Yearbook.UserId = user.Id;
                Yearbook.UserDisplayName = user.DisplayName;
                Yearbook.UserCountry = user.Country;
                Yearbook.UserState = user.State;
                Yearbook.Category = "Yearbook";
                Yearbook.CategoryId = "category-" + Yearbook.Category.ToIdFormat();
                Yearbook.TokGroup = "";
                Yearbook.TokType = "";
                Yearbook.TokTypeId = "";
            }
         

            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageDisplay.ContentDescription))
                {
                    Yearbook.Image = "data:image/jpeg;base64," + ImageDisplay.ContentDescription;
                }
            }

          
            Yearbook.YearbookGroupType = GroupType.GetItemAtPosition(GroupType.SelectedItemPosition).ToString() ;
            Yearbook.YearbookSchoolname = SchoolName.Text;
            Yearbook.YearbookTileType = TileType.GetItemAtPosition(TileType.SelectedItemPosition).ToString();
            Yearbook.YearbookType = SchoolType.GetItemAtPosition(SchoolType.SelectedItemPosition).ToString();
            Yearbook.YearbookTiming = TimingType.GetItemAtPosition(TimingType.SelectedItemPosition).ToString();
            Yearbook.PrimaryFieldText = TitleText.Text;
            Yearbook.SecondaryFieldText = DescriptionText.Text;

          var result = new ResultModel() {  };
            if (!isUpdate)
            {
                result = await TokService.Instance.CreateYearbookAsync(Yearbook);
            }
            else {

                 result = await TokService.Instance.UpdateYearbookAsync(Yearbook);
            }
            if (result.ResultEnum == Tokket.Shared.Helpers.Result.Success)
            {
                alertMessage("","Yearbook successfully saved!",0,(d,fv)=> {
                    if (!isUpdate)
                        YearbookActivity.Instance.AddYearbook(Yearbook);
                    else
                    {
                        Intent intent = new Intent();
                        intent.PutExtra("updatedPeerbook", JsonConvert.SerializeObject(Yearbook));
                        SetResult(Result.Ok, intent);
                    
                    }
                    Finish(); });
            }
            else {
                alertMessage("", "Yearbook failed saved!", 0);
            }

            linearLayoutProgress.Visibility = ViewStates.Gone;
        }

        void LoadSpinnerAdapters() {
            var adapter1 = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,SchoolTypeItem);
            var adapter2 = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,GroupTypeItem);
            var adapter3 = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,TimingTypeItem);
            var adapter4 = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,TileTypeItem);

            adapter1.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            adapter2.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            adapter3.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            adapter4.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            SchoolType.Adapter = adapter1;
            GroupType.Adapter = adapter2;
            TimingType.Adapter = adapter3;
            TileType.Adapter = adapter4;
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddYearbookActivity);
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            ImageDisplay.SetImageBitmap(null);
            Yearbook.Image = null;
            ImageDisplay.ContentDescription = "";
            BrowseImgButton.Visibility = ViewStates.Visible;
            RemoveImgButton.Visibility = ViewStates.Gone;
            
        }

        public void displayImageBrowse()
        {
            //Main Image
            ImageDisplay.SetImageBitmap(null);
            if (Settings.BrowsedImgTag == -1)
            {
                //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
                ImageDisplay.ContentDescription = Settings.ImageBrowseCrop;
                byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                ImageDisplay.SetBackgroundColor(Color.Transparent);
                BrowseImgButton.Visibility = ViewStates.Gone;
                RemoveImgButton.Visibility = ViewStates.Visible;
            }
           
          
            Settings.ImageBrowseCrop = null;
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

        private void alertMessage(string title, string message, int icon,EventHandler<DialogClickEventArgs> Okhandler = null)
        {
            if (Okhandler == null) {
                Okhandler = (d, fv) => { };
            }
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(icon);
            alertDialog.SetMessage(message);
            alertDialog.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy),Okhandler);
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.AddYearbookActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                Settings.ActivityInt = (int)ActivityType.AddYearbookActivity;
                this.StartActivityForResult(nextActivity, (int)ActivityType.AddYearbookActivity);

                if (Settings.BrowsedImgTag != -1)
                {
                    int vtag = Settings.BrowsedImgTag;

                  
                }
            }
        }
        #region view properties
        Spinner SchoolType => FindViewById<Spinner>(Resource.Id.spn_schooltype);
        Spinner GroupType => FindViewById<Spinner>(Resource.Id.spn_grouptype);
        Spinner TimingType => FindViewById<Spinner>(Resource.Id.spn_timingtype);

        Spinner TileType => FindViewById<Spinner>(Resource.Id.spn_tiletype);

        EditText SchoolName => FindViewById<EditText>(Resource.Id.txt_schoolname);

        EditText TitleText => FindViewById<EditText>(Resource.Id.txt_title);
        EditText DescriptionText => FindViewById<EditText>(Resource.Id.txt_desc);

        EditText GroupNameText => FindViewById<EditText>(Resource.Id.txt_groupname);

        TextView CancelButton => FindViewById<TextView>(Resource.Id.btnAddYearbookCancel);

        Button SaveYearbookButton => FindViewById<Button>(Resource.Id.btnAddCYearbookSave);
        ImageView ImageDisplay => FindViewById<ImageView>(Resource.Id.addyearbook_imagebrowse);

         Button BrowseImgButton => FindViewById<Button>(Resource.Id.btnAddYearbook_btnBrowseImage);
         Button RemoveImgButton => FindViewById<Button>(Resource.Id.btnAddYearbookRemoveImgMain);
        LinearLayout linearLayoutProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_ClassGroup);
        #endregion
    }
}