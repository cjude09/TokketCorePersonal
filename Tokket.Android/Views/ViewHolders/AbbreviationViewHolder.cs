using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;

namespace Tokket.Android.ViewHolders
{
    public class AbbreviationViewHolder : RecyclerView.ViewHolder
    {
        //public RelativeLayout RelativeImgAndBottom { get; private set; }
        public TextView TokUserTitle { get; private set; }
        public TextView lblTokViewMore { get; private set; }
        public ImageView TileSticker { get; private set; }
        public ImageView ImgPurpleGemTokImg { get; private set; }
        public ImageView ImgPurpleGem { get; private set; }
        public ImageView UserPhoto { get; private set; }
        public ImageView UserFlag { get; private set; }
        public ImageView TokImgUserPhoto { get; private set; }
        public ImageView TokImgUserFlag { get; private set; }
        public ImageView TokImgMain { get; private set; }
        public ImageView TileStickerImg { get; private set; }
        public TextView TokUserTitleImg { get; private set; }
        public TextView ImgUserDisplayName { get; private set; }
        public TextView UserDisplayName { get; private set; }
        public TextView PrimaryFieldText { get; private set; }
        public TextView SecondaryFieldText { get; private set; }
        public TextView EnglishPrimaryFieldText { get; private set; }
        public TextView Category { get; private set; }
        public TextView TokGroup { get; private set; }
        public TextView TokType { get; private set; }
        public TextView TokImgPrimaryFieldText { get; private set; }
        public TextView TokImgSecondaryFieldText { get; private set; }
        public TextView TokImgCategory { get; private set; }
        public TextView TokImgTokGroup { get; private set; }
        public TextView TokImgTokType { get; private set; }
        public GridLayout gridBackground { get; private set; }
        public GridLayout gridTokImage { get; private set; }
        public GridLayout Tokdrawable { get; private set; }
        public GridLayout tokimgdrawable { get; private set; }
        public TextView txtLinkImage { get; private set; }
        public TextView txtLink { get; private set; }
        public LinearLayout linearToast { get; private set; }
        public TextView txtToast { get; private set; }

        public RelativeLayout CommentSection { get; private set; }
        public ImageView TokImageFirstUserCommentPhoto { get; private set; }

        public TextView Comment { get; private set; }

        public TokModel Item { get; set; }

        public View LineView { get; private set; }

        public TextView CommentUser { get; private set; }

        public TextView CommentDate { get; private set; }

        private View ViewItem;
        // Get references to the views defined in the CardView layout.
        public AbbreviationViewHolder(View itemView,  Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:

            ViewItem = itemView;
            gridBackground = itemView.FindViewById<GridLayout>(Resource.Id.gridBackground);
            gridTokImage = ItemView.FindViewById<GridLayout>(Resource.Id.gridTokImage);
            Tokdrawable = itemView.FindViewById<GridLayout>(Resource.Id.gridBackground);
            tokimgdrawable = itemView.FindViewById<GridLayout>(Resource.Id.gridTokImage);
            UserPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageUserPhoto);
            UserFlag = itemView.FindViewById<ImageView>(Resource.Id.imageFlag);
            UserDisplayName = itemView.FindViewById<TextView>(Resource.Id.lbl_nameuser);
            PrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_row);
            SecondaryFieldText = itemView.FindViewById<TextView>(Resource.Id.secondarytext_row);
            EnglishPrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_englishPrimaryFieldText);
            Category = itemView.FindViewById<TextView>(Resource.Id.lblCategory);

#if (_TOKKEPEDIA)
            TokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokGroup);
            TokImgTokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
#endif
#if (_CLASSTOKS)
            txtLinkImage = itemView.FindViewById<TextView>(Resource.Id.txtLinkImage);
            txtLink = itemView.FindViewById<TextView>(Resource.Id.txtLink);
            linearToast = itemView.FindViewById<LinearLayout>(Resource.Id.linearToast);
            txtToast = itemView.FindViewById<TextView>(Resource.Id.txtToast);
#endif

            TokType = itemView.FindViewById<TextView>(Resource.Id.lblTokType);
            ImgPurpleGem = itemView.FindViewById<ImageView>(Resource.Id.toktile_imgpurplegem);
            TileSticker = itemView.FindViewById<ImageView>(Resource.Id.imgtile_stickerimage);
            TokUserTitle = ItemView.FindViewById<TextView>(Resource.Id.lbl_royaltitle);
            lblTokViewMore = ItemView.FindViewById<TextView>(Resource.Id.lblTokViewMore);

            //Tok Image
            TokImgUserPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageTokImgUserPhoto);
            TokImgUserFlag = itemView.FindViewById<ImageView>(Resource.Id.img_tokimgFlag);
            TokImgMain = itemView.FindViewById<ImageView>(Resource.Id.imgTokImgMain);
            ImgUserDisplayName = itemView.FindViewById<TextView>(Resource.Id.lbl_Imgnameuser);
            TokImgPrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_tokimgprimarytext);
            TokImgSecondaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_tokimgsecondarytext);
            TokImgCategory = itemView.FindViewById<TextView>(Resource.Id.lblTokImgCategory);
            TokImgTokType = itemView.FindViewById<TextView>(Resource.Id.lblTokImgType);
            TileStickerImg = itemView.FindViewById<ImageView>(Resource.Id.imgtile_stickerimageImg);
            TokUserTitleImg = ItemView.FindViewById<TextView>(Resource.Id.lbl_royaltitleImg);
            ImgPurpleGemTokImg = ItemView.FindViewById<ImageView>(Resource.Id.toktile_imgpurplegemtokimg);

            //RelativeImgAndBottom = ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeImgAndBottom);
            //Comments
            TokImageFirstUserCommentPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageTokImgFirstCommentUserPhoto);
            CommentSection = itemView.FindViewById<RelativeLayout>(Resource.Id.CommentSection);
            Comment = itemView.FindViewById<TextView>(Resource.Id.lblfirstcomment);
            CommentUser =itemView.FindViewById<TextView>(Resource.Id.lbl_commentnameuser);
            CommentDate = itemView.FindViewById<TextView>(Resource.Id.lbl_commentdate);
            LineView = itemView.FindViewById<View>(Resource.Id.viewline);
            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
        
        public async void LoadComment(TokModel processedItem) {
            ReactionQueryValues reactionQueryValues = new ReactionQueryValues();
            reactionQueryValues.item_id = processedItem.Id;
            reactionQueryValues.kind = "comments";
            reactionQueryValues.user_likes = true;
            reactionQueryValues.userid = processedItem.UserId;
            reactionQueryValues.detail_number = -1;
            
            //reactionQueryValues.pagination_id = string.IsNullOrEmpty(ContinuationToken) ? null : ContinuationToken;

           
            ReactionService newInst = new ReactionService();
            if (newInst._HttpClientHelper == null)
                newInst._HttpClientHelper = new HttpClientHelper(Tokket.Shared.Config.Configurations.BaseUrl);

            var items = await newInst.GetReactionsAsync(reactionQueryValues);
            var itemsXF = JsonConvert.DeserializeObject<Tokket.Core.Tools.ResultData<ReactionModel>>(JsonConvert.SerializeObject(items));
            var result = itemsXF.Results?.ToList();
            if (result.Count() > 0)
            {
                Glide.With(ViewItem).Load(result[0].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(TokImageFirstUserCommentPhoto);
                Comment.Text = result[0].Text;
                CommentSection.Visibility = ViewStates.Visible;
                CommentUser.Text = result[0].UserDisplayName;
                LineView.Visibility = ViewStates.Visible;

            }
            else {
                CommentSection.Visibility = ViewStates.Gone;
                LineView.Visibility = ViewStates.Gone;
            }
            //ContinuationToken = items?.ContinuationToken;
            //RemainingItemsThreshold = (!string.IsNullOrEmpty(ContinuationToken)) ? 0 : -1;
        }
    }
}