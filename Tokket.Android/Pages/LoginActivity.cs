using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tokket.Android.Custom;
using Tokket.Android.ViewModels;
using Tokket.Core;
using Xamarin.Essentials;
using SharedAccount = Tokket.Shared.Services;
using SharedHelpers = Tokket.Shared.Helpers;
using Android.Graphics;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;

namespace Tokket.Android
{
    [Activity(Label = "Login Activity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    public class LoginActivity : AppCompatActivity //, IFacebookCallback , GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        //Facebook
       /* private ICallbackManager mFBCallManager;
        private MyProfileTracker mProfileTracker;*/

        //Google
        /*GoogleApiClient mGoogleApiClient;
        private ConnectionResult mConnectionResult;
        SignInButton btnGoogleLogin;
        private bool mIntentInProgress;
        private bool mSignInClicked;
        private bool mInfoPopulated;*/

        Intent nextActivity = null;

        //AuthorizationTokenModel Useraccount;
        internal static LoginActivity Instance { get; private set; }
        public LoginPageViewModel LoginVm => App.Locator.LoginPageVM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.login_page);

            //End of lottie implementation

            SharedHelpers.Settings.ActivityInt = 0;

            TextView txtSignUp = FindViewById<TextView>(Resource.Id.txtSignup);
            //LoginButton btnFBLogin = FindViewById<LoginButton>(Resource.Id.btnFBLogin);
            //btnGoogleLogin = FindViewById<SignInButton>(Resource.Id.btnGoogleLogin);

            Instance = this;

            var message = Intent.GetStringExtra("message");
            if (!string.IsNullOrEmpty(message))
            {
                Toast.MakeText(this, message, ToastLength.Long).Show();
            }

            /*//Facebook
            mProfileTracker = new MyProfileTracker();
            mProfileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mProfileTracker.StartTracking();

            //Facebook
            btnFBLogin.SetPermissions(new List<string> {
                    "user_friends",
                    "public_profile"
                     });
            mFBCallManager = CallbackManagerFactory.Create();
            btnFBLogin.RegisterCallback(mFBCallManager, this);*/

            //Google
            /*btnGoogleLogin.Click += btnGoogleLogin_Click;
            GoogleApiClient.Builder builder = new GoogleApiClient.Builder(this);
            builder.AddConnectionCallbacks(this);
            builder.AddOnConnectionFailedListener(this);
            builder.AddApi(PlusClass.API);
            builder.AddScope(PlusClass.ScopePlusProfile);
            builder.AddScope(PlusClass.ScopePlusLogin);
            //Build our IGoogleApiClient  
            mGoogleApiClient = builder.Build();*/


            txtSignUp.Click += (object sender, EventArgs e) =>
            {
                nextActivity = new Intent(this, typeof(SignupActivity));
                StartActivityForResult(nextActivity,11111);
            };

            LabelForgotPassword.Click += delegate
            {
                nextActivity = new Intent(this, typeof(ForgotPasswordActivity));
                StartActivity(nextActivity);
            };

#if (_CLASSTOKS)
            BtnLogin.SetBackgroundResource(Resource.Drawable.blue_button);
            relativeLayoutParent.SetBackgroundResource(Resource.Drawable.bg_classtok);
            LogoText.SetImageResource(Resource.Drawable.classtok_text);
            ProgressBarLogin.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)), PorterDuff.Mode.Multiply);
#endif

            LoginVm.EmailInputLayout = inputLayoutEmail;
            LoginVm.PasswordInputLayout = inputLayoutPassword;
            LoginVm.linearProgress = linearProgress;

            LoginVm.Instance = this;

            BtnLogin.Click += async(s, e) =>
            {
                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(BtnLogin.WindowToken, HideSoftInputFlags.None);

                LoginVm.Credentials.Username = Username.Text;
                LoginVm.Credentials.Password = Password.Text;

                await LoginVm.Login();
            };

        }

        /*public void ShowEmailVerfy() {
            var intent = new Intent(this, typeof(VerifyEmailActivity));
            StartActivity(intent);
        }*/

        public void CloseLogin()
        {
            this.Finish();
        }
        public void ShowEmailVerfy(string email, string idtoken)
        {
            var lottieMessageDialog = new MessageLottieDialog(this, header: "Email Verification" , GetString(Resource.String.emailverify), false, "Resend Email", async (s, e) =>
            {
                showBlueLoading(this);
                var check = await SharedAccount.AccountService.Instance.SendEmailVerificationAsync(email, idtoken);
                hideBlueLoading(this);

                if (check)
                {
                    new MessageLottieDialog(this, header: "Email Verification", $"Email Verification sent to {email}", true, handlerOKText: "", (s, e) =>
                    {}).Show();
                }
                else
                {
                    new MessageLottieDialog(this, header: "Email Verification", "Failed to resend verification, try again!", false, handlerOKText: "Retry", (s, e) =>
                    {
                        ShowEmailVerfy(email, idtoken);
                    }).Show();
                }
            });
            lottieMessageDialog.Show();
        }

        public void GoToSubAccountPage()
        {
            Intent nextActivity = new Intent(this, typeof(SubAccountActivity));
            Finish();
            StartActivity(nextActivity);
        }

        public void GoToMainPage()
        {
            Intent nextActivity = new Intent(this, typeof(MainActivity));
            Finish();
            StartActivity(nextActivity);
        }

        private Dialog loadingDialog;
        private void showBlueLoading(Context context)
        {
            if (!IsFinishing)
            {
                if (loadingDialog == null)
                {
                    var imgDialog = new AppCompatImageView(context);
                    imgDialog.SetBackgroundResource(Resource.Drawable.blue_loading);
                    var tween = AnimationUtils.LoadAnimation(context, Resource.Animation.anim_blue_loading_dialog);
                    imgDialog.StartAnimation(tween);
                    loadingDialog = new Dialog(this);
                    loadingDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                    loadingDialog.Window.SetGravity(GravityFlags.Center);
                    loadingDialog.SetContentView(imgDialog);
                    loadingDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);
                    loadingDialog.SetCanceledOnTouchOutside(true);
                }
                if (loadingDialog != null)
                {
                    if (loadingDialog.IsShowing)
                    {
                        loadingDialog.Hide();
                        loadingDialog.Show();
                    }
                    else
                    {
                        loadingDialog.Show();
                    }
                }
            }
        }
        private void hideBlueLoading(Context context)
        {
            if (loadingDialog != null && loadingDialog.IsShowing)
            {
                loadingDialog.Dismiss();
            }
        }

        public TextInputLayout inputLayoutEmail => FindViewById<TextInputLayout>(Resource.Id.inputLayoutEmail);
        public TextInputLayout inputLayoutPassword => FindViewById<TextInputLayout>(Resource.Id.inputLayoutPassword);
        public TextInputEditText Username => FindViewById<TextInputEditText>(Resource.Id.txtEmail);
        public TextInputEditText Password => FindViewById<TextInputEditText>(Resource.Id.txtPassword);
        public Button BtnLogin => FindViewById<Button>(Resource.Id.btnLogin);

        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public ProgressBar ProgressBarLogin => FindViewById<ProgressBar>(Resource.Id.ProgressBarLogin);
        public TextView LabelForgotPassword => FindViewById<TextView>(Resource.Id.txtForgotLogin);

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /*public void OnCancel() { }
        public void OnError(FacebookException p0) { }
        public void OnSuccess(Java.Lang.Object p0)
        {
        }
        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                try
                {
                    //TxtFirstName.Text = e.mProfile.FirstName;
                    //TxtLastName.Text = e.mProfile.LastName;
                    //TxtName.Text = e.mProfile.Name;
                    //mprofile.ProfileId = e.mProfile.Id;
                }
                catch (Java.Lang.Exception ex) { }
            }
            else
            {
                //TxtFirstName.Text = "First Name";
                //TxtLastName.Text = "Last Name";
                //TxtName.Text = "Name";
                //mprofile.ProfileId = null;
            }
        }*/
        /* protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
         {
             base.OnActivityResult(requestCode, resultCode, data);
             mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);

             Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);
             if (requestCode == 0)
             {
                 if (resultCode != Result.Ok)
                 {
                     mSignInClicked = false;
                 }
                 mIntentInProgress = false;
                 if (!mGoogleApiClient.IsConnecting)
                 {
                     mGoogleApiClient.Connect();
                 }
             }
         }

         protected override void OnStart()
         {
             base.OnStart();
             mGoogleApiClient.Connect();
         }

         protected override void OnStop()
         {
             base.OnStop();
             if (mGoogleApiClient.IsConnected)
             {
                 mGoogleApiClient.Disconnect();
             }
         }
         public void OnConnected(Bundle connectionHint)
         {
             var person = PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient);
             var name = string.Empty;
             if (person != null)
             {
                 //TxtName.Text = person.DisplayName;
                 //TxtGender.Text = person.Nickname;
                 //var Img = person.Image.Url;
                 //var imageBitmap = GetImageBitmapFromUrl(Img.Remove(Img.Length - 5));
                 //if (imageBitmap != null) ImgProfile.SetImageBitmap(imageBitmap);
             }
         }

         private void btnGoogleLogin_Click(object sender, EventArgs e)
         {
             if (!mGoogleApiClient.IsConnecting)
             {
                 mSignInClicked = true;
                 ResolveSignInError();
             }
             else if (mGoogleApiClient.IsConnected)
             {
                 PlusClass.AccountApi.ClearDefaultAccount(mGoogleApiClient);
                 mGoogleApiClient.Disconnect();
             }
         }
         private void ResolveSignInError()
         {
             if (mGoogleApiClient.IsConnecting)
             {
                 return;
             }
             if (mConnectionResult.HasResolution)
             {
                 try
                 {
                     mIntentInProgress = true;
                     StartIntentSenderForResult(mConnectionResult.Resolution.IntentSender, 0, null, 0, 0, 0);
                 }
                 catch (Android.Content.IntentSender.SendIntentException io)
                 {
                     mIntentInProgress = false;
                     mGoogleApiClient.Connect();
                 }
             }
         }
         //private Bitmap GetImageBitmapFromUrl(System.String url)
         //{
         //    Bitmap imageBitmap = null;
         //    try
         //    {
         //        using (var webClient = new WebClient())
         //        {
         //            var imageBytes = webClient.DownloadData(url);
         //            if (imageBytes != null && imageBytes.Length > 0)
         //            {
         //                imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
         //            }
         //        }
         //        return imageBitmap;
         //    }
         //    catch (IOException e) { }
         //    return null;
         //}
         public void OnConnectionFailed(ConnectionResult result)
         {
             if (!mIntentInProgress)
             {
                 mConnectionResult = result;
                 if (mSignInClicked)
                 {
                     ResolveSignInError();
                 }
             }
         }*/
   
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 11111 && resultCode == Result.Ok && data != null) {
                var datas = JsonConvert.DeserializeObject<List<string>>(data.GetStringExtra("SignUpData"));
                Username.Text = datas[0];
                Password.Text = datas[1];
                LoginVm.Login();
            }
        }

        public void OnConnectionSuspended(int cause) { }
        private RelativeLayout relativeLayoutParent => FindViewById<RelativeLayout>(Resource.Id.relativeLayoutParent);
        private ImageView LogoText => FindViewById<ImageView>(Resource.Id.imageView1);
    }
}