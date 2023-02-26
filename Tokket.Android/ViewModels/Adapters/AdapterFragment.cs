using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Helpers;
using Tokket.Android.Fragments;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace Tokket.Android.Adapters
{
    public class AdapterFragment : FragmentPagerAdapter
    {
        List<Fragment> fragments;
        List<string> fragmentTitles;

        public AdapterFragment(FragmentManager fm, List<Fragment> _fragments, List<string> _fragmentTitles) : base(fm)
        {
            fragments = _fragments;
            fragmentTitles = _fragmentTitles;
        }

        public void AddFragment(Fragment fragment, String title)
        {
            fragments.Add(fragment);
            fragmentTitles.Add(title);
        }

        public override Fragment GetItem(int position)
        {
            //if (Settings.ActivityInt == Convert.ToInt16(ActivityType.ReactionValuesActivity))
            //{
            //    switch (position)
            //    {
            //        case 0: // Fragment # 0 - This will show FirstFragment
            //            return reactionvalues_users_fragment.newInstance("All");
            //        case 1: // Fragment # 0 - This will show FirstFragment different title
            //            return reactionvalues_users_fragment.newInstance("GemA");
            //        case 2: // Fragment # 1 - This will show SecondFragment
            //            return reactionvalues_users_fragment.newInstance("GemB");
            //        case 3:
            //            return reactionvalues_users_fragment.newInstance("GemC");
            //        case 4:
            //            return reactionvalues_users_fragment.newInstance("Accurate");
            //        case 5:
            //            return reactionvalues_users_fragment.newInstance("Inaccurate");
            //        default:
            //            return null;
            //    }
            //}
            //else
            //{
                return fragments[position];
            //}
        }

        public override int Count
        {
            get { return fragments.Count; }
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(fragmentTitles[position]);
        }
    }
}