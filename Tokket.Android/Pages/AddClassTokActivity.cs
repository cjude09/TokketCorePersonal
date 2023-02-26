using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Helpers;
using SharedService = Tokket.Shared.Services;
using Tokket.Core;
using static Android.App.ActionBar;
using Result = Tokket.Shared.Helpers.Result;
using Tokket.Shared.Models;  
using Tokket.Android.Fragments;
using Newtonsoft.Json;
using Android.Graphics;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using System.Threading.Tasks;
using Android.Webkit;
using Tokket.Android.Helpers;
using System.Collections.ObjectModel;
using Android.Animation;
using Tokket.Core.Tools;
using Tokket.Shared.Services;
using static Android.Views.View;
using AndroidX.Core.Content;
using System.Threading;
using Color = Android.Graphics.Color;
using Android.Content.Res;
using System.IO;
using Android.Graphics.Drawables;
using Xamarin.Essentials;
using Android.Content.PM;
using Tokket.Android.Custom;
using Java.IO;
using Android.Util;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.AppCompat.Content.Res;
using Android.Views.InputMethods;
using DE.Hdodenhof.CircleImageViewLib;
using AppResult = Android.App.Result;
using Android.Provider;
using Settings = Tokket.Shared.Helpers.Settings;
using NetUri = Android.Net.Uri;
using ClipboardManager = Android.Content.ClipboardManager;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Google.Flexbox;
using GalaSoft.MvvmLight.Helpers;
using System.Collections;
using Tokket.Shared.Models.AlphaToks;

namespace Tokket.Android
{
    [Activity(Name = "TokketCore.Tokket.Android.AddClassTokActivity", Label = "@string/add_classtok", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddClassTokActivity : BaseActivity, View.IOnTouchListener, IOnLongClickListener
    {
        // Allows us to know if we should use MotionEvent.ACTION_MOVE
        private bool tracking = false;
        // The Position where our touch event started
        private float startY;
        internal static AddClassTokActivity Instance { get; private set; }
        ObservableCollection<AddTokDetailModel> DetailCollection { get; set; }
        ObservableCollection<TokSection> MegaCollection { get; set; }
        ObservableCollection<TokSection> QNACollection { get; set; }
        bool isSave = true,isOptioanlsShown = false; bool[] ArrAnswer; TokketUser tokketUser;
        List<ClassTokModel> classTokLinkDetails;
        ClassTokModel classTokModel, classTokPaste, classTokMainPaste; string classGroupId = ""; int selectedGroupPosition = 0;
        ClassGroupModel ClassGroupModel;
        ObservableCollection<ClassGroupModel> ClassGroupCollection;
        Dialog popupGroupDialog; GestureDetector gesturedetector;
        LinearLayout linearDialogParent;
        Dialog tileDialog;
        int COLOR_REQUEST_CODE = 1001;
        int SEARCH_TOKS_REQUEST_CODE = 1002;
        string detailImageSelected;
        Bundle bundle = new Bundle();
        private Dialog messageCropDialog;
        private bool isSecondaryImage = false;
        public List<bool> IsIndent { get; set; } = new List<bool>() { false, false, false, false, false, false, false, false, false, false };
        public List<bool> IsImageTokPakVisible = new List<bool> { true, true, true, true, true, true, true, true, true, true };
        int basicPosition = 0, picPosition = 1, detailPosition = 2, listPosition = 3, megaPosition = 4, qNaPosition = 5;

        string[] arryTokGroup = new string[] {"Basic (Only 1 Detail, playable)",
                "Pic (Basic with only picture on tile)",
                "Detailed (2-10 Details, playable)",
                "List (Unlimited Details, not playable)",
                "Mega (Unlimited Sections of 150k chars, not playable)",
                "Q&A (V2) (Unlimited Details, not playable)" };
        string[] arryBulletType = new string[] { "Bullets", "Numbers", "Letters", "None" };

        private void setSettingsCurrentInt()
        {
            Settings.ActivityInt = (int)ActivityType.AddClassTokActivity;
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addclasstok_page);

            setSettingsCurrentInt();
            Instance = this;
            SetSupportActionBar(customToolbar);

            customToolbar.SetBackgroundColor(defaultPrimaryColor);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            txtToolbarTitle.Text = "Add Class Tok";

            gesturedetector = new GestureDetector(this, new MyGestureListener(this));

            tokketUser = await SharedService.AccountService.Instance.GetUserAsync(Settings.GetUserModel().UserId);

            ResetAll();
            classTokLinkDetails = new List<ClassTokModel>();
            classTokLinkDetails.AddRange(Enumerable.Repeat(new ClassTokModel(), 10).ToList());

            classTokPaste = new ClassTokModel();
            classTokMainPaste = new ClassTokModel();

            chkPublic.Checked = false;

            this.SetBinding(() => classTokModel.TokType, () => TokType.Text, BindingMode.TwoWay);
            this.SetBinding(() => classTokModel.Category, () => Category.Text, BindingMode.TwoWay);
            this.SetBinding(() => classTokModel.PrimaryFieldText, () => Primary.Text, BindingMode.TwoWay);
            this.SetBinding(() => classTokModel.SecondaryFieldText, () => Secondary.Text, BindingMode.TwoWay);
            this.SetBinding(() => classTokModel.Notes, () => Notes.Text, BindingMode.TwoWay);

            //Load TokMoji
            RecyclerDetail.SetLayoutManager(new LinearLayoutManager(this));

            var Aadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, arryTokGroup);
            TokGroup.Adapter = Aadapter;
            TokGroup.ItemClick += TokType_ItemClick;

            BulletType.ItemClick += BulletType_ItemClick;
            var BulletAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, arryBulletType);
            BulletType.Adapter = BulletAdapter;

            isSave = Intent.GetBooleanExtra("isSave", true);
            string classgroupmodelstr = Intent.GetStringExtra("ClassGroupModel");
            if (classgroupmodelstr != null)
            {
                ClassGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(classgroupmodelstr);
                if (ClassGroupModel != null)
                {
                    classTokModel.GroupId = ClassGroupModel.Id;
                }
            }
            
            if (isSave == false)
            {
                this.RunOnUiThread(async () => await EditClassTok());
            }
            else
            {
                classTokModel.TokGroup = "Basic"; //Default
                toolbarSave.Text = "+ Post";

#if (_CLASSTOKS)
                var linearGroup = FindViewById<LinearLayout>(Resource.Id.linearClassGroup);
                var txtClassGroup = FindViewById<TextView>(Resource.Id.txtClassGroup);
                if (MainActivity.Instance.tabLayout.SelectedTabPosition == 0 || MainActivity.Instance.tabLayout.SelectedTabPosition == 1) //Home || Search
                {
                    if (Settings.FilterFeed == (int)FilterType.All)
                    {
                        if (ClassGroupModel != null)
                        {
                            chkPublic.Checked = false;
                            chkGroup.Checked = true;
                            chkGroup.Enabled = false;
                            chkPublic.Enabled = false;
                            txtClassGroup.Text = ClassGroupModel.Name;
                            classGroupId = ClassGroupModel.Id;
                        }
                        else {
                            chkPublic.Checked = true;
                            linearGroup.Visibility = ViewStates.Gone;
                        }
                      
                    }
                   
                    else
                    {
                        //chkPrivate.Checked = true;
                    }
                }
                else if (MainActivity.Instance.tabLayout.SelectedTabPosition == 2) //Profile
                {
                    //chkPrivate.Checked = true;
                }
#endif
            }

            //If Save Button is clicked
            toolbarSave.Click += SaveClassTok_IsClicked;

            //If save multiple toks is clicked
            btnAddMultipleToks.Click += SaveMultipleToks_IsClicked;
            //chkPublic.Checked = ClassTokModel.IsPublic;
            //chkPrivate.Checked = ClassTokModel.IsPrivate;
            //chkGroup.Checked = ClassTokModel.IsGroup;
            txtChkPublic.SetOnTouchListener(this);
            txtChkGroup.SetOnTouchListener(this);
            chkPublic.Click += delegate
            {
                classTokModel.IsPublic = chkPublic.Checked;
                chkTokChannel.Checked = chkTokChannel.Checked;
                //changeSaveText();
            };

            classTokModel.IsPrivate = true;
            chkTokChannel.Click += delegate {
                chkPublic.Checked = true;
                classTokModel.IsPublic = chkPublic.Checked;
            };
            chkGroup.Click += delegate
            {
                classTokModel.IsGroup = chkGroup.Checked;
                //changeSaveText();
            };

            btnSelectColor.Click += delegate
            {
                if (!isSave)
                {
                    if (classTokModel.UserId != Settings.GetTokketUser().Id)
                    {
                        return;
                    }
                }
                var nextActivity = new Intent(this, typeof(ColorSelectionActivity));
                nextActivity.PutExtra("className", classTokModel.TokType);
                nextActivity.PutExtra("color", classTokModel.ColorMainHex);
                nextActivity.PutExtra("keyvalue", classTokModel.TokType);
                this.StartActivityForResult(nextActivity, COLOR_REQUEST_CODE);
            };

            btnPasteTokLink.Click += async(sender, e) =>
            {
                if (classTokModel.TokLink == null)
                {
                    await pasteTok_Click(true, btnPasteTokLink, btnSearchForToks, btnRemovePaste);
                }
                else
                {
                    goToTokInfo(classTokMainPaste);
                }
            };

            btnRemovePaste.Click += delegate
            {
                classTokModel.TokLink = null;
                btnPasteTokLink.Text = "Paste Tok Link";
                btnSearchForToks.Visibility = ViewStates.Visible;
                btnRemovePaste.Visibility = ViewStates.Gone;
            };

            btnCloseTipNote.Click += delegate
            {
                txtTipNotes.Visibility = ViewStates.Gone;
                circleImageViewTipNote.Visibility = ViewStates.Gone;
                btnCloseTipNote.Visibility = ViewStates.Gone;
            };

            btnSearchForToks.Click += delegate
            {
                goToSearchToks(true, -1);
            };

            btnPreviewTile.Click += btnPreview_IsClicked;

            if (classTokModel != null)
            {
                txtColor.SetBackgroundColor(Color.ParseColor(classTokModel.ColorMainHex));
            }

            txtAddMainImage.Click += delegate
            {
                isSecondaryImage = false;
                if (string.IsNullOrEmpty(txtAddMainImage.ContentDescription) || txtAddMainImage.ContentDescription == "+")
                {
                    OnClickAddTokImgMain(txtAddMainImage);
                }
                else if (txtAddMainImage.ContentDescription == "-")
                {
                    OnClickRemoveTokImgMain(txtAddMainImage);
                }
            };

            txtSecondaryImage.Click += delegate
            {
                isSecondaryImage = true;
                if (string.IsNullOrEmpty(txtSecondaryImage.ContentDescription) || txtSecondaryImage.ContentDescription == "+")
                {
                    OnClickSecondaryImage(txtSecondaryImage);
                }
                else if (txtSecondaryImage.ContentDescription == "-")
                {
                    OnClickRemoveSecondaryImage(txtSecondaryImage);
                }
            };

            BtnAutoPaste.Click += delegate {
                try
                {
                    string paste = "";
                    if (!string.IsNullOrEmpty(txtAutoPaste.Text.Trim()))
                    {
                        paste = txtAutoPaste.Text;
                        for (int i = 0; i < DetailCollection.Count; i++)
                        {
                            DetailCollection[i].Detail = paste;
                        }
                    }
                    else
                    {
                        var manager = GetSystemService(Context.ClipboardService) as ClipboardManager;
                        ClipData pasteData = manager.PrimaryClip;
                        if (pasteData != null)
                        {
                            ClipData.Item item = pasteData.GetItemAt(0);
                            paste = item.Text;

                            var detailList = paste.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < detailList.Length; i++)
                            {

                                if (i < DetailCollection.Count && string.IsNullOrEmpty(DetailCollection[i].Detail))
                                {
                                    DetailCollection[i].Detail = detailList[i];
                                }
                                else if (detailList.Length > DetailCollection.Count)
                                {
                                    var tokdetailModel = new AddTokDetailModel();
                                    tokdetailModel.Detail = detailList[i];
                                    DetailCollection.Add(tokdetailModel);
                                }

                                if (DetailCollection.Count == 10)
                                {
                                    TokStarDetail();
                                    break;
                                }
                            }
                        }
                    }

                    SetDetailRecyclerAdapter();
                }
                catch (Exception ex)
                {
                }
            };

            //Secondary.Hover += ItemOnHoverToolTip;
            Secondary.FocusChange += ItemOnFocusChange;
            ShowHideOptional.Click += ShowHideOptionalClick;
            BtnRemoveSticker.Click += RemoveStickerClick;
            if (isSave == false)
            {

            }
            else
            {
                TokGroup.SetText(arryTokGroup[0], false); //Set as default
            }
            constraintOptionalFields.Visibility = ViewStates.Gone;

            txtShowHideAutoPaste.Click += delegate
            {
                if (txtShowHideAutoPaste.Text == "Show Auto Paste Fields")
                {
                    txtShowHideAutoPaste.Text = "Hide Auto Paste Fields";
                    txtShowHideAutoPaste.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_upward_24, 0, 0, 0);
                    LinearDetail.Visibility = ViewStates.Visible;
                }
                else if (txtShowHideAutoPaste.Text == "Hide Auto Paste Fields")
                {
                    txtShowHideAutoPaste.Text = "Show Auto Paste Fields";
                    txtShowHideAutoPaste.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_downward_24, 0, 0, 0);
                    LinearDetail.Visibility = ViewStates.Gone;
                }
            };
            
            hideShowAllSections.Click += delegate
            {
                if (hideShowAllSections.Text.ToLower() == "show all sections")
                {
                    hideShowAllSections.Text = "Hide All Sections";
                    hideShowAllSections.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_upward_24, 0, 0, 0);
                    RecyclerDetail.Visibility = ViewStates.Visible;
                }
                else if (hideShowAllSections.Text.ToLower() == "hide all sections")
                {
                    hideShowAllSections.Text = "Show All Sections";
                    hideShowAllSections.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_downward_24, 0, 0, 0);
                    RecyclerDetail.Visibility = ViewStates.Gone;
                }
            };

            btnAddClassTokAddContent.Click += delegate
            {
                var TokQnAItem = new TokSection();
                TokQnAItem.Id = "";

                var QnAItem = new List<Qna>();
                var item = new Qna();
                QnAItem.Add(item);

                TokQnAItem.QuestionAnswer = QnAItem;

                QNACollection.Add(TokQnAItem);

                var adapterMega = QNACollection.GetRecyclerAdapter(BindAddClassTokQandA, Resource.Layout.item_q_n_a_content);
                RecyclerDetail.SetAdapter(adapterMega);
            };

            //Handling the incoming content
            IncomingContent();
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
            else if (Intent.ActionSendMultiple == action && type != null)
            {
                if (type.StartsWith("image/"))
                {
                    handleSendMultipleImages(intent); // Handle multiple images being sent    
                }
            }
            else
            {
                // Handle other intents
            }
        }

        private void handleSendText(Intent intent)
        {
            var sharedText = intent.GetStringExtra(Intent.ExtraText);
            if (sharedText != null)
            {
                var detailList = sharedText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (detailList.Length == 1)
                {
                    TokGroup.SetText(arryTokGroup[basicPosition], false);
                }
                else if (detailList.Length <= 10)
                {
                    TokGroup.SetText(arryTokGroup[detailPosition], false);
                }
                else if (detailList.Length > 10)
                {
                    TokGroup.SetText(arryTokGroup[listPosition], false);
                }

                var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];
                classTokModel.TokGroup = tokGroup;

                if (detailList.Length > 1)
                {
                    for (int i = 0; i < detailList.Length; i++)
                    {

                        if (i < DetailCollection.Count && string.IsNullOrEmpty(DetailCollection[i].Detail))
                        {
                            DetailCollection[i].Detail = detailList[i];
                        }
                        else if (detailList.Length > DetailCollection.Count)
                        {
                            var tokdetailModel = new AddTokDetailModel();
                            tokdetailModel.Detail = detailList[i];
                            DetailCollection.Add(tokdetailModel);
                        }
                    }

                    SetDetailRecyclerAdapter();
                }
                else
                {
                    Primary.Text = detailList[0];
                }
            }
        }

        private void handleSendImage(Intent intent)
        {
            var imageUri = (NetUri)intent.GetParcelableExtra(Intent.ExtraStream);
            if (imageUri != null)
            {
                TokGroup.SetText(arryTokGroup[basicPosition], false);
                
                var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];
                classTokModel.TokGroup = tokGroup;

                Settings.ImageBrowseCrop = (string)imageUri;

                onClickImage(imageUri, (int)ActivityType.AddTokActivityType);
            }
        }

        System.Collections.IList imageUriList;
        List<string> imageList;
        public System.Collections.IList GetImageUris()
        {
            return imageUriList;
        }

        public List<string> GetImageBase64()
        {
            return imageList;
        }

        private void handleSendMultipleImages(Intent intent)
        {
            imageList = new List<string>();
            imageUriList = intent.GetParcelableArrayListExtra(Intent.ExtraStream);
            if (imageUriList != null)
            {
                foreach (var item in imageUriList)
                {
                    var photoUri = (NetUri)item;

                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, photoUri);

                    MemoryStream outputStream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, outputStream);
                    byte[] byteArray = outputStream.ToArray();

                    imageList.Add(Base64.EncodeToString(byteArray, Base64Flags.Default));
                }

                messageCropDialog = new MessageDialog(this, "Option", "", "Crop All", "Save All", (s, e) =>
                {
                    intent = new Intent(this, typeof(SharedUrisActivity));
                    StartActivity(intent);
                },
                (s, e) =>
                {
                    SetAllSharedImages();
                });
                messageCropDialog.Show();
            }
        }

        public void UpdateImageBase64List(List<string> newImageList)
        {
            imageList = newImageList;
        }

        public void SetAllSharedImages()
        {
            setSettingsCurrentInt();
            if (imageList.Count <= 10)
            {
                TokGroup.SetText(arryTokGroup[detailPosition], false);
            }
            else if (imageList.Count > 10)
            {
                TokGroup.SetText(arryTokGroup[listPosition], false);
            }

            var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];
            classTokModel.TokGroup = tokGroup;

            for (int i = 0; i < imageList.Count; i++)
            {
                if (i < DetailCollection.Count && string.IsNullOrEmpty(DetailCollection[i].Detail))
                {
                    DetailCollection[i].Detail = "";
                }
                else if (imageList.Count > DetailCollection.Count)
                {
                    var tokdetailModel = new AddTokDetailModel();
                    tokdetailModel.Detail = "";
                    DetailCollection.Add(tokdetailModel);
                }
            }

            classTokModel.DetailImages = imageList.ToArray();

            SetDetailRecyclerAdapter();
        }

        private void RemoveStickerClick(object sender, EventArgs e)
        {
            classTokModel.Sticker = string.Empty; ;
            classTokModel.StickerImage = string.Empty; ;
            StickerImage.SetBackgroundResource(0);
            BtnAddTileSticker.Visibility = ViewStates.Visible;
            StickerImage.Visibility = ViewStates.Gone;
            BtnRemoveSticker.Visibility = ViewStates.Gone;
        }

        private void ShowHideOptionalClick(object sender, EventArgs e)
        {
            var textView = sender as TextView;
            var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];

            imageBtnAttachment.Visibility = ViewStates.Gone;
            SourceUrl.Hint = "Source URL";
            if (isOptioanlsShown)
            {
                constraintOptionalFields.Visibility = ViewStates.Gone;
                if (tokGroup == "basic" || tokGroup == "pic")
                {
                    SourceNote.Visibility = ViewStates.Gone;
                }
                textView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_downward_24,0,0,0);
                isOptioanlsShown = false;
                textView.Text = "Show Optional Fields";

            }
            else {
                constraintOptionalFields.Visibility = ViewStates.Visible;
                
                if (tokGroup == "basic" || tokGroup == "pic")
                {
                    SourceNote.Visibility = ViewStates.Visible;
                }
                else if (tokGroup == "mega" || tokGroup == "q&a")
                {
                    imageBtnAttachment.Visibility = ViewStates.Visible;
                    SourceUrl.Hint = "Link";
                }
                textView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_arrow_upward_24, 0, 0, 0);
                isOptioanlsShown = true;
                textView.Text = "Hide Optional Fields";
            }
           
        }

        //private void ItemOnHoverToolTip(object sender, HoverEventArgs e)
        //{
        //    var balloon = showTooltip(this, "The detail can show up to 600 characters");
        //    balloon.ShowAlignTop(sender as View);
        //}

        private void ItemOnFocusChange(object sender, FocusChangeEventArgs e)
        {
            var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];
            switch (tokGroup)
            {
                case "basic":
                    if (!e.HasFocus)
                        break;
                   customToolTip(this, "The Detail can show up to 600 characters.\n If you have more content then select the detailed tok type which can have to 10 details for up to 600 characters each.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;

                case "pic":
                    if (!e.HasFocus)
                        break;
                    customToolTip(this, "The Detail can show up to 600 characters.\n If you have more content then select the detailed tok type which can have to 10 details for up to 600 characters each.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;

                case "detailed":
                    if (!e.HasFocus)
                        break;
                    customToolTip(this, "There can be up to 10 details with up to 600 characters per detail.\n If you have more content then add more detailed toks or select the mega tok type.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;

                case "list":
                    if (!e.HasFocus)
                        break;
                    customToolTip(this, "Unlimited details, not playable.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;

                case "mega":
                    if (!e.HasFocus)
                        break;
                    customToolTip(this, "There can be unlimited sections with 100,000 characters per section.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;
                case "q&a":
                    customToolTip(this, "There can be up to 10 Details with up to 600 characters per detail. If you have more content, then add more detailed toks or select the mega tok type.", Resource.Drawable.tokstarguy2, (sender as View));
                    break;
            }
        }

        private void btnPreview_IsClicked(object sender, EventArgs e)
        {
            tileDialog = new Dialog(this);
            tileDialog.SetContentView(Resource.Layout.listview_row);
            tileDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            tileDialog.Show();

            tileDialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            tileDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            assignValuesPreviewTile();

        }

        private void SaveMultipleToks_IsClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editTextMultipleToks.Text))
            {
                dialogBoxOk("Requires text to be entered in the fieldbox.");
                return;
            }

            if (string.IsNullOrEmpty(Category.Text.Trim()))
            {
                dialogBoxOk("Category is required.");
                return;
            }
            
            if (string.IsNullOrEmpty(TokType.Text.Trim()))
            {
                dialogBoxOk("Class name is required.");
                return;
            }

            string textToDisplay = "";
            var multipleToks = editTextMultipleToks.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var separator = editTextSeparator.Text;
            if (string.IsNullOrEmpty(editTextSeparator.Text))
            {
                separator = "\t";
            }

            for (int i = 0; i < multipleToks.Length; i++)
            {
                textToDisplay += multipleToks[i];

                if (i < multipleToks.Length - 1)
                {
                    var splitSeparator = separator.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitSeparator.Length > 0)
                    {
                        foreach (var item in splitSeparator)
                        {
                            if (item == "n")
                            {
                                textToDisplay += "\n";
                            }
                            else if (item == "t")
                            {
                                textToDisplay += "\t";
                            }
                            else
                            {
                                textToDisplay += item;
                            }
                        }
                    }
                    else
                    {
                        textToDisplay += separator;
                    }
                }
            }

            var resultText = textToDisplay;

            //Hide Keyboard
            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow((sender as View).WindowToken, HideSoftInputFlags.None);

            AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("Create multiple toks with;");
            alertDiag.SetMessage(textToDisplay);
            alertDiag.SetPositiveButton("Confirm", async(senderAlert, args) => {
                showProgress();

                List<ClassTokModel> classTokList = new List<ClassTokModel>();

                for (int i = 0; i < multipleToks.Length; i++)
                {
                    ProgressText.Text = $"Saving {i + 1} of {multipleToks.Length}";
                    classTokModel.PrimaryFieldText = multipleToks[i];
                    var result = await saveMultipleToks();

                    if (result.ResultEnum == Result.Success)
                    {
                        var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                        var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
                        classTokList.Add(desClassTokResult);
                    }

                    if (i == multipleToks.Length - 1)
                    {
                        if (classTokList.Count > 0)
                        {

                            ClassToksFragment.Instance.AddNewMultipleToks(classTokList);

                            this.Finish();
                        }
                    }
                }

                hideProgress();
            });
            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Cancel</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        private async Task<ResultModel> saveMultipleToks()
        {
            string ClassTokGroup = TokGroup.Text.Split(" ")[0];
            if (!string.IsNullOrEmpty(SourceNote.Text))
                classTokModel.SourceNotes = SourceNote.Text;

            classTokModel.ReferenceId = txtReferenceId.Text;
            classTokModel.IsPublic = chkPublic.Checked;
            classTokModel.IsPrivate = true; //chkPrivate.Checked;
            classTokModel.IsGroup = chkGroup.Checked;
            classTokModel.GroupId = classGroupId;
            if (string.IsNullOrEmpty(classTokModel.AccountType))
            {
                classTokModel.AccountType = "individual";
            }
            else
            {
                classTokModel.AccountType = "group";
            }

            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageDisplay.ContentDescription))
                {
                    classTokModel.Image = "data:image/jpeg;base64," + ImageDisplay.ContentDescription;
                }
            }

            if (!string.IsNullOrEmpty(ImageSecondary.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageSecondary.ContentDescription))
                {
                    classTokModel.SecondaryImage = "data:image/jpeg;base64," + ImageSecondary.ContentDescription;
                }
            }

            if (isSave) //If Add
            {
                classTokModel.Level1 = Intent.GetStringExtra("level1");
                classTokModel.Level2 = Intent.GetStringExtra("level2");
                classTokModel.Level3 = Intent.GetStringExtra("level3");
                classTokModel.TokGroup = ClassTokGroup;
                classTokModel.TokType = TokType.Text;
                classTokModel.CategoryId = "classtokscategory-" + classTokModel.Category?.ToIdFormat();
                classTokModel.TokTypeId = $"toktype-{classTokModel.TokGroup?.ToIdFormat()}-{classTokModel.TokType?.ToIdFormat()}";
                classTokModel.UserState = Settings.GetTokketUser().State;
                classTokModel.IsIndent = IsIndent;
                classTokModel.ImagesIsTokPakVisible = IsImageTokPakVisible;
            }

            ResultModel result = new ResultModel();
            var taskCompletionSource = new TaskCompletionSource<ResultModel>();
            CancellationToken cancellationToken;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
            cancellationToken = cancellationTokenSource.Token;

            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });

            if (isSave)
            {
                //API
                classTokModel.Label = "classtok";

#if RELEASE

                result = await ClassService.Instance.AddClassToksAsync(classTokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    //showRetryDialog("Task was cancelled.");
                }
                else
                {
                    await MainActivity.Instance.RefreshAccount();
                    var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                    var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
                    classTokModel = desClassTokResult;
                    ClassToksFragment.Instance.UpdateClassTokCollection(classTokModel);
                }
#elif DEBUG

                result = await ClassService.Instance.AddClassToksAsync(classTokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    return result;
                    //showRetryDialog("Task was cancelled.");
                }
                else
                {
                    await MainActivity.Instance.RefreshAccount();
                }
#endif
            }

            return result;
        }

        private async void SaveClassTok_IsClicked(object sender, EventArgs e)
        {
            string ClassTokGroup = TokGroup.Text.Split(" ")[0];
            if (classTokModel.TokGroup == null)
            {
                classTokModel.TokGroup = ClassTokGroup;
            }

            if (chkGroup.Checked && string.IsNullOrEmpty(classGroupId))
            {
                dialogBoxOk("Group is checked and no Class Group is selected. Double tap the word GROUP next to checkbox to view the list of groups.");
                return;
            }

            if (ClassTokGroup.ToLower() == "pic" && string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                dialogBoxOk("Tok Group Pic requires an image to be attached.");
                return;
            }

            if (txtReferenceId.Text.Contains(" "))
            {
                dialogBoxOk("Reference Id should not contain a space.");
                return;
            }

            if (classTokModel.TokGroup == "Basic" && string.IsNullOrEmpty(classTokModel.SecondaryFieldText))
            {
                dialogBoxOk("Missing field. Add \"Detail\"");
                return;
            }

            /*if (!chkPublic.Checked && !chkGroup.Checked)
            {
                dialogBoxOk("Please select a feed.");
                return;
            }*/
            var url = SourceUrl.Text;
            Uri uriResult;
            if (!string.IsNullOrEmpty(url)) {
                if (URLUtil.IsValidUrl(url))
                    classTokModel.SourceLink = url;
                else {
                    //dialogBoxOk("Invalid url, please check again.");
                    ShowLottieMessageDialog(this, "Invalid url, please check again.", false);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(SourceNote.Text))
                classTokModel.SourceNotes = SourceNote.Text;

            classTokModel.ReferenceId = txtReferenceId.Text;
            classTokModel.IsPublic = chkPublic.Checked;
            classTokModel.IsPrivate = true; //chkPrivate.Checked;
            classTokModel.IsGroup = chkGroup.Checked;
            classTokModel.GroupId = classGroupId;
            if (string.IsNullOrEmpty(classTokModel.AccountType))
            {
                classTokModel.AccountType = "individual";
            }
            else
            {
                classTokModel.AccountType = "group";
            }


            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageDisplay.ContentDescription))
                {
                    classTokModel.Image = "data:image/jpeg;base64," + ImageDisplay.ContentDescription;
                }
            }

            if (!string.IsNullOrEmpty(ImageSecondary.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageSecondary.ContentDescription))
                {
                    classTokModel.SecondaryImage = "data:image/jpeg;base64," + ImageSecondary.ContentDescription;
                }
            }

            if (isSave) //If Add
            {
                if (chkMasterCopy.Checked)
                {
                    classTokModel.HasComments = false;
                    classTokModel.HasReactions = false;
                }

                classTokModel.Level1 = Intent.GetStringExtra("level1");
                classTokModel.Level2 = Intent.GetStringExtra("level2");
                classTokModel.Level3 = Intent.GetStringExtra("level3");
                classTokModel.TokGroup = ClassTokGroup;
                classTokModel.TokType = TokType.Text;
                classTokModel.CategoryId = "classtokscategory-" + classTokModel.Category?.ToIdFormat();
                classTokModel.TokTypeId = $"toktype-{classTokModel.TokGroup?.ToIdFormat()}-{classTokModel.TokType?.ToIdFormat()}";
                classTokModel.UserState = Settings.GetTokketUser().State;
                classTokModel.IsIndent = IsIndent;
                classTokModel.ImagesIsTokPakVisible = IsImageTokPakVisible;
            }
            else
            {
                if (chkMasterCopy.Checked && classTokModel.IsMasterCopy)
                {
                    //Edit -> if checked then set below variables to true else, just a normal edit
                    classTokModel.HasComments = true;
                    classTokModel.HasReactions = true;

                    //instead of normal edit, this will save new copy
                    isSave = true;

                    var replacer = new ClassTokModel();
                    classTokModel.Id = replacer.Id;
                    classTokModel.CreatedTime = replacer.CreatedTime;
                    classTokModel.DateCreated = replacer.DateCreated;
                    classTokModel.Timestamp = replacer.Timestamp;
                }
            }

            if (ClassTokGroup.ToLower() == "detailed" || ClassTokGroup.ToLower() == "list")
            {
                List<string> detailList = new List<string>();
                classTokModel.Details = new string[DetailCollection.Count];
                for (int d = 0; d < DetailCollection.Count; d++)
                {
                    detailList.Add(DetailCollection[d].Detail);

                }
                classTokModel.Details = detailList.ToArray();
            }

            ResultModel result = new ResultModel();
            showProgress();

            //Image
            if (classTokModel.DetailImages != null)
            {
                for (int i = 0; i < classTokModel.DetailImages.Length; i++)
                {
                    if (!string.IsNullOrEmpty(classTokModel.DetailImages[i]))
                    {
                        if (!URLUtil.IsValidUrl(classTokModel.DetailImages[i]))
                        {
                            classTokModel.DetailImages[i] = "data:image/jpeg;base64," + classTokModel.DetailImages[i];
                        }
                    }
                }
            }

            var taskCompletionSource = new TaskCompletionSource<ResultModel>();
            CancellationToken cancellationToken;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
            cancellationToken = cancellationTokenSource.Token;

            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });

            if (isSave)
            {
                //API
                classTokModel.Label = "classtok";
                ProgressText.Text = "Saving...";

#if RELEASE

                result = await ClassService.Instance.AddClassToksAsync(classTokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    return;
                    //showRetryDialog("Task was cancelled.");
                }
                else
                {
                    await MainActivity.Instance.RefreshAccount();
                    var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                    var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
                    classTokModel = desClassTokResult;
                    Intent intent = new Intent();
                    intent.PutExtra("classtokModel", serClassTokResult);
                    SetResult(AppResult.Ok, intent);
                }

            //result = await ClassService.Instance.AddClassToksAsync(classTokModel, cancellationToken);
            //    if (result.ResultMessage == "cancelled")
            //    {
            //        ShowLottieMessageDialog(this, "Task was cacnelled.", false);
            //        //showRetryDialog("Task was cancelled.");
            //    }
            //    else
            //    {
            //        await MainActivity.Instance.RefreshAccount();
            //        var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
            //        var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
            //        classTokModel = desClassTokResult;
            //        Intent intent = new Intent();
            //        intent.PutExtra("classtokModel", serClassTokResult);
            //        SetResult(Android.App.Result.Ok, intent);
            //    }
#elif DEBUG

                result = await ClassService.Instance.AddClassToksAsync(classTokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    hideProgress();
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    return;
                    //showRetryDialog("Task was cancelled.");
                }
                else if (result.ResultEnum == Result.Failed)
                {
                    hideProgress();
                    ShowLottieMessageDialog(this, "Failed. " + result.ResultMessage, false);
                    return;
                }
                else
                {
                    await MainActivity.Instance.RefreshAccount();
                    var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                    var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
                    classTokModel = desClassTokResult;
                    Intent intent = new Intent();
                    intent.PutExtra("classtokModel", serClassTokResult);
                    SetResult(AppResult.Ok, intent);
                }

                //var test =    await Tokket.Shared.IoC.AppContainer.Resolve<Tokket.Shared.Services.Interfaces.IClassTokService>().AddClassTokAsync<Dictionary<string, object>>(classTokModel);
                //    if (test.StatusCode == System.Net.HttpStatusCode.OK)
                //    {
                //        var stringCon = JsonConvert.SerializeObject(test.Result);
                //        var convert = JsonConvert.DeserializeObject<ClassTokModel>(stringCon);
                //    }
                //    else {
                //        var failed = "Did not work";
                //    }
#endif


            }
            else
            {
                //API
                ProgressText.Text = "Updating...";

#if RELEASE
        
                result = await ClassService.Instance.UpdateClassToksAsync(classTokModel, cancellationToken);

                if (result.ResultMessage == "cancelled")
                {
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    //showRetryDialog("Task was cancelled.");
                }
#elif DEBUG
                result = await ClassService.Instance.UpdateClassToksAsync(classTokModel, cancellationToken);

                if (result.ResultMessage == "cancelled")
                {
                    hideProgress();
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                    return;
                    //showRetryDialog("Task was cancelled.");
                }

                //var test = await Tokket.Shared.IoC.AppContainer.Resolve<Tokket.Shared.Services.Interfaces.IClassTokService>().UpdateClassTokAsync<Dictionary<string, object>>(classTokModel);
                //if (test.StatusCode == System.Net.HttpStatusCode.OK)
                //{
                //    var stringCon = JsonConvert.SerializeObject(test.Result);
                //    var convert = JsonConvert.DeserializeObject<ClassTokModel>(stringCon);
                //}
                //else
                //{
                //    var failed = "Did not work";
                //}
#endif
            }

            //Saving Sections
            if (result.ResultEnum == Result.Success)
            {
                if (ClassTokGroup.ToLower() == "mega") //If Mega
                {
                    ResultData<TokSection> OrigTokSectionResult = null;
                    IEnumerable<TokSection> OrigTokSection = null;
                    if (!isSave)
                    {
                        OrigTokSectionResult = await TokService.Instance.GetTokSectionsAsync(classTokModel.Id);
                        OrigTokSection = OrigTokSectionResult.Results;
                    }

                    bool isSuccess = false;
                    var cnt = 0;
                    
                    foreach (var sec in MegaCollection)
                    {
                        //Progress Text
                        Thread.Sleep(200);
                        double val1 = (double)(cnt + 1) / (double)MegaCollection.Count;
                        var val2 = val1 * 100;
                        int percent = (int)val2;
                        ProgressText.Text = percent.ToString() + " %";

                        if (isSave)
                        {
                            sec.Id = null;
                        }

                        if (string.IsNullOrEmpty(sec.Id)) //Save
                        {
                            var dummySec = new TokSection();
                            sec.Id = dummySec.Id;
                            sec.TokId = classTokModel.Id;
                            sec.TokTypeId = classTokModel.TokTypeId;
                            sec.UserId = Settings.GetUserModel().UserId;

                            isSuccess = await TokService.Instance.CreateTokSectionAsync(sec, classTokModel.Id, 0);
                        }
                        else
                        {
                            var resultSection = OrigTokSection.FirstOrDefault(c => c.Id == sec.Id);
                            if (resultSection != null) //If Edit
                            {
                                if (sec.Title == resultSection.Title && sec.Content == resultSection.Content && sec.Image == resultSection.Image) //Check if changes have been made, disregard update to avoid calling API
                                {
                                    //disregard update
                                }
                                else
                                {
                                    isSuccess = await TokService.Instance.UpdateTokSectionAsync(sec);
                                }
                            }
                        }
                        cnt += 1;
                    }
                } 
                else if(ClassTokGroup.ToLower() == "q&a")
                {
                    ResultData<TokSection> OrigTokSectionResult = null;
                    IEnumerable<TokSection> OrigTokSection = null;

                    if (isSave)
                    {
                        OrigTokSectionResult = await TokService.Instance.GetQnATokSectionsAsync(classTokModel.Id);
                        OrigTokSection = OrigTokSectionResult.Results;
                    }

                    bool isSuccess = false;
                    var cnt = 0;
                    foreach (var sec in QNACollection)
                    {
                        //Progress Text
                        Thread.Sleep(200);
                        double val1 = (double)(cnt + 1) / (double)QNACollection.Count;
                        var val2 = val1 * 100;
                        int percent = (int)val2;
                        ProgressText.Text = percent.ToString() + " %";

                        if (isSave)
                        {
                            sec.Id = null;
                        }

                        if (string.IsNullOrEmpty(sec.Id)) //Save
                        {
                            var dummySec = new TokSection();
                            sec.Id = dummySec.Id;
                            sec.TokId = classTokModel.Id;
                            sec.TokTypeId = classTokModel.TokTypeId;
                            sec.UserId = Settings.GetUserModel().UserId;

                            isSuccess = await TokService.Instance.CreateQnaTokSectionAsync(sec, classTokModel.Id, 0);
                        }
                        else
                        {
                            var resultSection = OrigTokSection.FirstOrDefault(c => c.Id == sec.Id);
                            if (resultSection != null) //If Edit
                            {
                                if (sec.Title == resultSection.Title && sec.Content == resultSection.Content && sec.Image == resultSection.Image) //Check if changes have been made, disregard update to avoid calling API
                                {
                                    //disregard update
                                }
                                else
                                {
                                    isSuccess = await TokService.Instance.UpdateQnATokSectionAsync(sec);
                                }
                            }
                        }
                        cnt += 1;
                    }
                }

            }
            hideProgress();

            if (result.ResultMessage != "cancelled")
            {
                var isSuccess = false;
                if (result.ResultEnum == Result.Success)
                    isSuccess = true;

                new ClassTokEarnedCoinsDialog(this, classTokModel, handlerClick: (s, e) =>
                {
                    if (result.ResultEnum == Result.Success)
                    {
                        if (!isSave) //TokInfo
                        {
                            if (ClassTokGroup.ToLower() == "mega") //If Mega
                            {
                                var modelSerialized = JsonConvert.SerializeObject(MegaCollection.ToList());
                                Intent intent = new Intent();
                                intent.PutExtra("toksection", modelSerialized);
                                SetResult(AppResult.Ok, intent);
                                this.Finish();
                            }
                            else if (ClassTokGroup.ToLower() == "q&a")
                            {
                                var modelSerialized = JsonConvert.SerializeObject(QNACollection.ToList());
                                Intent intent = new Intent();
                                intent.PutExtra("qnatoksection", modelSerialized);
                                SetResult(AppResult.Ok, intent);
                                this.Finish();
                            }
                            else
                            {
                                if (result.ResultObject != null)
                                {
                                    //Get the new tok model
                                    var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                                    var desClassTokResult = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);
                                    classTokModel.Image = desClassTokResult.Image;
                                }
                                else
                                {
                                    if (classTokModel.Image.Contains("data:image/jpeg;base64,"))
                                    {
                                        //Empty to avoid OOM issue and lag due to image that is so big. API should return the updated tok
                                        classTokModel.Image = "";
                                    }
                                }
                                ClassToksFragment.Instance.AddClassTokCollection(classTokModel);
                                var modelSerialized = JsonConvert.SerializeObject(classTokModel);
                                Intent intent = new Intent();
                                intent.PutExtra("classtokModel", modelSerialized);

                                SetResult(AppResult.Ok, intent);
                                this.Finish();
                            }
                        }
                        else
                        {
                            ClassToksFragment.Instance.AddClassTokCollection(classTokModel);

                            if (ProfileFragment.Instance.ClassTokList != null)
                            {
                                ProfileFragment.Instance.AddTokCollection(classTokModel, null);
                            }

                            var modelSerialized = JsonConvert.SerializeObject(classTokModel);
                            Intent intent = new Intent();
                            intent.PutExtra("tokModel", modelSerialized);

                            SetResult(AppResult.Ok, intent);
                            this.Finish();
                        }

                        this.Finish();
                    }

                    this.Finish();
                }).Show();
            }
        }

        private string TokMessage() {
            string message = string.Empty;
            int coins = 0;
            var tokGroup = TokGroup.Text.ToLower().Split(" ")[0];
            switch (tokGroup) {
                case "basic": coins = 2; message = "Thank you for adding a Basic Tok\n";  break;
                case "pic": coins = 2; message = "Thank you for adding a Pic Tok\n"; break;
                case "detailed": coins = 5; message = "Thank you for adding a Detailed Tok\n"; break;
                case "list": coins = 5; message = "Thank you for adding a List Tok\n"; break;
                case "mega": coins = 10; message = "Thank you for adding a Mega Tok\n"; break;
                case "q&a": coins = 10; message = "Thank you for adding a Q&A Tok\n"; break;
            }
            if (Settings.GetTokketUser().MembershipEnabled) {
                coins *= 2;
            }
            if (!string.IsNullOrEmpty(classTokModel?.Sticker))
                coins -= 7;
            message += $"You earned {coins} coins.";
            return message;
        }

        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(this)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {

                            })
                            .SetNegativeButton("Retry", (_, args) =>
                            {
                                SaveClassTok_IsClicked(_, args);
                            })
                            .SetCancelable(false)
                            .Show();
        }

        private void changeSaveText()
        {
            int cntCheck = 0;
            if (chkPublic.Checked) cntCheck += 1;
            //if (chkPrivate.Checked) cntCheck += 1;
            if (chkGroup.Checked) cntCheck += 1;

            if (cntCheck == 0 || cntCheck == 2 || cntCheck == 3)
            {
                toolbarSave.Text = "Add Class Tok";
            }
            else
            {
                if (chkPublic.Checked)
                {
                    toolbarSave.Text = "Add Public Class Tok";
                }
                /*else if (chkPrivate.Checked)
                {
                    SaveClassTok.Text = "Add Private Class Tok";
                }*/
                else if (chkGroup.Checked)
                {
                    toolbarSave.Text = "Add Class Tok to Group";
                }
            }
        }
        private void showProgress()
        {
            LinearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
            ProgressBarCircle.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)), PorterDuff.Mode.Multiply);
        }
        private void hideProgress()
        {
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;
        }
        private void ResetAll()
        {
            DetailCollection = new ObservableCollection<AddTokDetailModel>();
            MegaCollection = new ObservableCollection<TokSection>();
            QNACollection = new ObservableCollection<TokSection>();
            classTokModel = new ClassTokModel();
            classTokModel.DetailImages = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, };
            //User Data
            classTokModel.UserDisplayName = tokketUser.DisplayName;
            classTokModel.UserId = Settings.GetUserModel().UserId;
            classTokModel.UserCountry = tokketUser.Country;
            classTokModel.UserPhoto = tokketUser.UserPhoto;
            ImageDisplay.ContentDescription = "";
            ImageSecondary.ContentDescription = "";

            AddTokDetailModel TokDetail = new AddTokDetailModel();
            DetailCollection.Add(TokDetail);

            TokDetail = new AddTokDetailModel();
            DetailCollection.Add(TokDetail);

            TokDetail = new AddTokDetailModel();
            DetailCollection.Add(TokDetail);

            var TokDetailMega = new TokSection();
            MegaCollection.Add(TokDetailMega);

            var tokContentQna = new TokSection();
            var QnAItem = new List<Qna>();
            var item = new Qna();
            QnAItem.Add(item);

            tokContentQna.Id = "";
            tokContentQna.QuestionAnswer = QnAItem;
            QNACollection.Add(tokContentQna);

            ArrAnswer = new bool[10];
            classTokModel.DetailTokLinks = new string[10];

            bundle = new Bundle();
        }
        private void dialogBoxOk(string message)
        {
            var alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage(message);
            alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        private void BulletType_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (BulletType.Text.ToString().ToLower()) {
                case "none": break;
                    classTokModel.BulletKind = "none";
                case "bullets":
                    classTokModel.BulletKind = "bullet";
                    break;
                case "numbers":
                    classTokModel.BulletKind = "number"; 
                    break;
                case "letters":
                    classTokModel.BulletKind = "letter";
                    break;
            }
        }

        private void showHideDetails(bool isShow)
        {
            if (isShow)
            {
                txtShowHideAutoPaste.Visibility = ViewStates.Visible;
                RecyclerDetail.Visibility = ViewStates.Visible;
                AddDetailButton.Visibility = ViewStates.Visible;
                linearToksGirlDetail.Visibility = ViewStates.Visible;
            }
            else
            {
                txtShowHideAutoPaste.Visibility = ViewStates.Gone;
                RecyclerDetail.Visibility = ViewStates.Gone;
                AddDetailButton.Visibility = ViewStates.Gone;
                linearToksGirlDetail.Visibility = ViewStates.Gone;
            }
        }

        private void TokType_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            classTokModel.IsMega = false;
            classTokModel.IsDetailBased = false;

            var tokGroup = TokGroup.Text.ToString().Split(" ")[0];
            classTokModel.TokGroup = tokGroup;

            imageBtnAttachment.Visibility = ViewStates.Gone;
            txtSecondaryImage.Visibility = ViewStates.Gone;
            //ImageSecondary.Visibility = ViewStates.Gone;

            TokStarDetail();
            showHideDetails(false);
            SourceUrl.Hint = "Source URL";
            hideShowAllSections.Visibility = ViewStates.Gone;
            hideShowAllSections.Text = "Hide All Sections";

            layoutMultipleToks.Visibility = ViewStates.Gone;
            btnAddClassTokAddContent.Visibility = ViewStates.Gone;
            txtTipNotes.Visibility = ViewStates.Gone;
            circleImageViewTipNote.Visibility = ViewStates.Gone;
            btnCloseTipNote.Visibility = ViewStates.Gone;
            switch (tokGroup.ToLower())
            {
                case "basic":
                    LinearSecondary.Visibility = ViewStates.Visible;
                    inputNotes.Visibility = ViewStates.Visible;
                    txtTipNotes.Visibility = ViewStates.Visible;
                    circleImageViewTipNote.Visibility = ViewStates.Visible;
                    btnCloseTipNote.Visibility = ViewStates.Visible;

                    inputBulletType.Visibility = ViewStates.Gone;
                    txtSecondaryImage.Visibility = ViewStates.Visible;
                    showHideDetails(false);
                    break;
                case "pic":
                    LinearSecondary.Visibility = ViewStates.Visible;
                    inputNotes.Visibility = ViewStates.Visible;
                    txtTipNotes.Visibility = ViewStates.Visible;
                    circleImageViewTipNote.Visibility = ViewStates.Visible;
                    btnCloseTipNote.Visibility = ViewStates.Visible;

                    inputBulletType.Visibility = ViewStates.Gone;
                    ImageSecondary.ContentDescription = "";
                    showHideDetails(false);
                    break;
                case "detailed":
                    classTokModel.IsDetailBased = true;
                    SetDetailRecyclerAdapter();
                    LinearSecondary.Visibility = ViewStates.Gone;
                    inputNotes.Visibility = ViewStates.Gone;

                    inputBulletType.Visibility = ViewStates.Visible;
                    ImageSecondary.ContentDescription = "";
                    AddDetailButton.Visibility = ViewStates.Gone;
                    linearToksGirlDetail.Visibility = ViewStates.Gone;
                    showHideDetails(true);
                    break;

                case "list":
                    classTokModel.IsDetailBased = true;
                    SetDetailRecyclerAdapter();
                    LinearSecondary.Visibility = ViewStates.Gone;
                    inputNotes.Visibility = ViewStates.Gone;
                    inputBulletType.Visibility = ViewStates.Visible;
                    ImageSecondary.ContentDescription = "";
                    showHideDetails(true);
                    break;

                case "mega":
                    var adapterMega = MegaCollection.GetRecyclerAdapter(BindAddClassTokMega, Resource.Layout.addtokmegadetail_row);
                    RecyclerDetail.SetAdapter(adapterMega);
                    classTokModel.IsMega = true;
                    LinearSecondary.Visibility = ViewStates.Gone;
                    inputNotes.Visibility = ViewStates.Gone;
                    inputBulletType.Visibility = ViewStates.Gone;
                    ImageSecondary.ContentDescription = "";
                    imageBtnAttachment.Visibility = ViewStates.Visible;
                    hideShowAllSections.Visibility = ViewStates.Visible;
                    SourceUrl.Hint = "Link";
                    showHideDetails(true);
                    break;
                case "q&a":
                    var adapterQnA = QNACollection.GetRecyclerAdapter(BindAddClassTokQandA, Resource.Layout.item_q_n_a_content);
                    RecyclerDetail.SetAdapter(adapterQnA);

                    btnAddClassTokAddContent.Visibility = ViewStates.Visible;

                    LinearSecondary.Visibility = ViewStates.Gone;
                    inputNotes.Visibility = ViewStates.Gone;
                    inputBulletType.Visibility = ViewStates.Gone;
                    ImageSecondary.ContentDescription = "";
                    imageBtnAttachment.Visibility = ViewStates.Visible;
                    hideShowAllSections.Visibility = ViewStates.Visible;
                    SourceUrl.Hint = "Link";
                    showHideDetails(true);

                    AddDetailButton.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void BindAddClassTokDetail(CachingViewHolder holder, AddTokDetailModel model, int position)
        {
            var DeleteImage = holder.FindCachedViewById<Button>(Resource.Id.btnDeltokdtl_img);
            var AddImage = holder.FindCachedViewById<ImageView>(Resource.Id.btnAddtokdtl_img);
            var CheckAnswer = holder.FindCachedViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField);
            var inputAddTokDetailField = holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputAddTokDetailField);
            var EnglishHeader = holder.FindCachedViewById<TextView>(Resource.Id.lblAddTokDetailEng1);
            var Detail = holder.FindCachedViewById<EditText>(Resource.Id.txtAddTokDetailField);
            var EnglishDetail = holder.FindCachedViewById<EditText>(Resource.Id.txtAddTokDetailFieldEngTrans);
            var btnDeleteDetail = holder.FindCachedViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1);
            var btnPasteTokLinkDetail = holder.FindCachedViewById<Button>(Resource.Id.btnPasteTokLink);
            var btnSearchForToksDetail = holder.FindCachedViewById<Button>(Resource.Id.btnSearchForToks);
            var btnRemoveTokDetail = holder.FindCachedViewById<Button>(Resource.Id.btnRemovePaste);
            var flexPaste = holder.FindCachedViewById<FlexboxLayout>(Resource.Id.flexPaste);
            var subbullet = holder.FindCachedViewById<CheckBox>(Resource.Id.subcheckbox);
            flexPaste.Visibility = ViewStates.Visible;

            var detailBinding = new Binding<string, string>(model,
                                                  () => model.Detail,
                                                  Detail,
                                                  () => Detail.Text,
                                                  BindingMode.TwoWay);

            var englishdetailBinding = new Binding<string, string>(model,
                                                  () => model.EnglishDetail,
                                                  EnglishDetail,
                                                  () => EnglishDetail.Text,
                                                  BindingMode.TwoWay);

            var chkAnswerBinding = new Binding<bool, bool>(model,
                                                 () => model.ChkAnswer,
                                                 CheckAnswer,
                                                 () => CheckAnswer.Checked,
                                                 BindingMode.TwoWay);

            if (classTokModel.DetailTokLinks == null)
            {
                classTokModel.DetailTokLinks = new string[1];
            }
            
            if (position < classTokModel.DetailTokLinks.Length)
            {
                if (!string.IsNullOrEmpty(classTokModel.DetailTokLinks[position]))
                {
                    if (string.IsNullOrEmpty(classTokLinkDetails[position].PrimaryFieldText))
                    {
                        btnSearchForToksDetail.Visibility = ViewStates.Gone;
                        btnPasteTokLinkDetail.Visibility = ViewStates.Gone;
                        btnRemoveTokDetail.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        btnPasteTokLinkDetail.Text = classTokLinkDetails[position].PrimaryFieldText; //ClassTokModel.DetailTokLinks[position];
                        btnRemoveTokDetail.Visibility = ViewStates.Visible;
                    }
                }
            }
            else
            {
                string[] arrayTokLinks = classTokModel.DetailTokLinks;
                Array.Resize<string>(ref arrayTokLinks, classTokModel.DetailTokLinks.Length + 1);
                classTokModel.DetailTokLinks = arrayTokLinks;
            }

            if (position > ArrAnswer.Length - 1)
            {
                bool[] arrayAnswers = ArrAnswer;
                Array.Resize<bool>(ref arrayAnswers, ArrAnswer.Length + 1);
                ArrAnswer = arrayAnswers;
            }

            CheckAnswer.Checked = ArrAnswer[position];
            CheckAnswer.Visibility = ViewStates.Gone;
            AddImage.Tag = position;
            if (position < 3) //Default detail is 3
            {
                btnDeleteDetail.Visibility = ViewStates.Gone;
               
            }
            else
            {
                btnDeleteDetail.Visibility = ViewStates.Visible;
            }

            if (position == 0)
            {
            
                //Sets the first detail to not indented
                IsIndent[0]=false;
                subbullet.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (position > IsIndent.Count - 1)
                {
                    IsIndent.Add(false);
                }
                IsIndent[position] = false;
            }
            holder.FindCachedViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField).Tag = position;
            holder.FindCachedViewById<Button>(Resource.Id.btnDeltokdtl_img).Tag = position;

            if (!CheckAnswer.Checked)
            {
                inputAddTokDetailField.Hint = "Detail " + (position + 1);
            }
            else
            {
                inputAddTokDetailField.Hint = "Answer";
            }

            holder.FindCachedViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1).Tag = position;

            if (classTokModel.DetailImages != null)
            {
                if (position < classTokModel.DetailImages.Length)
                {
                    if (!string.IsNullOrEmpty(classTokModel.DetailImages[position]))
                    {
                        AddImage.SetImageBitmap(null);
                        DeleteImage.Visibility = ViewStates.Visible;
                    }

                    if (URLUtil.IsValidUrl(classTokModel.DetailImages[position]))
                    {
                        ImageDownloaderHelper.AssignImageAsync(AddImage, classTokModel.DetailImages[position], this);
                    }
                    else
                    {
                        byte[] imageDetailBytes = Convert.FromBase64String(classTokModel.DetailImages[position] ?? "");
                        AddImage.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    }
                }
            }

            //if (ChkTokEnglish.Checked == false)
            //{
            //    EnglishHeader.Text = "English Translation";
            //    EnglishHeader.Visibility = ViewStates.Visible;
            //    holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Visible;
            //}
            //else
            //{
                EnglishHeader.Visibility = ViewStates.Gone;
                holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Gone;
            //}


            btnPasteTokLinkDetail.Click -= async (sender, e) => {};
            btnPasteTokLinkDetail.Tag = position;
            btnPasteTokLinkDetail.Click += async (sender, e) =>
            {
                if (classTokModel.DetailTokLinks[position] == null)
                {
                    await pasteTok_Click(false, btnPasteTokLinkDetail, btnSearchForToksDetail, btnRemoveTokDetail);
                }
                else
                {
                    //Go to Tok Info
                    goToTokInfo(classTokLinkDetails[position]);
                }
            };

            btnRemoveTokDetail.Click += delegate
            {
                classTokModel.DetailTokLinks[position] = null;
                classTokLinkDetails[position] = null;
                btnPasteTokLinkDetail.Text = "Paste Tok Link";
                IsIndent[position] = false;
                btnSearchForToksDetail.Visibility = ViewStates.Visible;
                btnRemoveTokDetail.Visibility = ViewStates.Gone;
            };

            btnSearchForToksDetail.Click += delegate
            {
                goToSearchToks(false, position);
            };

            subbullet.CheckedChange += (obj, evt) => {
                if (evt.IsChecked)
                {
                    subbullet.Text = "Undo Sub-Bullet";
                }
                else
                {
                    subbullet.Text = "Convert as Sub-bullet";
                }
                IsIndent[position] = evt.IsChecked;
            };

            Detail.FocusChange += ItemOnFocusChange;
            TokStarDetail();
        }
        private void TokStarDetail()
        {
            if (classTokModel.TokGroup == null)
            {
                classTokModel.TokGroup = "Basic";
            }

            if (classTokModel.TokGroup.ToLower() == "list")
            {
                AddDetailButton.Visibility = ViewStates.Visible;
                txtTokStartDetailLimit.Text = "You can have unlimited details. For more, create new tok.";
                txtTokStartDetailLimit.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.colorPrimary);
            }
            else if (classTokModel.TokGroup.ToLower() == "q&a")
            {
                txtTokStartDetailLimit.Text = "You can have unlimited Q & A. For more, create new tok.";
                AddDetailButton.Visibility = ViewStates.Gone;
            }
            else if (classTokModel.TokGroup.ToLower() == "mega")
            {
                txtTokStartDetailLimit.Text = "You can have unlimited mega details. For more, create new tok.";
                AddDetailButton.Visibility = ViewStates.Gone;
            }
            else
            {
                if (DetailCollection.Count == 10)
                {
                    AddDetailButton.Visibility = ViewStates.Gone;
                    txtTokStartDetailLimit.Text = "Detail limit reached! Create new tok.";
                    txtTokStartDetailLimit.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.alli_red);
                }
                else
                {
                    AddDetailButton.Visibility = ViewStates.Visible;
                    txtTokStartDetailLimit.Text = "You can have up to 10 details. For more, create new tok.";
                    txtTokStartDetailLimit.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.colorPrimary);
                }
            }
        }

        private void goToTokInfo(ClassTokModel classTokModel)
        {
            Intent nextActivity = new Intent(this, typeof(TokInfoActivity));
            var modelConvert = JsonConvert.SerializeObject(classTokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivity(nextActivity);
        }
        private void goToSearchToks(bool isMain, int position = -1)
        {
            bundle.PutBoolean("isMain", isMain);
            bundle.PutInt("position", position);
            Intent nextActivity = new Intent(this, typeof(SearchToksDialog));
            this.StartActivityForResult(nextActivity, SEARCH_TOKS_REQUEST_CODE);
        }

        private void BindAddClassTokMega(CachingViewHolder holder, TokSection model, int position)
        {
            var layout = holder.FindCachedViewById<GridLayout>(Resource.Id.gridAddTokMegaDetail);
            var Title = holder.FindCachedViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle);
            var Content = holder.FindCachedViewById<EditText>(Resource.Id.txtAddTokMegaContent);
            var Image = holder.FindCachedViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg);
            var RelaveAddTokMegaImg = holder.FindCachedViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg);
            var SectionNumber = holder.FindCachedViewById<TextView>(Resource.Id.txtAddTokMegaNumber);

            SectionNumber.Text = (position + 1).ToString();

            var titleBinding = new Binding<string, string>(model,
                                              () => model.Title,
                                              Title,
                                              () => Title.Text,
                                              BindingMode.TwoWay);
            var contentBinding = new Binding<string, string>(model,
                                              () => model.Content,
                                              Content,
                                              () => Content.Text,
                                              BindingMode.TwoWay);

            Image.ContentDescription = position.ToString();
            if (URLUtil.IsValidUrl(model.Image))
            {
                Glide.With(this).Load(model.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(Image);
            }
            else
            {
                Image.Tag = position;
                byte[] imageDetailBytes = Convert.FromBase64String(model.Image ?? "");
                Image.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
            }

            Image.Click += delegate
            {
                Bitmap bitmapImage = ((BitmapDrawable)Image.Drawable).Bitmap;
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmapImage.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();

                Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
                Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
                this.StartActivity(nextActivity);
            };

            if (model.Image != null)
            {
                Image.SetBackgroundColor(Color.ParseColor("#3498db"));
                RelaveAddTokMegaImg.Visibility = ViewStates.Visible;
            }

            //Adding tag
            holder.FindCachedViewById<ImageView>(Resource.Id.btnAddtokmegadtl_img).Tag = position;
            var DeleteBtn = holder.FindCachedViewById<Button>(Resource.Id.btnAddTokMega_deletedtlField1);
            DeleteBtn.Tag = position;
            holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaTitle).Tag = position;
            holder.FindCachedViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = position;
            holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaContent).Tag = position;
            Title.Tag = position;
            holder.FindCachedViewById<ImageView>(Resource.Id.btnAddTokMega_deleteImg).Tag = position;
            holder.FindCachedViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Tag = position;

            if (position > 0)
            {
                DeleteBtn.Visibility = ViewStates.Visible;
            }

            Content.FocusChange += ItemOnFocusChange;

            holder.ItemView.ContentDescription = "mega_parent";
            holder.ItemView.SetOnLongClickListener(this);
        }

        private void BindAddClassTokQandA(CachingViewHolder holder, TokSection model, int position)
        {
            var inputLayoutContent = holder.FindCachedViewById<TextInputLayout>(Resource.Id.inputLayoutContent);
            var txtContent = holder.FindCachedViewById<AppCompatEditText>(Resource.Id.txtContent);
            var imageView = holder.FindCachedViewById<AppCompatImageView>(Resource.Id.imageView);
            var linearQNA = holder.FindCachedViewById<LinearLayout>(Resource.Id.linearQNA);
            var btnAddClasstokAddQnA = holder.FindCachedViewById<Button>(Resource.Id.btnAddClasstokAddQnA);
            btnAddClasstokAddQnA.Visibility = ViewStates.Visible;

            btnAddClasstokAddQnA.Tag = position;
            inputLayoutContent.Tag = position;

            var contentBinding = new Binding<string, string>(model,
                                              () => model.Content,
                                              txtContent,
                                              () => txtContent.Text,
                                              BindingMode.TwoWay);

            imageView.ContentDescription = position.ToString();
            if (URLUtil.IsValidUrl(model.Image))
            {
                Glide.With(this).Load(model.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(imageView);
            }
            else
            {
                imageView.Tag = position;
                byte[] imageDetailBytes = Convert.FromBase64String(model.Image ?? "");
                imageView.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
            }

            imageView.Click -= OnClickImageQnA;
            imageView.Click += OnClickImageQnA;

            if (model.Image != null)
            {
                imageView.SetBackgroundColor(Color.ParseColor("#3498db"));
            }

            //Adding tag
            imageView.Tag = position;

            var btnDeleteImage = holder.FindCachedViewById<Button>(Resource.Id.btnDeleteImage);
            btnDeleteImage.Tag = position;

            txtContent.Tag = position;
            linearQNA.Tag = position;


            linearQNA.RemoveAllViews();
            foreach (var item in model.QuestionAnswer)
            {
                linearAddQnADetail(linearQNA, item);
            }

            if (position > 0)
            {
                btnDeleteImage.Visibility = ViewStates.Visible;
            }

            btnAddClasstokAddQnA.Click -= OnClickAddQnA;
            btnAddClasstokAddQnA.Click += OnClickAddQnA;

            holder.ItemView.ContentDescription = "qna_parent";
            holder.ItemView.SetOnLongClickListener(this);
        }

        private void OnClickAddQnA(object sender, EventArgs e)
        {
            int ndx = 0;
            try { ndx = (int)(sender as Button).Tag; } catch { ndx = int.Parse((string)(sender as Button).Tag); }
            View z = RecyclerDetail.GetChildAt(ndx);
            z.FindViewById<LinearLayout>(Resource.Id.linearQNA).Tag = ndx;

            var item = new Qna();

            QNACollection[ndx].QuestionAnswer.Add(item);
            linearAddQnADetail(z.FindViewById<LinearLayout>(Resource.Id.linearQNA), item);

            if (QNACollection[ndx].QuestionAnswer.Count == 10)
            {
                (sender as Button).Visibility = ViewStates.Gone;
            }
        }

        private void OnClickImageQnA(object sender, EventArgs e)
        {
            var imageView = sender as AppCompatImageView;

            int ndx = 0;
            try { ndx = (int)imageView.Tag; } catch { ndx = int.Parse((string)imageView.Tag); }
                        
            Bitmap bitmapImage = ((BitmapDrawable)imageView.Drawable).Bitmap;
            if (bitmapImage == null)
            {
                Settings.BrowsedImgTag = (int)imageView.Tag;
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);
            }
            else
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmapImage.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();

                Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
                Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
                this.StartActivity(nextActivity);
            }
        }

        private void linearAddQnADetail(LinearLayout linear, Qna item)
        {
            View viewQnA = LayoutInflater.Inflate(Resource.Layout.item_q_n_a_row, null);
            var txtQuestion = viewQnA.FindViewById<AppCompatEditText>(Resource.Id.txtQuestions);
            var txtAnswers = viewQnA.FindViewById<AppCompatEditText>(Resource.Id.txtAnswers);

            var questionBinding = new Binding<string, string>(item,
                                             () => item.Question,
                                             txtQuestion,
                                             () => txtQuestion.Text,
                                             BindingMode.TwoWay);

            var answerBinding = new Binding<string, string>(item,
                                             () => item.Answer,
                                             txtAnswers,
                                             () => txtAnswers.Text,
                                             BindingMode.TwoWay);

            /*txtQuestion.Text = item.Question;
            txtAnswers.Text = item.Answer;*/

            linear.AddView(viewQnA);
        }
        private async Task EditClassTok()
        {
            chkMasterCopy.Visibility = ViewStates.Gone;
            txtToolbarTitle.Text = "Update Class Tok";

            classTokModel = JsonConvert.DeserializeObject<ClassTokModel>(Intent.GetStringExtra("ClassTokModel"));
            classTokModel.IsMasterCopy = !classTokModel.HasReactions && !classTokModel.HasComments;
            chkMasterCopy.Checked = classTokModel.IsMasterCopy;
            if (classTokModel.IsMasterCopy)
            {
                chkMasterCopy.Visibility = ViewStates.Visible;
                chkMasterCopy.Text = "Nonmaster Copy - with comments, reactions, and answers.";
            }

            //chkPrivate.Checked = Convert.ToBoolean(ClassTokModel.IsPrivate);
            chkGroup.Checked = classTokModel.IsGroup;
            chkPublic.Checked = classTokModel.IsPublic;

            //chkPrivate.ButtonTintList = Android.Content.Res.ColorStateList.ValueOf(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));
            chkGroup.ButtonTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));
            chkPublic.ButtonTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));
            chkTokChannel.ButtonTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.btnDisable)));

            //chkPrivate.Enabled = false;
            //chkGroup.Enabled = false;
            //chkPublic.Enabled = false;
            //chkTokChannel.Enabled = false;

            //Tok Group
            var spinnerTGPosition = 1;
            if (classTokModel.TokGroup.ToLower() == "basic")
            {
                spinnerTGPosition = basicPosition;
            }
            else if (classTokModel.TokGroup.ToLower() == "pic")
            {
                spinnerTGPosition = picPosition;
            }
            else if (classTokModel.TokGroup.ToLower() == "detail" || classTokModel.TokGroup.ToLower() == "detailed")
            {
                spinnerTGPosition = detailPosition;
            }
            else if (classTokModel.TokGroup.ToLower() == "list")
            {
                spinnerTGPosition = listPosition;
            }
            else if (classTokModel.TokGroup.ToLower() == "mega")
            {
                spinnerTGPosition = megaPosition;
            }
            else if (classTokModel.TokGroup.ToLower() == "q&a")
            {
                spinnerTGPosition = qNaPosition;
            }

            TokGroup.SetText(arryTokGroup[spinnerTGPosition], false);
            TokGroup.Enabled = false;

            TokType.Text = classTokModel.TokType;
            if (!String.IsNullOrEmpty(classTokModel.GroupName))
            {
                classGroupName.Visibility = ViewStates.Visible;
                classGroupName.Text = "Class Group: " + classTokModel.GroupName;
            }

            txtReferenceId.Text = classTokModel.ReferenceId;
            Category.Text = classTokModel.Category;
            Primary.Text = classTokModel.PrimaryFieldText;
            Secondary.Text = classTokModel.SecondaryFieldText;
            ImageDisplay.ContentDescription = classTokModel.Image;
            ImageSecondary.ContentDescription = classTokModel.SecondaryImage;
            if (!string.IsNullOrEmpty(classTokModel.Image))
            {
                Glide.With(this).Load(classTokModel.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(ImageDisplay);
                ImageDisplay.Background = null;
            }

            ImageSecondary.Visibility = ViewStates.Visible;
            if (!string.IsNullOrEmpty(classTokModel.SecondaryImage))
            {
                Glide.With(this).Load(classTokModel.SecondaryImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(ImageSecondary);
                ImageSecondary.Background = null;
            }

            toolbarSave.Text = "Save Edit(s)";

            if (classTokModel.IsMegaTok == true || classTokModel.TokGroup.ToLower() == "mega") //If Mega
            {
                var GetToksSecResult = await SharedService.TokService.Instance.GetTokSectionsAsync(classTokModel.Id);
                var GetToksSec = GetToksSecResult.Results;
                classTokModel.Sections = GetToksSec.ToArray();

                MegaCollection.Clear();
                foreach (var item in classTokModel.Sections)
                {
                    MegaCollection.Add(item);
                }

                var adapterMega = MegaCollection.GetRecyclerAdapter(BindAddClassTokMega, Resource.Layout.addtokmegadetail_row);
                RecyclerDetail.SetAdapter(adapterMega);
            }
            else if (classTokModel.TokGroup.ToLower() == "q&a")
            {
                var getTokSectionsResult = await TokService.Instance.GetQnATokSectionsAsync(classTokModel.Id);
                var getTokSections = getTokSectionsResult.Results;
                var listTokContentQna = getTokSections.ToList();
                classTokModel.Sections = getTokSections.ToArray();

                QNACollection.Clear();
                foreach (var section in getTokSections)
                {
                    var TokQnAItem = section;

                    TokQnAItem.QuestionAnswer = section.QuestionAnswer;

                    QNACollection.Add(TokQnAItem);
                }

                var adapterQnA = QNACollection.GetRecyclerAdapter(BindAddClassTokQandA, Resource.Layout.item_q_n_a_content);
                RecyclerDetail.SetAdapter(adapterQnA);
            }
            else if (classTokModel.TokGroup.ToLower() == "detailed" || classTokModel.TokGroup.ToLower() == "detail" || classTokModel.TokGroup.ToLower() == "list")
            {
                IsIndent.Clear();
                DetailCollection.Clear();
                ArrAnswer = new bool[classTokModel.Details.Length];
                IsIndent = classTokModel.IsIndent;
                foreach (var item in classTokModel.Details)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var tokdetailModel = new AddTokDetailModel();
                        tokdetailModel.Detail = item;
                        DetailCollection.Add(tokdetailModel);
                    }
                }

                //If total image is lesser than the details, add new
                if (classTokModel.DetailImages != null)
                {
                    var detailImages = classTokModel.DetailImages.ToList();
                    for (int i = detailImages.Count; i < 10; i++)
                    {
                        detailImages.Add(null);
                    }
                    classTokModel.DetailImages = detailImages.ToArray();
                }

                SetDetailRecyclerAdapter();
            }

            //if (classTokModel.ColorMainHex == "#FFFFFF" || string.IsNullOrEmpty(classTokModel.ColorMainHex)) {
            //    ColorSelectorLayout.Visibility = ViewStates.Gone;
            //}

            if (!string.IsNullOrEmpty(classTokModel.TokShare)) {
                Category.Enabled = false;
                Secondary.Enabled = false;
                SourceNote.Enabled = false;
                SourceUrl.Enabled = false;
                txtAddMainImage.Enabled = false;
                txtSecondaryImage.Enabled = false;
                btnPasteTokLink.Enabled = false;
                btnSearchForToks.Enabled = false;
                btnSelectColor.Enabled = false;
                Notes.Enabled = false;
                AddDetailButton.Enabled = false;
                TokType.Enabled = false;
                TokGroup.Enabled = false;
                chkGroup.Enabled = false;
                chkPublic.Enabled = false;
                chkTokChannel.Enabled = false;
                BtnAddTileSticker.Enabled = false;
            }

            if (!string.IsNullOrEmpty(classTokModel.TokLink))
            {
                btnRemovePaste.Visibility = ViewStates.Visible;
                btnSearchForToks.Visibility = ViewStates.Gone;
                btnPasteTokLink.Visibility = ViewStates.Gone;
            }
        }
        private void SetDetailRecyclerAdapter()
        {
            var adapterDetail = DetailCollection.GetRecyclerAdapter(BindAddClassTokDetail, Resource.Layout.addtokdetail_row);
            RecyclerDetail.SetAdapter(adapterDetail);
        }
        [Java.Interop.Export("OnClickAddDetail")]
        public void OnClickAddDetail(View v)
        {
            var tokGroup = TokGroup.Text.ToString().Split(" ")[0];
            if (tokGroup.ToLower() == "mega") //If Mega
            {
                var TokDetailMega = new TokSection();
                TokDetailMega.Id = "";
                MegaCollection.Add(TokDetailMega);

                var adapterMega = MegaCollection.GetRecyclerAdapter(BindAddClassTokMega, Resource.Layout.addtokmegadetail_row);
                RecyclerDetail.SetAdapter(adapterMega);
            }
            else
            {
                var TokDetail = new AddTokDetailModel();
                DetailCollection.Add(TokDetail);

                #region TokList
                if (classTokModel.DetailTokLinks.Count() < DetailCollection.Count)
                {
                    string[] arrayTokLinks = classTokModel.DetailTokLinks;
                    Array.Resize<string>(ref arrayTokLinks, classTokModel.DetailTokLinks.Length + 1);
                    classTokModel.DetailTokLinks = arrayTokLinks;
                }

                if (ArrAnswer.Count() < DetailCollection.Count)
                {
                    Array.Resize<bool>(ref ArrAnswer, ArrAnswer.Length + 1);
                }

                if (IsIndent.Count() < DetailCollection.Count)
                {
                    IsIndent.Add(false);
                }
                #endregion

                SetDetailRecyclerAdapter();
                LinearDetail.Visibility = ViewStates.Visible;
                AddDetailButton.Visibility = ViewStates.Visible;

                TokStarDetail();
            }
        }
        [Java.Interop.Export("OnCheckAddTok")]
        public void OnCheckAddTok(View v)
        {
            //Clear all checked
            int vtag = (int)v.Tag;

            ArrAnswer[vtag] = (v as CheckBox).Checked;
            //SetDetailRecyclerAdapter();
        }
        [Java.Interop.Export("OnDelete")] // The value found in android:onClick attribute.
        public void OnDelete(View v) // Does not need to match value in above attribute.
        {
            int vtag = (int)v.Tag;

            var tokGroup = TokGroup.Text.ToString().Split(" ")[0];
            if (tokGroup.ToLower() == "mega") //If Mega
            {
                MegaCollection.RemoveAt(vtag);
                RecyclerDetail.RemoveViewAt(vtag);

                for (int i = 0; i < RecyclerDetail.ChildCount; ++i)
                {
                    View z = RecyclerDetail.GetChildAt(i);

                    //Adding tag
                    z.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_img).Tag = i;
                    z.FindViewById<Button>(Resource.Id.btnAddTokMega_deletedtlField1).Tag = i;
                    z.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaTitle).Tag = i;
                    z.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = i;
                    z.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg).Tag = i;
                    z.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaContent).Tag = i;
                    z.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = i;
                    z.FindViewById<ImageView>(Resource.Id.btnAddTokMega_deleteImg).Tag = i;
                    z.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Tag = i;
                    z.FindViewById<TextView>(Resource.Id.txtAddTokMegaNumber).Text = (i + 1).ToString();
                }
            }
            else if (tokGroup.ToLower() == "q&a") //If Q&a
            {
                QNACollection.RemoveAt(vtag);
                RecyclerDetail.RemoveViewAt(vtag);

                for (int i = 0; i < RecyclerDetail.ChildCount; ++i)
                {
                    View z = RecyclerDetail.GetChildAt(i);

                    //Adding tag
                    z.FindViewById<TextInputLayout>(Resource.Id.inputLayoutContent).Tag = i;
                    z.FindViewById<AppCompatEditText>(Resource.Id.txtContent).Tag = i;
                    z.FindViewById<AppCompatImageView>(Resource.Id.imageView).Tag = i;
                    z.FindViewById<Button>(Resource.Id.btnDeleteImage).Tag = i;
                    z.FindViewById<LinearLayout>(Resource.Id.linearQNA).Tag = i;
                }
            }
            else
            {
                DetailCollection.RemoveAt(vtag);
                RecyclerDetail.RemoveViewAt(vtag);

                for (int i = 0; i < RecyclerDetail.ChildCount; ++i)
                {
                    View view = RecyclerDetail.GetChildAt(i);
                    var CheckAnswer = view.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField);
                    var inputAddTokDetailField = view.FindViewById<TextInputLayout>(Resource.Id.inputAddTokDetailField);
                    if (!CheckAnswer.Checked)
                    {
                        inputAddTokDetailField.Hint = "Detail " + (i + 1);
                    
                    }
                    else
                    {
                        inputAddTokDetailField.Hint = "Answer";
                    }

                    view.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField).Tag = i;
                    view.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img).Tag = i;
                    view.FindViewById<Button>(Resource.Id.btnDeltokdtl_img).Tag = i;
                    view.FindViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1).Tag = i;
                }

                AddDetailButton.Visibility = ViewStates.Visible;
                TokStarDetail();

                if (classTokModel.DetailImages != null)
                {
                    if (vtag < classTokModel.DetailImages.Length)
                    {
                        classTokModel.DetailImages.ToList().RemoveAt(vtag);
                    }
                }
            }
        }
        [Java.Interop.Export("OnDeleteImageDtl")]
        public void OnDeleteImageDtl(View v)
        {
            int vtag = (int)v.Tag;

            for (int x = vtag; x == vtag; ++x)
            {
                View view;
                var tokGroup = TokGroup.Text.Split(" ")[0];
                if (tokGroup.ToLower() == "mega") //If Mega
                {
                    view = RecyclerDetail.GetChildAt(x);
                    RelativeLayout RelaveAddTokMegaImg = view.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg);
                    RelaveAddTokMegaImg.Visibility = ViewStates.Gone;
                }
                else
                {
                    view = RecyclerDetail.GetChildAt(x);
                    var linearView = view.FindViewById<LinearLayout>(Resource.Id.DetailImageView);
                    linearView.Visibility = ViewStates.Gone;

                    v.Visibility = ViewStates.Gone;

                    if (classTokModel.DetailImages.Length <= (vtag + 1))
                    {
                        classTokModel.DetailImages[vtag] = null;
                    }
                }
            }
        }
        [Java.Interop.Export("OnClickAddTokImgDetail")]
        public void OnClickAddTokImgDetail(View v)
        {
            Settings.BrowsedImgTag = (int)v.Tag;//(int)v.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img).Tag;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);
        }
        public void OnClickSecondaryImage(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);
        }

        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            v.ContentDescription = "";
            txtAddMainImage.Text = "Select an image.";
            Drawable img = ContextCompat.GetDrawable(this, Resource.Drawable.image_icon);
            txtAddMainImage.SetCompoundDrawablesWithIntrinsicBounds(img, null, null, null);

            ImageDisplay.Background = ContextCompat.GetDrawable(this, Resource.Drawable.linearboard_nopadding);
            ImageDisplay.SetImageBitmap(null);
            classTokModel.Image = null;
            ImageDisplay.ContentDescription = "";
        }
        public void OnClickRemoveSecondaryImage(View v)
        {
            v.ContentDescription = "";
            txtSecondaryImage.Text = "Select secondary image.";
            Drawable img = ContextCompat.GetDrawable(this, Resource.Drawable.image_icon);
            txtSecondaryImage.SetCompoundDrawablesWithIntrinsicBounds(img, null, null, null);

            ImageSecondary.Visibility = ViewStates.Gone;
            ImageSecondary.Background = ContextCompat.GetDrawable(this, Resource.Drawable.linearboard_nopadding);
            ImageSecondary.SetImageBitmap(null);
            classTokModel.SecondaryImage = null;
            ImageSecondary.ContentDescription = "";
        }

        private void onClickImage(NetUri uri, int requestCode)
        {
            messageCropDialog = new MessageDialog(this, "Option", "", "Crop", "Save", (s, e) =>
            {
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                this.StartActivityForResult(nextActivity, requestCode);

                if (Settings.BrowsedImgTag != -1)
                {
                    int vtag = Settings.BrowsedImgTag;

                    var tokGroup = TokGroup.Text.ToString().Split(" ")[0];
                    if (tokGroup.ToLower() == "mega") //If Mega
                    {
                    }
                    else if (tokGroup.ToLower() == "q&a") //If Q&A
                    {
                    }
                    else
                    {
                        for (int x = vtag; x == vtag; ++x)
                        {
                            View view = RecyclerDetail.GetChildAt(x);
                            Button btnDeltokdtl_img = view.FindViewById<Button>(Resource.Id.btnDeltokdtl_img);
                            detailImageSelected = uri.ToString();
                            btnDeltokdtl_img.Visibility = ViewStates.Visible;
                        }
                    }
                }
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
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] AppResult resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddTokActivityType) && (resultCode == AppResult.Ok) && (data != null))
            {
                NetUri uri = data.Data;
                Settings.ImageBrowseCrop = (string)uri;

                onClickImage(uri, requestCode);
            }
            else if ((requestCode == COLOR_REQUEST_CODE) && (resultCode == AppResult.Ok) && (data != null))
            {
                var colorHex = data.GetStringExtra("color");
                classTokModel.ColorMainHex = colorHex;
                txtColor.SetBackgroundColor(Color.ParseColor(colorHex));
            }
            else if ((requestCode == SEARCH_TOKS_REQUEST_CODE) && (resultCode == AppResult.Ok) && (data != null))
            {
                var isMain = bundle.GetBoolean("isMain", false);
                var position = bundle.GetInt("position", -2);

                var classtokData = data.GetStringExtra("classtokModel");
                classTokPaste = JsonConvert.DeserializeObject<ClassTokModel>(classtokData);

                if (isMain)
                {
                    btnPasteTokLink.Text = classTokPaste.PrimaryFieldText;
                    classTokModel.TokLink = classTokPaste.Id;
                    classTokMainPaste = classTokPaste;

                    btnSearchForToks.Visibility = ViewStates.Gone;
                    btnRemovePaste.Visibility = ViewStates.Visible;
                }
                else if (position >= 0)
                {
                    classTokModel.DetailTokLinks[position] = classTokPaste.Id;
                    classTokLinkDetails[position] = classTokPaste;

                    var view = RecyclerDetail.GetChildAt(position);
                    view.FindViewById<Button>(Resource.Id.btnPasteTokLink).Text = classTokPaste.PrimaryFieldText;
                    view.FindViewById<Button>(Resource.Id.btnSearchForToks).Visibility = ViewStates.Gone;
                    view.FindViewById<Button>(Resource.Id.btnRemovePaste).Visibility = ViewStates.Visible;
                }
            }
            else if ((requestCode == (int)ActivityType.AddStickerDialogActivity) && (resultCode == AppResult.Ok))
            {
                var dataTokModelstr = data.GetStringExtra("tokModel");
                if (dataTokModelstr != null)
                {
                    var dataTokModel = JsonConvert.DeserializeObject<TokModel>(dataTokModelstr);
                    if (dataTokModel != null)
                    {
                        classTokModel.Sticker = dataTokModel.Sticker;
                        classTokModel.StickerImage = dataTokModel.StickerImage;
                        Glide.With(this).Load(dataTokModel.StickerImage).Into(StickerImage);
                        BtnAddTileSticker.Visibility = ViewStates.Gone;
                        StickerImage.Visibility = ViewStates.Visible;
                        BtnRemoveSticker.Visibility = ViewStates.Visible;
                    }
                }
            }
        }
        public void displayImageBrowse()
        {
            //Main Image
            if (Settings.BrowsedImgTag == -1)
            {
                if (isSecondaryImage)
                {
                    ImageSecondary.SetImageBitmap(null);
                    txtSecondaryImage.ContentDescription = "-";
                    txtSecondaryImage.Text = "Remove Image";
                    Drawable img = ContextCompat.GetDrawable(this, Resource.Drawable.closered_24px);
                    txtSecondaryImage.SetCompoundDrawablesWithIntrinsicBounds(img, null, null, null);

                    ImageSecondary.ContentDescription = Settings.ImageBrowseCrop;
                    byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                    ImageSecondary.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                    ImageSecondary.Background = null;
                    ImageSecondary.Visibility = ViewStates.Visible;
                }
                else
                {
                    ImageDisplay.SetImageBitmap(null);
                    txtAddMainImage.ContentDescription = "-";
                    txtAddMainImage.Text = "Remove Image";
                    Drawable img = ContextCompat.GetDrawable(this, Resource.Drawable.closered_24px);
                    txtAddMainImage.SetCompoundDrawablesWithIntrinsicBounds(img, null, null, null);

                    ImageDisplay.ContentDescription = Settings.ImageBrowseCrop;
                    byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                    ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                    ImageDisplay.Background = null;
                }
            }
            else
            {
                //Detail Image
                int detailpos = Settings.BrowsedImgTag; //Position of Control
                byte[] imageDetailBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);

                var tokGroup = TokGroup.Text.Split(" ")[0];

                if (tokGroup.ToLower() == "mega") //If Mega
                {
                    View view = RecyclerDetail.GetChildAt(detailpos);
                    view.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Visibility = ViewStates.Visible;
                    ImageView btnAddtokdtl_img = view.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg);

                    btnAddtokdtl_img.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    MegaCollection[detailpos].Image = Settings.ImageBrowseCrop;
                }
                else if (tokGroup.ToLower() == "q&a") //Q&A
                {
                    View view = RecyclerDetail.GetChildAt(detailpos);
                    var imageView = view.FindViewById<AppCompatImageView>(Resource.Id.imageView);

                    imageView.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    QNACollection[detailpos].Image = Settings.ImageBrowseCrop;
                }
                else
                {
                    if (classTokModel.DetailImages != null)
                    {
                        var detailImages = classTokModel.DetailImages.ToList();
                        for (int i = detailImages.Count; i < detailpos; i++)
                        {
                            detailImages.Add(null);
                        }
                        classTokModel.DetailImages = detailImages.ToArray();

                        for (int x = detailpos; x == detailpos; ++x)
                        {
                            View view = RecyclerDetail.GetChildAt(x);
                            ImageView btnAddtokdtl_img = view.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img);
                            btnAddtokdtl_img.Visibility = ViewStates.Visible;
                            classTokModel.DetailImages[x] = Settings.ImageBrowseCrop;
                            // btnAddtokdtl_img.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                            var deleteButton = view.FindViewById<Button>(Resource.Id.btnDeltokdtl_img);
                            LinearLayout layout = view.FindViewById<LinearLayout>(Resource.Id.DetailImageView);
                            CheckBox isimagevisible = view.FindViewById<CheckBox>(Resource.Id.chk_includeImage);
                            ImageView detailPic = view.FindViewById<ImageView>(Resource.Id.detailpic);

                            byte[] imageBytes = Convert.FromBase64String(classTokModel.DetailImages[x]);
                            detailPic.SetImageBitmap((BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length)));

                            deleteButton.Visibility = ViewStates.Visible;
                            layout.Visibility = ViewStates.Visible;
                            isimagevisible.Checked = true;
                            isimagevisible.CheckedChange += (obj, _event) => { IsImageTokPakVisible[x] = _event.IsChecked; };
                           
                        }
                    }
                }
            }
            Settings.ImageBrowseCrop = null;
        }

        public bool OnLongClick(View v)
        {
            if (v.ContentDescription == "mega_parent")
            {
                var txtMegaTitle = v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaTitle);
                customToolTip(this, "There can be unlimited sections with 100,000 characters per section.", Resource.Drawable.tokstarguy2, txtMegaTitle);
            };
            return true;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (ClassGroupModel == null) {
                if (e.Action == MotionEventActions.Up)
                {
                    if (v.ContentDescription == "privacy")
                    {
                        LinearToast.Visibility = ViewStates.Gone;
                        if (v.Id == Resource.Id.txtChkGroup)
                        {
                            if (chkGroup.Checked)
                            {
                                popupGroupDialog = new Dialog(this);
                                popupGroupDialog.SetContentView(Resource.Layout.dialog_classgroup_list);
                                popupGroupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                                popupGroupDialog.Show();

                                double widthD = getLayoutWidth();
                                popupGroupDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
                                popupGroupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

                                var btnCancel = popupGroupDialog.FindViewById<Button>(Resource.Id.btnCancel);
                                btnCancel.Click += delegate
                                {
                                    classGroupId = "";
                                    popupGroupDialog.Dismiss();
                                };

                                dialogRecyclerGroup.SetLayoutManager(new GridLayoutManager(Application.Context, 1));

                                ClassGroupCollection = new ObservableCollection<ClassGroupModel>();
                                RunOnUiThread(async () => await InitializeGroupList());
                            }
                        }
                    }
                }
                else if (e.Action == MotionEventActions.Down)
                {
                    if (v.ContentDescription == "privacy")
                    {
                        LinearToast.Visibility = ViewStates.Visible;

                        if (!isSave)
                        {
                            TextToast.Text = "*Cannot edit the tok's feed.*";
                        }
                        else
                        {
                            switch (v.Id)
                            {
                                /*case Resource.Id.txtChkPrivate:
                                    TextToast.Text = "Only you can see this class tok.";
                                    break;*/
                                case Resource.Id.txtChkPublic:
                                    TextToast.Text = "Anyone can see this class tok.";
                                    break;
                                case Resource.Id.txtChkGroup:
                                    TextToast.Text = "Only users in the selected group can see this class tok.";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (e.Action == MotionEventActions.Cancel)
                {
                    if (v.ContentDescription == "privacy")
                    {
                        LinearToast.Visibility = ViewStates.Gone;
                    }
                }
            }
         
            return false; //set false to be able to call onLongClick
        }
        void recyclerTouchEvent(object sender, TouchEventArgs e)
        {
            int position = (int)(sender as View).Tag;
            selectedGroupPosition = position;
            classGroupId = ClassGroupCollection[position].Id;
            gesturedetector.OnTouchEvent(e.Event);
        }
        private async Task InitializeGroupList()
        {
            dialogProgress.Visibility = ViewStates.Visible;
            GroupFilter filter = (GroupFilter)Settings.FilterGroup;
            ResultData<ClassGroupModel> results = new ResultData<ClassGroupModel>();

            switch (filter)
            {
                case GroupFilter.OwnGroup:
                    results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = null,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        startswith = false,
                        joined = false
                    });
                    break;
                case GroupFilter.JoinedGroup:
                    results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = null,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false
                    });
                    break;
                case GroupFilter.MyGroup:
                    var myGroups = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = null,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = false,
                        startswith = false
                    });

                    var joined = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = null,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false
                    });

                    var combined = myGroups.Results.ToList();
                    combined.AddRange(joined.Results);

                    results.Results = combined;

                    break;
            }

            dialogRecyclerGroup.ContentDescription = results.ContinuationToken;
            var classgroupResult = results.Results.ToList();

            foreach (var item in classgroupResult)
            {
                ClassGroupCollection.Add(item);
            }

            ClassGroupModel items = new ClassGroupModel();
            items.Name = "James";
            items.Description = "SSDF";
            ClassGroupCollection.Add(items);

            SetGroupRecyclerAdapter();
        }

        [Java.Interop.Export("OnAddTokSticker")]
        public void OnAddTokSticker(View v)
        {
            classTokModel.UserCountry = tokketUser.UserPhoto;
            classTokModel.UserPhoto = tokketUser.UserPhoto;
            classTokModel.UserDisplayName = tokketUser.DisplayName;
            if (isSave)
            {
                var tokGroup = TokGroup.Text.Split(" ")[0];
                classTokModel.TokGroup = tokGroup;
            }

            classTokModel.TokTypeId = $"toktype-{classTokModel.TokGroup?.ToIdFormat()}-{classTokModel.TokType?.ToIdFormat()}";

            TokModel tokModel = classTokModel;
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            Intent nextActivity = new Intent(this, typeof(AddStickerDialogActivity));
            nextActivity.PutExtra("tokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.AddStickerDialogActivity);
        }

        private async Task pasteTok_Click(bool isMain, View v, Button btnSearchT, Button btnRemove)
        {
            var classTokString = await Clipboard.GetTextAsync();
            if (!string.IsNullOrEmpty(classTokString))
            {
                try
                {
                    classTokPaste = JsonConvert.DeserializeObject<ClassTokModel>(classTokString);
                    if (classTokPaste == null)
                    {
                        showalertDialog();
                    }
                    else
                    {
                        (v as Button).Text = classTokPaste.PrimaryFieldText;
                        if (isMain)
                        {
                            classTokModel.TokLink = classTokPaste.Id;
                            classTokMainPaste = classTokPaste;
                            Secondary.Text = classTokPaste.PrimaryFieldText;
                        }
                        else
                        {
                            int ndx = 0;
                            try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }

                            classTokModel.DetailTokLinks[ndx] = classTokPaste.Id;
                            classTokLinkDetails[ndx] = classTokPaste;

                            if (DetailCollection.Count > ndx)
                            {
                                DetailCollection[ndx].Detail = classTokPaste.PrimaryFieldText;
                            }
                            SetDetailRecyclerAdapter();
                        }
                        await Clipboard.SetTextAsync("");
                        btnSearchT.Visibility = ViewStates.Gone;
                        btnRemove.Visibility = ViewStates.Visible;
                    }
                }
                catch (Exception ex)
                {
                    showalertDialog();
                }
            }
            else
            {
                showalertDialog();
            }
        }

        private void showalertDialog()
        {
            var alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage("Pasted Tok is invalid!");
            alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }
        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private AddClassTokActivity mainActivity;

            public MyGestureListener(AddClassTokActivity Activity)
            {
                mainActivity = Activity;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                //Console.WriteLine("Double Tab");
                var alertDiag = new AlertDialog.Builder(mainActivity);
                alertDiag.SetTitle("");
                alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
                alertDiag.SetMessage("Selected Group: " + mainActivity.ClassGroupCollection[mainActivity.selectedGroupPosition].Name);
                alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Proceed</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) => {
                    alertDiag.Dispose();
                    mainActivity.popupGroupDialog.Dismiss();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
                diag.SetCanceledOnTouchOutside(false);
                return true;
            }
        }
        private void SetGroupRecyclerAdapter()
        {
            var adapterClassGroup = ClassGroupCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.dialog_classgroup_list_item);
            dialogRecyclerGroup.SetAdapter(adapterClassGroup);

            dialogProgress.Visibility = ViewStates.Invisible;
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, ClassGroupModel model, int position)
        {
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtBody = holder.FindCachedViewById<TextView>(Resource.Id.txtBody);
            linearDialogParent = holder.FindCachedViewById<LinearLayout>(Resource.Id.linearDialogParent);
            linearDialogParent.Tag = position;
            linearDialogParent.Touch -= recyclerTouchEvent;
            linearDialogParent.Touch += recyclerTouchEvent;
            txtHeader.Text = model.Name;
            txtBody.Text = model.Description;
        }

        private void assignValuesPreviewTile()
        {
            Stream sr = null;
            if (!string.IsNullOrEmpty(Settings.GetTokketUser().Country))
            {
                try
                {
                    sr = Assets.Open("Flags/" + Settings.GetTokketUser().Country + ".jpg");
                }
                catch (Exception)
                {

                }
            }
            Bitmap bitmap = BitmapFactory.DecodeStream(sr);
            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                string tokimg = ImageDisplay.ContentDescription;

                byte[] imageDetailBytes = Convert.FromBase64String(tokimg);
                tileTokImgMain.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));


                Glide.With(this).Load(classTokModel.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(tileTokImgUserPhoto);
                tileTokImgUserPhoto.SetOnTouchListener(this);
                tileTokImgUserFlag.SetImageBitmap(bitmap);
                tileImgUserDisplayName.Text = classTokModel.UserDisplayName;
                tileTokImgPrimaryFieldText.Text = classTokModel.PrimaryFieldText;

                tileTokImgCategory.Text = classTokModel.Category;
                tileTokImgTokGroup.Text = classTokModel.TokGroup;
                tileTokImgTokType.Text = classTokModel.TokType;

                tilegridTokImage.SetBackgroundResource(Resource.Drawable.tileview_layout);
                GradientDrawable tokimagedrawable = (GradientDrawable)tiletokimgdrawable.Background;
                tokimagedrawable.SetColor(Color.White);

                tilegridTokImage.Visibility = ViewStates.Visible;
                tilegridBackground.Visibility = ViewStates.Gone;
            }
            else
            {

                Glide.With(this).Load(classTokModel.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(tileUserPhoto);
                tileUserFlag.SetImageBitmap(bitmap);

                tileUserDisplayName.Text = classTokModel.UserDisplayName;
                tilePrimaryFieldText.Text = classTokModel.PrimaryFieldText;
                tileCategory.Text =classTokModel.Category;
                tileTokGroup.Text = classTokModel.TokGroup;
                tileTokType.Text = classTokModel.TokType;
                if (string.IsNullOrEmpty(classTokModel.EnglishPrimaryFieldText))
                {
                    tileEnglishPrimaryFieldText.Visibility = ViewStates.Gone;
                }
                else
                {
                    tileEnglishPrimaryFieldText.Visibility = ViewStates.Visible;
                }
                tileEnglishPrimaryFieldText.Text = classTokModel.EnglishPrimaryFieldText;
                tilegridBackground.SetBackgroundResource(Resource.Drawable.tileview_layout);
                GradientDrawable Tokdrawable = (GradientDrawable)tileTokdrawable.Background;

                if (classTokModel.ColorMainHex == "#FFFFFF"  || string.IsNullOrEmpty(classTokModel.ColorMainHex))
                {
                    Tokdrawable.SetColor(Color.White);
                    setTextColor(Color.Black);
                }
                else
                {
                    Tokdrawable.SetColor(Color.ParseColor(classTokModel.ColorMainHex));
                    setTextColor(Color.White);
                }

                tilegridBackground.Visibility = ViewStates.Visible;
                tilegridTokImage.Visibility = ViewStates.Gone;
            }
        }
        private void setTextColor(Color color)
        {
            tileUserDisplayName.SetTextColor(color);
            tilePrimaryFieldText.SetTextColor(color);
            tileCategory.SetTextColor(color);
            tileTokGroup.SetTextColor(color);
            tileTokType.SetTextColor(color);
            tileEnglishPrimaryFieldText.SetTextColor(color);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    new SimpleAlertMessageDialog(this, handlerOKText: "Keep Editing", handlerOkClick: null, handlerCancelText: "Discard Post", handlerCancel: (s, e) => {
                        this.Finish();
                    }, animation: "lottie_exclamation.json").Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

#region UI Properties

        public ConstraintLayout ColorSelectorLayout => FindViewById<ConstraintLayout>(Resource.Id.colorSelectorLayout);
        public Button btnPreviewTile => FindViewById<Button>(Resource.Id.btnPreviewTile);
        public Button btnAddClassTokAddContent => FindViewById<Button>(Resource.Id.btnAddClassTokAddContent);
        public TextView txtColor => FindViewById<TextView>(Resource.Id.txtColor);
        public Button btnSelectColor => FindViewById<Button>(Resource.Id.btnSelectColor);
        public AutoCompleteTextView TokGroup => FindViewById<AutoCompleteTextView>(Resource.Id.txtAddClassTokType);
        public AutoCompleteTextView BulletType => FindViewById<AutoCompleteTextView>(Resource.Id.txtAddBulletType);
        public EditText TokType => FindViewById<EditText>(Resource.Id.txtAddClassTokClass);
        public EditText Category => FindViewById<EditText>(Resource.Id.txtAddClassTokCategory);
        public EditText Primary => FindViewById<EditText>(Resource.Id.txtAddClassTokPrimary);
        public EditText Secondary => FindViewById<EditText>(Resource.Id.txtAddClassTokSecondary);
        public EditText Notes => FindViewById<EditText>(Resource.Id.txtAddClassTokNotes);
        public TextInputLayout inputNotes => FindViewById<TextInputLayout>(Resource.Id.inputNotes);
        public ImageView ImageDisplay => FindViewById<ImageView>(Resource.Id.addclasstok_imagebrowse);
        public ImageView ImageSecondary => FindViewById<ImageView>(Resource.Id.imageViewSecondaryImage);
        public LinearLayout LinearDetail => FindViewById<LinearLayout>(Resource.Id.LinearAddClassTokDetail); 
        public LinearLayout LinearSecondary => FindViewById<LinearLayout>(Resource.Id.LinearAddClassTokSecondary);
        public RecyclerView RecyclerDetail => FindViewById<RecyclerView>(Resource.Id.RecyclerAddClassTok);
        public Button AddDetailButton => FindViewById<Button>(Resource.Id.btnAddClassTokAddDetail); 
        public ProgressBar ProgressBarCircle => FindViewById<ProgressBar>(Resource.Id.progressbarAddClassTok);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextAddClassTok);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_addclasstok);
        public TextView classGroupName => FindViewById<TextView>(Resource.Id.classGroupName);
        public CheckBox chkPublic => FindViewById<CheckBox>(Resource.Id.chkPublic);
        public CheckBox chkGroup => FindViewById<CheckBox>(Resource.Id.chkGroup);
        public ImageView imageToksGirlDetail => FindViewById<ImageView>(Resource.Id.imageToksGirlDetail);
        public LinearLayout linearToksGirlDetail => FindViewById<LinearLayout>(Resource.Id.linearToksGirlDetail);

        public CheckBox chkTokChannel => FindViewById<CheckBox>(Resource.Id.chkTokChannel);
        public TextView txtChkGroup => FindViewById<TextView>(Resource.Id.txtChkGroup);
        public EditText SourceUrl => FindViewById<EditText>(Resource.Id.txtSourceUrl);
        public TextView hideShowAllSections => FindViewById<TextView>(Resource.Id.hideShowAllSections);
        public EditText SourceNote => FindViewById<EditText>(Resource.Id.txtSourceNotes);
        public TextInputLayout inputBulletType => FindViewById<TextInputLayout>(Resource.Id.inputBulletType);
        public TextView txtChkPublic => FindViewById<TextView>(Resource.Id.txtChkPublic);
        public LinearLayout LinearToast => FindViewById<LinearLayout>(Resource.Id.LinearToast);
        public TextView TextToast => FindViewById<TextView>(Resource.Id.TextToast);
        public RelativeLayout dialogProgress => popupGroupDialog.FindViewById<RelativeLayout>(Resource.Id.relativeProgress);
        public RecyclerView dialogRecyclerGroup => popupGroupDialog.FindViewById<RecyclerView>(Resource.Id.recyclerClassGroupList);

        public Button BtnRemoveSticker => FindViewById<Button>(Resource.Id.removeSticker);
        public Button BtnAddTileSticker => FindViewById<Button>(Resource.Id.btnAddTileSticker);
        public ImageView StickerImage => FindViewById<ImageView>(Resource.Id.addclasstok_stickerimage);
        public TextView txtAddMainImage => FindViewById<TextView>(Resource.Id.txtAddMainImage);
        public TextView txtSecondaryImage => FindViewById<TextView>(Resource.Id.txtSecondaryImage);
        public AppCompatImageButton imageBtnAttachment => FindViewById<AppCompatImageButton>(Resource.Id.imageBtnAttachment);
        public ConstraintLayout constraintOptionalFields => FindViewById<ConstraintLayout>(Resource.Id.constraintOptionalFields);
        public TextView ShowHideOptional => FindViewById<TextView>(Resource.Id.hideShowOptionals);

        public Button BtnAutoPaste => FindViewById<Button>(Resource.Id.btnAutoPaste);
        public EditText txtReferenceId => FindViewById<EditText>(Resource.Id.txtReferenceId);

        public TextView txtTipNotes => FindViewById<TextView>(Resource.Id.txtTipNotes);
        public CircleImageView circleImageViewTipNote => FindViewById<CircleImageView>(Resource.Id.circleImageViewTipNote);
        public AppCompatImageButton btnCloseTipNote => FindViewById<AppCompatImageButton>(Resource.Id.btnCloseTipNote);
        public AppCompatCheckBox chkMasterCopy => FindViewById<AppCompatCheckBox>(Resource.Id.chkMasterCopy);

        #endregion

        #region Preview Tile UI
        public GridLayout tilegridBackground =>  tileDialog.FindViewById<GridLayout>(Resource.Id.gridBackground);
        public GridLayout tilegridTokImage => tileDialog.FindViewById<GridLayout>(Resource.Id.gridTokImage);
        public GridLayout tileTokdrawable => tileDialog.FindViewById<GridLayout>(Resource.Id.gridBackground);
        public GridLayout tiletokimgdrawable => tileDialog.FindViewById<GridLayout>(Resource.Id.gridTokImage);
        public ImageView tileUserPhoto => tileDialog.FindViewById<ImageView>(Resource.Id.imageUserPhoto);
        public ImageView tileUserFlag => tileDialog.FindViewById<ImageView>(Resource.Id.imageFlag);
        public TextView tileUserDisplayName => tileDialog.FindViewById<TextView>(Resource.Id.lbl_nameuser);
        public TextView tilePrimaryFieldText => tileDialog.FindViewById<TextView>(Resource.Id.lbl_row);
        public TextView tileSecondaryFieldText => tileDialog.FindViewById<TextView>(Resource.Id.secondarytext_row);
        public TextView tileEnglishPrimaryFieldText => tileDialog.FindViewById<TextView>(Resource.Id.lbl_englishPrimaryFieldText);
        public TextView tileCategory => tileDialog.FindViewById<TextView>(Resource.Id.lblCategory);
        public TextView tileTokGroup => tileDialog.FindViewById<TextView>(Resource.Id.lblTokGroup);
        public TextView tileTokType => tileDialog.FindViewById<TextView>(Resource.Id.lblTokType);
        public ImageView tileImgPurpleGem => tileDialog.FindViewById<ImageView>(Resource.Id.toktile_imgpurplegem);
        public ImageView tileTileSticker =>  tileDialog.FindViewById<ImageView>(Resource.Id.imgtile_stickerimage);
        public TextView tileTokUserTitle => tileDialog.FindViewById<TextView>(Resource.Id.lbl_royaltitle);

     

        //Tok Image
        public ImageView tileTokImgUserPhoto => tileDialog.FindViewById<ImageView>(Resource.Id.imageTokImgUserPhoto);
        public ImageView tileTokImgUserFlag => tileDialog.FindViewById<ImageView>(Resource.Id.img_tokimgFlag);
        public ImageView tileTokImgMain => tileDialog.FindViewById<ImageView>(Resource.Id.imgTokImgMain);
        public TextView tileImgUserDisplayName => tileDialog.FindViewById<TextView>(Resource.Id.lbl_Imgnameuser);
        public TextView tileTokImgPrimaryFieldText => tileDialog.FindViewById<TextView>(Resource.Id.lbl_tokimgprimarytext);
        public TextView tileTokImgSecondaryFieldText => tileDialog.FindViewById<TextView>(Resource.Id.lbl_tokimgsecondarytext);
        public TextView tileTokImgCategory => tileDialog.FindViewById<TextView>(Resource.Id.lblTokImgCategory);
        public TextView tileTokImgTokGroup => tileDialog.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
        public TextView tileTokImgTokType => tileDialog.FindViewById<TextView>(Resource.Id.lblTokImgType);
        public ImageView tileTileStickerImg => tileDialog.FindViewById<ImageView>(Resource.Id.imgtile_stickerimageImg);
        public TextView tileTokUserTitleImg => tileDialog.FindViewById<TextView>(Resource.Id.lbl_royaltitleImg);
        public ImageView tileImgPurpleGemTokImg => tileDialog.FindViewById<ImageView>(Resource.Id.toktile_imgpurplegemtokimg);
        public Button btnPasteTokLink => FindViewById<Button>(Resource.Id.btnPasteTokLink);
        public Button btnSearchForToks => FindViewById<Button>(Resource.Id.btnSearchForToks);
        public Button btnRemovePaste => FindViewById<Button>(Resource.Id.btnRemovePaste);
        public TextView txtTokStartDetailLimit => FindViewById<TextView>(Resource.Id.txtTokStartDetailLimit);
        public TextView txtShowHideAutoPaste => FindViewById<TextView>(Resource.Id.txtShowHideAutoPaste);
        public EditText txtAutoPaste => FindViewById<EditText>(Resource.Id.txtAutoPaste);
        public View layoutMultipleToks => FindViewById<View>(Resource.Id.layoutMultipleToks);
        public EditText editTextMultipleToks => layoutMultipleToks.FindViewById<EditText>(Resource.Id.editTextMultipleToks);
        public EditText editTextSeparator => layoutMultipleToks.FindViewById<EditText>(Resource.Id.editTextSeparator);
        public Button btnAddMultipleToks => layoutMultipleToks.FindViewById<Button>(Resource.Id.btnAddMultipleToks);
        public AndroidX.AppCompat.Widget.Toolbar customToolbar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.includeHeaderLayout);
        public TextView txtToolbarTitle => customToolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
        public Button toolbarSave => customToolbar.FindViewById<Button>(Resource.Id.btnMenu);
        #endregion
    }
}