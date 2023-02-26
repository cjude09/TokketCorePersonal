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
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.ViewHolders
{
    public class MySetsViewHolderConvertible : RecyclerView.ViewHolder
    {

        public TextView txtSetsTokUpper { get; private set; }
        public TextView txtSetsTokBottom { get; private set; }
        public TextView lblMySetPopUp { get; private set; }
        public TextView txtClassDescription { get; private set; }
        public LinearLayout linearMySetsColor { get; private set; }
        public ImageView ImgMySetsRow { get; private set; }
        public MySetsViewHolderConvertible(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:                                     
            txtSetsTokUpper = itemView.FindViewById<TextView>(Resource.Id.txtSetsTokUpperConv);
            txtClassDescription = itemView.FindViewById<TextView>(Resource.Id.txtClassDescriptionConv);
            txtSetsTokBottom = itemView.FindViewById<TextView>(Resource.Id.txtSetsTokBottomConv);
            lblMySetPopUp = itemView.FindViewById<TextView>(Resource.Id.lblMySetPopUpConv);
            linearMySetsColor = itemView.FindViewById<LinearLayout>(Resource.Id.linearMySetsColorConv);
            ImgMySetsRow = itemView.FindViewById<ImageView>(Resource.Id.ImgMySetsRowConv);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
            lblMySetPopUp.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}