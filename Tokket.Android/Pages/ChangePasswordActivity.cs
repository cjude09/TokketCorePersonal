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
using SharedService = Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Android.Content.PM;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Change Password", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Change Password", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class ChangePasswordActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.changepassword);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            btnChangePassword.Click += async (object sender, EventArgs e) =>
            {
                txtErrorMismatch.Visibility = ViewStates.Gone;
                txtErrorOldPassword.Visibility = ViewStates.Gone;
                progressbar.Visibility = ViewStates.Visible;
                progressBarinsideText.Visibility = ViewStates.Visible;

                if(txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    txtErrorMismatch.Visibility = ViewStates.Visible;
                }
                else
                {
                    //change password                                           
                    var changePasswordResult = await SharedService.AccountService.Instance.ChangePasswordAsync(Settings.GetUserModel().UserId, txtOldPassword.Text, txtNewPassword.Text);
                    if (changePasswordResult.ResultEnum != Shared.Helpers.Result.Success)
                    {
                        txtErrorOldPassword.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        var dialog = new AppCompatDialogService();
                        await dialog.ShowError(
                                   "Successfully changed password!",
                                   "Success",
                                   "OK",
                                   null);
                    }
                }

                progressbar.Visibility = ViewStates.Gone;
                progressBarinsideText.Visibility = ViewStates.Gone;
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item) { 
            switch (item.ItemId) { 
                case HomeId: Finish();
                    break; 
            } 
            return base.OnOptionsItemSelected(item);
        }


        public Button btnChangePassword => FindViewById<Button>(Resource.Id.btnChangePassword);
        public EditText txtOldPassword => FindViewById<EditText>(Resource.Id.txtOldPassword);
        public EditText txtNewPassword => FindViewById<EditText>(Resource.Id.txtNewPassword);              
        public EditText txtConfirmPassword => FindViewById<EditText>(Resource.Id.txtConfirmPassword);
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbar);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideText);

        public TextView txtErrorOldPassword => FindViewById<TextView>(Resource.Id.txtErrorOldPassword);
        public TextView txtErrorMismatch => FindViewById<TextView>(Resource.Id.txtErrorMismatch);

    }
}