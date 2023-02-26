using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using DE.Hdodenhof.CircleImageViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.ViewModels;

namespace Tokket.Android
{
    [Activity(Label = "Add Class Tok", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class GuideAddClassTokActivity : BaseActivity
    {
        GestureDetector gesturedetector;
        int guideSequence = 1;
        long duration = 4000;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_guide_add_class_tok);

            BtnCancelTok.Click += (object sender, EventArgs e) =>
            {
                this.Finish();
            };

            /*imgGuide1.Alpha = 0;
            txt_dialogue1.Alpha = 0;

            imageViewArrowGuide2.Alpha = 0;
            imgGuide2.Alpha = 0;
            txt_dialogue2.Alpha = 0;

            imageViewArrowGuide3.Alpha = 0;
            imgGuide3.Alpha = 0;
            txt_dialogue3.Alpha = 0;

            imageViewArrowGuide4.Alpha = 0;
            imgGuide4.Alpha = 0;
            txt_dialogue4.Alpha = 0;

            imageViewArrowGuide5.Alpha = 0;
            imgGuide5.Alpha = 0;
            txt_dialogue5.Alpha = 0;

            imageViewArrowGuide6.Alpha = 0;
            imgGuide6.Alpha = 0;
            txt_dialogue6.Alpha = 0;*/

            imgGuide1.Visibility = ViewStates.Visible;
            txt_dialogue1.Visibility = ViewStates.Visible;
            StartAnimation(txt_dialogue1, 1);
            guideSequence += 1;

            gesturedetector = new GestureDetector(this, new MyGestureListener(this));
            viewTapCancel.Touch -= guideParentViewTouched;
            viewTapCancel.Touch += guideParentViewTouched;
        }

        private void guideParentViewTouched(object sender, View.TouchEventArgs e)
        {
            gesturedetector.OnTouchEvent(e.Event);
        }

        public Animation SetAnimationFade(long durataion)
        {
            var fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Interpolator = new DecelerateInterpolator(); //add this
            fadeIn.Duration = durataion;

            AnimationSet animation = new AnimationSet(false); //change to false
            animation.AddAnimation(fadeIn);

            return animation;
        }

        public void InitAnimation(View view)
        {
            var anim1 = SetAnimationFade(duration);
            view.Alpha = 1;
            view.StartAnimation(anim1);
        }

        public void InitBlinkAnimation(View view)
        {
            view.Alpha = 1;
            Animation animation = new AlphaAnimation(1, 0); //to change visibility from visible to invisible 
            animation.Duration = 600; //1 second duration for each animation cycle 
            animation.Interpolator = new LinearInterpolator();
            animation.RepeatCount = Animation.Infinite; //repeating indefinitely 
            animation.RepeatMode = RepeatMode.Reverse; //animation will start from end point once ended.
            view.StartAnimation(animation); //to start animation
        }

        private void StartAnimation(View v, int currentSequence)
        {
            v.Alpha = 1;

            var guidelineAnimation = new GuidelineAnimation(this);

            ObjectAnimator positionAnimator = ObjectAnimator.OfFloat(v, "alpha", 0.25f, 1, 1);
            positionAnimator.SetDuration(duration);
            positionAnimator.AddListener(guidelineAnimation);
            positionAnimator.Start();
        }

        private void showAllGuides()
        {
            //Show all after all animation ends
            guideSequence = 10; //set beyond the limit;
            imgGuide1.ClearAnimation();
            imgGuide1.Visibility = ViewStates.Visible;
            txt_dialogue1.ClearAnimation();
            txt_dialogue1.Visibility = ViewStates.Visible;

            imageViewArrowGuide2.ClearAnimation();
            imageViewArrowGuide2.Visibility = ViewStates.Invisible;
            imgGuide2.ClearAnimation();
            imgGuide2.Visibility = ViewStates.Visible;
            txt_dialogue2.ClearAnimation();
            txt_dialogue2.Visibility = ViewStates.Visible;

            imgGuide3.ClearAnimation();
            imgGuide3.Visibility = ViewStates.Visible;
            txt_dialogue3.ClearAnimation();
            txt_dialogue3.Visibility = ViewStates.Visible;
            imageViewArrowGuide3.ClearAnimation();
            imageViewArrowGuide3.Visibility = ViewStates.Invisible;

            imgGuide4.ClearAnimation();
            imgGuide4.Visibility = ViewStates.Visible;
            txt_dialogue4.ClearAnimation();
            txt_dialogue4.Visibility = ViewStates.Visible;
            imageViewArrowGuide4.ClearAnimation();
            imageViewArrowGuide4.Visibility = ViewStates.Invisible;

            imgGuide5.ClearAnimation();
            imgGuide5.Visibility = ViewStates.Visible;
            txt_dialogue5.ClearAnimation();
            txt_dialogue5.Visibility = ViewStates.Visible;
            imageViewArrowGuide5.ClearAnimation();
            imageViewArrowGuide5.Visibility = ViewStates.Invisible;

            imgGuide6.ClearAnimation();
            imgGuide6.Visibility = ViewStates.Visible;
            txt_dialogue6.ClearAnimation();
            txt_dialogue6.Visibility = ViewStates.Visible;
            imageViewArrowGuide6.ClearAnimation();
            imageViewArrowGuide6.Visibility = ViewStates.Invisible;
        }

        private class GuidelineAnimation : Java.Lang.Object, Animator.IAnimatorListener
        {
            GuideAddClassTokActivity activity;
            public GuidelineAnimation(GuideAddClassTokActivity _activity)
            {
                activity = _activity;
            }
            public void OnAnimationCancel(Animator animation)
            {
                Console.WriteLine("OnAnimationCancel");
            }

            public void OnAnimationEnd(Animator animation)
            {
                switch (activity.guideSequence)
                {
                    case 1:
                        /* imgGuide1.Visibility = ViewStates.Visible;
                         txt_dialogue1.Visibility = ViewStates.Visible;

                         InitAnimation(imgGuide1, 1500);
                         InitAnimation(txt_dialogue1, 1500);*/
                        break;
                    case 2:
                        activity.imgGuide1.Visibility = ViewStates.Invisible;
                        activity.txt_dialogue1.Visibility = ViewStates.Invisible;

                        activity.imageViewArrowGuide2.Visibility = ViewStates.Visible;

                        activity.imgGuide2.Visibility = ViewStates.Visible;
                        activity.txt_dialogue2.Visibility = ViewStates.Visible;

                        activity.StartAnimation(activity.txt_dialogue2, 2);
                        break;
                    case 3:
                        activity.imageViewArrowGuide2.ClearAnimation();
                        activity.imageViewArrowGuide2.Visibility = ViewStates.Invisible;
                        activity.imgGuide2.Visibility = ViewStates.Invisible;
                        activity.txt_dialogue2.Visibility = ViewStates.Invisible;

                        activity.imageViewArrowGuide3.Visibility = ViewStates.Visible;

                        activity.txt_dialogue3.Visibility = ViewStates.Visible;
                        activity.imgGuide3.Visibility = ViewStates.Visible;

                        activity.StartAnimation(activity.txt_dialogue3, 3);

                        break;
                    case 4:
                        activity.imageViewArrowGuide3.ClearAnimation();
                        activity.imageViewArrowGuide3.Visibility = ViewStates.Invisible;
                        activity.txt_dialogue3.Visibility = ViewStates.Invisible;
                        activity.imgGuide3.Visibility = ViewStates.Invisible;

                        activity.imageViewArrowGuide4.Visibility = ViewStates.Visible;

                        activity.txt_dialogue4.Visibility = ViewStates.Visible;
                        activity.imgGuide4.Visibility = ViewStates.Visible;

                        activity.StartAnimation(activity.txt_dialogue4, 4);

                        break;
                    case 5:
                        activity.imageViewArrowGuide4.ClearAnimation();
                        activity.imageViewArrowGuide4.Visibility = ViewStates.Invisible;
                        activity.txt_dialogue4.Visibility = ViewStates.Invisible;
                        activity.imgGuide4.Visibility = ViewStates.Invisible;

                        activity.imageViewArrowGuide5.Visibility = ViewStates.Visible;

                        activity.txt_dialogue5.Visibility = ViewStates.Visible;
                        activity.imgGuide5.Visibility = ViewStates.Visible;

                        activity.StartAnimation(activity.txt_dialogue5, 5);

                        break;
                    case 6:
                        activity.imageViewArrowGuide5.ClearAnimation();
                        activity.imageViewArrowGuide5.Visibility = ViewStates.Invisible;
                        activity.txt_dialogue5.Visibility = ViewStates.Invisible;
                        activity.imgGuide5.Visibility = ViewStates.Invisible;

                        activity.imageViewArrowGuide6.Visibility = ViewStates.Visible;

                        activity.txt_dialogue6.Visibility = ViewStates.Visible;
                        activity.imgGuide6.Visibility = ViewStates.Visible;

                        activity.StartAnimation(activity.txt_dialogue6, 6);

                        break;
                    default:
                        activity.showAllGuides();
                        break;

                }
                activity.guideSequence++;
            }

            public void OnAnimationStart(Animator animation)
            {
                switch (activity.guideSequence)
                {
                    case 1:
                        activity.imgGuide1.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide1);
                        break;
                    case 2:
                        activity.imageViewArrowGuide2.Visibility = ViewStates.Visible;
                        activity.imgGuide2.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide2);
                        activity.InitBlinkAnimation(activity.imageViewArrowGuide2);
                        break;
                    case 3:
                        activity.imageViewArrowGuide3.Visibility = ViewStates.Visible;
                        activity.txt_dialogue3.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide3);
                        activity.InitBlinkAnimation(activity.imageViewArrowGuide3);

                        break;
                    case 4:
                        activity.imageViewArrowGuide4.Visibility = ViewStates.Visible;
                        activity.imgGuide4.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide4);
                        activity.InitBlinkAnimation(activity.imageViewArrowGuide4);
                        break;
                    case 5:
                        activity.imageViewArrowGuide5.Visibility = ViewStates.Visible;
                        activity.imgGuide5.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide5);
                        activity.InitBlinkAnimation(activity.imageViewArrowGuide5);
                        break;
                    case 6:
                        activity.imageViewArrowGuide6.Visibility = ViewStates.Visible;
                        activity.imgGuide6.Visibility = ViewStates.Visible;

                        activity.InitAnimation(activity.imgGuide6);
                        activity.InitBlinkAnimation(activity.imageViewArrowGuide6);
                        break;
                }

                Console.WriteLine("OnAnimationStart");
            }

            public void OnAnimationRepeat(Animator animation)
            {
                Console.WriteLine("");
            }
        }

        
        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private GuideAddClassTokActivity activity;
            public MyGestureListener(GuideAddClassTokActivity Activity)
            {
                activity = Activity;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                activity.showAllGuides();
                return true;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {

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

        public View viewTapCancel => FindViewById<View>(Resource.Id.viewTapCancel);
        public TextView BtnCancelTok => FindViewById<TextView>(Resource.Id.btnAddClassTokCancel);
        public CircleImageView imgGuide1 => FindViewById<CircleImageView>(Resource.Id.imgGuide1);
        public TextView txt_dialogue1 => FindViewById<TextView>(Resource.Id.txt_dialogue1);
        public TextView txt_dialogue2 => FindViewById<TextView>(Resource.Id.txt_dialogue2);
        public ImageView imageViewArrowGuide2 => FindViewById<ImageView>(Resource.Id.imageViewArrowGuide2);
        public CircleImageView imgGuide2 => FindViewById<CircleImageView>(Resource.Id.imgGuide2);
        public ImageView imageViewArrowGuide3 => FindViewById<ImageView>(Resource.Id.imageViewArrowGuide3);
        public TextView txt_dialogue3 => FindViewById<TextView>(Resource.Id.txt_dialogue3);
        public CircleImageView imgGuide3 => FindViewById<CircleImageView>(Resource.Id.imgGuide3);
        public ImageView imageViewArrowGuide4 => FindViewById<ImageView>(Resource.Id.imageViewArrowGuide4);
        public TextView txt_dialogue4 => FindViewById<TextView>(Resource.Id.txt_dialogue4);
        public CircleImageView imgGuide4 => FindViewById<CircleImageView>(Resource.Id.imgGuide4);
        public ImageView imageViewArrowGuide5 => FindViewById<ImageView>(Resource.Id.imageViewArrowGuide5);
        public TextView txt_dialogue5 => FindViewById<TextView>(Resource.Id.txt_dialogue5);
        public CircleImageView imgGuide5 => FindViewById<CircleImageView>(Resource.Id.imgGuide5);
        public ImageView imageViewArrowGuide6 => FindViewById<ImageView>(Resource.Id.imageViewArrowGuide6);
        public TextView txt_dialogue6 => FindViewById<TextView>(Resource.Id.txt_dialogue6);
        public CircleImageView imgGuide6 => FindViewById<CircleImageView>(Resource.Id.imgGuide6);
    }
}