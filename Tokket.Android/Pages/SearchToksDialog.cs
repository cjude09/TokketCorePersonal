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
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using AndroidX.Core.Content;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "Search Toks", Theme = "@style/Theme.AppCompat.Light.Dialog.NoTitle")]
    public class SearchToksDialog : BaseActivity
    {
        SearchTokDataAdapter searchTokDataAdapter;
        GridLayoutManager mLayoutManager;
        List<ClassTokModel> ClassTokCollection;
        internal static SearchToksDialog Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dialog_search_toks);

            Instance = this;
            ClassTokCollection = new List<ClassTokModel>();
            int numcol = 1;
            /*if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }*/

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            recyclerView.SetLayoutManager(mLayoutManager);

            RunOnUiThread(async () => await InitializeData());

            swiperefresh_ListToks.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            swiperefresh_ListToks.Refresh -= RefreshLayout_Refresh;
            swiperefresh_ListToks.Refresh += RefreshLayout_Refresh;

            btnSearch.Click += async(sender, e) =>
            {
                if (string.IsNullOrEmpty(txtSearchToks.Text))
                {
                    var builder = new AlertDialog.Builder(this)
                            .SetMessage("Nothing searched!")
                            .SetPositiveButton("Ok", (_, args) =>
                            {

                            })
                            .SetCancelable(false)
                            .Show();
                }
                else
                {
                    await InitializeData();
                }
            };

            btnCancel.Click += delegate
            {
                this.Finish();
            };

            btnAddTokLink.Click += delegate
            {
                //btnAddTokLink.ContentDescription value is passed from SearchTokDataAdapter
                Intent intent = new Intent();
                intent.PutExtra("classtokModel", btnAddTokLink.ContentDescription);
                SetResult(Result.Ok, intent);
                this.Finish();
            };

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    await LoadMoreData();
                };

                recyclerView.AddOnScrollListener(onScrollListener);

                recyclerView.SetLayoutManager(mLayoutManager);
            }
        }
        private void showBottomLoading()
        {
            progressbar.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)), PorterDuff.Mode.Multiply);
            progressbar.Visibility = ViewStates.Visible;
        }
        private void hideBottomLoading()
        {
            progressbar.Visibility = ViewStates.Gone;
        }
        public async Task InitializeData()
        {
            ClassTokCollection = new List<ClassTokModel>();
            ClassTokCollection.Clear();
            recyclerView.SetAdapter(null);

            shimmer_view_container.StartShimmerAnimation();
            shimmer_view_container.Visibility = ViewStates.Visible;

            var resultToksData = await GetClassToksData();

            if (resultToksData != null)
            {
                foreach (var item in resultToksData)
                {
                    ClassTokCollection.Add(item);
                }
            }
            SetClassTokRecyclerAdapter();
            shimmer_view_container.Visibility = ViewStates.Gone;
        }
        public async Task<List<ClassTokModel>> GetClassToksData()
        {
            bool isPublicFeed = false;
            if (Settings.FilterFeed == (int)FilterType.All)
            {
                isPublicFeed = true;
            }
            var fromGroup = Intent.GetBooleanExtra("fromGroupModel", false);
            var queryValues = new ClassTokQueryValues();
            if (!fromGroup)
            {
                queryValues = new ClassTokQueryValues()
                {
                    partitionkeybase = $"{Settings.GetUserModel().UserId}-classtoks",
                    groupid = "",
                    userid = isPublicFeed ? "" : Settings.GetUserModel().UserId,
                    text = "",
                    startswith = false,
                    publicfeed = isPublicFeed,
                    FilterBy = FilterBy.None,
                    FilterItems = null,
                    searchvalue = txtSearchToks.Text
                };
            }
            else {
                queryValues = ClassGroupActivity.Instance.CurrentClassToksQuery;
            }
           

            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();
            tokResult = await ClassService.Instance.GetClassToksAsync(new GetClassToksRequest() { 
                QueryValues = queryValues
            });

            recyclerView.ContentDescription = tokResult.ContinuationToken;

            return tokResult.Results.ToList();
        }
        private void SetClassTokRecyclerAdapter(List<ClassTokModel> loadMoreItems = null)
        {
            if (loadMoreItems == null)
            {
                searchTokDataAdapter = new SearchTokDataAdapter(ClassTokCollection);
                recyclerView.SetAdapter(searchTokDataAdapter);
            }
            else
            {
                searchTokDataAdapter.UpdateItems(loadMoreItems, recyclerView.ChildCount);
                recyclerView.SetAdapter(searchTokDataAdapter);
                recyclerView.ScrollToPosition(searchTokDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }
        public async Task LoadMoreData()
        {
            if (!string.IsNullOrEmpty(recyclerView.ContentDescription))
            {
                var tokQueryModel = new ClassTokQueryValues();
                tokQueryModel.paginationid = recyclerView.ContentDescription;
                //tokQueryModel.loadmore = "yes";
                tokQueryModel.partitionkeybase = "classtoks";
                tokQueryModel.text = ""; // filter;
                tokQueryModel.startswith = false;
                recyclerView.ContentDescription = "";

                showBottomLoading();
                var result = await ClassService.Instance.GetClassToksAsync(new GetClassToksRequest() { 
                    QueryValues = tokQueryModel
                });
                var resultList = result.Results.ToList();
                hideBottomLoading();

                recyclerView.ContentDescription = result.ContinuationToken;
                foreach (var item in resultList)
                {
                    ClassTokCollection.Add(item);
                }

                SetClassTokRecyclerAdapter(resultList);
            }
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swiperefresh_ListToks.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            RunOnUiThread(async () => await InitializeData());
            Thread.Sleep(1000);
        }
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbar);
        private EditText txtSearchToks => FindViewById<EditText>(Resource.Id.txtSearchToks);
        private Button btnSearch => FindViewById<Button>(Resource.Id.btnSearch);
        public Button btnAddTokLink => FindViewById<Button>(Resource.Id.btnAddTokLink);
        private Button btnCancel => FindViewById<Button>(Resource.Id.btnCancel);
        private SwipeRefreshLayout swiperefresh_ListToks => FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh_ListToks);
        private RecyclerView recyclerView => FindViewById<RecyclerView>(Resource.Id.recyclerView);
        private ShimmerLayout shimmer_view_container => FindViewById<ShimmerLayout>(Resource.Id.shimmer_view_container);
    }
}