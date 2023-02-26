using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.ObjectModel;
using Tokket.Android.Interface;
using Tokket.Android.ViewModels;
using static Android.Views.View;

namespace Tokket.Android.Adapters
{
    public class AddSchedNotifAdapter : RecyclerView.Adapter, ITemTouchHelperAdapter, View.IOnTouchListener, IOnLongClickListener
    {
        private Context context;
        private ObservableCollection<ScheduleNotifViewModel> itemList;
        public readonly IOnStartDragListener _mDragStartListener;
        public AddSchedNotifViewHolder vh;
        public override int ItemCount => itemList.Count;
        int selectedPosition = -1;
        public AddSchedNotifAdapter(Context _context, ObservableCollection<ScheduleNotifViewModel> list, IOnStartDragListener mDragStartListener)
        {
            context = _context;
            itemList = list;
            _mDragStartListener = mDragStartListener;
        }
        
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as AddSchedNotifViewHolder;
            if (viewHolder == null) return;

            vh = viewHolder;
            vh.ReorderView.Tag = position;
            vh.ReorderView.SetOnLongClickListener(this);

            vh.ItemView.Tag = position;
            vh.ItemView.SetOnTouchListener(this);

            vh.txtItem.Hint = itemList[position].stringHeader;
            vh.txtHeader.Text = itemList[position].stringHeader;

            var contentBinding = new Binding<string, string>(itemList[position],
                                                  () => itemList[position].stringText,
                                                  vh.txtItem,
                                                  () => vh.txtItem.Text,
                                                  BindingMode.TwoWay);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(context);
            var itemView = inflater.Inflate(Resource.Layout.string_notification_row, parent, false);
            return new AddSchedNotifViewHolder(itemView);
        }

        public void OnItemDismiss(int position)
        {
            var item = itemList[position];
            itemList.Remove(item);
            NotifyItemRemoved(position);
        }

        public bool OnItemMove(int fromPosition, int toPosition)
        {
            var tempPlanResource = itemList[fromPosition];
            itemList[fromPosition] = itemList[toPosition];
            itemList[toPosition] = tempPlanResource;

            ClassGroupActivity.Instance.ScheduleNotifCollection = itemList;
            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }

        public bool OnLongClick(View v)
        {
            //Get the Recycler's vh. Add AddTokPakActivity.Instance.setRecyclerPagesAdapter() in onRowClear to refresh the list
            var currentVH = ClassGroupActivity.Instance.getRecyclerSchedNotifViewHolder(selectedPosition);
            _mDragStartListener.OnStartDrag(currentVH);
            return true;
        }

        public void onRowClear(AddSchedNotifViewHolder myViewHolder)
        {
            myViewHolder.ReorderView.SetBackgroundColor(Color.Transparent);
            ClassGroupActivity.Instance.RefreshRecyclerSchedNotif(); //Refresh to apply sequence change
        }

        public void onRowSelected(AddSchedNotifViewHolder myViewHolder)
        {
            myViewHolder.ReorderView.SetBackgroundColor(Color.LightBlue);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            int ndx = 0;
            try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }

            selectedPosition = ndx;


            //View viewGroup = ((ViewGroup)v).GetChildAt(3); //btnAddClassTok was in position 3

           // var line = vh.btnAddClassTok;//viewGroup.FindViewById<View>(Resource.Id.btnAddClassTok);

            if (e.Action == MotionEventActions.Down)
            {
             //   line.Visibility = ViewStates.Visible;
            }
            else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
            {
                //line.Visibility = ViewStates.Gone;
            }

            /* try
             {
                 //Add try catch to avoid error or crash due to conflict
                 NotifyDataSetChanged();
             }
             catch (Exception)
             {
             }*/

            return false;
        }

        public void onRowSelected(AddTokPakViewHolder myViewHolder)
        {
            throw new NotImplementedException();
        }

        public void onRowClear(AddTokPakViewHolder myViewHolder)
        {
            throw new NotImplementedException();
        }
    }
    class AddShedNotifAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }

    public class AddSchedNotifViewHolder : RecyclerView.ViewHolder {

        public LinearLayout ReorderView;
        public TextView txtHeader;
        public EditText txtItem;
        public AddSchedNotifViewHolder(View view) : base(view) {
            txtHeader = view.FindViewById<TextView>(Resource.Id.txtHeader);
            txtItem = view.FindViewById<EditText>(Resource.Id.txtItem);
            ReorderView = view.FindViewById<LinearLayout>(Resource.Id.linearSchedNotifRow);
        }
    }
}