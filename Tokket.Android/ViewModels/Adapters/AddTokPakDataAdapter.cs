using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Flexbox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tokket.Android.Interface;
using Tokket.Android.ViewModels;
using Tokket.Core;
using static Android.Views.View;

namespace Tokket.Android.Adapters
{
    public class AddTokPakDataAdapter : RecyclerView.Adapter, ITemTouchHelperAdapter, View.IOnTouchListener, IOnLongClickListener
    {
        private Context context;
        private  ObservableCollection<TokPakDetailViewModel> itemList;
        public readonly IOnStartDragListener _mDragStartListener;
        public AddTokPakViewHolder vh;

        public AddTokPakDataAdapter(Context _context, ObservableCollection<TokPakDetailViewModel> list, IOnStartDragListener mDragStartListener)
        {
            context = _context;
            itemList = list;
            _mDragStartListener = mDragStartListener;
        }

        public override int ItemCount => itemList.Count;
        int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as AddTokPakViewHolder;
            if (viewHolder == null) return;

            vh = viewHolder;
            vh.ReorderView.Tag = position;
            vh.ReorderView.SetOnLongClickListener(this);

            vh.txtPrimaryNumber.Text = "Page " + (position + 1).ToString();

            vh.btnRemovePaste.Tag = position;
            vh.btnFindTok.Tag = position;
            vh.btnPasteTok.Tag = position;

            if (position > 4)
            {
                vh.btnRemovePaste.Visibility = ViewStates.Visible;
            }

            if (itemList[position] != null)
            {
                if (!string.IsNullOrEmpty(itemList[position].classTokModel.PrimaryFieldText))
                {
                    vh.txtPagePrimary.Text = itemList[position].classTokModel.PrimaryFieldText;
                    vh.btnRemovePaste.Visibility = ViewStates.Visible;
                    vh.btnInsert.Visibility = ViewStates.Visible;
                    vh.txtPagePrimary.Visibility = ViewStates.Visible;

                    vh.flexButtonToks.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.flexButtonToks.Visibility = ViewStates.Visible;

                    vh.btnRemovePaste.Visibility = ViewStates.Gone;
                    vh.btnInsert.Visibility = ViewStates.Gone;
                    vh.txtPagePrimary.Visibility = ViewStates.Gone;
                }
            }

            vh.btnRemovePaste.Tag = position;
            vh.btnRemovePaste.Click -= AddTokPakActivity.Instance.btnRemove_Click;
            vh.btnRemovePaste.Click += AddTokPakActivity.Instance.btnRemove_Click;

            vh.btnFindTok.Tag = position;
            vh.btnFindTok.Click -= AddTokPakActivity.Instance.btnFindTok_Click;
            vh.btnFindTok.Click += AddTokPakActivity.Instance.btnFindTok_Click;

            vh.btnPasteTok.Tag = position;
            vh.btnPasteTok.Click -= AddTokPakActivity.Instance.btnPasteTok_Click;
            vh.btnPasteTok.Click += AddTokPakActivity.Instance.btnPasteTok_Click;

            vh.btnAddClassTok.Tag = position;
            vh.btnAddClassTok.Click -= AddTokPakActivity.Instance.btnAddClassTok_Click;
            vh.btnAddClassTok.Click += AddTokPakActivity.Instance.btnAddClassTok_Click;

            //Load the copied or paste tok
            vh.recyclerTok.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(context));

            var tokAdapter = new ClassTokDataAdapter(context, itemList[position].tokItemList, new List<Tokmoji>());
            vh.recyclerTok.SetAdapter(tokAdapter);

            if (itemList[position].tokItemList.Count == 0)
            {
                vh.recyclerTok.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.recyclerTok.Visibility = ViewStates.Visible;
            }

            vh.ItemView.Tag = position;
            vh.ItemView.SetOnTouchListener(this);

            //if (position == selectedPosition)
            //{
            //    vh.btnAddClassTok.Visibility = ViewStates.Visible;
            //}
            //else
            //{
            //    vh.btnAddClassTok.Visibility = ViewStates.Gone;
            //}

            vh.btnAddClassTok.Visibility = ViewStates.Visible;

            vh.btnInsert.Tag = position;
            vh.btnInsert.Click -= btnInsertClicked;
            vh.btnInsert.Click += btnInsertClicked;
        }

        private void btnInsertClicked(object sender, EventArgs e)
        {
            int ndx = 0;
            View v = sender as View;// (((sender as View).Parent as View).Parent as View);
            try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }

            var menu = new AndroidX.AppCompat.Widget.PopupMenu(context, v);

            // Call inflate directly on the menu:
            menu.Inflate(Resource.Menu.add_tok_pak_insert_menu);
            var itemAbove = menu.Menu.FindItem(Resource.Id.itemAbove);
            var itemBelow = menu.Menu.FindItem(Resource.Id.itemBelow);

            itemAbove.SetVisible(true);
            itemBelow.SetVisible(true);
            if (ndx == 0)
            {
                itemAbove.SetVisible(false);
            }
            else if (ndx == itemList.Count - 1)
            {
                itemBelow.SetVisible(false);
            }

            int newPos = 0;
            // A menu item was clicked:
            menu.MenuItemClick += (s1, arg1) => {
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "above":
                        newPos = ndx;// - 1;
                        //OnItemMove(ndx, newPos);
                        //(sender as View).Tag = newPos;
                        itemList = AddTokPakActivity.Instance.InsertPage(newPos);
                        break;
                    case "below":
                        newPos = ndx + 1;
                        //OnItemMove(ndx, ndx + 1);
                        //(sender as View).Tag = newPos;
                        itemList = AddTokPakActivity.Instance.InsertPage(newPos);
                        break;
                }
               
                ApplySequenceChange();
                AddTokPakActivity.Instance.UpdateItemPosition();
                //Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
            };

            // Menu was dismissed:
            menu.DismissEvent += (s2, arg2) => {
                //Console.WriteLine("menu dismissed");
            };

            menu.Show();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var itemView = inflater.Inflate(Resource.Layout.add_tok_pak_row, parent, false);
            return new AddTokPakViewHolder(itemView);
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
            
            AddTokPakActivity.Instance.PagesCollection = itemList;
            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }

        public void onRowSelected(AddTokPakViewHolder myViewHolder)
        {
            myViewHolder.ReorderView.SetBackgroundColor(Color.LightBlue);
        }

        public void onRowClear(AddTokPakViewHolder myViewHolder)
        {
            myViewHolder.ReorderView.SetBackgroundColor(Color.Transparent);
            AddTokPakActivity.Instance.setRecyclerPagesAdapter(); //Refresh to apply sequence change
        }


        private async void ApplySequenceChange() {
            try
            {
                await Task.Delay(150);
                //Add try catch to avoid error or crash due to conflict
                NotifyDataSetChanged();
            }
            catch (Exception)
            {
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            int ndx = 0;
            try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }

            selectedPosition = ndx;


            //View viewGroup = ((ViewGroup)v).GetChildAt(3); //btnAddClassTok was in position 3

            var line = vh.btnAddClassTok;//viewGroup.FindViewById<View>(Resource.Id.btnAddClassTok);

            if (e.Action == MotionEventActions.Down)
            {
                line.Visibility = ViewStates.Visible;
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
        
         public bool OnLongClick(View v)
        {
            //Get the Recycler's vh. Add AddTokPakActivity.Instance.setRecyclerPagesAdapter() in onRowClear to refresh the list
            var currentVH = AddTokPakActivity.Instance.getRecyclerViewHolder(selectedPosition);
            _mDragStartListener.OnStartDrag(currentVH);
            return true;
        }

        public void onRowSelected(AddSchedNotifViewHolder myViewHolder)
        {
            throw new NotImplementedException();
        }

        public void onRowClear(AddSchedNotifViewHolder myViewHolder)
        {
            throw new NotImplementedException();
        }
    }

    public class AddTokPakViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout ReorderView;
        public AndroidX.RecyclerView.Widget.RecyclerView recyclerTok;
        public TextView txtPrimaryNumber;
        public TextView txtPagePrimary;
        public Button btnPasteTok;
        public Button btnFindTok;
        public Button btnRemovePaste;
        public Button btnAddClassTok;
        public Button btnInsert;
        public FlexboxLayout flexButtonToks;

        public AddTokPakViewHolder(View view) : base(view)
        {
            ReorderView = view.FindViewById<LinearLayout>(Resource.Id.linearTokPakRow);
            recyclerTok = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.recyclerTok);
            txtPrimaryNumber = view.FindViewById<TextView>(Resource.Id.txtPrimaryNumber);
            txtPagePrimary = view.FindViewById<TextView>(Resource.Id.txtPagePrimary);
            btnPasteTok = view.FindViewById<Button>(Resource.Id.btnPasteTok);
            btnFindTok = view.FindViewById<Button>(Resource.Id.btnFindTok);
            btnRemovePaste = view.FindViewById<Button>(Resource.Id.btnRemovePaste);
            btnAddClassTok = view.FindViewById<Button>(Resource.Id.btnAddClassTok);
            btnInsert = view.FindViewById<Button>(Resource.Id.btnInsert);
            flexButtonToks = view.FindViewById<FlexboxLayout>(Resource.Id.flexButtonToks);
        }
    }
}