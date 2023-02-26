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
using Tokket.Android.ViewModels;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Extensions;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using Google.Android.Material.Color;

namespace Tokket.Android.Adapters
{
    class TokChannelsLevelAdapter : RecyclerView.Adapter
    {

        #region Members/Properties
        public event EventHandler<int> ItemClick;
        View itemView;
        Context context;
        List<LevelViewModel> items;
        TokChannelsLevelViewHolder vh;
        int level = 0;
        #endregion

        public TokChannelsLevelAdapter(Context context, List<LevelViewModel> item, int levelType = 2)
        {
            this.context = context;
            this.items = item;
            this.level = levelType;
            this.firstLoad = true;
        }

        public override int ItemCount => items.Count;

        int selectedPosition = -1;
        bool firstLoad = true;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as TokChannelsLevelViewHolder;
            vh.TextLevelName.Text = items[position].levelName;
            vh.GridLevel.SetBackgroundResource(Resource.Drawable.tile_tok_channel_background);
            GradientDrawable Tokdrawable = (GradientDrawable)vh.GridLevelDrawable.Background;
            Tokdrawable.SetStroke(10, Color.ParseColor(items[position].colorBackground));

            int textColor = MaterialColors.GetColor(context, Resource.Attribute.textColor, Color.Black); //If theme is dark this will change to white
            int bgColor = MaterialColors.GetColor(context, Resource.Attribute.bgColor, Color.White); //If theme is dark this will change to grey
            vh.TextLevelName.SetTextColor(new Color(textColor));
            Tokdrawable.SetColor(new Color(bgColor));

            vh.GridLevel.Click += delegate
            {
                firstLoad = false;
                selectedPosition = position;
                items[position].isSelected = !items[position].isSelected;

                NotifyDataSetChanged();
            };

            if (firstLoad)
            {
                if (items[position].levelName.ToLower() == "all")
                {
                    selectedPosition = position;
                    items[position].isSelected = !items[position].isSelected;
                }
            }

            if (selectedPosition >= 0)
            {
                if (position == selectedPosition)
                {
                    vh.TextLevelName.SetTextColor(Color.White);

                    GradientDrawable TokdrawableClick = (GradientDrawable)vh.GridLevelDrawable.Background;
                    TokdrawableClick.SetColor(Color.ParseColor(items[selectedPosition].colorBackground));

                    TokChannelActivity.Instance.LoadLevel3List(vh.TextLevelName.Text, items[selectedPosition].colorBackground, level);
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.tok_channel_level_row, parent, false);

            vh = new TokChannelsLevelViewHolder(itemView, OnClick);

            return vh;
        }

        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
        #endregion
    }
}