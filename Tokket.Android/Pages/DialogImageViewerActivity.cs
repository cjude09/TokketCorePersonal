using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using ImageViews.Photo;
using Java.Interop;
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Tokket.Android.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Tokket.Android.Custom;
using Tokket.Core;
using Android.Webkit;
using static Android.App.ActionBar;

namespace Tokket.Android
{
    [Activity(Label = "DialogImageViewerActivity", Theme = "@style/Theme.Transparent", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class DialogImageViewerActivity : BaseActivity
    {
        ClassGroupModel classGroupModel;
        TokModel tokPicModel;
        GlideImgListener GListener; float imgScale = 0;
        ReportDialouge Report = null;
        internal static DialogImageViewerActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_dialog_image_viewer);
            this.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);

            Instance = this;
            Report = new ReportDialouge(this);
            var tokString = Intent.GetStringExtra("tokPicModel");
            if (tokString != null)
            {
                tokPicModel = JsonConvert.DeserializeObject<TokModel>(tokString);
            }

            var groupModelString = Intent.GetStringExtra("classGroupModel");
            if (!string.IsNullOrEmpty(groupModelString))
            {
                classGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(groupModelString);
            }

            ImgImageView.SetImageResource(0);
            if (URLUtil.IsValidUrl(Settings.byteImageViewer))
            {
                var GListenerCover = new GlideImgListener();
                GListenerCover.ParentActivity = this;

                Glide.With(this)
                    .Load(Settings.byteImageViewer)
                    .Listener(GListenerCover)
                    .Into(ImgImageView);

                ImgImageView.LayoutChange += delegate
                {
                    relateiveBackgroundImage.SetBackgroundColor(ManipulateColor.manipulateColor(GListenerCover.mColorPalette, 0.32f));

                };
            }
            else
            {
                //Convert to jsonObject because if byte is too large, it will not open the activity
                var imageIntent = JsonConvert.DeserializeObject<byte[]>(Settings.byteImageViewer);
                Bitmap bitmap = BitmapFactory.DecodeByteArray(imageIntent, 0, imageIntent.Length);
                ImgImageView.SetImageBitmap(bitmap);

                GListener = new GlideImgListener();
                GListener.ParentActivity = this;

                relateiveBackgroundImage.SetBackgroundColor(GetDominantColor.GetDominantColorImg(((BitmapDrawable)ImgImageView.Drawable).Bitmap, 0.62f));

                imgScale = ImgImageView.Scale;
            }

            tokPicMenu();

            btnClose.Click += delegate
            {
                Settings.byteImageViewer = string.Empty;
                closeActivity();
            };
        }
        public void closeActivity()
        {
            //always set Settings.byteImageViewer to empty when the activity closes
            Settings.byteImageViewer = "";

            this.Finish();
        }
        private void tokPicMenu()
        {
            if (tokPicModel != null)
            {
                if (!string.IsNullOrEmpty(tokPicModel.PrimaryFieldText))
                {
                    txtImageViewDateLocation.Visibility = ViewStates.Visible;
                    txtImageViewDateLocation.Text = tokPicModel.DateCreated + " - " + tokPicModel.UserCountry;

                    txtImageViewCaption.Visibility = ViewStates.Visible;
                    txtImageViewCaption.Text = tokPicModel.PrimaryFieldText;
                };
                lblImageViewerMenu.Visibility = ViewStates.Visible;
            }

            lblImageViewerMenu.Click += delegate
            {
                PopupMenu menu = new PopupMenu(this, lblImageViewerMenu);

                // Call inflate directly on the menu:
                menu.Inflate(Resource.Menu.class_tokpic_menu);

                var edit = menu.Menu.FindItem(Resource.Id.itemEdit);
                var delete = menu.Menu.FindItem(Resource.Id.itemDelete);
                var report = menu.Menu.FindItem(Resource.Id.itemReport) ;

                edit.SetVisible(false);
                delete.SetVisible(false);

                if (tokPicModel.UserId == Settings.GetUserModel().UserId)
                {
                    edit.SetVisible(true);
                    delete.SetVisible(true);
                    report.SetVisible(false);
                }
                else
                {
                    if (classGroupModel != null)
                    {
                        if (!string.IsNullOrEmpty(tokPicModel.GroupId) && classGroupModel.UserId == Settings.GetUserModel().UserId)
                        {
                            delete.SetVisible(true);
                        };
                    }
                    report.SetVisible(true);
                }

                // A menu item was clicked:
                menu.MenuItemClick += async (s1, arg1) => {
                    switch (arg1.Item.TitleFormatted.ToString().ToLower())
                    {
                        case "edit":
                            ClassGroupActivity.Instance.showAddPicDialog(false, ImgImageView.Drawable, tokPicModel);
                            closeActivity();
                            break;
                        case "delete":
                            LinearProgress.Visibility = ViewStates.Visible;
                            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                            var result = await TokService.Instance.DeleteTokAsync(tokPicModel.Id, tokPicModel.PartitionKey);

                            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                            LinearProgress.Visibility = ViewStates.Gone;

                            classgroup_pics_fragmentt.Instance.removeTok(tokPicModel);

                            closeActivity();

                            break;
                        case "report":
                            Report.Show();
                            break;
                    }
                };

                // Menu was dismissed:
                menu.DismissEvent += (s2, arg2) => {
                    //Console.WriteLine("menu dismissed");
                };

                menu.Show();
            };
        }

        [Java.Interop.Export("onRadioButtonClicked")]
        public void onRadioButtonClicked(View view)
        {
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
                r.ItemId = tokPicModel.Id;
                r.ItemLabel = tokPicModel.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = tokPicModel.UserId;
                r.UserId = Settings.GetTokketUser()?.Id;

                var result = await ReactionService.Instance.AddReport(r);

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    alert.SetTitle("Report Successful!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                else
                {
                    alert.SetTitle("Report Failed!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                alert.Show();
            }
            else
            {
                alert.SetTitle("Select a reason for the report!");
                alert.Show();
            }
            Report.ReportProgress.Visibility = ViewStates.Gone;
        }
        // Allows us to know if we should use MotionEvent.ACTION_MOVE
        private bool tracking = false;
        // The Position where our touch event started
        private float startY;
        private int mSlop;
        private float mDownX;
        private float mDownY;
        private bool mSwiping;
        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            switch (ev.Action)
            {
                case MotionEventActions.Cancel:
                    tracking = false;
                    break;
                case MotionEventActions.Down:
                    mDownX = ev.GetX();
                    mDownY = ev.GetY();
                    mSwiping = false;

                    Rect hitRect = new Rect();
                    LinearImageView.GetHitRect(hitRect);

                    if (hitRect.Contains((int)ev.GetX(), (int)ev.GetY()))
                    {
                        tracking = true;
                    }
                    startY = ev.GetY();
                    break;
                case MotionEventActions.Move:
                    if (ImgImageView.Scale == imgScale)
                    {
                        float x = ev.GetX();
                        float y = ev.GetY();
                        float xDelta = Math.Abs(x - mDownX);
                        float yDelta = Math.Abs(y - mDownY);

                        if (yDelta > mSlop && yDelta / 2 > xDelta)
                        {
                            mSwiping = true;
                            //return true;
                        }


                        if (ev.GetY() - startY > 1)
                        {
                            if (tracking)
                            {
                                LinearImageView.TranslationY = ev.GetY() - startY;
                            }
                            animateSwipeView(LinearImageView.Height);
                        }
                        else if (startY - ev.GetY() > 1)
                        {
                            if (tracking)
                            {
                                LinearImageView.TranslationY = ev.GetY() - startY;
                            }
                            animateSwipeView(LinearImageView.Height);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    if (mSwiping)
                    {
                        if (LinearImageView.Visibility == ViewStates.Visible)
                        {
                            int quarterHeight = LinearImageView.Height / 4;
                            float currentPosition = LinearImageView.TranslationY;
                            if (currentPosition < -quarterHeight)
                            {
                                closeUpAndDismissDialog((int)currentPosition);
                            }
                            else if (currentPosition > quarterHeight)
                            {
                                closeDownAndDismissDialog((int)currentPosition);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return base.DispatchTouchEvent(ev);
        }
        private void animateSwipeView(int parentHeight)
        {
            int quarterHeight = parentHeight / 4;
            float currentPosition = LinearImageView.TranslationY;
            float animateTo = 0.0f;
            if (currentPosition < -quarterHeight)
            {
                animateTo = -parentHeight;
            }
            else if (currentPosition > quarterHeight)
            {
                animateTo = parentHeight;
            }
            ObjectAnimator.OfFloat(LinearImageView, "translationY", currentPosition, animateTo)
                    .SetDuration(200)
                    .Start();
        }

        private void closeUpAndDismissDialog(int currentPosition)
        {
            var swipeListener = new SwipeListener(this);

            ObjectAnimator positionAnimator = ObjectAnimator.OfFloat(LinearImageView, "translationY", currentPosition, -LinearImageView.Height);
            positionAnimator.SetDuration(300);
            positionAnimator.AddListener(swipeListener);
            positionAnimator.Start();
        }

        private void closeDownAndDismissDialog(int currentPosition)
        {
            Display display = WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);
            int screenHeight = size.Y;

            var swipeListener = new SwipeListener(this);

            ObjectAnimator positionAnimator = ObjectAnimator.OfFloat(LinearImageView, "translationY", currentPosition, screenHeight + LinearImageView.Height);
            positionAnimator.SetDuration(300);
            positionAnimator.AddListener(swipeListener);
            positionAnimator.Start();
        }

        public LinearLayout LinearImageView => FindViewById<LinearLayout>(Resource.Id.LinearImageView);
        public TextView txtImageViewDateLocation => FindViewById<TextView>(Resource.Id.txtImageViewDateLocation);
        public PhotoView ImgImageView => FindViewById<PhotoView>(Resource.Id.ImgImageView);
        public TextView lblImageViewerMenu => FindViewById<TextView>(Resource.Id.lblImageViewerMenu);
        public TextView txtImageViewCaption => FindViewById<TextView>(Resource.Id.txtImageViewCaption);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
        public TextView TextProgressStatus => FindViewById<TextView>(Resource.Id.TextProgressStatus);
        public Button btnClose => FindViewById<Button>(Resource.Id.btnClose);
        public RelativeLayout relateiveBackgroundImage => FindViewById<RelativeLayout>(Resource.Id.relateiveBackgroundImage);
    }

    class SwipeListener: Java.Lang.Object, Animator.IAnimatorListener
    {
        Activity activity;
        public SwipeListener(Activity _activity)
        {
            activity = _activity;
        }
        public void OnAnimationCancel(Animator animation)
        {
            Console.WriteLine("OnAnimationCancel");
        }

        public void OnAnimationEnd(Animator animation)
        {
            //Always set Settings.byteImageViewer to empty when the activity closes
            Settings.byteImageViewer = "";
            this.activity.Finish();
        }

        public void OnAnimationRepeat(Animator animation)
        {
            Console.WriteLine("");
        }

        public void OnAnimationStart(Animator animation)
        {
            Console.WriteLine("OnAnimationStart");
        }
    }
}