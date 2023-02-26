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
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Purchase;
using ServiceAccount = Tokket.Shared.Services;

namespace Tokket.Android
{
    [Activity(Label = "CreateSubaccountActivity")]
    public class CreateSubaccountActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_subaccount);
            // Create your application here

            progress.Visibility = ViewStates.Gone;


            if (Settings.GetTokketUser().GroupAccountType == "family")
            {
                header.Text = "Create Family Subaccount ($2.99 USD)";
                subaccountName.Text = "Family Subaccount Name";
                BtnCreate.Text = "Create Family Subaccount ($2.99 USD)";
            }
            else
            {
                header.Text = "Create Organization Subaccount ($2.99 USD)";
                subaccountName.Text = "Organization Subaccount Name";
                BtnCreate.Text = "Create Organization Subaccount ($2.99 USD)";
            }

            btnBack.Click += (obj, _event) => {
                Finish();
            };
            BtnCreate.Click += async (obj, _event) => {
                progress.Visibility = ViewStates.Visible;
                var user = Settings.GetTokketUser();

                var groupPurchase = PurchasesTool.GetProduct("subaccount_tokket");
                var subName = TxtSubName.Text;
                var key = subKey.Text;
                var result = await ServiceAccount.PurchaseService.Instance.BillingStart(groupPurchase.Id, groupPurchase, subaccountName: subName, subaccountKey: key);
                if (result)
                {

                   
                    var objBuilder = new AlertDialog.Builder(this);
                    objBuilder.SetTitle("");
                    objBuilder.SetMessage("Subaccount Created");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                    {



                    });
                    objDialog.Show();
                }
                else
                {
                    progress.Visibility = ViewStates.Gone;
                }

            };
         
        }

        TextView header => FindViewById<TextView>(Resource.Id.txt_viewTitle);
        TextView subaccountName => FindViewById<TextView>(Resource.Id.txt_subaccount);
        EditText TxtSubName => FindViewById<EditText>(Resource.Id.edit_subaccountName);
        EditText subKey => FindViewById<EditText>(Resource.Id.edit_subaccountKey);
        Button BtnCreate => FindViewById<Button>(Resource.Id.btn_createSub);
        Button btnBack => FindViewById<Button>(Resource.Id.btnBack);
        LinearLayout progress => FindViewById<LinearLayout>(Resource.Id.LinearProgress);
    }
}