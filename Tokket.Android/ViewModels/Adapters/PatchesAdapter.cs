using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Android.Graphics;
using Android.Text;
using Tokket.Android.Helpers;
using Color = Android.Graphics.Color;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.Adapters
{
    public class PatchesAdapter : RecyclerView.Adapter
    {
        int selectedPosition = -1;
        View holderView;
        public event EventHandler<int> ItemClick;
        List<PointsSymbolModel> PointsSymbols;
        public override int ItemCount => PointsSymbols.Count();
        View itemView; PatchesTab TabNum; PointsSymbolModel NextLevel;

        #region Constructor
        public PatchesAdapter(List<PointsSymbolModel> _PointsSymbols, PatchesTab tabNum, PointsSymbolModel nextLevel)
        {
            PointsSymbols = _PointsSymbols;
            TabNum = tabNum;
            NextLevel = nextLevel;
        }
        #endregion

        #region Override Events/Methods/Delegates
        PatchesViewHolder vh; string degreenumber = "";
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as PatchesViewHolder;

            vh.TextRankLabel.Text = PointsSymbols[position].Level;
            
            if (PointsSymbols[position].Degree >= 1)
            {
                degreenumber = IntegerExtensions.DisplayWithSuffix(PointsSymbols[position].Degree) + " Degree";
            }
            else
            {
                degreenumber = "";
            }

            vh.TextRankTitle.Text = PointsSymbols[position].Name + " " + degreenumber;
            vh.TextPoints.Text = PointsSymbols[position].PointsRequired.ToString() + " points";
            Glide.With(itemView).Load(PointsSymbols[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(vh.ImgPatches);

            if (TabNum == PatchesTab.LevelTable)
            {
                if (NextLevel != null)
                {
                    if (PointsSymbols[position].index < NextLevel.index)
                    {
                        vh.TextRankLabel.SetTextColor(Color.ParseColor("#30000000"));
                        vh.TextRankTitle.SetTextColor(Color.ParseColor("#30000000"));
                        vh.TextPoints.SetTextColor(Color.ParseColor("#30000000"));
                    }
                }
            }
            else if (TabNum == PatchesTab.PatchColor)
            {
                vh.TextPoints.Visibility = ViewStates.Gone;
                holder.ItemView.Click += delegate
                {
                    PatchesActivity.Instance.BtnChangeColorCmd.Tag = position;
                    if (selectedPosition != -1)
                    {
                        holderView.SetBackgroundColor(Color.ParseColor("#ffffff"));
                    }
                    holder.ItemView.SetBackgroundColor(Color.LightBlue);
                    PatchesActivity.Instance.BtnChangeColorCmd.Tag = position;
                    selectedPosition = position;
                    holderView = holder.ItemView;
                    if (PatchesActivity.Instance.TokketUserCur.PointsSymbolColor.ToLower() == PointsSymbols[position].Name.ToLower())
                    {
                        PatchesActivity.Instance.BtnChangeColorCmd.Enabled = false;
                        PatchesActivity.Instance.EnableChangeColorButton();

                    }
                    else {
                        PatchesActivity.Instance.BtnChangeColorCmd.Enabled = true;
                        PatchesActivity.Instance.EnableChangeColorButton(true);
                    }

                };
            }
            else
            {
                if (NextLevel != null)
                {
                    if (NextLevel.index - 1 == PointsSymbols[position].index) //If position is in the current rank
                    {
                        holder.ItemView.SetBackgroundColor(Color.ParseColor("#b085ed"));
                    }
                }
            }


            if (PatchesActivity.Instance.TokketUserCur.PointsSymbolColor?.ToLower() == PointsSymbols[position].Name.ToLower()) {
                holder.ItemView.SetBackgroundColor(Color.LightBlue);
                holderView = holder.ItemView;
                selectedPosition = position;
            }
             

          
            //vh.ImgBadge.Click += (sender, e) =>
            //{
            //    if (selectedPosition != -1)
            //    {
            //        holderView.SetBackgroundColor(Color.ParseColor("#dddddd"));
            //    }
            //    holder.ItemView.SetBackgroundColor(Color.LightBlue);

            //    BadgesActivity.Instance.SelectCommand.Tag = position;
            //    selectedPosition = position;
            //    holderView = holder.ItemView;
            //};
        }



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.patchesfragment_row, parent, false);

            vh = new PatchesViewHolder(itemView, OnClick);
            
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
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);

        
        }
    }
}