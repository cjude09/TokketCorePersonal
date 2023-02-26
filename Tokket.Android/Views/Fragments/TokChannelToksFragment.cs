using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Core;
using Tokket.Core.Tools;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Xamarin.Essentials;

namespace Tokket.Android.Views.Fragments
{
    public class TokChannelToksFragment : AndroidX.Fragment.App.Fragment
    {
        internal static TokChannelToksFragment Instance { get; private set; }
        View page;
        public List<ClassTokModel> ClassTokCollection; List<Tokmoji> ListTokmojiModel;
        ClassTokModel classtokModel;
        ClassTokDataAdapter ClassTokDataAdapter;
        string level1, level2, level3;
        public TokChannelToksFragment(string l1, string l2, string l3)
        {
            level1 = l1;
            level2 = l2;
            level3 = l3;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (page == null)
                page = inflater.Inflate(Resource.Layout.fragment_tok_channel_toks, container, false);

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            ClassTokCollection = new List<ClassTokModel>();

            var mLayoutManager = new GridLayoutManager(page.Context, numcol);
            RecyclerToks.SetLayoutManager(mLayoutManager);

            swipeRefreshContainer.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            swipeRefreshContainer.Refresh -= RefreshLayout_Refresh;
            swipeRefreshContainer.Refresh += RefreshLayout_Refresh;

            MainActivity.Instance.RunOnUiThread(async () => await LoadToks(level1, level2, level3));

            return page;
        }

        public async Task LoadToks(string level1, string level2, string level3, bool isSearch = false, string searchText = "")
        {
            RecyclerToks.SetAdapter(null);
            ClassTokCollection.Clear(); //TODO should not clear if there's a pagination
            shimmerContainer.StartShimmerAnimation();
            shimmerContainer.Visibility = ViewStates.Visible;
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            if (tokmojiResult != null)
                ListTokmojiModel = tokmojiResult.Results.ToList();

            var queryValues = new ClassTokQueryValues()
            {
                partitionkeybase = $"{Settings.GetUserModel().UserId}-classtoks",
                groupid = "",
                userid = true ? "" : Settings.GetUserModel().UserId,
                text = searchText,
                startswith = false,
                publicfeed = true,
                FilterBy = FilterBy.None,
                FilterItems = new List<string>(),
                searchvalue = null,
                level1 = level1,
                level2 = level2,
                level3 = level3
            };

            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();
            tokResult = await ClassService.Instance.GetClassToksAsync(new GetClassToksRequest()
            {
                QueryValues = queryValues
            });

            var tokResultList = tokResult.Results.ToList();
            ClassTokCollection.AddRange(tokResultList);
            RecyclerToks.ContentDescription = tokResult.ContinuationToken;
            SetClassTokRecyclerAdapter();

            if (isSearch)
            {
                txtSearchResult.Visibility = ViewStates.Visible;
                txtSearchResult.Text = "Search results for \"" + searchText + "\": " + tokResultList.Count();
            }

            shimmerContainer.Visibility = ViewStates.Gone;
        }

        public void HideSearchResult()
        {
            txtSearchResult.Visibility = ViewStates.Gone;
            txtSearchResult.Text = "";
        }

        public void SetClassTokRecyclerAdapter(List<ClassTokModel> loadMoreItems = null)
        {
            if (loadMoreItems == null)
            {
                ClassTokDataAdapter = new ClassTokDataAdapter(page.Context, ClassTokCollection, ListTokmojiModel);
                ClassTokDataAdapter.ItemClick -= OnGridBackgroundClick;
                ClassTokDataAdapter.ItemClick += OnGridBackgroundClick;
                RecyclerToks.SetAdapter(ClassTokDataAdapter);
            }
            else
            {
                ClassTokDataAdapter.UpdateItems(loadMoreItems, RecyclerToks.ChildCount);
                RecyclerToks.SetAdapter(ClassTokDataAdapter);
                RecyclerToks.ScrollToPosition(ClassTokDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }

        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;
            Intent nextActivity = new Intent(page.Context, typeof(TokInfoActivity));
            classtokModel = ClassTokCollection[position];
            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
        }

        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                /*TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Transparent);
                TextNothingFound.Visibility = ViewStates.Gone;*/

                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
            else
            {
                /*TextNothingFound.Text = "No Internet Connection!";
                TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Black);
                TextNothingFound.Visibility = ViewStates.Visible;*/
                swipeRefreshContainer.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipeRefreshContainer.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO
            /*RecyclerToks.RemoveAllViews();
            ClassTokCollection.Clear();
            this.RunOnUiThread(async () => await LoadToks());*/
            Thread.Sleep(1000);
        }

        public RecyclerView RecyclerToks => page.FindViewById<RecyclerView>(Resource.Id.recycyclerToks);
        public SwipeRefreshLayout swipeRefreshContainer => page.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshContainer);
        public ShimmerLayout shimmerContainer => page.FindViewById<ShimmerLayout>(Resource.Id.shimmerContainer);
        public AppCompatTextView txtSearchResult => page.FindViewById<AppCompatTextView>(Resource.Id.txtSearchResult);
    }
}
