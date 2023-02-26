using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Bumptech.Glide;
using ImageViews.Photo;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using static Android.Views.View;
using AlertDialog = Android.App.AlertDialog;
using HelperResult = Tokket.Shared.Helpers.Result;

namespace Tokket.Android
{
    [Activity(Label = "")]
    public class ModalImageViewerActivity : AppCompatActivity, View.IOnTouchListener
    {
        // Allows us to know if we should use MotionEvent.ACTION_MOVE
        private bool tracking = false;
        // The Position where our touch event started
        private float startY;
        View customView; View DummyView; GestureDetector gesturedetector;
        PhotoView ImgViewer;
        internal static ModalImageViewerActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.modal_imageviewer);
            customView = LayoutInflater.Inflate(Resource.Layout.modal_imageviewer, null);
            Instance = this;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetView(customView);

            builder.Create();
            builder.Show();

            ImgViewer = customView.FindViewById<PhotoView>(Resource.Id.ImgProfileImageView);
            DummyView = customView.FindViewById<View>(Resource.Id.ViewDummyForTouch);

            int ImgInt = Intent.GetIntExtra("Image",-1); //If 0 = UserPhoto, 1 = CoverPhoto
            if (ImgInt == 0)
            {
                if (Settings.ActivityInt == (int)ActivityType.ProfileActivity)
                {
                    ImgViewer.SetImageDrawable(ProfileUserActivity.Instance.ProfileUserPhoto.Drawable);
                }
                else if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
                {
                    ImgViewer.SetImageDrawable(ProfileFragment.Instance.ProfileUserPhoto.Drawable);
                }
            }
            else if (ImgInt == 1)
            {
                if (Settings.ActivityInt == (int)ActivityType.ProfileActivity)
                {
                    ImgViewer.SetImageDrawable(ProfileUserActivity.Instance.ProfileCoverPhoto.Drawable);
                }
                else if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
                {
                    ImgViewer.SetImageDrawable(ProfileFragment.Instance.ProfileCoverPhoto.Drawable);
                }
            }
            gesturedetector = new GestureDetector(this, new MyGestureListener(this));
            //DummyView.Touch += ImgTouchEvent;
            ImgViewer.SetOnTouchListener(this);
        }
        void ImgTouchEvent (object sender, TouchEventArgs e)
        {
            gesturedetector.OnTouchEvent(e.Event);
        }
        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private ModalImageViewerActivity ImgActivity;

            public MyGestureListener(ModalImageViewerActivity Activity)
            {
                ImgActivity = Activity;
            }
            public override bool OnDown(MotionEvent e)
            {
                Rect hitRect = new Rect();
                ImgActivity.ImgViewer.GetHitRect(hitRect);

                if (hitRect.Contains((int)e.GetX(), (int)e.GetY()))
                {
                    ImgActivity.tracking = true;
                }
                ImgActivity.startY = e.GetY();
                return true;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                return false;
            }
            public override bool OnDoubleTap(MotionEvent e)
            {
                //enable zooming of image
                return false;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                if (e2.GetY() - e1.GetY() > 1)
                {
                    if (ImgActivity.tracking)
                    {
                        ImgActivity.ImgViewer.TranslationY = e2.GetY() - e1.GetY();
                    }
                    ImgActivity.animateSwipeView(ImgActivity.ImgViewer.Height);
                }
                else
                {
                    return false;
                }
                return true;

                //float distanceX = e2.GetX() - e1.GetX();
                //float distanceY = e2.GetY() - e1.GetY();
                //if (Math.Abs(distanceX) > Math.Abs(distanceY) && Math.Abs(distanceX) > 100 && Math.Abs(velocityX) > 100)
                //{
                //    if (distanceX > 0)
                //        ImgActivity.ViewState = ViewImgState.Swipe;
                //    return true;
                //}
                //return false;
            }
            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                return false;
            }
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            gesturedetector.OnTouchEvent(e);

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    Rect hitRect = new Rect();
                    customView.GetHitRect(hitRect);

                    if (hitRect.Contains((int)e.GetX(), (int)e.GetY()))
                    {
                        tracking = true;
                    }
                    startY = e.GetY();
                    return true;
                    //case MotionEventActions.Cancel:
                    //    tracking = false;
                    //    //animateSwipeView(customView.Height);
                    //    return true;
                    //case MotionEventActions.Move:
                    //    if (e.GetY() - startY > 1)
                    //    {
                    //        if (tracking)
                    //        {
                    //            customView.TranslationY = e.GetY() - startY;
                    //        }
                    //        animateSwipeView(customView.Height);
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }

                    //    return true;
            }

            return false;
        }
        private void animateSwipeView(int parentHeight)
        {
            int quarterHeight = parentHeight / 4;
            float currentPosition = ImgViewer.TranslationY;
            float animateTo = 0.0f;
            if (currentPosition < -quarterHeight)
            {
                animateTo = -parentHeight;
            }
            else if (currentPosition > quarterHeight)
            {
                animateTo = parentHeight;
            }
            ObjectAnimator.OfFloat(ImgViewer, "translationY", currentPosition, animateTo)
                    .SetDuration(200)
                    .Start();
        }
        //public PhotoView ImgViewer => this.FindViewById<PhotoView>(Resource.Id.ImgProfileImageView);
    }
}