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

namespace Tokket.Android.Helpers
{
    public class MenuHeaderHelper
    {
        public static int TYPE_HEADER = 0;
        public static int TYPE_ITEM = 1;
        public interface IMenuItemsType
        {
            int GetMenuItemsType();
        }
        public class MenuHeaderItem : IMenuItemsType
        {
            public string HeaderText { get; set; }
            public string HeaderColor { get; set; }

            public int GetMenuItemsType()
            {
                return TYPE_HEADER;// return 0. It is header
            }

            public MenuHeaderItem(string _headerText, string _tilecolor)
            {
                HeaderText = _headerText;// return title of header
                HeaderColor = _tilecolor;
            }
        }

        public class MenuContentItem : IMenuItemsType
        {
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string TileColor { get; set; }

            public int GetMenuItemsType()
            {
                return TYPE_ITEM;// return 1
            }

            public MenuContentItem(string _title, string _subtitle, string _tilecolor)
            {
                Title = _title;
                SubTitle = _subtitle;
                TileColor = _tilecolor;
            }
        }
    }
}