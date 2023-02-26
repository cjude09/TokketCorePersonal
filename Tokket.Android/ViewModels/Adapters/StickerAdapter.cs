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
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Helpers;

namespace Tokket.Android.Adapters
{
    public class StickerAdapter : RecyclerView.Adapter, View.IOnTouchListener, View.IOnLongClickListener
    {
        #region Members/Properties
        public event EventHandler<int> ItemClick;
        View itemView;
        List<Tokket.Core.Sticker> items;
        public override int ItemCount => items.Count;
        #endregion

        #region Constructor
        public StickerAdapter(List<Tokket.Core.Sticker> _items)
        {
            items = _items;
        }
        #endregion

        #region Override Events/Methods/Delegates
        StickerViewHolder vh;
        int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as StickerViewHolder;
            var res = Application.Context.Resources;

            Glide.With(itemView).Load(items[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(vh.StickerImage);
            vh.StickerTitle.Text = items[position].Name;
            vh.NumCoins.Text = items[position].PriceCoins.ToString();

            if (selectedPosition == position)
            {
                vh.ItemView.SetBackgroundColor(Color.LightBlue);
            }
            else if (selectedPosition != position)
            {
                vh.ItemView.SetBackgroundColor(Color.Transparent);
            }

            vh.ItemView.ContentDescription = items[position].Image;
            vh.ItemView.SetOnLongClickListener(this);

            vh.ItemView.Click += (sender, e) =>
            {
                selectedPosition = position;
                NotifyDataSetChanged();
            };
        }

        public void OnGridBackgroundClick(object sender, int position)
        {

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.modal_addsticker_row, parent, false);

            vh = new StickerViewHolder(itemView, OnClick);

            return vh;
        }
        #endregion

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return false;
        }

        public bool OnLongClick(View v)
        {
            Settings.byteImageViewer = v.ContentDescription;
            Intent nextActivity = new Intent(v.Context, typeof(DialogImageViewerActivity));
            v.Context.StartActivity(nextActivity);
            return true;
        }
        #endregion
    }
}