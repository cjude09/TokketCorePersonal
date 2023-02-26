using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Android.Listener;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Newtonsoft.Json;
using Android.Graphics;
using Android.Text;
using Tokket.Android.Adapters;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;
using Android.Content.PM;
using Android.Content.Res;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Tokket.Android
{
    [Activity(Label = "Invite Users", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class InviteUsersActivity : BaseActivity
    {
        internal static InviteUsersActivity Instance { get; private set; }
        ObservableCollection<TokketUser> UsersCollection;
        ObservableCollection<ClassGroupRequestModel> UserRequestsCollection;
        ClassGroupModel model;
        GridLayoutManager mLayoutManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.inviteusers_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;

            UserRequestsCollection = new ObservableCollection<ClassGroupRequestModel>();
            UsersCollection = new ObservableCollection<TokketUser>();
            mLayoutManager = new GridLayoutManager (this,1);
            RecyclerInviteList.SetLayoutManager(mLayoutManager);

            model = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("ClassGroupModel"));

            RunOnUiThread(async () => await InitializeRequests(""));

            Typeface font = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            BtnSearch.SetTypeface(font, TypefaceStyle.Bold);

            BtnSearch.Click += async (object sender, EventArgs e) =>
            {
                await Initialize(isSearch:true);
            };

            if (RecyclerInviteList != null)
            {
                RecyclerInviteList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerInviteList.ContentDescription))
                    {
                        await Initialize(false, RecyclerInviteList.ContentDescription);
                    }
                };


                RecyclerInviteList.AddOnScrollListener(onScrollListener);

                RecyclerInviteList.SetLayoutManager(mLayoutManager);
            }
        }
        private async Task InitializeRequests(string continuationtoken, bool OnCreateViewLoad = true)
        {
            var resultRequests = await ClassService.Instance.GetClassGroupJoinRequests(continuationtoken, model.Id, RequestStatus.PendingInvite);
            var classgroupResult = resultRequests.Results.ToList();
            foreach (var item in classgroupResult)
            {
                UserRequestsCollection.Add(item);
            }

            if (!string.IsNullOrEmpty(resultRequests.ContinuationToken))
            {
                await InitializeRequests(resultRequests.ContinuationToken,false);
            }

            if (OnCreateViewLoad)
            {
                await Initialize();
            }
        }
        private async Task Initialize(bool isSearch = false, string continuationToken = "")
        {
            var result = await CommonService.Instance.SearchUsersAsync(EditSearch.Text, continuationToken);
            RecyclerInviteList.ContentDescription = result.ContinuationToken;
            var usersResult = result.Results.ToList();

            if (isSearch)
            {
                UsersCollection.Clear();
            }
            foreach (var item in usersResult)
            {
                UsersCollection.Add(item);
            }

            
            var adapterRecycler = new InviteUserAdapter(UsersCollection, UserRequestsCollection, model);
            RecyclerInviteList.SetAdapter(adapterRecycler);
        }
        [Java.Interop.Export("OnClickBtnInvite")]
        public async void OnClickBtnInvite(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }
            string alertmessage = "";

            View view = RecyclerInviteList.GetChildAt(position);
            var ProgressCircle = view.FindViewById<ProgressBar>(Resource.Id.ProgressInviteUsers);
            var BtnInvite = view.FindViewById<Button>(Resource.Id.BtnSubmitInvite);

            if (BtnInvite.Text.ToLower() == "invite")
            {
                var modelitem = new ClassGroupRequestModel();
                modelitem.ReceiverId = UsersCollection[position].Id;
                modelitem.SenderId = Settings.GetUserModel().UserId;
                modelitem.Name = model.Name;
                modelitem.Status = "1";
                modelitem.Remarks = "Invited";
                modelitem.Message = "You are invited to join group " + model.Name;
                modelitem.GroupId = model.Id;
                modelitem.GroupPartitionKey = model.PartitionKey;

                BtnInvite.Visibility = ViewStates.Gone;
                ProgressCircle.Visibility = ViewStates.Visible;
                var resultItem = await ClassService.Instance.RequestClassGroupAsync(modelitem);
                ProgressCircle.Visibility = ViewStates.Gone;
                BtnInvite.Visibility = ViewStates.Visible;
                
                if (resultItem != null)
                {
                    BtnInvite.Text = "Cancel";
                    BtnInvite.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#dc3545"));
                    alertmessage = "Invite sent.";
                    UserRequestsCollection.Insert(0, resultItem);
                }
                else
                {
                    alertmessage = "Failed to send an invite.";
                }


            }
            else if (BtnInvite.Text.ToLower() == "cancel")
            {
                //Find the id
                int ndx = 0;
                var resultRequest = UserRequestsCollection.FirstOrDefault(c => c.ReceiverId == UsersCollection[position].Id);
                if (resultRequest != null) //If Edit
                {
                    ndx = UserRequestsCollection.IndexOf(resultRequest);
                    BtnInvite.Visibility = ViewStates.Gone;

                    ProgressCircle.Visibility = ViewStates.Visible;
                    var isSuccess = await ClassService.Instance.DeclineRequest(UserRequestsCollection[ndx].Id, UserRequestsCollection[ndx].PartitionKey, model);
                    ProgressCircle.Visibility = ViewStates.Gone;
                    BtnInvite.Visibility = ViewStates.Visible;

                    if (isSuccess)
                    {
                        BtnInvite.Text = "Invite";
                        BtnInvite.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#007bff"));
                        alertmessage = "Cancelled Successfully.";
                    }
                    else
                    {
                        alertmessage = "Failed to decline.";
                    }
                }
            }
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle("");
            alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDialog.SetMessage(alertmessage);
            alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
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
        public EditText EditSearch => this.FindViewById<EditText>(Resource.Id.EditSearchInviteUsers);
        public Button BtnSearch => this.FindViewById<Button>(Resource.Id.BtnSearchInviteUsers);
        public RecyclerView RecyclerInviteList => this.FindViewById<RecyclerView>(Resource.Id.RecyclerInviteUsersList);
    }
}