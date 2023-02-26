using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Adapters
{
    public class ViewPagerAdapter : FragmentStateAdapter
    {
        List<XFragment> fragments;
        List<string> fragmentTitles;
        public ViewPagerAdapter(XFragmentManager fm, List<XFragment> _fragments, List<string> _fragmentTitles, Lifecycle lifecylce) : base(fm, lifecylce)
        {
            fragments = _fragments;
            fragmentTitles = _fragmentTitles;
        }

        public override int ItemCount => fragments.Count();

        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
        {
            return fragments[position];
        }

        public void RefreshFragment(int index, XFragment fragment)
        {
            fragments[index] = fragment;
            NotifyItemChanged(index);
        }

        public override long GetItemId(int position)
        {
            return (long)fragments[position].GetHashCode();
        }

        public override bool ContainsItem(long itemId)
        {
            return fragments.Find(x => (long)x.GetHashCode() == itemId) != null;
        }
    }
}