using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Android.Graphics;
using Android.Text;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;
using Android.Content.Res;

namespace Tokket.Android.Adapters
{
    public class InviteUserAdapter : RecyclerView.Adapter
    {
        private static int TYPE_VIEW = 0; //If type_view = 1, Invites from other users
        public event EventHandler<int> ItemClick;
        public ObservableCollection<TokketUser> UsersCollection;
        ObservableCollection<ClassGroupRequestModel> UserRequestsCollection;
        ClassGroupModel model;
        public override int ItemCount => TYPE_VIEW != 2 ? UsersCollection.Count() : UserRequestsCollection.Count();
        View itemView;

        #region Constructor
        public InviteUserAdapter(ObservableCollection<TokketUser> _items, ObservableCollection<ClassGroupRequestModel> _UserRequestsCollection, ClassGroupModel _model, int typeView = 0)
        {
            UsersCollection = _items;
            model = _model;
            UserRequestsCollection = _UserRequestsCollection;
            TYPE_VIEW = typeView;
        }

        public InviteUserAdapter( ObservableCollection<ClassGroupRequestModel> _UserRequestsCollection, ClassGroupModel _model, int typeView = 0)
        {
           
            model = _model;
            UserRequestsCollection = _UserRequestsCollection;
            TYPE_VIEW = typeView;
            
        }
        #endregion

        #region Override Events/Methods/Delegates
        InviteUsersViewHolder vh;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as InviteUsersViewHolder;

            if (TYPE_VIEW == 0) //From InviteUsersActivity
            {
                vh.BtnInvite.Tag = position;
                vh.Username.Text = UsersCollection[position].DisplayName;
                Glide.With(itemView).Load(UsersCollection[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.ImgUserPhoto);

                //Check if user have pending invites sent
                var resultRequest = UserRequestsCollection.FirstOrDefault(c => c.ReceiverId == UsersCollection[position].Id);
                if (resultRequest != null) //If Edit
                {
                    vh.BtnInvite.Text = "Cancel";
                    vh.BtnInvite.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#dc3545"));
                }
            }
            else if (TYPE_VIEW == 1) //From InvitesActivity
            {
                vh.btnAccept.Tag = position;
                vh.UsernameInvites.Text = UsersCollection[position].DisplayName;
                Glide.With(itemView).Load(UsersCollection[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.ImgUserPhotoInvites);

                vh.btnAccept.Click += async(s, e) =>
                {
                    //classGroup = await ClassService.Instance.GetClassGroupAsync(groupid)
                    model = new ClassGroupModel();
                    model.Id = UserRequestsCollection[position].GroupId;
                    model.PartitionKey = UserRequestsCollection[position].GroupPartitionKey;

                    vh.circleProgress.Visibility = ViewStates.Visible;
                    var isSuccess = await ClassService.Instance.AcceptRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, model);
                    vh.circleProgress.Visibility = ViewStates.Gone;

                    if (isSuccess)
                    {
                        UserRequestsCollection.RemoveAt(position);
                        InvitesActivity.Instance.RefreshAdapter();
                        //NotifyItemChanged(position);
                    }
                };

                vh.btnCancel.Click += async (s, e) =>
                {
                    model = new ClassGroupModel();
                    model.Id = UserRequestsCollection[position].GroupId;
                    model.PartitionKey = UserRequestsCollection[position].GroupPartitionKey;

                    vh.circleProgress.Visibility = ViewStates.Visible;
                    var isSuccess = await ClassService.Instance.DeclineRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, model);
                    vh.circleProgress.Visibility = ViewStates.Gone;

                    if (isSuccess)
                    {
                        UserRequestsCollection.RemoveAt(position);
                        NotifyItemChanged(position);
                    }
                };
            }
            else if (TYPE_VIEW == 2) //From InvitesActivity
            {
                vh.btnAccept.Tag = position;
                var message = string.Empty;
                if (UserRequestsCollection[position].Message.ToLower().Contains("invited"))
                {
                    message = $"You have been invited to join {UserRequestsCollection[position].Name}";
                    vh.RowFrame.SetBackgroundColor(Color.Black);
                    vh.UsernameInvites.SetTextColor(Color.White);
                }
                else if (UserRequestsCollection[position].Message.ToLower().Contains("request"))
                {
                    message = $"{UserRequestsCollection[position].SenderDisplayName} has requested to join {UserRequestsCollection[position].Name}";
                    vh.RowFrame.SetBackgroundColor(Color.White);
                    vh.UsernameInvites.SetTextColor(Color.Black);
                }

                vh.UsernameInvites.Text = message;
                Glide.With(itemView).Load(UserRequestsCollection[position].SenderImage).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.ImgUserPhotoInvites);

                vh.btnAccept.Click += async (s, e) =>
                {
                    //classGroup = await ClassService.Instance.GetClassGroupAsync(groupid)
                    model = new ClassGroupModel();
                    model.Id = UserRequestsCollection[position].GroupId;
                    model.PartitionKey = UserRequestsCollection[position].GroupPartitionKey;

                    vh.circleProgress.Visibility = ViewStates.Visible;
                    var isSuccess = await ClassService.Instance.AcceptRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, model);
                    vh.circleProgress.Visibility = ViewStates.Gone;

                    if (isSuccess)
                    {
                        UserRequestsCollection.RemoveAt(position);
                        InvitesActivity.Instance.RefreshAdapter();
                    }
                };

                vh.btnCancel.Click += async (s, e) =>
                {
                    model = new ClassGroupModel();
                    model.Id = UserRequestsCollection[position].GroupId;
                    model.PartitionKey = UserRequestsCollection[position].GroupPartitionKey;

                    vh.circleProgress.Visibility = ViewStates.Visible;
                    var isSuccess = await ClassService.Instance.DeclineRequest(UserRequestsCollection[position].Id, UserRequestsCollection[position].PartitionKey, model);
                    vh.circleProgress.Visibility = ViewStates.Gone;

                    if (isSuccess)
                    {
                        UserRequestsCollection.RemoveAt(position);
                        InvitesActivity.Instance.RefreshAdapter();
                        // NotifyItemChanged(position);
                    }
                };
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (TYPE_VIEW == 0)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.inviteusers_row, parent, false);
            }
            else if (TYPE_VIEW == 1 || TYPE_VIEW == 2)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.invites_row, parent, false);
            }

            vh = new InviteUsersViewHolder(itemView, OnClick);
            return vh;
        }
        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }
        #endregion
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}