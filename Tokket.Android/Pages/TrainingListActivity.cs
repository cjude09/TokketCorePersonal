using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Flexbox;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Result = Tokket.Shared.Helpers.Result;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using AppResult = Android.App.Result;
using Google.Android.Material.FloatingActionButton;

namespace Tokket.Android
{
    [Activity(Label = "Trainings", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TrainingListActivity : BaseActivity
    {
        internal static TrainingListActivity Instance { get; private set; }
        ObservableCollection<OpportunityTok> TrainingCollection;
        GridLayoutManager mLayoutManager;
        string Token = string.Empty;
        int selectedItem = 0;
        const int ADDOPPORTUNITY_RESULT = 1111;
        CustomTileAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.traininglist_view);
            // Create your application here
            Instance = this;

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerTrainingList.SetLayoutManager(mLayoutManager);
            RecyclerTrainingList.ScrollChange += RecyclerScrollEvent;
            TrainingCollection = new ObservableCollection<OpportunityTok>();

            AddTrainingButton.Click += delegate
            {
                var nextActivity = new Intent(this, typeof(AddTrainingActivity));
                this.StartActivityForResult(nextActivity, ADDOPPORTUNITY_RESULT);
            };

            swipeRefreshRecycler.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            swipeRefreshRecycler.Refresh += RefreshLayout_Refresh;
            RunOnUiThread(async () => await Initialize());
            if (RecyclerTrainingList != null)
            {
                RecyclerTrainingList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerTrainingList.ContentDescription))
                    {
                        await Initialize(RecyclerTrainingList.ContentDescription);
                    }
                };

                RecyclerTrainingList.AddOnScrollListener(onScrollListener);

                RecyclerTrainingList.SetLayoutManager(mLayoutManager);
            }

            btnAll.Click += delegate
            {
                SearchView.QueryHint = "Search for Groups...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TrainingCollection.Clear();
                RunOnUiThread(async () => await Initialize());
            };

            btnCourses.Click += delegate
            {
                SearchView.QueryHint = "Search for courses...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TrainingCollection.Clear();
                RunOnUiThread(async () => await Initialize(groupKind: "course"));
            };

            btnPrograms.Click += delegate
            {
                SearchView.QueryHint = "Search for programs...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TrainingCollection.Clear();
                RunOnUiThread(async () => await Initialize(groupKind: "program"));
                //TextNothingFound.Text = "No available clubs.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnTutors.Click += delegate
            {
                SearchView.QueryHint = "Search for tutors...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TrainingCollection.Clear();
                RunOnUiThread(async () => await Initialize(groupKind: "tutor"));
                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnRemoveOpportunity.Click += delegate
            {
                btnRemoveOpportunity.Visibility = ViewStates.Gone;
                btnDelete.Visibility = ViewStates.Visible;
                btnCancelDelete.Visibility = ViewStates.Visible;
                adapter.isShowCheckBoxDelete(true);
            };

            btnCancelDelete.Click += delegate
            {
                CancelDelete();
            };

            btnDelete.Click += async (o, s) =>
            {
                await DeleteTrainingStart();
            };
        }

        private void CancelDelete()
        {
            btnRemoveOpportunity.Visibility = ViewStates.Visible;
            btnDelete.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;
            adapter.itemsCheckedList.Clear();
        }
        private async Task DeleteTrainingStart()
        {
            if (adapter.itemsCheckedList.Count() > 0)
            {
                showProgress();
            }

            var itemList = adapter.itemsCheckedList.ToArray();
            for (int i = 0; i < itemList.Length; i++)
            {
                var item = itemList[i];
                if (item.isCheck)
                {
                    txtProgress.Text = $"Deleting {i + 1} of {itemList.Count()}";
                    var result = await TokService.Instance.DeleteTokAsync(item.Id, item.PartitionKey);

                    if (result.ResultEnum == Result.Success)
                    {
                        TrainingCollection.Remove(item);

                        if (i == itemList.Length - 1)
                        {
                            SetRecycler();
                            CancelDelete();
                            await Task.Delay(1000);
                            hideProgress();
                        };
                    }
                }
            }
        }

        public void AddOpportunity(OpportunityTok opportunity)
        {
            TrainingCollection.Insert(0, opportunity);
            SetRecycler();
        }

        public void RemoveOpportunity(OpportunityTok opportunity)
        {

            TrainingCollection.RemoveAt(selectedItem);
            SetRecycler();

        }

        public void UpdateOpportunityItem(OpportunityTok opportunity)
        {
            TrainingCollection[selectedItem] = opportunity;
            SetRecycler();
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                //Data Refresh Place  
                //BackgroundWorker work = new BackgroundWorker();
                //work.DoWork += Work_DoWork;
                //work.RunWorkerCompleted += Work_RunWorkerCompleted;
                //work.RunWorkerAsync();
            }
            else
            {
                swipeRefreshRecycler.Refreshing = false;
            }
        }

        private void RecyclerScrollEvent(object sender, View.ScrollChangeEventArgs e)
        {
            var layout = (sender as RecyclerView).GetLayoutManager();
            var manager = layout as GridLayoutManager;
            int totalItem = manager.ItemCount;
            int lastVisible = manager.FindLastVisibleItemPosition();
            bool endHasBeenReached = lastVisible + 1 >= totalItem;
            //if (lastVisible > 0 && endHasBeenReached)
            //{
            //    LimitReminder.Visibility = ViewStates.Visible;
            //}
            //else
            //{
            //    LimitReminder.Visibility = ViewStates.Gone;
            //}
        }


        private async Task Initialize(string pagination_id = "", string groupKind = "")
        {
            if (string.IsNullOrEmpty(pagination_id))
            {
                shimmerLayout.StartShimmerAnimation();
                shimmerLayout.Visibility = ViewStates.Visible;
                RecyclerTrainingList.Visibility = ViewStates.Invisible;
            }

            ResultData<OpportunityTok> results = new ResultData<OpportunityTok>();
            var queryValues = new TokQueryValues();
            queryValues.token = Token;
            queryValues.loadmore = "yes";
            // queryValues.userid = Settings.GetTokketUser().Id;
            queryValues.training_tok = "true";
            queryValues.training_type = groupKind;
            var getopportunity = await TokService.Instance.GetOpportunitiesAsync(queryValues, "tokket");
            if (getopportunity == null)
                results.Results = new List<OpportunityTok>();
            else
                results = getopportunity;
            int lastposition = 0;
            if (!string.IsNullOrEmpty(pagination_id))
            {
                lastposition = RecyclerTrainingList.ChildCount - 1;
                showBottomDialog();
            }

            if (!string.IsNullOrEmpty(pagination_id))
            {
                hideBottomDialog();
            }

            RecyclerTrainingList.ContentDescription = results.ContinuationToken;
            var opportunity = results.Results.Where(x => string.IsNullOrEmpty(x.OpportunityType) && !string.IsNullOrEmpty(x.Type)).ToList();

            if (string.IsNullOrEmpty(groupKind))
            {
                foreach (var item in opportunity)
                {

                    TrainingCollection.Add(item);
                }

                /*foreach (var item in classgroupResult)
                {
                    ClassGroupCollection.Add(item);
                }*/

            }
            else
            {
                foreach (var item in opportunity)
                {

                    TrainingCollection.Add(item);
                }
            }

            RecyclerTrainingList.Visibility = ViewStates.Visible;
            shimmerLayout.Visibility = ViewStates.Gone;

            SetRecycler();

            if (!string.IsNullOrEmpty(pagination_id))
            {
                RecyclerTrainingList.ScrollToPosition(lastposition);
            }
            if (TrainingCollection.Count == 0)
            {
                switch (groupKind)
                {
                    case "course":
                        TextNothingFound.Text = "No available courses.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "program":
                        TextNothingFound.Text = "No available programs.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "tutor":
                        TextNothingFound.Text = "No available tutors.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    default:
                        TextNothingFound.Text = "No available trainings.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                }
            }
        }
        public void AddTrainingCollection(OpportunityTok item, bool isSave = true)
        {
            if (isSave == false)
            {
                var result = TrainingCollection.FirstOrDefault(c => c.Id == item.Id);
                if (result != null) //If Edit
                {
                    int ndx = TrainingCollection.IndexOf(result);
                    TrainingCollection.Remove(result);

                    TrainingCollection.Insert(ndx, item);
                }
            }
            else
            {
                TrainingCollection.Insert(0, item);
            }
            SetRecycler();
        }

        public void RemoveClassGroupCollection(ClassGroupModel item)
        {
            var collection = TrainingCollection.FirstOrDefault(a => a.Id == item.Id);
            if (collection != null) //If item exist
            {
                int ndx = TrainingCollection.IndexOf(collection); //Get index
                TrainingCollection.Remove(collection);

                SetRecycler();
            }
        }
        void SetRecycler()
        {
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            RecyclerTrainingList.SetLayoutManager(new GridLayoutManager(this, numcol));
            adapter = new CustomTileAdapter(TrainingCollection.ToList()) { context = this };
            adapter.ItemClick -= OnGridBackgroundClick;
            adapter.ItemClick += OnGridBackgroundClick;
            RecyclerTrainingList.SetAdapter(adapter);
        }

        private void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;

            Intent nextActivity = new Intent(MainActivity.Instance, typeof(OpportunityInfoActivity));
            selectedItem = position;
            var classtokModel = TrainingCollection[position];
            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("opportunityTok", modelConvert);

            this.StartActivityForResult(nextActivity, (int)ActivityType.OpportunityActivity);
        }
        private void showBottomDialog()
        {
            bottomProgress.Visibility = ViewStates.Visible;
        }
        private void hideBottomDialog()
        {
            bottomProgress.Visibility = ViewStates.Gone;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_group, menu);

            SearchManager searchManager = (SearchManager)GetSystemService(Context.SearchService);
            SearchView = (SearchView)menu.FindItem(Resource.Id.menu_search).ActionView;
            SearchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            SearchView.QueryHint = "Search...";
            SearchView.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                this.RunOnUiThread(async () => await SearchGroup(searchText: e.NewText));
                e.Handled = true;
            };

            return base.OnCreateOptionsMenu(menu);
        }
        private async Task SearchGroup(string searchText)
        {
            showBottomDialog();
            TrainingCollection.Clear();
            var result = await TokService.Instance.GetOpportunitiesAsync(new TokQueryValues()
            {
                text = searchText,
                startswith = false
            }, "tokket");
            //var classgroupResult = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //{
            //    partitionkeybase = "classgroups",
            //    startswith = false,
            //    text = searchText
            //});

            foreach (var item in result.Results.Where(x => string.IsNullOrEmpty(x.OpportunityType) && !string.IsNullOrEmpty(x.Type)).ToList())
            {
                TrainingCollection.Add(item);
            }

            hideBottomDialog();
            SetRecycler();
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.btnFilter:
                    //var nextActivity = new Intent(this, typeof(FilterActivity));
                    //nextActivity.PutExtra("activitycaller", "Home");
                    //StartActivityForResult(nextActivity, (int)ActivityName.Filter);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] AppResult resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ADDOPPORTUNITY_RESULT && resultCode == AppResult.Ok && data != null)
            {
                var datas = JsonConvert.DeserializeObject<OpportunityTok>(data.GetStringExtra("TrainingData"));
                AddOpportunity(datas);
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TextNothingFound.Visibility = ViewStates.Gone;
            }
        }
        private void showProgress()
        {
            linearProgress.Visibility = ViewStates.Visible;
            Window.AddFlags(WindowManagerFlags.NotTouchable);
        }
        private void hideProgress()
        {
            linearProgress.Visibility = ViewStates.Gone;
            Window.ClearFlags(WindowManagerFlags.NotTouchable);
        }
        public ShimmerLayout shimmerLayout => FindViewById<ShimmerLayout>(Resource.Id.shimmerLayout);
        public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgress);
        public FloatingActionButton AddTrainingButton => FindViewById<FloatingActionButton>(Resource.Id.fab_addTraining);
        public RecyclerView RecyclerTrainingList => FindViewById<RecyclerView>(Resource.Id.RecyclerTrainingList);
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbar);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideText);
        public SwipeRefreshLayout swipeRefreshRecycler => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecycler);
        public Button btnCourses => FindViewById<Button>(Resource.Id.btnCourses);
        public Button btnPrograms => FindViewById<Button>(Resource.Id.btnPrograms);
        public Button btnTutors => FindViewById<Button>(Resource.Id.btnTutors);

        public Button btnAll => FindViewById<Button>(Resource.Id.btnAll);
        public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFound);
        public LinearLayout LimitReminder => FindViewById<LinearLayout>(Resource.Id.limitStar);

        public FlexboxLayout headerButtons => FindViewById<FlexboxLayout>(Resource.Id.FlexHeaderButtons);
        SearchView SearchView;

        public TextView btnRemoveOpportunity => FindViewById<TextView>(Resource.Id.btnRemoveOpportunity);
        public TextView btnDelete => FindViewById<TextView>(Resource.Id.btnDelete);
        public TextView btnCancelDelete => FindViewById<TextView>(Resource.Id.btnCancelDelete);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView txtProgress => FindViewById<TextView>(Resource.Id.txtProgress);
    }
}