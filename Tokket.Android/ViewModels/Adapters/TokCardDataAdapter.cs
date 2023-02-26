using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Core;
using static Bumptech.Glide.Request.Transition.NoTransition;

namespace Tokket.Android.Adapters
{
    public class TokCardDataAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        #region Members/Properties
        // Event handler for item clicks:
        private bool Showingback;
        public event EventHandler<int> ItemClick;
        SpannableStringBuilder ssbName;
        ISpannable spannableResText;
        List<TokModel> items;
        public override int ItemCount => items.Count;
        View itemView;
        #endregion

        #region Constructor
        public TokCardDataAdapter(List<TokModel> _items, List<Tokmoji> _listTokMoji)
        {
            items = _items;
            SpannableHelper.ListTokMoji = _listTokMoji;
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<TokModel> listUpdate)
        {
            items.AddRange(listUpdate);
            NotifyDataSetChanged();
        }

        public void AddItem(TokModel item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(TokModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        CardViewHolder vh;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as CardViewHolder;
            ssbName = new SpannableStringBuilder(items[position].PrimaryFieldText);
            spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);

            //vh.tokcardfront.Text = items[position].PrimaryFieldText;
            vh.tokcardfront.SetText(spannableResText, TextView.BufferType.Spannable);

            if (items[position].IsDetailBased == true)
            {
                if (items[position].Details != null)
                {
                    string detailstr = "";
                    for (int i = 0; i < items[position].Details.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(items[position].Details[i]))
                        {
                            if (i == 0)
                            {
                                detailstr = "• " + items[position].Details[i].ToString();
                            }
                            else
                            {
                                detailstr += "\n• " + items[position].Details[i].ToString();
                            }
                        }
                    }
                    vh.tokcardback.Typeface = Typeface.Default;

                    ssbName = new SpannableStringBuilder(detailstr);
                    spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
                    vh.tokcardback.SetText(spannableResText, TextView.BufferType.Spannable);
                }
            }
            else
            {
                //vh.tokcardback.Text = items[position].SecondaryFieldText;
                ssbName = new SpannableStringBuilder(items[position].SecondaryFieldText);
                spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
                vh.tokcardback.SetText(spannableResText, TextView.BufferType.Spannable);
            }

            vh.tokcardviewflipper.Tag = position;
            vh.tokcardviewflipper.SetOnTouchListener(this);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.mysets_tokcards_row, parent, false);

            vh = new CardViewHolder(itemView, OnClick);
            return vh;
        }
        public void OnItemBackgroundClick(object sender, int position)
        {
           
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                //When Tok Card is flipped
                var flipper = v.FindViewById<ViewFlipper>(Resource.Id.viewFlipper_settokcard);
                if (Showingback)
                { //Front
                  // Use custom animations
                  flipper.SetInAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_left_in);
                  flipper.SetOutAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_left_out);

                    // Use Android built-in animations
                    //flipper.SetInAnimation(MainActivity.Instance, Android.Resource.Animation.SlideInLeft);
                    //flipper.SetOutAnimation(MainActivity.Instance, Android.Resource.Animation.SlideOutRight);
                    flipper.ShowPrevious();
                    Showingback = false;
                }
                else
                { //Back
                  // Use custom animations
                  //flipper.SetInAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_right_out);
                  //flipper.SetOutAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_right_out);

                    // Use Android built-in animations
                    //flipper.SetInAnimation(MainActivity.Instance, Android.Resource.Animation.SlideInLeft);
                    //flipper.SetOutAnimation(MainActivity.Instance, Android.Resource.Animation.SlideOutRight);

                    flipper.SetInAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_left_in);
                    flipper.SetOutAnimation(MainActivity.Instance, Resource.Animation.viewflipper_card_left_out);

                    flipper.ShowNext();
                }
            }
            return true;
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