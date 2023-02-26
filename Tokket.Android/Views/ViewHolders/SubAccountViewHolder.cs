using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.ViewHolders
{
    public class SubAccountViewHolder : RecyclerView.ViewHolder
    {
        public GridLayout GridAccountImageDrawable { get; private set; }
        public GridLayout GridAccountImage { get; private set; }
        public ImageView ImgSubAccount { get; private set; }
        public TextView TextImgSubAccntName { get; private set; }
        public GridLayout GridAccount { get; private set; }
        public GridLayout GridAccountDrawable { get; private set; }
        public TextView TextSubAccntHeader { get; private set; }
        public TextView TextSubAccntName { get; private set; }


        public SubAccountViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            GridAccountImage = itemView.FindViewById<GridLayout>(Resource.Id.GridAccountImage);
            GridAccountImageDrawable = itemView.FindViewById<GridLayout>(Resource.Id.GridAccountImage);
            ImgSubAccount = itemView.FindViewById<ImageView>(Resource.Id.ImgSubAccount);
            TextImgSubAccntName = itemView.FindViewById<TextView>(Resource.Id.TextImgSubAccntName);
            GridAccount = itemView.FindViewById<GridLayout>(Resource.Id.GridAccount);
            GridAccountDrawable = itemView.FindViewById<GridLayout>(Resource.Id.GridAccount);
            TextSubAccntHeader = itemView.FindViewById<TextView>(Resource.Id.TextSubAccntHeader);
            TextSubAccntName = itemView.FindViewById<TextView>(Resource.Id.TextSubAccntName);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}