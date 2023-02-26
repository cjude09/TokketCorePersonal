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
    public class PatchesViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextRankLabel { get; private set; }
        public TextView TextRankTitle { get; private set; }
        public TextView TextPoints { get; private set; }
        public ImageView ImgPatches { get; private set; }
        public PatchesViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            TextRankLabel = itemView.FindViewById<TextView>(Resource.Id.TextRankLabel);
            TextRankTitle = itemView.FindViewById<TextView>(Resource.Id.TextRankTitle);
            ImgPatches = itemView.FindViewById<ImageView>(Resource.Id.imgPatchesRow);
            TextPoints = ItemView.FindViewById<TextView>(Resource.Id.TextPoints);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}