using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SharedHelpers = Tokket.Shared.Helpers;
using Android.Graphics;
using static Tokket.Android.Helpers.MenuHeaderHelper;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android.Adapters
{
    public class TokGroupBasicAdapter : RecyclerView.Adapter
    {
        List<IMenuItemsType> items;
        private Context ctx;
        private static int TYPE_HEADER = 0;
        private static int TYPE_ITEM = 1;
        string groupheader = "";
        public TokGroupBasicAdapter(Context _ctx,List<IMenuItemsType> _items)
        {
            this.items = _items;
            this.ctx = _ctx;
        }
        public override int ItemCount => items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            IMenuItemsType item = items[position];
            Console.WriteLine(item.GetMenuItemsType());
            if (item.GetMenuItemsType() == TYPE_HEADER) //Header
            {
                MenuHeaderItem _headerItem = (MenuHeaderItem)item;
                HeaderViewHolder headerHolder = holder as HeaderViewHolder;

                groupheader = _headerItem.HeaderText;
                headerHolder.headerTitle.Text = groupheader;
                headerHolder.headerTitle.SetBackgroundColor(Color.ParseColor(_headerItem.HeaderColor));

                headerHolder.headerTitle.Click += delegate
                {
                    SharedHelpers.Settings.FilterTag = 6;
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ToksActivity));
                    nextActivity.PutExtra("titlepage", "Tok Group");
                    nextActivity.PutExtra("filter", headerHolder.headerTitle.Text.ToLower());
                    nextActivity.PutExtra("headerpage", headerHolder.headerTitle.Text.ToLower());
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    MainActivity.Instance.StartActivity(nextActivity);
                };
            }
            else if (item.GetMenuItemsType() == TYPE_ITEM) //data
            {
                MenuContentItem _contentItem = (MenuContentItem)item;
                RecyclerViewHolder viewHolder = holder as RecyclerViewHolder;
                viewHolder.txtTitle.Tag = groupheader.ToLower();
                viewHolder.txtTitle.Text = _contentItem.Title;
                viewHolder.txtTitle.SetBackgroundColor(Color.ParseColor(_contentItem.TileColor));

                viewHolder.txtTitle.Click += delegate
                {
                    string[] titlearr = viewHolder.txtTitle.Text.ToLower().Split(" ");
                    string titletext = "";
                    for (int c = 0; c < titlearr.Length; c++)
                    {
                        titletext += titlearr[c];
                    }
                    string filter = "toktype-" + (string)viewHolder.txtTitle.Tag + "-" + titletext;
                    SharedHelpers.Settings.FilterTag = 1;
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ToksActivity));
                    nextActivity.PutExtra("titlepage", "Tok Type");
                    nextActivity.PutExtra("filter", filter);
                    nextActivity.PutExtra("headerpage", viewHolder.txtTitle.Text);
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    MainActivity.Instance.StartActivity(nextActivity);
                };
            }

            ////Check oif Header orData
            //if (holder.GetType() == typeof(HeaderViewHolder))
            //{
            //    HeaderViewHolder headerHolder = holder as HeaderViewHolder;
            //    headerHolder.headerTitle.Text = "hall0";

            ////    headerHolder.headerTitle.Click += delegate
            ////    {
            ////        Toast.MakeText(ctx, "Header!", ToastLength.Long).Show();
            ////    };
            //}
            //else if (holder.GetType() == typeof(RecyclerViewHolder))
            //{
            //    RecyclerViewHolder viewHolder = holder as RecyclerViewHolder;
            //    viewHolder.txtTitle.Text = (lstData[position - 1].description); // - 1 because of the header

            //}
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == TYPE_ITEM)
            {
                // Data
                LayoutInflater inflater = LayoutInflater.From(parent.Context);
                View itemView = inflater.Inflate(Resource.Layout.basictoktype_row, parent, false);
                return new RecyclerViewHolder(itemView, ctx);
            }
            else if (viewType == TYPE_HEADER)
            {
                //Inflating header view
                LayoutInflater inflater = LayoutInflater.From(parent.Context);
                View itemView = inflater.Inflate(Resource.Layout.basictoktypeheader_row, parent, false);
                return new HeaderViewHolder(itemView, ctx);
            }
            else return null;
        }
        // Custom Override to decided whether its the header or the data
        public override int GetItemViewType(int position)
        {
            IMenuItemsType item = items[position];
            if (item.GetMenuItemsType() == TYPE_HEADER)
            {
                return TYPE_HEADER;
            }
            else if (item.GetMenuItemsType() == TYPE_ITEM)
            {
                return TYPE_ITEM;
            }
            return TYPE_ITEM;
        }
    }
    // class for header
    public class HeaderViewHolder : RecyclerView.ViewHolder
    {
        public TextView headerTitle;

        public HeaderViewHolder(View itemView, Context ctx) : base(itemView)
        {
            headerTitle = itemView.FindViewById<TextView>(Resource.Id.lblBasicTokTypeHeaderRow);
        }
    }
    // xml
    class RecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtTitle { get; set; }
        private Context ctx;


        public RecyclerViewHolder(View itemView, Context ctx) : base(itemView)
        {
            this.ctx = ctx;
            txtTitle = itemView.FindViewById<TextView>(Resource.Id.lblBasicTokTypeRow);

        }
    }
}