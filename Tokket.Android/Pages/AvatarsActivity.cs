using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Android.Content.PM;
using AndroidX.RecyclerView.Widget;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    /*#if (_TOKKEPEDIA)
        [Activity(Label = "Select an avatar:", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif

    #if (_CLASSTOKS)
        [Activity(Label = "Select an avatar:", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif*/
    [Activity(Label = "Select an avatar:", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AvatarsActivity : BaseActivity
    {
        internal static AvatarsActivity Instance { get; private set; }
        public ObservableCollection<Avatar> AvatarsCollection;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.avatarspage); 
            SetSupportActionBar(toolBar);
            setActivityTitle("Select an avatar:");
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            showBlueLoading(this);
            RecyclerAvatarsPage.SetLayoutManager(new GridLayoutManager(this, 4));
            AvatarsCollection = new ObservableCollection<Avatar>();
            AvatarsCollection.Clear();

            var resultAvatars = await AvatarsService.Instance.GetAvatarsAsync();
            var resultList = resultAvatars.Results.ToList();
            foreach (var avatars in resultList)
            {
                AvatarsCollection.Add(avatars);
            }

            var adapterTokMoji = new AvatarAdapter(AvatarsCollection);
            RecyclerAvatarsPage.SetAdapter(adapterTokMoji);
            SaveCommand.Enabled = false;
            SaveCommand.Click += delegate
            {
                int selectedPosition = (int)SaveCommand.Tag;
                if (selectedPosition >= 0)
                {
                    AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
                    alertDiag.SetTitle("This cost 10 coins.");
                    alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertDiag.SetMessage("Usage will end when changing to a profile picture or another avatar.");
                    alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                        alertDiag.Dispose();
                    });
                    alertDiag.SetPositiveButton(Html.FromHtml("<font color='#007bff'>Continue with Coin Purchase</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {

                        int positionAvatar = (int)SaveCommand.Tag;
                        Avatar AvatarSelected = AvatarsCollection[positionAvatar];

                        LinearProgress.Visibility = ViewStates.Visible;
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                        TextProgressStatus.Text = "Purchasing...";
                        var resultbool = await AvatarsService.Instance.SelectAvatarAsync(AvatarSelected.Id);
                        TextProgressStatus.Text = "Loading...";
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        LinearProgress.Visibility = ViewStates.Gone;

                        string message = "";
                        if (resultbool)
                        {
                            Settings.UserCoins -= 10;
                            MainActivity.Instance.TextCoinsToast.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);
                            ProfileFragment.Instance.TextProfileCoins.Text = string.Format("{0:#,##0.##}", Settings.UserCoins);

                            await AvatarsService.Instance.UserSelectAvatarAsync(AvatarSelected.Id);
                            await AvatarsService.Instance.UseAvatarAsProfilePictureAsync(true);
                            message = "Avatar selected successfully!";

                            var tokketUser = Settings.GetTokketUser();
                            tokketUser.UserPhoto = AvatarSelected.Image;
                            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                        }
                        else
                        {
                            message = "Failed to select.";
                        }

                        var dialog = new AlertDialog.Builder(this);
                        var alertDialog = dialog.Create();
                        alertDialog.SetTitle("");
                        alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertDialog.SetMessage(message);
                        alertDialog.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => 
                        {
                            if (resultbool)
                            {
                                var avatarConvert = JsonConvert.SerializeObject(AvatarSelected);
                                Intent = new Intent();
                                Intent.PutExtra("Avatar", avatarConvert);
                                SetResult(AppResult.Ok, Intent);
                                Finish();
                            }
                        });
                        alertDialog.Show();
                        alertDialog.SetCanceledOnTouchOutside(false);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                }
            };
            hideBlueLoading(this);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public RecyclerView RecyclerAvatarsPage => FindViewById<RecyclerView>(Resource.Id.RecyclerAvatarsPage); 
        public Button SaveCommand => FindViewById<Button>(Resource.Id.btnAvatarsSave);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_AvatarProgress);
        public TextView TextProgressStatus => FindViewById<TextView>(Resource.Id.TextProgressStatus);

        public LinearLayout LinearProgress2 => FindViewById<LinearLayout>(Resource.Id.linear_BadgeProgress);
    }
}