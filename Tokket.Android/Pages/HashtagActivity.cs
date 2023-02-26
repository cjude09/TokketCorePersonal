using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
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
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Color = Android.Graphics.Color;
using SharedService = Tokket.Shared.Services;
using Result = Android.App.Result;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
     [Activity(Label = "Tag", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Tag", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class HashtagActivity : BaseActivity
    {
        List<Tokmoji> ListTokmojiModel;
        GridLayoutManager mLayoutManager;
        List<TokModel> tokResult, TokDataList; TokModel tokModel;
        TokDataAdapter tokDataAdapter; TokCardDataAdapter tokCardAdapter; FilterType type; string filter;
        internal static HashtagActivity Instance { get; private set; }
        string strtoken, hashtag;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_hashtag);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;

            type = FilterType.Tag;
            Settings.FilterTag = (int)FilterType.All;//set back to default

            hashtag = Intent.GetStringExtra("hashtag");
            SupportActionBar.Subtitle = "Tag: " + hashtag;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerToks.SetLayoutManager(mLayoutManager);


            this.RunOnUiThread(async () => await InitializeData());

#if (_TOKKEPEDIA)
            RefreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
#endif

#if (_CLASSTOKS)
            RefreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
#endif
            RefreshLayout.Refresh += RefreshLayout_Refresh;

            if (RecyclerToks != null)
            {
                RecyclerToks.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(Settings.ContinuationToken))
                    {
                        //await GetSearchData();
                    }
                };

                RecyclerToks.AddOnScrollListener(onScrollListener);

                RecyclerToks.SetLayoutManager(mLayoutManager);
            }
        }

        public async Task InitializeData()
        {
            TokDataList = new List<TokModel>();
            RecyclerToks.SetAdapter(null);
            Shimmer.StartShimmerAnimation();
            Shimmer.Visibility = ViewStates.Visible;
            var result = await GetToksData();
            TokDataList = result;

            //if (Settings.FilterToks == (int)FilterToks.Toks)
            //{
                SetRecyclerAdapter(result);
            //}
            //else if (Settings.FilterToks == (int)FilterToks.Cards)
            //{
                //SetCardRecyclerAdapter(result);
            //}

            Shimmer.Visibility = ViewStates.Invisible;
        }

        private void SetRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokDataAdapter = new TokDataAdapter(tokModelRes, ListTokmojiModel);
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToks.SetAdapter(tokDataAdapter);
        }
        private void SetCardRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokCardAdapter = new TokCardDataAdapter(tokModelRes, ListTokmojiModel);
            RecyclerToks.SetAdapter(tokCardAdapter);
        }
        public async Task<List<Shared.Models.TokModel>> GetToksData()
        {
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();

            tokResult = new List<Shared.Models.TokModel>();
            switch (type)
            {
                case FilterType.Tag:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { tagid = hashtag.Replace("#", "tag-"), streamtoken = strtoken });
                    break;
                default:
                    tokResult = await SharedService.TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken });
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
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
            RefreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            this.RunOnUiThread(async () => await InitializeData());
            Thread.Sleep(1000);
        }

        public SwipeRefreshLayout RefreshLayout => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
        public RecyclerView RecyclerToks => FindViewById<RecyclerView>(Resource.Id.recyclerToks);
        public ShimmerLayout Shimmer => FindViewById<ShimmerLayout>(Resource.Id.shimmerLayout);
    }
}