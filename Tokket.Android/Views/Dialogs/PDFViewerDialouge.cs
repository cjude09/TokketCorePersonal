using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.App.ActionBar;

namespace Tokket.Android.Custom
{
    public class PDFViewerDialouge : Dialog
    {
        public WebView WebView => FindViewById<WebView>(Resource.Id.pdfViewer);

        private Button CloseBTN => FindViewById<Button>(Resource.Id.closeWebView);

        public TextView FileName => FindViewById<TextView>(Resource.Id.FileNametxt);
        public PDFViewerDialouge(Context context) : base(context)
        {
            SetContentView(Resource.Layout.web_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);

            CloseBTN.Click += (obj, eve) => { Dismiss(); };
        }

        public PDFViewerDialouge(Context context, int themeResId) : base(context, themeResId)
        {
            SetContentView(Resource.Layout.web_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);
            CloseBTN.Click += (obj, eve) => { Dismiss(); };
        }

        protected PDFViewerDialouge(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetContentView(Resource.Layout.web_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);
            CloseBTN.Click += (obj, eve) => { Dismiss(); };
        }

        protected PDFViewerDialouge(Context context, bool cancelable, EventHandler cancelHandler) : base(context, cancelable, cancelHandler)
        {
            SetContentView(Resource.Layout.web_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);
            CloseBTN.Click += (obj, eve) => { Dismiss(); };
        }

        protected PDFViewerDialouge(Context context, bool cancelable, IDialogInterfaceOnCancelListener cancelListener) : base(context, cancelable, cancelListener)
        {
            SetContentView(Resource.Layout.web_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.MatchParent);
            CloseBTN.Click += (obj, eve) => { Dismiss(); };
        }
    }
}