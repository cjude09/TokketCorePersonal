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
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Fragments
{
    public class ToksFragment : AndroidX.Fragment.App.Fragment
    {
        View page = null; List<Tokmoji> ListTokmojiModel;
        GridLayoutManager mLayoutManager; RecyclerView recyclerView; SwipeRefreshLayout refreshLayout = null;
        ShimmerLayout shimmerLayout = null; TokDataAdapter tokDataAdapter; List<Shared.Models.TokModel> tokResult, TokDataList;
        string userid; Set itemSet; TokketUser tokketUser; TokModel tokModel;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.toks_page, container, false);
            page = v;

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            userid = prefs.GetString("userid", "");
            itemSet = JsonConvert.DeserializeObject<Set>(prefs.GetString("setList",""));
            
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            tokModel = new TokModel();
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            recyclerView = v.FindViewById<RecyclerView>(Resource.Id.toks_recyclerView);
            recyclerView.SetLayoutManager(mLayoutManager);
            //ViewCompat.SetNestedScrollingEnabled(recyclerView, false);

            ((Activity)Context).RunOnUiThread(async () => await InitializeData());

            refreshLayout = v.FindViewById<SwipeRefreshLayout>(Resource.Id.toks_swiperefresh_ListToks);
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    await GetSearchData();
                };

                recyclerView.AddOnScrollListener(onScrollListener);

                recyclerView.SetLayoutManager(mLayoutManager);
            }

            return v;
        }
        public async Task GetSearchData()
        {
            if (itemSet == null)
            {
                TokQueryValues tokQueryModel = new TokQueryValues() { userid = userid, streamtoken = null, sortby = Settings.SortByFilter };
                tokQueryModel.token = Settings.ContinuationToken;
                tokQueryModel.loadmore = "yes";
                var result = await TokService.Instance.GetAllToks(tokQueryModel);
                tokDataAdapter.UpdateItems(result);
                TokDataList.AddRange(result);
            }
            
        }
        public async Task InitializeData(FilterType type = FilterType.None)
        {
            TokDataList = new List<TokModel>();
            recyclerView.SetAdapter(null);
            shimmerLayout = page.FindViewById<ShimmerLayout>(Resource.Id.toks_shimmer_view_container);
            shimmerLayout.StartShimmerAnimation();
            shimmerLayout.Visibility = ViewStates.Visible;

            //get tokmojis
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();

            tokketUser = await AccountService.Instance.GetUserAsync(userid);
            var result = await GetToksData();
            TokDataList = result;
            SetRecyclerAdapter(result);
            shimmerLayout.Visibility = ViewStates.Gone;
        }
        private void SetRecyclerAdapter(List<TokModel> TokDataRes)
        {
            tokDataAdapter = new TokDataAdapter(TokDataRes, ListTokmojiModel);
            //tokDataAdapter.ItemClick += tokDataAdapter.OnGridBackgroundClick;
            tokDataAdapter.ItemClick -= OnGridBackgroundClick;
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            recyclerView.SetAdapter(tokDataAdapter);
        }
        public async Task<List<Shared.Models.TokModel>> GetToksData()
        {
            FilterType type = (FilterType)Settings.FilterTag;
            tokResult = new List<Shared.Models.TokModel>();
            string strtoken = tokketUser.StreamToken;
            switch (type)
            {
                case FilterType.User:
                    tokResult = await TokService.Instance.GetAllToks(new TokQueryValues() { userid = userid, streamtoken = strtoken, sortby = Settings.SortByFilter });
                    break;
                case FilterType.Set:
                    var list = new List<TokModel>();
                    //foreach (var tok in itemSet.TokIds)
                    //{
                    //    var tokRes = await TokService.Instance.GetTokIdAsync(tok);
                    //    if (tokRes != null)
                    //        list.Add(tokRes);
                    //}
                    //tokResult = list;
                    var tokRes = await TokService.Instance.GetToksByIdsAsync(itemSet.TokIds.ToList());
                    if (tokRes != null)
                    {
                        list = tokRes;
                    }
                    tokResult = list;
                    break;
            }
            
            var toksWithSticker = new List<TokModel>();
            tokResult = tokResult.OrderByDescending(x => x.DateCreated.Value).ToList() ?? new List<TokModel>();
            var cnt = 0;
            foreach (var tok in tokResult)
            {
                var sticker = StickersTool.Stickers.FirstOrDefault(x => x.Id == (string.IsNullOrEmpty(tok.Sticker) ? tok.Sticker : tok.Sticker.Split("-")[0]));
                tok.StickerImage = sticker?.Image ?? string.Empty;
                tok.IndexCounter = cnt;
                toksWithSticker.Add(tok);
                cnt += 1;
            }
            toksWithSticker = toksWithSticker.ToList();
            return tokResult;
        }
        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
            tokModel = TokDataList[position];
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            nextActivity.PutExtra("tokModel", modelConvert);
            StartActivityForResult(nextActivity, (int)ActivityType.ToksFragment);
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.ToksFragment) && (resultCode == -1))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    TokDataList.Remove(tokModel);
                    SetRecyclerAdapter(TokDataList);
                }
            }
            else if ((requestCode == (int)ActivityType.AddStickerDialogActivity) && (resultCode == -1))
            {
                var dataTokModelstr = data.GetStringExtra("tokModel");
                if (dataTokModelstr != null)
                {
                    var dataTokModel = JsonConvert.DeserializeObject<TokModel>(dataTokModelstr);
                    if (dataTokModel != null)
                    {
                        var result = TokDataList.FirstOrDefault(c => c.Id == dataTokModel.Id);
                        if (result != null) //If Edit
                        {
                            int ndx = TokDataList.IndexOf(result);
                            TokDataList.Remove(result);

                            TokDataList.Insert(ndx, dataTokModel);
                            SetRecyclerAdapter(TokDataList);
                        }
                    }
                }
            }
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