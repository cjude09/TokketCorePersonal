using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Tokket.Core;

namespace Tokket.Android
{
    [Activity(Label = "AlphaToksMain")]
    public class AlphaToksMain : BaseActivity
    {
        public static AlphaToksMain Instance { get; private set; }
        GridLayoutManager mLayoutManager;
        View v; Shared.Models.TokModel classtokModel;
        public AlphaTokDataAdapter AlphaTokDataAdapter, newAlphaTokDataAdapter;
        public List<Shared.Models.TokModel> ClassTokCollection, newClassTokCollection; List<Tokmoji> ListTokmojiModel;
        Tokket.Shared.Models.TokQueryValues Values = null;

        string groupId = "", classSetId = "";
        string ContinutationToken;
        int RemainingItemsThreshold;
        public Intent ParentIntent { get; set; }

     

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.alphatoks_mainpage);

            AbbrRefreshLayout.Refresh += OnRefreshTok;
            //            var tokback_toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.alphatoksmain_toolbar);
            //#if (_CLASSTOKS)
            //            tokback_toolbar.SetBackgroundResource(Resource.Color.colorAccent);
            //#endif
            //            SetSupportActionBar(tokback_toolbar);

            //            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //            SupportActionBar.SetDisplayShowTitleEnabled(false);
            ReturnhButton.Click += OnClickReturn;
            FilterButton.Click += OnClickFilter;
            SearchButton.Click += OnCLickSearch;
            AddEditButton.Click += OnClickShowAbbrPage;
            try {
                Values = JsonConvert.DeserializeObject<TokQueryValues>(Intent.GetStringExtra("TokQuery"));
            } catch (Exception ex) {
                Values = null;
            }
           
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            TaskRecyclerView.SetLayoutManager(mLayoutManager);
            if (TaskRecyclerView != null)
            {
                TaskRecyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {

                    await LoadMoreData();

                };

                TaskRecyclerView.AddOnScrollListener(onScrollListener);

                TaskRecyclerView.SetLayoutManager(mLayoutManager);
            }
            RunOnUiThread(async () => await InitializeData());
            SetClassTokRecyclerAdapter();
            Instance = this;
            

            // Create your application here
        }

        private void OnRefreshTok(object sender, EventArgs e)
        {
            AbbrRefreshLayout.Refreshing = false;
        }

        private void OnClickShowAbbrPage(object sender, EventArgs e)
        {
            var intent = new Intent(this,typeof(AddEditAbbreviation));
            intent.PutExtra("isAdd","true");
            StartActivity(intent);
        }

        private void OnClickFilter(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AlphaTokFilter));
            if (Values != null) {
                var convert = JsonConvert.SerializeObject(Values);
                intent.PutExtra("TokQuery",convert);
            }
            StartActivity(intent);
            Finish();
        }

        private void OnClickReturn(object sender, EventArgs e)
        {
            Finish();
        }

        private async  void OnCLickSearch(object sender, EventArgs e)
        {
            if(Values == null)
                 Values = new TokQueryValues() { serviceid = "alphaguess", itemsbase = "toks" };

            Values.text = SearchText.Text;
          await SearchTok(Values) ;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.abbr_menu, menu);

            return base.OnPrepareOptionsMenu(menu);
        }
        public async Task InitializeData()
        {
            ListTokmojiModel = new List<Tokmoji>();
            // cachedClassTokList = new List<ClassTokModel>();
            newClassTokCollection = new List<Shared.Models.TokModel>();
            ClassTokCollection = new List<Shared.Models.TokModel>();
            TaskRecyclerView.SetAdapter(null);
            MainShimmerLayout.StartShimmerAnimation();
            MainShimmerLayout.Visibility = ViewStates.Gone;

            //Load Cached Data of TOKS
            //getCachedValues();
            //End

            //if (cachedClassTokList.Count == 0)
            //{
            //    //If no cache data found then load new toks
            //    await LoadNewToks();
            //}
            await LoadNewToks();
        }

        public async Task LoadMoreData()
        {

            ContinutationToken = AlphaTokService.ContinuationToken;

            if (!string.IsNullOrEmpty(ContinutationToken))
            {
                //var tokQueryModel = new ClassTokQueryValues();
                //tokQueryModel.paginationid = ContinutationToken;
                ////tokQueryModel.loadmore = "yes";
                //tokQueryModel.partitionkeybase = "classtoks";
                //tokQueryModel.text = ""; // filter;
                //tokQueryModel.startswith = false;

                newClassTokCollection = new List<Shared.Models.TokModel>();
                if (Values == null)
                    Values =  new TokQueryValues() { serviceid = "alphaguess", itemsbase = "toks" };

                Values.token = ContinutationToken;
                Values.loadmore = "yes";
               
                var result = await TokService.Instance.GetToksAsync(Values, "abbreviations"); // await ClassService.Instance.GetClassToksAsync(tokQueryModel);
             


                foreach (var item in result)
                { string check = string.Empty;
                    try
                    {
                       
                        if (ClassTokCollection.Count > 0)
                            check = ClassTokCollection?.ToList().Where(j => j.Id == item.Id).Select(j => j.Id)?.First() == null ? string.Empty : "found";
                    }
                    catch (Exception ex) { }
                    if (string.IsNullOrEmpty(check))
                    {

                        newClassTokCollection.Add(item);


                    }
                 

                  
                }

                if(newClassTokCollection.Count > 0)
                    SetClassTokRecyclerAdapter(newClassTokCollection);
            }
        }

        public async Task SearchTok(TokQueryValues values) {
            ClassTokCollection.Clear();
            MainShimmerLayout.StartShimmerAnimation();
            MainShimmerLayout.Visibility = ViewStates.Visible;
            TaskRecyclerView.Visibility = ViewStates.Gone;
            var result = await TokService.Instance.GetToksAsync(values, "abbreviations"); // await ClassService.Instance.GetClassToksAsync(tokQueryModel);

            foreach (var item in result)
            {
                if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                {
                    bool isGetSection = false;


                    if (isGetSection)
                    {

                        //var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                        //var getTokSections = getTokSectionsResult.Results;
                        //  item.Sections = getTokSections.ToArray();
                    }
                }

                ClassTokCollection.Add(item);
            }
            SetClassTokRecyclerAdapter();
            MainShimmerLayout.Visibility = ViewStates.Gone;
            TaskRecyclerView.Visibility = ViewStates.Visible;
        }
        private async Task LoadNewToks()
        {
            MainShimmerLayout.StartShimmerAnimation();
            MainShimmerLayout.Visibility = ViewStates.Visible;

            List<Task> tasksList = new List<Task>();

            tasksList.Add(GetNewToks());

            //if (ListTokmojiModel.Count == 0)
            //{
            //    tasksList.Add(LoadTokMoji());
            //}

            //if (isLoadTab)
            //{
            //    tasksList.Add(CheckCredentials());
            //}

            await Task.WhenAll(tasksList);
        }
        private async Task GetNewToks()
        {
            List<TokModel> resultToksData = new List<TokModel>();


            resultToksData = await GetClassToksData();


            if (resultToksData != null)
            {
                foreach (var item in resultToksData)
                {
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                    {
                        bool isGetSection = false;


                        if (isGetSection)
                        {
                            var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                            var getTokSections = getTokSectionsResult.Results;

                            //  item.Sections = getTokSections.ToArray();
                        }
                    }

                    newClassTokCollection.Add(item);
                    ClassTokCollection.Add(item);
                    //if (cachedClassTokList.Count == 0)
                    //{
                    //    ClassTokCollection.AddRange(newClassTokCollection);
                    //}
                }

                //  setDefaultAdapter();
            }
            SetClassTokRecyclerAdapter();
            MainShimmerLayout.Visibility = ViewStates.Gone;
        }

        public async Task<List<TokModel>> GetClassToksData()
        {
            var taskCompletionSource = new TaskCompletionSource<List<TokModel>>();
            CancellationToken cancellationToken;
            List<TokModel> classTokModelsResult = new List<TokModel>();
            bool isPublicFeed = false;


            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;


                ResultData<TokModel> tokResult = new ResultData<TokModel>();
                tokResult.Results = new List<TokModel>();
                //     var tokmodels = await TokService.Instance.GetAllToks(null);
               var tokResults = await TokService.Instance.GetToksAsync(Values, "abbreviations"); //await ClassService.Instance.GetClassToksAsync(null, cancellationToken, fromCaller: "classtoks_fragment");
            

                if (tokResult == null)
                    return null;

                if (tokResult.ContinuationToken == "cancelled")
                {
                    MainShimmerLayout.Visibility = ViewStates.Gone;
                    // showRetryDialog("Task was cancelled.");
                }
                else
                {
                    classTokModelsResult = tokResults;
                    TaskRecyclerView.ContentDescription = tokResult.ContinuationToken;

                }
            }

            return classTokModelsResult;
        }

        private void SetClassTokRecyclerAdapter(List<TokModel> loadMoreItems = null)
        {
            if (loadMoreItems == null)
            {
                AlphaTokDataAdapter = new AlphaTokDataAdapter(ClassTokCollection, ListTokmojiModel);
                AlphaTokDataAdapter.ItemClick -= OnGridBackgroundClick;
                AlphaTokDataAdapter.ItemClick += OnGridBackgroundClick;
                TaskRecyclerView.SetAdapter(AlphaTokDataAdapter);
            }
            else
            {
                AlphaTokDataAdapter.UpdateItems(loadMoreItems, TaskRecyclerView.ChildCount);
                TaskRecyclerView.SetAdapter(AlphaTokDataAdapter);
                TaskRecyclerView.ScrollToPosition(AlphaTokDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }
        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(AbbreviationInfoActivity));
            classtokModel = ClassTokCollection[position];
            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            if (ParentIntent != null)
            {
                var classGroup = JsonConvert.DeserializeObject<ClassGroupModel>(ParentIntent.GetStringExtra("ClassGroupModel"));
                var classGroupConvert = JsonConvert.SerializeObject(classGroup);
                nextActivity.PutExtra("classGroupModel", classGroupConvert);
            }
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
        }
        [Java.Interop.Export("OnClickTokBack")]
        public void OnClickTokBack(View v)
        {
            Finish();
        }
        #region Properties
        public ShimmerLayout MainShimmerLayout => FindViewById<ShimmerLayout>(Resource.Id.main_shimmer_layout);

        public RecyclerView TaskRecyclerView => FindViewById<RecyclerView>(Resource.Id.main_recyclerView);

        public Button SearchButton => FindViewById<Button>(Resource.Id.btn_search);
        public Button ReturnhButton => FindViewById<Button>(Resource.Id.btn_return);
        public Button FilterButton => FindViewById<Button>(Resource.Id.btn_filter);

        public EditText SearchText => FindViewById<EditText>(Resource.Id.txt_search);

        public Button AddEditButton => FindViewById<Button>(Resource.Id.btn_addabbreviation);

        public SwipeRefreshLayout AbbrRefreshLayout => FindViewById<SwipeRefreshLayout>(Resource.Id.main_swiperefresh_ListToks);
        #endregion
    }
}