using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Android.Graphics;
using Android.Text;
using Newtonsoft.Json;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Android.Webkit;
using Android.Content.PM;
using Result = Android.App.Result;
using Tokket.Shared.Models.Tokquest;

namespace Tokket.Android
{
    [Activity(Label = "Class Group", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddClassGroupActivity : BaseActivity
    {
        bool isSaving = true;
        internal static AddClassGroupActivity Instance { get; private set; }
        ClassGroupModel ClassModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addclassgroup_page);
            Settings.ActivityInt = (int)ActivityType.AddClassGroupActivity;
            isSaving = Intent.GetBooleanExtra("isSaving", true);
            Instance = this;
            ClassModel = new ClassGroupModel();
            
            ClassModel.IsCommunity = Intent.GetBooleanExtra("isCommunity", false);
            ClassModel.Level1 = Intent.GetStringExtra("level1");
            ClassModel.Level2 = Intent.GetStringExtra("level2");
            ClassModel.Level3 = Intent.GetStringExtra("level3");

            loadSpinnerType();
            CancelButton.Click += delegate
            {
                Finish();
            };

            SaveButton.Click += async(sender,e) =>
            {
                if (isSaving)
                {
                    await SaveClassGroup();
                }
                else
                {
                    await EditClassGroup();
                }
            };

            ButtonBrowse.Click += delegate
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), 1001);
            };

            ButtonRemoveBrowse.Click += delegate
           {
               ImgBrowse.ContentDescription = "";
               ImgBrowse.SetImageBitmap(null);
               ButtonRemoveBrowse.Visibility = ViewStates.Gone;
               ButtonBrowse.Visibility = ViewStates.Visible;
           };

            if (!isSaving)
            {
                SaveButton.Text = "Update";
                ClassModel = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("ClassGroupModel"));
                EditGroupName.Text = ClassModel.Name;
                EditDescription.Text = ClassModel.Description;
                EditSchool.Text = ClassModel.School;
                ImgBrowse.ContentDescription = ClassModel.Image;
                Glide.With(this).Load(ClassModel.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(ImgBrowse);
            }
        }
        private async Task SaveClassGroup()
        {
            ClassModel.UserId = Settings.GetUserModel().UserId;
            ClassModel.Name = EditGroupName.Text;
            ClassModel.Description = EditDescription.Text;
            ClassModel.School = EditSchool.Text;
            ClassModel.Image = ImgBrowse.ContentDescription;

            if (!string.IsNullOrEmpty(ClassModel.Image))
            {
                ClassModel.Image = "data:image/jpeg;base64," + ClassModel.Image;
            }

            LinearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var result = await ClassService.Instance.AddClassGroupAsync(ClassModel);

            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            if (result != null)
            {
                var dialog = new AlertDialog.Builder(this);
                var alertDialog = dialog.Create();
                alertDialog.SetTitle("");
                alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
                alertDialog.SetMessage("Save Successfully.");
                alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => 
                {
                    if (ClassGroupListActivity.Instance != null)
                    {
                        ClassGroupListActivity.Instance.AddClassGroupCollection(result);
                    }
                    else if (TokChannelActivity.Instance != null)
                    {
                        ClassGroup item = new ClassGroup();
                        var parentProperties = result.GetType().GetProperties();
                        var childProperties = item.GetType().GetProperties();

                        foreach (var parentProperty in parentProperties)
                        {
                            foreach (var childProperty in childProperties)
                            {
                                if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                                {
                                    childProperty.SetValue(item, parentProperty.GetValue(result));
                                    break;
                                }
                            }
                        }

                        TokChannelActivity.Instance.AddClassGroupCollection(item);
                    }
                    Finish();
                });
                alertDialog.Show();
                alertDialog.SetCanceledOnTouchOutside(false);
            }
            else
            {
                var dialog = new AlertDialog.Builder(this);
                var alertDialog = dialog.Create();
                alertDialog.SetTitle("");
                alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
                alertDialog.SetMessage("Failed to save.");
                alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                {
                   alertDialog.Dismiss();
                });
                alertDialog.Show();
                alertDialog.SetCanceledOnTouchOutside(false);
            }
        }
        private async Task EditClassGroup()
        {
            ClassModel.Name = EditGroupName.Text;
            ClassModel.Description = EditDescription.Text;
            ClassModel.School = EditSchool.Text;
            ClassModel.Image = ImgBrowse.ContentDescription;

            if (!string.IsNullOrEmpty(ClassModel.Image))
            {
                if (!URLUtil.IsValidUrl(ClassModel.Image))
                {
                    ClassModel.Image = "data:image/jpeg;base64," + ClassModel.Image;
                }
            }

            LinearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var result = await ClassService.Instance.UpdateClassGroupAsync(ClassModel);
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            string alertmssg = "Failed to update.";
            if (result)
            {
                alertmssg = "Updated Successfully.";
            }

            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle("");
            alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDialog.SetMessage(alertmssg);
            alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
            {
                if (result)
                {
                    ClassGroupActivity.Instance.model = ClassModel;
                    ClassGroupActivity.Instance.Initialize();
                    ClassGroupListActivity.Instance.AddClassGroupCollection(ClassModel, false);
                    Finish();
                }
            });
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == 1001) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }

        }
        public void displayImageBrowse()
        {
            //Main Image
            //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
            ImgBrowse.ContentDescription = Settings.ImageBrowseCrop;
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            ImgBrowse.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            ButtonBrowse.Visibility = ViewStates.Gone;
            ButtonRemoveBrowse.Visibility = ViewStates.Visible;
        }
        public void loadSpinnerType()
        {
            spinnerGroupType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerType_ItemSelected);
            List<string> spinnerTypeList = new List<string>();

            spinnerTypeList.Add("Choose...");
            spinnerTypeList.Add("Class");
            spinnerTypeList.Add("Clubs");
            spinnerTypeList.Add("Teams");
            spinnerTypeList.Add("Study");
            spinnerTypeList.Add("Faith");
            spinnerTypeList.Add("Other");
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, spinnerTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerGroupType.Adapter = Aadapter;
            if (!isSaving) {
             
                var type = string.IsNullOrEmpty(ClassModel.GroupKind) ? string.Empty : ClassModel.GroupKind;
                switch (type.ToLower()) {
                    case "class": spinnerGroupType.SetSelection(1); break;
                    case "club": spinnerGroupType.SetSelection(2); break;
                    case "team": spinnerGroupType.SetSelection(3); break;
                    case "study": spinnerGroupType.SetSelection(3); break;
                    case "faith": spinnerGroupType.SetSelection(3); break;
                    case "other": spinnerGroupType.SetSelection(3); break;
                    default: spinnerGroupType.SetSelection(1); break;
                }
             
            }
        }
        private void spinnerType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var typeSelected = spinnerGroupType.GetItemAtPosition(e.Position).ToString();
            switch (typeSelected.ToLower())
            {
                case "class":
                    ClassModel.GroupKind = "class";
                    break;
                case "clubs":
                    ClassModel.GroupKind = "club";
                    break;
                case "teams":
                    ClassModel.GroupKind = "team";
                    break;
                case "study":
                    ClassModel.GroupKind = "study";
                    break;
                case "faith":
                    ClassModel.GroupKind = "faith";
                    break;
                case "other":
                    ClassModel.GroupKind = "other";
                    break;
                default:
                    break;
            }
        }
        public EditText EditGroupName => FindViewById<EditText>(Resource.Id.EditACGGroupName);
        public EditText EditDescription => FindViewById<EditText>(Resource.Id.EditACGDescription);
        public EditText EditSchool => FindViewById<EditText>(Resource.Id.EditACGSchool);
        public ImageView ImgBrowse => FindViewById<ImageView>(Resource.Id.addclassgroup_imagebrowse);
        public Button ButtonBrowse => FindViewById<Button>(Resource.Id.btnAddClassGroup_btnBrowseImage);
        public Button ButtonRemoveBrowse => FindViewById<Button>(Resource.Id.btnAddClassGroupRemoveImgMain);
        public TextView CancelButton => FindViewById<TextView>(Resource.Id.btnAddClassGroupCancel);
        public TextView SaveButton => FindViewById<TextView>(Resource.Id.btnAddClassGroupSave);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_addclassgroup);
        public ProgressBar ProgressCircle => FindViewById<ProgressBar>(Resource.Id.progressbarAddClassGroup);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextAddClassGroup);
        public Spinner spinnerGroupType => FindViewById<Spinner>(Resource.Id.spinnerGroupType);
    }
}