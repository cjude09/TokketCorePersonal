using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Android.Helpers
{
    //
    // Summary:
    //     An extension of RecyclerView.ViewHolder optimized for
    //     usage with the GalaSoft.MvvmLight.Helpers.ObservableRecyclerAdapter`2. Provides
    //     additional features that can be used with the MVVM Light data binding system.
    public class CachingViewHolder : RecyclerView.ViewHolder
    {
        private readonly Dictionary<object, Binding> _bindings = new Dictionary<object, Binding>();

        private readonly Dictionary<int, View> _cachedViews = new Dictionary<int, View>();

        private Action<int, View> _clickCallback;

        //
        // Summary:
        //     A callback that will be executed when the corresponding item is clicked or tapped
        //     by the user.
        public Action<int, View> ClickCallback
        {
            get
            {
                return _clickCallback;
            }
            set
            {
                if (value == null)
                {
                    base.ItemView.Click -= new EventHandler(OnViewClick);
                }
                else
                {
                    base.ItemView.Click += new EventHandler(OnViewClick);
                }

                _clickCallback = value;
            }
        }

        //
        // Summary:
        //     Initializes an instance of this class. In most cases this method is not used
        //     by the developer.
        //
        // Parameters:
        //   javaReference:
        //
        //   transfer:
        public CachingViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        //
        // Summary:
        //     Initializes an instance of this class.
        //
        // Parameters:
        //   itemView:
        //     The view that this holder is attached to.
        public CachingViewHolder(View itemView)
            : base(itemView)
        {
        }

        //
        // Summary:
        //     Detaches and removes a binding from this holder. Use this method (in case a view
        //     is getting recycled) to clean up existing bindings before attaching new ones.
        //     The binding that needs to be detached must have been added to the holder using
        //     the GalaSoft.MvvmLight.Helpers.CachingViewHolder.SaveBinding(System.Object,GalaSoft.MvvmLight.Helpers.Binding)
        //     method.
        //
        // Parameters:
        //   key:
        //     The key corresponding to the binding to detach and delete. This is the same key
        //     that was used in the GalaSoft.MvvmLight.Helpers.CachingViewHolder.SaveBinding(System.Object,GalaSoft.MvvmLight.Helpers.Binding)
        //     method.
        //
        // Returns:
        //     The binding that has been detached and deleted, in case further processing is
        //     necessary/
        public Binding DeleteBinding(object key)
        {
            lock (_bindings)
            {
                if (_bindings.ContainsKey(key))
                {
                    Binding binding = _bindings[key];
                    binding.Detach();
                    _bindings.Remove(key);
                    return binding;
                }

                return null;
            }
        }

        //
        // Summary:
        //     Explores the attached view and returns the UI element corresponding to the viewId.
        //     If no element is found with this ID, the method returns null.
        //
        // Parameters:
        //   viewId:
        //     The ID of the subview that needs to be retrieved.
        //
        // Type parameters:
        //   TView:
        //     The type of the view to be returned.
        //
        // Returns:
        //     The sub view corresponding to the viewId, or null if no corresponding sub view
        //     is found.
        public TView FindCachedViewById<TView>(int viewId) where TView : View
        {
            if (_cachedViews.ContainsKey(viewId))
            {
                return (TView)_cachedViews[viewId];
            }

            TView val = base.ItemView.FindViewById<TView>(viewId);
            _cachedViews.Add(viewId, val);
            return val;
        }

        //
        // Summary:
        //     Saves a binding between a element of the view and an element of the data item
        //     represented by the view. If the view is getting recycled, the binding should
        //     be deleted using the GalaSoft.MvvmLight.Helpers.CachingViewHolder.DeleteBinding(System.Object)
        //     method.
        //
        // Parameters:
        //   key:
        //     The key used to retriev the binding later. Typically the key is the view to which
        //     the binding is attached (if there is only one binding for this view).
        //
        //   binding:
        //     The binding to be saved.
        public void SaveBinding(object key, Binding binding)
        {
            lock (_bindings)
            {
                if (_bindings.ContainsKey(key))
                {
                    _bindings[key].Detach();
                    _bindings[key] = binding;
                }
                else
                {
                    _bindings.Add(key, binding);
                }
            }
        }

        private void OnViewClick(object sender, EventArgs e)
        {
            int adapterPosition = base.AdapterPosition;
            if (adapterPosition != -1)
            {
                _clickCallback?.Invoke(adapterPosition, base.ItemView);
            }
        }
    }
}
