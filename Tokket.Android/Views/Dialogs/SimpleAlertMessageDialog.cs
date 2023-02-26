using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Android.Custom
{
    public class SimpleAlertMessageDialog : Dialog
    {
        private LottieAnimationView lottieAnimationView;
        private Button btnOk;
        private Button btnCancel;
        public SimpleAlertMessageDialog(Context context, string handlerOKText, EventHandler handlerOkClick, string handlerCancelText = "",  EventHandler handlerCancel = null, String animation = "") : base(context)
        {
            SetContentView(Resource.Layout.dialog_simple_alert_message);

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            lottieAnimationView = FindViewById<LottieAnimationView>(Resource.Id.lottieAnimationView);
            btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);

            lottieAnimationView.SetAnimation(animation);
            lottieAnimationView.PlayAnimation();

            if (handlerCancel == null)
                btnCancel.Visibility = ViewStates.Gone;
            if (!string.IsNullOrEmpty(handlerCancelText))
                btnCancel.Text = handlerCancelText;
            if (!string.IsNullOrEmpty(handlerOKText))
                btnOk.Text = handlerOKText;

            if (handlerOkClick != null)
            {
                btnOk.Click += handlerOkClick;
            }
            else
            {
                btnOk.Click += btnOk_Click;
            }

            if (handlerCancel != null)
            {
                btnCancel.Click += handlerCancel;
            }
            else
            {
                btnCancel.Click += btnCancel_Click;
            }
        }

        private void btnOk_Click(Object sender, EventArgs e)
        {
            Dismiss();
        }

        private void btnCancel_Click(Object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}