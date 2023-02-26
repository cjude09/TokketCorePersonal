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
    public class ClassFilterByViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtClassFilter { get; private set; }
        public ClassFilterByViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            txtClassFilter = itemView.FindViewById<TextView>(Resource.Id.lblSettingsRow);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}