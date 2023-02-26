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
    public class ReplyViewHolder : RecyclerView.ViewHolder
    {
        public EditText EditCommentText { get; private set; }
        public Button BtnCancelComment { get; private set; }
        public Button BtnUpdateComment { get; private set; }
        public TextView PopUpMenuComments { get; private set; }
        public TextView txtCommentHeartCount { get; private set; }
        public Button btnCommentHeart { get; private set; }
        public ImageView ImgUserReply { get; private set; }
        public TextView NameUserReply { get; private set; }
        public TextView DateReplied { get; private set; }
        public TextView replyContent { get; private set; }
        public TextView replyContentEllip { get; private set; }
        public ReplyViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            EditCommentText = itemView.FindViewById<EditText>(Resource.Id.EditCommentRowContent);
            BtnCancelComment = itemView.FindViewById<Button>(Resource.Id.BtnCancelComment);
            BtnUpdateComment = itemView.FindViewById<Button>(Resource.Id.BtnUpdateComment);
            PopUpMenuComments = itemView.FindViewById<TextView>(Resource.Id.lblCommentPopUpMenu);

            txtCommentHeartCount = itemView.FindViewById<TextView>(Resource.Id.txtCommentHeartCount);
            btnCommentHeart = itemView.FindViewById<Button>(Resource.Id.btnCommentHeart);

            ImgUserReply = itemView.FindViewById<ImageView >(Resource.Id.imgreply_userphoto);
            NameUserReply = itemView.FindViewById<TextView>(Resource.Id.lbl_replynameuser);
            DateReplied = itemView.FindViewById<TextView>(Resource.Id.lbl_replydate);
            replyContent = itemView.FindViewById<TextView>(Resource.Id.lblReplyRowContent);
            replyContentEllip = itemView.FindViewById<TextView>(Resource.Id.lblReplyRowContentEllip);

            itemView.FindViewById<View>(Resource.Id.ViewrepliesTopLine).Visibility = ViewStates.Gone;
            itemView.FindViewById<View>(Resource.Id.ViewrepliesBottomLine).Visibility = ViewStates.Visible;

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}