using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Core;

namespace Tokket.Android.Fragments
{
    public class ProfileTitleFragment : AndroidX.Fragment.App.Fragment
    {
        View v; string titlepage = "";
        List<TokketTitle> TokketTitleList;
        public ProfileTitleFragment(string title, List<TokketTitle> ListTitles)
        {
            titlepage = title;
            TokketTitleList = ListTitles;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.profiletitlesfragment_page, container, false);


            if (titlepage.ToLower() == "my title")
            {
                BuyButton.Visibility = ViewStates.Gone;
            }
            else {
                BuyButton.Visibility = ViewStates.Visible;
            }

            if (TokketTitleList.Count == 0)
            {
                if (titlepage.ToLower() == "unique title")
                {
                    TextNoResult.Text = "No Unique Title";
                }
                else if (titlepage.ToLower() == "generic title")
                {
                    TextNoResult.Text = "No Generic Title";
                }
                else if (titlepage.ToLower() == "royal title")
                {
                    TextNoResult.Text = "No Royal Title";
                }
            }
            else
            {
                TextNoResult.Visibility = ViewStates.Gone;
            }

            var mLayoutManager = new LinearLayoutManager(MainActivity.Instance);
            RecyclerTitles.SetLayoutManager(mLayoutManager);

            var TitleAdapter = new ProfileTitleAdapter(TokketTitleList);
            RecyclerTitles.SetAdapter(TitleAdapter);

            BuyButton.Click += (obj,e) => {
                if (titlepage.ToLower() == "unique title")
                {
                    Intent nextActivity = new Intent(this.Context, typeof(ProfileUniqueTitleBuyActivity));
                    this.StartActivity(nextActivity);
                }
                else if (titlepage.ToLower() == "generic title")
                {
                    Intent nextActivity = new Intent(this.Context, typeof(ProfileGenericTitleBuyActivity));
                    this.StartActivity(nextActivity);
                }
                else if (titlepage.ToLower() == "royal title")
                {
                    Intent nextActivity = new Intent(this.Context, typeof(ProfileRoyaltyTitleBuyActivity));
                    this.StartActivity(nextActivity);
                }
                else if (titlepage.ToLower() == "my title")
                {

                }
            };
            return v;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == -1 && data != null) {
                var datatitle = data.GetStringExtra("AddedTitle");
                //TokketTitleList.Add(datatitle);
            }
        }

        public TextView TextNoResult => v.FindViewById<TextView>(Resource.Id.TextNoResult);
        public RecyclerView RecyclerTitles => v.FindViewById<RecyclerView>(Resource.Id.RecyclerTitles);

        public Button BuyButton => v.FindViewById<Button>(Resource.Id.buyButton);
    }
}