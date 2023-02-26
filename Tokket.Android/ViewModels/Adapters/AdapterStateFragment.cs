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
using Java.Lang;

namespace Tokket.Android.Adapters
{
    public class AdapterStateFragment : FragmentStatePagerAdapter
    {
        List<AndroidX.Fragment.App.Fragment> fragments;
        List<string> fragmentTitles;

        public AdapterStateFragment(AndroidX.Fragment.App.FragmentManager fm, List<AndroidX.Fragment.App.Fragment> _fragments, List<string> _fragmentTitles) : base(fm)
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

        public override AndroidX.Fragment.App.Fragment GetItem(int position)
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