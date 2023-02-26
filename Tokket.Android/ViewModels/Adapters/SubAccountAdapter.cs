using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Extensions;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Core;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Adapters
{
    public class SubAccountAdapter : RecyclerView.Adapter
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        List<TokketSubaccount> items;
        //List<string> Colors = new List<string>() {
        //       "#4472C7", "#732FA0", "#05adf4", "#73AD46",
        //       "#E23DB5", "#BE0400", "#195B28", "#E88030",
        //       "#873B09", "#FFC100"
        //       };
        List<string> Colors;
        public override int ItemCount => items.Count;
        View itemView;
        #endregion

        #region Constructor
        public SubAccountAdapter(List<TokketSubaccount> _items, List<string> _colors)
        {
            items = _items;
            Colors = _colors;
        }
        #endregion

        #region Override Events/Methods/Delegates
        SubAccountViewHolder vh; int selectedGrid = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as SubAccountViewHolder;

            string currentUser = Settings.GetTokketSubaccount()?.SubaccountName;//this is the subaccount that the user is currently selecting

            if (string.IsNullOrEmpty(items[position].SubaccountPhoto))
            {
                vh.TextSubAccntName.Text = items[position].SubaccountName;
                vh.TextSubAccntHeader.Text = items[position].SubaccountName.Substring(0, 2).ToUpper();

                vh.TextSubAccntHeader.SetBackgroundColor(Color.ParseColor(Colors[position]));

                vh.GridAccount.Visibility = ViewStates.Visible;
                vh.GridAccountImage.Visibility = ViewStates.Gone;
                vh.GridAccount.SetBackgroundResource(Resource.Drawable.tileview_layout);

                GradientDrawable griddrawable = (GradientDrawable)vh.GridAccountDrawable.Background;
                //hightlight current selected user
                if (currentUser == items[position].SubaccountName)
                {
                    griddrawable.SetColor(Color.ParseColor("#884bdf"));
                }

                //hightlight current selected user
                if (selectedGrid == position || currentUser == items[position].SubaccountName)
                {
                    griddrawable.SetColor(Color.ParseColor("#884bdf"));
                }
                else if (selectedGrid != position)
                {
                    griddrawable.SetColor(Color.Black);
                }
            }
            else
            {
                Glide.With(itemView).Load(items[position].SubaccountPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(vh.ImgSubAccount);
                vh.TextImgSubAccntName.Text = items[position].SubaccountName;
                vh.GridAccount.Visibility = ViewStates.Gone;
                vh.GridAccountImage.Visibility = ViewStates.Visible;
                vh.GridAccountImage.SetBackgroundResource(Resource.Drawable.tileview_layout);

                GradientDrawable gridimagedrawable = (GradientDrawable)vh.GridAccountImageDrawable.Background;

                //hightlight current selected user
                if (selectedGrid == position || currentUser == items[position].SubaccountName)
                {
                    gridimagedrawable.SetColor(Color.ParseColor("#884bdf"));
                }
                else if (selectedGrid != position)
                {
                    gridimagedrawable.SetColor(Color.Black);
                }
            }

           

            vh.ItemView.Click += delegate
            {
                SubAccountActivity.Instance.BtnSelectSubAccnt.ContentDescription = "selected";
                SubAccountActivity.Instance.SelectedSubAccnt(items[position]);
                selectedGrid = position;

                NotifyDataSetChanged();
            };
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.subaccounts_row, parent, false);

            vh = new SubAccountViewHolder(itemView, OnClick);
            return vh;
        }
        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }
        #endregion

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
        #endregion
    }
}