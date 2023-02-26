using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.ViewPager.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.ViewModels;
using Tokket.Shared.Models;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Presentation", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullSensor)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Presentation", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullSensor)]
#endif
    //NOTE -> This needs to be changed because it is using an old view that is not compatible in net6
    public class TokPakPresentationActivity : BaseActivity
    {
        List<ClassTokModel> classTokModelList;
        internal static TokPakPresentationActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tok_pak_presentation);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;
            classTokModelList = new List<ClassTokModel>();

            var classtokModelString = Intent.GetStringExtra("classtokModel");
            var modelList = JsonConvert.DeserializeObject<List<ClassTokModel>>(classtokModelString);
           
            classTokModelList.AddRange(modelList);
            var title = Intent.GetStringExtra("TitleActivity");
            Title =  title;

            var tokPakAdapter = new TokPakPresentationPagerAdapter(this, classTokModelList);
            viewPagerPresentation.Adapter = tokPakAdapter;
            //viewPagerPresentation.reset() //In order to reset the current page position
            /*circlePageIndicator.SetSnap(true);
            circlePageIndicator.SetViewPager(viewPagerPresentation);*/

            btnLeft.Click += delegate
            {
                /*var position = circlePageIndicator.mCurrentPage;
                circlePageIndicator.SetCurrentItem(position - 1);*/
            };

            btnRight.Click += delegate
            {
                /*var position = circlePageIndicator.mCurrentPage;
                circlePageIndicator.SetCurrentItem(position + 1);*/
            };

           /* viewPagerPresentation.IndicatorProgress += delegate
            {
                if (circlePageIndicator.mCurrentPage == 0)
                {
                    return;
                }
                txtPresentationCount.Text = circlePageIndicator.mCurrentPage + " / " + classTokModelList.Count;
            };*/

            txtPresentationCount.Text = "1 / " + classTokModelList.Count; //Set default position 1;
        }
/*        protected override void OnResume()
        {
            base.OnResume();
            viewPagerPresentation.ResumeAutoScroll();
        }
        protected override void OnPause()
        {
            viewPagerPresentation.PauseAutoScroll();
            base.OnPause();
        }*/
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
        public ViewPager viewPagerPresentation => FindViewById<ViewPager>(Resource.Id.viewPagerPresentation);
        //public InfiniteCirclePageIndicator circlePageIndicator => FindViewById<InfiniteCirclePageIndicator>(Resource.Id.circlePageIndicator);
        public TextView txtPresentationCount => FindViewById<TextView>(Resource.Id.txtPresentationCount);
        public AppCompatImageButton btnLeft => FindViewById<AppCompatImageButton>(Resource.Id.btnLeft);
        public AppCompatImageButton btnRight => FindViewById<AppCompatImageButton>(Resource.Id.btnRight);
    }
}