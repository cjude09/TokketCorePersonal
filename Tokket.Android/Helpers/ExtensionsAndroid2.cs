using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Android.Helpers
{
    //
    // Summary:
    //     Defines extension methods for Android only.
    public static class ExtensionsAndroid2
    {
        //
        // Summary:
        //     Creates a new GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2 for a given
        //     System.Collections.Generic.IList`1.
        //
        // Parameters:
        //   list:
        //     The list that the adapter will be created for.
        //
        //   bindViewHolderDelegate:
        //     A delegate to the method used to bind the view to the corresponding item.
        //
        //   createViewHolderDelegate:
        //     A delegate to the method used to create the view for the corresponding item.
        //
        //   clickCallback:
        //     An optional delegate to a method called when an item is clicked or tapped.
        //
        // Type parameters:
        //   TItem:
        //     The type of the items contained in the System.Collections.Generic.IList`1.
        //
        //   THolder:
        //     The type of the Android.Support.V7.Widget.RecyclerView.ViewHolder used in the
        //     RecyclerView. For better results and simpler implementation, you can use a GalaSoft.MvvmLight.Helpers.CachingViewHolder
        //     or provide your own implementation.
        //
        // Returns:
        //     The created GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.
        public static ObservableRecyclerAdapter<TItem, THolder> GetRecyclerAdapter<TItem, THolder>(this IList<TItem> list, Action<THolder, TItem, int> bindViewHolderDelegate, Func<ViewGroup, int, THolder> createViewHolderDelegate, Action<int, View, int, View> clickCallback = null) where THolder : RecyclerView.ViewHolder
        {
            return new ObservableRecyclerAdapter<TItem, THolder>
            {
                DataSource = list,
                BindViewHolderDelegate = bindViewHolderDelegate,
                CreateViewHolderDelegate = createViewHolderDelegate,
                ClickCallback = clickCallback
            };
        }

        //
        // Summary:
        //     Creates a new GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2 for a given
        //     System.Collections.Generic.IList`1.
        //
        // Parameters:
        //   list:
        //     The list that the adapter will be created for.
        //
        //   bindViewHolderDelegate:
        //     A delegate to the method used to bind the view to the corresponding item.
        //
        //   cellLayoutId:
        //     The resource ID of the AXML file used to create the cell.
        //
        //   clickCallback:
        //     An optional delegate to a method called when an item is clicked or tapped.
        //
        // Type parameters:
        //   TItem:
        //     The type of the items contained in the System.Collections.Generic.IList`1.
        //
        // Returns:
        //     The created GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.
        public static ObservableRecyclerAdapter<TItem, CachingViewHolder> GetRecyclerAdapter<TItem>(this IList<TItem> list, Action<CachingViewHolder, TItem, int> bindViewHolderDelegate, int cellLayoutId, Action<int, View, int, View> clickCallback = null)
        {
            return new ObservableRecyclerAdapter<TItem, CachingViewHolder>
            {
                DataSource = list,
                BindViewHolderDelegate = bindViewHolderDelegate,
                CellLayoutId = cellLayoutId,
                ClickCallback = clickCallback
            };
        }
    }
}