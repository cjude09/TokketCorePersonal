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
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;

namespace Tokket.Android.ViewHolders
{
    public class TokViewHolder : RecyclerView.ViewHolder
    {
        //public RelativeLayout RelativeImgAndBottom { get; private set; }
        public ImageView imgTokGroup { get; private set; }
        public ImageView imgTokGroupImage { get; private set; }
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
        public ConstraintLayout gridBackground { get; private set; }
        public ConstraintLayout gridTokImage { get; private set; }
        public ConstraintLayout Tokdrawable { get; private set; }
        public ConstraintLayout tokimgdrawable { get; private set; }
        public TextView txtLinkImage { get; private set; }
        public TextView txtLink { get; private set; }
        public TextView txtToastLinkCopied { get; private set; }
        public TextView txtToastLinkCopiedImg { get; private set; }
        public ImageView imageViewHandleImage { get; private set; }
        public ImageView imageViewHandle { get; private set; }
        public TextView SharedMark { get; private set; }

        public TextView SharedMarkTxt { get; private set; }
        public TextView lblTokImgReferenceId { get; private set; }

        public TextView lblTokReferenceId { get; private set; }
        public TextView txtDateCreated { get; private set; }
        public TextView txtDateCreatedImg { get; private set; }

        public LinearLayout DetailLinearContainer { get; private set; }
        public LinearLayout linearDetailImage { get; private set; }

        public ImageView BtnTokInfoEyeIcon { get; private set; }

        public TextView LblTokInfoViewCpunt { get; private set; }
        public LinearLayout linearRowUsernameImg { get; private set; }
        public LinearLayout linearRow1 { get; private set; }

        //Tok Group Pic
        public ConstraintLayout TokdrawableGroupPic { get; private set; }
        public ConstraintLayout gridTokGroupPic { get; private set; }
        public AppCompatImageView imageTokGroupPic { get; private set; }

        // Get references to the views defined in the CardView layout.
        public TokViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            imgTokGroup = itemView.FindViewById<ImageView>(Resource.Id.imgTokGroup);
            imgTokGroupImage = itemView.FindViewById<ImageView>(Resource.Id.imgTokGroupImage);

            gridBackground = itemView.FindViewById<ConstraintLayout>(Resource.Id.gridBackground);
            gridTokImage = ItemView.FindViewById<ConstraintLayout>(Resource.Id.gridTokImage);
            Tokdrawable = itemView.FindViewById<ConstraintLayout>(Resource.Id.gridBackground);
            tokimgdrawable = itemView.FindViewById<ConstraintLayout>(Resource.Id.gridTokImage);
            UserPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageUserPhoto);
            UserFlag = itemView.FindViewById<ImageView>(Resource.Id.imageFlag);
            UserDisplayName = itemView.FindViewById<TextView>(Resource.Id.lbl_nameuser);
            PrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_row);
            SecondaryFieldText = itemView.FindViewById<TextView>(Resource.Id.secondarytext_row);
            EnglishPrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.lbl_englishPrimaryFieldText);
            Category = itemView.FindViewById<TextView>(Resource.Id.lblCategory);
            lblTokReferenceId = itemView.FindViewById<TextView>(Resource.Id.lblTokReferenceId);

            linearRowUsernameImg = itemView.FindViewById<LinearLayout>(Resource.Id.linearRowUsernameImg);
            linearRow1 = itemView.FindViewById<LinearLayout>(Resource.Id.linearRow1);

            imageViewHandleImage = itemView.FindViewById<ImageView>(Resource.Id.imageViewHandleImage);
            imageViewHandle = itemView.FindViewById<ImageView>(Resource.Id.imageViewHandle);

            //TOK GROUP PIC
            TokdrawableGroupPic = itemView.FindViewById<ConstraintLayout>(Resource.Id.gridTokGroupPic);
            gridTokGroupPic = itemView.FindViewById<ConstraintLayout>(Resource.Id.gridTokGroupPic);
            imageTokGroupPic = itemView.FindViewById<AppCompatImageView>(Resource.Id.imageTokGroupPic);

#if (_TOKKEPEDIA)
            TokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokGroup);
            TokImgTokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
#endif
#if (_CLASSTOKS)
            txtLinkImage = itemView.FindViewById<TextView>(Resource.Id.txtLinkImage);
            txtLink = itemView.FindViewById<TextView>(Resource.Id.txtLink);
            txtToastLinkCopied = itemView.FindViewById<TextView>(Resource.Id.txtToastLinkCopied);
            txtToastLinkCopiedImg = itemView.FindViewById<TextView>(Resource.Id.txtToastLinkCopiedImg);
#endif
            TokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokGroup);
            TokImgTokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
            TokType = itemView.FindViewById<TextView>(Resource.Id.lblTokType);
            ImgPurpleGem = itemView.FindViewById<ImageView>(Resource.Id.toktile_imgpurplegem);
            TileSticker = itemView.FindViewById<ImageView>(Resource.Id.imgtile_stickerimage);
            TokUserTitle = ItemView.FindViewById<TextView>(Resource.Id.lbl_royaltitle);
            lblTokViewMore = ItemView.FindViewById<TextView>(Resource.Id.lblTokViewMore);
            txtDateCreated = itemView.FindViewById<TextView>(Resource.Id.txtDateCreated);

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
            SharedMark = itemView.FindViewById<TextView>(Resource.Id.shared_mark);
            SharedMarkTxt = itemView.FindViewById<TextView>(Resource.Id.txtshared_mark);
            lblTokImgReferenceId = itemView.FindViewById<TextView>(Resource.Id.lblTokImgReferenceId);
            txtDateCreatedImg = itemView.FindViewById<TextView>(Resource.Id.txtDateCreatedImg);
            //RelativeImgAndBottom = ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeImgAndBottom);
            DetailLinearContainer = itemView.FindViewById<LinearLayout>(Resource.Id.detialLinear);
            linearDetailImage = itemView.FindViewById<LinearLayout>(Resource.Id.linearDetailImage);

            BtnTokInfoEyeIcon = itemView.FindViewById<ImageView>(Resource.Id.btnTokInfoEyeIcon);
            LblTokInfoViewCpunt = itemView.FindViewById<TextView>(Resource.Id.lblTokInfoViews);
            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}