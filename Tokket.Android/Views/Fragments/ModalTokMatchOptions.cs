using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Helpers;
using Tokket.Core;

namespace Tokket.Android.Fragments
{
    public class ModalTokMatchOptions : AndroidX.AppCompat.App.AppCompatDialogFragment
    {
        TokMatchActivity tokMatchActivity;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            tokMatchActivity = Activity as TokMatchActivity;
            var v = inflater.Inflate(Resource.Layout.layout_modal_tokmatch_option, container, false);
            var btnClose = v.FindViewById<Button>(Resource.Id.btnClose);
            var btnSwitchPrimarySecondary = v.FindViewById<Button>(Resource.Id.btnSwitchPrimarySecondary);
            var radioButtonTFOff = v.FindViewById<RadioButton>(Resource.Id.radioButtonTFOff);
            var radioButtonTFOnly = v.FindViewById<RadioButton>(Resource.Id.radioButtonTFOnly);
            var radioButtonExcludeAllTF = v.FindViewById<RadioButton>(Resource.Id.radioButtonExcludeAllTF);
            var switchMultiLine = v.FindViewById<Switch>(Resource.Id.switchMultiLine);
            var switchRetryIncorrect = v.FindViewById<Switch>(Resource.Id.switchRetryIncorrect);

            Bundle mArgs = Arguments;
            var trueFalseMode = mArgs.GetInt("trueFalseMode");
            var isMultilinetext = mArgs.GetBoolean("isMultilinetext");
            var isRetryOnlyIncorrectMode = mArgs.GetBoolean("isRetryOnlyIncorrectMode");
            var isNormalMode = mArgs.GetBoolean("isNormalMode");

            if (!isNormalMode)
            {
                switchRetryIncorrect.Visibility = ViewStates.Gone;
            }

            switchRetryIncorrect.Checked = isRetryOnlyIncorrectMode;

            if (trueFalseMode == (int)TrueFalseMode.TFOnly)
            {
                radioButtonTFOnly.Checked = true;
            }
            else if(trueFalseMode == (int)TrueFalseMode.ExcludeAllTF)
            {
                radioButtonExcludeAllTF.Checked = true;
            }
            else
            {
                radioButtonTFOff.Checked = true;
            }

            switchMultiLine.Checked = isMultilinetext;

            btnClose.Click += delegate
            {
                Dismiss();
            };

            btnSwitchPrimarySecondary.Click += (object sender, EventArgs e) =>
            {
                tokMatchActivity.switchPrimarySecondary();
            };

            radioButtonTFOff.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokMatchActivity.TrueFalseModeToks(TrueFalseMode.Off);
                Dismiss();
            };

            radioButtonTFOnly.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokMatchActivity.TrueFalseModeToks(TrueFalseMode.TFOnly);
                Dismiss();
            };

            radioButtonExcludeAllTF.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokMatchActivity.TrueFalseModeToks(TrueFalseMode.ExcludeAllTF);
                Dismiss();
            };

            switchMultiLine.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokMatchActivity.isMultiLineTexts(switchMultiLine.Checked);
                Dismiss();
            };

            switchRetryIncorrect.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                tokMatchActivity.switchRetryOnlyIncorrectMode(switchRetryIncorrect.Checked);
            };
            return v;
        }
    }
}