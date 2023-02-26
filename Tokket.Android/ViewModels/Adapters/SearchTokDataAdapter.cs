using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Extensions;
using Android.Graphics.Drawables;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.Adapters
{
    class SearchTokDataAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<ClassTokModel> items;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => items.Count;
        View itemView;

        #region Abstract Methods
        public void UpdateItems(List<ClassTokModel> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeChanged(position, listUpdate.Count);
        }

        public void AddItem(ClassTokModel item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(ClassTokModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Constructor
        public SearchTokDataAdapter(List<ClassTokModel> _items)
        {
            items = _items;
        }
        #endregion

        SearchTokViewHolder vh;
        int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as SearchTokViewHolder;

            vh.txtUserDisplayName.Text = items[position].UserDisplayName;
            vh.txtPrimaryFieldText.Text = items[position].PrimaryFieldText;
            Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.imageTokImgUserPhoto);
            vh.flexBox.SetBackgroundResource(Resource.Drawable.tileview_layout1);
            vh.flexBox.Tag = position;
            int ndx = position % Colors.Count;
            if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            GradientDrawable Tokdrawable = (GradientDrawable)vh.flexBox.Background;
            Tokdrawable.SetColor(Color.ParseColor(randomcolors[ndx]));
            var tokimg = items[position].ThumbnailImage;
            if (string.IsNullOrEmpty(items[position].Image))
            {

            }
            else
            {
                if (items[position].ThumbnailImage.EndsWith(".png") || items[position].ThumbnailImage.EndsWith(".jpg"))
                {
                    tokimg = items[position].ThumbnailImage;
                }
                else
                {
                    tokimg = items[position].ThumbnailImage + ".jpg";
                }
                Glide.With(itemView).Load(tokimg).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(vh.imageTok);
            }

            if (position == selectedPosition)
            {
                vh.BG.SetBackgroundColor(Color.LightBlue);
            }
            else
            {
                vh.BG.SetBackgroundColor(Color.White);
            }

            holder.ItemView.Click += (sender, e) =>
            {
                /*if (selectedPosition != -1)
                {
                    holder.ItemView.SetBackgroundColor(Color.Transparent);
                }*/
                SearchToksDialog.Instance.btnAddTokLink.ContentDescription = JsonConvert.SerializeObject(items[position]);
                holder.ItemView.SetBackgroundColor(Color.LightBlue);
                selectedPosition = position;
                NotifyDataSetChanged();
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.search_toks_row, parent, false);

            vh = new SearchTokViewHolder(itemView, OnClick);
            return vh;
        }
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}