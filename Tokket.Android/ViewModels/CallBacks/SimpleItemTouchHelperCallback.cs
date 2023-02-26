using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Android.Interface;

namespace Tokket.Android.CallBack
{
    public class SimpleItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private readonly ITemTouchHelperAdapter _mAdapter;

        public SimpleItemTouchHelperCallback(ITemTouchHelperAdapter adapter)
        {
            _mAdapter = adapter;
        }

        public override bool IsLongPressDragEnabled => false;

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            const int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            const int swipeFlags = ItemTouchHelper.ActionStateIdle;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            if (viewHolder.ItemViewType != target.ItemViewType)
            {
                return false;
            }

            // Notify the adapter of the move
            _mAdapter.OnItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Notify the adapter of the dismissal
            _mAdapter.OnItemDismiss(viewHolder.AdapterPosition);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);

            if (viewHolder is AddTokPakViewHolder) {
                var myViewHolder = (AddTokPakViewHolder)viewHolder;
                _mAdapter.onRowClear(myViewHolder);
            }
            else if (viewHolder is AddSchedNotifViewHolder)
            {
                var myViewHolder = (AddSchedNotifViewHolder)viewHolder;
                _mAdapter.onRowClear(myViewHolder);
            }
        }
        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            if (actionState != ItemTouchHelper.ActionStateIdle)
            {
                if (viewHolder is AddTokPakViewHolder) {
                    var myViewHolder = (AddTokPakViewHolder)viewHolder;
                    _mAdapter.onRowSelected(myViewHolder);
                }else if (viewHolder is AddSchedNotifViewHolder)
                {
                    var myViewHolder = (AddSchedNotifViewHolder)viewHolder;
                    _mAdapter.onRowSelected(myViewHolder);
                }

            }

            base.OnSelectedChanged(viewHolder, actionState);
        }
    }
}