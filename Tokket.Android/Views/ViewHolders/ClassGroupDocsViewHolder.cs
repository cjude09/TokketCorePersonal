using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Android.ViewHolders
{
    public class ClassGroupDocsViewHolder : RecyclerView.ViewHolder
    {
        public TextView textDocTitle { get; private set; }
        public TextView lblDocMenu { get; private set; }
        public LinearLayout linearDocsColor { get; private set; }
        public ClassGroupDocsViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:                                     
            textDocTitle = itemView.FindViewById<TextView>(Resource.Id.txtDocTitle);
            lblDocMenu = itemView.FindViewById<TextView>(Resource.Id.lblDocMenu);
            linearDocsColor = itemView.FindViewById<LinearLayout>(Resource.Id.linearDocsColor);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}