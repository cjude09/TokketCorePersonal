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
    public class AvatarViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgAvatar { get; private set; }
        public AvatarViewHolder(View itemView, Action<int> listener)
                 : base(itemView)
        {
            // Locate and cache view references:
            ImgAvatar = itemView.FindViewById<ImageView>(Resource.Id.imgAvatarsRow);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}