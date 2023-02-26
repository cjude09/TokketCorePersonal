using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;

namespace Tokket.Android.Adapters
{
    class ClassFilterByAdapter : RecyclerView.Adapter
    {
        View itemView;
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        Context context; List<CommonModel> listCommonModel;
        FilterBy filterByEnum = FilterBy.None;
        int typePosition;
        bool isSetSelected = true;
        string activityCaller = "";

        public ClassFilterByAdapter(Context context, List<CommonModel> _listCommonModel, int filterByEnum, int typePosition = -1, string activityCaller = "")
        {
            this.context = context;
            this.listCommonModel = _listCommonModel;
            this.filterByEnum = (FilterBy)filterByEnum;
            this.typePosition = typePosition;
            this.activityCaller = activityCaller;
        }

        public override int ItemCount => listCommonModel.Count;

        public override long GetItemId(int position)
        {
            return position;
        }


        ClassFilterByViewHolder vh; int selectedPosition = -1;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as ClassFilterByViewHolder;
            List<string> filterItems = new List<string>();

            if (listCommonModel[position].isSelected)
            {
                //auto highlight row when there is already selected based on cache
                vh.ItemView.SetBackgroundColor(new Color(ContextCompat.GetColor(context, Resource.Color.lightBlue)));
            }

            vh.txtClassFilter.Text = listCommonModel[position].Title;

            if (filterByEnum == FilterBy.Type)
            {
                if (typePosition >= 0)
                {
                    if (isSetSelected)
                    {
                        selectedPosition = typePosition;
                        isSetSelected = false;
                    }
                }
            }

            if (selectedPosition == position)
            {
                listCommonModel[position].isSelected = !listCommonModel[position].isSelected;

                //hightlight current selected row
                if (listCommonModel[position].isSelected)
                {
                    vh.ItemView.SetBackgroundColor(new Color(ContextCompat.GetColor(context, Resource.Color.lightBlue)));
                }
                else
                {
                    vh.ItemView.SetBackgroundColor(Color.Transparent);
                }

                //var filterByList = listCommonModel.Where(x => x.isSelected == true).ToList();
                foreach (var item in listCommonModel)
                {
                    if (item.isSelected)
                    {
                        string itemSelected = "";
                        switch (filterByEnum)
                        {
                            case FilterBy.Class:
                                itemSelected = item.Id;
                                break;
                            case FilterBy.Category:
                                itemSelected = item.Id;
                                break;
                            case FilterBy.Type:
                                itemSelected = item.Title;
                                break;
                            default:
                                break;
                        }
                        filterItems.Add(itemSelected);
                    }
                }

                ClassFilterbyActivity.Instance.btnApplyFilter.ContentDescription = JsonConvert.SerializeObject(filterItems);
            }
            else
            {
                vh.ItemView.SetBackgroundColor(Color.Transparent);
            }

            vh.ItemView.Tag = position;
            vh.ItemView.Click -= itemViewIsClicked;
            vh.ItemView.Click += itemViewIsClicked;
           /* vh.ItemView.Click += (sender, e) =>
            {
                selectedPosition = position;
                NotifyDataSetChanged();
            };*/
        }

        public void itemViewIsClicked(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as View).Tag; } catch { position = int.Parse((string)(sender as View).Tag); }

            if (selectedPosition >= 0)
            {
                //Unselect previous selected data
                listCommonModel[selectedPosition].isSelected = !listCommonModel[selectedPosition].isSelected;
            }
            selectedPosition = position;

            NotifyDataSetChanged();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.settings_row, parent, false);

            vh = new ClassFilterByViewHolder(itemView, OnClick);

            return vh;
        }
        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}