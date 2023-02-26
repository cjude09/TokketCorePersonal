using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using Tokket.Android.Custom;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Result = Android.App.Result;
using Android.Provider;
using Settings = Tokket.Shared.Helpers.Settings;
using NetUri = Android.Net.Uri;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Name = "TokketCore.Tokket.Android.AddClassSetActivity", Label = "@string/add_classset", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddClassSetActivity : BaseActivity
    {
        internal static AddClassSetActivity Instance { get; private set; }
        ObservableCollection<string> PrivacyList;
        ObservableCollection<ClassGroupModel> ClassGroupCollection;
        ClassSetModel model; ClassGroupModel ClassGroupModel;
        string UserId; bool isSave = true, isSuperSet = false;
        int COLOR_REQUEST_CODE = 1001;
        private Dialog messageCropDialog;
        string[] ArrTokGroup = new string[] {"Playable", "Non-playable" };
        string[] ArrPlayableType = new string[] {"Basic (Only 1 Detail)", "Detailed (2-10 Details)" };
        string[] ArrNonPlayableType = new string[] {"Mega (Unlimited Sections of 150k chars)", "Pic (Basic with only picture on tile)", "List (Unlimited Details)" };
        string[] arrayPlayableType;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.addclassset_page);
            Settings.ActivityInt = Convert.ToInt16(ActivityType.AddClassSetActivity);
            Instance = this;

            SetSupportActionBar(customToolbar);
            customToolbar.SetBackgroundColor(defaultPrimaryColor);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            txtToolbarTitle.Text = "Add Class Set";

            UserId = Settings.GetUserModel().UserId;
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            RecyclerGroupList.SetLayoutManager(new GridLayoutManager(Application.Context, numcol));

            ClassGroupCollection = new ObservableCollection<ClassGroupModel>();

            model = new ClassSetModel();

            var stringClassGroup = Intent.GetStringExtra("ClassGroupModel");
            if (stringClassGroup != null)
            {
                ClassGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(stringClassGroup);
                model.GroupId = ClassGroupModel.Id;
                TextGroupName.ContentDescription = ClassGroupModel.Id;
                TextGroupName.Text = ClassGroupModel.Name;
            
            }

            LoadSpinners();
            RunOnUiThread(async () => await LoadClassGroup());

            autoCompleteTokGroup.ItemClick += TokGroup_ItemClick;

            var Aadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, ArrTokGroup);
            autoCompleteTokGroup.Adapter = null;
            autoCompleteTokGroup.Adapter = Aadapter;

            var SpinPrivacyadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, PrivacyList);
            SpinPrivacyadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            //SpinPrivacy.Adapter = null;
            // SpinPrivacy.Adapter = SpinPrivacyadapter;

            toolbarSave.Click += async(sender,e) =>
            {
                if (isSave)
                {
                    await AddClassSetFunction();
                }
                else
                {
                    await EditClassSetFunction();
                }
            };
            
            try
            {
                isSuperSet = MyClassSetsActivity.Instance.viewpager.CurrentItem == 1;
                if (isSuperSet)
                {
                    SetTitleLabel.Text = "Enter the SuperSet info";
                    txtInputLayoutSetName.Hint = "SuperSet Name (required field)";
                    txtInputLayoutTokGroup.Visibility = ViewStates.Gone;
                    txtInputLayoutPlayableType.Visibility = ViewStates.Gone;
                    model.Label = "superset";  
                }
                else
                {
                    txtInputLayoutSetName.Hint = "Set Name(required field)";
                }
            }
            catch (Exception ex)
            {
            }

            isSave = Intent.GetBooleanExtra("isSave", true);
            if (!isSave && !isSuperSet)
            {
                toolbarSave.Text = "Update Post";
                LinearGroup.Visibility = ViewStates.Gone;
                model = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("ClassTokSetsModel"));

                if (model.Group != null)
                {
                    classGroupName.Visibility = ViewStates.Visible;
                    classGroupName.Text = "Class Group: " + model.Group.Name;
                }

                //Tok Group
                if (model.IsPlayable)
                {
                    autoCompleteTokGroup.SetText(ArrTokGroup[0], false);
                    autoCompleteTokGroup.ContentDescription = ArrTokGroup[0];
                }
                else
                {
                    autoCompleteTokGroup.SetText(ArrTokGroup[1], false);
                    autoCompleteTokGroup.ContentDescription = ArrTokGroup[1];
                }

                setArrPlayableType();

                var spinnerTGPosition = 0;
                switch (model.TokGroup.ToLower())
                {
                    case "basic": //Playable
                        spinnerTGPosition = 0;
                        break;
                    case "detail": 
                    case "detailed":  //Playable
                        spinnerTGPosition = 1;
                        break;
                    case "mega": //Non playable
                        spinnerTGPosition = 0;
                        break;
                    case "pic": //Non playable
                        spinnerTGPosition = 1;
                        break;
                    case "list": //Non playable
                        spinnerTGPosition = 2;
                        break;
                }
                
                txtPlayableType.SetText(arrayPlayableType[spinnerTGPosition], false);
                txtPlayableType.ContentDescription = model.TokGroup;

                EditClassName.Text = model.TokType;

                EditClassName.Text = model.TokType;
                //   SpinPrivacy.ContentDescription = model.Privacy;
                EditClassSetName.Text = model.Name;
                EditDescription.Text = model.Description;
                TextGroupName.ContentDescription = model.GroupId;
                txtReferenceId.Text = model.ReferenceId ?? "";

                if (!string.IsNullOrEmpty(model.Image))
                {
                    ImgClassSet.Visibility = ViewStates.Visible;
                }
                
                Glide.With(this).Load(model.Image).Into(ImgClassSet);

                var resultRequest = ClassGroupCollection.FirstOrDefault(c => c.Id == model.GroupId);
                if (resultRequest != null) //If Edit
                {
                    TextGroupName.Text = resultRequest.Name;
                }
            }
            else if(!isSave && isSuperSet) {
                toolbarSave.Text = "Update Superset";
                LinearGroup.Visibility = ViewStates.Gone;
                model = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("ClassTokSetsModel"));


                txtInputLayoutTokGroup.Visibility = ViewStates.Gone;
                EditClassName.Text = model.TokType;
                EditClassSetName.Text = model.Name;
                EditDescription.Text = model.Description;
                txtReferenceId.Text = model.ReferenceId ?? "";
                // TextGroupName.ContentDescription = model.GroupId;
                Glide.With(this).Load(model.Image).Into(ImgClassSet);

                if (!string.IsNullOrEmpty(model.Image))
                {
                    ImgClassSet.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                toolbarSave.Text = "+ Post";
            }

            // BtnBrowse.Click += BtnBrowseImageClick;
            txtAddMainImage.Click += delegate
            {
                if (string.IsNullOrEmpty(txtAddMainImage.ContentDescription) || txtAddMainImage.ContentDescription == "+")
                {
                    OnClickAddTokImgMain(txtAddMainImage);
                }
                else if (txtAddMainImage.ContentDescription == "-")
                {
                    OnClickRemoveTokImgMain(txtAddMainImage);
                }
            };
            btnSelectColor.Click += delegate
            {
                if (!isSave)
                {
                    if (model.UserId != Settings.GetTokketUser().Id)
                    {
                        return;
                    }
                }

                var nextActivity = new Intent(this, typeof(ColorSelectionActivity));
                nextActivity.PutExtra("className", model.TokType);
                nextActivity.PutExtra("color", model.ColorMainHex);
                nextActivity.PutExtra("keyvalue", model.TokType);
                this.StartActivityForResult(nextActivity, COLOR_REQUEST_CODE);
            };

            if (!string.IsNullOrEmpty(model.ColorMainHex))
            {
                txtColor.SetBackgroundColor(Color.ParseColor(model.ColorMainHex));
            }
            ChkPublic.CheckedChange += chkpublic_changed;
            ChkGroup.CheckedChange += chkgroup_changed;
            LinearGroup.Visibility = ViewStates.Gone;

            //Handling the incoming content
            IncomingContent();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    new SimpleAlertMessageDialog(this, handlerOKText: "Keep Editing", handlerOkClick: null, handlerCancelText: "Discard Post", handlerCancel: (s, e) => {
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                        Finish();
                    }, animation: "lottie_exclamation.json").Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void IncomingContent()
        {
            // Get intent, action and MIME type    
            var intent = Intent;
            var action = intent.Action;
            var type = intent.Type;

            if (Intent.ActionSend == action && type != null)
            {
                if ("text/plain" == type)
                {
                    handleSendText(intent); // Handle text being sent    
                }
                else if (type.StartsWith("image/"))
                {
                    handleSendImage(intent); // Handle single image being sent    
                }
            }
        }
        private void handleSendText(Intent intent)
        {
            var sharedText = intent.GetStringExtra(Intent.ExtraText);
            if (sharedText != null)
            {
                EditClassSetName.Text = sharedText;
            }
        }
        private void handleSendImage(Intent intent)
        {
            var imageUri = (NetUri)intent.GetParcelableExtra(Intent.ExtraStream);
            if (imageUri != null)
            {
                Settings.ImageBrowseCrop = (string)imageUri;
                onClickImage(imageUri, (int)ActivityType.AddClassSetActivity);
            }
        }

        private void chkgroup_changed(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            model.Privacy = "Group";
            ChkPublic.Checked = false;
        }

        private void chkpublic_changed(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            model.Privacy = "Public";
            ChkGroup.Checked = false;
        }

        private async Task AddClassSetFunction()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            CancellationToken cancellationToken;

            model.UserId = UserId;

            if (!isSuperSet)
            {
                if (autoCompleteTokGroup.ContentDescription.ToLower() == "playable")
                {
                    model.IsPlayable = true;
                }
                else
                {
                    model.IsPlayable = false;
                }

                model.TokGroup = txtPlayableType.ContentDescription;
            }

            model.TokType = EditClassName.Text;
            //model.Privacy = SpinPrivacy.ContentDescription;
            model.Name = EditClassSetName.Text;
            model.Description = EditDescription.Text;
            model.ReferenceId = txtReferenceId.Text;

            if (!string.IsNullOrEmpty(ImgClassSet.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImgClassSet.ContentDescription))
                {
                    model.Image = "data:image/jpeg;base64," + ImgClassSet.ContentDescription;
                }
            }

            if (!string.IsNullOrEmpty(model.TokGroup))
            {
                model.GroupId = TextGroupName.ContentDescription;

                model.TokTypeId = $"toktype-{model.TokGroup.ToIdFormat()}-{model.TokType.ToIdFormat()}";
            }
            else { 
                model.TokTypeId = $"toktype-{"superset".ToIdFormat()}-{model.TokType.ToIdFormat()}";
            }
          
            if(!isSuperSet)
                TextProgress.Text = "Adding class set...";
            else
                TextProgress.Text = "Adding super set...";
            LinearProgress.Visibility = ViewStates.Visible;
            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

            bool result = true;
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;

                result = await ClassService.Instance.AddClassSetAsync(model, cancellationToken);

            }

            Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            if (!result)
            {
                showRetryDialog("Failed to save.");
            }
            else
            {
                string alertmessage = "";
                if (result)
                {
                    if (!isSuperSet)
                        alertmessage = "Class set added!";
                    else
                        alertmessage = "Super set added"; ;
                }
                else
                {
                    alertmessage = "Failed to save.";
                }

                var builder = new AlertDialog.Builder(Instance);
                builder.SetMessage(alertmessage);
                builder.SetTitle("");
                var dialog = (AlertDialog)null;
                builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                {
                    if (result == true)
                    {
                        MyClassTokSetsFragment.Instance.PassItemClassSetsFromAddClassSet(model);
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                        this.Finish();
                    }
                });
                dialog = builder.Create();
                dialog.Show();
                if (result)
                {
                    dialog.SetCanceledOnTouchOutside(false);
                }
            }
        }

        private async Task EditClassSetFunction()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            CancellationToken cancellationToken;

            if (autoCompleteTokGroup.ContentDescription != null)
            {
                if (autoCompleteTokGroup.ContentDescription.ToLower() == "playable")
                {
                    model.IsPlayable = true;
                }
                else
                {
                    model.IsPlayable = false;
                }
            }

            if (txtPlayableType.ContentDescription != null)
            {
                model.TokGroup = txtPlayableType.ContentDescription;
            }

            model.TokType = EditClassName.Text;
          //  model.Privacy = SpinPrivacy.ContentDescription;
            model.Name = EditClassSetName.Text;
            model.Description = EditDescription.Text;
            model.ReferenceId = txtReferenceId.Text;

            if (!string.IsNullOrEmpty(ImgClassSet.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImgClassSet.ContentDescription))
                {
                    model.Image = "data:image/jpeg;base64," + ImgClassSet.ContentDescription;
                }
                else
                {
                    model.Image = ImgClassSet.ContentDescription;
                }
            }

            model.GroupId = TextGroupName.ContentDescription;

            TextProgress.Text = "Updating class set...";
            LinearProgress.Visibility = ViewStates.Visible;
            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

            bool result = true;
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;

                result = await ClassService.Instance.UpdateClassSetAsync(model, cancellationToken);
            }

            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            if (!result)
            {
                showRetryDialog("Failed to update.");
            }
            else
            {
                string alertmessage = "";
                if (result)
                {
                    alertmessage = "Class set updated.";
                }
                else
                {
                    alertmessage = "Failed to update.";
                }

                var builder = new AlertDialog.Builder(this);
                builder.SetMessage(alertmessage);
                builder.SetTitle("");
                var dialog = (AlertDialog)null;
                builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                {
                    if (result)
                    {
                        MyClassTokSetsFragment.Instance.PassItemClassSetsFromAddClassSet(model, isSave);
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);

                        Intent = new Intent();
                        Intent.PutExtra("classsetModel", JsonConvert.SerializeObject(model));
                        SetResult(Result.Ok, Intent);
                        Finish();
                    }
                });
                dialog = builder.Create();
                dialog.Show();
                if (result)
                {
                    dialog.SetCanceledOnTouchOutside(false);
                }
            }
        }
        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(this)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {

                            })
                            .SetNegativeButton("Retry", async (_, args) =>
                            {
                                if (isSave)
                                {
                                    await AddClassSetFunction();
                                }
                                else
                                {
                                    await EditClassSetFunction();
                                }
                            })
                            .SetCancelable(false)
                            .Show();
        }

        private void LoadSpinners()
        {
            PrivacyList = new ObservableCollection<string>();
            PrivacyList.Clear();
            PrivacyList.Add("Public");
            PrivacyList.Add("Private");
        }
        private async Task LoadClassGroup()
        {
            var resultGroup = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues() { partitionkeybase = "classgroups", startswith = false, userid = UserId });
            RecyclerGroupList.ContentDescription = resultGroup.ContinuationToken;
            var classgroupResult = resultGroup.Results.ToList();

            foreach (var item in classgroupResult)
            {
                ClassGroupCollection.Add(item);
            }

            var adapterClassGroup = ClassGroupCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.addclasssetgroup_row);
            RecyclerGroupList.SetAdapter(adapterClassGroup);

            if (!isSave) //Edit
            {
                var resultRequest = ClassGroupCollection.FirstOrDefault(c => c.Id == model.GroupId);
                if (resultRequest != null) //If Edit
                {
                    TextGroupName.Text = resultRequest.Name;
                }
            }
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, ClassGroupModel model, int position)
        {
            var ClassGroupHeader = holder.FindCachedViewById<TextView>(Resource.Id.TextClassSetGroupName);
            var ClassGroupBody = holder.FindCachedViewById<TextView>(Resource.Id.TextClassSetGroupDescription);
            var LinearRow = holder.FindCachedViewById<LinearLayout>(Resource.Id.LinearClassSetGroupRow);

            LinearRow.Tag = position;
            ClassGroupHeader.Text = model.Name;
            ClassGroupBody.Text = model.Description;

            LinearRow.Click -= RowClicked;
            LinearRow.Click += RowClicked;
        }

        private void RowClicked(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as LinearLayout).Tag; } catch { position = int.Parse((string)(sender as LinearLayout).Tag); }
            TextGroupName.Text = ClassGroupCollection[position].Name;
            TextGroupName.ContentDescription = ClassGroupCollection[position].Id;
        }
        private void TokGroup_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            autoCompleteTokGroup.ContentDescription = autoCompleteTokGroup.Text;
            setArrPlayableType();

            txtPlayableType.ItemClick -= PlayableType_ItemClick;
            txtPlayableType.ItemClick += PlayableType_ItemClick;
            var ArrPlayableTypeAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, arrayPlayableType);
            txtPlayableType.Adapter = null;
            txtPlayableType.Adapter = ArrPlayableTypeAdapter;
            txtPlayableType.SetText(arrayPlayableType[0], false);
        }

        private void setArrPlayableType()
        {
            arrayPlayableType = new string[] { "" };
            if (autoCompleteTokGroup.Text == ArrTokGroup[0])
            {
                arrayPlayableType = ArrPlayableType;
            }
            else if (autoCompleteTokGroup.Text == ArrTokGroup[1])
            {
                arrayPlayableType = ArrNonPlayableType;
            }
        }
        private void PlayableType_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var tokGroup = txtPlayableType.Text.ToLower().Split(" ")[0];
            txtPlayableType.ContentDescription = tokGroup;
        }
        public void SpinPrivacy_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
           // SpinPrivacy.ContentDescription = SpinPrivacy.GetItemAtPosition(SpinPrivacy.FirstVisiblePosition).ToString();
        }

        private void BtnBrowseImageClick(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddClassSetActivity);
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddClassSetActivity);
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            v.ContentDescription = "";
            txtAddMainImage.Text = "Select an image.";
            Drawable img = ContextCompat.GetDrawable(this, Resource.Drawable.image_icon);
            txtAddMainImage.SetCompoundDrawablesWithIntrinsicBounds(img, null, null, null);

            ImgClassSet.Background = ContextCompat.GetDrawable(this, Resource.Drawable.linearboard_nopadding);
            ImgClassSet.SetImageBitmap(null);
            model.Image = null;
            ImgClassSet.ContentDescription = "";
            ImgClassSet.Visibility = ViewStates.Gone;
        }

        private void onClickImage(NetUri uri, int requestCode)
        {
            messageCropDialog = new MessageDialog(this, "Option", "", "Crop", "Save", (s, e) =>
            {
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                this.StartActivityForResult(nextActivity, requestCode);
                int vtag = Settings.BrowsedImgTag;
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddClassSetActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Settings.ImageBrowseCrop = (string)uri;

                onClickImage(uri, requestCode);
            }
            else if ((requestCode == (int)ActivityType.AddClassSetActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                StartActivityForResult(nextActivity, requestCode);
            }
            else if ((requestCode == COLOR_REQUEST_CODE) && (resultCode == Result.Ok) && (data != null))
            {
                var colorHex = data.GetStringExtra("color");
                model.ColorMainHex = colorHex;
                txtColor.SetBackgroundColor(Color.ParseColor(colorHex));
            }
        }
        public void displayImageBrowse()
        {
            byte[] imageByte = null;
            imageByte = Convert.FromBase64String(Settings.ImageBrowseCrop);
            model.Image = "data:image/jpeg;base64," +  Settings.ImageBrowseCrop;
            //ImgClassSet.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
            Glide.With(this).AsBitmap().Load(imageByte).Into(ImgClassSet);
            ImgClassSet.Visibility = ViewStates.Visible;
            Settings.ImageBrowseCrop = null;
        }
        public TextInputLayout txtInputLayoutTokGroup => FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutAddClassSetTokGroup);
        public TextInputLayout txtInputLayoutPlayableType => FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutPlayableType);
        public AutoCompleteTextView autoCompleteTokGroup => FindViewById<AutoCompleteTextView>(Resource.Id.txtAddClassSetTokGroup);
        public AutoCompleteTextView txtPlayableType => FindViewById<AutoCompleteTextView>(Resource.Id.txtPlayableType);
        public TextView TextTokGroupDescription => FindViewById<TextView>(Resource.Id.lblAddSetDescription);
        public EditText EditClassName => FindViewById<EditText>(Resource.Id.EditAddClassSetClassName);
        public TextView TextGroupName => FindViewById<TextView>(Resource.Id.TextGroupName);
        public RecyclerView RecyclerGroupList => FindViewById<RecyclerView>(Resource.Id.RecyclerGroupList);
        // public Spinner SpinPrivacy => FindViewById<Spinner>(Resource.Id.txtAddClassSetPrivacy);

        public CheckBox ChkPublic => FindViewById<CheckBox>(Resource.Id.chkPublic);
        public CheckBox ChkGroup => FindViewById<CheckBox>(Resource.Id.chkGroup);
        public TextInputLayout txtInputLayoutSetName => FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutSetName);
        public EditText EditClassSetName => FindViewById<EditText>(Resource.Id.txtAddClassSetName);
        public EditText EditDescription => FindViewById<EditText>(Resource.Id.txtAddClassSetDescription); 
        public ImageView ImgClassSet => FindViewById<ImageView>(Resource.Id.addClassset_imagebrowse);
        //public Button BtnBrowse => FindViewById<Button>(Resource.Id.btnAddClassSet_btnBrowseImage);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_addClasssetprogress);
        public TextView TextProgress => FindViewById<TextView>(Resource.Id.progressBarTextAddClassSet);
        public LinearLayout LinearGroup => FindViewById<LinearLayout>(Resource.Id.LinearGroup);
        public TextView classGroupName => FindViewById<TextView>(Resource.Id.classGroupName);
        public TextView txtColor => FindViewById<TextView>(Resource.Id.txtColor);
        public Button btnSelectColor => FindViewById<Button>(Resource.Id.btnSelectColor);
        public TextView SetTitleLabel => FindViewById<TextView>(Resource.Id.lblSetTitle);
        public TextView txtAddMainImage => FindViewById<TextView>(Resource.Id.txtAddMainImage);
        public EditText txtReferenceId => FindViewById<EditText>(Resource.Id.txtReferenceId);
        public AndroidX.AppCompat.Widget.Toolbar customToolbar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.includeHeaderLayout);
        public TextView txtToolbarTitle => customToolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
        public Button toolbarSave => customToolbar.FindViewById<Button>(Resource.Id.btnMenu);
    }
}