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
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Supercharge;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Android.ViewModels;
using Tokket.Core;
using Tokket.Core.Tools;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Fragments
{
    public class MyTokSetsFragment : AndroidX.Fragment.App.Fragment
    {
        internal static MyTokSetsFragment Instance { get; private set; }
        View page;
        GridLayoutManager mLayoutManager; SwipeRefreshLayout refreshLayout = null;
        public MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.mysets_toksets_page, container, false);
            page = v;
            Instance = this;

            MySetsVm.RecyclerMainList = RecyclerMainList;
            MySetsVm.ShimmerLayout = ShimmerLayout;

            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerMainList.SetLayoutManager(mLayoutManager);
            
            ((Activity)Context).RunOnUiThread(async () => await MySetsVm.InitializeData());
            refreshLayout = v.FindViewById<SwipeRefreshLayout>(Resource.Id.mytoksets_swiperefresh);
#if (_TOKKEPEDIA)
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
#endif

#if (_CLASSTOKS)
            refreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
#endif
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (RecyclerMainList != null)
            {
                RecyclerMainList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //Load more stuff here
                    await MySetsVm.GetAllToks(MySetsVm.TokTypeID, Settings.ContinuationToken);
                };


                RecyclerMainList.AddOnScrollListener(onScrollListener);

                RecyclerMainList.SetLayoutManager(mLayoutManager);
            }

            return v;
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            //Data Refresh Place  
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
            ((Activity)Context).RunOnUiThread(async () => await MySetsVm.InitializeData());
            Thread.Sleep(1000);
        }
        public RecyclerView RecyclerMainList => page.FindViewById<RecyclerView>(Resource.Id.recyclerView_mytoksets);
        public ShimmerLayout ShimmerLayout => page.FindViewById<ShimmerLayout>(Resource.Id.mysets_shimmer_view_container);
    }
}