using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using static Android.Widget.CompoundButton;
using Tokket.Android.Adapters;
using Android.Util;
using Tokket.Shared.Models;
using Tokket.Android.Fragments;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using Newtonsoft.Json;
using System.IO;
using Android.Graphics;
using Android.Content.Res;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Android.Graphics.Drawables;
using Tokket.Shared.Extensions;
using System.Threading.Tasks;
using Tokket.Android.ViewModels;
using Tools = Tokket.Core.Tools;
using Android.Webkit;
using Tokket.Android.Helpers;
using System.Threading;
using Android.Views.InputMethods;
using Tokket.Android.ViewModels;
using Tokket.Shared.Services;
using Android.Text.Style;
using Android.Animation;
using AndroidX.AppCompat.App;
using Color = Android.Graphics.Color;
using Android.Content.PM;
using AppResult = Android.App.Result;
using NetUri = Android.Net.Uri;
using Tokket.Android.ViewHolders;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using AndroidX.RecyclerView.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.TextField;
using GalaSoft.MvvmLight.Helpers;

namespace Tokket.Android
{
    public class BITAdapter : ArrayAdapter<string>
    {
        public BITAdapter(Context context, int textViewResourceId, IList<string> objects) : base(context, textViewResourceId, objects)
        {
        }

        public override bool IsEnabled(int position)
        {
            if (position == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [Activity(Label = "Add Tok", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddTokActivity : BaseActivity, View.IOnTouchListener
    {
        SpannableStringBuilder ssbName;
        ISpannable spannableResText;

        // Allows us to know if we should use MotionEvent.ACTION_MOVE
        private bool tracking = false;
        // The Position where our touch event started
        private float startY;
        //float imgScale = 0;
        BottomSheetUserPhotoFragment bottomsheet;
        internal static AddTokActivity Instance { get; private set; }
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>(); string userid;
        AddTokOptionalAdapter optionaladapter; TokketUser tokketUser;
        ViewGroup.LayoutParams paramEngdetail;
        bool isAddTok = true; bool[] ArrAnswer;
        decimal lvEngdetailHeightdec; float lvEngdetailHeighfloat;
        int spinnerTGPosition = 0, detailSmilepos = 0; string SmileySelected = "";
        List<TokMojiDrawableViewModel> TokMojiDrawables;
        AddTokDetailModel[] TokDetails;
        GestureDetector gesturedetector; private bool Showingback;
        public AddTokViewModel AddTokVm => App.Locator.AddTokPageVM;
        public TokInfoViewModel TokInfoVm => App.Locator.TokInfoPageVM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addtok_page);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            isAddTok = Intent.GetBooleanExtra("isAddTok", true);
            Settings.ActivityInt = (int)ActivityType.AddTokActivityType;

            loadTokGroup();
            userid = Settings.GetUserModel().UserId;
            this.RunOnUiThread(async () => await InitializeData());
            TokMojiDrawables = new List<TokMojiDrawableViewModel>();
            //Load TokMoji
            RecyclerTokMoji.SetLayoutManager(new GridLayoutManager(this, 2));
            this.RunOnUiThread(async () => await RunTokMojis()); 

            ExpandableLayoutIsEnglish.Visibility = ViewStates.Gone;

            /*this.SetBinding(() => AddTokVm.TokModel.IsEnglish, () => ChkTokEnglish.Checked, BindingMode.TwoWay);
            this.SetBinding(() => AddTokVm.TokModel.Category, () => TxtAddTokCategory.Text, BindingMode.TwoWay);
            this.SetBinding(() => AddTokVm.TokModel.PrimaryFieldText, () => TxtAddTokPrimaryField.Text, BindingMode.TwoWay);
            this.SetBinding(() => AddTokVm.TokModel.SecondaryFieldText, () => TxtAddTokSecondaryField.Text, BindingMode.TwoWay);
            this.SetBinding(() => AddTokVm.TokModel.EnglishPrimaryFieldText, () => TxtAddTokPrimaryTrans.Text, BindingMode.TwoWay);
            this.SetBinding(() => AddTokVm.TokModel.EnglishSecondaryFieldText, () => TxtAddTokSecondaryTrans.Text, BindingMode.TwoWay);*/

            if (isAddTok)
            {
                if(!string.IsNullOrEmpty(AddTokVm.TokModel.PrimaryFieldText))
                {
                    //TokMoji for primary and secondary
                    ssbName = new SpannableStringBuilder(AddTokVm.TokModel.PrimaryFieldText);
                    spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);
                    TxtAddTokPrimaryField.SetText(spannableResText, TextView.BufferType.Spannable);
                }

                if (!string.IsNullOrEmpty(AddTokVm.TokModel.SecondaryFieldText))
                {
                    ssbName = new SpannableStringBuilder(AddTokVm.TokModel.SecondaryFieldText);
                    spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);
                    TxtAddTokSecondaryField.SetText(spannableResText, TextView.BufferType.Spannable);
                }
            }

            TxtAddTokCategory.AfterTextChanged += (sender, e) =>
            {
                txtCountTokCategory.Text = TxtAddTokCategory.Length().ToString() + "/50";
                if (TxtAddTokCategory.Text.ToUpper() == TokTypeDisplay.Text.ToUpper())
                {
                    Toast.MakeText(this, "Do not use the Tok Type as the Category because it will be redundant.  Instead, use the category to be more specific. e.g. If Tok Type is Music,  enter 1980's, Hit Song, Rock, or The Beatles for Category", ToastLength.Long).Show();
                }
            };

            TokTypeDisplay.AfterTextChanged += (sender, e) =>
            {
                if (TxtAddTokCategory.Text.ToUpper() == TokTypeDisplay.Text.ToUpper())
                {
                    Toast.MakeText(this, "Do not use the Tok Type as the Category because it will be redundant.  Instead, use the category to be more specific. e.g. If Tok Type is Music,  enter 1980's, Hit Song, Rock, or The Beatles for Category", ToastLength.Long).Show();
                }
            };

            TxtAddTokPrimaryField.AfterTextChanged += (sender, e) =>
            {
                txtCountTokPrimaryField.Text = TxtAddTokPrimaryField.Length().ToString() + "/200";
            };

            TxtAddTokSecondaryField.AfterTextChanged += (sender, e) =>
            {
                txtCountTokSecondaryField.Text = TxtAddTokSecondaryField.Length().ToString() + "/200";
            };

            txtAddTokNotes.AfterTextChanged += (sender, e) =>
            {
                txtCountTokNotes.Text = txtAddTokNotes.Length().ToString() + "/2000";
            };

            ArrAnswer = new bool[10];
            AddTokVm.TokModel.DetailImages = new string[10];
            TokDetails = new AddTokDetailModel[10];
            if (isAddTok == false) //If edit is clicked from TokInfoActivity
            {
                this.RunOnUiThread(async () => await EditTok());
            }
            else
            {
                //If Button is Save
                BtnAddTokAddDetail.Visibility = ViewStates.Gone;
                LinearMegaTokDetail.Visibility = ViewStates.Gone;
                ChkTokEnglish.Checked = true;
            }

            BtnCancelTok.Click += (object sender, EventArgs e) =>
            {
                AddTokVm.TokModel = new TokModel();
                Finish();
            };

            BtnAddTokSave.Click += SaveTok_IsClicked;

            ChkTokEnglish.CheckedChange += (object sender, CheckedChangeEventArgs e) =>
            {
                ExpandableLayoutIsEnglish.Visibility = ViewStates.Visible;
                //detailadapter.isEnglish = !ChkTokEnglish.Checked;

                if (LinearMegaTokDetail.Visibility == ViewStates.Visible)
                {
                    for (int x = 0, count = LinearMegaTokDetail.ChildCount; x < count; ++x)
                    {
                        View view = LinearMegaTokDetail.GetChildAt(x);
                        if (ChkTokEnglish.Checked == false)
                        {
                            view.FindViewById<TextView>(Resource.Id.lblAddTokDetailEng1).Visibility = ViewStates.Visible;
                            view.FindViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Visible;
                        }
                        else if (ChkTokEnglish.Checked == true)
                        {
                            view.FindViewById<TextView>(Resource.Id.lblAddTokDetailEng1).Visibility = ViewStates.Gone;
                            view.FindViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Gone;
                        }
                    }
                }
            };

            BtnPrimarySmiley.Click -= SmileyIsClicked;
            BtnPrimarySmiley.Click += SmileyIsClicked;

            BtnSecondarySmiley.Click -= SmileyIsClicked;
            BtnSecondarySmiley.Click += SmileyIsClicked;

            lblTokInfoTokGroupText.SetOnTouchListener(this);
            lblTokInfoTokTypeText.SetOnTouchListener(this);
            lblCategory.SetOnTouchListener(this);
            lblAddTokPrimaryField.SetOnTouchListener(this);
            lblAddTokSecondaryField.SetOnTouchListener(this);
            lblNotes.SetOnTouchListener(this);
         
            OkLevelButton.Click += delegate
            {
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                LinearRankLevel.Visibility = ViewStates.Gone;
                AddTokVm.TokModel = new TokModel();
                Finish();
            };

            TxtAddTokPrimaryField.FocusChange += delegate
            {
                if (TxtAddTokPrimaryField.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtAddTokPrimaryField.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtAddTokPrimaryField.RequestFocus();

                inputManager.ShowSoftInput(TxtAddTokPrimaryField, 0);
            };

            TxtAddTokSecondaryField.FocusChange += delegate
            {
                if (TxtAddTokSecondaryField.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtAddTokSecondaryField.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtAddTokSecondaryField.RequestFocus();

                inputManager.ShowSoftInput(TxtAddTokSecondaryField, 0);
            };

            TxtAddTokCategory.FocusChange += delegate
            {
                if (TxtAddTokCategory.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtAddTokCategory.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtAddTokCategory.RequestFocus();

                inputManager.ShowSoftInput(TxtAddTokCategory, 0);
            };

            TxtLanguageDialect.FocusChange += delegate
            {
                if (TxtLanguageDialect.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtLanguageDialect.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtLanguageDialect.RequestFocus();

                inputManager.ShowSoftInput(TxtLanguageDialect, 0);
            };

            TxtAddTokPrimaryTrans.FocusChange += delegate
            {
                if (TxtAddTokPrimaryTrans.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtAddTokPrimaryTrans.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtAddTokPrimaryTrans.RequestFocus();

                inputManager.ShowSoftInput(TxtAddTokPrimaryTrans, 0);
            };

            TxtAddTokSecondaryTrans.FocusChange += delegate
            {
                if (TxtAddTokSecondaryTrans.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtAddTokSecondaryTrans.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtAddTokSecondaryTrans.RequestFocus();

                inputManager.ShowSoftInput(TxtAddTokSecondaryTrans, 0);
            };

            TxtTokEnglish4.FocusChange += delegate
            {
                if (TxtTokEnglish4.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtTokEnglish4.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtTokEnglish4.RequestFocus();

                inputManager.ShowSoftInput(TxtTokEnglish4, 0);
            };

            TxtTokEnglish5.FocusChange += delegate
            {
                if (TxtTokEnglish5.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtTokEnglish5.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtTokEnglish5.RequestFocus();

                inputManager.ShowSoftInput(TxtTokEnglish5, 0);
            };

            TxtTokEnglish5.FocusChange += delegate
            {
                if (TxtTokEnglish5.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            TxtTokEnglish5.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                TxtTokEnglish5.RequestFocus();

                inputManager.ShowSoftInput(TxtTokEnglish5, 0);
            };

            txtAddTokNotes.FocusChange += delegate
            {
                if (txtAddTokNotes.IsFocused)
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;
                }
            };

            txtAddTokNotes.Click += delegate
            {
                RecyclerTokMoji.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                txtAddTokNotes.RequestFocus();

                inputManager.ShowSoftInput(txtAddTokNotes, 0);
            };
        }

        private async void SaveTok_IsClicked(object sender, EventArgs e)
        {
            RecyclerTokMoji.Visibility = ViewStates.Gone;
            AddTokVm.TokModel.Image = Addtok_imgTokImgMain.ContentDescription;
            if (isAddTok) //If Add
            {
                AddTokVm.TokModel.TokGroup = TokGroupDisplay.Text;
                AddTokVm.TokModel.TokType = TokTypeDisplay.Text;
                AddTokVm.TokModel.CategoryId = "category-" + AddTokVm.TokModel.Category?.ToIdFormat();
                AddTokVm.TokModel.TokTypeId = $"toktype-{AddTokVm.TokModel.TokGroup?.ToIdFormat()}-{AddTokVm.TokModel.TokType?.ToIdFormat()}";
                AddTokVm.TokModel.OptionalFieldValues = new string[LvOptional.Count];
            }

            if (AddTokVm.TokModel.IsEnglish) { AddTokVm.TokModel.Language = "english"; }

            //Optional Fields
            //Getting Values in ListView row | ListView value
            List<string> listOptional = new List<string>();
            for (int x = 0, count = LvOptional.Count; x < count; ++x)
            {
                View view = LvOptional.GetChildAt(x);
                EditText editText = view.FindViewById<EditText>(Resource.Id.txtAddTokOptional);

                listOptional.Add(editText.Text);
            }
            AddTokVm.TokModel.OptionalFieldValues = listOptional.ToArray();

            ResultModel result = new ResultModel();
            var previousLevel = new PointsSymbolModel();
            var nextLevel = new PointsSymbolModel();
            var currentLevel = new PointsSymbolModel();
            var updatedTok = new TokModel();
            var isPromoted = false;

            LinearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
            ProgressBarCircle.IndeterminateDrawable.SetColorFilter(Color.LightBlue, PorterDuff.Mode.Multiply);


            AddTokVm.TokModel.Image = !String.IsNullOrEmpty(AddTokVm.TokModel.Image) ?
                                        "data:image/jpeg;base64," + AddTokVm.TokModel.Image :
                                        AddTokVm.TokModel.Image;

            //Detail
            if (AddTokVm.TokModel.IsDetailBased == true)
            {
                for (int i = 0; i < AddTokVm.TokModel.Details.Length; i++)
                {
                    AddTokVm.TokModel.Details[i] = TokDetails[i].Detail;
                }

                //Image
                for (int i = 0; i < AddTokVm.TokModel.DetailImages.Length; i++)
                {
                    if (!string.IsNullOrEmpty(AddTokVm.TokModel.DetailImages[i]))
                    {
                        if (!URLUtil.IsValidUrl(AddTokVm.TokModel.DetailImages[i]))
                        {
                            AddTokVm.TokModel.DetailImages[i] = "data:image/jpeg;base64," + AddTokVm.TokModel.DetailImages[i];
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

            if (isAddTok)
            {
                //API                      
                ProgressText.Text = "Add Tok...";
                result = await TokService.Instance.CreateTokAsync(AddTokVm.TokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    showRetryDialog("Task was cancelled.");
                }
            }
            else
            {
                //API
                ProgressText.Text = "Updating...";
                result = await TokService.Instance.UpdateTokAsync(AddTokVm.TokModel, cancellationToken);
                if (result.ResultMessage == "cancelled")
                {
                    showRetryDialog("Task was cancelled.");
                }
            }

            //Saving Sections
            if (result.ResultEnum == Result.Success)
            {

                if (isAddTok)
                {
                    //set earned coins
                    if (AddTokVm.TokModel.IsMegaTok.Value)
                    {
                        txtEarnedCoins.Text = "You earned 10 coins!";
                        txtThankyouMessage.Text = "for adding a Mega Tok!";
                    }
                    else if (AddTokVm.TokModel.IsDetailBased)
                    {
                        txtEarnedCoins.Text = "You earned 5 coins!";
                        txtThankyouMessage.Text = "for adding a Detailed Tok!";
                    }
                    else
                    {
                        txtEarnedCoins.Text = "You earned 2 coins!";
                        txtThankyouMessage.Text = "for adding a Basic Tok!";
                    }
                    //start coins animation
                    var GifCoinIcon = FindViewById<ImageView>(Resource.Id.gif_profileCoins);
                    Glide.With(this)
                        .Load(Resource.Drawable.gold).Into(GifCoinIcon);
                    /*Stream input = Resources.OpenRawResource(Resource.Drawable.gold);
                    byte[] bytes = ConvertByteArray(input);
                    GifCoinIcon.SetBytes(bytes);
                    GifCoinIcon.StartAnimation();*/

                    //show earned coins
                    frameCoinsEarned.Visibility = ViewStates.Visible;


                    updatedTok = result.ResultObject as TokModel;

                    //check if promoted to next level
                    currentLevel = PointsSymbolsHelper.GetPatchExactResult(updatedTok.Points);
                    var previousPoints = PointsSymbolsHelper.GetPatchExactResult(updatedTok.PointsPrevious);
                    if (currentLevel.Level != previousPoints.Level)
                    {
                        isPromoted = true;
                        previousLevel = PointsSymbolsHelper.PointsSymbols.FirstOrDefault(x => x.index == currentLevel.index - 1);
                        nextLevel = PointsSymbolsHelper.PointsSymbols.FirstOrDefault(x => x.index == currentLevel.index + 1);
                    }
                }

                if (AddTokVm.TokModel.IsMegaTok == true || AddTokVm.TokModel.TokGroup.ToLower() == "mega") //If Mega
                {
                    var OrigTokSectionResult = await SharedService.TokService.Instance.GetTokSectionsAsync(AddTokVm.TokModel.Id);
                    var OrigTokSection = OrigTokSectionResult.Results;
                    bool isSuccess = false;
                    var cnt = 0;
                    foreach (var sec in AddTokVm.TokModel.Sections)
                    {
                        //Progress Text
                        System.Threading.Thread.Sleep(200);
                        double val1 = (double)(cnt + 1) / (double)AddTokVm.TokModel.Sections.Length;
                        var val2 = val1 * 100;
                        int percent = (int)val2;
                        ProgressText.Text = percent.ToString() + " %";

                        if (string.IsNullOrEmpty(sec.Id.Trim())) //Save
                        {
                            if (isAddTok)
                            {
                                AddTokVm.TokModel.Id = updatedTok.Id;
                            }
                            var dummySec = new TokSection();
                            sec.Id = dummySec.Id;
                            sec.TokId = AddTokVm.TokModel.Id;
                            sec.TokTypeId = AddTokVm.TokModel.TokTypeId;
                            sec.UserId = Settings.GetUserModel().UserId;

                            isSuccess = await SharedService.TokService.Instance.CreateTokSectionAsync(sec, AddTokVm.TokModel.Id, 0);
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
                                    isSuccess = await SharedService.TokService.Instance.UpdateTokSectionAsync(sec);
                                }
                            }
                        }
                        cnt += 1;
                    }
                }
            }
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            btnContinue.Click += delegate
            {
                frameCoinsEarned.Visibility = ViewStates.Gone;
                if (result.ResultEnum == Result.Success)
                {
                    if (!isAddTok)
                    {
                        if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
                        {
                            var modelSerialized = JsonConvert.SerializeObject(AddTokVm.TokModel.Sections.ToList());
                            Intent intent = new Intent();
                            intent.PutExtra("toksection", modelSerialized);
                            SetResult(AppResult.Ok, intent);
                        }
                        else
                        {
                            var modelSerialized = JsonConvert.SerializeObject(AddTokVm.TokModel);
                            Intent intent = new Intent();
                            intent.PutExtra("tokModel", modelSerialized);
                            SetResult(AppResult.Ok, intent);
                        }
                    }
                    else    //Saving
                    {
                        HomeFragment.HFInstance.InsertToksRecyclerAdapter((TokModel)result.ResultObject);
                        ProfileFragment.Instance.AddTokCollection(null, (TokModel)result.ResultObject);

                        if (isPromoted)
                        {
                            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                            var lackingpoints = nextLevel.PointsRequired - updatedTok.Points;
                            Glide.With(this).Load(previousLevel.Image).Into(ImgPreviousLevel);
                            Glide.With(this).Load(currentLevel.Image).Into(ImgCurrentLevel);
                            Glide.With(this).Load(nextLevel.Image).Into(ImgNextLevel);
                            TextPromotionRank.Text = "You have been promoted to " + currentLevel.Name;
                            TextPointsRank.Text = lackingpoints + " points away from the next rank.";

                            LinearRankLevel.Visibility = ViewStates.Visible;
                        }
                    }
                    AddTokVm.TokModel = new TokModel();
                    Finish();
                }
            };
        }
        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(MainActivity.Instance)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {

                            })
                            .SetNegativeButton("Retry", (_, args) =>
                            {
                                SaveTok_IsClicked(_, args);
                            })
                            .SetCancelable(false)
                            .Show();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                LinearToast.Visibility = ViewStates.Gone;
            }
            else if (e.Action == MotionEventActions.Down)
            {
                if (v.ContentDescription == "Tok Group" ||
                    v.ContentDescription == "Tok Type" ||
                    v.ContentDescription == "Category" ||
                    v.ContentDescription == "Primary Field Text" ||
                    v.ContentDescription == "Secondary Field Text" ||
                    v.ContentDescription == "Notes")
                {
                    LinearToast.Visibility = ViewStates.Visible;
                    CoordinatorLayout.LayoutParams p = (CoordinatorLayout.LayoutParams)LinearToast.LayoutParameters;
                    switch (v.Id)
                    {
                        case Resource.Id.lblTokInfoTokGroupText:
                            p.AnchorId = Resource.Id.lblTokInfoTokGroupText;
                            TextLabelToast.Text = "Tok Groups organize the Tokkepedia Tok Types.";
                            break;
                        case Resource.Id.lblTokInfoTokTypeText:
                            p.AnchorId = Resource.Id.lblTokInfoTokTypeText;
                            TextLabelToast.Text = "Tok Types organize Tokkepedia information. They are structured to easily review knowledge that is shared and to make the toks playable in our Tokket games.";
                            break;
                        case Resource.Id.lblCategory:
                            p.AnchorId = Resource.Id.lblCategory;
                            TextLabelToast.Text = "Area inside of the Tok Type that the Tok can fall under. It can be anything except the name of the Tok Group/Tok Type.";
                            break;
                        case Resource.Id.lblAddTokPrimaryField:
                            p.AnchorId = Resource.Id.lblAddTokPrimaryField;
                            TextLabelToast.Text = "Please select a Tok Group and a Tok Type.";
                            break;
                        case Resource.Id.lblAddTokSecondaryField:
                            p.AnchorId = Resource.Id.lblAddTokSecondaryField;
                            TextLabelToast.Text = "Please select a Tok Group and a Tok Type.";
                            break;
                        case Resource.Id.lblNotes:
                            p.AnchorId = Resource.Id.lblNotes;
                            TextLabelToast.Text = "Add additional relevant info here.";
                            break;
                    }

                }
            }
            return true;
        }
        
        /*private byte[] ConvertByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }*/
        
        private async Task RunTokMojis()
        {
            TokInfoVm.TokMojiCollection.Clear();

            await TokInfoVm.LoadTokMoji();

            SpannableHelper.ListTokMoji = TokInfoVm.TokMojiCollection.ToList();

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
        private async Task EditTok()
        {
            AddTokVm.TokModel = JsonConvert.DeserializeObject<TokModel>(Intent.GetStringExtra("tokModel"));
            TokGroupDisplay.Text = AddTokVm.TokModel.TokGroup;
            TokTypeDisplay.Text = AddTokVm.TokModel.TokType;

            TokGroupTextTitle.Text = "Tok Group:";
            TokTypeTexTitle.Text = "Tok Type:";

            TxtAddTokCategory.Text = AddTokVm.TokModel.Category;

            ssbName = new SpannableStringBuilder(AddTokVm.TokModel.PrimaryFieldText);
            spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);
            TxtAddTokPrimaryField.SetText(spannableResText, TextView.BufferType.Spannable);

            ssbName = new SpannableStringBuilder(AddTokVm.TokModel.SecondaryFieldText);
            spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);
            TxtAddTokSecondaryField.SetText(spannableResText, TextView.BufferType.Spannable);

            //TxtAddTokPrimaryField.Text = AddTokVm.TokModel.PrimaryFieldText;
            //TxtAddTokSecondaryField.Text = AddTokVm.TokModel.SecondaryFieldText;
            ChkTokEnglish.Checked = AddTokVm.TokModel.IsEnglish;
            Addtok_imgTokImgMain.ContentDescription = AddTokVm.TokModel.Image;
            Glide.With(this).Load(AddTokVm.TokModel.Image).Into(Addtok_imgTokImgMain);

            FrameAddtok.Visibility = ViewStates.Gone;
            LinearTokGroupSample.Visibility = ViewStates.Gone;
            LblAddTokSample.Visibility = ViewStates.Gone;
            TokGroupDisplay.Visibility = ViewStates.Visible;
            Cbxtokgroup.Visibility = ViewStates.Gone;
            TokTypeDisplay.Visibility = ViewStates.Visible;
            Cbxtoktype.Visibility = ViewStates.Gone;

            BtnAddTokSave.Text = "Update Tok";

            //Tok Group
            spinnerTGPosition = AddTokVm.TokGroup.FindIndex(c => c.TokGroup == TokGroupDisplay.Text);
            Cbxtokgroup.SetSelection(spinnerTGPosition);

            //Tok Type
            for (int i = 0; i < AddTokVm.TokGroup[spinnerTGPosition].TokTypes.Length; i++)
            {
                if (AddTokVm.TokGroup[spinnerTGPosition].TokTypes.ToString() == TokTypeDisplay.Text)
                {
                    Cbxtoktype.SetSelection(i);
                    break;
                }
            }
            //LVTokDetail.Adapter = AddTokVm.TokModelCollection.GetAdapter(GetDetailAdapter);
            if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
            {
                var GetToksSecResult = await SharedService.TokService.Instance.GetTokSectionsAsync(AddTokVm.TokModel.Id);
                var GetToksSec = GetToksSecResult.Results;
                AddTokVm.TokModel.Sections = GetToksSec.ToArray();

                if (AddTokVm.TokModel.Sections == null || AddTokVm.TokModel.Sections.Length == 0)
                {
                    AddTokVm.TokModel.Sections = new TokSection[1];
                }
                AddMegaSection(AddTokVm.TokModel.Sections);
            }
            else
            {
                if (AddTokVm.TokModel.IsDetailBased)
                {
                    LinearMegaTokDetail.Visibility = ViewStates.Visible;

                    var detail = new List<string>();
                    var engdetail = new List<string>();
                    for (int i = 0; i < AddTokVm.TokModel.Details.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(AddTokVm.TokModel.Details[i]))
                        {
                            if (TokDetails[i] == null)
                            {
                                TokDetails[i] = new AddTokDetailModel();
                            }

                            TokDetails[i].Detail = AddTokVm.TokModel.Details[i];
                            detail.Add(AddTokVm.TokModel.Details[i]);

                            if (AddTokVm.TokModel.EnglishDetails != null)
                            {
                                if (i < AddTokVm.TokModel.EnglishDetails.Length)
                                {
                                    TokDetails[i].EnglishDetail = AddTokVm.TokModel.EnglishDetails[i];
                                    engdetail.Add(AddTokVm.TokModel.EnglishDetails[i]);
                                }
                            }
                        }
                    }
                    AddTokVm.TokModel.Details = detail.ToArray();
                    AddTokVm.TokModel.EnglishDetails = engdetail.ToArray();

                    //If total image is lesser than the details, add new
                    var detailImages = AddTokVm.TokModel.DetailImages.ToList();
                    for (int i = detailImages.Count; i < 10; i++)
                    {
                        detailImages.Add(null);
                    }
                    AddTokVm.TokModel.DetailImages = detailImages.ToArray();
                    AddDetail();
                }
            }

            //Optional Fields
            optionaladapter = new AddTokOptionalAdapter(this, AddTokVm.TokGroup[spinnerTGPosition].OptionalFields, AddTokVm.TokGroup[spinnerTGPosition].OptionalLimits);
            LvOptional.Adapter = optionaladapter;
            paramEngdetail = LvOptional.LayoutParameters;
            lvEngdetailHeightdec = Convert.ToInt32(AddTokVm.TokGroup[spinnerTGPosition].OptionalFields.Length) * 90;
            lvEngdetailHeighfloat = (float)lvEngdetailHeightdec;
            paramEngdetail.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, lvEngdetailHeighfloat, Resources.DisplayMetrics);
            LvOptional.LayoutParameters = paramEngdetail;

            TxtAddTokPrimaryField.Enabled = true;
            TxtAddTokSecondaryField.Enabled = true;
            isEnglishPrimary(spinnerTGPosition);
            if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
            {
                LinearMegaTokDetail.Visibility = ViewStates.Visible;
                BtnAddTokAddDetail.Visibility = ViewStates.Visible;
                BtnAddTokAddDetail.Text = "+ Add Section";
            }
            else
            {
                if (AddTokVm.TokModel.IsDetailBased && LinearMegaTokDetail.ChildCount < 10)
                {
                    BtnAddTokAddDetail.Visibility = ViewStates.Visible;
                }
            }



        }
        private void SmileyIsClicked(object sender, EventArgs e)
        {
            var BtnSmile = (sender as ImageView);
            string fromsmile = BtnSmile.ContentDescription;

            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            if (RecyclerTokMoji.Visibility == ViewStates.Gone)
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_2b);
                inputManager.HideSoftInputFromWindow(BtnSmile.WindowToken, HideSoftInputFlags.None);
                RecyclerTokMoji.Visibility = ViewStates.Visible;
            }
            else
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMoji.Visibility = ViewStates.Gone;
                View TextEditor = TxtAddTokPrimaryField;
                if (fromsmile == "primary")
                {
                    TextEditor = TxtAddTokPrimaryField;
                }
                else if (fromsmile == "secondary")
                {
                    TextEditor = TxtAddTokSecondaryField;
                }
                else if (fromsmile == "detail")
                {
                    detailSmilepos = (int)BtnSmile.Tag;
                    View v = LinearMegaTokDetail.GetChildAt(detailSmilepos);
                    var DetailEditor = v.FindViewById<EditText>(Resource.Id.txtAddTokDetailField);
                    TextEditor = DetailEditor;
                }

                TextEditor.RequestFocus();
                inputManager.ShowSoftInput(TextEditor, 0);
            }

            if (fromsmile == "detail")
            {
                detailSmilepos = (int)BtnSmile.Tag;
            }

            SmileySelected = fromsmile;
        }
        private async Task InitializeData()
        {
            tokketUser = await SharedService.AccountService.Instance.GetUserAsync(userid);
            //User Data
            AddTokVm.TokModel.UserDisplayName = tokketUser.DisplayName;
            AddTokVm.TokModel.UserId = userid;
            AddTokVm.TokModel.UserCountry = tokketUser.Country;
            AddTokVm.TokModel.UserPhoto = tokketUser.UserPhoto;
        }
        public void loadTokGroup()
        {
            Cbxtokgroup.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Cbxtokgroup_ItemSelected);
            List<string> tokgroupList = new List<string>();

            tokgroupList.Add("Choose...");
            for (int i = 0; i < AddTokVm.TokGroup.Count(); i++)
            {
                tokgroupList.Add(AddTokVm.TokGroup[i].TokGroup);
            }
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, tokgroupList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            Cbxtokgroup.Adapter = Aadapter;
        }

       
        private void Cbxtokgroup_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            spinnerTGPosition = e.Position;
            TokGroupDisplay.Text = Cbxtokgroup.GetItemAtPosition(e.Position).ToString();

            if (TokGroupDisplay.Text.ToLower() == "basic")
            {
                linearNotes.Visibility = ViewStates.Visible;
            }
            else
            {
                linearNotes.Visibility = ViewStates.Gone;
            }

            if (TokGroupDisplay.Text.ToLower() != "mega")
            {
                AddTokVm.TokModel.IsMegaTok = false;
            }

            if (AddTokVm.TokGroup[e.Position].IsDetailBased == true)
            {
                BtnAddTokAddDetail.Visibility = ViewStates.Visible;
                LinearMegaTokDetail.Visibility = ViewStates.Visible;
            }
            else
            {
                BtnAddTokAddDetail.Visibility = ViewStates.Gone;
                LinearMegaTokDetail.Visibility = ViewStates.Gone;
            }
            
            AddTokVm.TokModel.IsDetailBased = AddTokVm.TokGroup[e.Position].IsDetailBased;

            if(e.Position > 0)
            {
                loadTokType(e.Position - 1);                                                    

                if (isAddTok)
                {
                    TxtAddTokPrimaryField.Enabled = false;
                    TxtAddTokSecondaryField.Enabled = false;
                }

                isEnglishPrimary(e.Position - 1);

                if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
                {
                    AddTokVm.TokModel.Sections = new TokSection[1];

                    AddMegaSection(AddTokVm.TokModel.Sections);
                    AddTokVm.TokModel.IsMegaTok = true;
                    BtnAddTokAddDetail.Text = "+ Add Section";
                }
                else
                {
                    AddTokVm.TokModel.SecondaryFieldName = AddTokVm.TokGroup[e.Position].SecondaryFieldName;
                    AddTokVm.TokModel.IsMegaTok = false;
                    AddTokVm.TokModel.Details = new string[3];
                    BtnAddTokAddDetail.Text = "+ Add " + AddTokVm.TokGroup[e.Position].SecondaryFieldName;
                    AddDetail();
                }


                //Optional Fields
                optionaladapter = new AddTokOptionalAdapter(this, AddTokVm.TokGroup[e.Position].OptionalFields, AddTokVm.TokGroup[e.Position].OptionalLimits);
                LvOptional.Adapter = optionaladapter;
                paramEngdetail = LvOptional.LayoutParameters;
                lvEngdetailHeightdec = Convert.ToInt32(AddTokVm.TokGroup[e.Position].OptionalFields.Length) * 90;
                lvEngdetailHeighfloat = (float)lvEngdetailHeightdec;
                paramEngdetail.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, lvEngdetailHeighfloat, Resources.DisplayMetrics);
                LvOptional.LayoutParameters = paramEngdetail;
            }
           
        }
        private void isEnglishPrimary(int position)
        {
            var lblprimaryfieldtext = FindViewById<TextView>(Resource.Id.lblAddTokPrimaryField);
            var lblsecondaryfieldtext = FindViewById<TextView>(Resource.Id.lblAddTokSecondaryField);
            var linearSecondary = FindViewById<LinearLayout>(Resource.Id.linearAddTokSecondary);
            var lblAddTokDescription = FindViewById<TextView>(Resource.Id.lblAddTokDescription);

            lblAddTokDescription.Text = AddTokVm.TokGroup[position].Description;
            lblsecondaryfieldtext.Text = AddTokVm.TokGroup[position].SecondaryFieldName;
            LblAddTokPrimaryTrans.Text = AddTokVm.TokGroup[position].PrimaryFieldName + " - English Translation";
            LblAddTokSecondaryTrans.Text = AddTokVm.TokGroup[position].SecondaryFieldName + " - English Translation";

            if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
            {
                lblsecondaryfieldtext.Visibility = ViewStates.Gone;
                InputlayoutSecondaryField.Visibility = ViewStates.Gone;
                lblprimaryfieldtext.Text = "Tok Title";
            }
            else
            {
                lblprimaryfieldtext.Text = AddTokVm.TokGroup[position].PrimaryFieldName;
            }

            if (lblsecondaryfieldtext.Text.ToLower() == "detail" && AddTokVm.TokGroup[position].IsDetailBased == true)
            {
                linearSecondary.Visibility = ViewStates.Gone;

                LblAddTokSecondaryTrans.Visibility = ViewStates.Gone;
                TxtAddTokSecondaryTrans.Visibility = ViewStates.Gone;
            }
            else
            {
                linearSecondary.Visibility = ViewStates.Visible;

                LblAddTokSecondaryTrans.Visibility = ViewStates.Visible;
                TxtAddTokSecondaryTrans.Visibility = ViewStates.Visible;
            }

            if (AddTokVm.TokGroup[position].TokGroup.ToLower() == "abbreviation")
            {
                LblTokEnglish4.Visibility = ViewStates.Visible;
                LblTokEnglish5.Visibility = ViewStates.Visible;
                TxtTokEnglish4.Visibility = ViewStates.Visible;
                TxtTokEnglish5.Visibility = ViewStates.Visible;
            }
            else
            {
                LblTokEnglish4.Visibility = ViewStates.Gone;
                LblTokEnglish5.Visibility = ViewStates.Gone;
                TxtTokEnglish4.Visibility = ViewStates.Gone;
                TxtTokEnglish5.Visibility = ViewStates.Gone;
            }
        }
        public void loadTokType(int ndx)
        {
            Cbxtoktype.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Cbxtoktype_ItemSelected);
            List<string> tokgroupList = new List<string>();
            for (int i = 0; i < AddTokVm.TokGroup[ndx].TokTypes.Length; i++)
            {
                tokgroupList.Add(AddTokVm.TokGroup[ndx].TokTypes[i]);
            }

            var Aadapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, tokgroupList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            Cbxtoktype.Adapter = Aadapter;
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
                if (SmileySelected == "primary")
                {
                    CommentEditor = TxtAddTokPrimaryField;

                    if (TxtAddTokPrimaryField.IsFocused == false)
                    {
                        TxtAddTokPrimaryField.RequestFocus();
                    }

                    start = TxtAddTokPrimaryField.SelectionStart;
                    
                }
                else if (SmileySelected == "secondary")
                {
                    CommentEditor = TxtAddTokSecondaryField;

                    if (TxtAddTokSecondaryField.IsFocused == false)
                    {
                        TxtAddTokSecondaryField.RequestFocus();
                    }

                    start = TxtAddTokSecondaryField.SelectionStart;
                }
                else if (SmileySelected == "detail")
                {
                    //int ndx = 0;
                    //try { ndx = (int)(sender as ImageView).Tag; } catch { ndx = int.Parse((string)(sender as ImageView).Tag); }

                    View v = LinearMegaTokDetail.GetChildAt(detailSmilepos);
                    var DetailEditor = v.FindViewById<EditText>(Resource.Id.txtAddTokDetailField);

                    CommentEditor = DetailEditor;

                    if (DetailEditor.IsFocused == false)
                    {
                        DetailEditor.RequestFocus();
                    }
                    start = DetailEditor.SelectionStart;
                }
                //int start = CommentEditor.SelectionStart;

                string tokmojiidx = (sender as ImageView).ContentDescription;
                string tokidx = ":" + tokmojiidx + ":";


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
                    CommentEditor.Text = CommentEditor.Text.Substring(0, start) + tokidx + CommentEditor.Text.Substring(start);

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
                    CommentEditor.SetSelection(start + tokidx.Length);
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
        private void Cbxtoktype_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            TokTypeDisplay.Text = Cbxtoktype.GetItemAtPosition(e.Position).ToString();
            LblAddTokSample.Text = e.Position < AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].Descriptions.Count() ?
                                        AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].Descriptions[e.Position] :
                                        "";
            LblAddTokSample.Text += " \n";
            LblAddTokSample.Text += " \n";
            LblAddTokSample.Text += e.Position < AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].Examples.Count() ?
                                        "Example: " + AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].Examples[e.Position]:
                                        "";

            InputlayoutPrimaryFieldText.CounterMaxLength = (int)AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].PrimaryCharLimit;
            InputlayoutSecondaryField.CounterMaxLength = (int)AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].SecondaryCharLimit;

            TxtAddTokPrimaryField.SetFilters(new IInputFilter[] { new InputFilterLengthFilter((int)AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].PrimaryCharLimit) });
            TxtAddTokSecondaryField.SetFilters(new IInputFilter[] { new InputFilterLengthFilter((int)AddTokVm.TokGroup[Cbxtokgroup.LastVisiblePosition].SecondaryCharLimit) });

            if (isAddTok)
            {
                TxtAddTokPrimaryField.Enabled = true;
                TxtAddTokSecondaryField.Enabled = true;
            }
        }
        private void AddMegaSection(TokSection[] tokSection)
        {
            LinearMegaTokDetail.RemoveAllViews();
            for (int i = 0; i < tokSection.Length; i++)
            {
                if (tokSection[i] == null)
                {
                    tokSection[i] = new TokSection();
                    tokSection[i].Id = " ";
                }

                View v = LayoutInflater.Inflate(Resource.Layout.addtokmegadetail_row, null);
                var Title = v.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle);
                var Content = v.FindViewById<EditText>(Resource.Id.txtAddTokMegaContent);
                var Image = v.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg);
                var RelaveAddTokMegaImg = v.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg);
                var SectionNumber = v.FindViewById<TextView>(Resource.Id.txtAddTokMegaNumber);

                SectionNumber.Text = (i + 1).ToString();

                var titleBinding = new Binding<string, string>(tokSection[i],
                                                  () => tokSection[i].Title,
                                                  Title,
                                                  () => Title.Text,
                                                  BindingMode.TwoWay);
                var contentBinding = new Binding<string, string>(tokSection[i],
                                                  () => tokSection[i].Content,
                                                  Content,
                                                  () => Content.Text,
                                                  BindingMode.TwoWay);

                Image.ContentDescription = i.ToString();
                if (URLUtil.IsValidUrl(tokSection[i].Image))
                {
                    Glide.With(this).Load(tokSection[i].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(Image);
                }
                else
                {
                    Image.Tag = i;
                    byte[] imageDetailBytes = Convert.FromBase64String(tokSection[i].Image ?? "");
                    Image.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }

                Image.Click += delegate
                {
                    Bitmap imgBitmap = ((BitmapDrawable)Image.Drawable).Bitmap;
                    MemoryStream byteArrayOutputStream = new MemoryStream();
                    imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                    byte[] byteArray = byteArrayOutputStream.ToArray();

                    Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
                    Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
                    this.StartActivity(nextActivity);
                };

                if (tokSection[i].Image != null)
                {
                    Image.SetBackgroundColor(Color.ParseColor("#3498db"));
                    RelaveAddTokMegaImg.Visibility = ViewStates.Visible;
                }

                //Adding tag
                v.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_img).Tag = i;
                var DeleteBtn = v.FindViewById<Button>(Resource.Id.btnAddTokMega_deletedtlField1);
                DeleteBtn.Tag = i;
                v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaTitle).Tag = i;
                v.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = i;
                v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaContent).Tag = i;
                Title.Tag = i;
                v.FindViewById<ImageView>(Resource.Id.btnAddTokMega_deleteImg).Tag = i;
                v.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Tag = i;

                if (i > 0)
                {
                    DeleteBtn.Visibility = ViewStates.Visible;
                }

                LinearMegaTokDetail.AddView(v, new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
            }
            AddTokVm.TokModel.Sections = tokSection;

            LinearMegaTokDetail.Visibility = ViewStates.Visible;
        }
        private void AddDetail()
        {
            LinearMegaTokDetail.RemoveAllViews();
            string SecondaryFieldName = AddTokVm.TokGroup[Cbxtokgroup.FirstVisiblePosition].SecondaryFieldName;
            int DtlCntr = 1;
            for (int i = 0; i < AddTokVm.TokModel.Details.Length; i++)
            {
                if (TokDetails[i] == null)
                {
                    TokDetails[i] = new AddTokDetailModel();
                }

                if (AddTokVm.TokModel.EnglishDetails != null)
                {
                    AddTokVm.TokModel.EnglishDetails[i] = TokDetails[i].EnglishDetail;
                }

                View v = LayoutInflater.Inflate(Resource.Layout.addtokdetail_row, null);
                var DeleteImage = v.FindViewById<Button>(Resource.Id.btnDeltokdtl_img);
                var AddImage = v.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img);
                var CheckAnswer = v.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField);
                var inputAddTokDetailField = v.FindViewById<TextInputLayout>(Resource.Id.inputAddTokDetailField);
                var EnglishHeader = v.FindViewById<TextView>(Resource.Id.lblAddTokDetailEng1);
                var Detail = v.FindViewById<EditText>(Resource.Id.txtAddTokDetailField);
                var EnglishDetail = v.FindViewById<EditText>(Resource.Id.txtAddTokDetailFieldEngTrans);
                var btnDeleteDetail = v.FindViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1);
                var ImgDetailSmiley = v.FindViewById<ImageView>(Resource.Id.btnAddTokDetailSmiley);

                ImgDetailSmiley.Tag = i;
                ImgDetailSmiley.Click -= SmileyIsClicked;
                ImgDetailSmiley.Click += SmileyIsClicked;


                Detail.FocusChange += delegate
                {
                    if (Detail.IsFocused)
                    {
                        RecyclerTokMoji.Visibility = ViewStates.Gone;
                    }
                };

                Detail.Click += delegate
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;

                    var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                    Detail.RequestFocus();

                    inputManager.ShowSoftInput(Detail, 0);
                };

                EnglishDetail.FocusChange += delegate
                {
                    if (EnglishDetail.IsFocused)
                    {
                        RecyclerTokMoji.Visibility = ViewStates.Gone;
                    }
                };

                EnglishDetail.Click += delegate
                {
                    RecyclerTokMoji.Visibility = ViewStates.Gone;

                    var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                    EnglishDetail.RequestFocus();

                    inputManager.ShowSoftInput(EnglishDetail, 0);
                };

                var detailBinding = new Binding<string, string>(TokDetails[i],
                                                      () => TokDetails[i].Detail,
                                                      Detail,
                                                      () => Detail.Text,
                                                      BindingMode.TwoWay);

                var englishdetailBinding = new Binding<string, string>(TokDetails[i],
                                                      () => TokDetails[i].EnglishDetail,
                                                      EnglishDetail,
                                                      () => EnglishDetail.Text,
                                                      BindingMode.TwoWay);

                var chkAnswerBinding = new Binding<bool, bool>(TokDetails[i],
                                                     () => TokDetails[i].ChkAnswer,
                                                     CheckAnswer,
                                                     () => CheckAnswer.Checked,
                                                     BindingMode.TwoWay);

                CheckAnswer.Checked = ArrAnswer[i];
                AddImage.Tag = i;
                if (i < 3) //Default detail is 3
                {
                    btnDeleteDetail.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnDeleteDetail.Visibility = ViewStates.Visible;
                }

                v.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField).Tag = i;
                v.FindViewById<Button>(Resource.Id.btnDeltokdtl_img).Tag = i;

                if (!CheckAnswer.Checked)
                {
                    inputAddTokDetailField.Hint = SecondaryFieldName + " " + DtlCntr;
                    DtlCntr += 1;
                }
                else
                {
                    inputAddTokDetailField.Hint = "Answer";
                }

                v.FindViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1).Tag = i;

                if (i < AddTokVm.TokModel.DetailImages.Length)
                {
                    if (!string.IsNullOrEmpty(AddTokVm.TokModel.DetailImages[i]))
                    {
                        AddImage.SetImageBitmap(null);
                        DeleteImage.Visibility = ViewStates.Visible;
                    }

                    if (URLUtil.IsValidUrl(AddTokVm.TokModel.DetailImages[i]))
                    {
                        ImageDownloaderHelper.AssignImageAsync(AddImage, AddTokVm.TokModel.DetailImages[i], this);
                    }
                    else
                    {
                        byte[] imageDetailBytes = Convert.FromBase64String(AddTokVm.TokModel.DetailImages[i] ?? "");
                        AddImage.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    }
                }

                if (ChkTokEnglish.Checked == false)
                {
                    EnglishHeader.Text = "English Translation";
                    EnglishHeader.Visibility = ViewStates.Visible;
                    v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Visible;
                }
                else
                {
                    EnglishHeader.Visibility = ViewStates.Gone;
                    v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutDetailEnglishTrans).Visibility = ViewStates.Gone;
                }

                //This is for TokMoji Display
                ssbName = new SpannableStringBuilder(Detail.Text);
                spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
                Detail.SetText(spannableResText, TextView.BufferType.Spannable);

                LinearMegaTokDetail.AddView(v, new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent));
            }

            if (LinearMegaTokDetail.ChildCount < 10)
            {
                if (AddTokVm.TokGroup[Cbxtokgroup.FirstVisiblePosition].IsDetailBased == true)
                {
                    BtnAddTokAddDetail.Visibility = ViewStates.Visible;
                }
            }
        }
        private void RemoveMegaSection(int ndx)
        {
            var sectionList = AddTokVm.TokModel.Sections.ToList();
            sectionList.RemoveAt(ndx);

            AddTokVm.TokModel.Sections = sectionList.ToArray();

            LinearMegaTokDetail.RemoveViewAt(ndx);

            for (int i = 0; i < LinearMegaTokDetail.ChildCount; ++i)
            {
                View v = LinearMegaTokDetail.GetChildAt(i);

                //Adding tag
                v.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_img).Tag = i;
                v.FindViewById<Button>(Resource.Id.btnAddTokMega_deletedtlField1).Tag = i;
                v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaTitle).Tag = i;
                v.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = i;
                v.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg).Tag = i;
                v.FindViewById<TextInputLayout>(Resource.Id.inputlayoutAddTokMegaContent).Tag = i;
                v.FindViewById<EditText>(Resource.Id.EdittxtAddTokMegaTitle).Tag = i;
                v.FindViewById<ImageView>(Resource.Id.btnAddTokMega_deleteImg).Tag = i;
                v.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Tag = i;
                v.FindViewById<TextView>(Resource.Id.txtAddTokMegaNumber).Text = (i + 1).ToString();
            }
        }

        [Java.Interop.Export("OnClickAddDetail")]
        public void OnClickAddDetail(View v)
        {
            //detailadapter.AddItem(tokGroup[Cbxtokgroup.FirstVisiblePosition].SecondaryFieldName + " " + ((int)listview.Count + 1));

            if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
            {
                TokSection[] myArr = AddTokVm.TokModel.Sections;
                Array.Resize<TokSection>(ref myArr, AddTokVm.TokModel.Sections.Length + 1);
                AddMegaSection(myArr);
            }
            else
            {
                string[] arrdetail = AddTokVm.TokModel.Details;
                Array.Resize<string>(ref arrdetail, AddTokVm.TokModel.Details.Length + 1);
                AddTokVm.TokModel.Details = arrdetail;

                string[] arrengdetail = AddTokVm.TokModel.EnglishDetails;
                Array.Resize<string>(ref arrengdetail, AddTokVm.TokModel.Details.Length + 1);
                AddTokVm.TokModel.EnglishDetails = arrengdetail;

                AddDetail();
                BtnAddTokAddDetail.Visibility = ViewStates.Visible;

                if (LinearMegaTokDetail.ChildCount == 10)
                {
                    BtnAddTokAddDetail.Visibility = ViewStates.Gone;
                }
            }
        }

        [Java.Interop.Export("OnDelete")] // The value found in android:onClick attribute.
        public void OnDelete(View v) // Does not need to match value in above attribute.
        {
            int vtag = (int)v.Tag;
            if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
            {
                RemoveMegaSection(vtag);
            }
            else
            {

                var detailList = AddTokVm.TokModel.Details.ToList();
                detailList.RemoveAt(vtag);
                AddTokVm.TokModel.Details = detailList.ToArray();

                var engdetailList = AddTokVm.TokModel.EnglishDetails.ToList();
                engdetailList.RemoveAt(vtag);
                AddTokVm.TokModel.EnglishDetails = engdetailList.ToArray();

                TokDetails = new AddTokDetailModel[10];

                LinearMegaTokDetail.RemoveViewAt(vtag);

                int DtlCntr = 1;
                for (int i = 0; i < LinearMegaTokDetail.ChildCount; ++i)
                {
                    View view = LinearMegaTokDetail.GetChildAt(i);
                    var CheckAnswer = view.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField);
                    var inputAddTokDetailField = view.FindViewById<TextInputLayout>(Resource.Id.inputAddTokDetailField);
                    if (!CheckAnswer.Checked)
                    {
                        inputAddTokDetailField.Hint = AddTokVm.TokModel.SecondaryFieldName + " " + DtlCntr;
                        DtlCntr += 1;
                    }
                    else
                    {
                        inputAddTokDetailField.Hint = "Answer";
                    }

                    if (TokDetails[i] == null)
                    {
                        TokDetails[i] = new AddTokDetailModel();
                        TokDetails[i].Detail = view.FindViewById<EditText>(Resource.Id.txtAddTokDetailField).Text;
                        TokDetails[i].EnglishDetail = view.FindViewById<EditText>(Resource.Id.txtAddTokDetailFieldEngTrans).Text;

                        AddTokVm.TokModel.Details[i] = TokDetails[i].Detail;
                        if (i < AddTokVm.TokModel.EnglishDetails.Length)
                        {
                            AddTokVm.TokModel.EnglishDetails[i] = TokDetails[i].EnglishDetail;
                        }
                    }

                    view.FindViewById<CheckBox>(Resource.Id.chkAddTokChkDtlField).Tag = i;
                    view.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img).Tag = i;
                    view.FindViewById<Button>(Resource.Id.btnDeltokdtl_img).Tag = i;
                    view.FindViewById<TextView>(Resource.Id.btnAddTok_deletedtlField1).Tag = i;
                }

                BtnAddTokAddDetail.Visibility = ViewStates.Visible;

                if (vtag < AddTokVm.TokModel.DetailImages.Length)
                {
                    AddTokVm.TokModel.DetailImages.ToList().RemoveAt(vtag);
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
                if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
                {
                    view = LinearMegaTokDetail.GetChildAt(x);
                    RelativeLayout RelaveAddTokMegaImg = view.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg);
                    RelaveAddTokMegaImg.Visibility = ViewStates.Gone;
                }
                else
                {
                    view = LinearMegaTokDetail.GetChildAt(x);
                    ImageView btnAddtokdtl_img = view.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img);
                    btnAddtokdtl_img.SetImageResource(Resource.Drawable.add_image_icon);
                    v.Visibility = ViewStates.Gone;

                    if (AddTokVm.TokModel.DetailImages.Length <= (vtag + 1))
                    {
                        AddTokVm.TokModel.DetailImages[vtag] = null;
                    }
                }
            }
        }

        [Java.Interop.Export("OnAddTokSticker")]
        public void OnAddTokSticker(View v)
        {
            AddTokVm.TokModel.UserCountry = tokketUser.UserPhoto;
            AddTokVm.TokModel.UserPhoto = tokketUser.UserPhoto;
            AddTokVm.TokModel.UserDisplayName = tokketUser.DisplayName;
            if (isAddTok)
            {
                AddTokVm.TokModel.TokGroup = Cbxtokgroup.GetItemAtPosition(Cbxtokgroup.FirstVisiblePosition).ToString();
                AddTokVm.TokModel.TokType = Cbxtoktype.GetItemAtPosition(Cbxtoktype.FirstVisiblePosition).ToString();
            }
           
            AddTokVm.TokModel.Category = TxtAddTokCategory.Text;
            AddTokVm.TokModel.PrimaryFieldText = TxtAddTokPrimaryField.Text;
            AddTokVm.TokModel.EnglishPrimaryFieldText = TxtAddTokPrimaryTrans.Text;
            AddTokVm.TokModel.CategoryId = "category-" + AddTokVm.TokModel.Category?.ToIdFormat();
            AddTokVm.TokModel.TokTypeId = $"toktype-{AddTokVm.TokModel.TokGroup?.ToIdFormat()}-{AddTokVm.TokModel.TokType?.ToIdFormat()}";
            AddTokVm.TokModel.IsDetailBased = AddTokVm.TokGroup[Cbxtokgroup.FirstVisiblePosition].IsDetailBased;

            if (AddTokVm.TokModel.IsEnglish) { AddTokVm.TokModel.Language = "english"; }

            var modelConvert = JsonConvert.SerializeObject(AddTokVm.TokModel);
            Intent nextActivity = new Intent(this, typeof(AddStickerDialogActivity));
            nextActivity.PutExtra("tokModel", modelConvert);
            this.StartActivity(nextActivity);
        }
        [Java.Interop.Export("OnTapTip")]
        public void OnTapTip(View v)
        {
            FrameAddtok.Visibility = ViewStates.Gone;
            FrameAddtokPreview.Visibility = ViewStates.Gone;
            FramePreviewCard.Visibility = ViewStates.Gone;


            btnClosePreview.Visibility = ViewStates.Gone;
            linearPreviewButtons.Visibility = ViewStates.Visible;
        }



        [Java.Interop.Export("OnCheckAddTok")]
        public void OnCheckAddTok(View v)
        {
            //Clear all checked
            int vtag = (int)v.Tag;

            ArrAnswer[vtag] = (v as CheckBox).Checked;
            AddDetail();
        }

        [Java.Interop.Export("OnClickPreviewTile")]
        public void OnClickPreviewTile(View v)
        {

            btnClosePreview.Visibility = ViewStates.Visible;
            linearPreviewButtons.Visibility = ViewStates.Gone;

            FramePreviewCard.Visibility = ViewStates.Gone;
            GradientDrawable tokimagedrawable;
            AssetManager assetManager = Application.Context.Assets;
            Stream sr = null;
            Bitmap bitmap = null;
            try
            {
                sr = assetManager.Open("Flags/" + tokketUser.Country + ".jpg");
                bitmap = BitmapFactory.DecodeStream(sr);
            }
            catch (Exception)
            {

            }

            int position = Cbxtokgroup.FirstVisiblePosition;
            if (AddTokVm.TokModel.Image != null) //addtok_imagebrowse
            {
                var imageUserPhoto = FindViewById<ImageView>(Resource.Id.addtok_imageTokImgUserPhoto);
                Glide.With(this).Load(tokketUser.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(imageUserPhoto);
                if (bitmap != null) FindViewById<ImageView>(Resource.Id.addtok_img_tokimgFlag).SetImageBitmap(bitmap);
                FindViewById<TextView>(Resource.Id.addtok_lbl_tokimgprimarytext).Text = TxtAddTokPrimaryField.Text;
                FindViewById<TextView>(Resource.Id.addtok_lblTokImgCategory).Text = TxtAddTokCategory.Text;
                FindViewById<TextView>(Resource.Id.addtok_lblTokImgGroup).Text = Cbxtokgroup.GetItemAtPosition(Cbxtokgroup.FirstVisiblePosition).ToString();
                if (Cbxtoktype.Adapter  != null)  FindViewById<TextView>(Resource.Id.addtok_lblTokImgType).Text = Cbxtoktype.GetItemAtPosition(Cbxtoktype.FirstVisiblePosition).ToString();

                FindViewById<GridLayout>(Resource.Id.gridTokPreviewImage).SetBackgroundResource(Resource.Drawable.tileview_layout);
                tokimagedrawable = (GradientDrawable)FindViewById<GridLayout>(Resource.Id.gridTokPreviewImage).Background;
                tokimagedrawable.SetColor(Color.White);

                FindViewById<GridLayout>(Resource.Id.gridTokPreview).Visibility = ViewStates.Gone;
                FindViewById<GridLayout>(Resource.Id.gridTokPreviewImage).Visibility = ViewStates.Visible;
            }
            else
            {
                var imageUserPhoto = FindViewById<ImageView>(Resource.Id.imagePreviewUserPhoto);
                Glide.With(this).Load(tokketUser.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(imageUserPhoto);
                if(bitmap != null) FindViewById<ImageView>(Resource.Id.imagePreviewFlag).SetImageBitmap(bitmap);
                FindViewById<TextView>(Resource.Id.lbl_previewnameuser).Text = tokketUser.DisplayName;
                FindViewById<TextView>(Resource.Id.lbl_PreviewPrimaryField).Text = TxtAddTokPrimaryField.Text;
                FindViewById<TextView>(Resource.Id.lbl_PreviewenglishPrimaryFieldText).Text = TxtAddTokPrimaryTrans.Text;
                FindViewById<TextView>(Resource.Id.lblPreviewTokGroup).Text = Cbxtokgroup.GetItemAtPosition(Cbxtokgroup.FirstVisiblePosition).ToString();
                if (Cbxtoktype.Adapter != null) FindViewById<TextView>(Resource.Id.lblPreviewTokType).Text = Cbxtoktype.GetItemAtPosition(Cbxtoktype.FirstVisiblePosition).ToString();
                FindViewById<TextView>(Resource.Id.lblPreviewCategory).Text = TxtAddTokCategory.Text;

                var gridTokPreview = FindViewById<GridLayout>(Resource.Id.gridTokPreview);
                var tokdrawable = FindViewById<GridLayout>(Resource.Id.gridTokPreview);
                gridTokPreview.SetBackgroundResource(Resource.Drawable.tileview_layout);
                int ndx = position % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                tokimagedrawable = (GradientDrawable)tokdrawable.Background;
                tokimagedrawable.SetColor(Color.ParseColor(randomcolors[ndx]));

                FindViewById<GridLayout>(Resource.Id.gridTokPreview).Visibility = ViewStates.Visible;
                FindViewById<GridLayout>(Resource.Id.gridTokPreviewImage).Visibility = ViewStates.Gone;
            }

            FrameAddtokPreview.Visibility = ViewStates.Visible;
        }

        [Java.Interop.Export("OnClickPreviewInfo")]
        public void OnClickPreviewInfo(View v)
        {
            btnClosePreview.Visibility = ViewStates.Visible;
            linearPreviewButtons.Visibility = ViewStates.Gone;

            AddTokVm.TokModel.Category = AddTokVm.TokModel.Category + "";
            AddTokVm.TokModel.PrimaryFieldText = AddTokVm.TokModel.PrimaryFieldText + "";
            AddTokVm.TokModel.UserCountry = tokketUser.UserPhoto;
            AddTokVm.TokModel.UserPhoto = tokketUser.UserPhoto;
            AddTokVm.TokModel.UserDisplayName = tokketUser.DisplayName;
            AddTokVm.TokModel.TokGroup = Cbxtokgroup.GetItemAtPosition(Cbxtokgroup.FirstVisiblePosition).ToString();
            AddTokVm.TokModel.TokType = Cbxtoktype.Adapter != null ? Cbxtoktype.GetItemAtPosition(Cbxtoktype.FirstVisiblePosition).ToString() : "";

            AddTokVm.TokModel.CategoryId = "category-" + AddTokVm.TokModel.Category?.ToIdFormat();
            AddTokVm.TokModel.TokTypeId = $"toktype-{AddTokVm.TokModel.TokGroup?.ToIdFormat()}-{AddTokVm.TokModel.TokType?.ToIdFormat()}";
            AddTokVm.TokModel.IsDetailBased = AddTokVm.TokGroup[Cbxtokgroup.FirstVisiblePosition].IsDetailBased;
            
            if (AddTokVm.TokModel.IsEnglish) { AddTokVm.TokModel.Language = "english"; }

            if (AddTokVm.TokModel != null)
            {
                var modelConvert = JsonConvert.SerializeObject(AddTokVm.TokModel);
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
                nextActivity.PutExtra("tokModel", modelConvert);
                this.StartActivity(nextActivity);
            }
        }

        [Java.Interop.Export("OnClickPreviewUser")]
        public void OnClickPreviewUser(View v)
        {
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", userid);
            this.StartActivity(nextActivity);
        }

        [Java.Interop.Export("OnClickAddTokImgDetail")]
        public void OnClickAddTokImgDetail(View v)
        {
            Settings.BrowsedImgTag = (int)v.Tag;//(int)v.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img).Tag;
            bottomsheet = new BottomSheetUserPhotoFragment(this, (ImageView)v);
            bottomsheet.Show(this.SupportFragmentManager, "tag");

            /*Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);*/
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            bottomsheet = new BottomSheetUserPhotoFragment(this, Addtok_imagebrowse);
            bottomsheet.Show(this.SupportFragmentManager, "tag");

            /*Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddTokActivityType);*/
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            Addtok_imagebrowse.SetImageBitmap(null);
            Addtok_imgTokImgMain.SetImageBitmap(null);
            AddTokVm.TokModel.Image = null;

            BtnAddTok_btnBrowseImage.Visibility = ViewStates.Visible;
            BtnAddTokRemoveImgMain.Visibility = ViewStates.Gone;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] AppResult resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddTokActivityType) && (resultCode == AppResult.Ok) && (data != null))
            {
                NetUri uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);

                if (Settings.BrowsedImgTag != -1)
                {
                    int vtag = Settings.BrowsedImgTag;

                    if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
                    {
                    }
                    else
                    {
                        for (int x = vtag; x == vtag; ++x)
                        {
                            View view = LinearMegaTokDetail.GetChildAt(x);
                            Button btnDeltokdtl_img = view.FindViewById<Button>(Resource.Id.btnDeltokdtl_img);
                            btnDeltokdtl_img.Visibility = ViewStates.Visible;
                        }
                    }
                }
            }

        }
        public void displayImageBrowse()
        {
            //Main Image
            Addtok_imgTokImgMain.SetImageBitmap(null);
            if (Settings.BrowsedImgTag == -1)
            {
                //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
                Addtok_imgTokImgMain.ContentDescription = Settings.ImageBrowseCrop;
                byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                Addtok_imagebrowse.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                Addtok_imgTokImgMain.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                BtnAddTok_btnBrowseImage.Visibility = ViewStates.Gone;
                BtnAddTokRemoveImgMain.Visibility = ViewStates.Visible;
            }
            else
            {
                //Detail Image
                int detailpos = Settings.BrowsedImgTag; //Position of Control
                byte[] imageDetailBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                
                if (AddTokVm.TokModel.IsMegaTok == true || TokGroupDisplay.Text.ToLower() == "mega") //If Mega
                {
                    View view = LinearMegaTokDetail.GetChildAt(detailpos);
                    view.FindViewById<RelativeLayout>(Resource.Id.RelaveAddTokMegaImg).Visibility = ViewStates.Visible;
                    ImageView btnAddtokdtl_img = view.FindViewById<ImageView>(Resource.Id.btnAddtokmegadtl_displayimg);

                    btnAddtokdtl_img.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    AddTokVm.TokModel.Sections[detailpos].Image = Settings.ImageBrowseCrop;
                }
                else
                {
                    View view = LinearMegaTokDetail.GetChildAt(detailpos);
                    ImageView btnAddtokdtl_img = view.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img);
                    btnAddtokdtl_img.Visibility = ViewStates.Visible;
                    AddTokVm.TokModel.DetailImages[detailpos] = Settings.ImageBrowseCrop;
                    btnAddtokdtl_img.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));

                    //Show delete button for image detail
                    view.FindViewById<View>(Resource.Id.btnDeltokdtl_img).Visibility = ViewStates.Visible;
                }
            }
            Settings.ImageBrowseCrop = null;
        }
        [Java.Interop.Export("OnClickbtnImgCrop")]
        public void OnClickbtnImgCrop(View v)
        {
            if (ExpandableLayoutImgCrop.Visibility == ViewStates.Visible)
            {
                ExpandableLayoutImgCrop.Visibility = ViewStates.Gone;
            }
            else if (ExpandableLayoutImgCrop.Visibility == ViewStates.Gone)
            {
                ExpandableLayoutImgCrop.Visibility = ViewStates.Visible;
            }
        }

        [Java.Interop.Export("OnClickPreviewCard")]
        public void OnClickPreviewCard(View v)
        {
            FrameAddtokPreview.Visibility = ViewStates.Gone;
            gesturedetector = new GestureDetector(this, new MyGestureListener(this));
            //if (this == null)
            //{
            AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();

            btnClosePreview.Visibility = ViewStates.Visible;
            linearPreviewButtons.Visibility = ViewStates.Gone;

            trans.Add(Resource.Id.framePreviewCard, new CardFrontFragment());
            trans.Commit();

            //}
            FramePreviewCard.Visibility = ViewStates.Visible;
        }
        private void FlipCard()
        {
            if (Showingback)
            {
                SupportFragmentManager.PopBackStack();
                Showingback = false;
            }
            else
            {
                AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();
                trans.SetCustomAnimations(Resource.Animation.card_flip_right_in, Resource.Animation.card_flip_right_out, Resource.Animation.card_flip_left_in, Resource.Animation.card_flip_left_out);
                trans.Replace(Resource.Id.framePreviewCard, new CardBackFragment());
                trans.AddToBackStack(null);
                trans.Commit();
                Showingback = true;
            }
        }
        private void clearForm(ViewGroup group)
        {
            for (int i = 0, count = group.ChildCount; i < count; ++i)
            {
                View view = group.GetChildAt(i);
                if (view is EditText)
                {
                    ((EditText)view).Text = "";
                }

                if (view is ViewGroup && (((ViewGroup)view).ChildCount > 0))
                    clearForm((ViewGroup)view);
            }

            AddTokVm.TokModel.Details = new string[3];
            AddTokVm.TokModel.EnglishDetails = new string[3];
            AddTokVm.TokModel.OptionalFieldValues = new string[LvOptional.Count];
            LinearMegaTokDetail.RemoveAllViews();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    AddTokVm.TokModel = new TokModel();
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void OnBackPressed()
        {
            AddTokVm.TokModel = new TokModel();
            base.OnBackPressed();
        }
        private class CardFrontFragment : AndroidX.Fragment.App.Fragment
        {
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                AddTokActivity addTokActivity = Activity as AddTokActivity;
                View frontcard_view = inflater.Inflate(Resource.Layout.preview_cardfront, container, false);
                var lblPreviewCardFront = frontcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardFront);
                if (!string.IsNullOrEmpty(addTokActivity.TxtAddTokPrimaryField.Text))
                {
                    lblPreviewCardFront.Text = addTokActivity.TxtAddTokPrimaryField.Text;
                }
                frontcard_view.Touch += Frontcard_view_Touch;
                return frontcard_view;
            }

            private void Frontcard_view_Touch(object sender, View.TouchEventArgs e)
            {
                AddTokActivity myactivity = Activity as AddTokActivity;

                myactivity.gesturedetector.OnTouchEvent(e.Event);
            }
        }
        private class CardBackFragment : AndroidX.Fragment.App.Fragment
        {
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                AddTokActivity addTokActivity = Activity as AddTokActivity;
                View backcard_view = inflater.Inflate(Resource.Layout.preview_cardback, container, false);
                var lblPreviewCardFront = backcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardBack);
                if (!string.IsNullOrEmpty(addTokActivity.TxtAddTokSecondaryField.Text))
                {
                    lblPreviewCardFront.Text = addTokActivity.TxtAddTokSecondaryField.Text;
                }

                backcard_view.Touch += Backcard_view_Touch;

                return backcard_view;
            }

            private void Backcard_view_Touch(object sender, View.TouchEventArgs e)
            {
                AddTokActivity myactivity = Activity as AddTokActivity;

                myactivity.gesturedetector.OnTouchEvent(e.Event);
            }
        }
        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private AddTokActivity mainActivity;

            public MyGestureListener(AddTokActivity Activity)
            {
                mainActivity = Activity;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                //Console.WriteLine("Double Tab");
                return true;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                mainActivity.FlipCard();
                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {
                Console.WriteLine("Long Press");
            }
            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {

                Console.WriteLine("Fling");
                return base.OnFling(e1, e2, velocityX, velocityY);
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                Console.WriteLine("Scroll");
                return base.OnScroll(e1, e2, distanceX, distanceY);
            }
        }
        private View GetDetailAdapter(int position, TokModel tokModel, View convertView)
        {
            // Not reusing views here
            convertView = LayoutInflater.Inflate(Resource.Layout.addtokdetail_row, null);

            var editTextDetail = convertView.FindViewById<EditText>(Resource.Id.txtAddTokDetailField);
            var editTextEnglish = convertView.FindViewById<EditText>(Resource.Id.txtAddTokDetailFieldEngTrans);
            var btnAddtokdtl_img = convertView.FindViewById<ImageView>(Resource.Id.btnAddtokdtl_img);

            var inputAddTokDetailField = FindViewById<TextInputLayout>(Resource.Id.inputAddTokDetailField);
            inputAddTokDetailField.Hint = AddTokVm.TokGroup[spinnerTGPosition].SecondaryFieldName + " " + (position + 1);

            editTextDetail.Text = AddTokVm.TokModel.Details[position];
            editTextEnglish.Text = AddTokVm.TokModel.EnglishDetails[position];
            Glide.With(this).Load(AddTokVm.TokModel.DetailImages[position]).Into(btnAddtokdtl_img);
            return convertView;
        }

        #region UI Properties                                                     
        public TextView lblTokInfoTokGroupText => FindViewById<TextView>(Resource.Id.lblTokInfoTokGroupText);
        public TextView lblTokInfoTokTypeText => FindViewById<TextView>(Resource.Id.lblTokInfoTokTypeText);
        public TextView lblCategory => FindViewById<TextView>(Resource.Id.lblCategory);
        public TextView lblAddTokPrimaryField => FindViewById<TextView>(Resource.Id.lblAddTokPrimaryField);
        public TextView lblAddTokSecondaryField => FindViewById<TextView>(Resource.Id.lblAddTokSecondaryField);
        public TextView lblNotes => FindViewById<TextView>(Resource.Id.lblNotes);
        public LinearLayout linearNotes => FindViewById<LinearLayout>(Resource.Id.linearNotes);

        public TextView TextLabelToast => FindViewById<TextView>(Resource.Id.TextLabelToast);
        public LinearLayout LinearToast => FindViewById<LinearLayout>(Resource.Id.LinearToast);
        public LinearLayout ExpandableLayoutIsEnglish => FindViewById<LinearLayout>(Resource.Id.expandableLayoutIsEnglish);
        public RadioGroup ExpandableLayoutImgCrop => FindViewById<RadioGroup>(Resource.Id.expandableLayoutImgCrop);
        public CheckBox ChkTokEnglish => FindViewById<CheckBox>(Resource.Id.chkTokEnglish);
        public TextView BtnAddTokSave => FindViewById<TextView>(Resource.Id.btnAddTokSave);
        public TextView BtnCancelTok => FindViewById<TextView>(Resource.Id.btnAddTokCancel);
        public TextView LblAddTokSample => FindViewById<TextView>(Resource.Id.lblAddTokSample);
        public TextInputLayout InputlayoutPrimaryFieldText => FindViewById<TextInputLayout>(Resource.Id.inputlayoutPrimaryFieldText);
        public TextInputLayout InputlayoutSecondaryField => FindViewById<TextInputLayout>(Resource.Id.inputlayoutSecondaryField);
        public EditText TxtAddTokPrimaryField => FindViewById<EditText>(Resource.Id.txtAddTokPrimaryField);
        public EditText TxtAddTokSecondaryField => FindViewById<EditText>(Resource.Id.txtAddTokSecondaryField);
        public TextView LblTokEnglish1 => FindViewById<TextView>(Resource.Id.lblTokEnglish1);
        public TextView LblAddTokPrimaryTrans => FindViewById<TextView>(Resource.Id.lblAddTokPrimaryTrans);
        public TextView LblAddTokSecondaryTrans => FindViewById<TextView>(Resource.Id.lblAddTokSecondaryTrans);
        public TextView LblTokEnglish4 => FindViewById<TextView>(Resource.Id.lblTokEnglish4);
        public TextView LblTokEnglish5 => FindViewById<TextView>(Resource.Id.lblTokEnglish5);
        public TextView TokGroupDisplay => FindViewById<TextView>(Resource.Id.txtTokGroupDisplay);
        public TextView TokTypeDisplay => FindViewById<TextView>(Resource.Id.txtTokTypeDisplay);
        public Spinner Cbxtokgroup => FindViewById<Spinner>(Resource.Id.txtAddTokTokGroup);
        public Spinner Cbxtoktype => FindViewById<Spinner>(Resource.Id.txtAddTokTokType);
        public EditText TxtAddTokCategory => FindViewById<EditText>(Resource.Id.txtAddTokCategory);
        public EditText TxtLanguageDialect => FindViewById<EditText>(Resource.Id.txtLanguageDialect);
        public EditText TxtAddTokPrimaryTrans => FindViewById<EditText>(Resource.Id.txtAddTokPrimaryTrans);
        public EditText TxtAddTokSecondaryTrans => FindViewById<EditText>(Resource.Id.txtAddTokSecondaryTrans);
        public EditText TxtTokEnglish4 => FindViewById<EditText>(Resource.Id.txtTokEnglish4);
        public EditText TxtTokEnglish5 => FindViewById<EditText>(Resource.Id.txtTokEnglish5);
        public Button BtnAddTokAddDetail => FindViewById<Button>(Resource.Id.btnAddTokAddDetail);
        public Button BtnAddTokImgCropOptions => FindViewById<Button>(Resource.Id.btnAddTokImgCropOptions);
        public Button BtnAddTok_btnBrowseImage => FindViewById<Button>(Resource.Id.btnAddTok_btnBrowseImage);
        public Button BtnAddTokRemoveImgMain => FindViewById<Button>(Resource.Id.btnAddTokRemoveImgMain);
        public GridLayout GridAddTok => FindViewById<GridLayout>(Resource.Id.gridAddTok);
        public LinearLayout LinearMegaTokDetail => FindViewById<LinearLayout>(Resource.Id.LinearAddTokMegaDtl);
        public ListView LvOptional => FindViewById<ListView>(Resource.Id.listViewAddTokOptionalFields);
        public FrameLayout FrameAddtok => FindViewById<FrameLayout>(Resource.Id.frameAddtok);
        public FrameLayout FrameAddtokPreview => FindViewById<FrameLayout>(Resource.Id.frameAddtokPreview);
        public FrameLayout FramePreviewCard => FindViewById<FrameLayout>(Resource.Id.framePreviewCard);
        public ImageView Addtok_imagebrowse => FindViewById<ImageView>(Resource.Id.addtok_imagebrowse);
        public ImageView Addtok_imgTokImgMain => FindViewById<ImageView>(Resource.Id.addtok_imgTokImgMain);
        public LinearLayout LinearTokGroupSample => FindViewById<LinearLayout>(Resource.Id.linearTokGroupSample);
        public ProgressBar ProgressBarCircle => FindViewById<ProgressBar>(Resource.Id.progressbarAddTok);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextAddTok);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_addtok);
        public TextView TokGroupTextTitle => FindViewById<TextView>(Resource.Id.lblTokInfoTokGroupText);
        public TextView TokTypeTexTitle => FindViewById<TextView>(Resource.Id.lblTokInfoTokTypeText);
        public RecyclerView RecyclerTokMoji => FindViewById<RecyclerView>(Resource.Id.addtokRecyclerTokMojis);
        public ImageView BtnPrimarySmiley => FindViewById<ImageView>(Resource.Id.btnAddTokPrimarySmiley);
        public ImageView BtnSecondarySmiley => FindViewById<ImageView>(Resource.Id.btnAddTokSecondarySmiley);
        public LinearLayout LinearRankLevel => FindViewById<LinearLayout>(Resource.Id.LinearRankLevel);
        public TextView TextPromotionRank => FindViewById<TextView>(Resource.Id.txtAddTokPromotionRank);
        public TextView TextPointsRank => FindViewById<TextView>(Resource.Id.txtAddTokPointsRank);
        public ImageView ImgPreviousLevel => FindViewById<ImageView>(Resource.Id.ImgPreviousLevel);
        public ImageView ImgCurrentLevel => FindViewById<ImageView>(Resource.Id.ImgCurrentLevel);
        public ImageView ImgNextLevel => FindViewById<ImageView>(Resource.Id.ImgNextLevel);
        public Button OkLevelButton => FindViewById<Button>(Resource.Id.btnAddTokOKLevel);
        public EditText txtAddTokNotes => FindViewById<EditText>(Resource.Id.txtAddTokNotes);
        public TextView txtThankyouMessage => FindViewById<TextView>(Resource.Id.txtThankyouMessage);
        public TextView txtEarnedCoins => FindViewById<TextView>(Resource.Id.txtEarnedCoins);
        public TextView txtCountTokCategory => FindViewById<TextView>(Resource.Id.txtCountTokCategory);
        public TextView txtCountTokPrimaryField => FindViewById<TextView>(Resource.Id.txtCountTokPrimaryField);
        public TextView txtCountTokSecondaryField => FindViewById<TextView>(Resource.Id.txtCountTokSecondaryField);
        public TextView txtCountTokNotes => FindViewById<TextView>(Resource.Id.txtCountTokNotes);
        public Button btnClosePreview => FindViewById<Button>(Resource.Id.btnClosePreview);
        public LinearLayout linearPreviewButtons => FindViewById<LinearLayout>(Resource.Id.linearPreviewButtons);
        public FrameLayout frameCoinsEarned => FindViewById<FrameLayout>(Resource.Id.frameCoinsEarned);
        public Button btnContinue => FindViewById<Button>(Resource.Id.btnContinue);

        #endregion
    }
}