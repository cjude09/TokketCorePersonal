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
//    class profileroyaltytitle_fragments : AndroidX.Fragment.App.Fragment
//    {
//        View v; string titlepage = "";
//        List<TokketTitle> TokketTitleList;

//        string[] RoyaltySelection = { "Select a royalty:", "King", "Queen","Prince","Princess", "Duke of", "Duchess of" };
//        string[] SeparatorsSelection = { "Select a separator:","Space","Underscore","Dash" };
//        public profileroyaltytitle_fragments(string title, List<TokketTitle> ListTitles)
//        {
//            titlepage = title;
//            TokketTitleList = ListTitles;
//        }
//        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            v = inflater.Inflate(Resource.Layout.profileroyaltytitlefragment_page, container, false);
//            ArrayAdapter<string> royatyadapter = new ArrayAdapter<string>(this.Context, Resource.Layout.support_simple_spinner_dropdown_item, RoyaltySelection);
//            royatyadapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

//            ArrayAdapter<string> separatorAdapter = new ArrayAdapter<string>(this.Context, Resource.Layout.support_simple_spinner_dropdown_item, SeparatorsSelection);
//            separatorAdapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

//            RoyalySpinner.Adapter = royatyadapter;
//            SeparatorSpinner.Adapter = separatorAdapter;

           
//            return v;
//        }


//        public Spinner RoyalySpinner => v.FindViewById<Spinner>(Resource.Id.RoyaltySpinner);

//        public Spinner SeparatorSpinner => v.FindViewById<Spinner>(Resource.Id.SeparatorSpinner);
//    }
//}