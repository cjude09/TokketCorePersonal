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
    public class CardViewHolder : RecyclerView.ViewHolder
    {
        public TextView tokcardfront { get; private set; }
        public TextView tokcardback { get; private set; }
        public ViewFlipper tokcardviewflipper {get;private set;}
        public CardViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            tokcardfront = itemView.FindViewById<TextView>(Resource.Id.lblTokSetBackCardFront);
            tokcardback = itemView.FindViewById<TextView>(Resource.Id.lblTokSetBackCardBack);
            tokcardviewflipper = ItemView.FindViewById<ViewFlipper>(Resource.Id.viewFlipper_settokcard);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}