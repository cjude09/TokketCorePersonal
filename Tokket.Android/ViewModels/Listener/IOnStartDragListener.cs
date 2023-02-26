using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Android.Listener
{
    public interface IOnStartDragListener
    {
        void OnStartDrag(RecyclerView.ViewHolder viewHolder);
    }
}