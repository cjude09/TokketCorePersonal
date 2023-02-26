using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Xamarin.Essentials;
using Tokket.Shared.Helpers;
using System.Threading;
using Tokket.Shared.Models;
using System.Threading.Tasks;
using SharedService = Tokket.Shared.Services;
using Tokket.Core.Tools;
using Newtonsoft.Json;
using Tokket.Shared.Services;
using Tokket.Core;
using AndroidX.AppCompat.App;
using AndroidX.SwipeRefreshLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.Preference;
using Color = Android.Graphics.Color;
using Android.Content.PM;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ToksActivity : BaseActivity
    {
        List<Tokmoji> ListTokmojiModel;
        ShimmerLayout shimmerLayout = null;
        SwipeRefreshLayout refreshLayout = null;
        GridLayoutManager mLayoutManager; RecyclerView recyclerView;
        List<Shared.Models.TokModel> tokResult, TokDataList; TokModel tokModel;
        TokDataAdapter tokDataAdapter; TokCardDataAdapter tokCardAdapter;  FilterType type; string filter;
        internal static ToksActivity Instance { get; private set; }
        string strtoken;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.toks_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();

            Instance = this;

            type = (FilterType)Settings.FilterTag;
            Settings.FilterTag = (int)FilterType.All;//set back to default

            this.Title = Intent.GetStringExtra("titlepage");
            filter = Intent.GetStringExtra("filter");

            Settings.ContinuationToken = "";
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            tokModel = new TokModel();
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.toks_recyclerView);
            recyclerView.SetLayoutManager(mLayoutManager);

            SupportActionBar.Title = SupportActionBar.Title; //+ " - " + Intent.GetStringExtra("headerpage").ToUpper();
            SupportActionBar.Subtitle = Intent.GetStringExtra("headerpage").ToUpper();
            
            UserModel myAccount = JsonConvert.DeserializeObject<UserModel>(Settings.UserAccount);
            strtoken = myAccount.StreamToken;

            this.RunOnUiThread(async () => await InitializeData());

            refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.toks_swiperefresh_ListToks);
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;
                
                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(Settings.ContinuationToken))
                    {
                        await GetSearchData();
                    }
                };

                recyclerView.AddOnScrollListener(onScrollListener);

                recyclerView.SetLayoutManager(mLayoutManager);
            }
        }
        
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.btnFilter:
                    string SubTitle = SupportActionBar.Title + ": " + (SupportActionBar.Subtitle.Substring(0, 1) + SupportActionBar.Subtitle.ToLower().Substring(1, SupportActionBar.Subtitle.Length - 1));
                    Intent nextActivity = new Intent(this, typeof(FilterActivity));
                    nextActivity.PutExtra("activitycaller", "ToksActivity");
                    nextActivity.PutExtra("SubTitle", SubTitle);
                    //nextActivity.PutExtra("TokList", JsonConvert.SerializeObject(TokDataList));
                    editor.PutString("TokModelList", JsonConvert.SerializeObject(TokDataList));
                    editor.Apply();

                    StartActivityForResult(nextActivity, 20001);
                    break;
            }
            return base.OnOptionsItemSelected(item);
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
            this.RunOnUiThread(async () => await InitializeData());
            Thread.Sleep(1000);
        }
        public async Task GetSearchData()
        {
            TokQueryValues tokQueryModel = new TokQueryValues();
            switch (type)
            {
                case FilterType.TokType:
                    tokQueryModel.toktype = filter;
                    break;
                case FilterType.Category:
                    tokQueryModel.category = filter;
                    break;
                case FilterType.Group:
                    tokQueryModel.tokgroup = filter;
                    break;
                default:
                    break;
            }
            tokQueryModel.sortby = Settings.SortByFilter;
            tokQueryModel.token = Settings.ContinuationToken;
            tokQueryModel.loadmore = "yes";
            var result = await SharedService.TokService.Instance.GetAllToks(tokQueryModel);

            //if (Settings.FilterToks == (int)FilterToks.Toks)
            //{
                tokDataAdapter.UpdateItems(result);
            //}
            //else if (Settings.FilterToks == (int)FilterToks.Cards)
            //{
                //tokCardAdapter.UpdateItems(result);
            //}

            TokDataList.AddRange(result);
        }
        public async Task InitializeData()
        {
            TokDataList = new List<TokModel>();
            recyclerView.SetAdapter(null);
            shimmerLayout = FindViewById<ShimmerLayout>(Resource.Id.toks_shimmer_view_container);
            shimmerLayout.StartShimmerAnimation();
            shimmerLayout.Visibility = ViewStates.Visible;
            var result = await GetToksData();
            TokDataList = result;

            //if (Settings.FilterToks == (int)FilterToks.Toks)
            //{
                SetRecyclerAdapter(result);
            //}
            //else if (Settings.FilterToks == (int)FilterToks.Cards)
            //{
               // SetCardRecyclerAdapter(result);
            //}

            shimmerLayout.Visibility = ViewStates.Invisible;
        }
        private void SetRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokDataAdapter = new TokDataAdapter(tokModelRes, ListTokmojiModel);
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            recyclerView.SetAdapter(tokDataAdapter);
        }
        private void SetCardRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokCardAdapter = new TokCardDataAdapter(tokModelRes, ListTokmojiModel);
            recyclerView.SetAdapter(tokCardAdapter);
        }
        public async Task<List<Shared.Models.TokModel>> GetToksData()
        {
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();

            tokResult = new List<Shared.Models.TokModel>();
            switch (type)
            {
                case FilterType.TokType:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { toktype = filter, streamtoken = strtoken });
                    break;
                case FilterType.Category:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { category = filter, streamtoken = strtoken});
                   //if(tokResult != null)
                   //     tokResult = tokResult?.Where(tok => tok.CategoryId?.ToLower() == filter.ToLower()).ToList();
                    break;
                case FilterType.Group:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { tokgroup = filter, streamtoken = strtoken });
                    break;
                default:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken});
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
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivityForResult(nextActivity, 10001);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == 10001) && (resultCode == Result.Ok))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    TokDataList.Remove(tokModel);
                    SetRecyclerAdapter(TokDataList);
                }

                //var dataTokModelstr = data.GetStringExtra("tokModel");
                //if (dataTokModelstr != null)
                //{
                //    //TokDataList
                //    var dataTokModel = JsonConvert.DeserializeObject<TokModel>(dataTokModelstr);
                //    if (dataTokModel != null)
                //    {
                //        var result = TokDataList.FirstOrDefault(c => c.Id == dataTokModel.Id);
                //        if (result != null) 
                //        {
                //            int ndx = TokDataList.IndexOf(result);
                //            TokDataList.Remove(result);

                //            TokDataList.Insert(ndx, result);
                //            SetRecyclerAdapter(TokDataList);
                //        }
                //    }
                //}
            }
            else if ((requestCode == (int)ActivityType.AddStickerDialogActivity) && (resultCode == Result.Ok))
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
            else if (requestCode == 20001 && resultCode == Result.Ok) //From FilterActivity
            {
                this.RunOnUiThread(async () => await InitializeData());
            }
        }
    }
}