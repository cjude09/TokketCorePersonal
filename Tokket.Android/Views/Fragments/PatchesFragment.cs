using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using System.Threading.Tasks;
using Supercharge;
using Tokket.Shared.Models;
using Tokket.Core;

namespace Tokket.Android.Fragments
{
    public class PatchesFragment : AndroidX.Fragment.App.Fragment
    {
        View view; List<PointsSymbolModel> ListPatches, ListCurrentPatches;
        GridLayoutManager mLayoutManager;
        private string titlepage = ""; PatchesTab TabNum;
        TokketUser User;
        public PatchesFragment(string title, TokketUser currentUser)
        {
            titlepage = title;
            User = currentUser;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.patchesfragment_page, container, false);
            view = v;

            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerPatches.SetLayoutManager(mLayoutManager);

            LoadData();

            return v;
        }
        private void LoadData()
        {
            ShimmerPatches.StartShimmerAnimation();
            ShimmerPatches.Visibility = ViewStates.Visible;

            ListCurrentPatches = new List<PointsSymbolModel>();
            ListPatches = PointsSymbolsHelper.PointsSymbols;

            var currentLevel = PointsSymbolsHelper.GetPatchExactResult(User.Points);
            PointsSymbolModel nextLevel = PointsSymbolsHelper.PointsSymbols.FirstOrDefault(x => x.index == currentLevel.index + 1);
            if (titlepage.ToLower() == "my patches")
            {
                foreach (var item in ListPatches)
                {
                    if (nextLevel != null)
                    {
                        if (item.index < nextLevel.index)
                        {
                            ListCurrentPatches.Add(item);
                        }
                    }
                }
                TabNum = PatchesTab.MyPatches;
            }
            else
            {
                ListCurrentPatches = ListPatches;
                TabNum = PatchesTab.LevelTable;
            }
            
            var adapter = new PatchesAdapter(ListCurrentPatches, TabNum, nextLevel);
            RecyclerPatches.SetAdapter(adapter);
            ShimmerPatches.Visibility = ViewStates.Gone;
        }
        
        public RecyclerView RecyclerPatches => view.FindViewById<RecyclerView>(Resource.Id.RecyclerPatches);
        public ShimmerLayout ShimmerPatches => view.FindViewById<ShimmerLayout>(Resource.Id.ShimmerPatches);
    }
}