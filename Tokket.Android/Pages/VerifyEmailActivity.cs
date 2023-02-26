using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SharedAccount = Tokket.Shared.Services;
using SharedHelpers = Tokket.Shared.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "VerifyEmailActivity")]
    public class VerifyEmailActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.emailverifyview);
            // Create your application here
            resendButton.Click += async (o, e) => {
                progress.Visibility = ViewStates.Visible;
                this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                var check = await SharedAccount.AccountService.Instance.SendEmailVerificationAsync(SharedHelpers.Settings.GetTokketUser().Email, SharedHelpers.Settings.GetUserModel().IdToken);
                var objBuilder = new AlertDialog.Builder(this);
                if (check)
                {

                    progress.Visibility = ViewStates.Gone;

                    objBuilder.SetTitle("Email Verify");
                    objBuilder.SetMessage($"Email Verification sent to {SharedHelpers.Settings.GetTokketUser().Email}");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                    {
                        Finish();
                    });
                    objDialog.Show();
                    this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                }
                else
                {
                    progress.Visibility = ViewStates.Gone;

                    objBuilder.SetTitle("Warning");
                    objBuilder.SetMessage("Failed to resend verification, try again!");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                    {
                        Finish();
                    });
                    objDialog.Show();
                    this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                }
            };
            cancelBtn.Click += (o, e) => { Finish(); };
        }

        public Button resendButton => FindViewById<Button>(Resource.Id.btnResendEmail);
        public Button cancelBtn => FindViewById<Button>(Resource.Id.btnCancel);
        public LinearLayout progress => FindViewById<LinearLayout>(Resource.Id.linerverify);
    }
}