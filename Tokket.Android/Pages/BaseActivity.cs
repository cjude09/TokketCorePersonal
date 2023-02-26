using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Newtonsoft.Json;
using Tokket.Android.Custom;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Xamarin.Essentials;
using Plugin.Connectivity;
using Skydoves.BalloonLib;
using AndroidX.AppCompat.Content.Res;
using DE.Hdodenhof.CircleImageViewLib;
using Android.Graphics.Drawables;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using AndroidX.Core.Content;
using static Android.Views.ViewGroup;

namespace Tokket.Android
{
    public abstract class BaseActivity : AppCompatActivity
    {
        public const int HomeId = 16908332; //Unable to find Android.Resource.Id.Home, so create a new one with its equivalent Id temporarily
        public List<string> Colors = new List<string>() {
               "#60a895", "#2e75b5", "#ff0000", "#70ad47",
               "#833c0b", "#7f6000", "#7030a0", "#1f3864", 
               "#225f85", "#6298a0", "#7f8d27", "#ba210d", 
               "#d37397",
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        public List<string> randomcolors = new List<string>();

        public Color defaultPrimaryColor;
        public AndroidX.AppCompat.Widget.Toolbar toolBarLayout;
        private Handler mHandler;
        private Java.Lang.Runnable Runnable;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

#if (_TOKKEPEDIA)
            defaultPrimaryColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary));
#endif

#if (_CLASSTOKS)
            defaultPrimaryColor = new Color(ContextCompat.GetColor(this, Resource.Color.navy_blue));
#endif

            mHandler = new Handler(Looper.MainLooper);
            Runnable = new Java.Lang.Runnable(() =>
            {
                Task.Factory.StartNew(async () => { await CheckToken(); });
                mHandler.PostDelayed(Runnable, 1800000);
            });
            Runnable.Run();
            TokenChecker();
        }

        public void setActivityTitle(string title, string subTitle = "")
        {
            toolBarLayout = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.includeHeaderLayout);
            toolBarLayout.SetBackgroundColor(defaultPrimaryColor);
            FindViewById<TextView>(Resource.Id.toolbar_title).Text = title;

            if (!string.IsNullOrEmpty(subTitle))
            {
                FindViewById<TextView>(Resource.Id.toolbar_subtitle).Visibility = ViewStates.Visible;
                FindViewById<TextView>(Resource.Id.toolbar_subtitle).Text = subTitle;
            }
        }

        public string getActivityTitle()
        {
            return FindViewById<TextView>(Resource.Id.toolbar_title).Text;
        }

        public string getActivitySubTitle()
        {
            return FindViewById<TextView>(Resource.Id.toolbar_subtitle).Text;
        }

        public double getLayoutWidth()
        {
            Display display = WindowManager.DefaultDisplay;
            Point size = new Point();
            try
            {
                display.GetRealSize(size);
            }
            catch (Exception)
            {
                display.GetSize(size);
            }
            double widthD = size.X;
            int height = size.Y;

            return widthD;
        }

        public double getLayoutHeight()
        {
            Display display = WindowManager.DefaultDisplay;
            Point size = new Point();
            try
            {
                display.GetRealSize(size);
            }
            catch (Exception)
            {
                display.GetSize(size);
            }
            double widthD = size.X;
            int height = size.Y;

            return height;
        }

        public void setWindowSize(double width, double height)
        {
            double widthD = getLayoutWidth();
            double heightD = getLayoutHeight();

            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            if (width == 0 && height == 0)
            {
                Window.SetLayout(LayoutParams.WrapContent, LayoutParams.WrapContent);
            }
            else if (width > 0 && height == 0)
            {
                Window.SetLayout(int.Parse((widthD * width).ToString()), LayoutParams.WrapContent);
            }
            else if (width == 0 && height > 0)
            {
                Window.SetLayout(LayoutParams.WrapContent, int.Parse((heightD * height).ToString()));
            }
            else
            {
                Window.SetLayout(int.Parse((widthD * width).ToString()), int.Parse((heightD * height).ToString()));
            }
            Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);
        }

        public void showAlertDialog(Context context, String message, EventHandler<DialogClickEventArgs> handler = null)
        {
         
            var alertDiag = new AlertDialog.Builder(context);
            alertDiag.SetTitle("");
            alertDiag.SetMessage(message);
            if (handler == null)
            {
                handler = (senderAlert, args) =>
                {
                    alertDiag.Dispose();
                };
            }
            else {
                alertDiag.SetPositiveButton("OK", handler);
            }
          
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        private Dialog loadingDialog, lottieMessageDialog;
        public void showBlueLoading(Context context)
        {
            if (!IsFinishing)
            {
                if (loadingDialog == null)
                {
                    var imgDialog = new AppCompatImageView(context);
                    imgDialog.SetBackgroundResource(Resource.Drawable.blue_loading);
                    var tween = AnimationUtils.LoadAnimation(context, Resource.Animation.anim_blue_loading_dialog);
                    imgDialog.StartAnimation(tween);
                    loadingDialog = new Dialog(context);
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

        public Balloon showTooltip(Context context, string tooltipText,int resource = 0) {
            var ballon = new Balloon.Builder(context);
            ballon.SetWidthRatio(1.0f);
            ballon.SetHeight(110);
            ballon.SetWidth(LayoutParams.WrapContent);
            ballon.SetText(tooltipText);
            ballon.SetBackgroundColorResource(Resource.Color.GREY);
            ballon.SetTextColorResource(Resource.Color.WHITE);
            if (resource != 0) {
              
                ballon.SetIconDrawableResource(resource);
                ballon.SetIconColor(ContextCompat.GetColor(this,Resource.Color.BLACK));
                ballon.SetIconSize(50);
                //ballon.SetIconSpace(10);
            }
               
            ballon.SetLifecycleOwner(this);
            ballon.SetPadding(10);
            ballon.Build();
         

            var b = new Balloon(context, ballon);
            b.BalloonOutsideTouch += delegate
            {
                b.DismissWithDelay(1000);
             
            };
          
            return b;
          
        }

        public void customToolTip(Context context, string tooltipText, int resource = 0, View v = null)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(context);
            View popupView = layoutInflater.Inflate(Resource.Layout.layout_balloon_bottom, null);
            var icon = popupView.FindViewById<CircleImageView>(Resource.Id.item_custom_icon);
            icon.Visibility = ViewStates.Gone;
            if (resource != 0)
            {
                icon.Visibility = ViewStates.Visible;
                icon.SetImageResource(resource);
            }
            popupView.FindViewById<TextView>(Resource.Id.item_custom_title).Text = tooltipText;
            popupView.FindViewById<View>(Resource.Id.view_bg).BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.GREY);

            var infoPopup = new PopupWindow(popupView,
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
            infoPopup.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            infoPopup.OutsideTouchable = true;
            infoPopup.ShowAsDropDown(v);
        }

        public void hideBlueLoading(Context context)
        {
            if (loadingDialog != null && loadingDialog.IsShowing)
            {
                loadingDialog.Dismiss();
            }
        }


        public void hideDialog(Dialog dialog)
        {
            if (dialog != null && dialog.IsShowing)
            {
                dialog.Dismiss();
            }
        }

        public void showDialog(Dialog dialog)
        {
            if (!IsFinishing)
            {
                if (dialog != null)
                {
                    if (dialog.IsShowing)
                    {
                        dialog.Hide();
                    }
                }

                dialog.Show();
            }
        }

        public void ShowLottieMessageDialog(Context context, string messageContent, bool success = false, string handlerOKText = "", EventHandler handlerOkClick = null, EventHandler handlerCancelClick = null, string _animation = "", string header = "",bool isImage = false)
        {
            if (!IsFinishing)
            {
                if (lottieMessageDialog != null)
                {
                    if (lottieMessageDialog.IsShowing)
                    {
                        lottieMessageDialog.Hide();
                    }
                }

                lottieMessageDialog = new MessageLottieDialog(context, header, message: messageContent, isSuccess: success, handlerOKText, handlerOkClick: handlerOkClick, handlerCancel: handlerCancelClick, animation: _animation,isImage);
                lottieMessageDialog.Show();
            }
        }

        public void hideLottieMessageDialog(Context context)
        {
            if (lottieMessageDialog != null && lottieMessageDialog.IsShowing)
            {
                lottieMessageDialog.Dismiss();
            }
        }
       

        private void TokenChecker() {
            var idtoken = SecureStorage.GetAsync("idtoken").GetAwaiter().GetResult();
            var refreshtoken = SecureStorage.GetAsync("refreshtoken").GetAwaiter().GetResult();
            var userid = SecureStorage.GetAsync("userid").GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(Settings.UserAccount))
            {
                var firebasemodel = JsonConvert.DeserializeObject<AuthorizationTokenModel>(Settings.UserAccount);
            }
        }

        public async Task CheckToken()
        {
            try
            {
                bool resultbool = false;
                var idtoken = await SecureStorage.GetAsync("idtoken");
                var refreshtoken = await SecureStorage.GetAsync("refreshtoken");
                var result = Shared.Services.AccountService.Instance.VerifyToken(idtoken, refreshtoken);
               // var test =  AppContainer.Resolve<IAccountService>();
               // var testResult = await test.VerifyTokenAsync(idtoken, refreshtoken);
                if (result.ResultMessage.Contains("refreshed"))
                {
                    SecureStorage.Remove("idtoken");
                    SecureStorage.Remove("refreshtoken");
                    var obj = JsonConvert.DeserializeObject<AuthorizationTokenModel>(result.ResultObject.ToString());

                    await SecureStorage.SetAsync("idtoken", obj.IdToken);
                    await SecureStorage.SetAsync("refreshtoken", obj.RefreshToken);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

        }

        public void VerifyEmail(string email, string idtoken)
        {
            ShowLottieMessageDialog(this, GetString(Resource.String.emailverify), false, "Resend Email", async(s, e) =>
            {
                showBlueLoading(this);
                var check = await Shared.Services.AccountService.Instance.SendEmailVerificationAsync(email, idtoken);
                hideBlueLoading(this);

                if (check)
                {
                    ShowLottieMessageDialog(this, $"Email Verification sent to {email}", true, header: "Email Verification");
                }
                else
                {
                    ShowLottieMessageDialog(this, "Failed to resend verification, try again!", false, "Try Again", (s, e) =>
                    {
                        VerifyEmail(email, idtoken);
                    }, header: "Email Verification");
                }
            }, header: "Email Verification");
        }

        #region Used in caching big data like user photo
        public string GetCachedAsync<T>(string url, bool forceRefresh = false)
        {
            var json = string.Empty;

            if (!CrossConnectivity.Current.IsConnected)
                json = Shared.Services.Barrel.Current.Get<string>(url);

            try
            {
                if (!forceRefresh && !Shared.Services.Barrel.Current.IsExpired(url))
                    json = Shared.Services.Barrel.Current.Get<string>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return json;
        }

        public void SetCachedAsync<T>(string url, string json, int days = 7)
        {
            try
            {
                Shared.Services.Barrel.Current.Add(url, json, TimeSpan.FromDays(days));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get information from server {ex}");
                //probably re-throw here :)
            }
        }

        #endregion

        public AndroidX.AppCompat.Widget.Toolbar toolBar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.includeHeaderLayout);
    }
}