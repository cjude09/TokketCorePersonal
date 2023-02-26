using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services;
using Tokket.Core;
using SharedService = Tokket.Shared.Services;
using Android.Graphics;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "ProfileMembershipActivity")]
    public class ProfileMembershipActivity : BaseActivity
    {
        private TokketUser tokketUser;
        private DateTime? memberLastUpdate;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MembershipPage);


            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.membership_toolbar);
#if (_CLASSTOKS)
            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
#endif
            SetSupportActionBar(tokback_toolbar);

            tokketUser = Settings.GetTokketUser();

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.close_24px);
            SupportActionBar.SetTitle(Resource.String.royalty_title);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.WhiteSmoke));
            memberLastUpdate = tokketUser.MembershipLastUpdated;
            if (memberLastUpdate != null && !IsExpired(memberLastUpdate.Value))
            {
                MembershipState.Text = "Click the button below to upgrade to a royalty member";
                BuyButton.SetBackgroundColor(Color.Green);
                BuyButton.Text = "Purchase Royalty Membership ($4.99 per year)";
                DaysLeft.Visibility = ViewStates.Visible;
               
                DaysLeft.Text = $"{365 - DaysRemaining} days before membership expires";
            }
            else {
                MembershipState.Text = "Click the button below to upgrade to a royalty member";
                BuyButton.SetBackgroundColor(Color.Purple);
                BuyButton.Text = "Purchase Royalty Membership ($4.99 per year)";
            }

            BuyButton.Click += OnBuyClicked;
            //String benefits = "<b>1.)</b> One royalty title<br><b>2.)</b> Royalty badge<br><b/>3.)</b> Double the coins for posting toks";
            //NumberSection.SetText(Html.FromHtml(benefits));
        }

        private async void OnBuyClicked(object sender, EventArgs e)
        {
            LinearProgress.Visibility = ViewStates.Visible;
            this.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var item = PurchasesTool.GetProduct("membership_tokket");
            var billing = await PurchaseService.Instance.BillingStart(item.Id,item);        
            

            if (billing)
            {
                //var resultModel = await SharedService.AccountService.Instance.SignUpAsync(email,
                //       password, displayname, country, bday, imgPhoto, SpinAccounType.ContentDescription, SpinGroupType.ContentDescription, EditOwnerName.Text);

                
                tokketUser = await SharedService.AccountService.Instance.GetUserAsync(Settings.GetUserModel().UserId);
                Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                Settings.UserCoins = tokketUser.Coins.Value;

                this.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                ShowLottieMessageDialog(this, "Membership Completed! You got 100 coins!", true, handlerOkClick: (s, e) =>
                {
                    Intent = new Intent();
                    SetResult(Result.Ok, Intent);
                    Finish();
                }, _animation: "dollar_coin_spinning.json");
            }
            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //...

                //Handle sandwich menu icon click
                case HomeId:
                    //If menu is open, close it. Else, open it.
                    Finish();
                    break;
     
            }
            return base.OnOptionsItemSelected(item);
        }
        int DaysRemaining;
        bool IsExpired(DateTime date) {
          
            DateTime purchasedDate = Convert.ToDateTime(date);
            DateTime dateToday = DateTime.Now.Date;

            TimeSpan ts = dateToday - purchasedDate;
            DaysRemaining = ts.Days;
            if (DaysRemaining < 365)
                return false;
            return true;
        }
        public TextView NumberSection => FindViewById<TextView>(Resource.Id.numberedSection);

        public TextView LetteredSection => FindViewById<TextView>(Resource.Id.letteredSection);

        public TextView MembershipState => FindViewById<TextView>(Resource.Id.membership_state_text);

        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);

        public TextView DaysLeft => FindViewById<TextView>(Resource.Id.daysLeft);
        public Button BuyButton => FindViewById<Button>(Resource.Id.BuyButton);
    }
}