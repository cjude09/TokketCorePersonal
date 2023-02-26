using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Shared.Extensions;
using Android.Content.Res;
using System.Timers;
using Android.Views.Animations;
using System.Threading;
using Tokket.Android.Fragments;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.ViewModels;
using Android.Text;
using Tokket.Android.Helpers;
using AndroidX.Preference;
using Color = Android.Graphics.Color;
using Android.Content.PM;
using Tokket.Core.Tools;
using Android.Text.Method;
using Xamarin.Essentials;
using Android.Animation;
using Tokket.Shared.Services.ServicesDB;
using Priority = Bumptech.Glide.Priority;
using Google.Android.Material.FloatingActionButton;

namespace Tokket.Android
{
    [Activity(Label = "Tok Cards", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokCardsMiniGameActivity : BaseActivity
    {
        int txtMinLimit = 13, txtMaxLimit = 10000;
        SpannableStringBuilder ssbName;
        ISpannable spannableResText;
        List<Tokmoji> ListTokmojiModel;
        public Set setList; 
        public ClassSetModel classSet; ClassSetViewModel classSetVM;
        public GestureDetector gesturedetector; private bool Showingback, isStop;
        public List<TokModel> restokList, TokLists, favList;
        public List<ClassTokModel> ClassTokLists;
        public int cnt = 0; public ProgressBar cardProgress, progressbarTokCardLoading;
        AndroidX.Fragment.App.FragmentTransaction trans; public List<bool> isFavorite;
        public bool isPlayFavorite, isImageVisible = true; bool isSet = true, allowAddTok = true;
        System.Timers.Timer _timer; object _lock = new object(); string IntentTokList = "";
        public TextView cardProgressText; public FloatingActionButton btnNext, btnPrevious;
        public FrameLayout frameTokCardMini; Button btnTokCardsPlay;
        public HomePageViewModel HomeVm => App.Locator.HomePageVM;
        string continuationToken = "", frontText = "", backText = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.tokcards_minigame_page);
            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.tokcards_toolbar);

            SetSupportActionBar(tokback_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            continuationToken = Settings.HomeContinuationToken;

            var txtTCLoadMore = FindViewById<TextView>(Resource.Id.txtTCLoadMore);
            var btnTokCardFlip = FindViewById<Button>(Resource.Id.btnTokCardFlip);
            frameTokCardMini = FindViewById<FrameLayout>(Resource.Id.frameTokCardMini);
            var btnTokCardShuffle = FindViewById<Button>(Resource.Id.btnTokCardShuffle);
            btnTokCardsPlay = FindViewById<Button>(Resource.Id.btnTokCardsPlay);
            var btnTokCardOptions = FindViewById<Button>(Resource.Id.btnTokCardOptions);

            Typeface font = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            btnTokCardOptions.Typeface = font;
            btnTokCardShuffle.Typeface = font;
            btnTokCardsPlay.Text = "► Play";

            cardProgressText = FindViewById<TextView>(Resource.Id.cardProgressText);
            cardProgress = FindViewById<ProgressBar>(Resource.Id.cardProgress);
            progressbarTokCardLoading = FindViewById<ProgressBar>(Resource.Id.progressbarTokCardLoading);
            TokLists = new List<TokModel>();

            isSet = Intent.GetBooleanExtra("isSet", isSet);
            //IntentTokList = Intent.GetStringExtra("TokList");
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            IntentTokList = prefs.GetString("TokModelList", "");
            if (string.IsNullOrEmpty(IntentTokList) || IntentTokList == "null")
            {
                IntentTokList = Intent.GetStringExtra("TokModelList");
            }

            if (isSet && (string.IsNullOrEmpty(IntentTokList) || IntentTokList == "null"))
            {
                txtTCLoadMore.Visibility = ViewStates.Gone;

                string StringExtra = Intent.GetStringExtra("setModel");

                if (StringExtra!=null)
                {
                    setList = JsonConvert.DeserializeObject<Set>(StringExtra);
                }
                else
                {
                    StringExtra = Intent.GetStringExtra("classsetModel"); //From Class Set
                    classSet = JsonConvert.DeserializeObject<ClassSetModel>(StringExtra);
                    setList = classSet;

                    var stringClassSetVM = Intent.GetStringExtra("ClassSetViewModel");
                    if (stringClassSetVM != null)
                    {
                        classSetVM = JsonConvert.DeserializeObject<ClassSetViewModel>(stringClassSetVM);
                    }
                }
                
            }
            else
            {
                SupportActionBar.Subtitle = Intent.GetStringExtra("SubTitle");
            }

            if (Settings.FilterTag == 9)
            {
                //Load more stuff here
                txtTCLoadMore.Visibility = ViewStates.Visible;
            }
            else
            {
                txtTCLoadMore.Visibility = ViewStates.Gone;
            }

         

            btnPrevious = FindViewById<FloatingActionButton>(Resource.Id.FabMiniGamePrevious);
            btnNext = FindViewById<FloatingActionButton>(Resource.Id.FabMiniGameNext);
            btnPrevious.Enabled = false;

            this.RunOnUiThread(async () => await InitializeData());

            _timer = new System.Timers.Timer();
            _timer.Interval = 6000;

            isStop = true; //default is true
            btnTokCardsPlay.Click -= OnPlayCardClick;
            btnTokCardsPlay.Click += OnPlayCardClick;

            //If Load More is clicked
            txtTCLoadMore.Click += async (object sender, EventArgs e) =>
            {
                progressbarTokCardLoading.Visibility = ViewStates.Visible;
#if (_TOKKEPEDIA)
                TokQueryValues tokQueryModel = new TokQueryValues();
                tokQueryModel.sortby = Settings.SortByFilter;
                tokQueryModel.token = Settings.ContinuationToken;
                tokQueryModel.loadmore = "yes";
                var restokList = await TokService.Instance.GetAllToks(tokQueryModel);
#endif
#if (_CLASSTOKS)
                var tokQueryModel = new ClassTokQueryValues();
                tokQueryModel.paginationid = continuationToken;
                //tokQueryModel.loadmore = "yes";
                tokQueryModel.partitionkeybase = "classtoks";
                tokQueryModel.text = ""; // filter;
                tokQueryModel.startswith = false;

                var result = await ClassService.Instance.GetClassToksAsync(new GetClassToksRequest() { 
                    QueryValues = tokQueryModel
                });
                var resultList = result.Results.ToList();
                continuationToken = result.ContinuationToken;
#endif
                foreach (var item in resultList)
                {
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega") //If Mega
                    {
                        allowAddTok = false;
                    }
                    else
                    {
                        allowAddTok = true;
                    }

                    if (allowAddTok)
                    {
                        TokLists.Add(item);
                    }
                }

                isFavorite.AddRange(Enumerable.Repeat(false, TokLists.Count).ToList());
                cardProgress.Max = TokLists.Count;
                cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                progressbarTokCardLoading.Visibility = ViewStates.Gone;
            };

            btnPrevious.Click += (object sender, EventArgs e) =>
            {
                Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaleup);
                frameTokCardMini.StartAnimation(myAnim);
                cnt = cnt - 1;
                cardProgress.IncrementProgressBy(-1);
                cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;

                trans = SupportFragmentManager.BeginTransaction();
                if(!Showingback)
                     trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
                else
                    trans.Replace(Resource.Id.frameTokCardMini, new CardBackFragment());
                trans.AddToBackStack(null);
                trans.Commit();

                if (cnt == 0)
                {
                    btnPrevious.Enabled = false;
                }
                btnNext.Enabled = true;
            };
            btnNext.Click -= OnNextCard;
            btnNext.Click += OnNextCard;
            btnTokCardShuffle.Click += (object sender, EventArgs e) =>
            {
                cnt = 0;
                cardProgress.Progress = cnt + 1;
                cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                btnPrevious.Enabled = false;
                if (cnt < TokLists.Count)
                {
                    btnNext.Enabled = true;
                }

                var joined = TokLists.Zip(isFavorite, (x, y) => new { x, y });
                var shuffled = joined.OrderBy(x => Guid.NewGuid()).ToList();
                TokLists = shuffled.Select(pair => pair.x).ToList();
                isFavorite = shuffled.Select(pair => pair.y).ToList();

                trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
                trans.AddToBackStack(null);
                trans.Commit();
            };
            btnTokCardFlip.Click -= OnFlipCard;
            btnTokCardFlip.Click += OnFlipCard;

            btnTokCardOptions.Click += (object sender, EventArgs e) =>
            {
                Bundle args = new Bundle();
                args.PutString("isPlayFavorite", isPlayFavorite.ToString());
                args.PutString("isImageVisible", isImageVisible.ToString());
                AndroidX.AppCompat.App.AppCompatDialogFragment newFragment = new ModalTokCardsOptions();
                newFragment.Arguments = args;
                newFragment.Show(SupportFragmentManager, "Options");
            };
        }
        public void OnFlipCard(object sender, EventArgs e)
        {
            FlipCard();
        }
        private void OnPlayCardClick(object sender, EventArgs e)
        {
            isStop = !isStop;
            PlayCard();
        }
        public void TriggerFavorite()
        {
            if (isPlayFavorite)
            {
                favList = new List<TokModel>();
                for (int i = 0; i < isFavorite.Count; i++)
                {
                    if (isFavorite[i] == true)
                    {
                        favList.Add(TokLists[i]);
                    }
                }

                cnt = 0;

                if (favList.Count != 0)
                {
                    loadFrontCard();
                }

                cardProgress.Progress = 1;
                cardProgress.Max = favList.Count;
                cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;

                if (favList.Count != 0)
                {
                    isStop = false; //to auto play
                    PlayCard();
                }
            }
            else
            {
                cnt = 0;
                cardProgress.Max = TokLists.Count;
                cardProgress.IncrementProgressBy(1);
                cardProgressText.Text = 1 + "/" + cardProgress.Max;
            }
        }
        private void PlayCard()
        {
            if (isStop) //If Stop
            {
                _timer.Stop();
                btnTokCardsPlay.Text = "► Play";
                btnTokCardsPlay.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#007bff"));
            }
            else if (!isStop) //If Play
            {
                _timer.AutoReset = true;
                _timer.Enabled = true;
                _timer.Elapsed += OnTimeEvent;
                btnTokCardsPlay.Text = "■ Stop";
                btnTokCardsPlay.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#dc3545"));
            }

            if (cnt == 0)
            {
                btnPrevious.Enabled = false;
            }

            if (cardProgress.Max > 1)
            {
                btnNext.Enabled = true;
            }
        }

        private void loadGlidImage(Context context, string image, ImageView imageView)
        {
            RequestOptions options = new RequestOptions()
                           .Placeholder(Resource.Animation.loader_animation)
                           .Error(Resource.Drawable.no_image)
                           .Override(500);

            Glide.With(context).Load(image).Thumbnail(0.05f).Apply(options).Into(imageView);
        }

        private async Task LoadTokMoji()
        {
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();
        }

        private async Task GetToksData()
        {
            restokList = await HomeVm.GetToksData("", (FilterType)Settings.FilterTag);
        }
        public async Task InitializeData()
        {
            ClassTokLists = new List<ClassTokModel>();
            bool isNonPlayable = false;
            progressbarTokCardLoading.Visibility = ViewStates.Visible;

            //Get Tokmoji
            var tokmojiResult = TokMojiService.Instance.GetCacheTokmojisAsync();
            if (tokmojiResult.Results != null)
            {
                ListTokmojiModel = tokmojiResult.Results.ToList();
            }

            List<Task> tasksList = new List<Task>();
            if (ListTokmojiModel?.Count == 0)
            {
                tasksList.Add(LoadTokMoji());
            }

            if (setList != null)
            {
                if (classSetVM == null)
                {
                    string classTokModelstr = Intent.GetStringExtra("classTokModel");
                    restokList = new List<TokModel>();

                    if (!string.IsNullOrEmpty(classTokModelstr))
                    {
                        var classtok = JsonConvert.DeserializeObject<List<ClassTokModel>>(classTokModelstr);

                        if (classtok.Count > 0)
                        {
                            restokList.AddRange(classtok);
                        }
                    }
                    else
                    {
                        classSetVM = await GetClassToksAsync();
                        if (classSetVM.ClassToks.Count() > 0)
                        {
                            ClassTokLists = classSetVM.ClassToks;
                        }
                    }

                    if (restokList.Count == 0 && ClassTokLists.Count ==0)
                    {
                        restokList = await GetSetToks();
                    }
                }
                else
                {
                    ClassTokLists = classSetVM.ClassToks;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IntentTokList))
                {
                    restokList = JsonConvert.DeserializeObject<List<TokModel>>(IntentTokList);
                }
                else
                {
#if (_TOKKEPEDIA)
                    tasksList.Add(GetToksData());
#endif
#if (_CLASSTOKS)
                    var cachedClassToks = ClassService.Instance.GetCacheClassToksAsync("classtoks_fragment");
                    if (cachedClassToks.Results != null)
                    {
                        ClassTokLists = cachedClassToks.Results.ToList();
                    }

                    if (ClassTokLists.Count == 0)
                    {
                        tasksList.Add(GetClassToksData());
                    }
#endif
                }
            }

            if (tasksList.Count > 0)
            {
                await Task.WhenAll(tasksList);
            }

            
            if (classSetVM == null && ClassTokLists.Count == 0)
            {
                string classTokModelstr = Intent.GetStringExtra("classTokModel");
                if (!string.IsNullOrEmpty(classTokModelstr))
                {
                    var classtok = JsonConvert.DeserializeObject<List<ClassTokModel>>(classTokModelstr);
                    if (restokList.Count == 0)
                    {
                        foreach (var tok in classtok)
                        {
                            restokList.Add(tok);
                        }
                    }

                    foreach (var item in restokList)
                    {
                        //if (SupportActionBar.Subtitle.ToLower().Contains("category:") && SupportActionBar.Subtitle.ToLower().Contains("user:"))
                        //{
                        if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega" || item.TokGroup.ToLower() == "pic" || item.TokGroup.ToLower() == "list") //If Mega
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
                }
            }
            else
            {
                foreach (var item in ClassTokLists)
                {
                    //if (SupportActionBar.Subtitle.ToLower().Contains("category:") && SupportActionBar.Subtitle.ToLower().Contains("user:"))
                    //{
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega" || item.TokGroup.ToLower() == "pic" || item.TokGroup.ToLower() == "list") //If Mega
                    {
                        allowAddTok = false;
                        isNonPlayable = true;
                    }

                    if (allowAddTok)
                    {
                        TokLists.Add(item);
                    }
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

            progressbarTokCardLoading.Visibility = ViewStates.Gone;

            cardProgress.Max = TokLists.Count;
            isFavorite = Enumerable.Repeat(false, TokLists.Count).ToList();
            cardProgress.Progress = 1;
            cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;

            if (TokLists.Count <= 1)
            {
                btnNext.Enabled = false;
            }

            if (TokLists.Count > 0)
            {
                gesturedetector = new GestureDetector(this, new MyGestureListener(this));

                trans = SupportFragmentManager.BeginTransaction();
                trans.Add(Resource.Id.frameTokCardMini, new CardFrontFragment());
                trans.Commit();
            }
        }
        public async Task<List<TokModel>> GetSetToks()
        {
            var list = new List<TokModel>();
            foreach (var tok in setList.TokIds)
            {
                var tokRes = await TokService.Instance.GetTokIdAsync(tok);
                if (tokRes != null)
                    list.Add(tokRes);
            }
            return list;
        }

        private async Task<ClassSetViewModel> GetClassToksAsync()
        {
            classSetVM = new ClassSetViewModel();
            var classtokRes = await ClassService.Instance.GetClassToksAsync(
                new GetClassToksRequest() { 
                    QueryValues = new ClassTokQueryValues() { partitionkeybase = $"{classSet.Id}-classtoks", publicfeed = false }
                }
               );
            classSetVM.ClassToks = classtokRes.Results.ToList();

            classSetVM.ClassSet = classSet;

            return classSetVM;
        }

        private void OnNextCard(object sender, EventArgs e)
        {
            if (cardProgress.Max > 1)
            {
                cnt = cnt + 1;
                if (!Showingback)
                    loadFrontCard();
                else
                    loadBackCard();


                cardProgress.IncrementProgressBy(1);
                cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                if (TokLists.Count <= 1 || cardProgress.Progress == TokLists.Count)
                {
                    btnNext.Enabled = false;
                }
                btnPrevious.Enabled = true;
            }
        }
        public void loadFrontCard()
        {
            Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaleup);
            frameTokCardMini.StartAnimation(myAnim);

            trans = SupportFragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
            trans.AddToBackStack(null);
            trans.CommitAllowingStateLoss();
            Showingback = false;

            cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
        }

        
        public void loadBackCard() {
            Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaleup);
            frameTokCardMini.StartAnimation(myAnim);

            trans = SupportFragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.frameTokCardMini, new CardBackFragment());
            trans.AddToBackStack(null);
            trans.CommitAllowingStateLoss();
            Showingback = true;

            cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
        }
        public void removeFragmentinFL()
        {
            trans = SupportFragmentManager.BeginTransaction();
            trans.Remove(new CardFrontFragment());
            trans.AddToBackStack(null);
            trans.CommitAllowingStateLoss();
        }
        private void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            FlipCard();
            Thread.Sleep(5000);

            RunOnUiThread(() =>
            {
                OnNextCard(sender, e);
                CheckProgress(cnt);
            });
        }
        public void CheckProgress(int progress)
        {
            lock (_lock)
            {
                if (progress >= cardProgress.Max)
                {
                    cnt = 0;
                    cardProgress.Progress = 1;
                    cardProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                }
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

        private void FlipCard()
        {
            AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();
            if (Showingback)
            {
                trans.SetCustomAnimations(Resource.Animation.card_flip_right_in, Resource.Animation.card_flip_right_out, Resource.Animation.card_flip_left_in, Resource.Animation.card_flip_left_out);
                trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
                trans.AddToBackStack(null);
                trans.Commit();
              
                // SupportFragmentManager.PopBackStack();
                //loadFrontCard();
                Showingback = false;
            }
            else
            {
                //AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();
                trans.SetCustomAnimations(Resource.Animation.card_flip_right_in, Resource.Animation.card_flip_right_out, Resource.Animation.card_flip_left_in, Resource.Animation.card_flip_left_out);
                trans.Replace(Resource.Id.frameTokCardMini, new CardBackFragment());
                trans.AddToBackStack(null);
                trans.Commit();
                Showingback = true;
            }
        }

        private void showText()
        {
            string message = "";
            if (Showingback)
            {
                message = backText;
            }
            else
            {
                message = frontText;
            }
            
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("");
            alertDialog.SetMessage(message);
            alertDialog.SetNegativeButton("OK", (senderAlert, args) => {
                alertDialog.Dispose();
            });
            AlertDialog alert = alertDialog.Create();
            alert.Show();
        }
        private class CardFrontFragment : AndroidX.Fragment.App.Fragment
        {
            TokCardsMiniGameActivity tokCardsMiniGameActivity;
            bool expand = false; TextView lblPreviewCardFront; Button btnViewMore; ImageView img_previewcardfront;

            void frontCardScrollChangeGlobalLayoutHandler(object sender, EventArgs e)
            {
                var v = sender as ViewTreeObserver;
                Console.WriteLine("frontCardScrollChangeGlobalLayoutHandler " + lblPreviewCardFront.Bottom + " " + (lblPreviewCardFront.Height - lblPreviewCardFront.ScrollY));

                if (!lblPreviewCardFront.CanScrollVertically(1)) //Bottom has been reached
                {
                    btnViewMore.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnViewMore.Visibility = ViewStates.Gone;
                }
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                tokCardsMiniGameActivity = Activity as TokCardsMiniGameActivity;
                View frontcard_view = inflater.Inflate(Resource.Layout.preview_cardfront, container, false);

                var linear_previewcardfront = frontcard_view.FindViewById<LinearLayout>(Resource.Id.linear_previewcardfront);
                lblPreviewCardFront = frontcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardFront);
                btnViewMore = frontcard_view.FindViewById<Button>(Resource.Id.btnViewMore);
                img_previewcardfront = frontcard_view.FindViewById<ImageView>(Resource.Id.img_previewcardfront);
                img_previewcardfront.Visibility = ViewStates.Visible;
                expand = false;

                var viewTreeObserver = lblPreviewCardFront.ViewTreeObserver;
                viewTreeObserver.ScrollChanged -= frontCardScrollChangeGlobalLayoutHandler;
                viewTreeObserver.ScrollChanged += frontCardScrollChangeGlobalLayoutHandler;

                lblPreviewCardFront.MovementMethod = new ScrollingMovementMethod();
                lblPreviewCardFront.Measure(0, 0);
                List<TokModel> tokList = new List<TokModel>();
                if (tokCardsMiniGameActivity.isPlayFavorite)
                {
                    tokList = tokCardsMiniGameActivity.favList;
                }
                else
                {
                    tokList = tokCardsMiniGameActivity.TokLists;
                }

                bool isCharLimited = false;
                string detailstr = "", detailCompleteStr = "";
                if (tokList.Count > 0)
                {
                    if (tokCardsMiniGameActivity.cnt < tokList.Count)
                    {
                        if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].PrimaryFieldText))
                        {
                            if (tokList[tokCardsMiniGameActivity.cnt].IsDetailBased)
                            {
                                if (tokList[tokCardsMiniGameActivity.cnt].Details != null)
                                {
                                    for (int i = 0; i < tokList[tokCardsMiniGameActivity.cnt].Details.Count(); i++)
                                    {
                                        if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Details[i]))
                                        {
                                            var handler = new Handler();
                                            var detailStr = "• " + tokList[tokCardsMiniGameActivity.cnt].Details[i].ToString();
                                            var limitedDetail = "";
                                            if (detailStr.Length > 50)
                                            {
                                                isCharLimited = true;
                                                limitedDetail = detailStr.Substring(0, 49) + "...";
                                            }
                                            else
                                            {
                                                limitedDetail = detailStr;
                                            }

                                            if (i == 0)
                                            {
                                                detailstr = limitedDetail;

                                                detailCompleteStr = "• " + tokList[tokCardsMiniGameActivity.cnt].Details[i].ToString();
                                            }
                                            else
                                            {
                                                detailstr += "\n " + limitedDetail;

                                                detailCompleteStr += "\n• " + tokList[tokCardsMiniGameActivity.cnt].Details[i].ToString();
                                            }
                                        }
                                    }
                                    //vh.tokcardback.Typeface = Typeface.Default;

                                    tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(detailstr);
                                    tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, tokCardsMiniGameActivity.ssbName);
                                    lblPreviewCardFront.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);
                                }
                            }
                            else
                            {
                                tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(tokList[tokCardsMiniGameActivity.cnt].PrimaryFieldText);
                                tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(tokCardsMiniGameActivity, tokCardsMiniGameActivity.ssbName);

                                if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Image) && !string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].SecondaryImage))
                                {
                                    img_previewcardfront.Visibility = ViewStates.Visible;
                                    tokCardsMiniGameActivity.loadGlidImage(tokCardsMiniGameActivity, tokList[tokCardsMiniGameActivity.cnt].Image, img_previewcardfront);
                                }
                                
                                lblPreviewCardFront.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);
                            }
                        }
                    }
                }

                tokCardsMiniGameActivity.frontText = lblPreviewCardFront.Text;


                var txt_previewcardfrontstar = frontcard_view.FindViewById<TextView>(Resource.Id.txt_previewcardfrontstar);
                //load favorites
                if (tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] == true || tokCardsMiniGameActivity.isPlayFavorite == true)
                {
                    linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_yellowborder);
                    txt_previewcardfrontstar.SetBackgroundResource(Resource.Drawable.star_yellow);
                }
                else
                {
                    linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_violetborder);
                    txt_previewcardfrontstar.SetBackgroundResource(Resource.Drawable.star_gray);
                }

                if (isCharLimited)
                {
                    btnViewMore.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnViewMore.Visibility = ViewStates.Gone;
                }

                btnViewMore.Click += delegate
                {
                    if (!expand)
                    {
                        expand = true;
                        btnViewMore.Text = GetString(Resource.String.hide);
                        ObjectAnimator animation = ObjectAnimator.OfInt(lblPreviewCardFront, "maxLines", tokCardsMiniGameActivity.txtMaxLimit);
                        animation.SetDuration(100).Start();
                        lblPreviewCardFront.Text = detailCompleteStr;
                        lblPreviewCardFront.ScrollTo(0, 0);
                    }
                    else
                    {
                        expand = false;
                        btnViewMore.Text = GetString(Resource.String.view_more);
                        ObjectAnimator animation = ObjectAnimator.OfInt(lblPreviewCardFront, "maxLines", tokCardsMiniGameActivity.txtMinLimit);
                        animation.SetDuration(100).Start();
                        lblPreviewCardFront.Text = detailstr;
                        lblPreviewCardFront.ScrollTo(0, 0);
                    }
                };

                float heightText = lblPreviewCardFront.Paint.MeasureText(lblPreviewCardFront.Text,0,lblPreviewCardFront.Text.Length);
          
             

                //if star is clicked
                txt_previewcardfrontstar.Click += (Object sender, EventArgs e) =>
                {
                    if (tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] == true)
                    {
                        tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] = false;
                        linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_violetborder);
                        txt_previewcardfrontstar.SetBackgroundResource(Resource.Drawable.star_gray);
                    }
                    else
                    {
                        linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_yellowborder);
                        txt_previewcardfrontstar.SetBackgroundResource(Resource.Drawable.star_yellow);
                        tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] = true;
                    }
                };
                frontcard_view.Touch += Frontcard_view_Touch;
                return frontcard_view;
            }

            private void Frontcard_view_Touch(object sender, View.TouchEventArgs e)
            {
                TokCardsMiniGameActivity myactivity = Activity as TokCardsMiniGameActivity;

                myactivity.gesturedetector.OnTouchEvent(e.Event);
            }
        }
        public class CardBackFragment : AndroidX.Fragment.App.Fragment
        {
            TokCardsMiniGameActivity tokCardsMiniGameActivity;
            public View backcard_view; public ImageView img_previewcardback;
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                bool withSecondaryImage = false;
                tokCardsMiniGameActivity = Activity as TokCardsMiniGameActivity;
                backcard_view = inflater.Inflate(Resource.Layout.preview_cardback, container, false);
                img_previewcardback = backcard_view.FindViewById<ImageView>(Resource.Id.img_previewcardback);
                var linear_previewcardfront = backcard_view.FindViewById<LinearLayout>(Resource.Id.linear_previewcardback);
                var lblPreviewCardBack = backcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardBack);
                var btnViewMore = backcard_view.FindViewById<Button>(Resource.Id.btnViewMore);

                lblPreviewCardBack.MovementMethod = new ScrollingMovementMethod();
                if (tokCardsMiniGameActivity.isImageVisible)
                {
                    img_previewcardback.Visibility = ViewStates.Visible;
                }
                else
                {
                    img_previewcardback.Visibility = ViewStates.Gone;
                }

                List<TokModel> tokList = new List<TokModel>();
                if (tokCardsMiniGameActivity.isPlayFavorite)
                {
                    tokList = tokCardsMiniGameActivity.favList;
                }
                else
                {
                    tokList = tokCardsMiniGameActivity.TokLists;
                }

                if (tokList.Count > 0)
                {
                    if (tokList[tokCardsMiniGameActivity.cnt].IsMegaTok == true || tokList[tokCardsMiniGameActivity.cnt].TokGroup.ToLower() == "mega")
                    {
                        //Mega
                        for (int i = 0; i < tokList[tokCardsMiniGameActivity.cnt].Sections.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Sections[i].Title))
                            {
                                //lblPreviewCardBack.Text = tokList[tokCardsMiniGameActivity.cnt].Sections[i].Title;

                                tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(tokList[tokCardsMiniGameActivity.cnt].Sections[i].Title);
                                tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(tokCardsMiniGameActivity, tokCardsMiniGameActivity.ssbName);

                                lblPreviewCardBack.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);

                                break;
                            }
                        }
                    }
                    else if (tokList[tokCardsMiniGameActivity.cnt].IsDetailBased == true)
                    {
                        tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(tokList[tokCardsMiniGameActivity.cnt].PrimaryFieldText);
                        tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(tokCardsMiniGameActivity, tokCardsMiniGameActivity.ssbName);

                        lblPreviewCardBack.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);

                        /*if (tokCardsMiniGameActivity.TokLists[tokCardsMiniGameActivity.cnt].Details != null)
                        {
                            string detailstr = "";
                            for (int i = 0; i < tokList[tokCardsMiniGameActivity.cnt].Details.Count(); i++)
                            {
                                if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Details[i]))
                                {
                                    if (i == 0)
                                    {
                                        detailstr = "• " + tokList[tokCardsMiniGameActivity.cnt].Details[i].ToString();
                                    }
                                    else
                                    {
                                        detailstr += "\n• " + tokList[tokCardsMiniGameActivity.cnt].Details[i].ToString();
                                    }
                                }
                            }
                            lblPreviewCardBack.Typeface = Typeface.Default;
                            //lblPreviewCardBack.Text = detailstr;

                            tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(detailstr);
                            tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(tokCardsMiniGameActivity, tokCardsMiniGameActivity.ssbName);

                            lblPreviewCardBack.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);
                        }*/
                    }
                    else
                    {
                        //lblPreviewCardBack.Text = tokList[tokCardsMiniGameActivity.cnt].SecondaryFieldText;
                        if (string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].SecondaryFieldText))
                        {
                            tokList[tokCardsMiniGameActivity.cnt].SecondaryFieldText = "";
                        }

                        tokCardsMiniGameActivity.ssbName = new SpannableStringBuilder(tokList[tokCardsMiniGameActivity.cnt].SecondaryFieldText);
                        tokCardsMiniGameActivity.spannableResText = SpannableHelper.AddStickersSpannable(tokCardsMiniGameActivity, tokCardsMiniGameActivity.ssbName);

                        lblPreviewCardBack.SetText(tokCardsMiniGameActivity.spannableResText, TextView.BufferType.Spannable);
                        if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].SecondaryImage))
                        {
                            tokCardsMiniGameActivity.loadGlidImage(tokCardsMiniGameActivity, tokList[tokCardsMiniGameActivity.cnt].SecondaryImage, img_previewcardback);
                            img_previewcardback.Visibility = ViewStates.Visible;
                            withSecondaryImage = true;
                        }
                    }

                    //if tok is image
                    if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Image) && !withSecondaryImage)
                    {
                        string tokimg = tokList[tokCardsMiniGameActivity.cnt].Image + ".jpg";
                        Glide.With(tokCardsMiniGameActivity).Load(tokimg).Into(img_previewcardback);
                    };
                }

                tokCardsMiniGameActivity.backText = lblPreviewCardBack.Text;
                var txt_previewcardbackstar = backcard_view.FindViewById<TextView>(Resource.Id.txt_previewcardbackstar);

                //Load the colors
                if (tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] == true || tokCardsMiniGameActivity.isPlayFavorite == true)
                {
                    linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_yellowborder);
                    txt_previewcardbackstar.SetBackgroundResource(Resource.Drawable.star_yellow);
                }
                else
                {
                    linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_violetborder);
                    txt_previewcardbackstar.SetBackgroundResource(Resource.Drawable.star_gray);
                }

                //If star is clicked
                txt_previewcardbackstar.Click += (Object sender, EventArgs e) =>
                {
                    if (tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] == true)
                    {
                        tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] = false;
                        linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_violetborder);
                        txt_previewcardbackstar.SetBackgroundResource(Resource.Drawable.star_gray);
                    }
                    else
                    {
                        linear_previewcardfront.SetBackgroundResource(Resource.Drawable.linear_yellowborder);
                        txt_previewcardbackstar.SetBackgroundResource(Resource.Drawable.star_yellow);
                        tokCardsMiniGameActivity.isFavorite[tokCardsMiniGameActivity.cnt] = true;
                    }
                };

                if (img_previewcardback.Visibility == ViewStates.Visible)
                {
                    tokCardsMiniGameActivity.isImageVisible = true;
                }
                else
                {
                    tokCardsMiniGameActivity.isImageVisible = false;
                }

                backcard_view.Touch += Backcard_view_Touch;
                return backcard_view;
            }

            private void Backcard_view_Touch(object sender, View.TouchEventArgs e)
            {
                TokCardsMiniGameActivity myactivity = Activity as TokCardsMiniGameActivity;

                myactivity.gesturedetector.OnTouchEvent(e.Event);
            }
        }


        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private TokCardsMiniGameActivity mainActivity;

            public MyGestureListener(TokCardsMiniGameActivity Activity)
            {
                mainActivity = Activity;
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                mainActivity.FlipCard();
                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {
                Console.WriteLine("Long Press");

                mainActivity.showText();
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

        public async Task<List<ClassTokModel>> GetClassToksData()
        {
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;
            List<ClassTokModel> classTokModelsResult = new List<ClassTokModel>();
            bool isPublicFeed = false;
            if (Settings.FilterFeed == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/


            var userId = Settings.GetUserModel().UserId;

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;

                var queryValues = new ClassTokQueryValues()
                {
                    partitionkeybase = $"{Settings.GetUserModel().UserId}-classtoks",
                    groupid = isPublicFeed ? userId : "",
                    userid = isPublicFeed ? "" : userId,
                    text = "",
                    startswith = false,
                    publicfeed = isPublicFeed,
                    FilterBy = FilterBy.None,
                    FilterItems = new List<string>(),
                    searchvalue = null
                };

                ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
                tokResult.Results = new List<ClassTokModel>();

                string callername = "classtoks_fragment";

                tokResult = await ClassService.Instance.GetClassToksAsync(queryValues, cancellationToken, fromCaller: callername);

                classTokModelsResult = tokResult.Results.ToList();
            }

            return classTokModelsResult;
        }
    }
}