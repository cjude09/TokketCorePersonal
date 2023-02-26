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
    public class GameSetViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgGame { get; private set; }
        public TextView TextTitle { get; private set; }
        public Button BtnAddGameSet { get; private set; }
        public Button BtnViewGameSet { get; private set; }
        public Button BtnViewMyGameSet { get; private set; }
        public LinearLayout LinearGame { get; private set; }
        
        // Get references to the views defined in the CardView layout.
        public GameSetViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            ImgGame = ItemView.FindViewById<ImageView>(Resource.Id.imgViewLogo);
            TextTitle = ItemView.FindViewById<TextView>(Resource.Id.txtTitle);
            BtnAddGameSet = ItemView.FindViewById<Button>(Resource.Id.btnAddGameSet);
            BtnViewGameSet = ItemView.FindViewById<Button>(Resource.Id.btnViewGameSet);
            BtnViewGameSet = ItemView.FindViewById<Button>(Resource.Id.btnViewGameSet);
            BtnViewMyGameSet = ItemView.FindViewById<Button>(Resource.Id.btnViewMyGameSet);
            LinearGame = ItemView.FindViewById<LinearLayout>(Resource.Id.LinearGame);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}