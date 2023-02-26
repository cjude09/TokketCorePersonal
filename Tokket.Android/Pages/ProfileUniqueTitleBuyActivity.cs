using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "ProfileUniqueTitleBuyActivity", Theme = "@style/CustomAppThemeBlue")]
    public class ProfileUniqueTitleBuyActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profileuniquetitlefragment_page);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.profileunique_toolbar);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
         //   SetSupportActionBar(tokback_toolbar);
            
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            BuyButton.Click += OnBuyClicked;
            // Create your application here
        }
        private async void OnBuyClicked(object sender, EventArgs e)
        {
            LinearProgress.Visibility = ViewStates.Visible;
            var item = PurchasesTool.GetProduct("title_tokket");
            var billing = await PurchaseService.Instance.BillingStart(item.Id, item, titleId: EntryTitle.Text,isUnique: true);
            if (billing)
            {
                LinearProgress.Visibility = ViewStates.Gone;
                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                var objBuilder = new AlertDialog.Builder(this);
                objBuilder.SetTitle("");
                objBuilder.SetMessage("Title Purchased Complete!");
                objBuilder.SetCancelable(false);

                AlertDialog objDialog = objBuilder.Create();
                objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                {
                    SetResult(AppResult.Ok);
                    this.Finish();

                });
                objDialog.Show();
            }
            else
            {
                LinearProgress.Visibility = ViewStates.Gone;
                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                var objBuilder = new AlertDialog.Builder(this);
                objBuilder.SetTitle("");
                objBuilder.SetMessage("Title Purchase Failed");
                objBuilder.SetCancelable(false);

                AlertDialog objDialog = objBuilder.Create();
                objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                {

                    this.Finish();

                });
                objDialog.Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
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
        public Button BuyButton => FindViewById<Button>(Resource.Id.BuyButtonUnique);

        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
        public EditText EntryTitle => FindViewById<EditText>(Resource.Id.entryTitle);
    }
}