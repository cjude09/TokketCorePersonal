using System;
using System.Collections.Generic;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Adapters
{
    public class AdapterStateFragmentX : AndroidX.Fragment.App.FragmentStatePagerAdapter
    {
        List<XFragment> fragments;
        List<string> fragmentTitles;

        public AdapterStateFragmentX(XFragmentManager fm, List<XFragment> _fragments, List<string> _fragmentTitles) : base(fm, 1) //1 = BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT
        {
            fragments = _fragments;
            fragmentTitles = _fragmentTitles;
        }

        #region implemented abstract members of PagerAdapter

        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }

        #endregion

        #region implemented abstract members of FragmentStatePagerAdapter

        public override XFragment GetItem(int position)
        {
            return fragments[position];
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(fragmentTitles[position]);
        }
        #endregion
    }
}