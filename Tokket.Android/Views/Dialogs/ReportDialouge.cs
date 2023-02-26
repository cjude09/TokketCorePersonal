using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.App.ActionBar;

namespace Tokket.Android.Custom
{
    public class ReportDialouge : Dialog
    {
        #region Properties
        private Context DialougeContext;

        public string SelectedReportMessage = string.Empty;

        public RadioButton RB1 => FindViewById<RadioButton>(Resource.Id.rb_1);
        public RadioButton RB2 => FindViewById<RadioButton>(Resource.Id.rb_2);
        public RadioButton RB3 => FindViewById<RadioButton>(Resource.Id.rb_3);
        public RadioButton RB4 => FindViewById<RadioButton>(Resource.Id.rb_4);
        public RadioButton RB5 => FindViewById<RadioButton>(Resource.Id.rb_5);
        public RadioButton RB6 => FindViewById<RadioButton>(Resource.Id.rb_6);
        public RadioButton RB7 => FindViewById<RadioButton>(Resource.Id.rb_7);
        public RadioButton RB8 => FindViewById<RadioButton>(Resource.Id.rb_8);

        public Spinner SP1 => FindViewById<Spinner>(Resource.Id.spn_1);
        public Spinner SP2 => FindViewById<Spinner>(Resource.Id.spn_2);
        public Spinner SP3 => FindViewById<Spinner>(Resource.Id.spn_3);
        public Spinner SP4 => FindViewById<Spinner>(Resource.Id.spn_4);
        public Spinner SP7 => FindViewById<Spinner>(Resource.Id.spn_7);
        public Spinner SP8 => FindViewById<Spinner>(Resource.Id.spn_8);

        public Button BtnCancel => FindViewById<Button>(Resource.Id.cancelBTN);

        public Button BTNClose => FindViewById<Button>(Resource.Id.closeBTN);

        public LinearLayout ReportProgress => FindViewById<LinearLayout>(Resource.Id.reportLinearProgress);
        #endregion

        #region Constructors
        public ReportDialouge(Context context) : base(context)
        {
            DialougeContext = context;
            SetContentView(Resource.Layout.report_selection_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            PopulateSpinners();
            BtnCancel.Click += (obj, _event) => { Dismiss(); };
            BTNClose.Click += (obj, _event) => { Dismiss(); };

            onRadioButtonClicked();
        }

        public ReportDialouge(Context context, int themeResId) : base(context, themeResId)
        {
            DialougeContext = context;
            SetContentView(Resource.Layout.report_selection_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            PopulateSpinners();
            BtnCancel.Click += (obj, _event) => { Dismiss(); };
            BTNClose.Click += (obj, _event) => { Dismiss(); };

            onRadioButtonClicked();
        }

        protected ReportDialouge(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
         
            SetContentView(Resource.Layout.report_selection_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            PopulateSpinners();
            BtnCancel.Click += (obj, _event) => { Dismiss(); };
            BTNClose.Click += (obj, _event) => { Dismiss(); };

            onRadioButtonClicked();
        }

        protected ReportDialouge(Context context, bool cancelable, EventHandler cancelHandler) : base(context, cancelable, cancelHandler)
        {
            DialougeContext = context;
            SetContentView(Resource.Layout.report_selection_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            PopulateSpinners();
            BtnCancel.Click += (obj, _event) => { Dismiss(); };
            BTNClose.Click += (obj, _event) => { Dismiss(); };

            onRadioButtonClicked();
        }

        protected ReportDialouge(Context context, bool cancelable, IDialogInterfaceOnCancelListener cancelListener) : base(context, cancelable, cancelListener)
        {
            DialougeContext = context;
            SetContentView(Resource.Layout.report_selection_view);
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            PopulateSpinners();
            BtnCancel.Click += (obj, _event) => { Dismiss(); };
            BTNClose.Click += (obj, _event) => { Dismiss(); };

            onRadioButtonClicked();
        }
        #endregion

        public void ItemSelected(View view) {
            var button = view as RadioButton;
            SelectedReportMessage = button.Text;
            if (button.Id.Equals(Resource.Id.rb_1) && button.Checked)
            {
                SP1.Visibility = ViewStates.Visible;
            }
            else {
                SP1.Visibility = ViewStates.Gone;
                SP1.SetSelection(0);
            }

            if (button.Id.Equals(Resource.Id.rb_2) && button.Checked)
            {
                SP2.Visibility = ViewStates.Visible;
            }
            else
            {
                SP2.Visibility = ViewStates.Gone;
                SP2.SetSelection(0);
            }

            if (button.Id.Equals(Resource.Id.rb_3) && button.Checked)
            {
                SP3.Visibility = ViewStates.Visible;
            }
            else
            {
                SP3.Visibility = ViewStates.Gone;
                SP3.SetSelection(0);
            }

            if (button.Id.Equals(Resource.Id.rb_4) && button.Checked)
            {
                SP4.Visibility = ViewStates.Visible;
            }
            else
            {
                SP4.Visibility = ViewStates.Gone;
                SP4.SetSelection(0);
            }

            if (button.Id.Equals(Resource.Id.rb_7) && button.Checked)
            {
                SP7.Visibility = ViewStates.Visible;
            }
            else
            {
                SP7.Visibility = ViewStates.Gone;
                SP7.SetSelection(0);
            }

            if (button.Id.Equals(Resource.Id.rb_8) && button.Checked)
            {
                SP8.Visibility = ViewStates.Visible;
            }
            else
            {
                SP8.Visibility = ViewStates.Gone;
                SP8.SetSelection(0);
            }    
        }

        private void PopulateSpinners() {
            var SP1Items = new string[] { "Choose One", "Graphic sexual activity", "Nudity", "Suggestive but without nudity", "Content involving minors", "Abusive title or description","Other sexual content"  };
            var SP2Items = new string[] { "Choose One", "Adults fighting", "Physical Attack", "Youth Violence","Animal Abuse" };
            var SP3Items = new string[] { "Choose One", "Promotes hatred or violence", "Abusing vulnerable individuals","Bullying","Abusive title or description" };
            var SP4Items = new string[] { "Choose One", "Pharmaceutical or drug abuse", "Abuse of fire or explosive", "Suicide or self injury","Other dangerous acts" };
            var SP7Items = new string[] { "Choose One", "Mass Advertising", "Pharmaceutical drugs for sale","Misleading text","Misleading Image","Scams/Fraud"  };
            var SP8Items = new string[] { "Choose One", "Infringes my copyright" , "Invades my privacy","Other legal claim" };

            var sp1adapter = new BITAdapter(Context,Resource.Layout.support_simple_spinner_dropdown_item,SP1Items);
            var sp2adapter = new BITAdapter(Context, Resource.Layout.support_simple_spinner_dropdown_item, SP2Items);
            var sp3adapter = new BITAdapter(Context, Resource.Layout.support_simple_spinner_dropdown_item, SP3Items);
            var sp4adapter = new BITAdapter(Context, Resource.Layout.support_simple_spinner_dropdown_item, SP4Items);
            var sp7adapter = new BITAdapter(Context, Resource.Layout.support_simple_spinner_dropdown_item, SP7Items);
            var sp8adapter = new BITAdapter(Context, Resource.Layout.support_simple_spinner_dropdown_item, SP8Items);

            sp1adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            sp2adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            sp3adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            sp4adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            sp7adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            sp8adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

            SP1.Adapter = sp1adapter;
            SP2.Adapter = sp2adapter;
            SP3.Adapter = sp3adapter;
            SP4.Adapter = sp4adapter;
            SP7.Adapter = sp7adapter;
            SP8.Adapter = sp8adapter;

            SP1.ItemSelected += SpinnerItemSelected;
            SP2.ItemSelected += SpinnerItemSelected;
            SP3.ItemSelected += SpinnerItemSelected;
            SP4.ItemSelected += SpinnerItemSelected;
            SP7.ItemSelected += SpinnerItemSelected;
            SP8.ItemSelected += SpinnerItemSelected;
        }

        private void SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = sender as Spinner;
            SelectedReportMessage = spinner.SelectedItem.ToString();
        }

        private void radioButtonCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var rb = sender as RadioButton;
            ColorStateList colorStateList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(rb.Context, Resource.Color.DIM_GREY)));
            if (rb.Checked)
            {
                colorStateList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(rb.Context, Resource.Color.colorPrimary)));
                ItemSelected(rb);
            }

            rb.ButtonTintList = colorStateList;
            rb.SetTextColor(colorStateList);
        }
        private void onRadioButtonClicked()
        {
            RB1.CheckedChange += radioButtonCheckedChange;

            RB2.CheckedChange += radioButtonCheckedChange;

            RB3.CheckedChange += radioButtonCheckedChange;

            RB4.CheckedChange += radioButtonCheckedChange;

            RB5.CheckedChange += radioButtonCheckedChange;

            RB6.CheckedChange += radioButtonCheckedChange;

            RB7.CheckedChange += radioButtonCheckedChange;

            RB8.CheckedChange += radioButtonCheckedChange;
        }
    }
}