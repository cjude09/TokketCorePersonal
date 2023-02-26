using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Android.Listener;
using AndroidX.Fragment.App;
using DE.Hdodenhof.CircleImageViewLib;
using Bumptech.Glide.Load.Resource.Drawable;
using Java.Net;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;
using AndroidX.AppCompat.Widget;

namespace Tokket.Android.Fragments
{
    public class ClassGroupMemberFragment : AndroidX.Fragment.App.Fragment
    {
        View v;
        ObservableCollection<TokketUser> MembersCollection;
        ClassGroupModel classGroupModel;
        GridLayoutManager mLayoutManager;
        private FragmentActivity myContext;
        public ClassGroupMemberFragment(ClassGroupModel _classGroupModel)
        {
            classGroupModel = _classGroupModel;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.classgroupmember_page, container, false);

            myContext = MainActivity.Instance;

            mLayoutManager = new GridLayoutManager(MainActivity.Instance, 1);
            RecyclerMembersRequestsList.SetLayoutManager(mLayoutManager);

            // MainActivity.Instance.RunOnUiThread(async () => await InitializeRequests());
            MainActivity.Instance.RunOnUiThread(async () => await InitializeMembers());

            MembersCollection = new ObservableCollection<TokketUser>();

            if (RecyclerMembersRequestsList != null)
            {
                RecyclerMembersRequestsList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerMembersRequestsList.ContentDescription))
                    {
                        await InitializeMembers();
                    }
                };


                RecyclerMembersRequestsList.AddOnScrollListener(onScrollListener);

                RecyclerMembersRequestsList.SetLayoutManager(mLayoutManager);
            }

            return v;
        }
        private async Task InitializeMembers()
        {
            var resultRequests = await ClassService.Instance.GetGroupMembers(classGroupModel.Id, RecyclerMembersRequestsList.ContentDescription);
            if (resultRequests == null)
            {
                return;
            }

            RecyclerMembersRequestsList.ContentDescription = resultRequests.ContinuationToken;
            var classgroupResult = resultRequests.Results.ToList();

            bool isLoggedInUserOwner = false;
            if (classGroupModel.UserId == Settings.GetUserModel().UserId)
            {
                isLoggedInUserOwner = true;
                MembersCollection.Add(Settings.GetTokketUser());
            }

            foreach (var item in classgroupResult)
            {
                if (item.Id == Settings.GetUserModel().UserId) //If user is member
                {
                    if (!isLoggedInUserOwner)
                    {
                        //If logged in user is not the group owner
                        MembersCollection.Add(item);
                    }

                    if (classGroupModel.IsMember == false)
                    {
                        ClassGroupActivity.Instance.requesttojoin.SetVisible(true);
                        ClassGroupActivity.Instance.requesttojoin.SetTitle("Leave Group");
                        ClassGroupActivity.Instance.model.IsMember = true;
                    }
                }
                else
                {
                    MembersCollection.Add(item);
                }
            }
            SetRecyclerMembersAdapter();
        }
        private void SetRecyclerMembersAdapter()
        {
            var adapterClassGroup = MembersCollection.GetRecyclerAdapter(BindMembersViewHolder, Resource.Layout.userrequests_row);
            RecyclerMembersRequestsList.SetAdapter(adapterClassGroup);

        }
        //private void BindRequestsViewHolder(CachingViewHolder holder, ClassGroupRequestModel model, int position)
        //{
        //    var ImgRequestUserphoto = holder.FindCachedViewById<ImageView>(Resource.Id.ImgRequestUserphoto);
        //    var TextUserRequestName = holder.FindCachedViewById<TextView>(Resource.Id.TextUserRequestName);
        //    var BtnAccept = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestAccept);
        //    var btnDecline = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestDecline);
        //
        //    Glide.With(this).Load(model.SenderImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(ImgRequestUserphoto);
        //    TextUserRequestName.Text = model.SenderDisplayName;
        //
        //    BtnAccept.Tag = position;
        //    BtnAccept.Click -= BtnAcceptRequestClick;
        //    BtnAccept.Click += BtnAcceptRequestClick;
        //
        //    btnDecline.Tag = position;
        //    btnDecline.Click -= BtnDeclineRequestClick;
        //    btnDecline.Click += BtnDeclineRequestClick;
        //}
        //private async void BtnAcceptRequestClick(object sender, EventArgs e)
        //{
        //    int position = 0;
        //    try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }
        //
        //    HorizontalProgress.Visibility = ViewStates.Visible;
        //    var isSuccess = await ClassService.Instance.AcceptRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, classGroupModel);
        //    HorizontalProgress.Visibility = ViewStates.Gone;
        //
        //    string alertmessage = "";
        //    if (isSuccess)
        //    {
        //        alertmessage =  "Successfully accepted request.";
        //        var tokketUser = new TokketUser();
        //        tokketUser.UserPhoto = UserRequestsCollection[position].SenderImage;
        //        tokketUser.DisplayName = UserRequestsCollection[position].SenderDisplayName;
        //        MembersCollection.Insert(0, tokketUser);
        //        UserRequestsCollection.RemoveAt(position);
        //    }
        //    else
        //    {
        //        alertmessage = "Failed to accept request.";
        //    }
        //
        //    var dialog = new AlertDialog.Builder(ClassGroupActivity.Instance);
        //    var alertDialog = dialog.Create();
        //    alertDialog.SetTitle("");
        //    alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
        //    alertDialog.SetMessage(alertmessage);
        //    alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => 
        //    {
        //        if (isSuccess)
        //        {
        //            SetRecyclerRequestsAdapter();
        //        }
        //    });
        //    alertDialog.Show();
        //    alertDialog.SetCanceledOnTouchOutside(false);
        //}
        //private async void BtnDeclineRequestClick(object sender, EventArgs e)
        //{
        //    int position = 0;
        //    try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }
        //
        //
        //    HorizontalProgress.Visibility = ViewStates.Visible;
        //    var isSuccess = await ClassService.Instance.DeclineRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, classGroupModel);
        //    HorizontalProgress.Visibility = ViewStates.Gone;
        //
        //    string alertmessage = "";
        //    if (isSuccess)
        //    {
        //        alertmessage = "Successfully declined request.";
        //    }
        //    else
        //    {
        //        alertmessage =  "Failed to decline request.";
        //    }
        //
        //    var dialog = new AlertDialog.Builder(ClassGroupActivity.Instance);
        //    var alertDialog = dialog.Create();
        //    alertDialog.SetTitle("");
        //    alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
        //    alertDialog.SetMessage(alertmessage);
        //    alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => 
        //    {
        //        if (isSuccess)
        //        {
        //            UserRequestsCollection.RemoveAt(position);
        //            SetRecyclerRequestsAdapter();
        //        }
        //    });
        //    alertDialog.Show();
        //    alertDialog.SetCanceledOnTouchOutside(false);
        //}

        private void BindMembersViewHolder(CachingViewHolder holder, TokketUser userModel, int position)
        {
            var ImgRequestUserphoto = holder.FindCachedViewById<AppCompatImageView>(Resource.Id.ImgRequestUserphoto);
            var TextUserRequestName = holder.FindCachedViewById<TextView>(Resource.Id.TextUserRequestName);
            var BtnAccept = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestAccept);
            var BtnDelete = holder.FindCachedViewById<Button>(Resource.Id.BtnUserRequestDecline);
            var LabelOwner = holder.FindCachedViewById<TextView>(Resource.Id.LabelUserOwner);
            var LinearButtons = holder.FindCachedViewById<LinearLayout>(Resource.Id.LinearButtons);
            var menuButton = holder.FindCachedViewById<Button>(Resource.Id.btn_menu);  
            LinearButtons.Visibility = ViewStates.Gone;
            if (userModel.Id == classGroupModel.UserId)
            {
                LabelOwner.Visibility = ViewStates.Visible;
            }
            else {
                menuButton.Visibility = ViewStates.Gone;
            }
            ImgRequestUserphoto.ContentDescription = position.ToString();
            TextUserRequestName.ContentDescription = position.ToString();

            ImgRequestUserphoto.Click -= onImageUsernameClick;
            ImgRequestUserphoto.Click += onImageUsernameClick;

            TextUserRequestName.Click -= onImageUsernameClick;
            TextUserRequestName.Click += onImageUsernameClick;
            menuButton.Click += (obj, _event) => {

                AlertDialog.Builder alertDiag;
                Dialog diag; alertDiag = new AlertDialog.Builder(this.Activity);
                alertDiag.SetTitle("Confirm");
                alertDiag.SetMessage("Are you sure you want to remove " + userModel.DisplayName + " from this group?");
                alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Remove</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                {
                    ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;
                    var result = await ClassService.Instance.RemoveMemberClassGroupAsync(userModel.Id, userModel.PartitionKey, classGroupModel);
                    if (result)
                    {

                        MembersCollection.Remove(userModel);
                        SetRecyclerMembersAdapter();
                    }
                    else
                    {

                    }
                    ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;
                });
                diag = alertDiag.Create();
                diag.Show();
                diag.SetCanceledOnTouchOutside(false);


                //var menu = new PopupMenu(this.Context, menuButton);

                //// Call inflate directly on the menu:
                //menu.Inflate(Resource.Menu.member_menu);

                //var delete = menu.Menu.FindItem(Resource.Id.removemember);
                //var leave = menu.Menu.FindItem(Resource.Id.leavemember);
                //delete.SetVisible(true);
                //leave.SetVisible(false);
                //menu.MenuItemClick += async (obj, _event) => {
                //    switch (_event.Item.TitleFormatted.ToString().ToLower())
                //    {
                //        case "remove":

                //                AlertDialog.Builder alertDiag;
                //                Android.App.Dialog diag; alertDiag = new AlertDialog.Builder(this.Activity);
                //                alertDiag.SetTitle("Confirm");
                //                alertDiag.SetMessage("Are you sure you want to remove "+userModel.DisplayName+" from this group?");
                //                alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                //                    alertDiag.Dispose();
                //                });
                //                alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Remove</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                //                {
                //                    ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;
                //                    var result = await ClassService.Instance.RemoveMemberClassGroupAsync(userModel.Id, userModel.PartitionKey,classGroupModel);
                //                    if (result)
                //                    {

                //                        MembersCollection.Remove(userModel);
                //                    }
                //                    else { 

                //                    }
                //                    ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;
                //                });
                //                diag = alertDiag.Create();
                //                diag.Show();
                //                diag.SetCanceledOnTouchOutside(false);




                //            break;
                //    }
                //};
                //menu.DismissEvent += (s1, _event) => {

                //};

                //menu.Show();
            };
            Glide.With(ClassGroupActivity.Instance).Load(userModel.UserPhoto).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Into(ImgRequestUserphoto);
            TextUserRequestName.Text = userModel.DisplayName;
        }
        void onImageUsernameClick(object sender, EventArgs e)
        {
            var sendertype = sender.GetType().Name;
            int position = 0;
            if (sendertype == "AppCompatImageView")
            {
                position = Convert.ToInt32((sender as ImageView).ContentDescription);
            }
            else if (sendertype == "AppCompatTextView")
            {
                position = Convert.ToInt32((sender as TextView).ContentDescription);
            }
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", MembersCollection[position].Id);
            MainActivity.Instance.StartActivity(nextActivity);
        }
        public RecyclerView RecyclerMembersRequestsList => v.FindViewById<RecyclerView>(Resource.Id.RecyclerMembersRequestsList);
        public ProgressBar HorizontalProgress => v.FindViewById<ProgressBar>(Resource.Id.progressClassGroupMembers);
    }
}