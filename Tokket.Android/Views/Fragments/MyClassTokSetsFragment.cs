using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Com.Airbnb.Lottie;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.ViewModels;
using Tokket.Core.Tools;
using OperationCanceledException = Android.OS.OperationCanceledException;

namespace Tokket.Android.Fragments
{
    public class MyClassTokSetsFragment : AndroidX.Fragment.App.Fragment
    {
        internal static MyClassTokSetsFragment Instance { get; private set; }
        View page; string UserId, groupId; public List<ClassSetModel> ListClassTokSets; //public List<ClassSetViewModel> ListClassSetModel;
        MySetsAdapter MySetsAdapter; ClassGroupModel classGroupModel;
        GridLayoutManager mLayoutManager; SwipeRefreshLayout refreshLayout = null;
        public ClassSetModel SuperSetSelected { get; set; } = null;
        public ViewModels.MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        public bool isPublicSets,isSuperSets;
        public string searchText { get; set; } = "";
        public string searchType { get; set; } = "";
        bool isShowCheckBox = false, isDelete = false;
        string cachecaller = "";
        public MyClassTokSetsFragment(string _groupId,string _userId = "",bool ispublicsets = false,bool issupersets = false, bool _isShowCheckBox = false, bool _isDelete = false)
        {
            isShowCheckBox = _isShowCheckBox;
            isDelete = _isDelete;
            groupId = _groupId;
            UserId = _userId;
            isPublicSets = ispublicsets;
            isSuperSets = issupersets;
        } 
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (page == null)
                page = inflater.Inflate(Resource.Layout.mysets_toksets_page, container, false);
            
            Instance = this;
            if(string.IsNullOrEmpty(UserId))
                UserId = Settings.GetUserModel().UserId;

            if (isSuperSets)
            {
                if (isPublicSets)
                {
                    cachecaller = "myclasstoksets_fragment_supersets_public";
                }
                else
                {
                    cachecaller = "myclasstoksets_fragment_supersets";
                }
            }
            else
            {
                if (isPublicSets)
                {
                    cachecaller = "myclasstoksets_fragment_sets_public";
                }
                else
                {
                    cachecaller = "myclasstoksets_fragment_sets";
                }
            }

            ListClassTokSets = new List<ClassSetModel>();

            MySetsVm.RecyclerMainList = RecyclerMainList;
            MySetsVm.ShimmerLayout = ShimmerLayout;

            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerMainList.SetLayoutManager(mLayoutManager);

            if (!string.IsNullOrEmpty(groupId))
            {
                TextNothingFound.Visibility = ViewStates.Visible;
            }
            if (SuperSetSelected == null)
            {
                if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ProfileSetsView))
                {
                    PreloadUIToks();
                }
                else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                {
                    ((Activity)Context).RunOnUiThread(async () => await MySetsVm.InitializeData());
                }
            }
            else {
                ListClassTokSets = SuperSetSelected.Sets;

                if (ListClassTokSets.Count == 0)
                {
                    TextNothingFound.Text = "No class sets.";
                }
                else
                {
                    TextNothingFound.Visibility = ViewStates.Gone;
                }

                ShimmerLayout.Visibility = ViewStates.Gone;
                AssignRecyclerAdapter(); //ListClassSetModel
            }
       
            refreshLayout = page.FindViewById<SwipeRefreshLayout>(Resource.Id.mytoksets_swiperefresh);
            refreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (RecyclerMainList != null)
            {
                RecyclerMainList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //Load more stuff here
                    if (SuperSetSelected == null)
                    {
                        if (!string.IsNullOrEmpty(RecyclerMainList.ContentDescription))
                        {
                            showBottomProgress();
                            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                            {
                                await MySetsVm.InitializeData(RecyclerMainList.ContentDescription);
                            }
                            else
                            {
                                await Initialize(RecyclerMainList.ContentDescription);
                            }
                            hideBottomProgress();
                        }
                    }
                    else {
                        //ListClassTokSets = SuperSetSelected.Sets;

                        //if (ListClassTokSets.Count == 0)
                        //{
                        //    TextNothingFound.Text = "No class sets.";
                        //}
                        //else
                        //{
                        //    TextNothingFound.Visibility = ViewStates.Gone;
                        //}

                        //ShimmerLayout.Visibility = ViewStates.Gone;
                        //AssignRecyclerAdapter(); //ListClassSetModel
                    }
                };


                RecyclerMainList.AddOnScrollListener(onScrollListener);

                RecyclerMainList.SetLayoutManager(mLayoutManager);
            }

            return page;
        }
        private void showBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Visible;
        }
        private void hideBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Gone;
        }

        private void PreloadUIToks()
        {
            ShimmerLayout.Visibility = ViewStates.Gone;
            ListClassTokSets = new List<ClassSetModel>();
            var cachedClassToks = ClassService.Instance.GetCacheClassSetAsync(cachecaller);
            if (cachedClassToks != null)
            {
                if (cachedClassToks.Results != null)
                {
                    var cacheList = cachedClassToks.Results.ToList();

                    ListClassTokSets.AddRange(cacheList);

                    AssignRecyclerAdapter();
                }
            }

            if (ListClassTokSets.Count() == 0)
            {
                ((Activity)Context).RunOnUiThread(async () => await Initialize(""));
            }            
        }
        public async Task Initialize(string paginationId = "",bool myClassSet = true,string type= "", bool isRefresh = false)
        {
            int lastposition = RecyclerMainList.ChildCount - 1;
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;

            if (!isRefresh)
            {
                RecyclerMainList.SetAdapter(null);
                ShimmerLayout.StartShimmerAnimation();
                ShimmerLayout.Visibility = ViewStates.Visible;
            }

            //ListClassSetModel = new List<ClassSetViewModel>();
            ListClassTokSets = new List<ClassSetModel>();

            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            string userid = string.Empty;
            if (string.IsNullOrEmpty(groupId) && myClassSet)
                userid = UserId;
            else if (string.IsNullOrEmpty(groupId) && !myClassSet)
                userid = string.Empty;

            if (isPublicSets)
                userid = string.Empty;

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;
                 
                var setQueryValues = new ClassSetQueryValues()
                {
                    userid = userid,
                    groupid = groupId,
                    partitionkeybase = "classsets",
                    startswith = false,
                    paginationid = paginationId,
                    name = searchType == "name" ? searchText : string.Empty,
                    category = searchType == "category" ? searchText : string.Empty
                };


                ResultData<ClassSetModel> result = null;

                if (!string.IsNullOrEmpty(paginationId))
                {
                    showBottomProgress();
                }
                if (isSuperSets)
                    setQueryValues.label = "superset";

                if (!myClassSet || isPublicSets)
                {
                    //setQueryValues.userid = UserId;
                    result = await ClassService.Instance.GetClassSetAsync(setQueryValues, cancellationToken, cachecaller);
                    if (result.ContinuationToken == "cancelled")
                    {
                        //    ShimmerLayout.Visibility = ViewStates.Gone;
                        //    showRetryDialog("Task was cancelled.");
                    }
                    else
                    {
                        ListClassTokSets.AddRange(result.Results.ToList());
                    }
                }
                else
                {
                    result = await ClassService.Instance.GetClassSetAsync(setQueryValues, cancellationToken, cachecaller);
                }

                if (result == null)
                    result = new ResultData<ClassSetModel>();

                if (!string.IsNullOrEmpty(paginationId))
                {
                    hideBottomProgress();
                }

                if (result.ContinuationToken == "cancelled")
                {
                    ShimmerLayout.Visibility = ViewStates.Gone;
                    showRetryDialog("Task was cancelled.");
                }
                else
                {
                    RecyclerMainList.ContentDescription = result.ContinuationToken;
                    if (string.IsNullOrEmpty(type))
                    {
                        ListClassTokSets.AddRange(result.Results.ToList());
                    }
                    else
                    {
                        ListClassTokSets.Clear();

                        var filtersets = new List<ClassSetModel>();
                        foreach (var sets in result.Results.ToList())
                        {
                            if (type.ToLower() == "playable")
                            {
                                if (sets.IsPlayable == true)
                                {
                                    filtersets.Add(sets);
                                }
                            }
                            else if (type.ToLower() == "non-playable")
                            {
                                if (sets.IsPlayable == false)
                                {
                                    filtersets.Add(sets);
                                }
                            }
                        }
                        ListClassTokSets.AddRange(filtersets);
                    }

                    /*for (int i = 0; i < ListClassTokSets.Count; i++)
                    {
                        ClassSetViewModel ModelItem = new ClassSetViewModel();

                        var classtokRes = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues() { partitionkeybase = $"{ListClassTokSets[i].Id}-classtoks" }, cancellationToken);
                        if (classtokRes.ContinuationToken == "cancelled")
                        {
                            ShimmerLayout.Visibility = ViewStates.Gone;
                            showRetryDialog("Task was cancelled.");
                            return;
                        }
                        ModelItem.ClassToks = classtokRes.Results.ToList();
                        ModelItem.ClassSet = ListClassTokSets[i];
                        ListClassSetModel.Add(ModelItem);
                    }*/
                }
            }

            if (ListClassTokSets.Count == 0)
            {
                TextNothingFound.Text = "No class sets.";
            }
            else {
                TextNothingFound.Visibility = ViewStates.Gone;
            }
            
            ShimmerLayout.Visibility = ViewStates.Gone;
            AssignRecyclerAdapter(); //ListClassSetModel
            if (!string.IsNullOrEmpty(paginationId))
            {
                RecyclerMainList.ScrollToPosition(lastposition);
            }
        }

        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(MyClassSetsActivity.Instance)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {

                            })
                            .SetNegativeButton("Retry",async (_, args) =>
                            {
                                await Initialize("");
                            })
                            .SetCancelable(false)
                            .Show();
        }
        public void AssignRecyclerAdapter()
        {
            if (ListClassTokSets.Count == 0)
            {
                TextNothingFound.Text = "No class sets.";
            }
            else
            {
                TextNothingFound.Visibility = ViewStates.Gone;
            }
            MySetsAdapter = new MySetsAdapter(null, ListClassTokSets, null);
            MySetsAdapter.ItemClick += MySetsAdapter.OnItemRowClick;
            RecyclerMainList.SetAdapter(MySetsAdapter);
        }

        public void showCheckBoxView(bool isVisible)
        {
            MySetsAdapter.isShowCheckBoxDelete(isVisible);
        }

        public async Task deleteClassSetStart()
        {
            if (MySetsAdapter.ListClassItemsChecked.Count() > 0)
            {
                linearDeleteProgress.Visibility = ViewStates.Visible;
            }

            for (int i = 0; i < MySetsAdapter.ListClassItemsChecked.Count(); i++)
            {
                var item = MySetsAdapter.ListClassItemsChecked[i];
                if (item.isCheck)
                {
                    tvDeleteProgress.Text = $"Deleting {i + 1} of {MySetsAdapter.ListClassItemsChecked.Count()}...";
                    bool? result = null;
                    result = await ClassService.Instance.DeleteClassSetAsync(item.Id, item.PartitionKey);

                    if (result != null)
                    {
                        var dataResult = ListClassTokSets.FirstOrDefault(c => c.Id == item.Id);
                        if (dataResult != null)
                        {
                            int ndx = ListClassTokSets.IndexOf(dataResult);
                            ListClassTokSets.Remove(dataResult);
                        }

                        if (i == MySetsAdapter.ListClassItemsChecked.Count() - 1)
                        {
                            MyClassSetsActivity.Instance.CancelDeleteSets();
                            ClassService.Instance.SetCacheClassSetAsync(cachecaller, ListClassTokSets);
                            await Task.Delay(1000);
                            AssignRecyclerAdapter();
                            linearDeleteProgress.Visibility = ViewStates.Gone;
                        };
                    }
                }
            }
        }

        public void PassItemClassSetsFromAddClassSet(ClassSetModel model, bool isSave = true, bool isDelete = false)
        {
            if (isSave)
            {
                ListClassTokSets.Add(model);
                MySetsAdapter.updateItems(null, ListClassTokSets);
            }
            else if(isDelete)
            {
                //If Delete
                var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
                if (result != null) //If Edit
                {
                    int ndx = ListClassTokSets.IndexOf(result);
                    ListClassTokSets.Remove(result);
                }
            }
            else
            {
                //If Update
                var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
                if (result != null) //If Edit
                {
                    int ndx = ListClassTokSets.IndexOf(result);
                    ListClassTokSets.Remove(result);
                    ListClassTokSets.Insert(ndx, model);
                }
              
            }
            AssignRecyclerAdapter();
        }

        public void UpdateSuperSetInfoItem(List<ClassSetModel> modelList, bool isAddSet = true) {
            if (isAddSet)
            {
                ListClassTokSets.AddRange(modelList);
                MySetsAdapter.updateItems(null, ListClassTokSets);
            }
            else {
                foreach (var model in modelList)
                {
                    //If Update
                    var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
                    if (result != null) //If Edit
                    {
                        int ndx = ListClassTokSets.IndexOf(result);
                        ListClassTokSets.Remove(result);
                    }
                    // MySetsAdapter.updateItems(null, ListClassTokSets);
                }
            }
            AssignRecyclerAdapter();
        }

        public void deleteItemClassSet(ClassSetModel model)
        {
            var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
            if (result != null) //If Edit
            {
                int ndx = ListClassTokSets.IndexOf(result);
                ListClassTokSets.Remove(result);
            }
            AssignRecyclerAdapter();
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            //Data Refresh Place  
            if (SuperSetSelected == null) {
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
      
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            RecyclerMainList.ContentDescription = null;
            FilterType type = (FilterType)Settings.FilterTag;
            ((Activity)Context).RunOnUiThread(async () => await Initialize("", isRefresh: true));
            Thread.Sleep(1000);
        }

        public override void OnResume()
        {
            base.OnResume();
            //if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity))
            //{
            //    ((Activity)Context).RunOnUiThread(async () => await Initialize());
            //}
            //else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
            //{
            //    ((Activity)Context).RunOnUiThread(async () => await MySetsVm.InitializeData());
            //}
        }

        public ProgressBar bottomProgress => page.FindViewById<ProgressBar>(Resource.Id.bottomProgress);
        public RecyclerView RecyclerMainList => page.FindViewById<RecyclerView>(Resource.Id.recyclerView_mytoksets);
        public ShimmerLayout ShimmerLayout => page.FindViewById<ShimmerLayout>(Resource.Id.mysets_shimmer_view_container);
        public TextView TextNothingFound => page.FindViewById<TextView>(Resource.Id.TextNothingFound);
        public LinearLayout linearDeleteProgress => page.FindViewById<LinearLayout>(Resource.Id.linearDeleteProgress);
        public TextView tvDeleteProgress => page.FindViewById<TextView>(Resource.Id.tvDeleteProgress);
    }
}