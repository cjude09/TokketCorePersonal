using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Tokket.Core;
using Xamarin.Essentials;
using SharedHelpers = Tokket.Shared.Helpers;
using Android.Graphics;
using Tokket.Shared.IoC;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Theme = "@style/AppTheme", MainLauncher = true, Icon = "@drawable/tokkepedia_icon", NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Theme = "@style/AppTheme", MainLauncher = true, Icon = "@drawable/classtoks", NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class SplashActivity : AppCompatActivity
    {
        Action action;
        public Handler handler;
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        TokketUser tokketUser;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppContainer.RegisterAndroidDependencies();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!IsTaskRoot)
            {
                this.Finish();
                return;
            }

            SetContentView(Resource.Layout.splash_screen);

            action = Callback;
            handler = new Handler(Looper.MainLooper, new CustomHandlerCallback(this));

            tokketUser = SharedHelpers.Settings.GetTokketUser();
#if (_CLASSTOKS)
            imgTokkepediaIcon.SetImageResource(Resource.Drawable.classtoks);
            imgTokkepediaLogo.SetImageResource(Resource.Drawable.classtok_text);
            linearBackground.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));
            linearBackground.SetBackgroundResource(Resource.Drawable.bg_classtok);

            ProgressBar progressSplash = FindViewById<ProgressBar>(Resource.Id.progressSplash);
            progressSplash.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)), PorterDuff.Mode.Multiply);
#endif
            /*Task.Run(() => {
                Thread.Sleep(1000); // Simulate a long loading process on app startup.
                RunOnUiThread(() => {
                    startMainActivity();
                });
            });*/

            handler.SendEmptyMessageDelayed(0, 100);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        void Callback()
        {
            // repost itself
            handler.PostDelayed(action, 500);

            // do other stuff
        }
        public void startSplashActivity()
        {
            handler.SendEmptyMessageDelayed(1, 500);
        }
        public void startMainActivity()
        {
            clearcache();

            if (SharedHelpers.Settings.UserAccount == string.Empty)
            {
                LoginActivity();
            }
            else if (tokketUser != null)
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet && tokketUser.EmailVerified)
                {
                    if (tokketUser.AccountType == "group")
                    {
                        if (SharedHelpers.Settings.GetTokketSubaccount() == null)
                        {
                            var nextActivity = new Intent(this, typeof(SubAccountActivity));
                            Finish();
                            StartActivity(nextActivity);
                        }
                        else
                        {
                            MainActivity();
                        }
                    }
                    else
                    {
                        MainActivity();
                    }
                }
                else
                {
                    LoginActivity();
                }
            }
            else
            {
                LoginActivity();
            }
            
            Finish();


            handler.RemoveCallbacks(action);
            OverridePendingTransition(Resource.Animation.fade, Resource.Animation.hold);
        }

        private void clearcache()
        {
            SharedHelpers.Settings.classFilterByClass = string.Empty;
            SharedHelpers.Settings.classFilterByCategory = string.Empty;

            SharedHelpers.Settings.FilterByTypeHome = 0;
            SharedHelpers.Settings.FilterByItemsHome = string.Empty;

            SharedHelpers.Settings.FilterByTypeSearch = 0;
            SharedHelpers.Settings.FilterByItemsSearch = string.Empty;

            SharedHelpers.Settings.FilterByTypeProfile = 0;
            SharedHelpers.Settings.FilterByItemsProfile = string.Empty;
        }
        private void LoginActivity()
        {
            var mainIntent = new Intent(this, typeof(LoginActivity));
            this.StartActivity(mainIntent);
        }
        private void MainActivity()
        {
            var nextActivity = new Intent(this, typeof(MainActivity));
            StartActivity(nextActivity);
        }
        public override void OnBackPressed()
        {
        }

        private ImageView imgTokkepediaLogo => FindViewById<ImageView>(Resource.Id.imgTokkepediaLogo);
        private ImageView imgTokkepediaIcon => FindViewById<ImageView>(Resource.Id.imgTokkepediaIcon);
        private LinearLayout linearBackground => FindViewById<LinearLayout>(Resource.Id.linearBackground);
    }
    public class CustomHandlerCallback : Java.Lang.Object, Handler.ICallback
    {
        private SplashActivity mainActivity;
        public CustomHandlerCallback(SplashActivity activity)
        {
            this.mainActivity = activity;
        }

        public bool HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case 0:
                    mainActivity.startSplashActivity();
                    break;
                case 1:
                    mainActivity.startMainActivity();
                    break;
            }
            return true;
        }
    }
}