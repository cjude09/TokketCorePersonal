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
using Google.Flexbox;

namespace Tokket.Android.ViewHolders
{
    public class SearchTokViewHolder : RecyclerView.ViewHolder
    {
        public ImageView imageTok { get; private set; }
        public TextView txtPrimaryFieldText { get; private set; }
        public TextView txtUserDisplayName { get; private set; }
        public ImageView imageTokImgUserPhoto { get; private set; }

        public LinearLayout flexBox { get; private set; }

        public FlexboxLayout BG { get; private set; }
        // Get references to the views defined in the CardView layout.
        public SearchTokViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            imageTok = itemView.FindViewById<ImageView>(Resource.Id.imageTok);
            imageTokImgUserPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageTokImgUserPhoto);
            txtPrimaryFieldText = itemView.FindViewById<TextView>(Resource.Id.txtPrimaryFieldText);
            txtUserDisplayName = itemView.FindViewById<TextView>(Resource.Id.txtUserDisplayName);
            flexBox = itemView.FindViewById<LinearLayout>(Resource.Id.flexBackground);
            BG = itemView.FindViewById<FlexboxLayout>(Resource.Id.bodyBG);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}