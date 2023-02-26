using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Newtonsoft.Json;
using Android.Graphics;
using System.Threading.Tasks;
using SharedService = Tokket.Shared.Services;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Android.Util;
using Android.Webkit;
using Android.Graphics.Drawables;
using System.IO;
using Android.Views.InputMethods;
using Android.Text;
using Tokket.Shared.Services;
using Tokket.Android.Helpers;
using Android.Text.Style;
using Tokket.Android.Fragments;
using Android.Content.PM;
using AlertDialog = Android.App.AlertDialog;
using Base64 = Android.Util.Base64;
using Result = Android.App.Result;
using NetUri = Android.Net.Uri;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android
{
    [Activity(Label = "Add Set", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddSetActivity : BaseActivity
    {
        internal static AddSetActivity Instance { get; private set; }
        Set SetModel; TokketUser TokketUser; string userid;
        int SmileySelected = 0;
        List<TokMojiDrawableViewModel> TokMojiDrawables;
        public AddSetPageViewModel AddSetVm => App.Locator.AddSetPageVM;
        public TokInfoViewModel TokInfoVm => App.Locator.TokInfoPageVM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addset_page);
            Instance = this;
            Settings.ActivityInt = Convert.ToInt16(ActivityType.AddSetActivityType);

            AddSetVm.Instance = Instance;
            userid = Settings.GetUserModel().UserId;
            this.RunOnUiThread(async () => await InitializeData());

            TokMojiDrawables = new List<TokMojiDrawableViewModel>();
            //Load TokMoji
            RecyclerTokMoji.SetLayoutManager(new GridLayoutManager(this, 2));
            this.RunOnUiThread(async () => await RunTokMojis());

            EditName.FocusChange += delegate
            {
                if (EditName.IsFocused)
                {
                    BtnNameSmiley.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                    LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Gone;
                }
            };
            EditDescription.FocusChange += delegate
            {
                if (EditDescription.IsFocused)
                {
                    BtnDescriptionSmiley.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                    LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Gone;
                }
            };

            EditName.Click += delegate
            {
                BtnNameSmiley.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMoji.Visibility = ViewStates.Gone;
                LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                EditName.RequestFocus();

                inputManager.ShowSoftInput(EditName, 0);
            };

            EditDescription.Click += delegate
            {
                BtnDescriptionSmiley.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMoji.Visibility = ViewStates.Gone;
                LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                EditDescription.RequestFocus();

                inputManager.ShowSoftInput(EditDescription, 0);
            };

            AddSetVm.LinearProgress = LinearProgress;
            AddSetVm.ProgressCircle = Progressbar;
            AddSetVm.ProgressText = ProgressText;

            BtnCancel.Click += delegate
            {
                AddSetVm.CancelAddSet();
            };
            
            BtnBrowse.Click -= BtnBrowseImageClick;
            BtnBrowse.Click += BtnBrowseImageClick;

            SpinTokGroup.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTokGroup_ItemSelected);
            SpinPrivacy.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinPrivacy_ItemSelected);

            var SpinTGAadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, AddSetVm.TokGroupList);
            SpinTGAadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            SpinTokGroup.Adapter = null;
            SpinTokGroup.Adapter = SpinTGAadapter;

            var SpinPrivacyadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, AddSetVm.PrivacyList);
            SpinPrivacyadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            SpinPrivacy.Adapter = null;
            SpinPrivacy.Adapter = SpinPrivacyadapter;

            var passSetModel = Intent.GetStringExtra("setModel");
            if (passSetModel != null)
            {
                SetModel = JsonConvert.DeserializeObject<Set>(passSetModel);
                AddSetVm.Credentials = SetModel;
                TxtTokGroup.Text = SetModel.TokGroup;
                TxtTokType.Text = SetModel.TokType;
                TxtPrivacy.Text = SetModel.Privacy;
                EditName.Text = SetModel.Name;
                EditDescription.Text = SetModel.Description;

                if (!string.IsNullOrEmpty(SetModel.Image))
                {
                    if (URLUtil.IsValidUrl(SetModel.Image))
                    {
                        Glide.With(this).Load(SetModel.Image).Into(ImgBrowse);
                    }
                    else
                    {
                        //Glide.With(this).AsBitmap().Load(SetModel.Image).Into(ImgBrowse);
                        byte[] imageByte = Convert.FromBase64String(SetModel.Image);
                        ImgBrowse.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }            

                Settings.ImageBrowseCrop = SetModel.Image;

                BtnSave.Text = "Update Set";

                //Tok Group
                int spinnerTGPosition = AddSetVm.TokGroup.FindIndex(c => c.TokGroup == TxtTokGroup.Text);
                SpinTokGroup.SetSelection(spinnerTGPosition);

                //Tok Type
                for (int i = 0; i < AddSetVm.TokGroup[spinnerTGPosition].TokTypes.Length; i++)
                {
                    if (AddSetVm.TokGroup[spinnerTGPosition].TokTypes.ToString() == TxtTokType.Text)
                    {
                        SpinTokType.SetSelection(i);
                        break;
                    }
                }

                //Privacy
                for (int i = 0; i < AddSetVm.PrivacyList.Count; i++)
                {
                    if (AddSetVm.PrivacyList[i].ToString() == TxtPrivacy.Text)
                    {
                        SpinPrivacy.SetSelection(i);
                        break;
                    }
                }
                AddSetVm.Credentials.IsEdited = true;
            }
            else
            {
                BtnSave.Text = "Add Set";
            }
            BtnNameSmiley.Click -= SmileyIsClicked;
            BtnNameSmiley.Click += SmileyIsClicked;

            BtnDescriptionSmiley.Click -= SmileyIsClicked;
            BtnDescriptionSmiley.Click += SmileyIsClicked;

            BtnSave.Click += async (s, e) =>
            {
                AddSetVm.Credentials.TokGroup = TxtTokGroup.Text;
                AddSetVm.Credentials.TokType = TxtTokType.Text;
                AddSetVm.Credentials.Privacy = TxtPrivacy.Text;
                AddSetVm.Credentials.Name = EditName.Text;
                AddSetVm.Credentials.Description = EditDescription.Text;

                if (passSetModel != null)
                {
                    await AddSetVm.UpdateSet();
                }
                else
                {
                    AddSetVm.Credentials.IsEdited = false;
                    await AddSetVm.AddSet();
                }
            };

        }
        private async Task RunTokMojis()
        {
            TokInfoVm.TokMojiCollection.Clear();

            await TokInfoVm.LoadTokMoji();

            var adapterTokMoji = TokInfoVm.TokMojiCollection.GetRecyclerAdapter(BindTokMojiViewHolder, Resource.Layout.tokinfo_tokmoji_row);
            RecyclerTokMoji.SetAdapter(adapterTokMoji);

            RecyclerTokMoji.LayoutChange += (sender, e) =>
            {
                //Created a dummy recyclerTokMoji so that it will show the image.
                for (int i = 0; i < RecyclerTokMoji.ChildCount; i++)
                {
                    View view = RecyclerTokMoji.GetChildAt(i);
                    var ImgTokMoji = view.FindViewById<ImageView>(Resource.Id.imgTokInfoTokMojiRow);

                    if (ImgTokMoji.Drawable != null)
                    {
                        var detail = TokMojiDrawables.FirstOrDefault(a => a.TokmojiId == ImgTokMoji.ContentDescription);
                        if (detail == null)
                        {
                            var tokmojiModel = new TokMojiDrawableViewModel();
                            tokmojiModel.TokmojiId = ImgTokMoji.ContentDescription;

                            //encode image to base64 string
                            try //if Drawable is displaying the loader animation catch the error
                            {
                                Bitmap imgTokMojiBitmap = ((BitmapDrawable)ImgTokMoji.Drawable).Bitmap;
                                MemoryStream byteArrayOutputStream = new MemoryStream();
                                imgTokMojiBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                                byte[] byteArray = byteArrayOutputStream.ToArray();
                                tokmojiModel.TokmojImgBase64 = Base64.EncodeToString(byteArray, Base64Flags.Default);
                                //tokmojiModel.TokmojImg = ImgTokMoji.Drawable;
                                TokMojiDrawables.Add(tokmojiModel);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            };
        }
        private void SmileyIsClicked(object sender, EventArgs e)
        {
            var BtnSmile = (sender as ImageView);
            int tagnum = 0;
            try { tagnum = (int)BtnSmile.Tag; } catch { tagnum = int.Parse((string)BtnSmile.Tag); }

            //View TextEditor = EditName;
            //if (tagnum == 1)
            //{
            //    TextEditor = EditName;
            //}
            //else if (tagnum == 2)
            //{
            //    TextEditor = EditDescription;
            //}

            //TextEditor.SetFocusable(ViewFocusability.NotFocusable);

            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            if (RecyclerTokMoji.Visibility == ViewStates.Gone)
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_2b);
                inputManager.HideSoftInputFromWindow(BtnSmile.WindowToken, HideSoftInputFlags.None);
                RecyclerTokMoji.Visibility = ViewStates.Visible;
                LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Visible;
            }
            else
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMoji.Visibility = ViewStates.Gone;
                LinearAddSetTokMojiDummyMargin.Visibility = ViewStates.Gone;

                //View TextEditor = EditName;
                //if (tagnum == 1)
                //{
                //    TextEditor = EditName;
                //}
                //else if (tagnum == 2)
                //{
                //    TextEditor = EditDescription;
                //}

                //inputManager.ShowSoftInput(TextEditor, 0);
            }

            SmileySelected = tagnum;
        }

        private void BindTokMojiViewHolder(CachingViewHolder holder, Tokmoji tokmoji, int position)
        {
            var ImgTokMoji = holder.FindCachedViewById<ImageView>(Resource.Id.imgTokInfoTokMojiRow);
            Glide.With(this).Load(tokmoji.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgTokMoji);

            ImgTokMoji.ContentDescription = tokmoji.Id;
            ImgTokMoji.Click -= displayImageinCommentEditor;
            ImgTokMoji.Click += displayImageinCommentEditor;
        }

        EditText CommentEditor;
        int start;
        private void displayImageinCommentEditor(object sender, EventArgs e)
        {
            AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage("This tokmoji costs 3 coins. Continue?");
            alertDiag.SetPositiveButton("Cancel", (senderAlert, args) =>
            {
                alertDiag.Dispose();
            });
            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Proceed</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {

                var spannableTokMoji = new SpannableString((sender as ImageView).ContentDescription);

                if (SmileySelected == 1)
                {
                    CommentEditor = EditName;

                    if (EditName.IsFocused == false)
                    {
                        EditName.RequestFocus();
                    }

                    start = EditName.SelectionStart;

                }
                else if (SmileySelected == 2)
                {
                    CommentEditor = EditDescription;

                    if (EditDescription.IsFocused == false)
                    {
                        EditDescription.RequestFocus();
                    }

                    start = EditDescription.SelectionStart;
                }
                //int start = CommentEditor.SelectionStart;

                string tokmojiidx = (sender as ImageView).ContentDescription;
                string tokidx = ":" + tokmojiidx + ":";
                string spaceafter = tokidx + " ";

                //TokMoji Purchase
                ProgressText.Text = "Purchasing...";
                LinearProgress.Visibility = ViewStates.Visible;
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                var result = await TokMojiService.Instance.PurchaseTokmojiAsync(tokmojiidx, "tokmoji");

                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                LinearProgress.Visibility = ViewStates.Gone;

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    //Update Coins
                    Settings.UserCoins -= 3;
                    MainActivity.Instance.TextCoinsToast.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);
                    ProfileFragment.Instance.TextProfileCoins.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);

                    var resultObject = result.ResultObject as PurchasedTokmoji;
                    CommentEditor.Text = CommentEditor.Text.Substring(0, start) + spaceafter + CommentEditor.Text.Substring(start);

                    var spannableString = new SpannableString(CommentEditor.Text);
                    for (int i = 0; i < TokMojiDrawables.Count; i++)
                    {
                        var loopTokMojiID = ":" + TokMojiDrawables[i].TokmojiId + ":";
                        var indices = spannableString.ToString().IndexesOf(loopTokMojiID);

                        foreach (var index in indices)
                        {
                            var set = true;
                            foreach (ImageSpan span in spannableString.GetSpans(index, index + loopTokMojiID.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                            {
                                if (spannableString.GetSpanStart(span) >= index && spannableString.GetSpanEnd(span) <= index + loopTokMojiID.Length)
                                    spannableString.RemoveSpan(span);
                                else
                                {
                                    set = false;
                                    break;
                                }
                            }
                            if (set)
                            {

                                if (tokmojiidx == TokMojiDrawables[i].TokmojiId)
                                {
                                    TokMojiDrawables[i].PurchaseIds = resultObject.Id;
                                }

                                byte[] base64TokMoji = Convert.FromBase64String(TokMojiDrawables[i].TokmojImgBase64);
                                Bitmap decodedByte = BitmapFactory.DecodeByteArray(base64TokMoji, 0, base64TokMoji.Length);
                                spannableString.SetSpan(new ImageSpan(decodedByte), index, index + loopTokMojiID.Length, SpanTypes.ExclusiveExclusive);
                            }
                        }
                    }

                    CommentEditor.SetText(spannableString, TextView.BufferType.Spannable);
                    CommentEditor.SetSelection(start + spaceafter.Length);
                }
                else
                {
                    var dialog = new AlertDialog.Builder(this);
                    var alertDialog = dialog.Create();
                    alertDialog.SetTitle("");
                    alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertDialog.SetMessage("Not enough coins.");
                    alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                    alertDialog.Show();
                    alertDialog.SetCanceledOnTouchOutside(false);
                }
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }
        public async Task InitializeData()
        {
            TokketUser = await SharedService.AccountService.Instance.GetUserAsync(userid);
            AddSetVm.Credentials.UserId = userid;
            AddSetVm.Credentials.UserCountry = TokketUser.Country;
        }
        private void SpinTokGroup_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TxtTokGroup.Text = SpinTokGroup.GetItemAtPosition(SpinTokGroup.FirstVisiblePosition).ToString();
            LblDescription.Text = AddSetVm.TokGroup[e.Position].Description;
            loadTokType(e.Position);
        }
        public void loadTokType(int ndx)
        {
            SpinTokType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinTokType_ItemSelected);
            List<string> tokgroupList = new List<string>();
            for (int i = 0; i < AddSetVm.TokGroup[ndx].TokTypes.Length; i++)
            {
                tokgroupList.Add(AddSetVm.TokGroup[ndx].TokTypes[i]);
            }

            var Aadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, tokgroupList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            SpinTokType.Adapter = Aadapter;
        }
        public void SpinTokType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TxtTokType.Text = SpinTokType.GetItemAtPosition(SpinTokType.FirstVisiblePosition).ToString();
            LblSample.Text = AddSetVm.TokGroup[SpinTokGroup.LastVisiblePosition].Descriptions[e.Position];
            LblSample.Text += " \n";
            LblSample.Text += " \n";
            LblSample.Text += "Example: " + AddSetVm.TokGroup[SpinTokGroup.LastVisiblePosition].Examples[e.Position];
        }
        public void SpinPrivacy_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TxtPrivacy.Text = SpinPrivacy.GetItemAtPosition(SpinPrivacy.FirstVisiblePosition).ToString();
        }
        private void BtnBrowseImageClick(object sender, EventArgs e)
        {
            BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this, ImgBrowse);
            bottomsheet.Show(this.SupportFragmentManager, "tag");

            /*Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddSetActivityType);*/
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddSetActivityType) && (resultCode == Result.Ok) && (data != null))
            {
                NetUri uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                StartActivityForResult(nextActivity, requestCode);
            }

        }

        public void displayImageBrowse(Bitmap bitmap, string base64)
        {
            ImgBrowse.SetImageBitmap(bitmap);
            //byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            //ImgBrowse.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            AddSetVm.Credentials.Image = base64;
            Settings.ImageBrowseCrop = null;
        }

        public EditText TxtTokGroup => FindViewById<EditText>(Resource.Id.TxtTokGroup);
        public EditText TxtTokType => FindViewById<EditText>(Resource.Id.TxtTokType);
        public EditText TxtPrivacy => FindViewById<EditText>(Resource.Id.TxtPrivacy);
        public TextView BtnCancel => FindViewById<TextView>(Resource.Id.btnAddSetCancel);
        public TextView BtnSave => FindViewById<TextView>(Resource.Id.btnAddSetSave);
        public TextView LblDescription => FindViewById<TextView>(Resource.Id.lblAddSetDescription);
        public TextView LblSample => FindViewById<TextView>(Resource.Id.lblAddSetSample);
        public Spinner SpinTokGroup => FindViewById<Spinner>(Resource.Id.txtAddSetTokGroup);
        public Spinner SpinTokType => FindViewById<Spinner>(Resource.Id.txtAddSetTokType);
        public Spinner SpinPrivacy => FindViewById<Spinner>(Resource.Id.txtAddSetPrivacy);
        public EditText EditName => FindViewById<EditText>(Resource.Id.txtAddSetName);
        public EditText EditDescription => FindViewById<EditText>(Resource.Id.txtAddSetDescription);
        public ImageView ImgBrowse => FindViewById<ImageView>(Resource.Id.addset_imagebrowse);
        public Button BtnBrowse => FindViewById<Button>(Resource.Id.btnAddSet_btnBrowseImage);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_addsetprogress);
        public ProgressBar Progressbar => FindViewById<ProgressBar>(Resource.Id.progressbarAddSet);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextAddSet);
        public RecyclerView RecyclerTokMoji => FindViewById<RecyclerView>(Resource.Id.addsetRecyclerTokMojis);
        public ImageView BtnNameSmiley => FindViewById<ImageView>(Resource.Id.btnAddSetNameSmiley);
        public ImageView BtnDescriptionSmiley => FindViewById<ImageView>(Resource.Id.btnAddSetDescriptionSmiley);
        public LinearLayout LinearAddSetTokMojiDummyMargin => FindViewById<LinearLayout>(Resource.Id.LinearAddSetTokMojiDummyMargin);
    }
}