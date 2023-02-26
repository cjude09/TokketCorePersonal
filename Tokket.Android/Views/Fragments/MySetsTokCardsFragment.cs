using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Fragments
{
    public class MySetsTokCardsFragment : AndroidX.Fragment.App.Fragment
    {
        List<Tokmoji> ListTokmojiModel;
        GridLayoutManager mLayoutManager; RecyclerView recyclerView; SwipeRefreshLayout refreshLayout = null;
        TokCardDataAdapter tokDataAdapter; string userid; Set itemSet;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.mysets_tokcards, container, false);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            userid = prefs.GetString("userid", "");
            itemSet = JsonConvert.DeserializeObject<Set>(prefs.GetString("setList", ""));

            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            recyclerView = v.FindViewById<RecyclerView>(Resource.Id.mysettokcardsRecyclerView);
            recyclerView.SetLayoutManager(mLayoutManager);

            ((Activity)Context).RunOnUiThread(async () => await InitializeData());

            refreshLayout = v.FindViewById<SwipeRefreshLayout>(Resource.Id.mysettokcardsSwipeRefresh);

#if (_TOKKEPEDIA)
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
#endif

#if (_CLASSTOKS)
            refreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
#endif

            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //await GetSearchData();
                };

                recyclerView.AddOnScrollListener(onScrollListener);

                recyclerView.SetLayoutManager(mLayoutManager);
            }
            return v;
        }
        public async Task InitializeData(FilterType type = FilterType.None)
        {
            //Get Tokmoji
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();

            recyclerView.SetAdapter(null);
            var result = await GetSetToks("",100,0,false,true);
            tokDataAdapter = new TokCardDataAdapter(result, ListTokmojiModel);
            tokDataAdapter.ItemClick += tokDataAdapter.OnItemBackgroundClick;
            recyclerView.SetAdapter(tokDataAdapter);
        }
        public async Task<List<TokModel>> GetSetToks(string setId, int takeCnt = 100, int skip = 0, bool isListing = false, bool isCard = false)
        {
            var list = new List<TokModel>();
            foreach (var tok in itemSet.TokIds)
            {
                var tokRes = await TokService.Instance.GetTokIdAsync(tok);
                if (tokRes != null)
                    list.Add(tokRes);
            }
            return list;
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.RunWorkerAsync();
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            ((Activity)Context).RunOnUiThread(async () => await InitializeData(type));
            Thread.Sleep(1000);
        }
    }
}