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
using Tokket.Core;

namespace Tokket.Android.Custom
{
    public class NameplatePositionDialog : Dialog
    {
        private HandlePosition handlePosition = HandlePosition.None;
        private ImageButton imageBtnClose;
        private Button btnSaveChanges;
        private CheckBox chkOptionA, chkOptionB, chkOptionC, chkOptionD;
        public NameplatePositionDialog(Context context) : base(context)
        {
            SetContentView(Resource.Layout.dialog_nameplate_position);

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            chkOptionA = FindViewById<CheckBox>(Resource.Id.chkOptionA);
            chkOptionB = FindViewById<CheckBox>(Resource.Id.chkOptionB);
            chkOptionC = FindViewById<CheckBox>(Resource.Id.chkOptionC);
            chkOptionD = FindViewById<CheckBox>(Resource.Id.chkOptionD);

            imageBtnClose = FindViewById<ImageButton>(Resource.Id.imageBtnClose);
            btnSaveChanges = FindViewById<Button>(Resource.Id.btnSaveChanges);

            chkOptionA.CheckedChange += delegate
            {
                if (chkOptionA.Checked)
                {
                    handlePosition = HandlePosition.OptionA;
                    chkOptionB.Checked = false;
                    chkOptionC.Checked = false;
                    chkOptionD.Checked = false;
                }
            };

            chkOptionB.CheckedChange += delegate
            {
                if (chkOptionB.Checked)
                {
                    handlePosition = HandlePosition.OptionB;
                    chkOptionA.Checked = false;
                    chkOptionC.Checked = false;
                    chkOptionD.Checked = false;
                }
            };

            chkOptionC.CheckedChange += delegate
            {
                if (chkOptionC.Checked)
                {
                    handlePosition = HandlePosition.OptionC;
                    chkOptionA.Checked = false;
                    chkOptionB.Checked = false;
                    chkOptionD.Checked = false;
                }
            };

            chkOptionD.CheckedChange += delegate
            {
                if (chkOptionD.Checked)
                {
                    handlePosition = HandlePosition.OptionD;
                    chkOptionA.Checked = false;
                    chkOptionC.Checked = false;
                    chkOptionB.Checked = false;
                }
            };

            btnSaveChanges.Click += delegate
            {
                NameplateSettingsUIActivity.Instance.SelectedPosition(handlePosition);
                Dismiss();
            };

            imageBtnClose.Click += delegate
            {
                Dismiss();
            };
        }
    }
}