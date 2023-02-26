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
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Adapters
{
    public class AdapterStatePagerFragmentX : AndroidX.Fragment.App.FragmentStatePagerAdapter
    {
        List<XFragment> fragments;
        List<string> fragmentTitles;

        public AdapterStatePagerFragmentX(XFragmentManager fm, List<XFragment> _fragments, List<string> _fragmentTitles) : base(fm, 1) //1 = BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT
        {
            fragments = _fragments;
            fragmentTitles = _fragmentTitles;
        }

        public void AddFragment(XFragment fragment, System.String title)
        {
            fragments.Add(fragment);
            fragmentTitles.Add(title);
        }


        public void InsertFragmentAtPosition(XFragment fragment, int position)
        {
            fragments.Insert(position, fragment);
        }

        public void RemoveFragment(XFragment fragment)
        {
            fragments.Remove(fragment);
        }

        public override XFragment GetItem(int position)
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
            int idx = fragments.IndexOf((XFragment)item);
            return idx < 0 ? PositionNone : idx;
        }


    }
}