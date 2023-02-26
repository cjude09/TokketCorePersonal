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
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using Supercharge;
using Tokket.Android.Adapters;
using Tokket.Android.Custom;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Shared.ViewModels;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Color = Android.Graphics.Color;
using ServiceAccount = Tokket.Shared.Services;
using SharedHelpers = Tokket.Shared.Helpers;
using AuthorizationTokenModel = Tokket.Shared.Models.AuthorizationTokenModel;
using Tokket.Android.Helpers;

namespace Tokket.Android.Fragments
{
    public class ClassToksFragment : AndroidX.Fragment.App.Fragment
    {
        bool isFromFilterPage = false;
        internal static ClassToksFragment Instance { get; private set; }
        View v; ClassTokModel classtokModel;
        GridLayoutManager mLayoutManager;
        public ClassTokDataAdapter ClassTokDataAdapter; ClassTokCardDataAdapter classtokcardDataAdapter;
        public List<ClassTokModel> ClassTokCollection, newClassTokCollection; List<Tokmoji> ListTokmojiModel;
        string groupId = ""; ClassSetModel classSet; ClassSetViewModel ClassSetVM;
        public bool isSearchFragment { get; set; } = false;
        public string filter = "";
        public string filterText { get; set; } = "";
        public FilterType filterType { get; set; } = FilterType.All;
        FilterBy filterBy = FilterBy.None;
        List<string> filterItems = new List<string>();
        bool isLoadTab;
        ResultData<Tokmoji> tokmojiResult;
        public Intent ParentIntent { get; set; }
        string _fromCaller = "classtoks_fragment_home";
        bool isClassGroup;
        List<ClassTokModel> newToksData;
        private bool isLoadingMoreItems = false;
        List<ClassTokModel> cachedClassTokList;
        private Context context;
        public ClassToksFragment(string _groupId, ClassSetModel _classSetId = null, string _ClassSetVM = "", bool _isLoadTab = false, bool _isClassGroup = false, Context cntxt = null)
        {
            groupId = _groupId;
            classSet = _classSetId;
            ClassSetVM = JsonConvert.DeserializeObject<ClassSetViewModel>(_ClassSetVM);
            isLoadTab = _isLoadTab;
            isClassGroup = _isClassGroup;
            context = cntxt;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.home_page, container, false);
            Instance = this;

            if (groupId == "")
            {
                TextNothingFound.Visibility = ViewStates.Gone;
                ClassTokIconImg.Visibility = ViewStates.Visible;
            }
            else
            {
                if (!string.IsNullOrEmpty(groupId))
                {
                    _fromCaller = "classtoks_fragment_group";
                }
                TextNothingFound.Visibility = ViewStates.Visible;
                ClassTokIconImg.Visibility = ViewStates.Gone;
            }

            if (string.IsNullOrEmpty(TextNothingFound.Text)) TextNothingFound.Visibility = ViewStates.Gone;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            TaskRecyclerView.SetLayoutManager(mLayoutManager);

            initViews();
            
            RefreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            RefreshLayout.Refresh -= RefreshLayout_Refresh;
            RefreshLayout.Refresh += RefreshLayout_Refresh;

            btnLoadNewToks.Click += delegate
            {
                btnLoadNewTokisClick();
                btnLoadNewToks.Visibility = ViewStates.Gone;
            };

            if (string.IsNullOrEmpty(groupId) && !isClassGroup)
            {
                tokmojiResult = TokMojiService.Instance.GetCacheTokmojisAsync();
                if (tokmojiResult.Results != null)
                {
                    ListTokmojiModel = tokmojiResult.Results.ToList();
                }

                string callername = _fromCaller;
                if (isSearchFragment)
                {
                    callername = "";
                }

                if (classSet != null)
                {
                    btnLoadNewToks.Visibility = ViewStates.Gone;
                    ClassTokCollection.AddRange(ClassSetVM.ClassToks);

                    setDefaultAdapter();
                }
                else
                {
                    var cachedClassToks = ClassService.Instance.GetCacheClassToksAsync(callername);
                    if (cachedClassToks?.Results != null)
                    {
                        cachedClassTokList = cachedClassToks.Results.ToList();

                        if (cachedClassTokList.Count > 0)
                        {
                            //Removed the mega section TODO will add it in the future

                            ClassTokCollection.AddRange(cachedClassTokList);

                            setDefaultAdapter();
                        }
                    }
                }

                if (ClassTokCollection.Count() == 0)
                {
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += WORK_DATA_LOAD;
                    work.RunWorkerCompleted += WORK_DATA_LOAD_COMPLETE;
                    work.RunWorkerAsync();
                }
            }
            else
            {
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += WORK_DATA_LOAD;
                work.RunWorkerCompleted += WORK_DATA_LOAD_COMPLETE;
                work.RunWorkerAsync();
            }

            return v;
        }

        private void WORK_DATA_LOAD(object sender, DoWorkEventArgs e)
        {
            RequireActivity().RunOnUiThread(async() => {
                ShimmerLayout.StartShimmerAnimation();
                await LoadContentData();
            });
        }

        private async Task LoadContentData()
        {
            await InitializeData();
        }
        private void WORK_DATA_LOAD_COMPLETE(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TaskRecyclerView != null)
            {
                TaskRecyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //Add isLoadingMoreItems because it will go in this even twice
                    if (isLoadingMoreItems)
                        return;

                    if (isClassGroup)
                    {
                        if (!string.IsNullOrEmpty(TaskRecyclerView.ContentDescription))
                        {
                            await LoadMoreData();
                        }
                    }
                    else if (isSearchFragment)
                    {
                        if (!string.IsNullOrEmpty(Settings.SearchToksContinuationToken))
                        {
                            await LoadMoreData();
                        }
                    }
                    else
                    {
                        if (btnLoadNewToks.Visibility != ViewStates.Visible)
                        {
                            if (classSet == null)
                                await LoadMoreData();
                        }
                    }
                };

                TaskRecyclerView.AddOnScrollListener(onScrollListener);

                TaskRecyclerView.SetLayoutManager(mLayoutManager);
            }
        }

        private void showBottomLoading()
        {
            progressbar.IndeterminateDrawable.SetColorFilter(new Color(ContextCompat.GetColor(v.Context, Resource.Color.colorAccent)), PorterDuff.Mode.Multiply);
            progressbar.Visibility = ViewStates.Visible;
        }
        private void hideBottomLoading()
        {
            progressbar.Visibility = ViewStates.Gone;
        }

        private void initViews()
        {
            ListTokmojiModel = new List<Tokmoji>();
            cachedClassTokList = new List<ClassTokModel>();
            newClassTokCollection = new List<ClassTokModel>();
            ClassTokCollection = new List<ClassTokModel>();
        }
        public async Task InitializeData(bool isFromFilter = false, bool isSwipeRefresh = false)
        {
            isFromFilterPage = isFromFilter;
            ShimmerLayout.StartShimmerAnimation();
            ShimmerLayout.Visibility = ViewStates.Gone;

            if (!isSwipeRefresh)
            {
                //Only set this if not used by pull refresh
                TaskRecyclerView.SetAdapter(null);

                await CheckToken();
            }
            
            if (classSet != null)
            {
                btnLoadNewToks.Visibility = ViewStates.Gone;
                ClassTokCollection.AddRange(ClassSetVM.ClassToks);

                setDefaultAdapter();
            }
            else if (isClassGroup)
            {
                btnLoadNewToks.Visibility = ViewStates.Gone;

                ShimmerLayout.Visibility = ViewStates.Visible;
                var resultToks = await GetClassToksData();
                if (resultToks == null)
                    resultToks = new List<ClassTokModel>();
                ShimmerLayout.Visibility = ViewStates.Gone;
                RefreshLayout.Refreshing = false;

                TextNothingFound.Visibility = ViewStates.Gone;
                if (resultToks.Count == 0)
                {
                    TextNothingFound.Text = "No class toks.";
                    TextNothingFound.Visibility = ViewStates.Visible;
                }

                ClassTokCollection.AddRange(resultToks);
                setDefaultAdapter();
            }
            else
            {              
                if (cachedClassTokList.Count == 0 || isFromFilter == true)
                {
                    if (isFromFilter)
                    {
                        ClassTokCollection.Clear();
                        newClassTokCollection.Clear();
                        if (Barrel.Current.Exists(_fromCaller))
                        {
                            Barrel.Current.Empty(_fromCaller);
                        }
                    }

                    if (!isSwipeRefresh)
                    {
                        //Only set this if not used by pull refresh
                        ShimmerLayout.StartShimmerAnimation();
                        ShimmerLayout.Visibility = ViewStates.Visible;
                    }
                    
                    //If no cache data found then load new toks

                    if (isSwipeRefresh)
                    {
                        await GetNewToks(isSwipeRefresh);
                    }
                    else
                    {
                        await LoadNewToks(true, true);
                    }

                    if (newClassTokCollection.Count > 0)
                    {
                        if (isSwipeRefresh)
                        {
                            ClassTokCollection.Clear(); //Clear All to add new ones
                        }

                        ClassTokCollection.AddRange(newClassTokCollection);
                    }

                    setDefaultAdapter();
                    
                    RefreshLayout.Refreshing = false;
                }
                else
                {
                    //Empty list if new toks added
                    //param list of keys to flush Monkey Cache
                    try
                    {
                        string callername = _fromCaller;
                        if (isSearchFragment)
                        {
                            callername = "search_fragment";
                        }

                        if (Barrel.Current.Exists(callername))
                        {
                            Barrel.Current.Empty(callername);
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex);
                    }

                    if (isSwipeRefresh)
                    {
                        await GetNewToks(isSwipeRefresh);

                        RefreshLayout.Refreshing = false;
                    }
                    else
                    {
                        await LoadNewToks(false, false);
                    }

                    if (newClassTokCollection.Count > 0)
                    {
                        if (isSwipeRefresh)
                        {
                            ClassTokCollection.Clear(); //Clear All to add new ones
                        }
                        ClassTokCollection.AddRange(newClassTokCollection);
                    }

                    setDefaultAdapter();
                }
            }

            ShimmerLayout.Visibility = ViewStates.Gone;
        }

        public void refreshClassSetVMToks(ClassSetViewModel classSetVM)
        {
            ClassSetVM = classSetVM;
            ClassTokCollection.Clear();
            ClassTokCollection.AddRange(ClassSetVM.ClassToks);
            setDefaultAdapter();
        }
        public async Task LoadNewToks(bool isLoadTokMoji, bool isLoadCredentials)
        {
            List<Task> tasksList = new List<Task>();

            tasksList.Add(GetNewToks());

            if (isLoadTokMoji)
            {
                if (ListTokmojiModel.Count == 0)
                {
                    tasksList.Add(LoadTokMoji());
                }
            }

            if (isLoadCredentials)
            {
                if (isLoadTab)
                {
                    tasksList.Add(CheckCredentials());
                }
            }

            //await Task.WhenAll(tasksList);

            var downloadTasks = tasksList;
            while (downloadTasks.Any())
            {
                var finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
            }
        }

        //This is called in MainActivity
        public async Task LoadPublicMyClassToks()
        {
            TaskRecyclerView.SetAdapter(null);

            ShimmerLayout.StartShimmerAnimation();
            ShimmerLayout.Visibility = ViewStates.Visible;

            ClassTokCollection.Clear();
            if (Barrel.Current.Exists(_fromCaller))
            {
                Barrel.Current.Empty(_fromCaller);
            }

            var resultToks = await GetClassToksData();
            ClassTokCollection.AddRange(resultToks);

            setDefaultAdapter();
            ShimmerLayout.Visibility = ViewStates.Invisible;
        }
        private async Task GetNewToks(bool isRefresh = false)
        {
            List<ClassTokModel> resultToksData = new List<ClassTokModel>();
            newToksData = new List<ClassTokModel>();
            if (classSet != null && !isClassGroup)
            {
                resultToksData = ClassSetVM.ClassToks;
            }
            else
            {
                try
                {
                    resultToksData = await GetClassToksData();
                    if (resultToksData != null)
                        newToksData = resultToksData;
                }
                catch (Exception ex){
                    resultToksData = new List<ClassTokModel>();
                    showRetryDialog("Failed! " + ex.Message.ToString());
                }
            }

            if (resultToksData != null)
            {
                bool newToksFound = false;
                foreach (var item in resultToksData)
                {
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                    {
                        bool isGetSection = false;
                        if (isSearchFragment)
                        {
                            if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                            {
                                isGetSection = true;
                            }
                        }
                        else
                        {
                            if (Settings.FilterToksHome == (int)FilterToks.Cards)
                            {
                                isGetSection = true;
                            }
                        }

                        if (isGetSection)
                        {
                            var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                            var getTokSections = getTokSectionsResult.Results;

                            item.Sections = getTokSections.ToArray();
                        }
                    }

                    if (ClassTokCollection.Count > 0)
                    {
                        if (isRefresh)
                        {
                            newClassTokCollection.Add(item);
                        }
                        else
                        {
                            //Check if item already in the list.
                            var result = ClassTokCollection.FirstOrDefault(c => c.Id == item.Id);
                            if (result != null) //If Edit
                            {
                                int ndx = ClassTokCollection.IndexOf(result);
                                ClassTokCollection.Remove(result);

                                ClassTokCollection.Insert(ndx, item);
                            }
                            else
                            {
                                newToksFound = true;
                                newClassTokCollection.Add(item);
                            }
                        }
                    }
                    else
                    {
                        newClassTokCollection.Add(item);
                    }
                }

                if (isFromFilterPage)
                {
                    if (newClassTokCollection.Count > 0 && ClassTokCollection.Count == 0)
                    {
                        ClassTokCollection.AddRange(newClassTokCollection);
                        setDefaultAdapter();
                    }
                    else
                    {
                        setDefaultAdapter();
                    }
                }

                if (classSet == null && !isClassGroup)
                {                    
                    if (newToksFound)
                    {
                        btnLoadNewToks.Visibility = ViewStates.Visible;
                    }
                }
            }
        }
        public void btnLoadNewTokisClick()
        {
            //Insert new toks found
            //Remove previous data and load new toks
            if (Barrel.Current.Exists(_fromCaller))
            {
                Barrel.Current.Empty(_fromCaller);
            }
            var resultData = new ResultData<ClassTokModel>();
            resultData.Results = newToksData;
            ClassService.Instance.AddNewToksFoundCache(_fromCaller, JsonConvert.SerializeObject(resultData));

            ClassTokCollection.InsertRange(0, newClassTokCollection);

            setDefaultAdapter(true);
        }
        private async Task LoadTokMoji()
        {
            tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            if(tokmojiResult!= null)
                 ListTokmojiModel = tokmojiResult.Results.ToList();
        }

        public void setDefaultAdapter(bool isShowBtnNewToks = false)
        {
            ShimmerLayout.Visibility = ViewStates.Gone;
            if (isSearchFragment)
            {
                //FILTER only shows the "View As" option if the Toks tab is selected
                Settings.ActivityInt = (int)ActivityType.TokSearch;

                if (ClassTokCollection.Count == 0)
                {
                    TextNothingFound.Text = "No class toks.";
                }
                else
                {
                    TextNothingFound.Visibility = ViewStates.Gone;
                }

                if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                {
                    SetClassTokRecyclerAdapter(showbtnNewToks: isShowBtnNewToks);
                }
                else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                {
                    SetClassCardsRecyclerAdapter();
                }
            }
            else
            {
                if (Settings.FilterToksHome == (int)FilterToks.Toks)
                {
                    SetClassTokRecyclerAdapter(showbtnNewToks: isShowBtnNewToks);
                }
                else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                {
                    SetClassCardsRecyclerAdapter();
                }
            }
        }
        private async Task CheckToken() {
            try {
                bool resultbool = false;
                var idtoken = await SecureStorage.GetAsync("idtoken");
                var refreshtoken = await SecureStorage.GetAsync("refreshtoken");
                var result = ServiceAccount.AccountService.Instance.VerifyToken(idtoken, refreshtoken);
                if (result.ResultMessage.Contains("refreshed"))
                {
                    SecureStorage.Remove("idtoken");
                    SecureStorage.Remove("refreshtoken");
                    var obj = JsonConvert.DeserializeObject<AuthorizationTokenModel>(result.ResultObject.ToString());

                    await SecureStorage.SetAsync("idtoken", obj.IdToken);
                    await SecureStorage.SetAsync("refreshtoken", obj.RefreshToken);
                }
            } catch (Exception ex) { }
          
        }

        private async Task CheckCredentials()
        {
            string message = "";
            bool resultbool = false;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var refreshtoken = await SecureStorage.GetAsync("refreshtoken");
            var userid = await SecureStorage.GetAsync("userid");

            if (idtoken != null && refreshtoken != null && userid != null)
            {
                var result = ServiceAccount.AccountServiceDB.Instance.VerifyToken(idtoken, refreshtoken);
                if (result != null)
                {
                    if (result.ResultEnum == SharedHelpers.Result.Success)
                    {
                        resultbool = true;
                    }
                    else
                    {
                        message = result.ResultMessage;
                    }
                }
            }

            if (!resultbool)
            {
                GoToLoginActivity(message);
            }
        }

        private void GoToLoginActivity(string message = "")
        {
            //Close this Activity and go back to Login
            Intent logoutActivity = new Intent(MainActivity.Instance, typeof(LoginActivity));
            logoutActivity.PutExtra("message", message);
            logoutActivity.AddFlags(ActivityFlags.ClearTop);
            SecureStorage.Remove("idtoken");
            SecureStorage.Remove("refreshtoken");
            SecureStorage.Remove("userid");

            Settings.UserAccount = string.Empty;

            StartActivity(logoutActivity);
            MainActivity.Instance.Finish();
        }
        public void AddClassTokCollection(ClassTokModel classTokItem)
        {
            int ndx = 0;
            bool isEdit = false;
            var collection = ClassTokCollection.FirstOrDefault(a => a.Id == classTokItem.Id);
            if (collection != null) //If item exist
            {
                ndx = ClassTokCollection.IndexOf(collection); //Get index
                ClassTokCollection.Remove(collection);
                ClassTokCollection.Insert(ndx, classTokItem);
                isEdit = true;
            }
            else
            {
                ClassTokCollection.Insert(0, classTokItem);
            }

            if (ClassTokCollection.Count > 0)
            {
                TextNothingFound.Visibility = ViewStates.Gone;
            }

            if (isSearchFragment)
            {
                if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                {
                    SetClassTokRecyclerAdapter();
                }
                else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                {
                    SetClassCardsRecyclerAdapter();
                }
            }
            else
            {
                if (Settings.FilterToksHome == (int)FilterToks.Toks)
                {
                    SetClassTokRecyclerAdapter();
                }
                else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                {
                    SetClassCardsRecyclerAdapter();
                }
            }


            if (isEdit)
            {
                TaskRecyclerView.ScrollToPosition(ndx);
            }
        }
        private void SetClassTokRecyclerAdapter(List<ClassTokModel> loadMoreItems = null, bool showbtnNewToks = false)
        {
            if (loadMoreItems == null)
            {
                try
                {
                    ClassTokDataAdapter = new ClassTokDataAdapter(RequireContext(), ClassTokCollection, ListTokmojiModel);

                    ClassTokDataAdapter.ItemClick -= OnGridBackgroundClick;
                    ClassTokDataAdapter.ItemClick += OnGridBackgroundClick;
                    TaskRecyclerView.SetAdapter(ClassTokDataAdapter);
                    if (showbtnNewToks)
                    {
                        MainActivity.Instance.MainAppBar.SetExpanded(true, true);
                        TaskRecyclerView.ScrollToPosition(0);
                        btnLoadNewToks.Visibility = ViewStates.Gone;
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                ClassTokDataAdapter.UpdateItems(loadMoreItems, ClassTokDataAdapter.ItemCount);
                //TaskRecyclerView.SetAdapter(ClassTokDataAdapter);
                TaskRecyclerView.ScrollToPosition(ClassTokDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }
        private void SetClassCardsRecyclerAdapter(List<ClassTokModel> loadMoreItems = null)
        {
            if (loadMoreItems == null)
            {
                classtokcardDataAdapter = new ClassTokCardDataAdapter(ClassTokCollection, ListTokmojiModel);
                classtokcardDataAdapter.ItemClick += OnGridBackgroundClick;
                TaskRecyclerView.SetAdapter(classtokcardDataAdapter);
            }
            else
            {
                classtokcardDataAdapter.UpdateItems(loadMoreItems, TaskRecyclerView.ChildCount);
                TaskRecyclerView.SetAdapter(classtokcardDataAdapter);
                TaskRecyclerView.ScrollToPosition(classtokcardDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }
        public int RemoveClassTokCollection(ClassTokModel classTokItem)
        {
            var collection = ClassTokCollection.FirstOrDefault(a => a.Id == classTokItem.Id);
            if (collection != null) //If item exist
            {
                int ndx = ClassTokCollection.IndexOf(collection); //Get index
                ClassTokCollection.Remove(collection);

                if (isSearchFragment)
                {
                    if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                    {
                        SetClassTokRecyclerAdapter();
                    }
                    else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                    {
                        SetClassCardsRecyclerAdapter();
                    }
                }
                else
                {
                    if (Settings.FilterToksHome == (int)FilterToks.Toks)
                    {
                        SetClassTokRecyclerAdapter();
                    }
                    else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                    {
                        SetClassCardsRecyclerAdapter();
                    }
                }

                if (MainActivity.Instance.tabLayout.SelectedTabPosition == 1) //Search
                {
                    //To Do: needs to delete Home toks when selected tab is Search
                    //Since home_fragment or classtoks_fragment is called from Search Fragment
                    //it will not be able to delete the toks from the root parent
                }

                /*if (ClassTokCollection.Count >= ndx)
                {
                    TaskRecyclerView.ScrollToPosition(ndx);
                }*/

                if (ProfileFragment.Instance.ClassTokList != null)
                {
                    ProfileFragment.Instance.RemoveToksCollection(classTokItem.Id);
                }

                if (SearchFragment.Instance.tabLayout.SelectedTabPosition == 0)
                {
                    if (SearchFragment.Instance.isSearchedClicked)
                    {
                        SearchFragment.Instance.fragments[0] = new ClassToksFragment(Settings.UserId)
                        {
                            isSearchFragment = true,
                            filterText = SearchFragment.Instance.SearchText.Text,
                            filterType = FilterType.Text,
                            filter = SearchFragment.Instance.toksfilter
                        };
                        SearchFragment.Instance.setupViewPager(SearchFragment.Instance.viewpager, 0);
                        SearchFragment.Instance.tabLayout.GetTabAt(0).Select();
                    }
                }
            }

            return ClassTokCollection.Count;
        }

        public void AddNewMultipleToks(List<ClassTokModel> classTokList)
        {
            //Empty the barrel of the home
            if (Barrel.Current.Exists(_fromCaller))
            {
                Barrel.Current.Empty(_fromCaller);
            };

            ClassTokCollection.InsertRange(0, classTokList);

            //Add new data to insert
            ClassService.Instance.SetCacheClassToksAsync(_fromCaller, ClassTokCollection);

            setDefaultAdapter();
        }

        public int UpdateClassTokCollection(ClassTokModel classTokItem)
        {
            var collection = ClassTokCollection.FirstOrDefault(a => a.Id == classTokItem.Id);
            if (collection != null) //If item exist
            {
                int ndx = ClassTokCollection.IndexOf(collection); //Get index

                ClassTokCollection[ndx] = classTokItem;

                SetClassTokRecyclerAdapter();
                if (ProfileFragment.Instance.ClassTokList != null)
                {
                    //profile_fragment.Instance.RemoveToksCollection(classTokItem.Id);
                }

            }

            return ClassTokCollection.Count;
        }
        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;

            Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));

            classtokModel = ClassTokCollection[position];
            if (ClassTokCollection[position].ViewsModel != null) {
                if (Settings.GetTokketUser().Id == ClassTokCollection[position].UserId)
                {
                    ClassTokCollection[position].ViewsModel.TileTapViewsPersonal += 1;
                }
                else
                {
                    ClassTokCollection[position].ViewsModel.TileTapViews += 1;
                }
            }
          
           
            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            if (ParentIntent != null)
            {
                if (isClassGroup)
                {
                    var classGroup = JsonConvert.DeserializeObject<ClassGroupModel>(ParentIntent.GetStringExtra("ClassGroupModel"));
                    var classGroupConvert = JsonConvert.SerializeObject(classGroup);
                    nextActivity.PutExtra("classGroupModel", classGroupConvert);
                }

                if (classSet != null && !isClassGroup)
                {
                    var serializeSet = JsonConvert.SerializeObject(classSet);
                    nextActivity.PutExtra("ClassSet", serializeSet);
                    var classToks = JsonConvert.SerializeObject(ClassSetVM.ClassToks);
                    nextActivity.PutExtra("ClassSetToks", classToks);
                }

            }
          
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
            //Task.Run(() => { UpdateClassTokCollection(ClassTokCollection[position]); });
         
        }
        public async Task<List<ClassTokModel>> GetClassToksData()
        {
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;
            List<ClassTokModel> classTokModelsResult = new List<ClassTokModel>();
            bool isPublicFeed = false;
            if (Settings.FilterFeed == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            if (isClassGroup)
            {
                isPublicFeed = false;
            }

            if (isSearchFragment)
            {
                filterBy = (FilterBy)Settings.FilterByTypeSearch;
                filterItems = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsSearch);
            }
            else
            {
                filterBy = (FilterBy)Settings.FilterByTypeHome;
                filterItems = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsHome);
            }

            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            var ownerId = Settings.GetUserModel().UserId;

            string callername = _fromCaller;
            if (isSearchFragment)
            {
                callername = "";
            }
            else if (isClassGroup)
            {
                callername = "";
            }

            var timer = new TimerTask();
            timer.TimerStart(RequireActivity());

            var tokResult = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues()
            {
                partitionkeybase = isPublicFeed ? "classtoks" : (!string.IsNullOrEmpty(groupId) ? $"{groupId}-classtoks" : $"{ownerId}-classtoks"),
                groupid = groupId,
                userid = isPublicFeed || !string.IsNullOrEmpty(groupId) ? "" : ownerId,
                text = filter,
                startswith = false,
                publicfeed = isPublicFeed,
                FilterBy = filterBy,
                FilterItems = filterItems,
                paginationid = "",
                level1 = "",
                level2 = "",
                level3 = "",
                descending = Settings.SortByTitleAscending,
                orderby = Settings.SortByReferenceAscending,
                classtokmode = true
            });

            timer.TimerStop();

            var TimeElapsed = timer.GetTimeElapsed();

            // await ClassService.Instance.GetClassToksAsync(queryValues, cancellationToken, fromCaller: callername);


            if (tokResult == null)
            {
                MainActivity.Instance.ShowLottieMessageDialog(RequireContext(), "Failed to load toks.", false);
            }
            else
            {
                if (tokResult.ContinuationToken == "cancelled")
                {
                    ShimmerLayout.Visibility = ViewStates.Gone;
                    showRetryDialog("Task was cancelled.");
                }
                else
                {
                    //Cache the result so that it will load automatically once app is opened
                    ClassService.Instance.SetCacheClassToksAsync(callername, tokResult.Results.ToList());

                    classTokModelsResult = tokResult.Results.ToList();
                    if (isSearchFragment)
                    {
                        Settings.SearchToksContinuationToken = tokResult.ContinuationToken;
                    }
                    else if (isClassGroup)
                    {
                        TaskRecyclerView.ContentDescription = tokResult.ContinuationToken;
                    }
                    else
                    {
                        Settings.HomeContinuationToken = tokResult.ContinuationToken;
                    }
                }
            }

            var Itemwithvalue = new List<ClassTokModel>();
            foreach (var item in classTokModelsResult) {
                var reactionValue = await ReactionService.Instance.GetReactionsValueAsync(item.Id);
                if (reactionValue != null) {
                    item.ViewsModel = reactionValue.ViewsModel;
                    Itemwithvalue.Add(item);
                }
            }

            return Itemwithvalue;
        }

        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(MainActivity.Instance)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {
                                MainActivity.Instance.hideBlueLoading(MainActivity.Instance);
                            })
                            .SetNegativeButton("Retry", async (_, args) =>
                            {
                                MainActivity.Instance.showBlueLoading(MainActivity.Instance);
                                await InitializeData();
                                MainActivity.Instance.hideBlueLoading(MainActivity.Instance);
                            })
                            .SetCancelable(false)
                            .Show();
        }
        public async Task LoadMoreData()
        {
            isLoadingMoreItems = true;
            var continuationToken = "";
            if (isSearchFragment)
            {
                continuationToken = Settings.SearchToksContinuationToken;
            }
            if (isClassGroup)
            {
                continuationToken = TaskRecyclerView.ContentDescription;
            }
            else
            {
                continuationToken = Settings.HomeContinuationToken;
            }

            if (!string.IsNullOrEmpty(continuationToken))
            {
                var tokQueryModel = new ClassTokQueryValues();
                tokQueryModel.paginationid = continuationToken;
                //tokQueryModel.loadmore = "yes";
                tokQueryModel.partitionkeybase = "classtoks";
                tokQueryModel.text = ""; // filter;
                tokQueryModel.startswith = false;
                tokQueryModel.classtokmode = true;
          //      tokQueryModel.descending = Settings.SortByTitleAscending;
          //      tokQueryModel.orderby = Settings.SortByReferenceAscending;

                showBottomLoading();
               // var result = await ClassService.Instance.GetClassToksAsync(tokQueryModel);
                ResultData<ClassTokModel> result = new ResultData<ClassTokModel>();
                result.Results = new List<ClassTokModel>();

                GetClassToksRequest request = new GetClassToksRequest()
                {
                    QueryValues = tokQueryModel
                };
                result = await ClassService.Instance.GetClassToksAsync(request);

                //tokResult = await ClassService.Instance.GetClassToksAsync(queryValues, cancellationToken, fromCaller: TAG);
                if (result == null)
                    return;

                var resultList = result.Results.ToList();
                if (isSearchFragment)
                {
                    Settings.SearchToksContinuationToken = result.ContinuationToken;
                }
                else if (isClassGroup)
                {
                    TaskRecyclerView.ContentDescription = result.ContinuationToken;
                }
                else
                {
                    Settings.HomeContinuationToken = result.ContinuationToken;
                }
                hideBottomLoading();


                foreach (var item in resultList)
                {
                    if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                    {
                        bool isGetSection = false;
                        if (isSearchFragment)
                        {
                            if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                            {
                                isGetSection = true;
                            }
                        }
                        else
                        {
                            if (Settings.FilterToksHome == (int)FilterToks.Cards)
                            {
                                isGetSection = true;
                            }
                        }

                        if (isGetSection)
                        {
                            var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                            var getTokSections = getTokSectionsResult.Results;
                            item.Sections = getTokSections.ToArray();
                        }
                    }

                 

                    ClassTokCollection.Add(item);
                }

                if (isSearchFragment)
                {
                    if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                    {
                        SetClassTokRecyclerAdapter(resultList);
                    }
                    else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                    {
                        SetClassCardsRecyclerAdapter(resultList);
                    }
                }
                else
                {
                    if (Settings.FilterToksHome == (int)FilterToks.Toks)
                    {
                        SetClassTokRecyclerAdapter(resultList);
                    }
                    else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                    {
                        SetClassCardsRecyclerAdapter(resultList);
                    }
                }
            }

            isLoadingMoreItems = false;
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
                RefreshLayout.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //RefreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!isSearchFragment && !isClassGroup)
            {
                Settings.HomeContinuationToken = "";

                if (Barrel.Current.Exists(_fromCaller))
                {
                    Barrel.Current.Empty(_fromCaller);
                }
            }

            FilterType type = (FilterType)Settings.FilterTag;
            MainActivity.Instance.RunOnUiThread(async () => await InitializeData(isSwipeRefresh: true));
            //Thread.Sleep(1000);
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.HomePage || Settings.ActivityInt == (int)ActivityType.TokInfo) && (resultCode == -1))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    var tokModelstring = data.GetStringExtra("classtokModel");
                    if (tokModelstring != null)
                    {
                        classtokModel = JsonConvert.DeserializeObject<ClassTokModel>(tokModelstring);
                        RemoveClassTokCollection(classtokModel);
                    }
                }
                var isUpdated = data.GetBooleanExtra("isUpdated", false);
                if (isUpdated)
                {
                    var tokModelstring = data.GetStringExtra("classtokModel");
                    if (tokModelstring != null)
                    {
                        classtokModel = JsonConvert.DeserializeObject<ClassTokModel>(tokModelstring);
                        UpdateClassTokCollection(classtokModel);
                    }
                }
            } 
        
        }

        
        public ProgressBar progressbar => v.FindViewById<ProgressBar>(Resource.Id.progressbar);
        public RecyclerView TaskRecyclerView => v.FindViewById<RecyclerView>(Resource.Id.home_recyclerView);
        public ShimmerLayout ShimmerLayout => v.FindViewById<ShimmerLayout>(Resource.Id.home_shimmer_view_container);
        public SwipeRefreshLayout RefreshLayout => v.FindViewById<SwipeRefreshLayout>(Resource.Id.home_swiperefresh_ListToks);
        public ImageView ClassTokIconImg => v.FindViewById<ImageView>(Resource.Id.home_img_classtokicon);
        public TextView TextNothingFound => v.FindViewById<TextView>(Resource.Id.TextNothingFound);
        public AppCompatButton btnLoadNewToks => v.FindViewById<AppCompatButton>(Resource.Id.btnLoadNewToks);
    }
}