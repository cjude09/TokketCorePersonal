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
using Android.Text;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Requests", Theme = "@style/CustomAppThemeBlue")]
    public class RequestsDialogActivity : BaseActivity
    {
        ClassGroupModel classGroupModel;
        ObservableCollection<ClassGroupRequestModel> UserRequestsCollection;
        internal static RequestsDialogActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.classgroupmember_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;
            classGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("classGroupModel"));
            UserRequestsCollection = new ObservableCollection<ClassGroupRequestModel>();
            RecyclerRequests.SetLayoutManager(new LinearLayoutManager(this));

            this.RunOnUiThread(async () => await InitializeRequests());
        }
        private async Task InitializeRequests()
        {
            var resultRequests = await ClassService.Instance.GetClassGroupJoinRequests(RecyclerRequests.ContentDescription, classGroupModel.Id);
            RecyclerRequests.ContentDescription = resultRequests.ContinuationToken;
            var classgroupResult = resultRequests.Results.ToList();

            foreach (var item in classgroupResult)
            {
                UserRequestsCollection.Add(item);
            }
            SetRecyclerRequestsAdapter();
        }
        private void SetRecyclerRequestsAdapter()
        {
            var adapterClassGroup = UserRequestsCollection.GetRecyclerAdapter(BindRequestsViewHolder, Resource.Layout.userrequests_row);
            RecyclerRequests.SetAdapter(adapterClassGroup);
        }

        private void BindRequestsViewHolder(CachingViewHolder holder, ClassGroupRequestModel model, int position)
        {
            var ImgRequestUserphoto = holder.FindCachedViewById<ImageView>(Resource.Id.ImgRequestUserphoto);
            var TextUserRequestName = holder.FindCachedViewById<TextView>(Resource.Id.TextUserRequestName);
            var BtnAccept = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestAccept);
            var btnDecline = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestDecline);

            Glide.With(this).Load(model.SenderImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(ImgRequestUserphoto);
            TextUserRequestName.Text = model.SenderDisplayName;

            BtnAccept.Tag = position;
            BtnAccept.Click -= BtnAcceptRequestClick;
            BtnAccept.Click += BtnAcceptRequestClick;

            btnDecline.Tag = position;
            btnDecline.Click -= BtnDeclineRequestClick;
            btnDecline.Click += BtnDeclineRequestClick;
        }

        private async void BtnAcceptRequestClick(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }

            HorizontalProgress.Visibility = ViewStates.Visible;
            var isSuccess = await ClassService.Instance.AcceptRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, classGroupModel);
            HorizontalProgress.Visibility = ViewStates.Gone;

            string alertmessage = "";
            if (isSuccess)
            {
                alertmessage = "Successfully accepted request.";
                var tokketUser = new TokketUser();
                tokketUser.UserPhoto = UserRequestsCollection[position].SenderImage;
                tokketUser.DisplayName = UserRequestsCollection[position].SenderDisplayName;
                //MembersCollection.Insert(0, tokketUser);
                UserRequestsCollection.RemoveAt(position);
            }
            else
            {
                alertmessage = "Failed to accept request.";
            }

            var dialog = new AlertDialog.Builder(ClassGroupActivity.Instance);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle("");
            alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDialog.SetMessage(alertmessage);
            alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
            {
                if (isSuccess)
                {
                    SetRecyclerRequestsAdapter();
                }
            });
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        private async void BtnDeclineRequestClick(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }


            HorizontalProgress.Visibility = ViewStates.Visible;
            var isSuccess = await ClassService.Instance.DeclineRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, classGroupModel);
            HorizontalProgress.Visibility = ViewStates.Gone;

            string alertmessage = "";
            if (isSuccess)
            {
                alertmessage = "Successfully declined request.";
            }
            else
            {
                alertmessage = "Failed to decline request.";
            }

            var dialog = new AlertDialog.Builder(ClassGroupActivity.Instance);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle("");
            alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDialog.SetMessage(alertmessage);
            alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
            {
                if (isSuccess)
                {
                    UserRequestsCollection.RemoveAt(position);
                    SetRecyclerRequestsAdapter();
                }
            });
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
        public RecyclerView RecyclerRequests => this.FindViewById<RecyclerView>(Resource.Id.RecyclerMembersRequestsList);
        public ProgressBar HorizontalProgress => this.FindViewById<ProgressBar>(Resource.Id.progressClassGroupMembers);
    }
}