using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.App.ActionBar;
using ServiceAccount = Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using SharedHelpers = Tokket.Shared.Helpers;
using Android.Content.PM;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "More Settings", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "More Settings", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class MoreSettingsActivity : BaseActivity
    {
        private Dialog popupDialog;
        Button btnConfirm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.moresettings);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            btnDeleteAccount.Click += (object sender, EventArgs e) =>
            {
                popupDialog = new Dialog(this);
                popupDialog.SetContentView(Resource.Layout.deleteaccount_page);
                popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                popupDialog.Show();

                // Some Time Layout width not fit with windows size  
                // but Below lines are not necessary  
                double widthD = getLayoutWidth();
                popupDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
                popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);
                // Access Popup layout fields like below  
                btnConfirm = popupDialog.FindViewById<Button>(Resource.Id.btnConfirm);

                // Events for that popup layout  
                btnConfirm.Click += delegate
                {
                    var isDeleted = DeleteAccount();
                    if (isDeleted.Result)
                    {
                        MainActivity.Instance.Finish();
                        Intent logoutActivity = new Intent(this, typeof(LoginActivity));
                        logoutActivity.AddFlags(ActivityFlags.ClearTop);
                        SecureStorage.Remove("idtoken");
                        SecureStorage.Remove("refreshtoken");
                        SecureStorage.Remove("userid");

                        Settings.UserAccount = string.Empty;

                        this.StartActivity(logoutActivity);
                        this.Finish();
                    }
                };

            };
        }

        private async Task<bool> DeleteAccount()
        {
            return await ServiceAccount.AccountService.Instance.DeleteUserAsync(Settings.GetTokketUser().Id);
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

        public Button btnDeleteAccount => FindViewById<Button>(Resource.Id.btnDeleteAccount);

    }
}