using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using DE.Hdodenhof.CircleImageViewLib;

namespace Tokket.Android.ViewHolders
{
    public class MySetsViewHolder : RecyclerView.ViewHolder
    {
        
        public TextView txtSetsTokUpper { get; private set; }
        public TextView txtSetsTokBottom { get; private set; }
        public TextView lblMySetPopUp { get; private set; }
        public TextView txtClassDescription { get; private set; }
        public LinearLayout linearMySetsColor { get; private set; }
        public ImageView ImgMySetsRow { get; private set; }
        public AppCompatCheckBox chkBox { get; private set; }
        public LinearLayout linearUserInfo { get; private set; }
        public CircleImageView SetUserImage { get; private set; }

        public TextView SetUserName { get; private set; }
        public TextView txtSetsReferenceId { get; private set; }
        public MySetsViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            txtSetsReferenceId = itemView.FindViewById<TextView>(Resource.Id.txtSetsReferenceId);
            txtSetsTokUpper = itemView.FindViewById<TextView>(Resource.Id.txtSetsTokUpper);
            txtClassDescription = itemView.FindViewById<TextView>(Resource.Id.txtClassDescription);
            txtSetsTokBottom = itemView.FindViewById<TextView>(Resource.Id.txtSetsTokBottom);
            lblMySetPopUp = itemView.FindViewById<TextView>(Resource.Id.lblMySetPopUp);
            linearMySetsColor = itemView.FindViewById<LinearLayout>(Resource.Id.linearMySetsColor);
            ImgMySetsRow = itemView.FindViewById<ImageView>(Resource.Id.ImgMySetsRow);
            SetUserImage = itemView.FindViewById<CircleImageView>(Resource.Id.imgcomment_userphoto);
            SetUserName = itemView.FindViewById<TextView>(Resource.Id.txtUserDisplayName);
            chkBox = itemView.FindViewById<AppCompatCheckBox>(Resource.Id.chkBox);
            linearUserInfo = itemView.FindViewById<LinearLayout>(Resource.Id.linearUserInfo);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}