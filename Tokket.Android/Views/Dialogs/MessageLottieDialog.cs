using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using Bumptech.Glide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Android.Custom
{
    public class MessageLottieDialog : Dialog
    {
        private LottieAnimationView lottieAnimationView;
        private Button btnOk;
        private Button btnCancel;
        private TextView labelNote;
        private TextView txtHeaderText;
        private ImageView GifCoinIcon;

        public MessageLottieDialog(Context context, string header, string message, bool isSuccess, string handlerOKText, EventHandler handlerOkClick, EventHandler handlerCancel = null, string animation = "",bool isImage = false) : base(context)
        {
            SetContentView(Resource.Layout.dialog_message_lottie);

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            if (string.IsNullOrEmpty(header))
                header = context.GetString(Resource.String.simplealert_title);

            lottieAnimationView = FindViewById<LottieAnimationView>(Resource.Id.lottieAnimationView);
            btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            labelNote = FindViewById<TextView>(Resource.Id.labelNote);
            txtHeaderText = FindViewById<TextView>(Resource.Id.txtHeaderText);
            GifCoinIcon = FindViewById<ImageView>(Resource.Id.gif_profileCoins);

            if (string.IsNullOrEmpty(animation))
            {
                if (isSuccess)
                {
                    lottieAnimationView.SetAnimation("checkmark.json");
                    lottieAnimationView.PlayAnimation();
                }
                else
                {
                    lottieAnimationView.SetAnimation("wrongmark.json");
                    lottieAnimationView.PlayAnimation();
                }
            }
            else
            {
                lottieAnimationView.SetAnimation(animation);
                lottieAnimationView.PlayAnimation();
            }

            if (string.IsNullOrEmpty(message))
            {
                labelNote.Visibility = ViewStates.Gone;
            }
            else
            {
                labelNote.Visibility = ViewStates.Visible;
            }
            labelNote.Text = message;
            txtHeaderText.Text = header;

            btnOk.Visibility = ViewStates.Visible;

            if (handlerCancel == null)
                btnCancel.Visibility = ViewStates.Gone;

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

            if (!string.IsNullOrEmpty(handlerOKText))
            {
                btnOk.Text = handlerOKText;
            }

            //GIF Animation
            Glide.With(MainActivity.Instance)
                .Load(Resource.Drawable.gold)
                .Into(GifCoinIcon);

            btnOk.Click += btnOk_Click;

            btnCancel.Click += btnCancel_Click;

            if (isImage) {
                GifCoinIcon.Visibility = ViewStates.Visible;
                lottieAnimationView.Visibility = ViewStates.Gone;
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