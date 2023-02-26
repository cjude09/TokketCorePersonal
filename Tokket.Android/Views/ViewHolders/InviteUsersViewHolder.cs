using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.ViewHolders
{
    public class InviteUsersViewHolder : RecyclerView.ViewHolder
    {
        //InviteUsersActivity
        public ImageView ImgUserPhoto { get; private set; }
        public TextView Username { get; private set; }
        public Button BtnInvite { get; private set; }
        public ProgressBar ProgressCircle { get; private set; }

        //InvitesActivity
        public ImageView ImgUserPhotoInvites { get; private set; }
        public TextView UsernameInvites { get; private set; }
        public Button btnAccept { get; private set; }
        public Button btnCancel { get; private set; }
        public ProgressBar circleProgress { get; private set; }

        public FrameLayout RowFrame { get; private set; }
        public InviteUsersViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            //InviteUsersActivity
            ImgUserPhoto = itemView.FindViewById<ImageView>(Resource.Id.ImgInviteUserphoto);
            Username = itemView.FindViewById<TextView>(Resource.Id.TextInviteUserName);
            BtnInvite = itemView.FindViewById<Button>(Resource.Id.BtnSubmitInvite);
            ProgressCircle = itemView.FindViewById<ProgressBar>(Resource.Id.ProgressInviteUsers);

            //InvitesActivity
            ImgUserPhotoInvites = itemView.FindViewById<ImageView>(Resource.Id.imgUserPhoto);
            UsernameInvites = itemView.FindViewById<TextView>(Resource.Id.txtUserDisplayName);
            btnAccept = itemView.FindViewById<Button>(Resource.Id.btnAccept);
            btnCancel = itemView.FindViewById<Button>(Resource.Id.btnCancel);
            circleProgress = itemView.FindViewById<ProgressBar>(Resource.Id.circleProgress);
            RowFrame = itemView.FindViewById<FrameLayout>(Resource.Id.rowFrame);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}