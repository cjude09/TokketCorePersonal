using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Shared.Helpers;
using System.Threading.Tasks;
using Tokket.Shared.Services;
using Android.Text;
using Tokket.Core;
using Android.Webkit;
using AlertDialog = Android.App.AlertDialog;
using Android.Views.Animations;
using Tokket.Shared.ViewModels;
using Tokket.Shared.Extensions;
using Tokket.Android.ViewModels;
using Android.Text.Style;
using Tokket.Android.Helpers;
using Supercharge;
using Android.Views.InputMethods;
using DE.Hdodenhof.CircleImageViewLib;
using System.IO;
using Android.Graphics.Drawables;
using Tokket.Android.Fragments;
using SharedAccount = Tokket.Shared.Services;
using Android.Util;
using Java.Util.Regex;
using Pattern = Java.Util.Regex.Pattern;
using Android.Text.Method;
using Tokket.Android;
using Color = Android.Graphics.Color;
using System.ComponentModel;
using Android.Content.PM;
using Tokket.Core.Tools;
using Tokket.Android.Custom;
using Base64 = Android.Util.Base64;
using Result = Android.App.Result;
using NetUri = Android.Net.Uri;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AndroidX.AppCompat.Content.Res;
using Xamarin.Essentials;
using NetworkAccess = Xamarin.Essentials.NetworkAccess;
using AndroidX.Core.View;
using static Android.App.ActionBar;
using AndroidX.AppCompat.Widget;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;
using System.Threading;

namespace Tokket.Android
{
    [Activity(Label = "Tok Info", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokInfoActivity : BaseActivity, View.IOnTouchListener
    {
        // Allows us to know if we should use MotionEvent.ACTION_MOVE
        private bool isViewTokInfo = false;
        // The Position where our touch event started
        private float startY;
        float imgScale = 0;
        ReactionModel reactionUser;
        internal static TokInfoActivity Instance { get; private set; }
        GestureDetector gesturedetector;
        TokModel tokModel; ClassTokModel classTokModel; ClassGroupModel classGroupModel = null;
        Intent nextActivity; bool isInaccurateAdded = false, changesMade = false;
        List<ReactionModel> CommentList; List<TokMojiDrawableViewModel> TokMojiDrawables;
        Typeface font; GridLayoutManager mLayoutManager; string hashtagCode = "(^#[a-z0-9_]+|#[a-zA-Z0-9_]+$)";
        GlideImgListener GListener; SpannableString hashtagText;
        ReactionValueViewModel reactionValueVM; ReactionValueModel reactionValue; ResultData<TokkepediaReaction> resultDataUserReaction;
        TextView PrimaryFieldText, EnglishPrimaryFieldText; //Separated this one and excluded this from the #regio UI properties because this will return a null valuee when added in the linear.AddView
        LinearLayout LayoutClassGroup;
        public TokInfoViewModel TokInfoVm => App.Locator.TokInfoPageVM;

        const int REQUEST_TOK_INFO_REPLY = 1000, REQUEST_GROUPSELECTION_REPLY = 1001;
        string requestAction = string.Empty;
        ClassSetModel ClassSet;
        List<ClassTokModel> ClassTokList;
        ReportDialouge Report = null;
        public bool isUpdated = false;

        const string packageName = "com.tokket.classtoks";
        List<Task> tasksMain = new List<Task>();
        List<TokSection> listTokContentQna = new List<TokSection>();
        string fromCallerComments = "", fromCallerReactionValue = "", fromCallerUserReaction = "";
        private ObservableRecyclerAdapter<ReactionModel, CachingViewHolder> adapterComments;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            //FacebookClientManager.Initialize(this);
            SetContentView(Resource.Layout.tok_info_page);
            SetSupportActionBar(toolBar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            setActivityTitle("Tok Info");

            Initialize();

#if (_CLASSTOKS)

            classTokModel = JsonConvert.DeserializeObject<ClassTokModel>(Intent.GetStringExtra("classtokModel"));
            PrimaryFieldText.Text = classTokModel.PrimaryFieldText;
            //  SourceLinkView.TextFormatted = Html.FromHtml("<a href="+classTokModel.SourceLink+"/>", FromHtmlOptions.ModeLegacy);
            if (!string.IsNullOrEmpty(classTokModel.SourceLink))
            {
                SourceLinkView.Text = "Source Link: " + classTokModel.SourceLink;
            }
            else
            {
                SourceLinkView.Visibility = ViewStates.Gone;
            }

            try
            {
                classGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("classGroupModel"));
            }
            catch (Exception ex)
            {

            }

            tokModel = classTokModel;
            fromCallerUserReaction = tokModel.Id + "tokmode_userreaction_tokinfo";
            fromCallerReactionValue = tokModel.Id + "tokmodel_reactionValue_tokinfo";
            fromCallerComments = tokModel.Id + "tokmodel_tokinfo";
            classTokModel.IsMasterCopy = !classTokModel.HasReactions && !classTokModel.HasComments;

            PrepareViews();
            if (!string.IsNullOrEmpty(classTokModel.GroupId) || classTokModel.IsMasterCopy)
            {
                LinearTokInfoReaction.Visibility = ViewStates.Gone;
                NestedComment.Visibility = ViewStates.Gone;
            }
            LabelTokType.SetText(Resource.String.underClassName);
            LabelTokGroup.SetText(Resource.String.underType);

            SupportActionBar.Subtitle = classTokModel.GroupName;
#endif

#if (_TOKKEPEDIA)
            tokModel = JsonConvert.DeserializeObject<TokModel>(Intent.GetStringExtra("tokModel"));
#endif
            isViewTokInfo = Intent.GetBooleanExtra("view_tok_info", false);
            //When isViewTokInfo is true, hide the views
            HideViewTokInfoViews();

            font = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            ReactionCheck.Typeface = font;
            ReactionWrong.Typeface = font;
            BtnTokInfoEyeIcon.Typeface = font;
            BtnMegaAccurate.Typeface = font;
            BtnMegaInaccurate.Typeface = font;

            BtnTokInfoEyeIcon.SetOnTouchListener(this);

            txtUserDisplayName.Text = tokModel.UserDisplayName;
            TokDateTimeCreated.Text = tokModel.RelativeTime;

            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += WORK_DATA_LOAD;
            work.RunWorkerAsync();
        }

        private async void ShowHideOwnerAnswerAsync(object sender, EventArgs e)
        {
            classTokModel.AnswerEnabled = !classTokModel.AnswerEnabled;

            showBlueLoading(this);
            var result = await ClassService.Instance.UpdateClassToksAsync(classTokModel);
            hideBlueLoading(this);

            if (result.ResultEnum == Shared.Helpers.Result.Success)
            {
                if (classTokModel.AnswerEnabled)
                {
                    btnShowHideOwnerAnswer.Text = "Hide Owner Answers";
                }
                else
                {
                    btnShowHideOwnerAnswer.Text = "Show Owner Answers";
                }
            }
            else
            {
                ShowLottieMessageDialog(this, "Failed. " + result.ResultMessage, false);
            }
        }

        private void WORK_DATA_LOAD(object sender, DoWorkEventArgs e)
        {
            RunOnUiThread(() => {
                LoadContentData();
            });
        }

        private void LoadContentData()
        {
            tasksMain.Add(CheckToken());

            FrameReactionBtn.Touch -= ReactionTableTouched;
            FrameReactionBtn.Touch += ReactionTableTouched;

            GreenGemContainer.SetOnTouchListener(this);
            YellowGemContainer.SetOnTouchListener(this);
            RedGemContainer.SetOnTouchListener(this);

            BtnInaccurateComment.Click += async (sender, e) =>
            {
                await OnClickAddReaction(BtnInaccurateComment);
            };

            btnTokInfo_SendComment.Click += async (sender, e) =>
            {
                await OnClickAddReaction(btnTokInfo_SendComment);
            };

            BtnMegaAccurate.Click += async (sender, e) =>
            {
                int color = Color.YellowGreen;
                Drawable background = BtnMegaInaccurate.Background;

                //color = ((ColorDrawable)background).getColor();

                if (background is RippleDrawable) //Proceed if no ColorDrawable has been set
                {
                    await OnClickAddReaction(BtnMegaAccurate);
                }
            };

            HeartReaction.Click += async (sender, e) => {
                if (HundredReaction.Alpha == 1 && ImgPurpleGem.Alpha == 1)
                {
                    //Proceed to add reaction
                    await OnClickAddReaction(HeartReaction);
                }
            };

            HundredReaction.Click += async (sender, e) =>
            {
                if (HeartReaction.Drawable.GetConstantState() == ContextCompat.GetDrawable(this, Resource.Drawable.heart_outline_v2).GetConstantState() && ImgPurpleGem.Alpha == 1)
                {
                    //Proceed to add reaction
                    await OnClickAddReaction(HundredReaction);
                }
            };

            swipeRefreshComment.Refresh += RefreshLayout_Refresh;

            CommentEditor.FocusChange += delegate
            {
                if (CommentEditor.IsFocused)
                {
                    BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                    RecyclerTokMojis.Visibility = ViewStates.Gone;
                }
            };

            CommentEditor.Click += delegate
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMojis.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                CommentEditor.RequestFocus();

                inputManager.ShowSoftInput(CommentEditor, 0);
            };
            //  CommentEditor.RequestFocus();
            EditInaccurateComment.FocusChange += delegate
            {
                if (EditInaccurateComment.IsFocused)
                {
                    BtnSmileInaccurate.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                    RecyclerTokMojisInaccurate.Visibility = ViewStates.Gone;
                }
                else
                {
                    constraintMegaInaccurateComment.Visibility = ViewStates.Gone;
                    NestedComment.Visibility = ViewStates.Visible;
                }
            };

            EditInaccurateComment.Click += delegate
            {
                BtnSmileInaccurate.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMojisInaccurate.Visibility = ViewStates.Gone;

                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);

                EditInaccurateComment.RequestFocus();

                inputManager.ShowSoftInput(EditInaccurateComment, 0);
            };

            NestedComment.LayoutChange += delegate
            {
                Rect r = new Rect();
                NestedComment.GetWindowVisibleDisplayFrame(r);
                int screenHeight = NestedComment.RootView.Height;
                int heightDifference = screenHeight - (r.Bottom - r.Top);

                //If difference is greater than 500, show GemsParent to hide keyboard or smileys when tapped outside
                if (heightDifference > 500 || RecyclerTokMojis.Visibility == ViewStates.Visible) //189
                {
                    GemsParentContainer.Visibility = ViewStates.Visible;
                }
            };

            txtUserDisplayName.Click += (obj, eve) => {
                string commentorid = classTokModel.UserId;
                nextActivity = new Intent(this, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", commentorid);
                this.StartActivity(nextActivity);
            };

            tasksMain.Add(FillUpFields());

            //If purplegem is clicked
            //ImgPurpleGem.Click -= ShowGemsCollClicked;
            //ImgPurpleGem.Click += ShowGemsCollClicked;
            ImgPurpleGem.Click += async (sender, e) => {
                if (HeartReaction.Drawable.GetConstantState() == ContextCompat.GetDrawable(this, Resource.Drawable.heart_outline_v2).GetConstantState() && HundredReaction.Alpha == 1)
                {
                    //Proceed to add new reaction
                    await OnClickAddReaction(ImgPurpleGem);
                }
            };

            tokcategory.Tag = (int)Toks.Category;
            tokcategory.Click -= OnTokButtonClick;
            tokcategory.Click += OnTokButtonClick;

            tokgroup.Tag = (int)Toks.TokGroup;
            tokgroup.Click -= OnTokButtonClick;
            tokgroup.Click += OnTokButtonClick;

            toktype.Tag = (int)Toks.TokType;
            toktype.Click -= OnTokButtonClick;
            toktype.Click += OnTokButtonClick;

            imgUserPhoto.Click += delegate
            {
                nextActivity = new Intent(this, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", tokModel.UserId);
                this.StartActivity(nextActivity);
            };

            BtnSmile.Click += btnSmileOnClick;

            BtnSmileInaccurate.Click += (object sender, EventArgs e) =>
            {
                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                if (RecyclerTokMojisInaccurate.Visibility == ViewStates.Gone)
                {
                    BtnSmileInaccurate.SetImageResource(Resource.Drawable.TOKKET_smiley_2b);
                    inputManager.HideSoftInputFromWindow(BtnSmileInaccurate.WindowToken, HideSoftInputFlags.None);
                    RecyclerTokMojisInaccurate.Visibility = ViewStates.Visible;
                }
                else
                {
                    BtnSmileInaccurate.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                    RecyclerTokMojisInaccurate.Visibility = ViewStates.Gone;
                    View CommentView = EditInaccurateComment;
                    CommentView.RequestFocus();
                    inputManager.ShowSoftInput(CommentView, 0);
                }
            };

            if (tokModel.TokGroup.ToLower() == "mega") //If Mega
            {
                BtnMegaAccurate.Visibility = ViewStates.Visible;
                BtnMegaInaccurate.Visibility = ViewStates.Visible;
                TokBackButton.Visibility = ViewStates.Gone;
                if (tokModel.Sections == null)
                {
                    tasksMain.Add(loadSections());
                }

                if (tokModel.Sections != null)
                {
                    MegaTokSections();
                }
            }
            else if (tokModel.TokGroup.ToLower() == "q&a")
            {
                BtnMegaAccurate.Visibility = ViewStates.Visible;
                BtnMegaInaccurate.Visibility = ViewStates.Visible;
                TokBackButton.Visibility = ViewStates.Gone;
                if (tokModel.Sections == null)
                {
                    tasksMain.Add(loadQnASections());
                }
            }
            else
            {
                tasksMain.Add(LoadListViews());
            }

            //Load RecyclerView
            TokInfoVm.CommentsCollection.Clear();

            if (classTokModel.IsMasterCopy && (tokModel.TokGroup.ToLower() == "q&a" || tokModel.IsMegaTok == true || tokModel.TokGroup.ToLower() == "mega"))
            {
                ProgressViews.Visibility = ViewStates.Invisible;
                //Don't load comments or reactions
            } 
            else
            {
                RecyclerComments.ContentDescription = TokInfoVm.LoadCommentsCache(fromCallerComments); //Get cached comments
                if (TokInfoVm.CommentsCollection.Count > 0)
                {
                    SetCommentsAdapter();
                }
                else
                {
                    TokInfoVm.CommentsCollection.Clear();
                    tasksMain.Add(LoadComments());
                }

                //Load Cache data first
                tasksMain.Add(LoadSelectedGems(Id: tokModel.Id, isCache: true));

                //Load data in the background
                RunOnUiThread(async () => await LoadSelectedGems(Id: tokModel.Id, isCache: false));
            }

            LoadPhoto();

            //LoadMore
            if (RecyclerComments != null)
            {
                RecyclerComments.HasFixedSize = true;
                RecyclerComments.NestedScrollingEnabled = false;

                NestedScroll.ScrollChange += async (object sender, NestedScrollView.ScrollChangeEventArgs e) =>
                {
                    View view = (View)NestedScroll.GetChildAt(NestedScroll.ChildCount - 1);

                    int diff = (view.Bottom - (NestedScroll.Height + NestedScroll.ScrollY));

                    if (diff == 0)
                    {
                        if (!string.IsNullOrEmpty(RecyclerComments.ContentDescription))
                        {
                            await LoadComments(RecyclerComments.ContentDescription);
                        }
                    }

                    if (e.ScrollY > e.OldScrollX)
                    {

                    }
                };
            }

            //Load the comments first before the tokmojis
            tasksMain.Add(RunTokMojis());

            tokinfo_imgMain.Click += delegate
            {
                gotoImageViewer(tokinfo_imgMain);
            };
            tokinfo_imgSecondary.Click += delegate
            {
                gotoImageViewer(tokinfo_imgSecondary);
            };

            btnTotalReactions.Click += delegate
            {
                var modelConvert = JsonConvert.SerializeObject(reactionValueVM);
                nextActivity = new Intent(this, typeof(ReactionValuesActivity));
                nextActivity.PutExtra("reactionValueVM", modelConvert);
                nextActivity.PutExtra("tokId", tokModel.Id);
                StartActivity(nextActivity);
            };

            try
            {
                ClassSet = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("ClassSet"));
                ClassTokList = JsonConvert.DeserializeObject<List<ClassTokModel>>(Intent.GetStringExtra("ClassSetToks"));
            }
            catch (Exception ex) { }

            if (!string.IsNullOrEmpty(classTokModel.TokSharePk))
            {
                InitShareData(classTokModel);

            }
            InitShareButtons();

            var downloadTasks = tasksMain;
            var t = Task.Run(() =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    while (downloadTasks.Any())
                    {
                        var finishedTask = await Task.WhenAny(downloadTasks);
                        downloadTasks.Remove(finishedTask);
                    }
                    //await Task.WhenAll(tasksMain);
                });
            });
        }
        private void Initialize()
        {
            tasksMain = new List<Task>();
            reactionUser = new ReactionModel();
            Instance = this;
            Settings.ContinuationToken = null;

            RecyclerComments.ContentDescription = "";

            TokInfoVm.CircleProgress = CircleProgress;

            TokMojiDrawables = new List<TokMojiDrawableViewModel>();
            gesturedetector = new GestureDetector(this, new MyGestureListener(this));

            Report = new ReportDialouge(this);
            EnglishPrimaryFieldText = FindViewById<TextView>(Resource.Id.lbl_tokTopicConvert1);
            PrimaryFieldText = FindViewById<TextView>(Resource.Id.lbl_tokTopic1);

            RecyclerTokMojis.SetLayoutManager(new GridLayoutManager(this, 2));
            RecyclerTokMojisDummy.SetLayoutManager(new GridLayoutManager(this, 2));
            RecyclerTokMojisInaccurate.SetLayoutManager(new GridLayoutManager(this, 2));

            mLayoutManager = new GridLayoutManager(this, 1);
            RecyclerComments.SetLayoutManager(mLayoutManager);

            CommentList = new List<ReactionModel>();
        }
        private async Task LoadListViews()
        {
            if (tokModel.TokGroup.ToLower() == "list")
            {
                TokBackButton.Visibility = ViewStates.Gone;
            }
            else
            {
                TokBackButton.Visibility = ViewStates.Visible;
            }

            isEnglishLinear.RemoveAllViews();

            isEnglishLinear.AddView(EnglishPrimaryFieldText);

            if (tokModel.IsDetailBased)
            {
                AddTokDetails();

                if (!tokModel.IsEnglish)
                {
                    for (int i = 0; i < tokModel.EnglishDetails.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(tokModel.EnglishDetails[i]))
                        {
                            var bulletType = tokModel.BulletKind == null ? "bullet" : tokModel.BulletKind.ToLower();
                            isEnglishLinear.Visibility = ViewStates.Visible;
                            View viewEnglish = LayoutInflater.Inflate(Resource.Layout.tokinfo_isenglish, null);
                            TextView txtEnglish = viewEnglish.FindViewById<TextView>(Resource.Id.lbl_tokConvertAnswer1);
                            switch (bulletType)
                            {
                                case "none": txtEnglish.Text = tokModel.EnglishDetails[i]; break;
                                case "bullet":
                                case "bullets": txtEnglish.Text = "\u2022 " + tokModel.EnglishDetails[i]; break;
                                case "number":
                                case "numbers": txtEnglish.Text = $"{i + 1}.)" + tokModel.EnglishDetails[i]; break;
                                case "letter":
                                case "letters": txtEnglish.Text = $"{((char)i + 1).ToString().ToUpper()}.)" + tokModel.EnglishDetails[i]; break;
                                default: txtEnglish.Text = $"{i + 1}.)" + tokModel.EnglishDetails[i]; break;
                            }


                            isEnglishLinear.AddView(viewEnglish);
                        }
                    }
                }
                else
                {
                    isEnglishLinear.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                View view = LayoutInflater.Inflate(Resource.Layout.tokinfo_detail1, null);
                var btnAccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                var btnInaccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                var btnTotalReaction = view.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                var circleprogressReaction = view.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                btnAccurate.Typeface = font;
                btnInaccurate.Typeface = font;
                btnAccurate.Visibility = ViewStates.Gone;
                btnInaccurate.Visibility = ViewStates.Gone;
                btnTotalReaction.Visibility = ViewStates.Gone;
                circleprogressReaction.Visibility = ViewStates.Gone;

                TextView txtDetail = view.FindViewById<TextView>(Resource.Id.lbl_detail);

                if (!string.IsNullOrEmpty(classTokModel.TokLink))
                {
                    txtDetail.SetTextColor(Color.DeepSkyBlue);
                    txtDetail.PaintFlags = PaintFlags.UnderlineText;

                    txtDetail.Click += async delegate
                    {
                        await gotoLink_Clicked(classTokModel.TokLink);
                    };
                }

                hashtagText = new SpannableString(tokModel.SecondaryFieldText + "");
                Matcher matcher = Pattern.Compile(hashtagCode).Matcher(hashtagText);
                while (matcher.Find())
                {
                    hashtagText.SetSpan(new ClickableText(Actions.OpenHashTag, matcher.Group()), matcher.Start(), matcher.End(), 0);
                }

                txtDetail.Append(hashtagText);
                txtDetail.MovementMethod = LinkMovementMethod.Instance;

                //txtDetail.SetText(hashtagText, TextView.BufferType.Spannable);

                linearParent.AddView(view);

                if (!tokModel.IsEnglish)
                {
                    View viewEnglish2 = LayoutInflater.Inflate(Resource.Layout.tokinfo_isenglish, null);
                    TextView txtSecEnglish = viewEnglish2.FindViewById<TextView>(Resource.Id.lbl_tokConvertAnswer1);
                    txtSecEnglish.Text = tokModel.EnglishSecondaryFieldText;
                    isEnglishLinear.AddView(viewEnglish2);
                }
            }
        }

        private void PrepareViews()
        {
            TextTotalComments.Visibility = ViewStates.Visible;
            TextTotalComments.Text = " ";
            ProgressComments.Visibility = ViewStates.Gone;
            ShimmerCommentsList.Visibility = ViewStates.Gone;

            if (classTokModel.TokGroup.ToLower() == "list")
            {
                HeartReaction.Visibility = ViewStates.Gone;
                HundredReaction.Visibility = ViewStates.Gone;
                ImgPurpleGem.Visibility = ViewStates.Gone;
                BtnMegaAccurate.Visibility = ViewStates.Gone;
                BtnMegaInaccurate.Visibility = ViewStates.Gone;
                lblInaccurate.Visibility = ViewStates.Gone;
                btnTotalReactions.Visibility = ViewStates.Gone;
            }

            if (classTokModel.UserId == Settings.GetTokketUser().Id && classTokModel.TokGroup.ToLower() == "q&a")
            {
                if (classTokModel.AnswerEnabled)
                {
                    btnShowHideOwnerAnswer.Text = "Hide Owner Answers";
                }
                else
                {
                    btnShowHideOwnerAnswer.Text = "Show Owner Answers";
                }
                btnShowHideOwnerAnswer.Visibility = ViewStates.Visible;

                btnShowHideOwnerAnswer.Click += ShowHideOwnerAnswerAsync;
            }
            else
            {
                btnShowHideOwnerAnswer.Visibility = ViewStates.Invisible;
            }
        }

        private void HideViewTokInfoViews()
        {
            if (isViewTokInfo)
            {
                BtnShareFB.Visibility = ViewStates.Gone;
                BtnShareTwitter.Visibility = ViewStates.Gone;
                NestedComment.Visibility = ViewStates.Gone;
                ImgPurpleGem.Visibility = ViewStates.Gone;
                BtnMegaAccurate.Visibility = ViewStates.Gone;
                BtnMegaInaccurate.Visibility = ViewStates.Gone;
                btnTotalReactions.Visibility = ViewStates.Gone;
                LinearTokInfoReaction.Visibility = ViewStates.Gone;
            }
        }
        private void InitShareData(ClassTokModel classTokModel) {
            try {

                ShareView.Visibility = ViewStates.Visible;
                ShareCaptionLayout.Visibility = ViewStates.Visible;
                var getoriginalData = JsonConvert.DeserializeObject<ClassTokModel>(classTokModel.SharedTok);

                ShareUserName.Text = getoriginalData.UserDisplayName;
                BtnOriginalTok.Click += delegate {
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
                    var modelConvert = JsonConvert.SerializeObject(getoriginalData);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    StartActivity(nextActivity);
                };
                PrimaryFieldText.Text = classTokModel.PrimaryFieldText;
                SharedCaptionView.Text = getoriginalData.PrimaryFieldText;
                SharedCaptionView.Visibility = ViewStates.Visible;
                Glide.With(this).Load(getoriginalData.UserPhoto).Listener(GListener).Into(ShareImageProfile);
                SharedCaptionView.SetTypeface(null, TypefaceStyle.Italic);
                SharedCaptionView.SetTextColor(Color.Blue);
                ProgressViewsShare.Visibility = ViewStates.Gone;
            } catch (Exception ex) { }

        }

        private void gotoImageViewer(View v)
        {
            Bitmap imgBitmap = ((BitmapDrawable)(v as ImageView).Drawable).Bitmap;
            MemoryStream byteArrayOutputStream = new MemoryStream();
            imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
            byte[] byteArray = byteArrayOutputStream.ToArray();

            nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
            Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
            this.StartActivity(nextActivity);
        }

        private void btnSmileOnClick(object sender, EventArgs e)
        {
            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            if (RecyclerTokMojis.Visibility != ViewStates.Visible)
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_2b);
                inputManager.HideSoftInputFromWindow(BtnSmile.WindowToken, HideSoftInputFlags.None);
                RecyclerTokMojis.Visibility = ViewStates.Visible;
            }
            else
            {
                BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
                RecyclerTokMojis.Visibility = ViewStates.Gone;
                View CommentView = CommentEditor;
                CommentView.RequestFocus();
                inputManager.ShowSoftInput(CommentView, 0);
            }
        }
        private void LoadPhoto()
        {
            var drawableLoggedInUserPhoto = ((BitmapDrawable)ProfileFragment.Instance.ProfileUserPhoto.Drawable).Bitmap;

            imgcomment_userphoto.SetImageBitmap(drawableLoggedInUserPhoto);

            if (tokModel.UserId == Settings.GetUserModel().UserId)
            {
                imgUserPhoto.SetImageBitmap(drawableLoggedInUserPhoto);
            }
            else
            {
                Glide.With(this).Load(tokModel.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(imgUserPhoto);
            }

            if (string.IsNullOrEmpty(tokModel.StickerImage))
            {
                StickerImage.Visibility = ViewStates.Gone;
            }
            else
            {
                Glide.With(this).Load(tokModel.StickerImage).Into(StickerImage);
            }
        }
        private async Task FillUpFields()
        {
            //PrimaryFieldText.Text = tokModel.PrimaryFieldText;

            EnglishPrimaryFieldText.Text = tokModel.EnglishPrimaryFieldText ?? "";
            tokcategory.Text = tokModel.Category;
            tokgroup.Text = tokModel.TokGroup;
            toktype.Text = tokModel.TokType;
            if (classGroupModel == null)
            {
                LayoutClassGroup = FindViewById<LinearLayout>(Resource.Id.Tok_group_linear);

                LayoutClassGroup.Visibility = ViewStates.Gone;
            }
            else
            {
                tokclassgroup.Text = classGroupModel.Name;
            }

            if (!string.IsNullOrEmpty(tokModel.Image))
            {
                GListener = new GlideImgListener();
                GListener.ParentActivity = this;


                if (URLUtil.IsValidUrl(tokModel.Image))
                {
                    Glide.With(this).Load(tokModel.ThumbnailImage).Listener(GListener).Into(tokinfo_imgMain);
                }
                else
                {
                    byte[] imageDetailBytes = Convert.FromBase64String(tokModel.Image.Replace("data:image/jpeg;base64,", ""));
                    tokinfo_imgMain.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }
            }

            if (!string.IsNullOrEmpty(tokModel.SecondaryImage)) {
                tokinfo_imgSecondary.Visibility = ViewStates.Visible;
                var gListener = new GlideImgListener();
                gListener.ParentActivity = this;

                if (URLUtil.IsValidUrl(tokModel.SecondaryImage))
                {
                    Glide.With(this).Load(tokModel.SecondaryImage).Listener(gListener).Into(tokinfo_imgSecondary);
                }
                else
                {
                    byte[] imageDetailBytes = Convert.FromBase64String(tokModel.SecondaryImage.Replace("data:image/jpeg;base64,", ""));
                    tokinfo_imgSecondary.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }
            }
        }
        
        private async Task gotoLink_Clicked(string classTokId)
        {
            ClassTokModel tokResult;

            if (!string.IsNullOrEmpty(classTokId))
            {
                tokResult = await ClassService.Instance.GetClassTokAsync(classTokId, "classtoks0");
               // var test =  await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.ServicesDB.ClassTokServiceDB>().GetClassTok(classTokId, "classtoks0");
                if (tokResult != null)
                {
                    Intent nextActivity = new Intent(this, typeof(TokInfoActivity));
                    var modelConvert = JsonConvert.SerializeObject(tokResult);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    this.StartActivity(nextActivity);
                }
            }
        }
        private async Task RunTokMojis()
        {
            var tokmojiResult = TokMojiService.Instance.GetCacheTokmojisAsync();
            if (tokmojiResult.Results != null)
            {
                foreach (var item in tokmojiResult.Results.ToList())
                {
                    TokInfoVm.TokMojiCollection.Add(item);
                }
            }

            if (TokInfoVm.TokMojiCollection.Count == 0)
            {
                await TokInfoVm.LoadTokMoji();
            }

            //Show tokmoji in text
            SpannableHelper.ListTokMoji = TokInfoVm.TokMojiCollection.ToList();
            SpannableStringBuilder ssbName = new SpannableStringBuilder(tokModel.PrimaryFieldText);

            var resultSpan = SpannableHelper.AddStickersSpannable(this, ssbName);

            hashtagText = new SpannableString(tokModel.PrimaryFieldText);
            Matcher matcher = Pattern.Compile(hashtagCode).Matcher(hashtagText);
            while (matcher.Find())
            {
                resultSpan.SetSpan(new ClickableText(Actions.OpenHashTag, matcher.Group()), matcher.Start(), matcher.End(), 0);
            }

            PrimaryFieldText.Text = ""; //clear before adding text
            PrimaryFieldText.Append(resultSpan); //SetText(resultSpan, TextView.BufferType.Spannable);
            PrimaryFieldText.MovementMethod = LinkMovementMethod.Instance;

            var adapterTokMoji = TokInfoVm.TokMojiCollection.GetRecyclerAdapter(BindTokMojiViewHolder, Resource.Layout.tokinfo_tokmoji_row);
            RecyclerTokMojis.SetAdapter(adapterTokMoji);
            
            RecyclerTokMojisInaccurate.SetAdapter(adapterTokMoji);
            
            var adapterTokMojiDummy = TokInfoVm.TokMojiCollection.GetRecyclerAdapter(BindTokMojiViewHolderDummy, Resource.Layout.tokinfo_tokmoji_row);
            RecyclerTokMojisDummy.SetAdapter(adapterTokMoji);

            RecyclerTokMojisDummy.LayoutChange += (sender, e) =>
            {
                //Created a dummy recyclerTokMoji so that it will show the image.
                for (int i = 0; i < RecyclerTokMojisDummy.ChildCount; i++)
                {
                    View view = RecyclerTokMojisDummy.GetChildAt(i);
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
                                //imgTokMojiBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                                //Adjust size of tokmoji drawable
                                Bitmap scaledBitmap = SpannableHelper.scaleDown(imgTokMojiBitmap, 1000, true);
                                scaledBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
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

        private async Task LoadComments(string continuationtoken = "", bool isRefresh = false)
        {
            List<Task> tasksList = new List<Task>();

            if (continuationtoken == "")
            {
                TokInfoVm.commentsloaded = 0;
                TokInfoVm.CommentsCollection.Clear();
                ShimmerCommentsList.StartShimmerAnimation();
                ShimmerCommentsList.Visibility = ViewStates.Visible;
                ProgressComments.Visibility = ViewStates.Visible;
            }

            tasksList.Add(TokInfoVm.LoadComments(tokModel.Id, continuationtoken, fromCallerComments));

            if (continuationtoken == "")
            {
                TextTotalComments.Visibility = ViewStates.Visible;
                ProgressComments.Visibility = ViewStates.Gone;
                ShimmerCommentsList.Visibility = ViewStates.Gone;

                if (ShimmerCommentsList.Visibility == ViewStates.Gone) //Page is fully loaded
                {
                    if (!isRefresh)
                    {
                        //Writing/recording a view:
                        ReactionModel tokkepediaReaction = new ReactionModel();
                        tokkepediaReaction.ItemId = tokModel.Id;

                        if (Settings.GetUserModel().UserId == tokModel.UserId)
                        {
                            tokkepediaReaction.Kind = "tiletap_views_personal";
                        }
                        else
                        {
                            tokkepediaReaction.Kind = "tiletap_views";
                        }

                        tokkepediaReaction.Label = "reaction";
                        tokkepediaReaction.DetailNum = 0;
                        //tokkepediaReaction.CategoryId = tokModel.CategoryId;
                        //tokkepediaReaction.TokTypeId = tokModel.TokTypeId;
                        tokkepediaReaction.OwnerId = tokModel.UserId;
                        tokkepediaReaction.IsChild = false;

                        //API
                        //var result = await ReactionService.Instance.AddReaction(tokkepediaReaction);
                        tasksList.Add(ReactionService.Instance.AddReaction(tokkepediaReaction));
                        //tasksList.Add(LoadSelectedGems(Id: tokModel.Id));

                        //await LoadSelectedGems(Id: tokModel.Id);
                    }
                }
            }

            await Task.WhenAll(tasksList);

            swipeRefreshComment.Refreshing = false;

            SetCommentsAdapter();
        }

        private void SetCommentsAdapter()
        {
            if (TokInfoVm.CommentsCollection.Count == 0)
            {
                TextTotalComments.Text = " ";
            }
            else
            {
                TextTotalComments.Text = "Comments: " + TokInfoVm.CommentsCollection.Count.ToString();
            }

            RecyclerComments.ContentDescription = Settings.ContinuationToken;

            adapterComments = TokInfoVm.CommentsCollection.GetRecyclerAdapter(BindCommentsViewHolder, Resource.Layout.tokinfo_comments_row);
            RecyclerComments.SetAdapter(adapterComments);

            RecyclerComments.LayoutChange += (sender, e) =>
            {
                for (int i = TokInfoVm.commentsloaded; i < RecyclerComments.ChildCount; i++)
                {
                    View viewParent = RecyclerComments.GetChildAt(i);
                    var EditCommentText = viewParent.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
                    var BtnViewMoreClose = viewParent.FindViewById<Button>(Resource.Id.btnViewMoreCloseComment);
                    var txtEllipseComment = viewParent.FindViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
                    var CommentText = viewParent.FindViewById<TextView>(Resource.Id.lblCommentRowContent);
                    var ReplyUserDisplayText = viewParent.FindViewById<TextView>(Resource.Id.TokInfoCommentsReplyText);
                    var spannableStringComment = new SpannableString(CommentText.Text);
                    var spannableStringReply = new SpannableString(ReplyUserDisplayText.Text);

                    Layout layout = txtEllipseComment.Layout;
                    int commentEllipLine = txtEllipseComment.LineCount;

                    if (commentEllipLine > 1)
                    {
                        int ellipsisCount = layout.GetEllipsisCount(commentEllipLine - 1);
                        if (ellipsisCount > 0)
                        {
                            BtnViewMoreClose.Visibility = ViewStates.Visible;
                        }
                    }

                    //LoadTokMoji
                    for (int z = 0; z < TokMojiDrawables.Count; z++)
                    {
                        var loopTokMojiID = ":" + TokMojiDrawables[z].TokmojiId + ":";
                        var indicesComment = spannableStringComment.ToString().IndexesOf(loopTokMojiID);
                        var indicesReply = spannableStringReply.ToString().IndexesOf(loopTokMojiID);

                        foreach (var index in indicesComment)
                        {
                            var set = true;
                            foreach (ImageSpan span in spannableStringComment.GetSpans(index, index + loopTokMojiID.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                            {
                                if (spannableStringComment.GetSpanStart(span) >= index && spannableStringComment.GetSpanEnd(span) <= index + loopTokMojiID.Length)
                                    spannableStringComment.RemoveSpan(span);
                                else
                                {
                                    set = false;
                                    break;
                                }
                            }
                            if (set)
                            {
                                byte[] base64TokMoji = Convert.FromBase64String(TokMojiDrawables[i].TokmojImgBase64);
                                Bitmap decodedByte = BitmapFactory.DecodeByteArray(base64TokMoji, 0, base64TokMoji.Length);
                                spannableStringComment.SetSpan(new ImageSpan(decodedByte), index, index + loopTokMojiID.Length, SpanTypes.ExclusiveExclusive);
                            }
                        }

                        foreach (var index in indicesReply)
                        {
                            var set = true;
                            foreach (ImageSpan span in spannableStringReply.GetSpans(index, index + loopTokMojiID.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                            {
                                if (spannableStringReply.GetSpanStart(span) >= index && spannableStringReply.GetSpanEnd(span) <= index + loopTokMojiID.Length)
                                    spannableStringReply.RemoveSpan(span);
                                else
                                {
                                    set = false;
                                    break;
                                }
                            }
                            if (set)
                            {
                                byte[] base64TokMoji = Convert.FromBase64String(TokMojiDrawables[i].TokmojImgBase64);
                                Bitmap decodedByte = BitmapFactory.DecodeByteArray(base64TokMoji, 0, base64TokMoji.Length);
                                spannableStringReply.SetSpan(new ImageSpan(decodedByte), index, index + loopTokMojiID.Length, SpanTypes.ExclusiveExclusive);

                            }
                        }
                    }

                    txtEllipseComment.SetText(spannableStringComment, TextView.BufferType.Spannable);
                    CommentText.SetText(spannableStringComment, TextView.BufferType.Spannable);
                    EditCommentText.SetText(spannableStringComment, TextView.BufferType.Spannable);

                    EditCommentText.SetSelection(EditCommentText.Text.Length);

                    ReplyUserDisplayText.SetText(spannableStringReply, TextView.BufferType.Spannable);
                }
            };
            #region Have to add this code so tokmojis load on start
            for (int i = TokInfoVm.commentsloaded; i < RecyclerComments.ChildCount; i++)
            {
                View viewParent = RecyclerComments.GetChildAt(i);
                var EditCommentText = viewParent.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
                var BtnViewMoreClose = viewParent.FindViewById<Button>(Resource.Id.btnViewMoreCloseComment);
                var txtEllipseComment = viewParent.FindViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
                var CommentText = viewParent.FindViewById<TextView>(Resource.Id.lblCommentRowContent);
                var ReplyUserDisplayText = viewParent.FindViewById<TextView>(Resource.Id.TokInfoCommentsReplyText);
                var spannableStringComment = new SpannableString(CommentText.Text);
                var spannableStringReply = new SpannableString(ReplyUserDisplayText.Text);

                Layout layout = txtEllipseComment.Layout;
                int commentEllipLine = txtEllipseComment.LineCount;

                if (commentEllipLine > 1)
                {
                    int ellipsisCount = layout.GetEllipsisCount(commentEllipLine - 1);
                    if (ellipsisCount > 0)
                    {
                        BtnViewMoreClose.Visibility = ViewStates.Visible;
                    }
                }

                //LoadTokMoji
                for (int z = 0; z < TokMojiDrawables.Count; z++)
                {
                    var loopTokMojiID = ":" + TokMojiDrawables[z].TokmojiId + ":";
                    var indicesComment = spannableStringComment.ToString().IndexesOf(loopTokMojiID);
                    var indicesReply = spannableStringReply.ToString().IndexesOf(loopTokMojiID);

                    foreach (var index in indicesComment)
                    {
                        var set = true;
                        foreach (ImageSpan span in spannableStringComment.GetSpans(index, index + loopTokMojiID.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                        {
                            if (spannableStringComment.GetSpanStart(span) >= index && spannableStringComment.GetSpanEnd(span) <= index + loopTokMojiID.Length)
                                spannableStringComment.RemoveSpan(span);
                            else
                            {
                                set = false;
                                break;
                            }
                        }
                        if (set)
                        {
                            byte[] base64TokMoji = Convert.FromBase64String(TokMojiDrawables[i].TokmojImgBase64);
                            Bitmap decodedByte = BitmapFactory.DecodeByteArray(base64TokMoji, 0, base64TokMoji.Length);
                            spannableStringComment.SetSpan(new ImageSpan(decodedByte), index, index + loopTokMojiID.Length, SpanTypes.ExclusiveExclusive);
                        }
                    }

                    foreach (var index in indicesReply)
                    {
                        var set = true;
                        foreach (ImageSpan span in spannableStringReply.GetSpans(index, index + loopTokMojiID.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                        {
                            if (spannableStringReply.GetSpanStart(span) >= index && spannableStringReply.GetSpanEnd(span) <= index + loopTokMojiID.Length)
                                spannableStringReply.RemoveSpan(span);
                            else
                            {
                                set = false;
                                break;
                            }
                        }
                        if (set)
                        {
                            byte[] base64TokMoji = Convert.FromBase64String(TokMojiDrawables[i].TokmojImgBase64);
                            Bitmap decodedByte = BitmapFactory.DecodeByteArray(base64TokMoji, 0, base64TokMoji.Length);
                            spannableStringReply.SetSpan(new ImageSpan(decodedByte), index, index + loopTokMojiID.Length, SpanTypes.ExclusiveExclusive);

                        }
                    }
                }

                txtEllipseComment.SetText(spannableStringComment, TextView.BufferType.Spannable);
                CommentText.SetText(spannableStringComment, TextView.BufferType.Spannable);
                EditCommentText.SetText(spannableStringComment, TextView.BufferType.Spannable);
                EditCommentText.SetSelection(EditCommentText.Text.Length);
                ReplyUserDisplayText.SetText(spannableStringReply, TextView.BufferType.Spannable);
            }

            #endregion

            if (TokInfoVm.hasInaccurates)
            {
                lblInaccurate.Visibility = ViewStates.Visible;
                lblInaccurate.Click += (sender, e) => {
                    GoToInaccurateRepliesPage(TokInfoVm.InaccuratesCollection[0], 0);
                };
            }
        }

        private void BindCommentsViewHolder(CachingViewHolder holder, ReactionModel comment, int position)
        {
            var EditCommentText = holder.FindCachedViewById<EditText>(Resource.Id.EditCommentRowContent);
            var BtnCancelComment = holder.FindCachedViewById<Button>(Resource.Id.BtnCancelComment);
            var BtnUpdateComment = holder.FindCachedViewById<Button>(Resource.Id.BtnUpdateComment);
            var PopUpMenuComments = holder.FindCachedViewById<TextView>(Resource.Id.lblCommentPopUpMenu);
            var ImgCommentUserPhoto = holder.FindCachedViewById<ImageView>(Resource.Id.imgcomment_userphoto);
            ImgCommentUserPhoto.ContentDescription = comment.UserId;

            var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
            if (comment.UserId == Settings.GetUserModel().UserId && !string.IsNullOrEmpty(cacheUserPhoto))
            {
                var userPhotoByte = Convert.FromBase64String(cacheUserPhoto);
                ImgCommentUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
            }
            else
            {
                Glide.With(this).Load(comment.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ImgCommentUserPhoto);
            }

            var CommentorName = holder.FindCachedViewById<TextView>(Resource.Id.lbl_commentnameuser);
            CommentorName.Text = comment.UserDisplayName;
            CommentorName.ContentDescription = comment.UserId;

            BtnCancelComment.Tag = position;
            BtnUpdateComment.Tag = position;
            PopUpMenuComments.Tag = position;
            EditCommentText.Tag = position;

            holder.FindCachedViewById<TextView>(Resource.Id.lbl_commentdate).Text = DateConvert.ConvertToRelative(comment.CreatedTime).ToString();
            var kind = holder.FindCachedViewById<TextView>(Resource.Id.lblCommentRowKind);
            kind.Text = char.ToUpper(comment.Kind[0]) + comment.Kind.Substring(1);
            if (comment.Kind.ToLower() == "accurate")
            {
                kind.SetBackgroundColor(Color.DarkGreen);
            }
            else if (comment.Kind.ToLower() == "inaccurate")
            {
                kind.SetBackgroundColor(Color.Red);
            }
            else
            {
                kind.Visibility = ViewStates.Gone;
            }

            var txtCommentHeartCount = holder.FindCachedViewById<TextView>(Resource.Id.txtCommentHeartCount);
            var btnCommentHeart = holder.FindCachedViewById<Button>(Resource.Id.btnCommentHeart);
            var BtnCommentReply = holder.FindCachedViewById<Button>(Resource.Id.BtnCommentReply);
            var BtnViewMoreClose = holder.FindCachedViewById<Button>(Resource.Id.btnViewMoreCloseComment);
            var CommentTextEllipsize = holder.FindCachedViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
            var CommentText = holder.FindCachedViewById<TextView>(Resource.Id.lblCommentRowContent);
            txtCommentHeartCount.Text = comment.Likes == null ? "0" : comment.Likes.ToString();

            if (comment.UserLiked)
            {
                btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));
            }
            else
            {
                btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.placeholder_bg)));
            }

            btnCommentHeart.Typeface = font;
            CommentText.Text = comment.Text;
            CommentTextEllipsize.Text = comment.Text;
            EditCommentText.Text = comment.Text;

            btnCommentHeart.Tag = position;
            btnCommentHeart.ContentDescription = "like";
            btnCommentHeart.Click += async (sender, e) =>
            {
                int ndx = (int)btnCommentHeart.Tag;

                int heartCnt = int.Parse(txtCommentHeartCount.Text);
                var tokketUser = Settings.GetTokketUser();
                if (!TokInfoVm.CommentsCollection[ndx].UserLiked && btnCommentHeart.CurrentTextColor != new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)))
                {
                    reactionUser = comment;
                    reactionUser.ParentItem = comment.Id;
                    reactionUser.ParentUser = comment.UserId;
                    reactionUser.Kind = "like";
                    reactionUser.Label = "reaction";
                    reactionUser.DetailNum = comment.DetailNum;
                    reactionUser.CategoryId = comment.CategoryId;
                    reactionUser.TokTypeId = comment.TokTypeId;
                    reactionUser.OwnerId = tokketUser.Id;
                    reactionUser.ItemId = comment.ItemId;
                    reactionUser.IsChild = true;
                    reactionUser.UserDisplayName = comment.UserDisplayName;
                    reactionUser.UserPhoto = comment.UserPhoto;
                    reactionUser.Timestamp = DateTime.Now;
                    reactionUser.IsComment = true;
                    reactionUser.UserLiked = true;
                    reactionUser.UserId = Settings.GetUserModel().UserId;

                    comment.Likes = heartCnt + 1;
                    txtCommentHeartCount.Text = (heartCnt + 1).ToString();
                    btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));

                    RunOnUiThread(async () => await OnClickAddReaction(btnCommentHeart));
                }
                else
                {
                    var userLikedId = $"like-{comment.Id}-{Settings.GetUserModel().UserId}";
                    var result = await ReactionService.Instance.DeleteReaction(userLikedId);

                    if (result)
                    {
                        TokInfoVm.CommentsCollection[ndx].UserLiked = false;
                        comment.Likes = heartCnt - 1;
                        txtCommentHeartCount.Text = (heartCnt - 1).ToString();
                        btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.placeholder_bg)));
                    }
                    else
                    {
                        alertMessage("", "Failed to unlike comment.", 0);
                    }
                }
            };

            BtnViewMoreClose.Click += delegate
            {
                if (CommentTextEllipsize.Visibility == ViewStates.Visible)
                {
                    CommentTextEllipsize.Visibility = ViewStates.Gone;
                    CommentText.Visibility = ViewStates.Visible;
                    BtnViewMoreClose.Text = "Close";
                }
                else
                {
                    CommentTextEllipsize.Visibility = ViewStates.Visible;
                    CommentText.Visibility = ViewStates.Gone;
                    BtnViewMoreClose.Text = "View more";
                }
            };

            var LinearTokInfoReplyPreview = holder.FindCachedViewById<LinearLayout>(Resource.Id.LinearTokInfoReplyPreview);
            var BtnShowHideComments = holder.FindCachedViewById<Button>(Resource.Id.btnShowHideComment);
            //var result = await ReactionService.Instance.GetCommentReplyAsync(new ReactionQueryValues() { reaction_id = comment.Id, kind = "comments", detail_number = -1, item_id = tokModel.Id, pagination_id = "" });
            //Settings.ContinuationToken = result.ContinuationToken;
            //var repliesResult = result.Results.ToList();

            //Show 1 reply for preview
            var CircleReplyUserPhoto = holder.FindCachedViewById<CircleImageView>(Resource.Id.CircleReplyUserPhoto);
            var ReplyUserDisplayName = holder.FindCachedViewById<TextView>(Resource.Id.TokInfoCommentsReplyUsername);
            var ReplyUserDisplayText = holder.FindCachedViewById<TextView>(Resource.Id.TokInfoCommentsReplyText);

            if (comment.Children != null)
            {
                if (comment.Children.Count > 0)
                {
                    LinearTokInfoReplyPreview.Visibility = ViewStates.Visible;
                    BtnShowHideComments.Text = "View " + comment.Children.Count + " replies";

                    var cacheCommentUserPhoto = MainActivity.Instance.cacheUserPhoto;
                    if (comment.Children[0].UserId == Settings.GetUserModel().UserId && !string.IsNullOrEmpty(cacheCommentUserPhoto))
                    {
                        CircleReplyUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                    }
                    else
                    {
                        Glide.With(this).Load(comment.Children[0].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(CircleReplyUserPhoto);
                    }
                    ReplyUserDisplayName.Text = comment.Children[0].UserDisplayName;
                    ReplyUserDisplayText.Text = comment.Children[0].Text;
                }
                else
                {
                    LinearTokInfoReplyPreview.Visibility = ViewStates.Gone;
                    BtnShowHideComments.Visibility = ViewStates.Gone;
                }
            }

            BtnShowHideComments.Click += (object sender, EventArgs e) =>
            {
                GoToRepliesPage(comment, position);
            };

            LinearTokInfoReplyPreview.Click += delegate
            {
                GoToRepliesPage(comment, position);
            };

            BtnCommentReply.Click += (sender, e) =>
            {
                GoToRepliesPage(comment, position);
            };
        }

        private void GoToRepliesPage(ReactionModel comment, int position)
        {
            var tokmojiConvert = JsonConvert.SerializeObject(TokInfoVm.TokMojiCollection);
            var commentConvert = JsonConvert.SerializeObject(comment);
            var repliesConvert = JsonConvert.SerializeObject(comment.Children);
            LocalSettings.TokMojidrawable = JsonConvert.SerializeObject(TokMojiDrawables);

            Settings.ContinuationToken = comment.ChildrenToken;

            nextActivity = new Intent(this, typeof(TokInfoRepliesPageActivity));
            nextActivity.PutExtra("commentReaction", commentConvert);
            nextActivity.PutExtra("repliesCollection", repliesConvert);
            nextActivity.PutExtra("tokMojiCollection", tokmojiConvert);
            nextActivity.PutExtra("commentPosition", position);
            StartActivityForResult(nextActivity, REQUEST_TOK_INFO_REPLY);
        }


        private void GoToInaccurateRepliesPage(ReactionModel comment,int position) {
            if (comment.Children == null) {
                comment.Children = new List<TokkepediaReaction>();
            }
            var tokmojiConvert = JsonConvert.SerializeObject(TokInfoVm.TokMojiCollection);
            var commentConvert = JsonConvert.SerializeObject(comment);
            var repliesConvert = JsonConvert.SerializeObject(comment.Children);
            LocalSettings.TokMojidrawable = JsonConvert.SerializeObject(TokMojiDrawables);

            Settings.ContinuationToken = comment.ChildrenToken;
            var convert = JsonConvert.SerializeObject(TokInfoVm.InaccuratesCollection);
            nextActivity = new Intent(this, typeof(InaccurateRepliesPage));
            nextActivity.PutExtra("inaccurateReaction", convert);
            nextActivity.PutExtra("commentReaction", commentConvert);
            nextActivity.PutExtra("repliesCollection", repliesConvert);
            nextActivity.PutExtra("tokMojiCollection", tokmojiConvert);
            nextActivity.PutExtra("commentPosition", position);
            StartActivityForResult(nextActivity, REQUEST_TOK_INFO_REPLY);
        }

        public void UpdateReplies(int position, List<ReactionModel> listReplies)
        {
            View view = RecyclerComments.GetChildAt(position);
            //Show 1 reply for preview
            var LinearTokInfoReplyPreview = view.FindViewById<LinearLayout>(Resource.Id.LinearTokInfoReplyPreview);
            var txtCommentHeartCount = view.FindViewById<TextView>(Resource.Id.txtCommentHeartCount);
            var BtnShowHideComments = view.FindViewById<Button>(Resource.Id.btnShowHideComment);
            var CircleReplyUserPhoto = view.FindViewById<CircleImageView>(Resource.Id.CircleReplyUserPhoto);
            var ReplyUserDisplayName = view.FindViewById<TextView>(Resource.Id.TokInfoCommentsReplyUsername);
            var ReplyUserDisplayText = view.FindViewById<TextView>(Resource.Id.TokInfoCommentsReplyText);

            txtCommentHeartCount.Text = TokInfoVm.CommentsCollection[position].Likes == null ? "0" : TokInfoVm.CommentsCollection[position].Likes.ToString();
            if (listReplies.Count > 0)
            {
                LinearTokInfoReplyPreview.Visibility = ViewStates.Visible;
                BtnShowHideComments.Text = "View " + listReplies.Count + " replies";

                var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
                if (listReplies[0].UserId == Settings.GetUserModel().UserId && !string.IsNullOrEmpty(cacheUserPhoto))
                {
                    CircleReplyUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                }
                else
                {
                    Glide.With(this).Load(listReplies[0].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(CircleReplyUserPhoto);
                }

                ReplyUserDisplayName.Text = listReplies[0].UserDisplayName;
                ReplyUserDisplayText.Text = listReplies[0].Text;
            }
        }

        private void BindTokMojiViewHolder(CachingViewHolder holder, Tokmoji tokmoji, int position)
        {
            var ImgTokMoji = holder.FindCachedViewById<ImageView>(Resource.Id.imgTokInfoTokMojiRow);
            Glide.With(this).Load(tokmoji.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgTokMoji);

            ImgTokMoji.ContentDescription = tokmoji.Id;
            ImgTokMoji.Click -= displayImageinCommentEditor;
            ImgTokMoji.Click += displayImageinCommentEditor;
        }
        private void BindTokMojiViewHolderDummy(CachingViewHolder holder, Tokmoji tokmoji, int position)
        {
            var ImgTokMoji = holder.FindCachedViewById<ImageView>(Resource.Id.imgTokInfoTokMojiRow);
            Glide.With(this).Load(tokmoji.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgTokMoji);

            ImgTokMoji.ContentDescription = tokmoji.Id;
            ImgTokMoji.Click -= displayImageinCommentEditor;
            ImgTokMoji.Click += displayImageinCommentEditor;
        }

        private void displayImageinCommentEditor(object sender, EventArgs e)
        {
            AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage("This tokmoji costs 3 coins. Continue?");
            alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Proceed</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {

                var spannableTokMoji = new SpannableString((sender as ImageView).ContentDescription);
                int start = CommentEditor.SelectionStart;
                string tokmojiidx = (sender as ImageView).ContentDescription;
                string tokidx = ":" + tokmojiidx + ":";
                string spaceafter = tokidx + " ";

                //TokMoji Purchase
                txtProgressText.Text = "Purchasing...";
                linearProgress.Visibility = ViewStates.Visible;
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                var result = await TokMojiService.Instance.PurchaseTokmojiAsync(tokmojiidx, "tokmoji");

                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                linearProgress.Visibility = ViewStates.Gone;
                txtProgressText.Text = "Loading...";

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    //Update Coins
                    Settings.UserCoins -= 3;
                    MainActivity.Instance.TextCoinsToast.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);
                    ProfileFragment.Instance.TextProfileCoins.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);

                    SpannableString spannableString;
                    var resultObject = result.ResultObject as PurchasedTokmoji;

                    if (constraintMegaInaccurateComment.Visibility == ViewStates.Visible)
                    {
                        EditInaccurateComment.Text = EditInaccurateComment.Text.Substring(0, start) + spaceafter + EditInaccurateComment.Text.Substring(start);
                        spannableString = new SpannableString(EditInaccurateComment.Text);
                    }
                    else
                    {
                        CommentEditor.Text = CommentEditor.Text.Substring(0, start) + spaceafter + CommentEditor.Text.Substring(start);
                        spannableString = new SpannableString(CommentEditor.Text);
                    }


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

                    if (constraintMegaInaccurateComment.Visibility == ViewStates.Visible)
                    {
                        EditInaccurateComment.SetText(spannableString, TextView.BufferType.Spannable);
                        EditInaccurateComment.SetSelection(start + spaceafter.Length);
                    }
                    else
                    {
                        CommentEditor.SetText(spannableString, TextView.BufferType.Spannable);
                        CommentEditor.SetSelection(start + spaceafter.Length);
                    }
                }
                else
                {
                    alertMessage("", "Not enough coins.", Resource.Drawable.alert_icon_blue);
                }
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }
        private void alertMessage(string title, string message, int icon)
        {
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(icon);
            alertDialog.SetMessage(message);
            alertDialog.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        private async Task loadSections()
        {
            var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(tokModel.Id);
            var getTokSections = getTokSectionsResult.Results;
            tokModel.Sections = getTokSections.ToArray();
            MegaTokSections();
        }

        private async Task loadQnASections()
        {
            var getTokSectionsResult = await TokService.Instance.GetQnATokSectionsAsync(tokModel.Id);
            var getTokSections = getTokSectionsResult.Results;
            listTokContentQna = getTokSections.ToList();
            tokModel.Sections = getTokSections.ToArray();
            QnATokSections();
        }

        private void AddTokDetails(int calledfromonactivityresult = 0)
        {
            linearParent.RemoveAllViews();
            if (calledfromonactivityresult == 1)
            {
                if (!tokModel.IsDetailBased)
                {
                    View view = LayoutInflater.Inflate(Resource.Layout.tokinfo_detail1, null);
                    var btnAccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                    var btnInaccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                    var txtDetail = view.FindViewById<TextView>(Resource.Id.lbl_detail);
                    var btnTotalReaction = view.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                    var circleprogressReaction = view.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                    var videoView = view.FindViewById<VideoView>(Resource.Id.detailVid);
                    var viewHorizontalTopLine = view.FindViewById<View>(Resource.Id.viewHorizontalTopLine);

                    if (classTokModel.TokGroup.ToLower() == "list")
                    {
                        btnAccurate.Visibility = ViewStates.Gone;
                        btnInaccurate.Visibility = ViewStates.Gone;
                        btnTotalReaction.Visibility = ViewStates.Gone;
                        circleprogressReaction.Visibility = ViewStates.Gone;
                    }
                    if (!string.IsNullOrEmpty(classTokModel.TokLink))
                    {
                        txtDetail.SetTextColor(Color.DeepSkyBlue);
                        txtDetail.PaintFlags = PaintFlags.UnderlineText;

                        txtDetail.Click += async delegate
                        {
                            await gotoLink_Clicked(classTokModel.TokLink);
                        };
                    }

                    hashtagText = new SpannableString(tokModel.SecondaryFieldText);
                    Matcher matcher = Pattern.Compile(hashtagCode).Matcher(hashtagText);
                    while (matcher.Find())
                    {
                        hashtagText.SetSpan(new ClickableText(Actions.OpenHashTag, matcher.Group()), matcher.Start(), matcher.End(), 0);
                    }

                    txtDetail.Append(hashtagText);
                    txtDetail.MovementMethod = LinkMovementMethod.Instance;
                    //txtDetail.SetText(hashtagText, TextView.BufferType.Spannable);

                    btnAccurate.Visibility = ViewStates.Gone;
                    btnInaccurate.Visibility = ViewStates.Gone;
                    btnTotalReaction.Visibility = ViewStates.Gone;
                    circleprogressReaction.Visibility = ViewStates.Gone;
                    linearParent.AddView(view);

                    if (!tokModel.IsEnglish)
                    {
                        isEnglishLinear.RemoveAllViews();
                        View viewEnglish2 = LayoutInflater.Inflate(Resource.Layout.tokinfo_isenglish, null);
                        TextView txtSecEnglish = viewEnglish2.FindViewById<TextView>(Resource.Id.lbl_tokConvertAnswer1);
                        txtSecEnglish.Text = tokModel.EnglishSecondaryFieldText;
                        isEnglishLinear.AddView(viewEnglish2);
                    }
                }
            }

            if (tokModel.Details != null)
            {
                var hasIndents = tokModel.IsIndent != null;
                for (int i = 0; i < tokModel.Details.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tokModel.Details[i]))
                    {
                        View view = LayoutInflater.Inflate(Resource.Layout.tokinfo_detail1, null);
                        var btnAccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                        var btnInaccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                        var DetailDesc = view.FindViewById<TextView>(Resource.Id.lbl_detail);
                        var ImgDetail = view.FindViewById<ImageView>(Resource.Id.tokinfo_imgdetail);
                        var btnComment = view.FindViewById<Button>(Resource.Id.btnTokInfo_detailComment);
                        var videoView = view.FindViewById<VideoView>(Resource.Id.detailVid);
                        var btnTotalReaction = view.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                        var circleprogressReaction = view.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                        var viewHorizontalTopLine = view.FindViewById<View>(Resource.Id.viewHorizontalTopLine);

                        if (classTokModel.TokGroup.ToLower() == "list")
                        {
                            btnAccurate.Visibility = ViewStates.Gone;
                            btnInaccurate.Visibility = ViewStates.Gone;
                            btnTotalReaction.Visibility = ViewStates.Gone;
                            circleprogressReaction.Visibility = ViewStates.Gone;
                        }

                        if (classTokModel.DetailTokLinks != null)
                        {
                            if (i < classTokModel.DetailTokLinks.Length)
                            {
                                if (!string.IsNullOrEmpty(classTokModel.DetailTokLinks[i]))
                                {
                                    DetailDesc.SetTextColor(Color.DeepSkyBlue);
                                    DetailDesc.PaintFlags = PaintFlags.UnderlineText;
                                    DetailDesc.ContentDescription = classTokModel.DetailTokLinks[i];

                                    DetailDesc.Click += async delegate
                                    {
                                        await gotoLink_Clicked(DetailDesc.ContentDescription);
                                    };
                                }
                            }
                        }

                        btnComment.Tag = i + 1; //Add + 1 for detailnum OnClickAddReaction

                        btnAccurate.Typeface = font;
                        btnAccurate.Tag = i + 1; //Add + 1 for detailnum OnClickAddReaction
                        btnInaccurate.Typeface = font;
                        btnInaccurate.Tag = i;


                        btnAccurate.Click += async (sender, e) =>
                        {
                            await OnClickAddReaction(btnAccurate, 0); //Set default position = 0
                        };

                        btnComment.Click += async (sender, e) =>
                        {
                            await OnClickAddReaction(btnComment, 0); //Set default position = 0
                        };

                        hashtagText = new SpannableString(tokModel.Details[i] ?? "");
                        Matcher matcher = Pattern.Compile(hashtagCode).Matcher(hashtagText);
                        if (IsUrl(hashtagText.ToString())) {
                            videoView.Visibility = ViewStates.Visible;
                            var uri  = NetUri.Parse(hashtagText.ToString());
                            videoView.SetVideoURI(uri) ;
                            var controller = new MediaController(this);
                            controller.SetAnchorView(videoView);
                            videoView.SetMediaController(controller);
                            videoView.Start();
                        }
                        while (matcher.Find())
                        {
                            hashtagText.SetSpan(new ClickableText(Actions.OpenHashTag, matcher.Group()), matcher.Start(), matcher.End(), 0);
                        }
                        var bulletType = tokModel.BulletKind == null ? "bullet" : tokModel.BulletKind.ToLower();
                        switch (bulletType)
                        {
                            case "none": DetailDesc.Append(hashtagText); break;
                            case "bullet":

                                if (tokModel.IsIndent == null)
                                    tokModel.IsIndent = new List<bool>() { false, false, false, false, false, false, false, false, false, false };


                                if (!tokModel.IsIndent[i])
                                {
                                    DetailDesc.Append("\u2022 " + hashtagText);
                                    DetailDesc.SetTypeface(null, TypefaceStyle.Bold);
                                }
                                else {
                                    DetailDesc.Append("     \u25E6 " + hashtagText);
                                }
                                break;
                            case "number": DetailDesc.Append($"{i + 1}.) " + hashtagText); break;
                            case "letter": DetailDesc.Append($"{((char)i + 1).ToString().ToUpper()}.)" + hashtagText); break;
                            default: DetailDesc.Append($"{i + 1}.) " + hashtagText); break;

                        }

                        DetailDesc.MovementMethod = LinkMovementMethod.Instance;

                        //DetailDesc.SetText("\u2022 " + hashtagText, TextView.BufferType.Spannable);

                        if (tokModel.DetailImages != null)
                        {
                            if (i < tokModel.DetailImages.Length)
                            {
                                if (URLUtil.IsValidUrl(tokModel.DetailImages[i]))
                                {
                                    Glide.With(this).Load(tokModel.DetailImages[i]).Into(ImgDetail);
                                }
                                else
                                {
                                    if (tokModel.DetailImages[i] != null)
                                    {
                                        tokModel.DetailImages[i] = tokModel.DetailImages[i].Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");
                                        byte[] imageDetailBytes = Convert.FromBase64String(tokModel.DetailImages[i]);
                                        ImgDetail.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                                    }
                                }
                            }
                        }

                        if (linearParent.ChildCount > 0)
                        {
                            viewHorizontalTopLine.Visibility = ViewStates.Visible;
                        }

                        linearParent.AddView(view);
                    }
                }
            }
        }
        private void MegaTokSections()
        {
            linearParent.RemoveAllViews();

            for (int i = 0; i < tokModel.Sections.Length; i++)
            {
                View view = LayoutInflater.Inflate(Resource.Layout.tokinfo_megasectiondetail, null);
                //Button
                var btnEdit = view.FindViewById<Button>(Resource.Id.btnTokInfoMegaEditDtl);
                var btnView = view.FindViewById<Button>(Resource.Id.btnTokInfoMegaViewDtl);
                var btnRemove = view.FindViewById<Button>(Resource.Id.btnTokInfoMegaRemoveDtl);
                var txtView = view.FindViewById<TextView>(Resource.Id.txtView);
                //tags
                btnEdit.Tag = i;
                btnView.Tag = i;
                btnRemove.Tag = i;

                btnEdit.Typeface = font;
                btnView.Typeface = font;
                btnRemove.Typeface = font;
                //Button End
                var MegaNumber = view.FindViewById<TextView>(Resource.Id.lbl_tokinfo_megaNumber);
                var Title = view.FindViewById<TextView>(Resource.Id.lbl_tokinfo_megaTitle);
                var Content = view.FindViewById<TextView>(Resource.Id.lbl_tokinfo_megaContent);
                var ImgDetail = view.FindViewById<ImageView>(Resource.Id.tokinfo_imgmegadetail);

                MegaNumber.Text = (i + 1).ToString();
                Title.Text = tokModel.Sections[i].Title ?? "";
                Content.Text = tokModel.Sections[i].Content ?? "";

                if (URLUtil.IsValidUrl(tokModel.Sections[i].Image))
                {
                    Glide.With(this).Load(tokModel.Sections[i].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgDetail);
                }
                else
                {
                    if (tokModel.Sections[i].Image != null)
                    {
                        byte[] imageDetailBytes = Convert.FromBase64String(tokModel.Sections[i].Image);
                        ImgDetail.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                    }
                }

                txtView.ContentDescription = i.ToString();
                txtView.Click -= viewMegaSection_Click;
                txtView.Click += viewMegaSection_Click;

                linearParent.AddView(view);
            }
        }

        private void QnATokSections()
        {
            linearParent.RemoveAllViews();
            
            for (int i = 0; i < listTokContentQna.Count; i++)
            {
                View viewChild = LayoutInflater.Inflate(Resource.Layout.item_tok_info_q_n_a, null);
                var txtContent = viewChild.FindViewById<TextView>(Resource.Id.txtContent);
                var linearChild = viewChild.FindViewById<LinearLayout>(Resource.Id.linearLayout);
                linearChild.Tag = i;

                #region Content
                txtContent.Tag = i;
                txtContent.Text = tokModel.Sections[i].Content;
                txtContent.ContentDescription = "hide";
                txtContent.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.caret_up, 0, 0, 0);

                txtContent.Click -= txtContentIsClicked;
                txtContent.Click += txtContentIsClicked;
                #endregion

                #region Questions
                if (listTokContentQna[i].QuestionAnswer != null)
                {
                    for (int j = 0; j < listTokContentQna[i].QuestionAnswer.Count; j++)
                    {
                        View viewQuestions = LayoutInflater.Inflate(Resource.Layout.tokinfo_detail1, null);
                        var btnAccurate = viewQuestions.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                        var btnInaccurate = viewQuestions.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                        var DetailDesc = viewQuestions.FindViewById<TextView>(Resource.Id.lbl_detail);
                        var ImgDetail = viewQuestions.FindViewById<ImageView>(Resource.Id.tokinfo_imgdetail);
                        var btnComment = viewQuestions.FindViewById<Button>(Resource.Id.btnTokInfo_detailComment);
                        var videoView = viewQuestions.FindViewById<VideoView>(Resource.Id.detailVid);
                        var btnTotalReaction = viewQuestions.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                        var circleprogressReaction = viewQuestions.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                        var viewHorizontalTopLine = viewQuestions.FindViewById<View>(Resource.Id.viewHorizontalTopLine);

                        btnTotalReaction.Visibility = ViewStates.Gone;
                        circleprogressReaction.Visibility = ViewStates.Gone;

                        btnAccurate.Typeface = font;
                        btnAccurate.Tag = j;
                        btnInaccurate.Typeface = font;
                        btnInaccurate.Tag = j;

                        btnAccurate.Click += async (sender, e) =>
                        {
                            await OnClickAddReaction(btnAccurate, 0);
                        };

                        btnComment.Click += async (sender, e) =>
                        {
                            await OnClickAddReaction(btnComment, 0);
                        };

                        DetailDesc.Text = "\u2022" + listTokContentQna[i].QuestionAnswer[j].Question ?? "";
                        linearChild.AddView(viewQuestions);

                        #region Answer layout
                        View viewAnswers = LayoutInflater.Inflate(Resource.Layout.layout_tokinfo_qna_answer, null);
                        var txtAnswers = viewAnswers.FindViewById<TextView>(Resource.Id.txtAnswer);
                        var btnShowHideAnswer = viewAnswers.FindViewById<Button>(Resource.Id.btnShowHideAnswer);
                        txtAnswers.Tag = j;
                        btnShowHideAnswer.Tag = j;

                        string answer = listTokContentQna[i].QuestionAnswer[j].Answer ?? "";
                        txtAnswers.ContentDescription = answer;
                        //txtAnswers.Text = "\u2022 " + answerQna(answer); //Hide answer by default

                        if (classTokModel.AnswerEnabled)
                        {
                            txtAnswers.Text = "A" + (j + 1) + ": ";
                            btnShowHideAnswer.Text = "(Click for answer)";
                            btnShowHideAnswer.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorPrimary)));

                            //Even only called if answer is enabled
                            btnShowHideAnswer.Click -= btnShowHideClicked;
                            btnShowHideAnswer.Click += btnShowHideClicked;
                        }
                        else
                        {
                            txtAnswers.Text = "A" + (j + 1) + ": ";
                            btnShowHideAnswer.Text = "(Answer Hidden)";
                            btnShowHideAnswer.SetTextColor(Color.Black);
                        }


                        //Comment due to new UI requirements
                        /*if (string.IsNullOrEmpty(answer))
                            btnShowHideAnswer.Visibility = ViewStates.Gone;*/
                        #endregion

                        if (linearParent.ChildCount > 0)
                        {
                            viewHorizontalTopLine.Visibility = ViewStates.Visible;
                        }

                        linearChild.AddView(viewAnswers);
                    }
                }
                #endregion

                linearParent.AddView(viewChild);
            }
        }

        private void btnShowHideClicked(object sender, EventArgs e)
        {
            var btnShowHideAnswer = sender as Button;
            int ndx = 0;
            try { ndx = (int)btnShowHideAnswer.Tag; } catch { ndx = int.Parse((string)btnShowHideAnswer.Tag); }

            var parentView = btnShowHideAnswer.Parent as View;
            var txtAnswers = parentView.FindViewById<TextView>(Resource.Id.txtAnswer);

            if (btnShowHideAnswer.Text.ToLower().Contains("click for answer"))
            {
                btnShowHideAnswer.Text = "(Click to hide)";
                btnShowHideAnswer.SetTextColor(Color.Black);

                txtAnswers.Text = "A" + (ndx + 1) + ": " + txtAnswers.ContentDescription;
                //txtAnswers.Text = "\u2022 " + txtAnswers.ContentDescription;

            }
            else if (btnShowHideAnswer.Text.ToLower().Contains("hide"))
            {
                btnShowHideAnswer.Text = "(Click for answer)";
                btnShowHideAnswer.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorPrimary)));

                txtAnswers.Text = "A" + (ndx + 1) + ": ";
                //txtAnswers.Text = "\u2022 " + answerQna(txtAnswers.ContentDescription);
            }
        }

        private string answerQna(string words)
        {
            string answer = "";
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words))
                {
                    answer += words[i];
                }
                else if (i > 0 && i <= 12)
                {
                    answer += "_";
                }
                else
                {
                    answer += words[i];
                }
            }
            return answer;
        }
        private void txtContentIsClicked(object sender, EventArgs e)
        {
            var txtContent = sender as TextView;
            int ndx = 0;
            try { ndx = (int)txtContent.Tag; } catch { ndx = int.Parse((string)txtContent.Tag); }
            
            View viewLinear = linearParent.GetChildAt(ndx);
            var linearLayout = viewLinear.FindViewById<LinearLayout>(Resource.Id.linearLayout);
            if (txtContent.ContentDescription == "show")
            {
                txtContent.ContentDescription = "hide";
                txtContent.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.caret_up, 0, 0, 0);
                linearLayout.Visibility = ViewStates.Visible;
            }
            else if (txtContent.ContentDescription == "hide")
            {
                txtContent.ContentDescription = "show";
                txtContent.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.caret_down, 0, 0, 0);
                linearLayout.Visibility = ViewStates.Gone;
            }
        }

        private void viewMegaSection_Click(object sender, EventArgs e)
        {
            double widthD = getLayoutWidth();
            int ndx = int.Parse((sender as TextView).ContentDescription);

            var megaSectionDialog = new Dialog(this);
            megaSectionDialog.SetContentView(Resource.Layout.dialog_megasection_viewer);
            megaSectionDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            megaSectionDialog.Show();


            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            megaSectionDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
            megaSectionDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            // Access Popup layout fields like below  
            var txtViewSection = megaSectionDialog.FindViewById<TextView>(Resource.Id.txtViewSection);
            var txtCharacterCount = megaSectionDialog.FindViewById<TextView>(Resource.Id.txtCharacterCount);
            var txtSectionTitle = megaSectionDialog.FindViewById<TextView>(Resource.Id.txtSectionTitle);
            var txtSectionContent = megaSectionDialog.FindViewById<TextView>(Resource.Id.txtSectionContent);
            var ImgDetail = megaSectionDialog.FindViewById<ImageView>(Resource.Id.tokinfo_imgmegadetail);
            var btnClose = megaSectionDialog.FindViewById<Button>(Resource.Id.btnClose);

            // Events for that popup layout  
            btnClose.Click += delegate
            {
                megaSectionDialog.Dismiss();
            };

            txtViewSection.Text = "View Section " + (ndx + 1);
            txtCharacterCount.Text = "Character Count " + (tokModel.Sections[ndx].Title ?? "").Length;
            txtSectionTitle.Text = tokModel.Sections[ndx].Title ?? "";
            txtSectionContent.Text = tokModel.Sections[ndx].Content ?? "";

            ImgDetail.Click -= viewImageMega;
            ImgDetail.Click += viewImageMega;
            if (URLUtil.IsValidUrl(tokModel.Sections[ndx].Image))
            {
                Glide.With(this).Load(tokModel.Sections[ndx].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(ImgDetail);
            }
            else
            {
                if (tokModel.Sections[ndx].Image != null)
                {
                    byte[] imageDetailBytes = Convert.FromBase64String(tokModel.Sections[ndx].Image);
                    ImgDetail.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }
            }
        }
        private void viewImageMega(object sender, EventArgs e)
        {
            gotoImageViewer(sender as ImageView);
        }

        private async Task getResultDataUserReaction(bool isCache)
        {
            if (isCache)
            {
                resultDataUserReaction = TokService.Instance.GetUserReactionsCache(fromCallerUserReaction);
            }
            else
            {
                resultDataUserReaction = await TokService.Instance.UserReactionsGet(tokModel.Id, fromCallerUserReaction);
            }
        }

        private async Task getReactionValue(string Id = "", bool isCache = false)
        {
            if (isCache)
            {
                reactionValue = ReactionService.Instance.GetReactionsValueCache(fromCallerReactionValue);
            }
            else
            {
                reactionValue = await ReactionService.Instance.GetReactionsValueAsync(Id, fromCallerReactionValue);
            }

            ProgressViews.Visibility = ViewStates.Gone;
        }
        private async Task LoadSelectedGems(string reaction = "All", string Id = "", long Index = 0, bool isCache = false)
        {
            reactionValueVM = new ReactionValueViewModel();

            List<Task> tasksList = new List<Task>();

            tasksList.Add(getReactionValue(Id, isCache));
            tasksList.Add(getResultDataUserReaction(isCache));

            await Task.WhenAll(tasksList);

            await LoadSelectedGemsData(reaction, Id, Index);
        }
        private async Task LoadSelectedGemsData(string reaction = "All", string Id = "", long Index = 0)
        {
            //reactionValue = await ReactionService.Instance.GetReactionsValueAsync(Id);
            if (reactionValue == null)
            {
                return;
            }
            if (reactionValue.GemsModel != null)
            {
                reactionValueVM.GemA = (long)GetPropValue(reactionValue.GemsModel, "GemA" + (Index == 0 ? "" : Index.ToString()));
                reactionValueVM.GemB = (long)GetPropValue(reactionValue.GemsModel, "GemB" + (Index == 0 ? "" : Index.ToString()));
                reactionValueVM.GemC = (long)GetPropValue(reactionValue.GemsModel, "GemC" + (Index == 0 ? "" : Index.ToString()));

                //Since there is no data loaded from this function UserReactionsGet. We will use reactionValueVM instead
                //TODO remove this once UserReactionsGet will work again
                if (resultDataUserReaction.Results.Count() == 0)
                {
                    if (reactionValueVM.GemA > 0)
                    {
                        DisplaySelectedGem("gema", null);
                    }

                    if (reactionValueVM.GemB > 0)
                    {
                        DisplaySelectedGem("gemb", null);
                    }

                    if (reactionValueVM.GemC > 0)
                    {
                        DisplaySelectedGem("gemc", null);
                    }
                }
            }

            //Comment the code below because it will create a duplicate reaction in the loop below resultDataUserReaction.Results
            //if (reactionValue.CommentsModel != null)
            //{
            //    reactionValueVM.Accurate = (long)GetPropValue(reactionValue.CommentsModel, "Accurate" + (Index == 0 ? "" : Index.ToString()));
            //    reactionValueVM.Inaccurate = (long)GetPropValue(reactionValue.CommentsModel, "Inaccurate" + (Index == 0 ? "" : Index.ToString()));
            //}

            if (reactionValue.ViewsModel != null)
            {
                //if (Settings.GetUserModel().UserId == )
                TextTotalViews.Text = (reactionValue.ViewsModel.TileTapViews + reactionValue.ViewsModel.TileTapViewsPersonal + reactionValue.ViewsModel.PageVisitViews).ToString();

                TextToolTotalViews.Text = "Total Views: " + TextTotalViews.Text;
                TextTotalOpened.Text = "Tok was opened: " + reactionValue.ViewsModel.TileTapViews.ToString();
                TextTotalVisited.Text = "Tok was visited: " + reactionValue.ViewsModel.PageVisitViews.ToString();
                TextTotalOpenedByOwner.Text = "Tok was opened by its owner: " + reactionValue.ViewsModel.TileTapViewsPersonal.ToString();
                TextTotalVisitedByOwner.Text = "Tok was visited by its owner: " + reactionValue.ViewsModel.PageViewsPersonal.ToString();
            }


            //var getUserReaction = await TokService.Instance.UserReactionsGet(tokModel.Id);

            bool isDetailExist = false;
            if (resultDataUserReaction != null)
            {
                foreach (var item in resultDataUserReaction.Results)
                {
                    int i = Convert.ToInt32(item.DetailNum);
                    if (i != 0)
                    {
                        isDetailExist = true;
                    }
                }
            }

            if (!isDetailExist)
            {
                for (int zz = 0; zz < linearParent.ChildCount; zz++)
                {
                    View viewLinear = linearParent.GetChildAt(zz);
                    var circleprogressReaction = viewLinear.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                    //Hide ProgressBar
                    if (circleprogressReaction != null)
                    {
                        circleprogressReaction.Visibility = ViewStates.Gone;
                    }
                }
            }

            if (resultDataUserReaction != null)
            {
                foreach (var item in resultDataUserReaction.Results)
                {
                    var kind = item.Kind.ToLower();
                    int i = Convert.ToInt32(item.DetailNum);
                    updateReactionBackground(kind);

                    if (i == 0)
                    {
                        DisplaySelectedGem(kind, item);
                    }
                    else
                    {
                        int childPosition = i - 1;
                        View view = linearParent.GetChildAt(childPosition);
                        var btnAccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                        var btnInaccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                        var txtComment = view.FindViewById<EditText>(Resource.Id.EditTokInfo_detailcomment);
                        var linearComment = view.FindViewById<LinearLayout>(Resource.Id.linearTokInfo_detailComment);
                        var btnTotalReaction = view.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                        var lblInaccurateClick = view.FindViewById<TextView>(Resource.Id.lbl_inaccurate_detail);
                        linearComment.Visibility = ViewStates.Gone;

                        txtComment.Text = item.Text;
                        var reactionQueryValues = new ReactionQueryValues();
                        reactionQueryValues.item_id = tokModel.Id;
                        reactionQueryValues.detail_number = i;
                        reactionQueryValues.kind = kind;
                        reactionQueryValues.pagination_id = "";

                        var result = await ReactionService.Instance.GetReactionsUsersAsync(reactionQueryValues);

                        for (int zz = 0; zz < linearParent.ChildCount; zz++)
                        {
                            View viewLinear = linearParent.GetChildAt(zz);
                            var circleprogressReaction = viewLinear.FindViewById<ProgressBar>(Resource.Id.circleprogressReaction);
                            //Hide ProgressBar
                            circleprogressReaction.Visibility = ViewStates.Gone;
                        }

                        if (kind == "accurate")
                        {
                            reactionValueVM.Accurate = reactionValueVM.Accurate + 1;
                            btnAccurate.SetBackgroundColor(Color.YellowGreen);
                            btnTotalReaction.Text = "1"; //((long)result.Count * 5).ToKMB();
                        }
                        else if (kind == "inaccurate")
                        {
                            isInaccurateAdded = true;
                            btnInaccurate.SetBackgroundColor(Color.Red);
                            btnInaccurate.SetTextColor(Color.White);
                            lblInaccurateClick.Visibility = ViewStates.Visible;
                            lblInaccurateClick.Click += delegate {
                                GoToInaccurateRepliesPage(TokInfoVm.InaccuratesCollection[0], 0);
                            };
                            btnTotalReaction.Text = "1"; //Math.Abs((long)result.Count * -10).ToKMB();
                            reactionValueVM.Inaccurate = reactionValueVM.Inaccurate + 1;
                        }

                        btnAccurate.Click += async (sender, e) =>
                        {
                            reactionValueVM.Accurate = reactionValueVM.Accurate + 1;
                            await OnClickAddReaction(btnAccurate, childPosition);
                        };

                        btnTotalReaction.Click += async (sender, e) =>
                        {

                            var reactionDetail = new ReactionValueViewModel();
                            if (kind == "accurate")
                                reactionDetail.Accurate = result.Count;
                            else if (kind == "inaccurate")
                                reactionDetail.Inaccurate = result.Count;
                            var modelConvert = JsonConvert.SerializeObject(reactionDetail);
                            nextActivity = new Intent(this, typeof(ReactionValuesActivity));
                            nextActivity.PutExtra("reactionValueVM", modelConvert);
                            nextActivity.PutExtra("tokId", tokModel.Id);
                            nextActivity.PutExtra("isDetailed", "true");
                            nextActivity.PutExtra("detailNumber", i.ToString());
                            StartActivity(nextActivity);
                        };
                    }
                }
            }

            showTotalReactions();
        }

        private void DisplaySelectedGem(string kind, TokkepediaReaction item)
        {
            //#B6B6B6 Gray
            if (kind == "gema")
            {
                //reactionValueVM.GemA = reactionValueVM.GemA + 1;
                GreenGemContainer.TooltipText = "selected";
                GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                ReactionImgGreen.SetBackgroundColor(Color.YellowGreen);
                GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgYellow.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                RedGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgRed.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                RedGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                YellowGemContainer.Enabled = false;
                RedGemContainer.Enabled = false;
                HeartReaction.SetImageResource(Resource.Drawable.heart_filled_v2);
            }
            else if (kind == "gemb")
            {
                //reactionValueVM.GemB = reactionValueVM.GemB + 1;
                YellowGemContainer.TooltipText = "selected";
                YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                ReactionImgYellow.SetBackgroundColor(Color.YellowGreen);
                YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgGreen.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                RedGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgRed.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                RedGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                GreenGemContainer.Enabled = false;
                RedGemContainer.Enabled = false;
            }
            else if (kind == "gemc")
            {
                //reactionValueVM.GemC = reactionValueVM.GemC + 1;
                RedGemContainer.TooltipText = "selected";
                RedGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                ReactionImgRed.SetBackgroundColor(Color.YellowGreen);
                RedGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgGreen.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                ReactionImgYellow.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                GreenGemContainer.Enabled = false;
                YellowGemContainer.Enabled = false;
                ImgPurpleGem.Alpha = 0.5f;
            }
            else if (kind == "accurate")
            {
                reactionValueVM.Accurate = reactionValueVM.Accurate + 1;
                BtnMegaAccurate.SetBackgroundColor(Color.YellowGreen);
                NestedComment.Visibility = ViewStates.Gone;
            }
            else if (kind == "inaccurate")
            {
                isInaccurateAdded = true;
                EditInaccurateComment.Text = item.Text;
                BtnMegaInaccurate.SetBackgroundColor(Color.Red);
                BtnMegaInaccurate.SetTextColor(Color.White);
                reactionValueVM.Inaccurate = reactionValueVM.Inaccurate + 1;
            }
        }
        private void updateReactionBackground(string kind)
        {
            if (kind == "gema")
            {
                //HeartReaction.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.GREY);
                //HeartReaction.Alpha = 0.5f;
                //HeartReaction.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.light_grey)));
                //HeartReaction.Enabled = false;
                HeartReaction.SetImageResource(Resource.Drawable.heart_filled_v2);
            }
            else if (kind == "gemb")
            {
                //HundredReaction.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.GREY);
                HundredReaction.Alpha = 0.9f;
                HundredReaction.ImageTintList = AppCompatResources.GetColorStateList(this, Resource.Color.red_500);
            }
            else if (kind == "gemc")
            {
                //ImgPurpleGem.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.GREY);
                ImgPurpleGem.Alpha = 0.5f;
                ImgPurpleGem.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.light_grey)));
            }
        }

        private void showTotalReactions()
        {
            TotalGreen.Text = (reactionValueVM.GemA * 5).ToKMB();
            TotalYellow.Text = (reactionValueVM.GemB * 10).ToKMB();
            TotalRed.Text = (reactionValueVM.GemC * 15).ToKMB();
            TotalAccurate.Text = (reactionValueVM.Accurate * 5).ToKMB();
            TotalInaccurate.Text = Math.Abs(reactionValueVM.Inaccurate * -10).ToKMB();
            //OverallTotalReactions.Text = ((reactionValueVM.GemA * 5) + (reactionValueVM.GemB * 10) + (reactionValueVM.GemC * 15) + (reactionValueVM.Accurate * 5) + (reactionValueVM.Inaccurate * -10)).ToKMB();
            OverallTotalReactions.Text = (reactionValueVM.GemA + reactionValueVM.GemB  + reactionValueVM.GemC + reactionValueVM.Accurate + reactionValueVM.Inaccurate).ToKMB();
            
            if (double.Parse(OverallTotalReactions.Text) < 0) {
                OverallTotalReactions.Text = "0";
            }

            OverallTotalReactionsDisplay.Text = OverallTotalReactions.Text;
            btnTotalReactions.Text = OverallTotalReactions.Text;
        }
        private void ShowGemsCollClicked(object sender, EventArgs e)
        {
            if (tokModel.IsMegaTok == true || tokModel.TokGroup.ToLower() == "mega" || tokModel.TokGroup.ToLower() == "q&a")
            {
            }
            else
            {
                for (int i = 0; i < linearParent.ChildCount; i++)
                {
                    View childview = linearParent.GetChildAt(i);
                    var LinearComment = childview.FindViewById<LinearLayout>(Resource.Id.linearTokInfo_detailComment);
                    if (LinearComment.Visibility == ViewStates.Visible)
                    {
                        LinearComment.Visibility = ViewStates.Gone;
                    }
                }
            }
            GemsParentContainer.Visibility = ViewStates.Visible;
            PurpleGemContainer.Visibility = ViewStates.Visible;
        }
        [Java.Interop.Export("OnClickParentToCancel")]
        public void OnClickParentToCancel(View v)
        {
            PurpleGemContainer.Visibility = ViewStates.Gone;
            GemsParentContainer.Visibility = ViewStates.Gone;
            FrameViews.Visibility = ViewStates.Gone;
            constraintMegaInaccurateComment.Visibility = ViewStates.Gone;
            NestedComment.Visibility = ViewStates.Visible;

            //When tap outside NestedComment
            BtnSmile.SetImageResource(Resource.Drawable.TOKKET_smiley_1B);
            RecyclerTokMojis.Visibility = ViewStates.Gone;
            hideKeyboard(v);
        }
        [Java.Interop.Export("OnClickPopUpMenuComments")]
        public void OnClickPopUpMenuComments(View v)
        {
            int position = (int)v.Tag;
            string message = "";
            var menu = new PopupMenu(this, v);
            // Call inflate directly on the menu:
            menu.Inflate(Resource.Menu.comment_popmenu);
            var report = menu.Menu.FindItem(Resource.Id.itemReport);
            var edit = menu.Menu.FindItem(Resource.Id.itemEdit);
            var delete = menu.Menu.FindItem(Resource.Id.itemDelete);

            if (Settings.GetTokketUser().Id == TokInfoVm.CommentsCollection[position].UserId)
            {
                edit.SetVisible(true);
                delete.SetVisible(true);
                report.SetVisible(false);
            }
            else
            {
                edit.SetVisible(false);
                delete.SetVisible(false);
                report.SetVisible(true);
            }

            // A menu item was clicked:
            menu.MenuItemClick += async (s1, arg1) => {
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "report":
                        var builder = new Dialog(this);
                        builder.SetContentView(Resource.Layout.report_selection_view);
                        builder.Show();
                        break;
                    case "edit":
                        NestedComment.Visibility = ViewStates.Gone;
                        View view = RecyclerComments.GetChildAt(position);
                        var linearReplyButton = view.FindViewById<LinearLayout>(Resource.Id.linearReplyButton);
                        var LinearEditComment = view.FindViewById<LinearLayout>(Resource.Id.LinearEditComment);
                        var EditCommentText = view.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
                        var CommentTextEllipsize = view.FindViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
                        var CommentText = view.FindViewById<TextView>(Resource.Id.lblCommentRowContent);

                        LinearEditComment.Visibility = ViewStates.Visible;
                        CommentTextEllipsize.Visibility = ViewStates.Gone;
                        CommentText.Visibility = ViewStates.Gone;
                        linearReplyButton.Visibility = ViewStates.Gone;

                        v.Visibility = ViewStates.Gone; //Hide when editing a comment
                        break;
                    case "delete":
                        linearProgress.Visibility = ViewStates.Visible;
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                        txtProgressText.Text = "Deleting a comment...";
                        var result = await ReactionService.Instance.DeleteReaction(TokInfoVm.CommentsCollection[position].Id);
                        linearProgress.Visibility = ViewStates.Gone;
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        txtProgressText.Text = "Loading...";

                        if (result)
                        {
                            message = "Comment deleted.";
                            TokInfoVm.CommentsCollection.RemoveAt(position);

                            adapterComments = TokInfoVm.CommentsCollection.GetRecyclerAdapter(BindCommentsViewHolder, Resource.Layout.tokinfo_comments_row);
                            RecyclerComments.SetAdapter(adapterComments);

                            TextTotalComments.Text = "Comments: " + TokInfoVm.CommentsCollection.Count.ToString();
                            updateCacheComments();
                        }
                        else
                        {
                            message = "Could not delete comment";
                        }

                        alertMessage("", message, Resource.Drawable.alert_icon_blue);
                        break;
                }
            };

            // Menu was dismissed:
            menu.DismissEvent += (s2, arg2) => {
                //Console.WriteLine("menu dismissed");
            };

            menu.Show();
        }

        private void updateCacheComments()
        {
            var data = new ResultData<ReactionModel>();
            data.ContinuationToken = RecyclerComments.ContentDescription;
            data.Results = TokInfoVm.CommentsCollection.ToList();
            ReactionService.Instance.SetReactionsCache(fromCallerComments, data);
        }
        
        [Java.Interop.Export("OnClickInaccurate")]
        public void OnClickInaccurate(View v)
        {
            int ndx = 0;
            try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }
           
            View view = linearParent.GetChildAt(ndx);

            for (int i = 0; i < linearParent.ChildCount; i++) {
                View childeview = linearParent.GetChildAt(i);
                var childlinearComment = childeview.FindViewById<LinearLayout>(Resource.Id.linearTokInfo_detailComment);
                childlinearComment.Visibility = ViewStates.Gone;
            }

            var linearComment = view.FindViewById<LinearLayout>(Resource.Id.linearTokInfo_detailComment);
           
            if (linearComment.Visibility == ViewStates.Visible)
            {
                linearComment.Visibility = ViewStates.Gone;
            }
            else
            {
              
                //tokinfo_btnInaccurate
                if (v.Background is ColorDrawable || view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate).Background is ColorDrawable)
                {
                    /*if (((ColorDrawable)v.Background).Color != Color.Red) //Check if reaction is already added based on the button background color
                    {
                        //linearComment.Visibility = ViewStates.Visible;
                    }*/
                }
                else
                {
                    
                    linearComment.Visibility = ViewStates.Visible;
                }
            }
        }
        [Java.Interop.Export("OnClickInaccurateMega")]
        public void OnClickInaccurateMega(View v)
        {
            int color = Color.YellowGreen;
            Drawable background = BtnMegaAccurate.Background;
            
            //color = ((ColorDrawable)background).getColor();

            if (background is RippleDrawable) //Proceed if no ColorDrawable has been set
            {
                if (constraintMegaInaccurateComment.Visibility == ViewStates.Visible)
                {
                    constraintMegaInaccurateComment.Visibility = ViewStates.Gone;
                    NestedComment.Visibility = ViewStates.Visible;
                }
                else
                {
                    constraintMegaInaccurateComment.Visibility = ViewStates.Visible;
                    GemsParentContainer.Visibility = ViewStates.Visible;
                    var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                    EditInaccurateComment.RequestFocus();
                    inputManager.ShowSoftInput(EditInaccurateComment, 0);
                    NestedComment.Visibility = ViewStates.Gone;
                }
            }
        }

        [Java.Interop.Export("OnClickTokInfoCommenter")]
        public void OnClickTokInfoCommenter(View v)
        {
            string commentorid = v.ContentDescription;
            nextActivity = new Intent(this, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", commentorid);
            this.StartActivity(nextActivity);
        }

        private void showToastMessage(string icon, string message)
        {
            // inflate layout file in Layout Inflater
            View viewCustomToast = LayoutInflater.Inflate(Resource.Layout.layout_toast_message_with_icon, null);
            var txtToastIcon = viewCustomToast.FindViewById<TextView>(Resource.Id.txtIcon);
            var txtToastMessage = viewCustomToast.FindViewById<TextView>(Resource.Id.txtMessage);
            txtToastIcon.Typeface = font;

            Toast toast = new Toast(this);
            // showing toast on bottom
            toast.SetGravity(GravityFlags.Bottom, 0, 100);
            toast.Duration = ToastLength.Long;

            txtToastIcon.Text = icon;

            if (icon == "times")
            {
                txtToastIcon.SetTextColor(Color.Red);
            }
            else
            {
                txtToastIcon.SetTextColor(Color.Green);
            }
            txtToastMessage.Text = message;
            toast.View = viewCustomToast;
            // show toast
            toast.Show();
        }
        private async Task OnClickAddReaction(View v, int detailPosition = -1)
        {
            try
            {
                View detailView = linearParent.GetChildAt(0);
                if (detailPosition >= 0)
                {
                    detailView = linearParent.GetChildAt(detailPosition);
                }

                string message = "", titlemssg = "", gemValues = "", comment = "";
                decimal gemcost = 0; bool isAddReaction = true;

                var kind = v.ContentDescription.ToString();

                ReactionModel tokkepediaReaction = new ReactionModel();
                int ndx = 0;

                if (kind.ToLower() == "like")
                {
                    tokkepediaReaction = reactionUser;
                }
                else
                {
                    try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }

                    tokkepediaReaction.ItemId = tokModel.Id;
                    tokkepediaReaction.Kind = kind.ToLower();
                    tokkepediaReaction.Label = "reaction";
                    tokkepediaReaction.DetailNum = ndx;
                    tokkepediaReaction.CategoryId = tokModel.CategoryId;
                    tokkepediaReaction.TokTypeId = tokModel.TokTypeId;
                    tokkepediaReaction.OwnerId = tokModel.UserId;
                    tokkepediaReaction.PartitionKey = tokkepediaReaction.ItemId + "-reactions0";
                    //tokkepediaReaction.PartitionKey = tokModel.PartitionKey;
                    tokkepediaReaction.UserId = Settings.GetTokketUser().Id;

                    //#B6B6B6 Gray
                    if (kind == "gema")
                    {
                        gemValues = "Valuable";
                        gemcost = 0;

                        GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                        ReactionImgGreen.SetBackgroundColor(Color.YellowGreen);
                        GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                        YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgYellow.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        RedGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgRed.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        RedGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        YellowGemContainer.Enabled = false;
                        RedGemContainer.Enabled = false;
                        GreenGemContainer.TooltipText = "selected";

                        var drawable = HeartReaction.Drawable;
                        if (drawable.GetConstantState() == ContextCompat.GetDrawable(this, Resource.Drawable.heart_filled_v2).GetConstantState())
                        {
                            reactionValueVM.GemA = reactionValueVM.GemA - 1;
                            reactionValueVM.All = reactionValueVM.All - 1;
                            HeartReaction.SetImageResource(Resource.Drawable.heart_outline_v2);
                        }
                        else
                        {
                            reactionValueVM.GemA = reactionValueVM.GemA + 1;
                            reactionValueVM.All = reactionValueVM.All + 1;
                            HeartReaction.SetImageResource(Resource.Drawable.heart_filled_v2);
                        }
                    }
                    else if (kind == "gemb")
                    {
                        gemValues = "Brilliant";

                        YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                        ReactionImgYellow.SetBackgroundColor(Color.YellowGreen);
                        YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                        GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgGreen.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        RedGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgRed.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        RedGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        GreenGemContainer.Enabled = false;
                        RedGemContainer.Enabled = false;
                        YellowGemContainer.TooltipText = "selected";

                        if (HundredReaction.Alpha == 0.9)
                        {
                            HundredReaction.Alpha = 1f; //Full Visibility

                            reactionValueVM.GemB = reactionValueVM.GemB - 1;
                            reactionValueVM.All = reactionValueVM.All - 1;
                            gemcost = 0;
                            HundredReaction.ImageTintList = AppCompatResources.GetColorStateList(this, Resource.Color.BLACK);
                        }
                        else
                        {
                            HundredReaction.Alpha = 0.9f;

                            reactionValueVM.GemB = reactionValueVM.GemB + 1;
                            reactionValueVM.All = reactionValueVM.All + 1;
                            gemcost = 5;
                            HundredReaction.ImageTintList = AppCompatResources.GetColorStateList(this, Resource.Color.red_500);
                        }

                    }
                    else if (kind == "gemc")
                    {
                        gemValues = "Precious";

                        RedGemHeader.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);
                        ReactionImgRed.SetBackgroundColor(Color.YellowGreen);
                        RedGemFooter.Background.SetColorFilter(Color.ParseColor("#9acd32"), PorterDuff.Mode.SrcAtop);

                        GreenGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgGreen.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        GreenGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        YellowGemHeader.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);
                        ReactionImgYellow.SetBackgroundColor(Color.ParseColor("#B6B6B6"));
                        YellowGemFooter.Background.SetColorFilter(Color.ParseColor("#B6B6B6"), PorterDuff.Mode.SrcAtop);

                        GreenGemContainer.Enabled = false;
                        YellowGemContainer.Enabled = false;
                        RedGemContainer.TooltipText = "selected";

                        if (ImgPurpleGem.Alpha == 0.5)
                        {
                            ImgPurpleGem.Alpha = 1f; //Full Visibility
                            reactionValueVM.GemC = reactionValueVM.GemC - 1;
                            reactionValueVM.All = reactionValueVM.All - 1;
                            gemcost = 0;
                        }
                        else
                        {
                            ImgPurpleGem.Alpha = 0.5f;
                            reactionValueVM.GemC = reactionValueVM.GemC + 1;
                            reactionValueVM.All = reactionValueVM.All + 1;
                            gemcost = 10;
                        }
                    }
                    else if (kind == "comment")
                    {
                        txtProgressText.Text = "Adding a comment...";
                        comment = CommentEditor.Text;
                        CommentEditor.Text = "";

                        tokkepediaReaction.Text = comment; //Comment
                        tokkepediaReaction.UserDisplayName = Settings.GetUserModel().DisplayName;
                        tokkepediaReaction.UserPhoto = Settings.GetTokketUser().UserPhoto;
                        tokkepediaReaction.Timestamp = DateTime.Now;
                        tokkepediaReaction.IsComment = true;

                        ////Comment below because it causes to duplicate in display
                        ////Show temporarily
                        //TokInfoVm.CommentsCollection.Insert(0, tokkepediaReaction);
                        //adapterComments = TokInfoVm.CommentsCollection.GetRecyclerAdapter(BindCommentsViewHolder, Resource.Layout.tokinfo_comments_row);
                        //RecyclerComments.SetAdapter(adapterComments);
                    }
                    else
                    {
                        bool isMarkReaction = true;
                        if (ndx == 0) //Mega
                        {
                            isAddReaction = !isInaccurateAdded; //Unable to add reaction when inaccurate = true;
                            if (isInaccurateAdded == false)
                            {
                                comment = EditInaccurateComment.Text;
                                tokkepediaReaction.Text = EditInaccurateComment.Text;
                                constraintMegaInaccurateComment.Visibility = ViewStates.Gone;

                                if (kind == "accurate")
                                {
                                    /*BtnMegaAccurate.SetBackgroundColor(Color.YellowGreen);*/ //Only Show when successful
                                    NestedComment.Visibility = ViewStates.Gone;
                                }
                                else if (kind == "inaccurate")
                                {
                                    //Only Show when successful
                                    /*BtnInaccurateComment.SetBackgroundColor(Color.Red);
                                    BtnInaccurateComment.SetTextColor(Color.White);*/
                                }
                            }
                            else
                            {
                                isMarkReaction = false;

                                showToastMessage("times", "Inaccurate has already been given.");
                            }
                        }
                        else //detail
                        {
                            int childPosition = ndx - 1;
                            View view = linearParent.GetChildAt(childPosition);
                            var btnAccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                            var btnInaccurate = view.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                            var btnTotalReaction = view.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);
                            var txtComment = view.FindViewById<EditText>(Resource.Id.EditTokInfo_detailcomment);
                            var linearComment = view.FindViewById<LinearLayout>(Resource.Id.linearTokInfo_detailComment);
                            detailView = view;

                            linearComment.Visibility = ViewStates.Gone;

                            comment = txtComment.Text;
                            tokkepediaReaction.Text = txtComment.Text;

                            if (kind == "accurate")
                            {
                                if (btnAccurate.Background is ColorDrawable || btnInaccurate.Background is ColorDrawable)
                                {
                                    //if background have already a color, disregard
                                    isMarkReaction = false;
                                    isAddReaction = false;
                                }
                                /* else
                                 {
                                     btnAccurate.SetBackgroundColor(Color.YellowGreen);
                                     btnTotalReaction.Text = (long.Parse(btnTotalReaction.Text) + 5).ToKMB();
                                 }*/
                            }
                            /*else if (kind == "inaccurate")
                            {
                                btnInaccurate.SetBackgroundColor(Color.Red);
                                btnInaccurate.SetTextColor(Color.White);
                                btnTotalReaction.Text = Math.Abs(long.Parse(btnTotalReaction.Text) -10).ToKMB();
                            }*/

                            btnTotalReaction.Text = "1";
                        }

                        if (isMarkReaction)
                        {
                            if (kind == "accurate")
                            {
                                //No need to add this code since this should be applied if reaction is success
                                /* reactionValueVM.Accurate = reactionValueVM.Accurate + 1;
                                 reactionValueVM.All = reactionValueVM.All + 1;*/
                            }
                            else if (kind == "inaccurate")
                            {
                                /* reactionValueVM.Inaccurate = reactionValueVM.Inaccurate + 1;
                                 reactionValueVM.All = reactionValueVM.All + 1;*/
                                isInaccurateAdded = true;
                                txtProgressText.Text = "Marking as inaccurate...";

                                //Hide Keyboard
                                hideKeyboard(BtnSmile);
                            }
                        }
                    }
                }


                //API

                if (!string.IsNullOrEmpty(gemValues))
                {
                    //Check if user have enough coins to purchase
                    TokketUser tokketUser = await SharedAccount.AccountService.Instance.GetUserAsync(Settings.GetUserModel().UserId);
                    Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);

                    if (tokketUser.Coins < gemcost)
                    {
                        //Show error message not enough coins
                        isAddReaction = false;

                        showToastMessage("times", "Not enough coins.");
                    }
                }

                if (isAddReaction)
                {
                    var result = await ReactionService.Instance.AddReaction(tokkepediaReaction);

                    if (!string.IsNullOrEmpty(gemValues))
                    {
                        if (result.ResultEnum == Shared.Helpers.Result.Success)
                        {
                            v.Enabled = false;
                        }
                    }

                    if (kind.ToLower() != "like")
                    {
                        if (result.ResultEnum == Shared.Helpers.Result.Failed)
                        {
                            try
                            { 
                                var resultDataO = result.ResultObject as ResultModel;
                                if (resultDataO == null)
                                {
                                    message = result.ResultMessage;
                                }
                                else
                                {
                                    message = resultDataO.ResultMessage;
                                }

                                //showToastMessage("times", message);
                            }
                            catch (Exception ex)
                            {
                            }

                            if (kind == "accurate" || kind == "inaccurate")
                            {
                                //TODO reaction value should only be added when success
                                /*if (detailPosition >= 0) // If detail
                                {
                                    var btnAccurate = detailView.FindViewById<Button>(Resource.Id.tokinfo_btnAccurate);
                                    var btnInaccurate = detailView.FindViewById<Button>(Resource.Id.tokinfo_btnInaccurate);
                                    var btnTotalReaction = detailView.FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions_detailComment);

                                    btnTotalReaction.Text = "0"; //Revert back to 0 if failed
                                }
                                else
                                {
                                    //If main buttons
                                    if (kind == "accurate")
                                    {
                                        reactionValueVM.Accurate = reactionValueVM.Accurate - 1;
                                        BtnMegaAccurate.SetBackgroundColor(Color.Transparent);
                                    }
                                    else if (kind == "inaccurate")
                                    {
                                        BtnMegaInaccurate.SetBackgroundColor(Color.Transparent);
                                        BtnMegaInaccurate.SetTextColor(Color.Red);
                                        reactionValueVM.Inaccurate = reactionValueVM.Inaccurate - 1;
                                    }
                                }*/
                            }
                            else if (kind == "gema")
                            {
                                reactionValueVM.GemA = reactionValueVM.GemA - 1;
                                reactionValueVM.All = reactionValueVM.All - 1;
                                HeartReaction.SetImageResource(Resource.Drawable.heart_outline_v2);
                            }
                            else if (kind == "gemb")
                            {
                                reactionValueVM.GemB = reactionValueVM.GemB - 1;
                                reactionValueVM.All = reactionValueVM.All - 1;
                                HundredReaction.Alpha = 1f;
                                HundredReaction.ImageTintList = AppCompatResources.GetColorStateList(this, Resource.Color.BLACK);
                            }
                            else if (kind == "gemc")
                            {
                                reactionValueVM.GemC = reactionValueVM.GemC - 1;
                                reactionValueVM.All = reactionValueVM.All - 1;
                                ImgPurpleGem.Alpha = 1f;
                            }
                        }
                        else
                        {
                            if (result.ResultEnum == Shared.Helpers.Result.Failed)
                            {
                                showToastMessage("times", "Failed to like this comment.");

                                (v as Button).SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.placeholder_bg)));

                                var likeGroup = (ViewGroup)v.Parent;
                                var txtLikeCount = likeGroup.FindViewById<TextView>(Resource.Id.txtCommentHeartCount);
                                int heartCnt = int.Parse(txtLikeCount.Text);
                                txtLikeCount.Text = (heartCnt - 1).ToString();
                            }
                        }


                        if (v.TooltipText == "selected")
                        {
                            if (result.ResultEnum == Shared.Helpers.Result.Success)
                            {
                                message = "You have given a " + gemValues.ToLower() + " to this tok.";

                                //Gem Reaction is added. Changes have made in this tok
                                tokModel.HasGemReaction = true;
                                changesMade = true;
                            }
                            else
                            {
                                message = "Could not give gem.";
                                titlemssg = "Error!";

                                //If error, deduct the 
                                if (gemValues.ToLower() == "valuable")
                                {
                                    reactionValueVM.GemA = reactionValueVM.GemA - 1;
                                    reactionValueVM.All = reactionValueVM.All - 1;
                                }
                                else if (gemValues.ToLower() == "brilliant")
                                {
                                    reactionValueVM.GemB = reactionValueVM.GemB - 1;
                                    reactionValueVM.All = reactionValueVM.All + -1;
                                }
                                else if (gemValues.ToLower() == "precious")
                                {
                                    reactionValueVM.GemC = reactionValueVM.GemC - 1;
                                    reactionValueVM.All = reactionValueVM.All - 1;
                                }

                                showToastMessage("times", message);
                            }
                        }

                        //OverallTotalReactions.Text = reactionValueVM.All.ToKMB();
                        /* TotalGreen.Text = (reactionValueVM.GemA * 5).ToKMB();
                         TotalYellow.Text = (reactionValueVM.GemB * 10).ToKMB();
                         TotalRed.Text = (reactionValueVM.GemC * 15).ToKMB();
                         TotalAccurate.Text = (reactionValueVM.Accurate * 5).ToKMB();
                         TotalInaccurate.Text = Math.Abs((reactionValueVM.Inaccurate * -10)).ToKMB();

                         OverallTotalReactions.Text = ((reactionValueVM.GemA * 5) + (reactionValueVM.GemB * 10) + (reactionValueVM.GemC * 15) + (reactionValueVM.Accurate * 5) + (reactionValueVM.Inaccurate * -10)).ToKMB();
                         OverallTotalReactionsDisplay.Text = OverallTotalReactions.Text;*/

                        showTotalReactions();

                        if (kind == "comment" && ndx == 0) //Main Comment
                        {
                            if (result.ResultEnum == Shared.Helpers.Result.Success)
                            {
                                message = "Comment added.";
                                this.RunOnUiThread(async () => await LoadComments("", true));
                            }
                            else
                            {
                                message = "An error occurred. Please refresh the comment section.";
                            }

                            showToastMessage("times", message);
                        }

                        if (result.ResultEnum == Shared.Helpers.Result.Success)
                        {
                            showToastMessage("check", "Success!");

                            if (kind.ToLower() == "like")
                            {
                            }
                            else
                            {
                                var serObject = JsonConvert.SerializeObject(result.ResultObject);
                                var resObject = JsonConvert.DeserializeObject<ResultModel>(serObject);
                                tokkepediaReaction.Id = resObject.ResultObject.ToString();

                                //Add or show the commenter
                                tokkepediaReaction.UserDisplayName = Settings.GetUserModel().DisplayName;
                                tokkepediaReaction.UserId = Settings.GetUserModel().UserId;
                                tokkepediaReaction.UserPhoto = Settings.GetUserModel().UserPhoto;

                                //TokInfoVm.CommentsCollection.Insert(0, tokkepediaReaction);
                                //adapterComments = TokInfoVm.CommentsCollection.GetRecyclerAdapter(BindCommentsViewHolder, Resource.Layout.tokinfo_comments_row);
                                //RecyclerComments.SetAdapter(adapterComments);

                                TextTotalComments.Text = "Comments: " + TokInfoVm.CommentsCollection.Count.ToString();
                            }
                        }
                    }
                }
            } catch (Exception ex) { }
        }
        private void hideKeyboard(View v)
        {
            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);
        }

        [Java.Interop.Export("OnClickTokInfoMegaEdit")]
        public void OnClickTokInfoMegaEdit(View v)
        {
            int ndx = (int)v.Tag;
            nextActivity = new Intent(this, typeof(AddSectionPage));
            nextActivity.PutExtra("SectionNo", ndx + 1);
            nextActivity.PutExtra("isAddSection", 1);
            nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
            nextActivity.PutExtra("tokSection", JsonConvert.SerializeObject(tokModel.Sections[ndx]));
            nextActivity.SetFlags(ActivityFlags.NewTask);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            StartActivityForResult(nextActivity, (int)ActivityType.AddSectionPage);
        }

        [Java.Interop.Export("OnClickTokInfoMegaView")]
        public void OnClickTokInfoMegaView(View v)
        {
            int ndx = (int)v.Tag;
            nextActivity = new Intent(this, typeof(AddSectionPage));
            nextActivity.PutExtra("SectionNo", ndx + 1);
            nextActivity.PutExtra("isAddSection", 2);
            nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
            nextActivity.PutExtra("tokSection", JsonConvert.SerializeObject(tokModel.Sections[ndx]));
            nextActivity.SetFlags(ActivityFlags.NewTask);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            StartActivity(nextActivity);
        }

        [Java.Interop.Export("OnClickTokInfoMegaRemove")]
        public void OnClickTokInfoMegaRemove(View v)
        {
            int ndx = (int)v.Tag;

            AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("Confirm");
            alertDiag.SetMessage("Are you sure you want to delete this tok?");
            alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {
                string message = "";
                txtProgressText.Text = "Deleting...";
                linearProgress.Visibility = ViewStates.Visible;
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                var issuccess = await TokService.Instance.DeleteTokSectionAsync(tokModel.Sections[ndx]); //API
                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                linearProgress.Visibility = ViewStates.Gone;

                if (issuccess)
                {
                    message = "Deleted the section successfully!";
                }
                else
                {
                    message = "Failed to delete section!";
                }

                var detail = tokModel.Sections.FirstOrDefault(a => a.Id == tokModel.Sections[ndx].Id);
                if (detail != null)
                {
                    var dialogDelete = new AlertDialog.Builder(this);
                    var alertDelete = dialogDelete.Create();
                    alertDelete.SetTitle("");
                    alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertDelete.SetMessage(message);
                    alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                    {
                        var sectionList = tokModel.Sections.ToList();
                        sectionList.Remove(tokModel.Sections[ndx]);
                        tokModel.Sections = sectionList.ToArray();
                        MegaTokSections();
                    });
                    alertDelete.Show();
                    alertDelete.SetCanceledOnTouchOutside(false);
                }
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }
        [Java.Interop.Export("OnClickTokBack")]
        public void OnClickTokBack(View v)
        {
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            Intent nextActivity = new Intent(this, typeof(TokBackActivity));
            nextActivity.PutExtra("tokModel", modelConvert);
            MainActivity.Instance.StartActivity(nextActivity);
        }
        [Java.Interop.Export("OnClickCancelComment")]
        public void OnClickCancelComment(View v)
        {
            int position = (int)v.Tag;
            View view = RecyclerComments.GetChildAt(position);
            var linearReplyButton = view.FindViewById<LinearLayout>(Resource.Id.linearReplyButton);
            var LinearEditComment = view.FindViewById<LinearLayout>(Resource.Id.LinearEditComment);
            var EditCommentText = view.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
            var BtnCancelComment = view.FindViewById<Button>(Resource.Id.BtnCancelComment);
            var BtnUpdateComment = view.FindViewById<Button>(Resource.Id.BtnUpdateComment);
            var CommentTextEllipsize = view.FindViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
            var CommentText = view.FindViewById<TextView>(Resource.Id.lblCommentRowContent);

            LinearEditComment.Visibility = ViewStates.Gone;
            CommentTextEllipsize.Visibility = ViewStates.Visible;
            CommentText.Visibility = ViewStates.Gone;
            linearReplyButton.Visibility = ViewStates.Visible;
            NestedComment.Visibility = ViewStates.Visible;

            view.FindViewById<TextView>(Resource.Id.lblCommentPopUpMenu).Visibility = ViewStates.Visible;
        }
        [Java.Interop.Export("OnClickUpdateComment")]
        public async void OnClickUpdateComment(View v)
        {
            int position = (int)v.Tag;
            View view = RecyclerComments.GetChildAt(position);
            var linearReplyButton = view.FindViewById<LinearLayout>(Resource.Id.linearReplyButton);
            var LinearEditComment = view.FindViewById<LinearLayout>(Resource.Id.LinearEditComment);
            var EditCommentText = view.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
            var BtnCancelComment = view.FindViewById<Button>(Resource.Id.BtnCancelComment);
            var BtnUpdateComment = view.FindViewById<Button>(Resource.Id.BtnUpdateComment);
            var CommentTextEllipsize = view.FindViewById<TextView>(Resource.Id.lblCommentRowContentEllip);
            var CommentText = view.FindViewById<TextView>(Resource.Id.lblCommentRowContent);

            view.FindViewById<TextView>(Resource.Id.lblCommentPopUpMenu).Visibility = ViewStates.Visible;

            txtProgressText.Text = "Updating...";
            linearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

            ReactionModel updateReaction = TokInfoVm.CommentsCollection[position];
            updateReaction = TokInfoVm.CommentsCollection[position];
            updateReaction.Text = EditCommentText.Text;
            if (updateReaction._etag == null)
            {
                //400 or bad request is returned when newly added comment and comment is edited, this makes an invalid partitionkey. so set below
                updateReaction.PartitionKey = updateReaction.ItemId + "-reactions0";
            }
            var ResultUpdate = await ReactionService.Instance.UpdateReaction(updateReaction);

            linearProgress.Visibility = ViewStates.Gone;
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

            if (ResultUpdate)
            {
                TokInfoVm.CommentsCollection.Remove(TokInfoVm.CommentsCollection[position]);
                TokInfoVm.CommentsCollection.Insert(position, updateReaction);
                CommentTextEllipsize.Text = EditCommentText.Text;
                CommentText.Text = EditCommentText.Text;
                linearReplyButton.Visibility = ViewStates.Visible;
                LinearEditComment.Visibility = ViewStates.Gone;
                CommentTextEllipsize.Visibility = ViewStates.Visible;
                NestedComment.Visibility = ViewStates.Visible;

                //Cache updated comments
                updateCacheComments();
            }
            else
            {
                showToastMessage("times", "Failed updating the comment.");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.tokinfo_menu, menu);

            MenuCompat.SetGroupDividerEnabled(menu, true);
            var tokinfo_addtoktoset = menu.FindItem(Resource.Id.tokinfo_addtoktoset);
            var tokinfo_addtilesticker = menu.FindItem(Resource.Id.tokinfo_addtilesticker);
            var tokinfo_addsection = menu.FindItem(Resource.Id.tokinfomenu_addsection);
            var tokinfo_share = menu.FindItem(Resource.Id.tokinfo_share);
            var tokinfo_replicate = menu.FindItem(Resource.Id.tokinfo_replicate);
            var tokinfo_edit = menu.FindItem(Resource.Id.tokinfo_edit);
            var tokinfo_delete = menu.FindItem(Resource.Id.tokinfo_delete);
            var tokinfo_report = menu.FindItem(Resource.Id.tokinfo_report);
            var tokinfo_removefromgroup = menu.FindItem(Resource.Id.tokinfo_removefromgroup);
            var tokinfo_shareFB = menu.FindItem(Resource.Id.tokinfo_shareFB);
            var tokinfo_shareTwitter = menu.FindItem(Resource.Id.tokinfo_shareTwitter);
            var tokinfo_shareToPublic = menu.FindItem(Resource.Id.tokinfo_sharetoPublic);
            var tokinfo_shareToPrivate = menu.FindItem(Resource.Id.tokinfo_sharetoPrivate);
            var tokinfo_shareToGroup = menu.FindItem(Resource.Id.publicToGroup);
            var tokinfo_grouptToGroup = menu.FindItem(Resource.Id.groupToGroup);
            bool isPublicFeed = false;
            if (Settings.FilterFeed == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            if (!isPublicFeed)
            {
                tokinfo_shareToPublic.SetVisible(true);
                tokinfo_shareToPrivate.SetVisible(false);
                tokinfo_shareToGroup.SetVisible(false);
                tokinfo_grouptToGroup.SetVisible(false);
            }
            else {
                tokinfo_shareToPublic.SetVisible(false);
                tokinfo_shareToPrivate.SetVisible(true);
                tokinfo_shareToGroup.SetVisible(true);
                tokinfo_grouptToGroup.SetVisible(false);
            }

            if (classGroupModel!= null) {
                tokinfo_shareToPublic.SetVisible(false);
                tokinfo_shareToPrivate.SetVisible(false);
                tokinfo_shareToGroup.SetVisible(false);
                tokinfo_grouptToGroup.SetVisible(true);
            }

            tokinfo_removefromgroup.SetVisible(false);
            if (classTokModel != null)
            {
                if (!string.IsNullOrEmpty(classTokModel.GroupId) && tokModel.UserId == Settings.GetUserModel().UserId)
                {
                    tokinfo_removefromgroup.SetVisible(true);
                   
                }
            }
            tokinfo_shareFB.SetVisible(false);
            tokinfo_shareTwitter.SetVisible(false);
            if (tokModel.UserId == Settings.GetUserModel().UserId)
            {
                tokinfo_addtoktoset.SetVisible(true);
                tokinfo_addtilesticker.SetVisible(true);

                //if tok is mega, set visibility to true
                if (tokModel.IsMegaTok == true || tokModel.TokGroup.ToLower() == "mega") //If Mega
                {
                    tokinfo_addsection.SetVisible(true);
                }
                else
                {
                    tokinfo_addsection.SetVisible(false);
                }

                tokinfo_share.SetVisible(false);
                tokinfo_edit.SetVisible(true);
                tokinfo_replicate.SetVisible(true);
                tokinfo_delete.SetVisible(true);
                tokinfo_report.SetVisible(false);
            }
            else if (!string.IsNullOrEmpty(classTokModel.GroupId) && classTokModel.UserId == Settings.GetUserModel().UserId)
            {
                tokinfo_addtoktoset.SetVisible(true);
                tokinfo_addtilesticker.SetVisible(false);
                tokinfo_addsection.SetVisible(false);
                tokinfo_share.SetVisible(false);
                tokinfo_edit.SetVisible(false);
                tokinfo_replicate.SetVisible(false);
                tokinfo_delete.SetVisible(true);
                tokinfo_report.SetVisible(true);
            }
            else
            {
                tokinfo_addtoktoset.SetVisible(true);
                tokinfo_addtilesticker.SetVisible(false);
                tokinfo_addsection.SetVisible(false);
                tokinfo_share.SetVisible(false);
                tokinfo_edit.SetVisible(false);
                tokinfo_replicate.SetVisible(false);
                if (classGroupModel != null && classGroupModel.UserId == Settings.GetUserModel().UserId)
                {
                    tokinfo_removefromgroup.SetVisible(true);
                    tokinfo_delete.SetVisible(true);
                }
                else {
                    tokinfo_removefromgroup.SetVisible(false); 
                    tokinfo_delete.SetVisible(false);
                }
                  
                tokinfo_report.SetVisible(true);
            }

            return true;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        private void setResult()
        {
#if (_TOKKEPEDIA)
            Intent = new Intent();
            Intent.PutExtra("changesMade", changesMade);
            Intent.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
            SetResult(Android.App.Result.Ok, Intent);
            Finish();
#endif
#if (_CLASSTOKS)

            /* Intent = new Intent();
             Intent.PutExtra("changesMade", changesMade);
             Intent.PutExtra("classtokModel", JsonConvert.SerializeObject(classTokModel));
             SetResult(Android.App.Result.Ok, Intent);
             Finish();*/
#endif
        }
        public override void OnBackPressed()
        {
            setResult();
            if (constraintMegaInaccurateComment.Visibility == ViewStates.Visible) {
                constraintMegaInaccurateComment.Visibility = ViewStates.Gone;
                NestedComment.Visibility = ViewStates.Visible;

            }

            base.OnBackPressed();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    setResult();

                    Finish();
                    break;
                case Resource.Id.tokinfo_share:
                    RunOnUiThread(async () => await Share.RequestAsync(new ShareTextRequest
                    {
                        Uri = Shared.Config.Configurations.Url + "tok/" + item.ItemId,
                        Title = tokModel.PrimaryFieldName
                    }));

                    break;
                case Resource.Id.tokinfo_addtoktoset:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.TokInfo);
#if (_TOKKEPEDIA)
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
#endif
#if (_CLASSTOKS)
                    nextActivity = new Intent(this, typeof(AddToksToSetActivity));
                    nextActivity.PutExtra("classtokModel", JsonConvert.SerializeObject(classTokModel));
#endif
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", tokModel.TokTypeId);
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    StartActivity(nextActivity);
                    break;
                case Resource.Id.tokinfo_addtilesticker:
                    nextActivity = new Intent(this, typeof(AddStickerDialogActivity));
                    nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
                    // nextActivity.PutExtra("ClassTokModel", JsonConvert.SerializeObject(classTokModel));
                    StartActivityForResult(nextActivity, (int)ActivityType.AddStickerDialogActivity);
                    break;
                case Resource.Id.tokinfomenu_addsection:
                    nextActivity = new Intent(this, typeof(AddSectionPage));
                    nextActivity.PutExtra("SectionNo", linearParent.ChildCount + 1);
                    nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivityForResult(nextActivity, (int)ActivityType.AddSectionPage);
                    break;
                case Resource.Id.tokinfo_replicate:
                    ReplicateTok();
                    break;
                case Resource.Id.tokinfo_edit:
#if (_CLASSTOKS)
                    nextActivity = new Intent(this, typeof(AddClassTokActivity));
                    nextActivity.PutExtra("isSave", false);
                    nextActivity.PutExtra("ClassTokModel", JsonConvert.SerializeObject(classTokModel));
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivityForResult(nextActivity, (int)ActivityType.AddTokActivityType);
#endif
#if (_TOKKEPEDIA)
                    nextActivity = new Intent(this, typeof(AddTokActivity));
                    nextActivity.PutExtra("isAddTok", false);
                    nextActivity.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivityForResult(nextActivity, (int)ActivityType.AddTokActivityType);
#endif
                    break;
                case Resource.Id.tokinfo_delete:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.TokInfo);
                    AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
                    alertDiag.SetTitle("Confirm");
                    alertDiag.SetMessage("Are you sure you want to delete this tok?");
                    alertDiag.SetPositiveButton("Cancel", (senderAlert, args) =>
                    {
                        alertDiag.Dispose();
                    });
                    alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                    {
                        //Set Visibility of ProgressBar to Visible
                        txtProgressText.Text = "Deleting...";
                        linearProgress.Visibility = ViewStates.Visible;
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);

#if (_CLASSTOKS)
                        bool result = false;
                        if (ClassSet != null)
                        {
                            var newList = new List<string>() { classTokModel.Id };
                            result = await ClassService.Instance.DeleteClassToksFromClassSetAsync(ClassSet, newList);
                        }
                        else {
                             result = await ClassService.Instance.DeleteClassToksAsync(classTokModel.Id, classTokModel.PartitionKey);
                            //try
                            //{
                            //    var res = await Tokket.Shared.IoC.AppContainer.Resolve<Tokket.Shared.Services.Interfaces.IClassTokService>().DeleteClassTokAsync<ClassTokModel>(classTokModel.Id, classTokModel.PartitionKey);
                            //    if (res.StatusCode == HttpStatusCode.OK)
                            //        System.Console.WriteLine("Code worked!");
                            //}
                            //catch (Exception ex)
                            //{
                            //    System.Console.WriteLine("Code failed: " + ex.StackTrace);
                            //}
                        }

#endif

#if (_TOKKEPEDIA)
                        var result = await TokService.Instance.DeleteTokAsync(tokModel.Id);
#endif
                        //Set Visibility of ProgressBar to Gone
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        linearProgress.Visibility = ViewStates.Gone;

                        var builder = new AlertDialog.Builder(this);
                        builder.SetMessage("Deleted Successfully!");
                        builder.SetTitle("");
                        var dialog = (AlertDialog)null;
                        builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                        {
                            Intent = new Intent();
                            Intent.PutExtra("isDeleted", true);

#if (_CLASSTOKS)
                            Intent.PutExtra("classtokModel", JsonConvert.SerializeObject(classTokModel));
#endif

#if (_TOKKEPEDIA)
                    Intent.PutExtra("tokModel", JsonConvert.SerializeObject(tokModel));
#endif
                            SetResult(Result.Ok, Intent);
                            Finish();
                        });
                        dialog = builder.Create();
                        dialog.Show();
                        dialog.SetCanceledOnTouchOutside(false);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                    break;
                case Resource.Id.tokinfo_removefromgroup:
                    AlertDialog.Builder alertDiagcg = new AlertDialog.Builder(this);
                    alertDiagcg.SetTitle("Confirm");
                    alertDiagcg.SetMessage("Are you sure you want to remove this from a group?");
                    alertDiagcg.SetPositiveButton("Cancel", (senderAlert, args) =>
                    {
                        alertDiagcg.Dispose();
                    });
                    alertDiagcg.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Remove</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                    {
                        //Set Visibility of ProgressBar to Visible
                        txtProgressText.Text = "Removing...";
                        linearProgress.Visibility = ViewStates.Visible;
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                        classTokModel.GroupId = "";

                        var resultClassTokModel = await ClassService.Instance.UpdateClassToksAsync(classTokModel);

                        //Set Visibility of ProgressBar to Gone
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        linearProgress.Visibility = ViewStates.Gone;

                        var objBuilder = new AlertDialog.Builder(this);
                        objBuilder.SetTitle("");
                        objBuilder.SetMessage(resultClassTokModel.ResultEnum.ToString());
                        objBuilder.SetCancelable(false);

                        AlertDialog objDialog = objBuilder.Create();
                        objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                        {
                            if (resultClassTokModel.ResultEnum == Shared.Helpers.Result.Success)
                            {
                                ClassToksFragment.Instance.RemoveClassTokCollection(classTokModel);
                                Finish();
                            }
                        });
                        objDialog.Show();
                        objDialog.SetCanceledOnTouchOutside(false);
                    });
                    Dialog diagcg = alertDiagcg.Create();
                    diagcg.Show();
                    diagcg.SetCanceledOnTouchOutside(false);

                    break;

                case Resource.Id.tokinfo_report:
                    Report.Show();
                    break;
                case Resource.Id.tokinfo_shareFB:
                   
                    //FacebookModel.QouteUrl = $"https://tokket.com/classtoks/tok/{classTokModel.Id}/classtoks0";
                    //FacebookModel.QouteMessage = classTokModel.PrimaryFieldText;
                    //MainThread.BeginInvokeOnMainThread(async () => {
                    //    //await FacebookModel.LoginAsync();
                    //    await FacebookModel.ShareData();
                    //    });

                    break;
                case Resource.Id.tokinfo_shareTwitter:
                    //Intent tweet = new Intent(Android.Content.Intent.ActionView);
                    //// Android.Net.Uri.Parse(new Uri("http://twitter.com/?status=" + classTokModel.PrimaryFieldText)) 
                    //tweet.SetData(Android.Net.Uri.Parse("http://twitter.com/?status=" +Android.Net.Uri.Encode(classTokModel.PrimaryFieldText))); ;//where message is your string message
                    //StartActivity(tweet);

                    //Intent intentt = new Intent();
                    //intentt.SetAction(Android.Content.Intent.ActionSend);
                    //intentt.PutExtra(Android.Content.Intent.ExtraText, classTokModel.PrimaryFieldText);
                    //intentt.SetType("text/plain");
                    //intentt.PutExtra(Android.Content.Intent.ExtraText, $"http://tokket.com/classtoks/tok/{classTokModel.Id}/classtoks0");
                    //intentt.SetType("text/plain");
                    ////intent.putExtra(Intent.EXTRA_STREAM, uri);
                    ////intent.setType("image/jpeg");
                    //intentt.SetClassName("com.twitter.android", "com.twitter.applib.composer.TextFirstComposerActivity");
                    //StartActivity(intentt);
                    string url = $"\nhttps://tokket.com/classtoks/tok/{classTokModel.Id}/classtoks0";
                    string textEncoded = classTokModel.PrimaryFieldText.UrlEncode(System.Text.Encoding.UTF8);
                    string urlEncoded = url.UrlEncode(System.Text.Encoding.UTF8);
                    String tweetUrl = String.Format($"https://twitter.com/intent/tweet?text={textEncoded}&url={urlEncoded}");

                    Intent intents = new Intent(Intent.ActionView.ToString(), NetUri.Parse(tweetUrl));

                    var matches = PackageManager.QueryIntentActivities(intents,0);
                    foreach (var info in matches)
                    {
                        if (info.ActivityInfo.PackageName.ToLower().StartsWith("com.twitter"))
                        {
                            intents.SetPackage(info.ActivityInfo.PackageName);
                        }
                    }
                    StartActivity(intents);
                    break;

                case Resource.Id.tokinfo_sharetoPublic:
                    ShareTok(true);
                    break;

                case Resource.Id.tokinfo_sharetoPrivate:
                    ShareTok(false);
                    break;
                case Resource.Id.publicToGroup:
                    var intent = new Intent(this, typeof(ClassGroupListActivity));
                    intent.PutExtra("shareRequest","true");
                    StartActivityForResult(intent,REQUEST_GROUPSELECTION_REPLY);

                    break;
                case Resource.Id.groupToGroup:
                     intent = new Intent(this, typeof(ClassGroupListActivity));
                    intent.PutExtra("shareRequest", "true");
                    StartActivityForResult(intent, REQUEST_GROUPSELECTION_REPLY);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void InitShareButtons() {
            var pkCheck = classTokModel.PartitionKey.Contains(Settings.GetTokketUser().Id);
            bool pkCheckGroup = false;
            if (classGroupModel != null)
                pkCheckGroup = classGroupModel.PartitionKey.Contains(classGroupModel.Id);
            //if (pkCheck || pkCheckGroup)
            //{
            //    BtnShareFB.Visibility = ViewStates.Visible;
            //    BtnShareTwitter.Visibility = ViewStates.Visible;

            //}
            //else
            //{
            //    BtnShareFB.Visibility = ViewStates.Gone;
            //    BtnShareTwitter.Visibility = ViewStates.Gone;
            //}

            BtnShareFB.Click += delegate {
                //FacebookModel.QouteUrl = $"https://tokket.com/classtoks/tok/{classTokModel.Id}/classtoks0";
                //FacebookModel.QouteMessage = classTokModel.PrimaryFieldText;
                //MainThread.BeginInvokeOnMainThread(async () => {
                //    //await FacebookModel.LoginAsync();
                //    await FacebookModel.ShareData();
                //});
            };
            BtnShareTwitter.Click += delegate {


                string url = $"\nhttps://{Tokket.Shared.Config.Configurations.BaseUrl}/classtoks/tok/{classTokModel.Id}/{Settings.GetTokketUser().Id}classtoks0";
                string textEncoded = classTokModel.PrimaryFieldText.UrlEncode(System.Text.Encoding.UTF8);
                string urlEncoded = url.UrlEncode(System.Text.Encoding.UTF8);
                String tweetUrl = String.Format($"https://twitter.com/intent/tweet?text={textEncoded}&url={urlEncoded}");

                Intent intents = new Intent(Intent.ActionView.ToString(), NetUri.Parse(tweetUrl));

                var matches = PackageManager.QueryIntentActivities(intents, 0);
                foreach (var info in matches)
                {
                    if (info.ActivityInfo.PackageName.ToLower().StartsWith("com.twitter"))
                    {
                        intents.SetPackage(info.ActivityInfo.PackageName);
                    }
                }
                StartActivity(intents);
            };
        }
        private void ReplicateTok()
        {
            var dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.dialog_replicate_tok);
            var txtClassTokCategory = dialog.FindViewById<AppCompatEditText>(Resource.Id.txtClassTokCategory);
            var chkPrivate = dialog.FindViewById<CheckBox>(Resource.Id.chkPrivate);
            var chkPublic = dialog.FindViewById<CheckBox>(Resource.Id.chkPublic);
            var btnCancel = dialog.FindViewById<Button>(Resource.Id.btnCancel);
            var btnReplicate = dialog.FindViewById<Button>(Resource.Id.btnReplicate);

            btnCancel.Click += delegate { dialog.Dismiss(); };
            chkPrivate.Click += delegate { chkPublic.Checked = !chkPrivate.Checked; };
            chkPublic.Click += delegate { chkPrivate.Checked = !chkPublic.Checked; };

            btnReplicate.Click += async delegate {
                showBlueLoading(this);

                var replacer = new ClassTokModel();
                var copyClassTok = classTokModel;
                copyClassTok.Id = replacer.Id;
                copyClassTok.PrimaryFieldText = copyClassTok.PrimaryFieldText;
                copyClassTok.SecondaryFieldText = classTokModel.SecondaryFieldText;
                copyClassTok.UserDisplayName = Settings.GetTokketUser().DisplayName;
                copyClassTok.UserId = Settings.GetTokketUser().Id;
                copyClassTok.UserCountry = Settings.GetTokketUser().Country;
                copyClassTok.UserPhoto = Settings.GetTokketUser().UserPhoto;
                copyClassTok.IsGroup = classTokModel.IsGroup;
                copyClassTok.GroupId = classTokModel.GroupId;
                copyClassTok.SourceLink = classTokModel.SourceLink;
                copyClassTok.Level1 = classTokModel.Level1;
                copyClassTok.Level2 = classTokModel.Level2;
                copyClassTok.Level3 = classTokModel.Level3;
                copyClassTok.TokGroup = classTokModel.TokGroup;
                copyClassTok.TokType = classTokModel.TokType;
                copyClassTok.CategoryId = classTokModel.CategoryId;
                copyClassTok.TokTypeId = classTokModel.TokTypeId;
                copyClassTok.UserState = Settings.GetTokketUser().State;
                copyClassTok.IsIndent = classTokModel.IsIndent;
                copyClassTok.ImagesIsTokPakVisible = classTokModel.ImagesIsTokPakVisible;
                copyClassTok.Details = classTokModel.Details;
                copyClassTok.DetailImages = classTokModel.DetailImages;
                copyClassTok.CreatedTime = replacer.CreatedTime;
                copyClassTok.DateCreated = replacer.DateCreated;
                copyClassTok.Timestamp = replacer.Timestamp;
                copyClassTok.Category = txtClassTokCategory.Text;

                if (chkPublic.Checked)
                {
                    copyClassTok.IsPublic = true;
                    copyClassTok.IsPrivate = true;
                }
                else
                {
                    copyClassTok.IsPublic = false;
                    copyClassTok.IsPrivate = true;
                }

                if (classTokModel.IsMasterCopy)
                {
                    copyClassTok.HasComments = true;
                    copyClassTok.HasReactions = true;
                }

                var result = await ClassService.Instance.AddClassToksAsync(copyClassTok);

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    var serClassTokResult = JsonConvert.SerializeObject(result.ResultObject);
                    copyClassTok = JsonConvert.DeserializeObject<ClassTokModel>(serClassTokResult);

                    if ((classTokModel.IsMegaTok == true || classTokModel.TokGroup.ToLower() == "mega") || classTokModel.TokGroup.ToLower() == "q&a")
                    {
                        var doneSaving = false;
                        var cnt = 0;
                        var sections = classTokModel.Sections;
                        for (int i = 0; i < sections.Length; i++)
                        {
                            //Copy the 1st 10
                            if (i < 10)
                            {
                                var sec = sections[i];
                                sec.Id = null;
                                //Progress Text
                                Thread.Sleep(200);
                                double val1 = (double)(cnt + 1) / (double)sections.Length;
                                var val2 = val1 * 100;
                                int percent = (int)val2;
                                var progress = percent.ToString() + " %";

                                var dummySec = new TokSection();
                                sec.Id = dummySec.Id;
                                sec.TokId = copyClassTok.Id;
                                sec.TokTypeId = copyClassTok.TokTypeId;
                                sec.UserId = Settings.GetUserModel().UserId;

                                var isSuccess = await TokService.Instance.CreateTokSectionAsync(sec, copyClassTok.Id, 0);

                                if (sections.Length - 1 == cnt)
                                {
                                    doneSaving = true;
                                };

                                cnt += 1;
                            }
                            else
                            {
                                doneSaving = true;
                                break;
                            }
                        }

                        if (doneSaving)
                        {
                            hideBlueLoading(this);
                            showAlertDialog(this, "Copy succesful!", (obj, eve) => { Finish(); });
                        }
                    }
                    else
                    {
                        hideBlueLoading(this);
                        showAlertDialog(this, "Copy succesful!", (obj, eve) => { Finish(); });
                    }
                }
                else
                {
                    hideBlueLoading(this);
                    showAlertDialog(this, "Failed to copy tok.");
                }
            };

            dialog.Show();
        }
        private void ShareTok(bool topublic) {
            var dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.share_toks_popup);
            dialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            var primary = dialog.FindViewById<EditText>(Resource.Id.txt_primary);
            var btncancel = dialog.FindViewById<Button>(Resource.Id.btncancel);
            var btnclose = dialog.FindViewById<Button>(Resource.Id.btnclose);
            var btnshare = dialog.FindViewById<Button>(Resource.Id.btn_share);

            btnclose.Click += delegate { dialog.Dismiss(); };
            btncancel.Click += delegate { dialog.Dismiss(); };


            btnshare.Click += async delegate {
                linearProgress.Visibility = ViewStates.Visible;
                var replacer = new ClassTokModel();
                var sharedmodel = classTokModel;
                sharedmodel.TokSharePk = classTokModel.PartitionKey;
                sharedmodel.TokShare = classTokModel.Id;
                sharedmodel.Id = replacer.Id;
                sharedmodel.PrimaryFieldText = primary.Text;
                sharedmodel.SecondaryFieldText = classTokModel.SecondaryFieldText;
                sharedmodel.UserDisplayName = Settings.GetTokketUser().DisplayName;
                sharedmodel.UserId = Settings.GetTokketUser().Id;
                sharedmodel.UserCountry = Settings.GetTokketUser().Country;
                sharedmodel.UserPhoto = Settings.GetTokketUser().UserPhoto;
                sharedmodel.IsGroup = classTokModel.IsGroup;
                sharedmodel.GroupId = classTokModel.GroupId;
                sharedmodel.SourceLink = classTokModel.SourceLink;
                sharedmodel.Level1 = classTokModel.Level1;
                sharedmodel.Level2 = classTokModel.Level2;
                sharedmodel.Level3 = classTokModel.Level3;
                sharedmodel.TokGroup = classTokModel.TokGroup;
                sharedmodel.TokType = classTokModel.TokType;
                sharedmodel.CategoryId = classTokModel.CategoryId;
                sharedmodel.TokTypeId = classTokModel.TokTypeId;
                sharedmodel.UserState = Settings.GetTokketUser().State;
                sharedmodel.IsIndent = classTokModel.IsIndent;
                sharedmodel.ImagesIsTokPakVisible = classTokModel.ImagesIsTokPakVisible;
                sharedmodel.Details = classTokModel.Details;
                sharedmodel.DetailImages = classTokModel.DetailImages;
                sharedmodel.CreatedTime = replacer.CreatedTime;
                sharedmodel.DateCreated = replacer.DateCreated;
                sharedmodel.Timestamp = replacer.Timestamp;
                if (topublic)
                {
                    sharedmodel.IsPublic = true;
                    sharedmodel.IsPrivate = true;
                }
                else {
              
                    sharedmodel.IsPublic = false;
                    sharedmodel.IsPrivate = true;
                }
             
                var result = await ClassService.Instance.AddClassToksAsync(sharedmodel);
                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    showAlertDialog(this, "Share succesful!", (obj,eve)=> { Finish(); });
                  
                }
                else
                {
                    showAlertDialog(this, "Failed to share tok.");
                }
            };

            dialog.Show();
        }

        private void ShareTokGroup(ClassGroupModel group) {
            var dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.share_toks_popup);
            dialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            var primary = dialog.FindViewById<EditText>(Resource.Id.txt_primary);
            var btncancel = dialog.FindViewById<Button>(Resource.Id.btncancel);
            var btnclose = dialog.FindViewById<Button>(Resource.Id.btnclose);
            var btnshare = dialog.FindViewById<Button>(Resource.Id.btn_share);

            btnclose.Click += delegate { dialog.Dismiss(); };
            btncancel.Click += delegate { dialog.Dismiss(); };


            btnshare.Click += async delegate {
                linearProgress.Visibility = ViewStates.Visible;
                var replacer = new ClassTokModel();
                var sharedmodel = classTokModel;
                sharedmodel.TokSharePk = classTokModel.PartitionKey;
                sharedmodel.TokShare = classTokModel.Id;
                sharedmodel.Id = replacer.Id;
                sharedmodel.PrimaryFieldText = primary.Text;
                sharedmodel.SecondaryFieldText = classTokModel.SecondaryFieldText;
                sharedmodel.UserDisplayName = Settings.GetTokketUser().DisplayName;
                sharedmodel.UserId = Settings.GetTokketUser().Id;
                sharedmodel.UserCountry = Settings.GetTokketUser().Country;
                sharedmodel.UserPhoto = Settings.GetTokketUser().UserPhoto;
                sharedmodel.IsGroup = true;
                sharedmodel.IsPublic = false;
                sharedmodel.IsPrivate = false;
                sharedmodel.GroupId = group.Id;
                sharedmodel.SourceLink = classTokModel.SourceLink;
                sharedmodel.Level1 = classTokModel.Level1;
                sharedmodel.Level2 = classTokModel.Level2;
                sharedmodel.Level3 = classTokModel.Level3;
                sharedmodel.TokGroup = classTokModel.TokGroup;
                sharedmodel.TokType = classTokModel.TokType;
                sharedmodel.CategoryId = classTokModel.CategoryId;
                sharedmodel.TokTypeId = classTokModel.TokTypeId;
                sharedmodel.UserState = Settings.GetTokketUser().State;
                sharedmodel.IsIndent = classTokModel.IsIndent;
                sharedmodel.ImagesIsTokPakVisible = classTokModel.ImagesIsTokPakVisible;
                sharedmodel.Details = classTokModel.Details;
                sharedmodel.DetailImages = classTokModel.DetailImages;
                sharedmodel.CreatedTime = replacer.CreatedTime;
                sharedmodel.DateCreated = replacer.DateCreated;
                sharedmodel.Timestamp = replacer.Timestamp;
           
                var result = await ClassService.Instance.AddClassToksAsync(sharedmodel);
                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    showAlertDialog(this, $"Share to {group.Name} succesful!", (obj, eve) => { Finish(); });

                }
                else
                {
                    showAlertDialog(this, "Failed to share tok.");
                }
            };

            dialog.Show();

        }

        [Java.Interop.Export("onRadioButtonClicked")]
        public void onRadioButtonClicked(View view) {
            Report.ItemSelected(view);
        }

        [Java.Interop.Export("OnReport")]
        public async void OnReport(View view)
        {
            var alert = new AlertDialog.Builder(this);
            Report.ReportProgress.Visibility = ViewStates.Visible;
            if (!string.IsNullOrEmpty(Report.SelectedReportMessage) || Report.SelectedReportMessage != "Choose One")
            {
                Report r = new Report();
                r.ItemId = classTokModel.Id;
                r.ItemLabel = classTokModel.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = classTokModel.UserId;
                r.UserId = Settings.GetTokketUser()?.Id;

              var result =   await ReactionService.Instance.AddReport(r);
              
                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    alert.SetTitle("Report Successful!");
                    alert.SetPositiveButton("OK",(obj,eve)=> {
                        Report.Dismiss();
                    });
                }
                else {
                    alert.SetTitle("Report Failed!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                alert.Show();
            }
            else {
                alert.SetTitle("Select a reason for the report!");
                alert.Show();
            }
            Report.ReportProgress.Visibility = ViewStates.Gone;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //FacebookClientManager.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.AddSectionPage) && (resultCode == Result.Ok) && (data != null))
            {
                var tokdataSection = data.GetStringExtra("toksection");
                if (tokdataSection != null)
                {
                    var toksection = JsonConvert.DeserializeObject<TokSection>(tokdataSection);
                    if (toksection != null)
                    {
                        var ListTokSections = new List<TokSection>();
                        ListTokSections = tokModel.Sections.ToList();

                        var result = ListTokSections.FirstOrDefault(c => c.Id == toksection.Id);
                        if (result != null) //If Edit
                        {
                            int ndx = ListTokSections.IndexOf(result);
                            ListTokSections.Remove(result);

                            ListTokSections.Insert(ndx, toksection);
                        }
                        else //if save
                        {
                            ListTokSections.Add(toksection);
                        }

                        tokModel.Sections = ListTokSections.ToArray();
                        MegaTokSections();
                    }
                }
            }
            else if ((requestCode == (int)ActivityType.AddTokActivityType) && (resultCode == Result.Ok) && (data != null))
            {
                tokModel = new TokModel();
                tokModel = classTokModel;
                if (classTokModel != null)
                {
                    changesMade = true; //If there are changes made in AddTokActivity
                    if (classTokModel.IsMegaTok == true || classTokModel.TokGroup.ToLower() == "mega") //If Mega
                    {
                        var tokdataSection = data.GetStringExtra("toksection");
                        if (tokdataSection != null)
                        {
                            var toksection = JsonConvert.DeserializeObject<List<TokSection>>(tokdataSection);
                            if (toksection != null)
                            {
                                var ListTokSections = new List<TokSection>();
                                ListTokSections = tokModel.Sections.ToList();
                                foreach (var sec in toksection)
                                {
                                    var result = ListTokSections.FirstOrDefault(c => c.Id == sec.Id);
                                    if (result != null) //If Edit
                                    {
                                        int ndx = ListTokSections.IndexOf(result);
                                        ListTokSections.Remove(result);

                                        ListTokSections.Insert(ndx, sec);
                                    }
                                    else //if save
                                    {
                                        ListTokSections.Add(sec);
                                    }
                                }

                                classTokModel.Sections = ListTokSections.ToArray();
                                tokModel.Sections = classTokModel.Sections;
                                MegaTokSections();
                            }
                        }
                    }
                    else
                    {
                        var tokModelstring = data.GetStringExtra("classtokModel");
                        if (tokModelstring != null)
                        {
                            classTokModel = JsonConvert.DeserializeObject<ClassTokModel>(tokModelstring);
                            tokModel = classTokModel;

                            FillUpFields();
                            AddTokDetails(1);
                        }
                    }
                }
            }
            else if ((requestCode == (int)ActivityType.AddStickerDialogActivity) && (resultCode == Result.Ok))
            {
                var dataTokModelstr = data.GetStringExtra("tokModel");
                if (dataTokModelstr != null)
                {
                    var dataTokModel = JsonConvert.DeserializeObject<TokModel>(dataTokModelstr);
                    if (dataTokModel != null)
                    {
                        Intent = new Intent();
                        isUpdated = true;

                        tokModel.Sticker = dataTokModel.Sticker;
                        tokModel.StickerImage = dataTokModel.StickerImage;
                        classTokModel.Sticker = dataTokModel.Sticker;
                        classTokModel.StickerImage = dataTokModel.StickerImage;
                        Glide.With(this).Load(dataTokModel.StickerImage).Into(StickerImage);

                        StickerImage.Visibility = ViewStates.Visible;

                        Intent.PutExtra("isUpdated", isUpdated);
                        Intent.PutExtra("classtokModel", JsonConvert.SerializeObject(classTokModel));
                        SetResult(Result.Ok, Intent);
                    }
                }
            }
            else if ((requestCode == REQUEST_TOK_INFO_REPLY) && (resultCode == Result.Ok))
            {
                var dataCommentstr = data.GetStringExtra("commentReaction");
                if (dataCommentstr != null)
                {
                    var commentModel = JsonConvert.DeserializeObject<ReactionModel>(dataCommentstr);
                    if (commentModel.Kind != "inaccurate")
                    {
                        var result = TokInfoVm.CommentsCollection.FirstOrDefault(c => c.Id == commentModel.Id);
                        if (result != null) //If Edit
                        {
                            int ndx = TokInfoVm.CommentsCollection.IndexOf(result);
                            TokInfoVm.CommentsCollection.Remove(result);

                            TokInfoVm.CommentsCollection.Insert(ndx, commentModel);
                        }
                        else //if save
                        {
                            TokInfoVm.CommentsCollection.Add(commentModel);
                        }

                        adapterComments = TokInfoVm.CommentsCollection.GetRecyclerAdapter(BindCommentsViewHolder, Resource.Layout.tokinfo_comments_row);
                        RecyclerComments.SetAdapter(adapterComments);
                    }
                }
            }
            else if ((requestCode == REQUEST_GROUPSELECTION_REPLY) && (resultCode == Result.Ok)) { 
                var groupdata = data.GetStringExtra("ClassGroupModel");
                if (!string.IsNullOrEmpty(groupdata)) {
                    var group = JsonConvert.DeserializeObject<ClassGroupModel>(groupdata);
                    ShareTokGroup(group);
                }
             
            }
        }
        [Java.Interop.Export("OnFrameRTClicked")]
        public void OnFrameRTClicked(View v)
        {
            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_scaledown);
            GridReactionTable.StartAnimation(myAnim);
            FrameReactionTable.Visibility = ViewStates.Gone;
        }
        void OnTokButtonClick(object sender, EventArgs e)
        {
            //Filter for class toks
            FilterBy filterByClass = FilterBy.None;
            List<string> filterItems = new List<string>();
            //==== end ====

            string titlepage = "";
            string filter = "";
            string headerpage = (sender as TextView).Text;
            
            if ((int)(sender as TextView).Tag == (int)Toks.Category)
            {
                filter = tokModel.CategoryId;
#if (_CLASSTOKS)
                filterByClass = FilterBy.Category;
#endif

#if (_TOKKEPEDIA)
                Settings.FilterTag = 3;
#endif
                titlepage = "Category";
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokGroup)
            {
                filter = tokModel.TokGroup;
#if (_CLASSTOKS)

#endif

#if (_TOKKEPEDIA)
                Settings.FilterTag = 6;
#endif
                titlepage = "Tok Group";
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokType)
            {
                filter = tokModel.TokTypeId;
#if (_CLASSTOKS)
                filterByClass = FilterBy.Class;
                titlepage = "Class Name";
#endif

#if (_TOKKEPEDIA)
                Settings.FilterTag = 1;
                titlepage = "Tok Type";
#endif
            }

#if (_TOKKEPEDIA)
            Intent nextActivity = new Intent(this, typeof(ToksActivity));
            nextActivity.PutExtra("titlepage", titlepage);
            nextActivity.PutExtra("filter", filter);
            nextActivity.PutExtra("headerpage", headerpage);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            nextActivity.SetFlags(ActivityFlags.NewTask);
            this.StartActivity(nextActivity);
#endif
#if (_CLASSTOKS)

            filterItems.Add(filter);

            Intent nextActivity = new Intent(this, typeof(ClassToksActivity));
            nextActivity.PutExtra("titlepage", titlepage);
            nextActivity.PutExtra("filter", filter);
            nextActivity.PutExtra("headerpage", headerpage);
            nextActivity.PutExtra("filterItems", JsonConvert.SerializeObject(filterItems));
            nextActivity.PutExtra("filterBy", (int)filterByClass);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            nextActivity.SetFlags(ActivityFlags.NewTask);
            this.StartActivity(nextActivity);
#endif
        }

        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                //TextNothingFound.SetTextColor(Color.White);
                //TextNothingFound.SetBackgroundColor(Color.Transparent);
                //TextNothingFound.Visibility = ViewStates.Gone;

                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
            else
            {
                //TextNothingFound.Text = "No Internet Connection!";
                //TextNothingFound.SetTextColor(Color.White);
                //TextNothingFound.SetBackgroundColor(Color.Black);
                //TextNothingFound.Visibility = ViewStates.Visible;
                swipeRefreshComment.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //swipeRefreshComment.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            this.RunOnUiThread(async () => await LoadComments("", true));
        }

        private void ReactionTableTouched(object sender, View.TouchEventArgs e)
        {
            gesturedetector.OnTouchEvent(e.Event);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                if (v.ContentDescription == "ToolTipViews")
                {
                }
                else
                {
                    if (v.TooltipText == "selected")
                    {
                        var objBuilder = new AlertDialog.Builder(this);
                        objBuilder.SetTitle("");
                        objBuilder.SetMessage("You have already given this tok a gem.");
                        objBuilder.SetCancelable(false);

                        AlertDialog objDialog = objBuilder.Create();
                        objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) => { });
                        objDialog.Show();
                        objDialog.SetCanceledOnTouchOutside(false);
                    }
                    else
                    {
                        try
                        {
                            RunOnUiThread(async () => await OnClickAddReaction(v));
                        }
                        catch (Exception ex) { }


                    }
                }
            }
            else
            {
                if (v.ContentDescription == "ToolTipViews")
                {
                    FrameViews.Visibility = ViewStates.Visible;
                    GemsParentContainer.Visibility = ViewStates.Visible;
                }
            }

            return true;
        }

        private bool IsUrl(string uriName) {
            Uri uriResult;
            bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private TokInfoActivity tokInfoActivity;

            public MyGestureListener(TokInfoActivity Activity)
            {
                tokInfoActivity = Activity;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                //Console.WriteLine("Double Tab");
                return true;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {

                var modelConvert = JsonConvert.SerializeObject(tokInfoActivity.reactionValueVM);
                tokInfoActivity.nextActivity = new Intent(tokInfoActivity, typeof(ReactionValuesActivity));
                tokInfoActivity.nextActivity.PutExtra("reactionValueVM", modelConvert);
                tokInfoActivity.nextActivity.PutExtra("tokId", tokInfoActivity.tokModel.Id);
                tokInfoActivity.StartActivity(tokInfoActivity.nextActivity);
                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {
                tokInfoActivity.FrameReactionTable.Visibility = ViewStates.Visible;
                Animation myAnim = AnimationUtils.LoadAnimation(tokInfoActivity, Resource.Animation.fab_scaleup);
                tokInfoActivity.GridReactionTable.StartAnimation(myAnim);
            }
            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                return base.OnFling(e1, e2, velocityX, velocityY);
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                return base.OnScroll(e1, e2, distanceX, distanceY);
            }
        }
        #region UI Properties

        public ImageButton BtnShareFB => FindViewById<ImageButton>(Resource.Id.btnFBShare);

        public LinearLayout ShareCaptionLayout => FindViewById<LinearLayout>(Resource.Id.shareCaptionLayout);
        public ImageButton BtnShareTwitter => FindViewById<ImageButton>(Resource.Id.btnTwitterShare);
        public RecyclerView RecyclerTokMojisInaccurate => FindViewById<RecyclerView>(Resource.Id.tokinfoRecyclerTokMojisInaccurate);
        public ImageView BtnSmileInaccurate => FindViewById<ImageView>(Resource.Id.btnTokInfoSmileyInaccurate);
        public ImageView BtnSmile => FindViewById<ImageView>(Resource.Id.btnTokInfoSmiley);
        public RecyclerView RecyclerTokMojis => FindViewById<RecyclerView>(Resource.Id.tokinfoRecyclerTokMojis);
        public RecyclerView RecyclerTokMojisDummy => FindViewById<RecyclerView>(Resource.Id.tokinfoRecyclerTokMojisDummy);
        public TextView TotalGreen => FindViewById<TextView>(Resource.Id.reacttable_totalgreen);
        public TextView TotalYellow => FindViewById<TextView>(Resource.Id.reacttable_totalyellow);
        public TextView TotalRed => FindViewById<TextView>(Resource.Id.reacttable_totalred);
        public TextView TotalAccurate => FindViewById<TextView>(Resource.Id.reacttable_totalcheck);
        public TextView TotalInaccurate => FindViewById<TextView>(Resource.Id.reacttable_totalwrong);
        public TextView OverallTotalReactions => FindViewById<TextView>(Resource.Id.reacttable_overalltotal);
        public TextView txtUserDisplayName => FindViewById<TextView>(Resource.Id.lbl_tokinfonameuser);
        public CircleImageView imgUserPhoto => FindViewById<CircleImageView>(Resource.Id.img_tokUserPhoto);
        public CircleImageView imgcomment_userphoto => FindViewById<CircleImageView>(Resource.Id.imgcomment_userphoto);
        public ImageView StickerImage => FindViewById<ImageView>(Resource.Id.imgtokinfo_stickerimage);
        public LinearLayout isEnglishLinear => FindViewById<LinearLayout>(Resource.Id.linear_ConvertTopic);
        public LinearLayout linear_Reactions => FindViewById<LinearLayout>(Resource.Id.linear_Reactions);
        public LinearLayout linearParent => FindViewById<LinearLayout>(Resource.Id.linear_Topic);
        public TextView lbl_detail => FindViewById<TextView>(Resource.Id.lbl_detail);
        public TextView tokcategory => FindViewById<TextView>(Resource.Id.lblTokInfoTokCategory);
        public TextView tokgroup => FindViewById<TextView>(Resource.Id.lblTokInfoTokGroup);
        public TextView toktype => FindViewById<TextView>(Resource.Id.lblTokInfoTokType);
        public TextView tokclassgroup => FindViewById<TextView>(Resource.Id.lblClassGroup);
        public ImageView tokinfo_imgMain => FindViewById<ImageView>(Resource.Id.tokinfo_imgMain);
        public ImageView tokinfo_imgSecondary => FindViewById<ImageView>(Resource.Id.tokinfo_imgSecondary);
        public TextView ReactionCheck => FindViewById<TextView>(Resource.Id.reactiontable_check);
        public TextView ReactionWrong => FindViewById<TextView>(Resource.Id.reactiontable_wrong);
        public FrameLayout FrameReactionBtn => FindViewById<FrameLayout>(Resource.Id.frame_btntokinfoReaction);
        public FrameLayout FrameReactionTable => FindViewById<FrameLayout>(Resource.Id.framelayoutRL_reactiontable);
        public GridLayout GridReactionTable => FindViewById<GridLayout>(Resource.Id.gridtokinfo_reactiontable);
        public ImageView ImgPurpleGem => FindViewById<ImageView>(Resource.Id.tokinfo_purplegem);
        public FrameLayout PurpleGemContainer => FindViewById<FrameLayout>(Resource.Id.tokinfo_purplegemscontainer);
        public LinearLayout GreenGemContainer => FindViewById<LinearLayout>(Resource.Id.tokinfo_linear_greengem);
        public LinearLayout YellowGemContainer => FindViewById<LinearLayout>(Resource.Id.tokinfo_linear_yellowgem);
        public LinearLayout RedGemContainer => FindViewById<LinearLayout>(Resource.Id.tokinfo_linear_redgem);
        public LinearLayout TreasureContainer => FindViewById<LinearLayout>(Resource.Id.tokinfo_linear_treasure);
        public LinearLayout GemsParentContainer => FindViewById<LinearLayout>(Resource.Id.LinearGemsParentContainer);
        public TextView GreenGemHeader => FindViewById<TextView>(Resource.Id.greengemheader);
        public TextView TextTotalViews => FindViewById<TextView>(Resource.Id.lblTokInfoViews);
        public ProgressBar ProgressViews => FindViewById<ProgressBar>(Resource.Id.circleprogressViews);
        public ProgressBar ProgressViewsShare => FindViewById<ProgressBar>(Resource.Id.circleprogressViews1);
        public Button BtnTokInfoEyeIcon => FindViewById<Button>(Resource.Id.btnTokInfoEyeIcon);
        public TextView GreenGemFooter => FindViewById<TextView>(Resource.Id.greengemfooter);
        public TextView YellowGemHeader => FindViewById<TextView>(Resource.Id.yellowgemheader);
        public TextView YellowGemFooter => FindViewById<TextView>(Resource.Id.yellowgemfooter);
        public TextView RedGemHeader => FindViewById<TextView>(Resource.Id.redgemheader);
        public TextView RedGemFooter => FindViewById<TextView>(Resource.Id.redgemfooter);
        public TextView TreasureHeader => FindViewById<TextView>(Resource.Id.treasureheader);
        public TextView TreasureFooter => FindViewById<TextView>(Resource.Id.treasurefooter);
        public ImageView ReactionImgGreen => FindViewById<ImageView>(Resource.Id.tokinfo_imggreenreaction);
        public ImageView ReactionImgYellow => FindViewById<ImageView>(Resource.Id.tokinfo_imgyellowreaction);
        public ImageView ReactionImgRed => FindViewById<ImageView>(Resource.Id.tokinfo_imgredreaction);
        public ImageView ReactionImgTreasure => FindViewById<ImageView>(Resource.Id.tokinfo_imgtreasurereaction);
        public RecyclerView RecyclerComments => FindViewById<RecyclerView>(Resource.Id.tokinfo_comments_recyclerView);
        public NestedScrollView NestedScroll => FindViewById<NestedScrollView>(Resource.Id.NestedScrollTokInfo);
        public NestedScrollView NestedComment => FindViewById<NestedScrollView>(Resource.Id.NestedTokInfoComment);
        public EditText CommentEditor => FindViewById<EditText>(Resource.Id.tokinfo_txtComment);
        public TextView TokDateTimeCreated => FindViewById<TextView>(Resource.Id.lbl_tokDateCreated);
        public ImageButton TokBackButton => FindViewById<ImageButton>(Resource.Id.btnTokInfoTokBack);
        public ShimmerLayout ShimmerCommentsList => FindViewById<ShimmerLayout>(Resource.Id.ShimmerTokInfoComments);
        public ProgressBar CircleProgress => FindViewById<ProgressBar>(Resource.Id.circleprogressComments);
        public FrameLayout FrameViews => FindViewById<FrameLayout>(Resource.Id.frame_tokViews);
        public TextView TextToolTotalViews => FindViewById<TextView>(Resource.Id.TextTotalViews);
        public TextView TextTotalOpened => FindViewById<TextView>(Resource.Id.TextTotalOpened);
        public TextView TextTotalVisited => FindViewById<TextView>(Resource.Id.TextTotalVisited);
        public TextView TextTotalOpenedByOwner => FindViewById<TextView>(Resource.Id.TextTotalOpenedByOwner);
        public TextView TextTotalVisitedByOwner => FindViewById<TextView>(Resource.Id.TextTotalVisitedByOwner);
        public TextView OverallTotalReactionsDisplay => FindViewById<TextView>(Resource.Id.lbltokinfo_totalReactions);
        public TextView TextTotalComments => FindViewById<TextView>(Resource.Id.TextTotalComments);
        public ProgressBar ProgressComments => FindViewById<ProgressBar>(Resource.Id.circleprogressComments);
        public Button BtnMegaAccurate => FindViewById<Button>(Resource.Id.tokinfo_btnMegaAccurate);
        public Button BtnMegaInaccurate => FindViewById<Button>(Resource.Id.tokinfo_btnMegaInaccurate);
        public ConstraintLayout constraintMegaInaccurateComment => FindViewById<ConstraintLayout>(Resource.Id.constraintMegaInaccurateComment);
        public EditText EditInaccurateComment => FindViewById<EditText>(Resource.Id.EditInaccurateComment);
        public Button BtnInaccurateComment => FindViewById<Button>(Resource.Id.BtnInaccurateComment);
        public ImageView btnTokInfo_SendComment => FindViewById<ImageView>(Resource.Id.btnTokInfo_SendComment);
        public LinearLayout LinearTokInfoReaction => FindViewById<LinearLayout>(Resource.Id.LinearTokInfoReaction);
        public TextView LabelTokType => FindViewById<TextView>(Resource.Id.LabelTokType);
        public TextView LabelTokGroup => FindViewById<TextView>(Resource.Id.LabelTokGroup);
        public View ViewDummyForTouch => FindViewById<View>(Resource.Id.ViewDummyForTouch);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView txtProgressText => FindViewById<TextView>(Resource.Id.txtProgressText);
        public SwipeRefreshLayout swipeRefreshComment => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshComment);
        public Button btnTotalReactions => FindViewById<Button>(Resource.Id.btn_tok_info_total_reactions);
        public LinearLayout ShareView => FindViewById<LinearLayout>(Resource.Id.shareProfile);
        public CircleImageView ShareImageProfile => FindViewById<CircleImageView>(Resource.Id.img_shareImage);
        public TextView ShareUserName => FindViewById<TextView>(Resource.Id.shareuser_name);
        public Button BtnOriginalTok => FindViewById<Button>(Resource.Id.btnShowOriginalTok);
        public TextView SourceLinkView => FindViewById<TextView>(Resource.Id.lbl_sourceLink);
        public TextView SharedCaptionView => FindViewById<TextView>(Resource.Id.lbl_sharecaption);

        public TextView lblInaccurate => FindViewById<TextView>(Resource.Id.lbl_inaccurate);
        public ImageView HeartReaction => FindViewById<ImageView>(Resource.Id.tokinfo_heartdetail);
        public ImageView HundredReaction => FindViewById<ImageView>(Resource.Id.tokinfo_hundred);
        public Button btnShowHideOwnerAnswer => FindViewById<Button>(Resource.Id.btnShowHideOwnerAnswer);
        #endregion
    }
}
public class ClickableText : ClickableSpan
{
    public Actions ClickedAction;
    public string ActionText;

    public ClickableText(Actions ac, string at)
    {
        ClickedAction = ac;
        ActionText = at;
    }

    public override void OnClick(Android.Views.View widget)
    {
        widget.Invalidate();

        //(widget as TextView).SetHighlightColor(Color.ParseColor("#cde9fc"));

        /* var handler = new Handler();
         handler.PostDelayed(() =>
         {
             (widget as TextView).SetHighlightColor(Color.Transparent);
         }, 30L);*/

        if (ClickedAction == Actions.OpenHashTag)
        {
            //(widget as TextView).SetHighlightColor(Color.ParseColor("#cde9fc"));

            var nextActivity = new Intent(TokInfoActivity.Instance, typeof(HashtagActivity));
            nextActivity.PutExtra("hashtag", ActionText);
            TokInfoActivity.Instance.StartActivity(nextActivity);

            //(widget as TextView).SetHighlightColor(Color.Transparent);
        }
    }
    public override void UpdateDrawState(TextPaint ds)
    {
        base.UpdateDrawState(ds);
        //paint.setUnderlineText(true); // set underline if you want to underline

        //ds.BgColor = Color.ParseColor("#cde9fc");
        ds.Color = Color.ParseColor("#3498db"); // set the color to blue
    }
}