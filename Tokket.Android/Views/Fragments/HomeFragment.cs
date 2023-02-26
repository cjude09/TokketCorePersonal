using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Newtonsoft.Json;
using Supercharge;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Xamarin.Essentials;
using Tokket.Android.ViewModels;
using Tokket.Shared.Services;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Color = Android.Graphics.Color;
using ServiceAccount = Tokket.Shared.Services;
using SharedHelpers = Tokket.Shared.Helpers;
using Tokket.Shared.IoC;

namespace Tokket.Android.Fragments
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment
    {
        internal static HomeFragment HFInstance { get; private set; }
        View page = null;
        List<Tokmoji> ListTokmojiModel;
        SwipeRefreshLayout refreshLayout = null;
        GridLayoutManager mLayoutManager; TokModel tokModel;
        public TokDataAdapter tokDataAdapter;
        public bool isSearchFragment { get; set; } = false;
        public string filterText { get; set; } = "";
        public FilterType filterType { get; set; } = FilterType.All;

        //ObservableCollection<TokModel> TokCollection;
        public HomePageViewModel HomeVm => App.Locator.HomePageVM;
        public override LayoutInflater OnGetLayoutInflater(Bundle savedInstanceState)
        {
            var inflater = base.OnGetLayoutInflater(savedInstanceState);
            var contextThemeWrapper = new ContextThemeWrapper(RequireContext(), Resource.Style.AppTheme);
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                contextThemeWrapper = new ContextThemeWrapper(RequireContext(), Resource.Style.AppThemeDark);
            }
            return inflater.CloneInContext(contextThemeWrapper);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.home_page, container, false);

            page = v;
            HFInstance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            tokModel = new TokModel();
            mLayoutManager = new GridLayoutManager(MainActivity.Instance, numcol);
            TaskRecyclerView.SetLayoutManager(mLayoutManager);

            ((Activity)Context).RunOnUiThread(async () => await InitAsync(filterType));    

            refreshLayout = v.FindViewById<SwipeRefreshLayout>(Resource.Id.home_swiperefresh_ListToks);
            refreshLayout.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (TaskRecyclerView != null)
            {
                TaskRecyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (Settings.FilterTag == 9)
                    {
                        //show loading
                        progressbar.Visibility = ViewStates.Visible;

                        //Load more stuff here
                        await HomeVm.GetSearchData(new TokQueryValues());
                    };

                    //hide loading
                    progressbar.Visibility = ViewStates.Invisible;
                };


                TaskRecyclerView.AddOnScrollListener(onScrollListener);

                TaskRecyclerView.SetLayoutManager(mLayoutManager);
            }

            return v;
        }

        public async Task InitAsync(FilterType type = FilterType.All, string sortby = "standard")
        {
            //TokCollection = new ObservableCollection<TokModel>();
            //HomeVm.tokModelLists = TokCollection;
            //HomeVm.tokModelLists.Clear();
            HomeVm.TokDataList = new List<TokModel>();
            TaskRecyclerView.SetAdapter(null);

            shimmerLayout.StartShimmerAnimation();
            shimmerLayout.Visibility = ViewStates.Visible;

            //Stop Shimmery from main if it is now starting to shimmer
            //MainActivity.Instance.main_shimmer_dummy.Visibility = ViewStates.Gone;
            //If this is loaded from MainActivity after splashactivity, verify the token
            CheckCredentials();

            var resultToksData = await HomeVm.GetToksData(filterText, type, sortby);

            //Get Tokmoji
            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult.Results.ToList();

            if (isSearchFragment)
            {
                //FILTER only shows the "View As" option if the Toks tab is selected
                Settings.ActivityInt = (int)ActivityType.TokSearch;

                if (resultToksData.Count() == 0)
                {
                    TextNothingFound.Text = "No toks found.";
                    TextNothingFound.Visibility = ViewStates.Visible;
                }
                else
                {
                    TextNothingFound.Visibility = ViewStates.Gone;
                }
            }


            shimmerLayout.Visibility = ViewStates.Gone;

            //foreach (var item in resultToksData)
            //{
            //    HomeVm.tokModelLists.Add(item);
            //}

            if (Settings.MaintTabInt == (int)MainTab.Search)
            {
                if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                {
                    //HomeVm.TokDataList = resultToksData;
                    HomeVm.TokDataList.AddRange(resultToksData);
                    ToksRecyclerAdapter(resultToksData);
                }
                else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                {
                    HomeVm.tokcardDataAdapter = new TokCardDataAdapter(resultToksData, ListTokmojiModel);
                    HomeVm.tokcardDataAdapter.ItemClick += HomeVm.tokcardDataAdapter.OnItemBackgroundClick;
                    TaskRecyclerView.SetAdapter(HomeVm.tokcardDataAdapter);
                }
            }
            else
            {
                if (Settings.FilterToksHome == (int)FilterToks.Toks)
                {
                    //HomeVm.TokDataList = resultToksData;
                    HomeVm.TokDataList.AddRange(resultToksData);
                    ToksRecyclerAdapter(resultToksData);
                }
                else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                {
                    HomeVm.tokcardDataAdapter = new TokCardDataAdapter(resultToksData, ListTokmojiModel);
                    HomeVm.tokcardDataAdapter.ItemClick += HomeVm.tokcardDataAdapter.OnItemBackgroundClick;
                    TaskRecyclerView.SetAdapter(HomeVm.tokcardDataAdapter);
                }
            }
        }
        private void CheckCredentials()
        {
            bool resultbool = false;
            var idtoken = SecureStorage.GetAsync("idtoken").GetAwaiter().GetResult();
            var refreshtoken = SecureStorage.GetAsync("refreshtoken").GetAwaiter().GetResult();
            var userid = SecureStorage.GetAsync("userid").GetAwaiter().GetResult();

            if (idtoken != null && refreshtoken != null && userid != null)
            {
                var result = ServiceAccount.AccountServiceDB.Instance.VerifyToken(idtoken, refreshtoken);
                var Test = AppContainer.Resolve<AccountServiceDB>().VerifyTokenAsync(idtoken,refreshtoken);
                if (result != null)
                {
                    if (result.ResultEnum == SharedHelpers.Result.Success)
                    {
                        resultbool = true;
                    }
                }
            }

            if (!resultbool)
            {
                //Close this Activity and go back to Login
                Intent logoutActivity = new Intent(MainActivity.Instance, typeof(LoginActivity));
                logoutActivity.AddFlags(ActivityFlags.ClearTop);
                SecureStorage.Remove("idtoken");
                SecureStorage.Remove("refreshtoken");
                SecureStorage.Remove("userid");

                Settings.UserAccount = string.Empty;

                StartActivity(logoutActivity);
                MainActivity.Instance.Finish();
            }
        }
        public void ToksRecyclerAdapter(List<TokModel> tokDataRes)
        {
            tokDataAdapter = new TokDataAdapter(tokDataRes, ListTokmojiModel);
            tokDataAdapter.ItemClick -= OnGridBackgroundClick;
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            TaskRecyclerView.SetAdapter(tokDataAdapter);
        }
        public void InsertToksRecyclerAdapter(TokModel tokModelItem)
        {
            HomeVm.TokDataList.Insert(0, tokModelItem);
            tokDataAdapter = new TokDataAdapter(HomeVm.TokDataList, ListTokmojiModel);
            tokDataAdapter.ItemClick -= OnGridBackgroundClick;
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            TaskRecyclerView.SetAdapter(tokDataAdapter);
        }

        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;
            Intent nextActivity = new Intent(HFInstance.Context, typeof(TokInfoActivity));
            tokModel = HomeVm.TokDataList[position];
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            nextActivity.PutExtra("tokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
        }

        public void RemoveTokCollection(TokModel tokItem)
        {
            var collection = HomeVm.TokDataList.FirstOrDefault(c => c.Id == tokItem.Id);
            if (collection != null)
            {
                int ndx = HomeVm.TokDataList.IndexOf(collection);
                HomeVm.TokDataList.Remove(collection);

                HomeVm.TokDataList.Insert(ndx, tokItem);
                ToksRecyclerAdapter(HomeVm.TokDataList);

                if (MainActivity.Instance.tabLayout.SelectedTabPosition == 2) //Search
                {
                    //To Do: needs to delete Home toks when selected tab is Search
                    //Since home_fragment or classtoks_fragment is called from Search Fragment
                    //it will not be able to delete the toks from the root parent
                }

                /*              if (HomeVm.TokDataList.Count >= ndx)
                              {
                                  TaskRecyclerView.ScrollToPosition(ndx);
                              }*/

                if (ProfileFragment.Instance.ClassTokList != null)
                {
                    ProfileFragment.Instance.RemoveToksCollection(tokItem.Id);
                }

                if (SearchFragment.Instance.tabLayout.SelectedTabPosition == 0)
                {
                    if (SearchFragment.Instance.isSearchedClicked)
                    {
                        SearchFragment.Instance.fragments[0] = new ClassToksFragment(Settings.GetUserModel().UserId)
                        {
                            isSearchFragment = true,
                            filterText = SearchFragment.Instance.SearchText.Text,
                            filterType = FilterType.Text
                        };
                        SearchFragment.Instance.setupViewPager(SearchFragment.Instance.viewpager, 0);
                        SearchFragment.Instance.tabLayout.GetTabAt(0).Select();
                    }
                }
            }
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.HomePage) && (resultCode == -1))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    RemoveTokCollection(tokModel);
                }

                bool changesmade = data.GetBooleanExtra("changesMade",false);
                var dataTokModelstr = data.GetStringExtra("tokModel");
                
                if (changesmade)
                {
                    if (dataTokModelstr != null)
                    {
                        //TokDataList
                        var dataTokModel = JsonConvert.DeserializeObject<TokModel>(dataTokModelstr);
                        if (dataTokModel != null)
                        {
                            var result = HomeVm.TokDataList.FirstOrDefault(c => c.Id == dataTokModel.Id);
                            if (result != null)
                            {
                                int ndx = HomeVm.TokDataList.IndexOf(result);
                                HomeVm.TokDataList.Remove(result);

                                HomeVm.TokDataList.Insert(ndx, dataTokModel);
                                ToksRecyclerAdapter(HomeVm.TokDataList);
                                TaskRecyclerView.ScrollToPosition(ndx);
                            }
                        }
                    }
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
                        var result = HomeVm.TokDataList.FirstOrDefault(c => c.Id == dataTokModel.Id);
                        if (result != null) //If Edit
                        {
                            int ndx = HomeVm.TokDataList.IndexOf(result);
                            HomeVm.TokDataList.Remove(result);

                            HomeVm.TokDataList.Insert(ndx, dataTokModel);
                            ToksRecyclerAdapter(HomeVm.TokDataList);
                        }
                    }
                }
            }
        }
        
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Transparent);
                TextNothingFound.Visibility = ViewStates.Gone;

                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
            else
            {
                TextNothingFound.Text = "No Internet Connection!";
                TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Black);
                TextNothingFound.Visibility = ViewStates.Visible;
                refreshLayout.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            ((Activity)Context).RunOnUiThread(async () => await InitAsync());
            Thread.Sleep(1000);
        }
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }
        public ProgressBar progressbar  => page.FindViewById<ProgressBar>(Resource.Id.progressbar);
        public RecyclerView TaskRecyclerView => page.FindViewById<RecyclerView>(Resource.Id.home_recyclerView);
        public ShimmerLayout shimmerLayout => page.FindViewById<ShimmerLayout>(Resource.Id.home_shimmer_view_container);
        public TextView TextNothingFound => page.FindViewById<TextView>(Resource.Id.TextNothingFound);
    }
}