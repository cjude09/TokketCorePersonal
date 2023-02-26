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

namespace Tokket.Android.Custom
{
    public class MessageDialog: Dialog
    {
        private ImageButton btnClose;
        private Button btnOption1;
        private Button btnOption2;
        private TextView labelNote;
        private TextView txtHeaderText;
        public MessageDialog(Context context, string header, string message, string option1Text, string option2Text, EventHandler handlerOption1, EventHandler handlerOption2) : base(context)
        {
            SetContentView(Resource.Layout.dialog_message);

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            if (string.IsNullOrEmpty(header))
                header = context.GetString(Resource.String.simplealert_title);

            btnClose = FindViewById<ImageButton>(Resource.Id.imageBtnClose);
            btnOption1 = FindViewById<Button>(Resource.Id.btnOption1);
            btnOption2 = FindViewById<Button>(Resource.Id.btnOption2);
            labelNote = FindViewById<TextView>(Resource.Id.labelNote);
            txtHeaderText = FindViewById<TextView>(Resource.Id.txtHeaderText);

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

            btnOption1.Text = option1Text;
            btnOption2.Text = option2Text;

            btnOption1.Click += handlerOption1;
            btnOption2.Click += handlerOption2;

            btnOption1.Click += btnDismiss;
            btnOption2.Click += btnDismiss;

            btnClose.Click += btnDismiss;
        }

        private void btnDismiss(Object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}