using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services;

namespace Tokket.Android
{
    [Activity(Label = "ProfileRoyaltyTitleBuyActivity", Theme = "@style/CustomAppThemeBlue")]
    public class ProfileRoyaltyTitleBuyActivity : BaseActivity
    {
        string[] RoyaltySelection = { "Select a royalty:", "King", "Queen", "Prince", "Princess", "Duke of", "Duchess of" };
        string[] SeparatorsSelection = { "Select a separator:", "Space", "Underscore", "Dash" };
        List<string> placeholder = new List<string>() { "","","" };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profileroyaltytitlefragment_page);

            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.profileroyalty_toolbar);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
         //   SetSupportActionBar(tokback_toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);


            ArrayAdapter<string> royatyadapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, RoyaltySelection);
            royatyadapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            ArrayAdapter<string> separatorAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, SeparatorsSelection);
            separatorAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            RoyalySpinner.Adapter = royatyadapter;
            SeparatorSpinner.Adapter = separatorAdapter;
            ResultText.SetRawInputType(InputTypes.Null);

            RoyalySpinner.ItemSelected += RoyaltySelected;
            SeparatorSpinner.ItemSelected += SeparatorSelected;
            SelectedText.TextChanged += SelectedTextChange;
            BuyButton.Click += OnBuyClicked;
        }

        private void SelectedTextChange(object sender, TextChangedEventArgs e)
        {
            placeholder[2] = e.Text.ToString();
            ResultText.Text = string.Join("", placeholder);
        }

        private void SeparatorSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {

            switch (SeparatorSpinner.GetItemAtPosition(e.Position).ToString()) {
                case "Dash": placeholder[1] = "-"; break ;
                case "Space": placeholder[1] = " "; break;
                case "Underscore": placeholder[1] = "_"; break;
            }
            ResultText.Text = string.Join("", placeholder);
        }

        private void RoyaltySelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = sender as Spinner;
            placeholder[0] = RoyalySpinner.GetItemAtPosition(e.Position).ToString();
            ResultText.Text = string.Join("",placeholder) ;
        }

        private async void OnBuyClicked(object sender, EventArgs e)
        {
            LinearProgress.Visibility = ViewStates.Visible;
            var item = PurchasesTool.GetProduct("title_royalty_tokket");
            var billing = await PurchaseService.Instance.BillingStart(item.Id, item, titleId: ResultText.Text);
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
                    Intent.PutExtra("AddedTitle", ResultText.Text);
                    SetResult(Result.Ok,Intent);
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
        public Spinner RoyalySpinner => FindViewById<Spinner>(Resource.Id.RoyaltySpinner);

        public Spinner SeparatorSpinner => FindViewById<Spinner>(Resource.Id.SeparatorSpinner);

        public EditText ResultText => FindViewById<EditText>(Resource.Id.Resulttitle);

        public EditText SelectedText => FindViewById<EditText>(Resource.Id.SelectedTitle);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
        public Button BuyButton => FindViewById<Button>(Resource.Id.BuyButtonRoyalty);
    }
}