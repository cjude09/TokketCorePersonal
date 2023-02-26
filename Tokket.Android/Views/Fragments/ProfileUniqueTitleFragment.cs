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
//    class profileuniquetitle_fragments : AndroidX.Fragment.App.Fragment
//    {
//        View v; string titlepage = "";
//        List<TokketTitle> TokketTitleList;
//        string[] smple = { "Title" };
//        public profileuniquetitle_fragments(string title, List<TokketTitle> ListTitles)
//        {
//            titlepage = title;
//            TokketTitleList = ListTitles;
//        }
//        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            v = inflater.Inflate(Resource.Layout.profileuniquetitlefragment_page, container, false);
//            ArrayAdapter<string> uniqueadapter = new ArrayAdapter<string>(this.Context, Resource.Layout.support_simple_spinner_dropdown_item, smple);
//            uniqueadapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);

//            SpinnerUnique.Adapter = uniqueadapter;
//            return v;
//        }

//        public Spinner SpinnerUnique => v.FindViewById<Spinner>(Resource.Id.SpinnerUnique);
      
//    }
//}