using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Tokket.Core;

namespace Tokket.Android.Fragments
{
    public class PatchesSettingsFragment : AndroidX.Fragment.App.Fragment
    {
        View v;
        private string titlepage = "";
        public PatchesSettingsFragment(string title)
        {
            titlepage = title;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.fragment_patches_settings, container, false);

            var User = Settings.GetTokketUser();
            chkEnablePatch.Checked = false;
            if (User.IsPointsSymbolEnabled != null)
            {
                chkEnablePatch.Checked = (bool)User.IsPointsSymbolEnabled;
            }

            btnSave.Click += async (s, e) =>
            {
                PatchesActivity.Instance.showBlueLoading(PatchesActivity.Instance);
                var result = await PatchService.Instance.UpdateUserPointsSymbolEnabledAsync(chkEnablePatch.Checked, User);
                PatchesActivity.Instance.hideBlueLoading(PatchesActivity.Instance);

                string message = "Success!";
                if (!result)
                    message = "Failed!";

                User.IsPointsSymbolEnabled = chkEnablePatch.Checked;
                Settings.TokketUser = JsonConvert.SerializeObject(User);

                MainActivity.Instance.ShowLottieMessageDialog(RequireContext(), message, result, handlerOkClick: (s, e) =>
                {
                    ProfileFragment.Instance.classtokDataAdapter.NotifyDataSetChanged();
                    ClassToksFragment.Instance.ClassTokDataAdapter.NotifyDataSetChanged();
                });
            };
            return v;
        }

        public Button btnSave => v.FindViewById<Button>(Resource.Id.btnSave);
        public CheckBox chkEnablePatch => v.FindViewById<CheckBox>(Resource.Id.chkEnablePatch);
    }
}