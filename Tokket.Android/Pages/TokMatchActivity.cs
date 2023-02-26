using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Extensions;
using Tokket.Core;
using System.IO;
using Android.Views.Animations;
using Tokket.Android.ViewModels;
using Bumptech.Glide.Request;
using Bumptech.Glide;
using Tokket.Android.Helpers;
using ImageViews.Photo;
using Android.Animation;
using Tokket.Shared.ViewModels;
using AndroidX.Preference;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Android.Fragments;

namespace Tokket.Android
{
    [Activity(Label = "Tok Match", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokMatchActivity : BaseActivity, View.IOnTouchListener
    {

        //float MAX_X_MOVE = 80, MAX_Y_MOVE = 80;
        GlideImgListener GListenerImage; 
        AlertDialog alert;
        private List<TokModel> tokListRound; RoundModel resCards; ClassSetViewModel classSetVM;
        private List<TokModel> restokList, TokLists; public Set setList; ClassSetModel classsetModel; string IntentTokList;
        Button btnTokMatchLeftArrow, btnTokMatchRightArrow, btnTokMatchCheck, btnTokMatchReset;
        Button btnTokMatchDroppable1, btnTokMatchDroppable2, btnTokMatchDroppable3, btnTokMatchDroppable4, btnTokMatchDropped1, btnTokMatchDropped2, btnTokMatchDropped3, btnTokMatchDropped4;
        FrameLayout frame_tokmatchdropzone1, frame_tokmatchdropzone2, frame_tokmatchdropzone3, frame_tokmatchdropzone4, frameTokMatchDroppable1, frameTokMatchDroppable2, frameTokMatchDroppable3, frameTokMatchDroppable4;
        FrameLayout frame_tokmatchback1, frame_tokmatchback2, frame_tokmatchback3, frame_tokmatchback4;
        ImageView IVTokMatchCheckWrong1, IVTokMatchCheckWrong2, IVTokMatchCheckWrong3, IVTokMatchCheckWrong4;
        Button btnToDrop, btnTokMatchQuit, btnTokMatchNext; TextView lblTokMatchSecondary1, lblTokMatchSecondary2, lblTokMatchSecondary3, lblTokMatchSecondary4, lblTokMatchScore, lblTokMatchResult;
        int CurrentRound = 0,TotalCards, MaxRound, DataCounter,CardRowNum = 0;
        ViewFlipper viewflipper_tokmatch1, viewflipper_tokmatch2, viewflipper_tokmatch3, viewflipper_tokmatch4;
        bool Showingback = true, isNormalMode = true, isCheck = false, isPrevClick=false; IList<CardsHistoryModel> cardHistoryList;
        ImageView gif_TokMatchCoin; ViewFlipper ViewFlipperNormalNext;
        bool isSet = true; int totalScore = 0, presentCards = 0, cardsUsed =0;
        List<int> arrayNumAnswers; List<TokModel> tokModelList;
        public HomePageViewModel HomeVm => App.Locator.HomePageVM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.tokmatch_page);
            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.tokmatch_toolbar);

            SetSupportActionBar(tokback_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            TokLists = new List<TokModel>();
            isSet = Intent.GetBooleanExtra("isSet", isSet);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            IntentTokList = prefs.GetString("TokModelList", "");
            if (string.IsNullOrEmpty(IntentTokList) || IntentTokList == "null")
            {
                IntentTokList = Intent.GetStringExtra("TokModelList");
            }

            //IntentTokList = Intent.GetStringExtra("TokList");

            SupportActionBar.Subtitle = Intent.GetStringExtra("SubTitle");

            ViewFlipperNormalNext = FindViewById<ViewFlipper>(Resource.Id.ViewFlipperNormalNext);
            btnTokMatchQuit = FindViewById<Button>(Resource.Id.btnTokMatchQuit);
            btnTokMatchQuit.Click -= OnQuit_Click;
            btnTokMatchQuit.Click += OnQuit_Click;

            btnTokMatchNext = FindViewById<Button>(Resource.Id.btnTokMatchNext);
            btnTokMatchNext.Click -= OnNext_Click;
            btnTokMatchNext.Click += OnNext_Click;
            
            gif_TokMatchCoin = FindViewById<ImageView>(Resource.Id.gif_TokMatchCoin);
            Glide.With(this)
                .Load(Resource.Drawable.coin)
                .Into(gif_TokMatchCoin);

            /*gif_TokMatchCoin = FindViewById<GifImageView>(Resource.Id.gif_TokMatchCoin);
            Stream input = Resources.OpenRawResource(Resource.Drawable.coin);
            byte[] bytes = ConvertByteArray(input);
            gif_TokMatchCoin.SetBytes(bytes);
            gif_TokMatchCoin.StartAnimation();*/


            //ViewFlipper
            viewflipper_tokmatch1 = FindViewById<ViewFlipper>(Resource.Id.viewflipper_tokmatch1);
            viewflipper_tokmatch2 = FindViewById<ViewFlipper>(Resource.Id.viewflipper_tokmatch2);
            viewflipper_tokmatch3 = FindViewById<ViewFlipper>(Resource.Id.viewflipper_tokmatch3);
            viewflipper_tokmatch4 = FindViewById<ViewFlipper>(Resource.Id.viewflipper_tokmatch4);
            IVTokMatchCheckWrong1 = FindViewById<ImageView>(Resource.Id.IVTokMatchCheckWrong1);
            IVTokMatchCheckWrong2 = FindViewById<ImageView>(Resource.Id.IVTokMatchCheckWrong2);
            IVTokMatchCheckWrong3 = FindViewById<ImageView>(Resource.Id.IVTokMatchCheckWrong3);
            IVTokMatchCheckWrong4 = FindViewById<ImageView>(Resource.Id.IVTokMatchCheckWrong4);

            //Back
            frame_tokmatchback1 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchback1);
            frame_tokmatchback2 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchback2);
            frame_tokmatchback3 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchback3);
            frame_tokmatchback4 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchback4);

            btnTokMatchCheck = FindViewById<Button>(Resource.Id.btnTokMatchCheck);
            btnTokMatchCheck.Click -= OnCheck_Click;
            btnTokMatchCheck.Click += OnCheck_Click;
            btnTokMatchCheck.Visibility = ViewStates.Gone;
            btnTokMatchReset = FindViewById<Button>(Resource.Id.btnTokMatchReset);
            btnTokMatchLeftArrow = FindViewById<Button>(Resource.Id.btnTokMatchLeftArrow);
            btnTokMatchRightArrow = FindViewById<Button>(Resource.Id.btnTokMatchRightArrow);
            //Set Font Style
            Typeface font = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            btnTokMatchLeftArrow.SetTypeface(font, TypefaceStyle.Bold);
            btnTokMatchRightArrow.SetTypeface(font, TypefaceStyle.Bold);
            btnOptions.Typeface = font;

            btnEyeIcon1.Typeface = font;
            btnEyeIcon2.Typeface = font;
            btnEyeIcon3.Typeface = font;
            btnEyeIcon4.Typeface = font;
            btnEyeIcon1.SetOnTouchListener(this);
            btnEyeIcon2.SetOnTouchListener(this);
            btnEyeIcon3.SetOnTouchListener(this);
            btnEyeIcon4.SetOnTouchListener(this);

            btnSecondaryEyeIcon1.Typeface = font;
            btnSecondaryEyeIcon2.Typeface = font;
            btnSecondaryEyeIcon3.Typeface = font;
            btnSecondaryEyeIcon4.Typeface = font;

            btnSecondaryEyeIcon1.SetOnTouchListener(this);
            btnSecondaryEyeIcon2.SetOnTouchListener(this);
            btnSecondaryEyeIcon3.SetOnTouchListener(this);
            btnSecondaryEyeIcon4.SetOnTouchListener(this);

            lblTokMatchResult = FindViewById<TextView>(Resource.Id.lblTokMatchResult);
            lblTokMatchSecondary1 = FindViewById<TextView>(Resource.Id.lblTokMatchSecondary1);
            lblTokMatchSecondary2 = FindViewById<TextView>(Resource.Id.lblTokMatchSecondary2);
            lblTokMatchSecondary3 = FindViewById<TextView>(Resource.Id.lblTokMatchSecondary3);
            lblTokMatchSecondary4 = FindViewById<TextView>(Resource.Id.lblTokMatchSecondary4);
            lblTokMatchScore = FindViewById<TextView>(Resource.Id.lblTokMatchScore);

            btnTokMatchDroppable1 = FindViewById<Button>(Resource.Id.btnTokMatchDroppable1);
            btnTokMatchDroppable2 = FindViewById<Button>(Resource.Id.btnTokMatchDroppable2);
            btnTokMatchDroppable3 = FindViewById<Button>(Resource.Id.btnTokMatchDroppable3);
            btnTokMatchDroppable4 = FindViewById<Button>(Resource.Id.btnTokMatchDroppable4);
            btnTokMatchDropped1 = FindViewById<Button>(Resource.Id.btnTokMatchDropped1);
            btnTokMatchDropped2 = FindViewById<Button>(Resource.Id.btnTokMatchDropped2);
            btnTokMatchDropped3 = FindViewById<Button>(Resource.Id.btnTokMatchDropped3);
            btnTokMatchDropped4 = FindViewById<Button>(Resource.Id.btnTokMatchDropped4);

            frame_tokmatchdropzone1 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchdropzone1);
            frame_tokmatchdropzone2 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchdropzone2);
            frame_tokmatchdropzone3 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchdropzone3);
            frame_tokmatchdropzone4 = FindViewById<FrameLayout>(Resource.Id.frame_tokmatchdropzone4);

            frameTokMatchDroppable1 = FindViewById<FrameLayout>(Resource.Id.frameTokMatchDroppable1);
            frameTokMatchDroppable2 = FindViewById<FrameLayout>(Resource.Id.frameTokMatchDroppable2);
            frameTokMatchDroppable3 = FindViewById<FrameLayout>(Resource.Id.frameTokMatchDroppable3);
            frameTokMatchDroppable4 = FindViewById<FrameLayout>(Resource.Id.frameTokMatchDroppable4);
            DropZoneGone();

            btnTokMatchDroppable1.SetOnTouchListener(this);
            btnTokMatchDroppable2.SetOnTouchListener(this);
            btnTokMatchDroppable3.SetOnTouchListener(this);
            btnTokMatchDroppable4.SetOnTouchListener(this);

            //Dropped Control
            btnTokMatchDropped1.SetOnTouchListener(this);
            btnTokMatchDropped2.SetOnTouchListener(this);
            btnTokMatchDropped3.SetOnTouchListener(this);
            btnTokMatchDropped4.SetOnTouchListener(this);

            // Attach event to drop zone
            frame_tokmatchdropzone1.Drag -= DropZone_Drag;
            frame_tokmatchdropzone1.Drag += DropZone_Drag;

            frame_tokmatchdropzone2.Drag -= DropZone_Drag;
            frame_tokmatchdropzone2.Drag += DropZone_Drag;

            frame_tokmatchdropzone3.Drag -= DropZone_Drag;
            frame_tokmatchdropzone3.Drag += DropZone_Drag;

            frame_tokmatchdropzone4.Drag -= DropZone_Drag;
            frame_tokmatchdropzone4.Drag += DropZone_Drag;

            frameTokMatchDroppable1.Drag -= DropZone_Drag;
            frameTokMatchDroppable1.Drag += DropZone_Drag;

            frameTokMatchDroppable2.Drag -= DropZone_Drag;
            frameTokMatchDroppable2.Drag += DropZone_Drag;

            frameTokMatchDroppable3.Drag -= DropZone_Drag;
            frameTokMatchDroppable3.Drag += DropZone_Drag;

            frameTokMatchDroppable4.Drag -= DropZone_Drag;
            frameTokMatchDroppable4.Drag += DropZone_Drag;

            btnTokMatchLeftArrow.Click -= OnBack_Click;
            btnTokMatchLeftArrow.Click += OnBack_Click;
            btnTokMatchRightArrow.Click -= OnNext_Click;
            btnTokMatchRightArrow.Click += OnNext_Click;

            btnTokMatchReset.Click -= OnReset_Click;
            btnTokMatchReset.Click += OnReset_Click;

            btnOptions.Click += delegate
            {
                Bundle args = new Bundle();
                args.PutInt("trueFalseMode", Convert.ToInt16(trueFalseMode));
                args.PutBoolean("isMultilinetext", isMultilinetext);
                args.PutBoolean("isRetryOnlyIncorrectMode", isRetryOnlyIncorrectMode);
                args.PutBoolean("isNormalMode", isNormalMode);

                AndroidX.AppCompat.App.AppCompatDialogFragment newFragment = new ModalTokMatchOptions();
                newFragment.Arguments = args;
                newFragment.Show(SupportFragmentManager, "Options");
            };

            RunOnUiThread(async () => await InitializeData());
        }
        private async Task GetClassToksAsync()
        {
            try
            {
                var classtokRes = await ClassService.Instance.GetClassToksAsync(
                    new GetClassToksRequest() { 
                        QueryValues = new ClassTokQueryValues() { partitionkeybase = $"{classsetModel.Id}-classtoks", publicfeed = false }
                    }
                  );
                classSetVM.ClassToks = classtokRes.Results.ToList();
                classSetVM.ClassSet = classsetModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task InitializeData()
        {
            showBlueLoading(this);
            if (isSet == true)
            {
                string StringExtra = Intent.GetStringExtra("setModel");
                if (StringExtra != null)
                {
                    setList = JsonConvert.DeserializeObject<Set>(StringExtra);
                }
                else
                {
                    StringExtra = Intent.GetStringExtra("classsetModel");
                    classsetModel = JsonConvert.DeserializeObject<ClassSetModel>(StringExtra);
                    setList = classsetModel;

                    classSetVM = new ClassSetViewModel();
                    await GetClassToksAsync();
                }
            }

            CardRowNum = 0;
            cardsUsed = 0;
            TokLists = new List<TokModel>();
            cardHistoryList = new List<CardsHistoryModel>();
            bool allowAddTok = true, isNonPlayable = false;
            restokList = new List<TokModel>();

            if (!string.IsNullOrEmpty(IntentTokList))
            {
                restokList = JsonConvert.DeserializeObject<List<TokModel>>(IntentTokList);
            }
            else
            {
                if (isSet)
                {
                    if (classSetVM == null)
                    {
                        restokList = new List<TokModel>();
                        var classtok = JsonConvert.DeserializeObject<List<ClassTokModel>>(Intent.GetStringExtra("classTokModel"));

                        if (classtok.Count > 0)
                        {
                            restokList.AddRange(classtok);
                        }

                        if (restokList.Count == 0)
                        {
                            restokList = await TokMatch(setList.Id, FilterType.Set);
                        }
                    }
                    else
                    {
                        if (classSetVM.ClassToks != null)
                        {
                            foreach (var item in classSetVM.ClassToks)
                            {
                                restokList.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    restokList = await TokMatch("", (FilterType)Settings.FilterTag);//GetToks();
                }
            }

            hideBlueLoading(this);

            if (restokList.Count <= 2)
            {
                showAlertDialog(this, "No available toks to show.", (s, e) =>
                {
                    this.Finish();
                });
            }
            else
            {
                foreach (var item in restokList)
                {
                    allowAddTok = true;
                    //if (SupportActionBar.Subtitle.ToLower().Contains("category:") && SupportActionBar.Subtitle.ToLower().Contains("user:"))
                    //{
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega" || item.TokGroup.ToLower() == "pic" || item.TokGroup.ToLower() == "list")
                    {
                        allowAddTok = false;
                        isNonPlayable = true;
                    }
                    else
                    {
                        allowAddTok = true;
                    }
                    //}

                    if (allowAddTok)
                    {
                        TokLists.Add(item);
                    }
                }

                if (isNonPlayable)
                {
                    //AlertMessage
                    var objBuilder = new AlertDialog.Builder(this);
                    objBuilder.SetTitle("");
                    objBuilder.SetMessage("Only playable toks can be played.");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) => { });
                    objDialog.Show();
                }

                if (TokLists.Count > 2)
                {
                    cardHistoryList = Enumerable.Repeat(new CardsHistoryModel(), TokLists.Count).ToList();
                    tokListRound = GetTokMatchRound(TokLists, CurrentRound);
                    var cur = resCards.Rounds;//(resCards.Rounds - 1) == 0 ? 1 : (resCards.Rounds - 1);
                    TextRound.Text = "Round " + 1 + " of " + cur.ToString();
                    loadToks(isSwitchPrimarySecondary, tokListRound);
                    cardsUsed = CardRowNum;
                }
            }
        }
        private void loadToks(bool isSwitchPrimarySecondary, List<TokModel> tokModelsList)
        {
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
            this.LinearProgress.Visibility = ViewStates.Visible;
            presentCards = 0;

            tokModelList = tokModelsList;
            arrayNumAnswers = new List<int>();
            for (int i = 0; i < tokModelsList.Count(); i++)
            {
                arrayNumAnswers.Add(i); //Add the possible row needed
            };

            arrayNumAnswers = arrayNumAnswers.Shuffle().ToList();

            for (int i = 0; i < tokModelsList.Count(); i++)
            {
                var tokModel = tokModelsList[i];
                bool isDetail = false;
                string detailstr = "";
                if (tokModelsList[i].IsDetailBased == true)
                {
                    if (tokModelsList[i].Details != null)
                    {
                        isDetail = true;

                        for (int d = 0; d < tokModelsList[i].Details.Count(); d++)
                        {
                            if (!string.IsNullOrEmpty(tokModelsList[i].Details[d]))
                            {
                                if (isMultilinetext && d > 0)
                                {
                                    detailstr += "\n ";
                                }
                                else
                                {
                                    if (d > 0)
                                    {
                                        detailstr += " ";
                                    }
                                }

                                var bulletType = tokModel.BulletKind == null ? "bullet" : tokModel.BulletKind.ToLower();
                                switch (bulletType)
                                {
                                    case "none": detailstr += tokModel.Details[d]; break;
                                    case "bullet":
                                    case "bullets": detailstr += "\u2022 " + tokModel.Details[d]; break;
                                    case "number":
                                    case "numbers": detailstr += $"{d + 1}.)" + tokModel.Details[d]; break;
                                    case "letter":
                                    case "letters": detailstr += $"{((char)d + 1).ToString().ToUpper()}.)" + tokModel.Details[d]; break;
                                    default: detailstr += $"{d + 1}.)" + tokModel.Details[d]; break;
                                }
                            }
                        }
                    }
                }

                var tokSecondary = tokModel;
                string primary = tokModel.PrimaryFieldText;
                string secondary = tokModel.SecondaryFieldText;

                if (isDetail)
                {
                    secondary = detailstr;
                }

                if (isSwitchPrimarySecondary)
                {
                    tokSecondary.PrimaryFieldText = secondary;
                    tokSecondary.SecondaryFieldText = primary;
                    tokSecondary.SecondaryImage = tokModel.Image;
                }
                else
                {
                    tokSecondary.PrimaryFieldText = primary;
                    tokSecondary.SecondaryFieldText = secondary;
                    tokSecondary.SecondaryImage = tokModel.SecondaryImage;
                }

                if (arrayNumAnswers[i] == 0)
                {
                    frameTokMatchDroppable1.Visibility = ViewStates.Visible;
                    viewflipper_tokmatch1.Visibility = ViewStates.Visible;
                    btnTokMatchDroppable1.Text = tokSecondary.PrimaryFieldText;
                    btnTokMatchDropped1.Tag = CardRowNum;
                    presentCards++;
                } 
                else if (arrayNumAnswers[i] == 1)
                {
                    frameTokMatchDroppable2.Visibility = ViewStates.Visible;
                    viewflipper_tokmatch2.Visibility = ViewStates.Visible;
                    btnTokMatchDroppable2.Text = tokSecondary.PrimaryFieldText;

                    btnTokMatchDropped2.Tag = CardRowNum;
                    presentCards++;
                }
                else if (arrayNumAnswers[i] == 2)
                {
                    frameTokMatchDroppable3.Visibility = ViewStates.Visible;
                    viewflipper_tokmatch3.Visibility = ViewStates.Visible;
                    btnTokMatchDroppable3.Text = tokSecondary.PrimaryFieldText;

                    btnTokMatchDropped3.Tag = CardRowNum;
                    presentCards++;
                }
                else if (arrayNumAnswers[i] == 3)
                {
                    frameTokMatchDroppable4.Visibility = ViewStates.Visible;
                    viewflipper_tokmatch4.Visibility = ViewStates.Visible;
                    btnTokMatchDroppable4.Text = tokSecondary.PrimaryFieldText;

                    btnTokMatchDropped4.Tag = CardRowNum;
                    presentCards++;
                }

                if (isDetail)
                {
                    if (i == 0)
                    {
                        lblTokMatchSecondary1.Typeface = Typeface.Default;
                        lblTokMatchSecondary1.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped1.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 1)
                    {
                        lblTokMatchSecondary2.Typeface = Typeface.Default;
                        lblTokMatchSecondary2.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped2.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 2)
                    {
                        lblTokMatchSecondary3.Typeface = Typeface.Default;
                        lblTokMatchSecondary3.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped3.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 3)
                    {
                        lblTokMatchSecondary4.Typeface = Typeface.Default;
                        lblTokMatchSecondary4.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped4.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        btnSecondaryEyeIcon1.Visibility = ViewStates.Gone;
                        imageViewSecondary1.Visibility = ViewStates.Gone;
                        if (!string.IsNullOrEmpty(tokSecondary.SecondaryImage))
                        {
                            btnSecondaryEyeIcon1.Visibility = ViewStates.Visible;
                            lblTokMatchSecondary1.Visibility = ViewStates.Gone;
                            imageViewSecondary1.Visibility = ViewStates.Visible;
                            Glide.With(this).Load(tokSecondary.SecondaryImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imageViewSecondary1);
                        }
                        lblTokMatchSecondary1.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped1.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 1)
                    {
                        btnSecondaryEyeIcon2.Visibility = ViewStates.Gone;
                        imageViewSecondary2.Visibility = ViewStates.Gone;
                        if (!string.IsNullOrEmpty(tokSecondary.SecondaryImage))
                        {
                            btnSecondaryEyeIcon2.Visibility = ViewStates.Visible;
                            lblTokMatchSecondary2.Visibility = ViewStates.Gone;
                            imageViewSecondary2.Visibility = ViewStates.Visible;
                            Glide.With(this).Load(tokSecondary.SecondaryImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imageViewSecondary2);
                        }
                        lblTokMatchSecondary2.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped2.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 2)
                    {
                        btnSecondaryEyeIcon3.Visibility = ViewStates.Gone;
                        imageViewSecondary3.Visibility = ViewStates.Gone;
                        if (!string.IsNullOrEmpty(tokSecondary.SecondaryImage))
                        {
                            btnSecondaryEyeIcon3.Visibility = ViewStates.Visible;
                            lblTokMatchSecondary3.Visibility = ViewStates.Gone;
                            imageViewSecondary3.Visibility = ViewStates.Visible;
                            Glide.With(this).Load(tokSecondary.SecondaryImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imageViewSecondary3);
                        }
                        lblTokMatchSecondary3.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped3.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                    else if (i == 3)
                    {
                        btnSecondaryEyeIcon4.Visibility = ViewStates.Gone;
                        imageViewSecondary4.Visibility = ViewStates.Gone;
                        if (!string.IsNullOrEmpty(tokSecondary.SecondaryImage))
                        {
                            btnSecondaryEyeIcon4.Visibility = ViewStates.Visible;
                            lblTokMatchSecondary4.Visibility = ViewStates.Gone;
                            imageViewSecondary4.Visibility = ViewStates.Visible;
                            Glide.With(this).Load(tokSecondary.SecondaryImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imageViewSecondary4);
                        }
                        lblTokMatchSecondary4.Text = tokSecondary.SecondaryFieldText;

                        //Answer
                        btnTokMatchDropped4.ContentDescription = tokSecondary.PrimaryFieldText;
                    }
                }

                if (CardRowNum < TokLists.Count - 1)
                    CardRowNum += 1;
            }

            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            this.LinearProgress.Visibility = ViewStates.Gone;
        }

        TrueFalseMode trueFalseMode = TrueFalseMode.Off;
        bool isSwitchPrimarySecondary = false;
        bool isMultilinetext = false;
        bool isRetryOnlyIncorrectMode = false;
        public void switchRetryOnlyIncorrectMode(bool isRetry)
        {
            isRetryOnlyIncorrectMode = isRetry;
        }
        public void switchPrimarySecondary()
        {
            isSwitchPrimarySecondary = !isSwitchPrimarySecondary;
            loadToks(isSwitchPrimarySecondary, tokListRound);
        }

        public void isMultiLineTexts(bool isMultiline)
        {
            isMultilinetext = isMultiline;
            loadToks(isSwitchPrimarySecondary, tokListRound);
        }
        public void TrueFalseModeToks(TrueFalseMode optionTF)
        {
            trueFalseMode = optionTF;
            List<TokModel> tfTokList = new List<TokModel>();

            if (optionTF == TrueFalseMode.Off)
            {
                tfTokList = tokListRound;
            }
            else if (optionTF == TrueFalseMode.TFOnly)
            {
                foreach (var item in tokListRound)
                {
                    var primary = item.PrimaryFieldText == null ? "" : item.PrimaryFieldText;
                    var secondary = item.SecondaryFieldText == null ? "" : item.SecondaryFieldText;

                    if (primary.Trim().ToLower() == "t" ||
                        primary.Trim().ToLower() == "true" ||
                        secondary.Trim().ToLower() == "f" ||
                        secondary.Trim().ToLower() == "false")
                    {
                        tfTokList.Add(item);
                    }
                }
            }
            else if (optionTF == TrueFalseMode.ExcludeAllTF)
            {
                foreach (var item in tokListRound)
                {
                    var primary = item.PrimaryFieldText == null ? "" : item.PrimaryFieldText;
                    var secondary = item.SecondaryFieldText == null ? "" : item.SecondaryFieldText;

                    if (primary.Trim().ToLower() != "t" &&
                        primary.Trim().ToLower() != "true" &&
                        secondary.Trim().ToLower() != "f" &&
                        secondary.Trim().ToLower() != "false")
                    {
                        tfTokList.Add(item);
                    }
                }
            }


            if (tfTokList.Count > 2)
            {
                cardHistoryList = Enumerable.Repeat(new CardsHistoryModel(), tfTokList.Count).ToList();
                tokListRound = GetTokMatchRound(tfTokList, CurrentRound);
                var cur = resCards.Rounds;//(resCards.Rounds - 1) == 0 ? 1 : (resCards.Rounds - 1);
                TextRound.Text = "Round " + 1 + " of " + cur.ToString();
                loadToks(isSwitchPrimarySecondary, tfTokList);
                cardsUsed = CardRowNum;
            }
            else
            {
                showAlertDialog(this, "No available toks to show.", (s, e) =>
                {
                });
            }
        }
        private void OnBack_Click(object sender, EventArgs e)
        {
            if (CurrentRound !=0)
            {
                isPrevClick = true;
                ClearButtons();
                CurrentRound -= 1;
                ReturnToNormal();
                //Set PrevClick back to false after loadToks
                isPrevClick = false;

                CheckPrevious();
            }
        }
        private void CheckPrevious()
        {
            //Check the previous
            bool isVisible1 = true, isVisible2 = true, isVisible3 = true, isVisible4 = true;
            int rowNum = (int)btnTokMatchDropped1.Tag;
            int cardLimiter = 0;
            if (CurrentRound+1 != 1)
                cardLimiter = cardsUsed;

            for (int z = 0; z < tokListRound.Count; z++)
            {
                string droppedText = cardHistoryList[rowNum]?.DroppedText?.ToString() ?? "";
                int resourceid = cardHistoryList[rowNum].ResourceID;
                if (z == 0)
                {
                    btnTokMatchDropped1.Text = droppedText;
                    if (!string.IsNullOrEmpty(droppedText))
                    {
                        btnTokMatchDropped1.Visibility = ViewStates.Visible;
                    }

                    IVTokMatchCheckWrong1.SetImageResource(resourceid);
                    if (resourceid > 0)
                    {
                        viewflipper_tokmatch1.DisplayedChild = 1;
                    }
                }
                else if (z == 1)
                {
                    btnTokMatchDropped2.Text = droppedText;
                    if (!string.IsNullOrEmpty(droppedText))
                    {
                        btnTokMatchDropped2.Visibility = ViewStates.Visible;
                    }

                    IVTokMatchCheckWrong2.SetImageResource(resourceid);
                    if (resourceid > 0)
                    {
                        viewflipper_tokmatch2.DisplayedChild = 1;
                    }
                }
                else if (z == 2)
                {
                    btnTokMatchDropped3.Text = droppedText;
                    if (!string.IsNullOrEmpty(droppedText))
                    {
                        btnTokMatchDropped3.Visibility = ViewStates.Visible;
                    }

                    IVTokMatchCheckWrong3.SetImageResource(resourceid);
                    if (resourceid > 0)
                    {
                        viewflipper_tokmatch3.DisplayedChild = 1;
                    }
                }
                else if (z == 3)
                {
                    btnTokMatchDropped4.Text = droppedText;
                    if (!string.IsNullOrEmpty(droppedText))
                    {
                        btnTokMatchDropped4.Visibility = ViewStates.Visible;
                    }

                    IVTokMatchCheckWrong4.SetImageResource(resourceid);
                    if (resourceid > 0)
                    {
                        viewflipper_tokmatch4.DisplayedChild = 1;
                    }
                }

                //Set Visibility to false if text are the same 
                if (droppedText == btnTokMatchDroppable1.Text && isVisible1)
                {
                    btnTokMatchDroppable1.Visibility = ViewStates.Invisible;
                    btnEyeIcon1.Visibility = ViewStates.Invisible;
                    isVisible1 = false;
                }
                else if (droppedText == btnTokMatchDroppable2.Text && isVisible2)
                {
                    btnTokMatchDroppable2.Visibility = ViewStates.Invisible;
                    btnEyeIcon2.Visibility = ViewStates.Invisible;
                    isVisible2 = false;
                }
                else if (droppedText == btnTokMatchDroppable3.Text && isVisible3)
                {
                    btnTokMatchDroppable3.Visibility = ViewStates.Invisible;
                    btnEyeIcon3.Visibility = ViewStates.Invisible;
                    isVisible3 = false;
                }
                else if (droppedText == btnTokMatchDroppable4.Text && isVisible4)
                {
                    btnTokMatchDroppable4.Visibility = ViewStates.Invisible;
                    btnEyeIcon4.Visibility = ViewStates.Invisible;
                    isVisible4 = false;
                }
                
                if(rowNum<cardHistoryList.Count-2)
                    ++rowNum;
                //rowNum += 1;
            }
        }
        private void OnNext_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Text.ToLower() == "next")
            {
                if (CurrentRound < resCards.Rounds)//-1
                {
                    if (!isNormalMode)
                    {
                        btnTokMatchCheck.Tag = 0;
                        btnTokMatchCheck.Text = "Check";
                    }

                    CurrentRound += 1;
                    var cur = resCards.Rounds;//(resCards.Rounds - 1) == 0 ? 1 : (resCards.Rounds - 1);
                    TextRound.Text = "Round "+ (CurrentRound + 1).ToString() + " of " + cur.ToString();


                    ReturnToNormal();

                    CheckPrevious();
                }
            }
            else if ((sender as Button).Text.ToLower() == "restart")
            {
                ContinueRestart();
            }
        }
        private void OnReset_Click(object sender, EventArgs e)
        {
            btnTokMatchCheck.Visibility = ViewStates.Gone;
            ContinueReset();
        }
        private void ContinueReset()
        {
            //lblTokMatchScore.Text = "";
            //CurrentRound = 0;
            //CardRowNum = 0;
            btnTokMatchCheck.Tag = 0;
            btnTokMatchCheck.Text = "Check";

            //cardHistoryList = new List<CardsHistoryModel>();
            //cardHistoryList = Enumerable.Repeat(new CardsHistoryModel(), TokLists.Count).ToList();

            btnTokMatchReset.Visibility = ViewStates.Gone;
            ReturnToNormal();
        }
        private void ContinueRestart()
        {
            gif_TokMatchCoin.Visibility = ViewStates.Visible;
            btnTokMatchNext.Text = "Next";

            lblTokMatchScore.Text = "";
            totalScore = 0;
            CurrentRound = 0;
            CardRowNum = 0;
            btnTokMatchCheck.Tag = 0;
            btnTokMatchCheck.Text = "Check";

            cardHistoryList = new List<CardsHistoryModel>();
            cardHistoryList = Enumerable.Repeat(new CardsHistoryModel(), TokLists.Count).ToList();
            var cur = resCards.Rounds;// (resCards.Rounds - 1) == 0 ? 1 : (resCards.Rounds - 1);
            TextRound.Text = "Round " + 1+ " of " + cur.ToString();

            ReturnToNormal();
        }
        private void ReturnToNormal()
        {
            ViewFlipperNormalNext.Visibility = ViewStates.Invisible;
            DropZoneGone();

            //Minus the current page
            if (isPrevClick)
            {
                CardRowNum += 1;
                CardRowNum -= tokListRound.Count;
            }

            tokListRound = GetTokMatchRound(TokLists, CurrentRound);

            //Minus the Previous page
            if (isPrevClick)
            {
                CardRowNum -= tokListRound.Count;
            }

            loadToks(false, tokListRound);

            if (viewflipper_tokmatch1.DisplayedChild == 1)
            {
                OnFlipFrames(viewflipper_tokmatch1);
            }
            if (viewflipper_tokmatch2.DisplayedChild == 1)
            {
                OnFlipFrames(viewflipper_tokmatch2);
            }
            if (viewflipper_tokmatch3.DisplayedChild == 1)
            {
                OnFlipFrames(viewflipper_tokmatch3);
            }
            if (viewflipper_tokmatch4.DisplayedChild == 1)
            {
                OnFlipFrames(viewflipper_tokmatch4);
            }

            ClearButtons();
            CheckDroppableIsEmpty();
            isCheck = false;
        }
        private void ClearButtons()
        {
            //Clear Fields
            btnTokMatchDropped1.Text = "";
            btnTokMatchDropped2.Text = "";
            btnTokMatchDropped3.Text = "";
            btnTokMatchDropped4.Text = "";

            IVTokMatchCheckWrong1.SetImageResource(0);
            IVTokMatchCheckWrong2.SetImageResource(0);
            IVTokMatchCheckWrong3.SetImageResource(0);
            IVTokMatchCheckWrong4.SetImageResource(0);
        }
        private void OnFlipFrames(ViewFlipper v)
        {
            if (Showingback)
            { //Front
              // Use custom animations
                v.SetInAnimation(this, Resource.Animation.viewflipper_card_left_in);
                v.SetOutAnimation(this, Resource.Animation.viewflipper_card_left_out);
                v.ShowPrevious();
                Showingback = false;
            }
            else
            { //Back
              // Use custom animations
                v.SetInAnimation(this, Resource.Animation.viewflipper_card_right_out);
                v.SetOutAnimation(this, Resource.Animation.viewflipper_card_right_out);
                v.ShowNext();
                Showingback = true;
            }
        }
        private void OnCheck_Click(object sender, EventArgs e)
        {
            int dropped = 0, score = 0;
            int check = Convert.ToInt16(btnTokMatchCheck.Tag.ToString());//Check = 0, uncheck = 0

            score = totalScore;

            int stageScore = 0;
            if (isCheck==false)
            {
                for (int i = 0; i < tokModelList.Count(); i++)
                {
                    if (i == 0)
                    {
                        if (btnTokMatchDropped1.Visibility == ViewStates.Visible)
                        {
                            dropped += 1;

                            if (btnTokMatchDropped1.Text == btnTokMatchDropped1.ContentDescription) //If equal then correct
                            {
                                score += 1;

                                stageScore += 1;

                                IVTokMatchCheckWrong1.SetImageResource(Resource.Drawable.check_green);

                                cardHistoryList[(int)btnTokMatchDropped1.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.check_green };
                            }
                            else
                            {
                                IVTokMatchCheckWrong1.SetImageResource(Resource.Drawable.x_red);

                                cardHistoryList[(int)btnTokMatchDropped1.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.x_red };
                            }

                            if (viewflipper_tokmatch1.DisplayedChild == 0)
                            {
                                OnFlipFrames(viewflipper_tokmatch1);
                            }
                            else if (viewflipper_tokmatch1.DisplayedChild == 1 && check == 1)
                            {
                                OnFlipFrames(viewflipper_tokmatch1);
                            }
                        }
                    }
                    else if (i == 1)
                    {
                        if (btnTokMatchDropped2.Visibility == ViewStates.Visible)
                        {
                            dropped += 1;

                            if (btnTokMatchDropped2.Text == btnTokMatchDropped2.ContentDescription)
                            {
                                score += 1;

                                stageScore += 1;

                                IVTokMatchCheckWrong2.SetImageResource(Resource.Drawable.check_green);

                                cardHistoryList[(int)btnTokMatchDropped2.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.check_green };
                            }
                            else
                            {
                                IVTokMatchCheckWrong2.SetImageResource(Resource.Drawable.x_red);

                                cardHistoryList[(int)btnTokMatchDropped2.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.x_red };
                            }

                            if (viewflipper_tokmatch2.DisplayedChild == 0)
                            {
                                OnFlipFrames(viewflipper_tokmatch2);
                            }
                            else if (viewflipper_tokmatch2.DisplayedChild == 1 && check == 1)
                            {
                                OnFlipFrames(viewflipper_tokmatch2);
                            }
                        }
                    }
                    else if (i == 2)
                    {
                        if (btnTokMatchDropped3.Visibility == ViewStates.Visible)
                        {
                            dropped += 1;

                            if (btnTokMatchDropped3.Text == btnTokMatchDropped3.ContentDescription)
                            {
                                score += 1;

                                stageScore += 1;

                                IVTokMatchCheckWrong3.SetImageResource(Resource.Drawable.check_green);

                                cardHistoryList[(int)btnTokMatchDropped3.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.check_green };
                            }
                            else
                            {
                                IVTokMatchCheckWrong3.SetImageResource(Resource.Drawable.x_red);

                                cardHistoryList[(int)btnTokMatchDropped3.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.x_red };
                            }

                            if (viewflipper_tokmatch3.DisplayedChild == 0)
                            {
                                OnFlipFrames(viewflipper_tokmatch3);
                            }
                            else if (viewflipper_tokmatch3.DisplayedChild == 1 && check == 1)
                            {
                                OnFlipFrames(viewflipper_tokmatch3);
                            }
                        }
                    }
                    else if (i == 3)
                    {
                        if (btnTokMatchDropped4.Visibility == ViewStates.Visible)
                        {
                            dropped += 1;

                            if (btnTokMatchDropped4.Text == btnTokMatchDropped4.ContentDescription)
                            {
                                score += 1;

                                stageScore += 1;

                                IVTokMatchCheckWrong4.SetImageResource(Resource.Drawable.check_green);

                                cardHistoryList[(int)btnTokMatchDropped4.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.check_green };
                            }
                            else
                            {
                                IVTokMatchCheckWrong4.SetImageResource(Resource.Drawable.x_red);

                                cardHistoryList[(int)btnTokMatchDropped4.Tag] = new CardsHistoryModel() { ResourceID = Resource.Drawable.x_red };
                            }

                            if (viewflipper_tokmatch4.DisplayedChild == 0)
                            {
                                OnFlipFrames(viewflipper_tokmatch4);
                            }
                            else if (viewflipper_tokmatch4.DisplayedChild == 1 && check == 1)
                            {
                                OnFlipFrames(viewflipper_tokmatch4);
                            }
                        }
                    }
                }
            }
            
            if (!isNormalMode)
            {
                totalScore = 0;
                lblTokMatchScore.Text = "";
                if (check == 0)
                {
                    btnTokMatchCheck.Text = "Uncheck";
                    btnTokMatchCheck.Tag = 1;
                    isCheck = false;
                }
                else
                {
                    btnTokMatchCheck.Text = "Check";
                    btnTokMatchCheck.Tag = 0;
                    isCheck = true;
                }
            }
            else
            {

                if (dropped == tokListRound.Count())
                {
                    if (isCheck == false)
                    {
                        if (CurrentRound < resCards.Rounds - 1)
                        {
                            totalScore = score;
                            lblTokMatchScore.Text = score.ToString() + " points";
                            if (score == 0)
                            {
                                lblTokMatchResult.Text = "Failed!";
                            }
                            else
                            {
                                lblTokMatchResult.Text = "Good!";
                            }

                            ViewFlipperNormalNext.Visibility = ViewStates.Visible;
                            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_scaleup);
                            ViewFlipperNormalNext.StartAnimation(myAnim);
                        }
                    }
                    //Set Check=true When in Normal Mode
                    isCheck = true;
                }

                if (stageScore < 2 && isRetryOnlyIncorrectMode)
                {
                    btnTokMatchReset.Visibility = ViewStates.Visible;
                }

                //If Current Row  == Last Round
                //Show Game Over Message
                if (CurrentRound+1 == resCards.Rounds) //-2
                {
                    lblTokMatchResult.Text = "Game Over!";
                    gif_TokMatchCoin.Visibility = ViewStates.Gone;
                    btnTokMatchNext.Text = "Restart";
                    ViewFlipperNormalNext.Visibility = ViewStates.Visible;
                    Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_scaleup);
                    ViewFlipperNormalNext.StartAnimation(myAnim);
                }
            }
        }
        void DropZone_Drag(object sender, View.DragEventArgs e)
        {
            bool isSetVisible = true;
            FrameLayout frameLayout = (FrameLayout)sender;
            string controlName = "";
            string fullResourceName = "";
            fullResourceName = Application.Context.Resources.GetResourceName(frameLayout.Id);

            controlName = fullResourceName.Split('/')[1];

            // React on different dragging events
            var evt = e.Event;
            switch (evt.Action)
            {
                case DragAction.Ended:
                case DragAction.Started:
                    e.Handled = true;
                    break;
                // Dragged element enters the drop zone
                case DragAction.Entered:
                    break;
                // Dragged element exits the drop zone
                case DragAction.Exited:
                    break;
                // Dragged element has been dropped at the drop zone
                case DragAction.Drop:
                    // You can check if element may be dropped here
                    // If not do not set e.Handled to true
                    e.Handled = true;
                    // Try to get clip data
                    var data = e.Event.ClipData;

                    if (data != null)
                        //result.Text = data.GetItemAt(0).Text + " has been dropped.";
                        switch (controlName)
                        {
                            case "frame_tokmatchdropzone1":
                                if (btnTokMatchDropped1.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDropped1.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDropped1.Visibility = ViewStates.Visible;

                                    cardHistoryList[(int)btnTokMatchDropped1.Tag] = new CardsHistoryModel() { DroppedText = btnTokMatchDropped1.Text};
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frame_tokmatchdropzone2":
                                if (btnTokMatchDropped2.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDropped2.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDropped2.Visibility = ViewStates.Visible;

                                    cardHistoryList[(int)btnTokMatchDropped2.Tag] = new CardsHistoryModel() { DroppedText = btnTokMatchDropped2.Text };
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frame_tokmatchdropzone3":
                                if (btnTokMatchDropped3.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDropped3.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDropped3.Visibility = ViewStates.Visible;

                                    cardHistoryList[(int)btnTokMatchDropped3.Tag] = new CardsHistoryModel() { DroppedText = btnTokMatchDropped3.Text };
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frame_tokmatchdropzone4":
                                if (btnTokMatchDropped4.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDropped4.Text = data.GetItemAt(0).Text;

                                    cardHistoryList[(int)btnTokMatchDropped4.Tag] = new CardsHistoryModel() { DroppedText = btnTokMatchDropped4.Text };

                                    btnTokMatchDropped4.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frameTokMatchDroppable1":
                                if (btnTokMatchDroppable1.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDroppable1.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDroppable1.Visibility = ViewStates.Visible;
                                    btnEyeIcon1.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frameTokMatchDroppable2":
                                if (btnTokMatchDroppable2.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDroppable2.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDroppable2.Visibility = ViewStates.Visible;
                                    btnEyeIcon2.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frameTokMatchDroppable3":
                                if (btnTokMatchDroppable3.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDroppable3.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDroppable3.Visibility = ViewStates.Visible;
                                    btnEyeIcon3.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                            case "frameTokMatchDroppable4":
                                if (btnTokMatchDroppable4.Visibility == ViewStates.Invisible)
                                {
                                    btnTokMatchDroppable4.Text = data.GetItemAt(0).Text;
                                    btnTokMatchDroppable4.Visibility = ViewStates.Visible;
                                    btnEyeIcon4.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    isSetVisible = false;
                                }
                                break;
                        }
                    //Set Visibility to false
                    if (isSetVisible)
                    {
                        if ((int)btnToDrop.Id == btnTokMatchDroppable1.Id)
                        {
                            btnTokMatchDroppable1.Visibility = ViewStates.Invisible;
                            btnEyeIcon1.Visibility = ViewStates.Invisible;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDroppable2.Id)
                        {
                            btnTokMatchDroppable2.Visibility = ViewStates.Invisible;
                            btnEyeIcon2.Visibility = ViewStates.Invisible;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDroppable3.Id)
                        {
                            btnTokMatchDroppable3.Visibility = ViewStates.Invisible;
                            btnEyeIcon3.Visibility = ViewStates.Invisible;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDroppable4.Id)
                        {
                            btnTokMatchDroppable4.Visibility = ViewStates.Invisible;
                            btnEyeIcon4.Visibility = ViewStates.Invisible;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDropped1.Id)
                        {
                            btnTokMatchDropped1.Visibility = ViewStates.Invisible;
                            btnTokMatchDropped1.Text = "";

                            cardHistoryList[(int)btnTokMatchDropped1.Tag].DroppedText = null;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDropped2.Id)
                        {
                            btnTokMatchDropped2.Visibility = ViewStates.Invisible;
                            btnTokMatchDropped2.Text = "";

                            cardHistoryList[(int)btnTokMatchDropped2.Tag].DroppedText = null;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDropped3.Id)
                        {
                            btnTokMatchDropped3.Visibility = ViewStates.Invisible;
                            btnTokMatchDropped3.Text = "";

                            cardHistoryList[(int)btnTokMatchDropped3.Tag].DroppedText = null;
                        }
                        else if ((int)btnToDrop.Id == btnTokMatchDropped4.Id)
                        {
                            btnTokMatchDropped4.Visibility = ViewStates.Invisible;
                            btnTokMatchDropped4.Text = "";

                            cardHistoryList[(int)btnTokMatchDropped4.Tag].DroppedText = null;
                        }
                    }

                    CheckDroppableIsEmpty();
                    break;
            }
        }

        private void CheckDroppableIsEmpty() {
            bool check1 = btnTokMatchDroppable1.Visibility == ViewStates.Invisible;
            bool check2 = btnTokMatchDroppable2.Visibility == ViewStates.Invisible;
            bool check3 = btnTokMatchDroppable3.Visibility == ViewStates.Invisible;
            bool check4 = btnTokMatchDroppable4.Visibility == ViewStates.Invisible;
            switch (presentCards) {
                case 2:
                    if (check1 && check2)
                        btnTokMatchCheck.Visibility = ViewStates.Visible;
                    else
                        btnTokMatchCheck.Visibility = ViewStates.Gone; 
                    break;
                case 3:
                    if (check1 && check2 && check3)
                        btnTokMatchCheck.Visibility = ViewStates.Visible;
                    else
                        btnTokMatchCheck.Visibility = ViewStates.Gone;
                    break;
                case 4:
                    if (check1 && check2 && check3 && check4)
                        btnTokMatchCheck.Visibility = ViewStates.Visible;
                    else
                        btnTokMatchCheck.Visibility = ViewStates.Gone;
                    break;
            }
          
        }
        public void showImageViewer(View v)
        {
            Bitmap imgBitmap = ((BitmapDrawable)(v as ImageView).Drawable).Bitmap;
            MemoryStream byteArrayOutputStream = new MemoryStream();
            imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
            byte[] byteArray = byteArrayOutputStream.ToArray();

            Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
            Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
            this.StartActivity(nextActivity);
        }

        [Java.Interop.Export("OnClickFrameDropZone")]
        public void OnClickFrameDropZone(View v)
        {
            int tag = Convert.ToInt16(v.Tag.ToString());
            LayoutInflater inflater = LayoutInflater.From(this);
            View view = inflater.Inflate(Resource.Layout.alertmessage_dialog, null);
            var txtAlertMessage = view.FindViewById<TextView>(Resource.Id.txtAlertMessage);
            var ImgTok = view.FindViewById<ImageView>(Resource.Id.ImgAlertMessage);
            ImgTok.Click += delegate
            {
                showImageViewer(ImgTok);
            };

            TextView txtOriginal = null;
            ImageView imageOriginal = null;
            if (tag == 1)
            {
                txtOriginal = v.FindViewById<TextView>(Resource.Id.lblTokMatchSecondary1);
                imageOriginal = imageViewSecondary1;
            }
            else if (tag == 2)
            {
                txtOriginal = v.FindViewById<TextView>(Resource.Id.lblTokMatchSecondary2);
                imageOriginal = imageViewSecondary2;
            }
            else if (tag == 3)
            {
                txtOriginal = v.FindViewById<TextView>(Resource.Id.lblTokMatchSecondary3);
                imageOriginal = imageViewSecondary3;
            }
            else if (tag == 4)
            {
                txtOriginal = v.FindViewById<TextView>(Resource.Id.lblTokMatchSecondary4);
                imageOriginal = imageViewSecondary4;
            }
            txtAlertMessage.Text = txtOriginal.Text;

            //If tok has secondary image
            if (imageOriginal.Visibility == ViewStates.Visible)
            {
                ImgTok.Visibility = ViewStates.Visible;
                GListenerImage = new GlideImgListener();
                GListenerImage.ParentActivity = this;

                try
                {
                    Bitmap imgBitmap = ((BitmapDrawable)(imageOriginal as ImageView).Drawable).Bitmap;
                    ImgTok.SetImageBitmap(imgBitmap);
                }
                catch (Exception ex) { }
            }
            else if (!string.IsNullOrEmpty(tokModelList[tag - 1].Image))
            {
                ImgTok.Visibility = ViewStates.Visible;
                GListenerImage = new GlideImgListener();
                GListenerImage.ParentActivity = this;

                Glide.With(this).Load(tokModelList[tag - 1].Image).Listener(GListenerImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgTok);
            }
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("");
            alertDialog.SetView(view);
            alertDialog.SetNegativeButton("OK", (senderAlert, args) => {
                alertDialog.Dispose();
            });
            alert = alertDialog.Create();
            alert.Show();
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            //float mX = 0, mY = 0;

            if (e.Action == MotionEventActions.Down)
            {
                //if (Math.Abs(e.GetX() - mX) < MAX_X_MOVE || Math.Abs(e.GetY() - mY) < MAX_Y_MOVE)
                //{
                //}
                //else
                //{

                bool allowdrag = false;
                if (v.Id == Resource.Id.btnTokMatchDroppable1 || v.Id == Resource.Id.btnTokMatchDropped1)
                {
                    allowdrag = true;
                }
                else if (v.Id == Resource.Id.btnTokMatchDroppable2 || v.Id == Resource.Id.btnTokMatchDropped2)
                {
                    allowdrag = true;
                }
                else if (v.Id == Resource.Id.btnTokMatchDroppable3 || v.Id == Resource.Id.btnTokMatchDropped3)
                {
                    allowdrag = true;
                }
                else if (v.Id == Resource.Id.btnTokMatchDroppable4 || v.Id == Resource.Id.btnTokMatchDropped4)
                {
                    allowdrag = true;
                }

                if (allowdrag)
                {
                    Button btnSender = (Button)v;
                    btnToDrop = btnSender;
                    var data = ClipData.NewPlainText("name", btnSender.Text);
                    View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(v);
                    v.StartDragAndDrop(data, shadowBuilder, v, 0);
                }

                //mX = e.GetX();
                //mY = e.GetY();
                //}
            }
            else if (e.Action == MotionEventActions.Up)
            {
                string message = "";
                bool showalert = false;
                switch (v.Id)
                {
                    case Resource.Id.btnEyeIcon1:
                        message = btnTokMatchDroppable1.Text;
                        showalert = true;
                        break;
                    case Resource.Id.btnEyeIcon2:
                        message = btnTokMatchDroppable2.Text;
                        showalert = true;
                        break;
                    case Resource.Id.btnEyeIcon3:
                        message = btnTokMatchDroppable3.Text;
                        showalert = true;
                        break;
                    case Resource.Id.btnEyeIcon4:
                        message = btnTokMatchDroppable4.Text;
                        showalert = true;
                        break;
                    case Resource.Id.btnSecondaryEyeIcon1:
                        OnClickFrameDropZone(frame_tokmatchdropzone1);
                        break;
                    case Resource.Id.btnSecondaryEyeIcon2:
                        OnClickFrameDropZone(frame_tokmatchdropzone2);
                        break;
                    case Resource.Id.btnSecondaryEyeIcon3:
                        OnClickFrameDropZone(frame_tokmatchdropzone3);
                        break;
                    case Resource.Id.btnSecondaryEyeIcon4:
                        OnClickFrameDropZone(frame_tokmatchdropzone4);
                        break;
                }

                if (showalert)
                {
                    AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
                    alertDialog.SetTitle("");
                    alertDialog.SetMessage(message);
                    alertDialog.SetNegativeButton("OK", (senderAlert, args) => {
                        alertDialog.Dispose();
                    });
                    AlertDialog alert = alertDialog.Create();
                    alert.Show();
                }
            }
            return true;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.option_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            AlertDialog.Builder alertDiag;
            Dialog diag;

            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.option_normal:
                    alertDiag = new AlertDialog.Builder(this);
                    alertDiag.SetTitle("Confirm");
                    alertDiag.SetMessage("Proceeding will reset this page.");
                    alertDiag.SetPositiveButton("Proceed", (senderAlert, args) => {
                        NormalMode();
                        ContinueReset();
                    });
                    alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                        alertDiag.Dispose();
                    });
                    diag = alertDiag.Create();
                    diag.Show();

                    break;
                case Resource.Id.option_education:
                    alertDiag = new AlertDialog.Builder(this);
                    alertDiag.SetTitle("Confirm");
                    alertDiag.SetMessage("Proceeding will reset this page.");
                    alertDiag.SetPositiveButton("Proceed", (senderAlert, args) => {
                        EducationMode();
                        ContinueReset();
                    });
                    alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                        alertDiag.Dispose();
                    });
                    diag = alertDiag.Create();
                    diag.Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void NormalMode()
        {
            isNormalMode = true;
            btnTokMatchRightArrow.Visibility = ViewStates.Gone;
            btnTokMatchLeftArrow.Visibility = ViewStates.Gone;
        }
       
        private void EducationMode()
        {
            isNormalMode = false;
            btnTokMatchRightArrow.Visibility = ViewStates.Visible;
            btnTokMatchLeftArrow.Visibility = ViewStates.Visible;
        }
        private void DropZoneGone()
        {
            viewflipper_tokmatch1.Visibility = ViewStates.Gone;
            viewflipper_tokmatch2.Visibility = ViewStates.Gone;
            viewflipper_tokmatch3.Visibility = ViewStates.Gone;
            viewflipper_tokmatch4.Visibility = ViewStates.Gone;

            frameTokMatchDroppable1.Visibility = ViewStates.Gone;
            frameTokMatchDroppable2.Visibility = ViewStates.Gone;
            frameTokMatchDroppable3.Visibility = ViewStates.Gone;
            frameTokMatchDroppable4.Visibility = ViewStates.Gone;

            btnTokMatchDroppable1.Visibility = ViewStates.Visible;
            btnTokMatchDroppable2.Visibility = ViewStates.Visible;
            btnTokMatchDroppable3.Visibility = ViewStates.Visible;
            btnTokMatchDroppable4.Visibility = ViewStates.Visible;

            btnEyeIcon1.Visibility = ViewStates.Visible;
            btnEyeIcon2.Visibility = ViewStates.Visible;
            btnEyeIcon3.Visibility = ViewStates.Visible;
            btnEyeIcon4.Visibility = ViewStates.Visible;

            btnTokMatchDropped1.Visibility = ViewStates.Invisible;
            btnTokMatchDropped2.Visibility = ViewStates.Invisible;
            btnTokMatchDropped3.Visibility = ViewStates.Invisible;
            btnTokMatchDropped4.Visibility = ViewStates.Invisible;
        }
        private RoundModel GetCardRounds(int totalCards)
        {
            RoundModel roundModel = new RoundModel();
            var rnd = 0;
            bool isValid = false;
            // If total cards is below 5
            if (totalCards < 5)
            {
                roundModel.Rounds = 1;
                roundModel.DivisibleBy = totalCards;
                return roundModel;
            }

            for (int i = 4; i >= 2; i--)
            {
                int rem = (totalCards % i);
                int initial = int.Parse(Math.Truncate((decimal)totalCards / i).ToString());
                int ext = (((decimal)totalCards / i) % 1) > 0 ? 1 : 0;
                rnd = initial + ext;
                switch (rem)
                {
                    case 0:
                    case 2:
                    case 3:
                        roundModel.Rounds = rnd;
                        roundModel.DivisibleBy = i;
                        return roundModel;
                    case 1:
                        continue; // if invalid, continue until the end
                }
            }
            if (!isValid) // All divisibility is invalid
            {
                int initial = int.Parse(Math.Truncate((decimal)totalCards / 4).ToString());
                int ext = (((decimal)totalCards / 4) % 1) > 0 ? 1 : 0;
                rnd = initial + ext;
            }
            roundModel.Rounds = rnd;
            roundModel.DivisibleBy = 4;
            return roundModel;
        }
        public List<TokModel> GetTokMatchRound(List<TokModel> toks, int round)
        {
            var invalidCards = new List<int> { 13, 25, 37, 49, 61, 73, 85, 97 };
            var tempToks = toks ?? new List<TokModel>();
            var totalCards = tempToks.Count;
            resCards = GetCardRounds(totalCards);
            int takeCnt = 0, rounds = resCards.Rounds - 1, divBy = resCards.DivisibleBy;
            CurrentRound = round; // Set the current round
            if (round >= rounds && round <= rounds) // Before last round and last round
            {
                // If invalid cards
                switch (totalCards)
                {
                    case 13: // Invalid Numbers
                    case 25:
                    case 37:
                    case 49:
                    case 61:
                    case 73:
                    case 85:
                    case 97:
                        if (round == rounds) takeCnt = (totalCards % divBy) + 1; // Last round
                        else takeCnt = divBy - 1; // Before the last round
                        break;
                    default:
                        if (round == rounds && totalCards > 5) takeCnt = (totalCards % divBy); // Last round
                        else takeCnt = divBy; // Before the last round
                        break;
                }
            }
            else
                takeCnt = divBy;


            if (round == rounds)
            {
                //Get the last remaining value
                var lastRange = tempToks.Count - (round * 4);
                return tempToks.GetRange(round * 4, lastRange).ToList(); //multiply by 4 as default display of each round
            }
            else
            {
                return tempToks.GetRange(round * 4, 4).ToList(); //multiply by 4 as default display of each round
            }
            //return tempToks.Skip((((round * divBy) - divBy) + takeCnt) - takeCnt).Take(takeCnt).ToList();
        }
        public async Task<List<Shared.Models.TokModel>> TokMatch(string id = "", FilterType type = FilterType.None, int cnt = 0)
        {
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
            this.LinearProgress.Visibility = ViewStates.Visible;

            List<TokModel> tokResult = new List<TokModel>();
            var list = new List<TokModel>();
            var totalCards = 0;
            string strtoken = string.Empty;
            Settings.ContinuationToken = "";
            if (!string.IsNullOrEmpty(id))
            {
                switch (type)
                {
                    case FilterType.Category:
                        for (int i = 0; i <= cnt; i++)
                        {
                            if (string.IsNullOrEmpty(Settings.ContinuationToken))
                                tokResult = await TokService.Instance.GetToksAsync(new TokQueryValues() { category = id, streamtoken = strtoken , sortby = Settings.SortByFilter});
                            else
                                tokResult = await TokService.Instance.GetToksAsync(new TokQueryValues() { category = id, loadmore = "yes", token = Settings.ContinuationToken, order = "newest", sortby = Settings.SortByFilter });
                            list.AddRange(tokResult);
                        }
                        totalCards = list.Count;
                        break;
                    case FilterType.TokType:
                        for (int i = 0; i <= cnt; i++)
                        {
                            if (string.IsNullOrEmpty(Settings.ContinuationToken))
                                tokResult = await TokService.Instance.GetToksAsync(new TokQueryValues() { toktype = id, streamtoken = strtoken, sortby = Settings.SortByFilter });
                            else
                                tokResult = await TokService.Instance.GetToksAsync(new TokQueryValues() { toktype = id, loadmore = "yes", token = Settings.ContinuationToken, order = "newest", sortby = Settings.SortByFilter });
                            list.AddRange(tokResult);
                        }
                        totalCards = list.Count;
                        break;
                    case FilterType.Set:
                        try {
                            //    var item = await SetService.Instance.GetSetAsync(id);
                            var tokQueryModel = new ClassTokQueryValues();
                            var groupset = JsonConvert.DeserializeObject<List<ClassTokModel>>(Intent.GetStringExtra("classTokModel"));
       
                            foreach (var tok in groupset)
                            {
                              
                                //var tokRes = await ClassService.Instance.GetClassTokAsync(tok.Id,tok.PartitionKey);
                                //var con = setList.TokIds;

                                //if (tokRes != null)
                                    list.Add(tok);
                            }
                            tokResult = list;
                            totalCards = list.Count;
                        }
                        catch(Exception ex) {
                            
                        }
                     
                        break;
                    case FilterType.None:
                    default:
                        var res = await TokService.Instance.GetAllFeaturedToksAsync();
                        tokResult = res;
                        totalCards = res.Count;
                        break;
                }
            }
            else
            {
                var res = await TokService.Instance.GetAllFeaturedToksAsync();
                tokResult = res;
                totalCards = res.Count;
            }
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            this.LinearProgress.Visibility = ViewStates.Gone;

            resCards = GetCardRounds(totalCards);
            int rounds = resCards.Rounds, divBy = (int)resCards.DivisibleBy;
            TotalCards = totalCards;
            MaxRound = rounds;
            DataCounter = cnt;
            return tokResult;
        }
        private void OnQuit_Click(object sender, EventArgs e)
        {
            Finish();
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
        public Button btnEyeIcon1 => this.FindViewById<Button>(Resource.Id.btnEyeIcon1);
        public Button btnEyeIcon2 => this.FindViewById<Button>(Resource.Id.btnEyeIcon2);
        public Button btnEyeIcon3 => this.FindViewById<Button>(Resource.Id.btnEyeIcon3);
        public Button btnEyeIcon4 => this.FindViewById<Button>(Resource.Id.btnEyeIcon4);
        public View ViewDummyForTouch => FindViewById<View>(Resource.Id.ViewDummyForTouch);
        public TextView TextRound => FindViewById<TextView>(Resource.Id.TextRound); 
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
        public ImageView imageViewSecondary1 => FindViewById<ImageView>(Resource.Id.imageViewSecondary1);
        public ImageView imageViewSecondary2 => FindViewById<ImageView>(Resource.Id.imageViewSecondary2);
        public ImageView imageViewSecondary3 => FindViewById<ImageView>(Resource.Id.imageViewSecondary3);
        public ImageView imageViewSecondary4 => FindViewById<ImageView>(Resource.Id.imageViewSecondary4);
        public Button btnSecondaryEyeIcon1 => FindViewById<Button>(Resource.Id.btnSecondaryEyeIcon1);
        public Button btnSecondaryEyeIcon2 => FindViewById<Button>(Resource.Id.btnSecondaryEyeIcon2);
        public Button btnSecondaryEyeIcon3 => FindViewById<Button>(Resource.Id.btnSecondaryEyeIcon3);
        public Button btnSecondaryEyeIcon4 => FindViewById<Button>(Resource.Id.btnSecondaryEyeIcon4);
        public Button btnOptions => FindViewById<Button>(Resource.Id.btnOptions);
    }
}