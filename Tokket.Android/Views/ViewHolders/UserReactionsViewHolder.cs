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
    public class UserReactionsViewHolder : RecyclerView.ViewHolder
    {
        public ImageView UserPhoto { get; private set; }
        public TextView Username { get; private set; }
        public ImageView UserReactions { get; private set; }

        public UserReactionsViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            //Tok Image
            UserPhoto = itemView.FindViewById<ImageView>(Resource.Id.imageRVUserPhoto);
            Username = itemView.FindViewById<TextView>(Resource.Id.lbl_reactionvalues_username);
            UserReactions = itemView.FindViewById<ImageView>(Resource.Id.imageRVReactions);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}