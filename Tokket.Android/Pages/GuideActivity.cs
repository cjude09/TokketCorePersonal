using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Resource;
using Animation = Android.Views.Animations.Animation;

namespace Tokket.Android
{
    [Activity(Label = "Guide", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class GuideActivity : BaseActivity
    {
        int guideSequence = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
         
            SetContentView(Resource.Layout.guide_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);


        
            D1.Alpha = 0;
            D2.Alpha = 0;
            D3.Alpha = 0;
            TSG1.Alpha = 0;
            TSG2.Alpha = 0;
            TSG3.Alpha = 0;
            InitAnimation(TSG1,1500);
        }

        public Animation SetAnimationFade(long durataion) {
            var fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Interpolator = new DecelerateInterpolator(); //add this
            fadeIn.Duration = durataion;

            AnimationSet animation = new AnimationSet(false); //change to false
            animation.AddAnimation(fadeIn);
            
            return animation;
        }

        public async void InitAnimation(View view, long duration) {
            var anim1 = SetAnimationFade(duration);
            anim1.AnimationEnd += AnimationEnds;
            view.Alpha = 1;
            view.StartAnimation(anim1);
        }

        private void AnimationEnds(object sender, Animation.AnimationEndEventArgs e)
        {
            switch (guideSequence) {
                case 1:
                    InitAnimation(D1, 1500); break;
                case 2:
                    InitAnimation(TSG2, 1500);
                    break;
                case 3:
                    InitAnimation(D2, 1500);
                    break;
                case 4:
                    InitAnimation(TSG3, 1500);
                    break;
                case 5:
                    InitAnimation(D3, 1500);
                    break;
              

            }
            guideSequence++;
          

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

        public ImageView TSG1 => FindViewById<ImageView>(Resource.Id.img_tokstargirl);

        public ImageView TSG2 => FindViewById<ImageView>(Resource.Id.img_tokstargirl1);

        public ImageView TSG3 => FindViewById<ImageView>(Resource.Id.img_tokstarboy1);
        public TextView D1 => FindViewById<TextView>(Resource.Id.txtguide1);
        public TextView D2 => FindViewById<TextView>(Resource.Id.txtguide2);
        public TextView D3 => FindViewById<TextView>(Resource.Id.txtguide3);
    }
}