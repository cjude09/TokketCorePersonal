using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using Xamarin.Essentials;

namespace Tokket.Android
{
    [Activity(Label = "Notifications", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class InvitesActivity : BaseActivity
    {
        ObservableCollection<TokketUser> UsersCollection;
        ObservableCollection<ClassGroupRequestModel> InvitesCollection;
        internal static InvitesActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_invites);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;

            InvitesCollection = new ObservableCollection<ClassGroupRequestModel>();
            UsersCollection = new ObservableCollection<TokketUser>();
            var mLayoutManager = new GridLayoutManager(this, 1);
            recyclerInvites.SetLayoutManager(mLayoutManager);

            RunOnUiThread(async () => await InitializeInvites("", true));

            swipRefreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            swipRefreshLayout.Refresh -= RefreshLayout_Refresh;
            swipRefreshLayout.Refresh += RefreshLayout_Refresh;
        }

        private async Task InitializeInvites(string continuationtoken, bool OnCreateViewLoad = true)
        {
            var values = new ClassGroupRequestQueryValues();
            values.receiverid = Settings.GetUserModel().UserId;
            values.status = RequestStatus.PendingInvite;
            values.partitionkeybase = "classgrouprequests";


            var resultRequests = await ClassService.Instance.GetClassGroupRequestAsync(values);
            var classgroupResult = resultRequests.Results.ToList();
            foreach (var item in classgroupResult)
            {
                InvitesCollection.Add(item);
            }

            if (!string.IsNullOrEmpty(resultRequests.ContinuationToken))
            {
                await InitializeInvites(resultRequests.ContinuationToken, false);
            }

            if (OnCreateViewLoad)
            {
                await Initialize();
            }
        }

        private async Task Initialize(bool isSearch = false, string continuationToken = "")
        {
            var result = await CommonService.Instance.SearchUsersAsync("", continuationToken);
            swipRefreshLayout.Refreshing = false;

            recyclerInvites.ContentDescription = result.ContinuationToken;
            var usersResult = result.Results.ToList();

            if (isSearch)
            {
                UsersCollection.Clear();
            }
            foreach (var item in usersResult)
            {
                UsersCollection.Add(item);

            }

            if (InvitesCollection.Count > 0)
            {
                txtNoInvites.Visibility = ViewStates.Gone;
                var adapterRecycler = new InviteUserAdapter(InvitesCollection, null,2);
                recyclerInvites.SetAdapter(adapterRecycler);
            }
        }


        public void RefreshAdapter() {
            if (InvitesCollection.Count > 0)
            {
                txtNoInvites.Visibility = ViewStates.Gone;
                var adapterRecycler = new InviteUserAdapter(InvitesCollection, null, 2);
                recyclerInvites.SetAdapter(adapterRecycler);
            }
            else {
                txtNoInvites.Visibility = ViewStates.Visible;
            }
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

        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
            else
            {
                swipRefreshLayout.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //swipRefreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            UsersCollection.Clear();
            InvitesCollection.Clear();
            RunOnUiThread(async () => await InitializeInvites("", true));
        }

        public RecyclerView recyclerInvites => FindViewById<RecyclerView>(Resource.Id.recyclerInvites);
        public TextView txtNoInvites => FindViewById<TextView>(Resource.Id.txtNoInvites);
        public SwipeRefreshLayout swipRefreshLayout => FindViewById<SwipeRefreshLayout>(Resource.Id.swipRefreshLayout);
    }
}