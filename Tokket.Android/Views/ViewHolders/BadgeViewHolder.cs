using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;

namespace Tokket.Android.ViewHolders
{
    public class BadgeViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgBadge { get; private set; }
        public BadgeViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            ImgBadge = itemView.FindViewById<ImageView>(Resource.Id.imgBadgesRow);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}