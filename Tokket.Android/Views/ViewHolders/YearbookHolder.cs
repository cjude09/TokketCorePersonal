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
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;

namespace Tokket.Android.ViewHolders
{
   
    public class YearbookHolder : RecyclerView.ViewHolder
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
        public AppCompatCheckBox chkBox { get; private set; }

        public SquareImageView ImageYearbook { get; private set; }

        // Get references to the views defined in the CardView layout.
        public YearbookHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
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
            chkBox = itemView.FindViewById<AppCompatCheckBox>(Resource.Id.chkBox);
#if (_TOKKEPEDIA)
            TokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokGroup);
            TokImgTokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
#endif
#if (_CLASSTOKS)
            //txtLinkImage = itemView.FindViewById<TextView>(Resource.Id.txtLinkImage);
            //txtLink = itemView.FindViewById<TextView>(Resource.Id.txtLink);
            linearToast = itemView.FindViewById<LinearLayout>(Resource.Id.linearToast);
            txtToast = itemView.FindViewById<TextView>(Resource.Id.txtToast);
#endif
            TokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokGroup);
            TokImgTokGroup = itemView.FindViewById<TextView>(Resource.Id.lblTokImgGroup);
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
            ImageYearbook = itemView.FindViewById<SquareImageView>(Resource.Id.imgPic);
            //RelativeImgAndBottom = ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeImgAndBottom);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
            ImageYearbook.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}