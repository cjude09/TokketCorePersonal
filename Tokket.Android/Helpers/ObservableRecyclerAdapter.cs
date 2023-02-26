using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Android.Helpers
{
    //
    // Summary:
    //     A RecyclerView.Adapter that automatically updates the
    //     associated Android.Support.V7.Widget.RecyclerView when its data source changes.
    //     Note that the changes are only observed if the data source implements System.Collections.Specialized.INotifyCollectionChanged.
    //
    // Type parameters:
    //   TItem:
    //     The type of the items in the data source.
    //
    //   THolder:
    //     The type of the RecyclerView.ViewHolder used in the
    //     RecyclerView. For better results and simpler implementation, you can use a GalaSoft.MvvmLight.Helpers.CachingViewHolder
    //     or provide your own implementation.
    public class ObservableRecyclerAdapter<TItem, THolder> : RecyclerView.Adapter, INotifyPropertyChanged where THolder : RecyclerView.ViewHolder
    {
        //
        // Summary:
        //     The GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.SelectedItem property's
        //     name.
        public const string SelectedItemPropertyName = "SelectedItem";

        private IList<TItem> _dataSource;

        private INotifyCollectionChanged _notifier;

        private int _oldPosition = -1;

        private View _oldView;

        private TItem _selectedItem;

        //
        // Summary:
        //     A delegate to a method taking a Android.Support.V7.Widget.RecyclerView.ViewHolder
        //     and setting its View's properties according to the item passed as second parameter.
        public Action<THolder, TItem, int> BindViewHolderDelegate { get; set; }

        //
        // Summary:
        //     The Resource ID of the AXML file we should use to create cells for the RecyclerView.
        //     Alternatively you can use the GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.CreateViewHolderDelegate
        //     property.
        public int CellLayoutId { get; set; }

        //
        // Summary:
        //     A delegate to a callback that will be called when an item in the list is clicked
        //     (or tapped) by the user. This can be used to perform UI operations such as changing
        //     the background color, etc.
        public Action<int, View, int, View> ClickCallback { get; set; }

        //
        // Summary:
        //     A delegate to a method taking an item's position and a Android.Support.V7.Widget.RecyclerView.ViewHolder
        //     and creating and returning a cell for the RecyclerView. Alternatively you can
        //     use the GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.CellLayoutId property.
        public Func<ViewGroup, int, THolder> CreateViewHolderDelegate { get; set; }

        //
        // Summary:
        //     The data source of this list adapter.
        public IList<TItem> DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (!object.Equals(_dataSource, value))
                {
                    if (_notifier != null)
                    {
                        _notifier.CollectionChanged -= HandleCollectionChanged;
                    }

                    _dataSource = value;
                    _notifier = value as INotifyCollectionChanged;
                    if (_notifier != null)
                    {
                        _notifier.CollectionChanged += HandleCollectionChanged;
                    }

                    NotifyDataSetChanged();
                }
            }
        }

        //
        // Summary:
        //     Gets the number of items in the data source.
        public override int ItemCount
        {
            get
            {
                if (_dataSource != null)
                {
                    return _dataSource.Count;
                }

                return 0;
            }
        }

        //
        // Summary:
        //     Gets the RecyclerView's selected item. You can use one-way databinding on this
        //     property.
        public TItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            protected set
            {
                if (!object.Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                    RaiseSelectionChanged();
                }
            }
        }

        //
        // Summary:
        //     Occurs when a property of this instance changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Occurs when a new item gets selected in the UICollectionView.
        public event EventHandler SelectionChanged;

        //
        // Summary:
        //     Gets an item corresponding to a given row position.
        //
        // Parameters:
        //   row:
        //     The row position of the item.
        //
        // Returns:
        //     An item corresponding to a given row position.
        public TItem GetItem(int row)
        {
            return _dataSource[row];
        }

        //
        // Summary:
        //     Called when the View should be bound to the represented Item.
        //
        // Parameters:
        //   holder:
        //     The Android.Support.V7.Widget.RecyclerView.ViewHolder for this item.
        //
        //   position:
        //     The position of the item in the data source.
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (BindViewHolderDelegate == null)
            {
                throw new InvalidOperationException("OnBindViewHolder was called but no BindViewHolderDelegate was found");
            }

            BindViewHolderDelegate((THolder)holder, _dataSource[position], position);
        }

        //
        // Summary:
        //     Called when the View should be created.
        //
        // Parameters:
        //   parent:
        //     The parent for the view.
        //
        //   viewType:
        //     The resource ID (unused).
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (CellLayoutId != 0)
            {
                Type typeFromHandle = typeof(THolder);
                ConstructorInfo constructor = typeFromHandle.GetConstructor(new Type[1] { typeof(View) });
                if (constructor == null)
                {
                    throw new InvalidOperationException("No suitable constructor find for " + typeFromHandle.FullName);
                }

                View view = LayoutInflater.From(parent.Context)!.Inflate(CellLayoutId, parent, attachToRoot: false);
                RecyclerView.ViewHolder obj = constructor.Invoke(new object[1] { view }) as RecyclerView.ViewHolder;
                CachingViewHolder cachingViewHolder = obj as CachingViewHolder;
                if (cachingViewHolder != null)
                {
                    cachingViewHolder.ClickCallback = OnItemClick;
                }

                return obj;
            }

            if (CreateViewHolderDelegate == null)
            {
                throw new InvalidOperationException("OnCreateViewHolder was called but no CreateViewHolderDelegate was found");
            }

            return CreateViewHolderDelegate(parent, viewType);
        }

        //
        // Summary:
        //     Called when an item is clicked (or tapped) in the list.
        //
        // Parameters:
        //   newPosition:
        //     The position of the clicked item.
        //
        //   newView:
        //     The view representing the clicked item.
        public void OnItemClick(int newPosition, View newView)
        {
            if (ClickCallback != null)
            {
                ClickCallback(_oldPosition, _oldView, newPosition, newView);
                _oldPosition = newPosition;
                _oldView = newView;
            }

            if (_dataSource != null && _dataSource.Count >= newPosition)
            {
                SelectedItem = _dataSource[newPosition];
            }
        }

        //
        // Summary:
        //     Raises the GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2.PropertyChanged
        //     event.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property that changed.
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ((Action)delegate
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            int count2 = e.NewItems.Count;
                            for (int j = 0; j < count2; j++)
                            {
                                NotifyItemInserted(e.NewStartingIndex + j);
                            }

                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            int count = e.OldItems.Count;
                            Console.WriteLine($"Count: {count}");
                            for (int i = 0; i < count; i++)
                            {
                                NotifyItemRemoved(e.OldStartingIndex + i);
                                object objB = e.OldItems[i];
                                if (object.Equals(SelectedItem, objB))
                                {
                                    SelectedItem = default(TItem);
                                }
                            }

                            break;
                        }
                    default:
                        NotifyDataSetChanged();
                        break;
                }
            })();
        }

        private void RaiseSelectionChanged()
        {
            this.SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
