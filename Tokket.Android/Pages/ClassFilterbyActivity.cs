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
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "Filter By", Theme = "@style/Theme.AppCompat.Light.Dialog.NoTitle")]
    public class ClassFilterbyActivity : BaseActivity
    {
        string filterby = "", currentFilterByItems = "", caller = "home"; FilterBy filterByEnum = FilterBy.None;
        CancellationToken cancellationToken; TaskCompletionSource<bool>  taskCompletionSource;
        internal static ClassFilterbyActivity Instance { get; private set; }
        private bool isRecent = true;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_class_filterby);

            Instance = this;
            currentFilterByItems = Intent.GetStringExtra("filterByItems");
            filterby = Intent.GetStringExtra("filterby");
            caller = Intent.GetStringExtra("caller");

            var layoutManager = new LinearLayoutManager(this);
            recyclerFilterBy.SetLayoutManager(layoutManager);

            taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });

            RunOnUiThread(async() => await Initialize());

            rbtnRecent.CheckedChange += delegate
            {
                isRecent = rbtnRecent.Checked;
                RunOnUiThread(async () => await Initialize());
            };

            btnCancel.Click += delegate
            {
                taskCompletionSource.SetCanceled();
                this.Finish();
            };

            btnApplyFilter.Click += delegate
            {
                if (linearProgress.Visibility != ViewStates.Visible)   //Only apply if progress is not visible
                {
                    Intent = new Intent();
                    Intent.PutExtra("filterby", (int)filterByEnum);
                    Intent.PutExtra("filterByList", btnApplyFilter.ContentDescription);
                    SetResult(AppResult.Ok, Intent);
                    Finish();
                }
            };

            swipeRefreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            swipeRefreshLayout.Refresh -= RefreshLayout_Refresh;
            swipeRefreshLayout.Refresh += RefreshLayout_Refresh;
        }
        private async Task Initialize()
        {
            txtProgressText.Text = "Loading...";
            linearProgress.Visibility = ViewStates.Visible;
            //this.Window.AddFlags(WindowManagerFlags.NotTouchable);       // Comment this one out so that user can cancel

            var listCommonModel = await GetMoreFilterOptions();
                                
            int typePosition = -1;
            if (filterByEnum == FilterBy.Type)
            {
                string itemType = "";
                if (caller.ToLower() == "home")
                {
                    itemType = Settings.FilterByTypeSelectedHome;
                }
                else if (caller.ToLower() == "search")
                {
                    itemType = Settings.FilterByTypeSelectedSearch;
                }
                else if (caller.ToLower() == "profile")
                {
                    itemType = Settings.FilterByTypeSelectedProfile;
                }

                for (int i = 0; i < listCommonModel.Count; i++)
                {
                    if (listCommonModel[i].Title == itemType)
                    {
                        typePosition = i;
                    }
                }
            }

            var adapterClassFilter = new ClassFilterByAdapter(this, listCommonModel, (int)filterByEnum, typePosition, caller);
            recyclerFilterBy.SetAdapter(adapterClassFilter);

            linearProgress.Visibility = ViewStates.Gone;
            //this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
        }
        public async Task<List<CommonModel>> GetMoreFilterOptions()
        {
            filterByEnum = FilterBy.None;
            string cachedListCommonModel = string.Empty;
            switch (filterby.ToLower())
            {
                case "class":
                    filterByEnum = FilterBy.Class;
                    cachedListCommonModel = Settings.classFilterByClass;
                    break;
                case "category":
                    filterByEnum = FilterBy.Category;
                    cachedListCommonModel = Settings.classFilterByCategory;
                    break;
                case "type":
                    filterByEnum = FilterBy.Type;
                    break;
                default:
                    filterByEnum = FilterBy.None;
                    break;
            }

            List<CommonModel> listCommonModel = new List<CommonModel>();

            if (string.IsNullOrEmpty(cachedListCommonModel))
            {
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                    cancellationToken = cancellationTokenSource.Token;

                    var result = await ClassService.Instance.GetMoreFilterOptions(new ClassTokQueryValues()
                    {
                        FilterBy = filterByEnum,
                        RecentOnly = rbtnRecent.Checked
                    }, cancellationToken);

                    if (result.Results != null)
                    {
                        listCommonModel = result.Results.ToList();
                    }

                    switch (filterByEnum)
                    {
                        case FilterBy.None:
                            break;
                        case FilterBy.Class:
                            Settings.classFilterByClass = JsonConvert.SerializeObject(listCommonModel);
                            break;
                        case FilterBy.Category:
                            Settings.classFilterByCategory = JsonConvert.SerializeObject(listCommonModel);
                            break;
                        case FilterBy.Type:
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                listCommonModel = JsonConvert.DeserializeObject<List<CommonModel>>(cachedListCommonModel);

                //Load default selected items
                List<string> defaultList = new List<string>();
                if (caller.ToUpper() == "HOME")
                {
                    defaultList = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsHome);
                }
                else if (caller.ToUpper() == "SEARCH")
                {
                    defaultList = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsSearch);
                }
                else if (caller.ToUpper() == "PROFILE")
                {
                    defaultList = JsonConvert.DeserializeObject<List<string>>(Settings.FilterByItemsProfile);
                }
                else
                {
                    defaultList = JsonConvert.DeserializeObject<List<string>>(currentFilterByItems);
                }

                if (defaultList != null)
                {
                    foreach (var item in defaultList)
                    {
                        switch (filterByEnum)
                        {
                            case FilterBy.Class:
                                var resultClass = listCommonModel.FirstOrDefault(c => c.Id == item);
                                if (resultClass != null)
                                {
                                    int ndx = listCommonModel.IndexOf(resultClass);
                                    listCommonModel[ndx].isSelected = true;
                                }
                                break;
                            case FilterBy.Category:
                                var resultCategory = listCommonModel.FirstOrDefault(c => c.Id == item);
                                if (resultCategory != null)
                                {
                                    int ndx = listCommonModel.IndexOf(resultCategory);
                                    listCommonModel[ndx].isSelected = true;
                                }
                                break;
                            case FilterBy.Type:
                                var resultType = listCommonModel.FirstOrDefault(c => c.Title == item);
                                if (resultType != null)
                                {
                                    int ndx = listCommonModel.IndexOf(resultType);
                                    listCommonModel[ndx].isSelected = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                //End of loading default selected items
            }


            // In case user will do a refresh
            if (!string.IsNullOrEmpty(btnApplyFilter.ContentDescription))
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(btnApplyFilter.ContentDescription);
                foreach (var item in selectedList)
                {
                    switch (filterByEnum)
                    {
                        case FilterBy.Class:
                            var resultClass = listCommonModel.FirstOrDefault(c => c.Id == item);
                            if (resultClass != null)
                            {
                                int ndx = listCommonModel.IndexOf(resultClass);
                                listCommonModel[ndx].isSelected = true;
                            }
                            break;
                        case FilterBy.Category:
                            var resultCategory = listCommonModel.FirstOrDefault(c => c.Id == item);
                            if (resultCategory != null)
                            {
                                int ndx = listCommonModel.IndexOf(resultCategory);
                                listCommonModel[ndx].isSelected = true;
                            }
                            break;
                        case FilterBy.Type:
                            var resultType = listCommonModel.FirstOrDefault(c => c.Title == item);
                            if (resultType != null)
                            {
                                int ndx = listCommonModel.IndexOf(resultType);
                                listCommonModel[ndx].isSelected = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!isRecent)
            {
                listCommonModel = listCommonModel.OrderBy(x => x.Title).ToList();
            }
            return listCommonModel;
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
            else
            {
                swipeRefreshLayout.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipeRefreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            RunOnUiThread(async () => await Initialize());
            Thread.Sleep(1000);
        }

        private RadioButton rbtnRecent => FindViewById<RadioButton>(Resource.Id.rbtnRecent);
        private RadioButton rbtnAZ => FindViewById<RadioButton>(Resource.Id.rbtnAZ);
        private RecyclerView recyclerFilterBy => FindViewById<RecyclerView>(Resource.Id.recyclerFilterBy);
        private Button btnCancel => FindViewById<Button>(Resource.Id.rbtnCancel);
        public Button btnApplyFilter => FindViewById<Button>(Resource.Id.rbtnApplyFilter);
        private LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        private TextView txtProgressText => FindViewById<TextView>(Resource.Id.txtProgressText);
        private SwipeRefreshLayout swipeRefreshLayout => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
    }
}