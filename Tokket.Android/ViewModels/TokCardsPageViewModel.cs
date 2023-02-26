using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.ViewModels.Base;
using Tokket.Core;
using Color = Android.Graphics.Color;
using Android.Content.Res;
using GalaSoft.MvvmLight.Command;

namespace Tokket.Android.ViewModels
{
    public class TokCardsPageViewModel : BaseViewModel
    {
        #region Properties
        System.Timers.Timer _timer;
        public List<TokModel> tokList { get; private set; }
        public List<TokModel> favList { get; private set; }
        public Set setList { get; private set; }
        public List<bool> isFavorite { get; private set; }
        AndroidX.Fragment.App.FragmentTransaction trans { get; set; }
        public int cnt { get; set; }
        public bool Showingback { get; set; }
        #endregion

        #region Commands
        public RelayCommand LoadMore { get; set; }
        public Button Previous { get; set; }
        public Button Next { get; set; }
        public Button Play { get; set; }
        public RelayCommand Shuffle { get; set; }
        public RelayCommand Flip { get; set; }
        public ProgressBar cardProgress { get; set; }
        public ProgressBar ProgressCircle { get; set; }
        public TextView ProgressText { get; set; }
        public FrameLayout frameTokCardMini { get; set; }
        public Tokket.Android.TokCardsMiniGameActivity activity { get; set; }
        public GestureDetector gesturedetector { get; set; }
        public HomePageViewModel HomeVm => App.Locator.HomePageVM;
        object _lock = new object();
        public bool isStop { get; set; }
        public bool isPlayFavorite { get; set; }
        #endregion
        #region Constructors
        /// <summary>
        ///     Constructors will be called during the Registration in ViewModelLocator (Applying Dependency Injection or Inversion of Controls)
        /// </summary>
        public TokCardsPageViewModel()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 4000;

            isStop = true; //default is true

            if (Previous != null)
                Previous.Enabled = false;

            if (Play != null)
            {
                Play.Click -= OnPlayCardClick;
                Play.Click += OnPlayCardClick;
            }
        }
        #endregion
        #region Methods/Events
        public RelayCommand ShuffleClick
        {
            get
            {
                return Shuffle
                       ?? (Shuffle = new RelayCommand(
                           () =>
                           {
                               cnt = 0;
                               cardProgress.Progress = cnt + 1;
                               ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                               Previous.Enabled = false;
                               if (cnt < tokList.Count)
                               {
                                   Next.Enabled = true;
                               }

                               var joined = tokList.Zip(isFavorite, (x, y) => new { x, y });
                               var shuffled = joined.OrderBy(x => Guid.NewGuid()).ToList();
                               tokList = shuffled.Select(pair => pair.x).ToList();
                               isFavorite = shuffled.Select(pair => pair.y).ToList();

                               trans = activity.SupportFragmentManager.BeginTransaction();
                               trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
                               trans.AddToBackStack(null);
                               trans.Commit();
                           }));
            }
        }
        public RelayCommand OnFlipCard
        {
            get
            {
                return Flip
                       ?? (Flip = new RelayCommand(
                           () =>
                           {
                               FlipCard();
                           }));
            }
        }
        public RelayCommand OnLoadMore
        {
            get
            {
                return LoadMore
                       ?? (LoadMore = new RelayCommand(
                           async () =>
                           {
                               ProgressCircle.Visibility = ViewStates.Visible;
                               TokQueryValues tokQueryModel = new TokQueryValues();
                               tokQueryModel.token = Settings.ContinuationToken;
                               tokQueryModel.loadmore = "yes";
                               tokQueryModel.sortby = Settings.SortByFilter;
                               var tokresult = await TokService.Instance.GetAllToks(tokQueryModel);
                               tokList.AddRange(tokresult);
                               isFavorite.AddRange(Enumerable.Repeat(false, tokresult.Count).ToList());
                               cardProgress.Max = tokList.Count;
                               ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                               ProgressCircle.Visibility = ViewStates.Gone;
                           }));
            }
        }
        private void OnPlayCardClick(object sender, EventArgs e)
        {
            isStop = !isStop;
            PlayCard();
        }
        private void OnPreviousClick(object sender, EventArgs e)
        {
            Animation myAnim = AnimationUtils.LoadAnimation(activity, Resource.Animation.fab_scaleup);
            frameTokCardMini.StartAnimation(myAnim);
            cnt = cnt - 1;
            cardProgress.IncrementProgressBy(-1);
            ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;

            trans = activity.SupportFragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.frameTokCardMini, new CardFrontFragment());
            trans.AddToBackStack(null);
            trans.Commit();

            if (cnt == 0)
            {
                Previous.Enabled = false;
            }
            Next.Enabled = true;
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
                        favList.Add(tokList[i]);
                    }
                }

                cnt = 0;

                if (favList.Count != 0)
                {
                    loadFrontCard();
                }

                cardProgress.Progress = 1;
                cardProgress.Max = favList.Count;
                ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;

                if (favList.Count != 0)
                {
                    isStop = false; //to auto play
                    PlayCard();
                }
            }
            else
            {
                cnt = 0;
                cardProgress.Max = tokList.Count;
                cardProgress.IncrementProgressBy(1);
                ProgressText.Text = 1 + "/" + cardProgress.Max;
            }
        }
        private void PlayCard()
        {
            if (isStop) //If Stop
            {
                _timer.Stop();
                Play.Text = "play PIay";
                Play.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#007bff"));
            }
            else if (!isStop) //If Play
            {
                _timer.AutoReset = true;
                _timer.Enabled = true;
                _timer.Elapsed += OnTimeEvent;
                Play.Text = "stop St0p";
                Play.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#dc3545"));
            }

            if (cnt == 0)
            {
                Previous.Enabled = false;
            }

            if (cardProgress.Max > 1)
            {
                Next.Enabled = true;
            }
        }
        public async Task InitializeData()
        {
            ProgressCircle.Visibility = ViewStates.Visible;
            if (setList != null)
            {
                tokList = await GetSetToks();
            }
            else
            {
                tokList = await HomeVm.GetToksData("", (FilterType)Settings.FilterTag);
            }

            cardProgress.Max = tokList.Count;
            isFavorite = Enumerable.Repeat(false, tokList.Count).ToList();
            cardProgress.Progress = 1;
            ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
            if (tokList.Count <= 1)
            {
                Next.Enabled = false;
            }

            gesturedetector = new GestureDetector(activity, new MyGestureListener(activity));

            trans = activity.SupportFragmentManager.BeginTransaction();
            trans.Add(Resource.Id.frameTokCardMini, new CardFrontFragment());
            trans.Commit();
            ProgressCircle.Visibility = ViewStates.Gone;
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
        private void OnNextCard(object sender, EventArgs e)
        {
            if (cardProgress.Max > 1)
            {
                cnt = cnt + 1;
                loadFrontCard();

                cardProgress.IncrementProgressBy(1);
                ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                if (tokList.Count <= 1 || cardProgress.Progress == tokList.Count)
                {
                    Next.Enabled = false;
                }
                Previous.Enabled = true;
            }
        }
        public void loadFrontCard()
        {
            Animation myAnim = AnimationUtils.LoadAnimation(activity, Resource.Animation.fab_scaleup);
            frameTokCardMini.StartAnimation(myAnim);

            trans = activity.SupportFragmentManager.BeginTransaction();
            trans.Replace(frameTokCardMini.Id, new CardFrontFragment());
            trans.AddToBackStack(null);
            trans.CommitAllowingStateLoss();
            Showingback = false;

            ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
        }

        public void FlipCard()
        {
            if (Showingback)
            {
                activity.SupportFragmentManager.PopBackStack();
                Showingback = false;
            }
            else
            {
                AndroidX.Fragment.App.FragmentTransaction trans = activity.SupportFragmentManager.BeginTransaction();
                trans.SetCustomAnimations(Resource.Animation.card_flip_right_in, Resource.Animation.card_flip_right_out, Resource.Animation.card_flip_left_in, Resource.Animation.card_flip_left_out);
                trans.Replace(frameTokCardMini.Id, new CardBackFragment());
                trans.AddToBackStack(null);
                trans.Commit();
                Showingback = true;
            }
        }
        private void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            FlipCard();
            Thread.Sleep(3000);

            activity.RunOnUiThread(() =>
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
                    ProgressText.Text = cardProgress.Progress + "/" + cardProgress.Max;
                }
            }
        }
        public void removeFragmentinFL()
        {
            trans = activity.SupportFragmentManager.BeginTransaction();
            trans.Remove(new CardFrontFragment());
            trans.AddToBackStack(null);
            trans.CommitAllowingStateLoss();
        }
        #endregion
    }

    internal class CardFrontFragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            TokCardsMiniGameActivity tokCardsMiniGameActivity = Activity as TokCardsMiniGameActivity;
            View frontcard_view = inflater.Inflate(Resource.Layout.preview_cardfront, container, false);

            var linear_previewcardfront = frontcard_view.FindViewById<LinearLayout>(Resource.Id.linear_previewcardfront);
            var lblPreviewCardFront = frontcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardFront);

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
                if (tokCardsMiniGameActivity.cnt < tokList.Count)
                {
                    if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].PrimaryFieldText))
                    {
                        lblPreviewCardFront.Text = tokList[tokCardsMiniGameActivity.cnt].PrimaryFieldText;
                    }
                }
            }


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
    internal class CardBackFragment : AndroidX.Fragment.App.Fragment
    {
        public View backcard_view; public ImageView img_previewcardback;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            TokCardsMiniGameActivity tokCardsMiniGameActivity = Activity as TokCardsMiniGameActivity;
            backcard_view = inflater.Inflate(Resource.Layout.preview_cardback, container, false);
            img_previewcardback = backcard_view.FindViewById<ImageView>(Resource.Id.img_previewcardback);
            var linear_previewcardfront = backcard_view.FindViewById<LinearLayout>(Resource.Id.linear_previewcardback);
            var lblPreviewCardBack = backcard_view.FindViewById<TextView>(Resource.Id.lblPreviewCardBack);

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
                if (tokList[tokCardsMiniGameActivity.cnt].IsDetailBased == true)
                {
                    if (tokCardsMiniGameActivity.TokLists[tokCardsMiniGameActivity.cnt].Details != null)
                    {
                        string detailstr = "";
                        for (int i = 0; i < tokList[tokCardsMiniGameActivity.cnt].Details.Count(); i++)
                        {
                            if (tokList[tokCardsMiniGameActivity.cnt].Details[i] != null)
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
                        lblPreviewCardBack.Text = detailstr;
                    }
                }
                else
                {
                    lblPreviewCardBack.Text = tokList[tokCardsMiniGameActivity.cnt].SecondaryFieldText;
                }

                //if tok is image
                if (!string.IsNullOrEmpty(tokList[tokCardsMiniGameActivity.cnt].Image))
                {
                    string tokimg = tokList[tokCardsMiniGameActivity.cnt].Image + ".jpg";
                    Glide.With(tokCardsMiniGameActivity).Load(tokimg).Apply(RequestOptions.PlaceholderOf(Resource.Drawable.no_image).FitCenter()).Into(img_previewcardback);
                };
            }

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
    internal class MyGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private TokCardsMiniGameActivity mainActivity;
        public TokCardsPageViewModel TokCardsVm => App.Locator.TokCardsPageVM;
        public MyGestureListener(TokCardsMiniGameActivity Activity)
        {
            mainActivity = Activity;
        }

        public override bool OnSingleTapUp(MotionEvent e)
        {
            TokCardsVm.FlipCard();
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
}