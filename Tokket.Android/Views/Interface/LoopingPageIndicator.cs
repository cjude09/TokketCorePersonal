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
using Asksira.LoopingViewPagerLib;
using Java.Interop;

namespace Tokket.Android.Interface
{
    public interface LoopingPageIndicator : LoopingViewPager.IOnPageChangeListener, IJavaObject, IDisposable, IJavaPeerable
    {
        void NotifyDataSetChanged();
        void SetCurrentItem(int item);
        void SetOnPageChangeListener(LoopingViewPager.IOnPageChangeListener listener);
        void SetOrientation(int orientation);
        void SetViewPager(LoopingViewPager view);
        void SetViewPager(LoopingViewPager view, int initialPosition);
    }
}