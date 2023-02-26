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
    public class ProfileTitleViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextTitle { get; private set; }
        public ProfileTitleViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            TextTitle = itemView.FindViewById<TextView>(Resource.Id.LabelTitle);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}