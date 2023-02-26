//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Util;
//using Android.Views;
//using Android.Widget;
//using Supercharge;
//using Tokket.Android.Adapters;
//using Tokket.Core;

//namespace Tokket.Android.Fragments
//{
//    class profilemytitle_fragment : AndroidX.Fragment.App.Fragment
//    {
//        View v; string titlepage = "";
//        List<TokketTitle> TokketTitleList;
//        public profilemytitle_fragment(string title, List<TokketTitle> ListTitles)
//        {
//            titlepage = title;
//            TokketTitleList = ListTitles;
//        }
//        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            v = inflater.Inflate(Resource.Layout.profilesmytitlefragment_page, container, false);
           
//            if (TokketTitleList.Count == 0)
//            {
//                if (titlepage.ToLower() == "unique title")
//                {
//                    TextNoResult.Text = "No Unique Title";
//                }
//                else if (titlepage.ToLower() == "generic title")
//                {
//                    TextNoResult.Text = "No Generic Title";
//                }
//                else if (titlepage.ToLower() == "royal title")
//                {
//                    TextNoResult.Text = "No Royal Title";
//                }
//            }
//            else
//            {
//                TextNoResult.Visibility = ViewStates.Gone;
//            }

//            var mLayoutManager = new LinearLayoutManager(MainActivity.Instance);
//            RecyclerTitles.SetLayoutManager(mLayoutManager);

//            var TitleAdapter = new ProfileTitleAdapter(TokketTitleList);
//            RecyclerTitles.SetAdapter(TitleAdapter);
//            return v;
//        }
//        public TextView TextNoResult => v.FindViewById<TextView>(Resource.Id.TextNoResult);
//        public RecyclerView RecyclerTitles => v.FindViewById<RecyclerView>(Resource.Id.RecyclerTitles);

//        public RecyclerView GenericTitles => v.FindViewById<RecyclerView>(Resource.Id.GenericTitles);

//        public RecyclerView UniqueTitles => v.FindViewById<RecyclerView>(Resource.Id.RoyaltyTitles);

//    }
//}