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
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Adapters
{
    public class AdapterStatePagerFragment : FragmentStatePagerAdapter
    {
        List<Fragment> fragments;
        List<string> fragmentTitles;

        public AdapterStatePagerFragment(FragmentManager fm, List<Fragment> _fragments, List<string> _fragmentTitles) : base(fm)
        {
            fragments = _fragments;
            fragmentTitles = _fragmentTitles;
        }

        public void AddFragment(Fragment fragment, System.String title)
        {
            fragments.Add(fragment);
            fragmentTitles.Add(title);
        }


        public void InsertFragmentAtPosition(Fragment fragment, int position)
        {
            fragments.Insert(position, fragment);
        }

        public void RemoveFragment(Fragment fragment)
        {
            fragments.Remove(fragment);
        }

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override int Count
        {
            get { return fragments.Count; }
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(fragmentTitles[position]);
        }

        public override int GetItemPosition(Java.Lang.Object item)
        {
            int idx = fragments.IndexOf((Fragment)item);
            return idx < 0 ? PositionNone : idx;
        }


    }
}