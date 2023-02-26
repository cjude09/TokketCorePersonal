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
    public class TokChannelsLevelViewHolder : RecyclerView.ViewHolder
    {
        public GridLayout GridLevel { get; private set; }
        public GridLayout GridLevelDrawable { get; private set; }
        public TextView TextLevelName { get; private set; }
        public TokChannelsLevelViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            TextLevelName = itemView.FindViewById<TextView>(Resource.Id.txtText);
            GridLevel = itemView.FindViewById<GridLayout>(Resource.Id.gridLevel);
            GridLevelDrawable = itemView.FindViewById<GridLayout>(Resource.Id.gridLevel);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}