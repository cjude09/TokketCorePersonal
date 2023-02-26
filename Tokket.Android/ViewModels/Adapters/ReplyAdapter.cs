using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.Adapters
{
    public class ReplyAdapter : RecyclerView.Adapter
    {

        Context context;
        List<ReactionModel> ReplyList;
        View itemView;
        ReplyViewHolder vh;
        public override int ItemCount => ReplyList.Count;

        public event EventHandler<int> ItemClick;
   
        public ReplyAdapter(Context context, List<ReactionModel> _ReplyList)
        {
            this.context = context;
            ReplyList = _ReplyList;
        
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as ReplyViewHolder;
            var comment = ReplyList[position];

            var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
            if (comment.UserId == Settings.GetUserModel().UserId && !string.IsNullOrEmpty(cacheUserPhoto))
            {
                vh.ImgUserReply.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
            }
            else
            {
                Glide.With(context).Load(ReplyList[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.ImgUserReply);
            }
            
            vh.NameUserReply.Text = ReplyList[position].UserDisplayName;
            vh.DateReplied.Text = DateConvert.ConvertToRelative(ReplyList[position].CreatedTime).ToString();
            vh.replyContent.Text = ReplyList[position].Text;
            vh.replyContentEllip.Text = ReplyList[position].Text;
            vh.EditCommentText.Text = ReplyList[position].Text;
            vh.txtCommentHeartCount.Text = comment.Likes == null ? "0" : comment.Likes.ToString();
           

            vh.txtCommentHeartCount.Text = ReplyList[position].Likes == null ? "0" : ReplyList[position].Likes.ToString();

            if (ReplyList[position].UserLiked)
            {
                vh.btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.colorAccent)));
            }
            else
            {
                vh.btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.placeholder_bg)));
            }
            int heartCnt = int.Parse(vh.txtCommentHeartCount.Text);
            var font = Typeface.CreateFromAsset(Application.Context.Assets, "fa_solid_900.otf");
            vh.btnCommentHeart.Typeface = font;

            vh.btnCommentHeart.Click += async (obj,eve)=> {

                if (!comment.UserLiked && vh.btnCommentHeart.CurrentTextColor != new Color(ContextCompat.GetColor(this.context, Resource.Color.colorAccent)))
                {
                    
                    var tokketUser = Settings.GetTokketUser();
                    var reactionUser = comment;
                    reactionUser.ParentItem = comment.Id;
                    reactionUser.ParentUser = comment.UserId;
                    reactionUser.Kind = "like";
                    reactionUser.Label = "reaction";
                    reactionUser.DetailNum = comment.DetailNum;
                    reactionUser.CategoryId = comment.CategoryId;
                    reactionUser.TokTypeId = comment.TokTypeId;
                    reactionUser.OwnerId = tokketUser.Id;
                    reactionUser.ItemId = comment.ItemId;
                    reactionUser.IsChild = true;
                    reactionUser.UserDisplayName = comment.UserDisplayName;
                    reactionUser.UserPhoto = comment.UserPhoto;
                    reactionUser.Timestamp = DateTime.Now;
                    reactionUser.IsComment = true;
                    reactionUser.UserLiked = true;
                    reactionUser.UserId = Settings.GetUserModel().UserId;

                    var result = await ReactionService.Instance.AddReaction(reactionUser);
                    vh.txtCommentHeartCount.Text = (heartCnt + 1).ToString();
                    vh.btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this.context, Resource.Color.colorAccent)));
                }
                else
                {
                    var userLikedId = $"like-{comment.Id}-{Settings.GetUserModel().UserId}";
                    var result = await ReactionService.Instance.DeleteReaction(userLikedId);

                    if (result)
                    {
                        vh.txtCommentHeartCount.Text = (heartCnt - 1).ToString();
                        vh.btnCommentHeart.SetTextColor(new Color(ContextCompat.GetColor(this.context, Resource.Color.placeholder_bg)));
                    }
                    else
                    {
                      
                    }
                }
              
            };

            vh.ImgUserReply.Click += (obj, _evt) => {
                string commentorid = ReplyList[position].UserId;
                var nextActivity = new Intent(context, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", commentorid);
                context.StartActivity(nextActivity);
            };

            vh.NameUserReply.Click += (obj, _evt) => {
                string commentorid = ReplyList[position].UserId;
                var nextActivity = new Intent(context, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", commentorid);
                context.StartActivity(nextActivity);
            };
        }
     
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.tokinfo_comments_replies_row, parent, false);

            vh = new ReplyViewHolder(itemView, OnClick);
            return vh;
        }
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}