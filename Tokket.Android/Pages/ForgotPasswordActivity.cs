using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.TextField;
using SharedAccount = Tokket.Shared.Services;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Forgot Password", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Forgot Password", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class ForgotPasswordActivity : BaseActivity
    {
        Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.forgotpassword_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

#if (_CLASSTOKS)
            relativeLayout.SetBackgroundResource(Resource.Drawable.bg_classtok);
            logo.SetImageResource(Resource.Drawable.classtok_text);
#endif
            SubmitButton.Click += async(sender,e) =>
            {
                if (Patterns.EmailAddress.Matcher(EditEmailAddress.Text.Trim()).Matches())
                {
                    var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                    inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);

                    ProgressBarLogin.Visibility = ViewStates.Visible;
                    ProgressBarText.Visibility = ViewStates.Visible;

                    var result = await SharedAccount.AccountService.Instance.SendPasswordResetAsync(EditEmailAddress.Text.Trim());
                    if (result.ResultEnum == Tokket.Shared.Helpers.Result.Success)
                    {
                        ForgotPasswordConfirmation.Text = "Forgot password confirmation";
                        SubmitButton.Visibility = ViewStates.Gone;
                        inputLayoutEmail.Visibility = ViewStates.Gone;
                        ForgotPasswordVerificationSent.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        inputLayoutEmail.Error = result.ResultMessage;
                    }
                }
                else
                {
                    inputLayoutEmail.Error = "Invalid email address";
                }

                ProgressBarLogin.Visibility = ViewStates.Gone;
                ProgressBarText.Visibility = ViewStates.Gone;
            };
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


        public RelativeLayout relativeLayout => FindViewById<RelativeLayout>(Resource.Id.relativeLayout);
        public ImageView logo => FindViewById<ImageView>(Resource.Id.logoImage);
        public TextInputLayout inputLayoutEmail => FindViewById<TextInputLayout>(Resource.Id.inputLayoutEmail);
        public TextView ForgotPasswordConfirmation => FindViewById<TextView>(Resource.Id.ForgotPasswordConfirmation);
        public TextView ForgotPasswordVerificationSent => FindViewById<TextView>(Resource.Id.ForgotPasswordVerificationSent);
        public EditText EditEmailAddress => FindViewById<EditText>(Resource.Id.ForgotPasswordEditTextEmail);
        public Button SubmitButton => FindViewById<Button>(Resource.Id.ForgotPasswordBtnSubmit);

        TextView txtProgressBar; ProgressBar progressBarLogin;
        public ProgressBar ProgressBarLogin
        {
            get
            {
                return progressBarLogin
                       ?? (progressBarLogin = FindViewById<ProgressBar>(Resource.Id.progressbarLogin));
            }
        }

        public TextView ProgressBarText
        {
            get
            {
                return txtProgressBar
                       ?? (txtProgressBar = FindViewById<TextView>(Resource.Id.progressBarinsideText));
            }
        }
    }
}