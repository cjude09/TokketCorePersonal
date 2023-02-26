#define _TOKKEPEDIA

#if _TOKKEPEDIA

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Tokket.Shared.Models.Notification;
using Android.Text;
using Newtonsoft.Json;
using System.Threading;
using System.ComponentModel;
using AndroidX.Fragment.App;
using Color = Android.Graphics.Color;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android.Fragments
{
    public class NotificationFragment : AndroidX.Fragment.App.Fragment
    {
        View v;
        private FragmentActivity myContext;
        internal static NotificationFragment NFInstance { get; private set; }
        ObservableCollection<TokkepediaNotificationNew> NotifCollection { get; set; }
        List<TokkepediaNotificationNew> ListNotifications;
        SwipeRefreshLayout refreshLayout = null;
        ObservableRecyclerAdapter<TokkepediaNotificationNew, CachingViewHolder> adapterNotifications;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            NFInstance = this;
            v = inflater.Inflate(Resource.Layout.notification_page, container, false);
            
            myContext = MainActivity.Instance;

            RecyclerList.SetLayoutManager(new GridLayoutManager(MainActivity.Instance, 1));

            ListNotifications = new List<TokkepediaNotificationNew>();
            NotifCollection = new ObservableCollection<TokkepediaNotificationNew>();

            MainActivity.Instance.RunOnUiThread(async () => await LoadNotifications());

            refreshLayout = v.FindViewById<SwipeRefreshLayout>(Resource.Id.home_swiperefresh_ListToks);
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            refreshLayout.Refresh += RefreshLayout_Refresh;

            return v;
        }

        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            //Data Refresh Place  
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.RunWorkerAsync();
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            MainActivity.Instance.RunOnUiThread(async () => await LoadNotifications());
            Thread.Sleep(1000);
        }

        private async Task LoadNotifications()
        {

            ProgressLoading.Visibility = ViewStates.Visible;
            var resultNotification = await NotificationService.Instance.GetNotif(Settings.GetTokketUser().Id);
            ProgressLoading.Visibility = ViewStates.Gone;

            ListNotifications = resultNotification.Results.ToList();
            RecyclerList.ContentDescription = resultNotification.ContinuationToken;

            foreach (var notif in ListNotifications)
            {
                NotifCollection.Add(notif);
            }
            SetNotifAdapter();
        }
        private void SetNotifAdapter()
        {
            adapterNotifications = NotifCollection.GetRecyclerAdapter(BindNotifsViewHolder, Resource.Layout.notifications_row);
            RecyclerList.SetAdapter(adapterNotifications);
        }

        private void BindNotifsViewHolder(CachingViewHolder holder, TokkepediaNotificationNew notification, int position)
        {
            var ProgressDelRow = holder.FindCachedViewById<ProgressBar>(Resource.Id.ProgressbarCircle);
            var GridBG = holder.FindCachedViewById<GridLayout>(Resource.Id.gridBackground);
            var GridBGDrawable = holder.FindCachedViewById<GridLayout>(Resource.Id.gridBackground);
            var UserPhoto = holder.FindCachedViewById<ImageView>(Resource.Id.img_UserPhoto);
            var TextUsername = holder.FindCachedViewById<TextView>(Resource.Id.TextUsername);
            var NotificationText = holder.FindCachedViewById<TextView>(Resource.Id.NotificationText);
            var TextDelete = holder.FindCachedViewById<TextView>(Resource.Id.TextDelete);
            var btnTok = holder.FindCachedViewById<LinearLayout>(Resource.Id.btnTok);
            var TextMarkAsRead = holder.FindCachedViewById<TextView>(Resource.Id.TextMarkAsRead);

            var progressCircle = holder.FindCachedViewById<ProgressBar>(Resource.Id.ProgressbarCircle);
            var progressBarinsideText = holder.FindCachedViewById<TextView>(Resource.Id.progressBarinsideText);


            //var TextAccountType = holder.FindCachedViewById<TextView>(Resource.Id.TextAccountType);
            var TextSelectedTitle = holder.FindCachedViewById<TextView>(Resource.Id.TextSelectedTitle);
            //var TextSubAccountName = holder.FindCachedViewById<TextView>(Resource.Id.TextSubAccountName);

            GridBG.SetBackgroundResource(Resource.Drawable.tileview_layout);
            GradientDrawable GridDrawable = (GradientDrawable)GridBGDrawable.Background;

            if (notification.IsRead)
            {
                TextMarkAsRead.Visibility = ViewStates.Gone;
                GridDrawable.SetColor(Color.LightGray);
            }
            else
            {
                TextMarkAsRead.Visibility = ViewStates.Visible;
                GridDrawable.SetColor(Color.White);
            }

            var font = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            TextDelete.Typeface = font;


            Glide.With(myContext).Load(notification.SenderPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(UserPhoto);
            UserPhoto.Click -= onRedirectProfile;
            UserPhoto.Click += onRedirectProfile;

            //TextAccountType.Text = notification.AccountType;
            TextSelectedTitle.Text = notification.AccountType == "individual" ? notification.TitleId: notification.SubaccountName;
            //TextSubAccountName.Text = " - " + notification.SubaccountName;

            btnTok.Tag = position;
            btnTok.Click -= OnGridBackgroundClick;
            btnTok.Click += OnGridBackgroundClick;

            TextMarkAsRead.Tag = position;
            TextMarkAsRead.Click -= OnGridBackgroundClick;
            TextMarkAsRead.Click += OnGridBackgroundClick;

            TextUsername.Text = notification.SenderDisplayName;
            TextUsername.Click -= onRedirectProfile;
            TextUsername.Click += onRedirectProfile;

            switch (notification.Kind)
            {
                case "accurate":
                case "accurate1":
                case "accurate2":
                case "accurate3":
                    NotificationText.Text = " gave your tok an accurate rating.";
                    break;
                case "inaccurate":
                case "inaccurate1":
                case "inaccurate2":
                case "inaccurate3":
                    NotificationText.Text = " gave your tok an inaccurate rating.";
                    break;
                case "comment":
                case "comment1":
                case "comment2":
                case "comment3":
                    NotificationText.Text = " commented on your tok.";
                    break;
                case "reply":
                    NotificationText.Text = " replied to your comment.";
                    break;
                case "gema":
                    NotificationText.Text = " gave a valuable gem to your tok.";
                    break;
                case "gemb":
                    NotificationText.Text = " gave a brilliant gem to your tok.";
                    break;
                case "gemc":
                    NotificationText.Text = " gave a precious gem to your tok.";
                    break;
                case "treasure":
                    NotificationText.Text = " gave a treasure chest to your tok.";
                    break;
                default:
                    NotificationText.Text = " viewed your tok.";
                    break;
            }

            TextDelete.Tag = position;
            TextDelete.Click -= DeleteRowNotif;
            TextDelete.Click += DeleteRowNotif;
        }
        public async void OnGridBackgroundClick(object sender, EventArgs e)
        {
            if (sender.GetType().ToString().EndsWith("TextView"))
            {
                int tokposition = (int)(sender as TextView).Tag;
                var view = RecyclerList.FindViewHolderForLayoutPosition(tokposition);
                var TextMarkAsRead = view.ItemView.FindViewById<TextView>(Resource.Id.TextMarkAsRead);
                var progressCircle = view.ItemView.FindViewById<ProgressBar>(Resource.Id.ProgressbarCircle);
                var progressBarinsideText = view.ItemView.FindViewById<TextView>(Resource.Id.progressBarinsideText);

                //show progressbar
                progressCircle.Visibility = ViewStates.Visible;
                progressBarinsideText.Visibility = ViewStates.Visible;
                progressBarinsideText.Text = "Mark as read...";

                var gridLayout = view.ItemView.FindViewById(Resource.Id.gridBackground);
                gridLayout.SetBackgroundResource(Resource.Drawable.tileview_layout);
                GradientDrawable GridDrawable = (GradientDrawable)gridLayout.Background;
                GridDrawable.SetColor(Color.LightGray);

                //mark notification as read   
                var item_id = adapterNotifications.GetItem(tokposition).ItemId;
                var tokModel = await TokService.Instance.GetTokIdAsync(item_id);
                await NotificationService.Instance.MarkAsReadAsync(tokModel.Id, tokModel.PartitionKey); 

                //hide progressbar
                progressCircle.Visibility = ViewStates.Invisible;
                progressBarinsideText.Visibility = ViewStates.Invisible;
                TextMarkAsRead.Visibility = ViewStates.Invisible;
            }
            else
            {
                int tokposition = (int)(sender as LinearLayout).Tag;
                Intent nextActivity = new Intent(NFInstance.Context, typeof(TokInfoActivity));

                var item_id = adapterNotifications.GetItem(tokposition).ItemId;
                var tokModel = await TokService.Instance.GetTokIdAsync(item_id);

                //mark as read (set background color to white)
                if (!adapterNotifications.GetItem(tokposition).IsRead)
                {
                    var view = RecyclerList.FindViewHolderForLayoutPosition(tokposition);
                    var gridLayout = view.ItemView.FindViewById(Resource.Id.gridBackground);
                    gridLayout.SetBackgroundResource(Resource.Drawable.tileview_layout);
                    GradientDrawable GridDrawable = (GradientDrawable)gridLayout.Background;
                    GridDrawable.SetColor(Color.LightGray);
                }

                if (tokModel != null)
                {

                    var modelConvert = JsonConvert.SerializeObject(tokModel);
                    nextActivity.PutExtra("tokModel", modelConvert);
                    this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
                }
            }
        }

        private void DeleteRowNotif(object sender, EventArgs e)
        {
            int position = (int)(sender as TextView).Tag;
            var view = RecyclerList.FindViewHolderForLayoutPosition(position);
            var progressCircle = view.ItemView.FindViewById<ProgressBar>(Resource.Id.ProgressbarCircle);
            var progressBarinsideText = view.ItemView.FindViewById<TextView>(Resource.Id.progressBarinsideText); 

            AlertDialog.Builder alertDiag = new AlertDialog.Builder(MainActivity.Instance);
            alertDiag.SetTitle("Confirm");
            alertDiag.SetMessage("Are you sure you want to delete this notification?");
            alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {
                string message = "";
                //show progressbar
                progressCircle.Visibility = ViewStates.Visible;
                progressBarinsideText.Visibility = ViewStates.Visible;
                progressBarinsideText.Text = "Deleting...";
                var issuccess = await NotificationService.Instance.RemoveNotificationsAsync(ListNotifications[position].Id, ListNotifications[position].PartitionKey);
                //hide progressbar
                progressCircle.Visibility = ViewStates.Invisible;
                progressBarinsideText.Visibility = ViewStates.Invisible;
                message = issuccess ? "Notification removed." : "Failed to delete!";
                
                var dialogDelete = new AlertDialog.Builder(MainActivity.Instance);
                var alertDelete = dialogDelete.Create();
                alertDelete.SetTitle("");
                alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                alertDelete.SetMessage(message);
                alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                {
                    if (issuccess)
                    {
                        RecyclerList.RemoveViewAt(position);
                        ListNotifications.RemoveAt(position);
                        NotifCollection.RemoveAt(position);
                        SetNotifAdapter();
                    }                        
                });
                alertDelete.Show();
                alertDelete.SetCanceledOnTouchOutside(false);
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        public void onRedirectProfile(object sender, EventArgs e)
        {
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", Settings.GetTokketUser().Id);
            MainActivity.Instance.StartActivity(nextActivity);
        }

        public RecyclerView RecyclerList => v.FindViewById<RecyclerView>(Resource.Id.RecyclerContainer);
        public ProgressBar ProgressLoading => v.FindViewById<ProgressBar>(Resource.Id.progressbar);
    }
}

#endif