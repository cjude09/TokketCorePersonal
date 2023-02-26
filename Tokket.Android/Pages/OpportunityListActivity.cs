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
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
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
using AndroidX.AppCompat.Widget;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using AppResult = Android.App.Result;
using Google.Android.Material.FloatingActionButton;
using Google.Flexbox;

namespace Tokket.Android
{
    [Activity(Label = "Opportunities", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class OpportunityListActivity : BaseActivity
    {
        internal static OpportunityListActivity Instance { get; private set; }
        ObservableCollection<OpportunityTok> OpportunityCollection;
        GridLayoutManager mLayoutManager;
        string Token = string.Empty;
        int selectedItem = 0;
        CustomTileAdapter adapter;
        const int ADDOPPORTUNITY_RESULT = 1111;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.oppotunities_view);

            Instance = this;

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerOpportunityList.SetLayoutManager(mLayoutManager);
            RecyclerOpportunityList.ScrollChange += RecyclerScrollEvent;
            OpportunityCollection = new ObservableCollection<OpportunityTok>();

            AddOpportunityButton.Click += delegate
            {
                var nextActivity = new Intent(this, typeof(AddOpportunityActivity));
                this.StartActivityForResult(nextActivity,ADDOPPORTUNITY_RESULT);
            };

            swipeRefreshRecycler.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            swipeRefreshRecycler.Refresh += RefreshLayout_Refresh;
            RunOnUiThread(async () => await Initialize());
            if (RecyclerOpportunityList != null)
            {
                RecyclerOpportunityList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerOpportunityList.ContentDescription))
                    {
                        await Initialize(RecyclerOpportunityList.ContentDescription);
                    }
                };

                RecyclerOpportunityList.AddOnScrollListener(onScrollListener);

                RecyclerOpportunityList.SetLayoutManager(mLayoutManager);
            }

            btnAll.Click += delegate
            {
                SearchView.QueryHint = "Search for Groups...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                OpportunityCollection.Clear();
               RunOnUiThread(async () => await Initialize());
            };

            btnJob.Click += delegate
            {
                SearchView.QueryHint = "Search for jobs...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                OpportunityCollection.Clear();
               RunOnUiThread(async () => await Initialize(groupKind: "job"));
            };

            btnInternship.Click += delegate
            {
                SearchView.QueryHint = "Search for interships...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                OpportunityCollection.Clear();
               RunOnUiThread(async () => await Initialize(groupKind: "internship"));
                //TextNothingFound.Text = "No available clubs.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnSholarship.Click += delegate
            {
                SearchView.QueryHint = "Search for scholarship...";
                TextNothingFound.Visibility = ViewStates.Gone;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                OpportunityCollection.Clear();
                RunOnUiThread(async () => await Initialize(groupKind: "scholarship"));
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
                await DeleteTokPakStart();
            };
        }

        private async Task DeleteTokPakStart()
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
                    txtProgress.Text = $"Deleting {i + 1} of {adapter.itemsCheckedList.Count()}.";
                    var result = await TokService.Instance.DeleteTokAsync(item.Id, item.PartitionKey);

                    if (result.ResultEnum == Result.Success)
                    {
                        OpportunityCollection.Remove(item);

                        if (i == (itemList.Length - 1))
                        {
                            SetRecycler();
                            CancelDelete();
                            hideProgress();
                        };
                    }
                }
            }
        }
        private void CancelDelete()
        {
            btnRemoveOpportunity.Visibility = ViewStates.Visible;
            btnDelete.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;
            adapter.itemsCheckedList.Clear();
        }
        public void AddOpportunity(OpportunityTok opportunity)
        {
            OpportunityCollection.Insert(0, opportunity);
            SetRecycler();
        }

        public void RemoveOpportunity(OpportunityTok opportunity)
        {
            OpportunityCollection.RemoveAt(selectedItem);
            SetRecycler();
        }

        public void UpdateOpportunityItem(OpportunityTok opportunity) {
            OpportunityCollection[selectedItem] = opportunity;
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
                RecyclerOpportunityList.Visibility = ViewStates.Invisible;
            }

            ResultData<OpportunityTok> results = new ResultData<OpportunityTok>();
            var queryValues = new TokQueryValues();
            queryValues.token = Token;
            queryValues.loadmore = "yes";
           // queryValues.userid = Settings.GetTokketUser().Id;
            queryValues.opportunity_type = groupKind;
            var getopportunity = await TokService.Instance.GetOpportunitiesAsync(queryValues, "tokket");
            if (getopportunity == null)
                results.Results = new List<OpportunityTok>();
            else
                results = getopportunity;
            int lastposition = 0;
            if (!string.IsNullOrEmpty(pagination_id))
            {
                lastposition = RecyclerOpportunityList.ChildCount - 1;
                showBottomDialog();
            }
         
            if (!string.IsNullOrEmpty(pagination_id))
            {
                hideBottomDialog();
            }
            
            RecyclerOpportunityList.ContentDescription = results.ContinuationToken;
            var opportunity = results.Results.Where(x => !string.IsNullOrEmpty(x.OpportunityType) && string.IsNullOrEmpty(x.Type)).ToList();

            if (string.IsNullOrEmpty(groupKind))
            {
                foreach (var item in opportunity)
                {

                    OpportunityCollection.Add(item);
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
                
                        OpportunityCollection.Add(item);
                }
            }

            RecyclerOpportunityList.Visibility = ViewStates.Visible;
            shimmerLayout.Visibility = ViewStates.Gone;

            SetRecycler();

            if (!string.IsNullOrEmpty(pagination_id))
            {
                RecyclerOpportunityList.ScrollToPosition(lastposition);
            }
            if (OpportunityCollection.Count == 0)
            {
                switch (groupKind)
                {
                    case "job":
                        TextNothingFound.Text = "No available jobs.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "scholarship":
                        TextNothingFound.Text = "No available scholarships.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "internship":
                        TextNothingFound.Text = "No available internships.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    default:
                        TextNothingFound.Text = "No available opportunities.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                }
            }
        }

        public void AddOpportunityCollection(OpportunityTok item, bool isSave = true)
        {
            if (isSave == false)
            {
                var result = OpportunityCollection.FirstOrDefault(c => c.Id == item.Id);
                if (result != null) //If Edit
                {
                    int ndx = OpportunityCollection.IndexOf(result);
                    OpportunityCollection.Remove(result);

                    OpportunityCollection.Insert(ndx, item);
                }
            }
            else
            {
                OpportunityCollection.Insert(0, item);
            }
            SetRecycler();
        }
        public void RemoveClassGroupCollection(ClassGroupModel item)
        {
            var collection = OpportunityCollection.FirstOrDefault(a => a.Id == item.Id);
            if (collection != null) //If item exist
            {
                int ndx = OpportunityCollection.IndexOf(collection); //Get index
                OpportunityCollection.Remove(collection);

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
            RecyclerOpportunityList.SetLayoutManager(new GridLayoutManager(this, numcol));
            adapter = new CustomTileAdapter(OpportunityCollection.ToList()) { context = this };
            adapter.ItemClick -= OnGridBackgroundClick;
            adapter.ItemClick += OnGridBackgroundClick;
            RecyclerOpportunityList.SetAdapter(adapter);
        }

        private void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;

            Intent nextActivity = new Intent(MainActivity.Instance, typeof(OpportunityInfoActivity));
            selectedItem = position;
            var classtokModel = OpportunityCollection[position];
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
            OpportunityCollection.Clear();
          var result =  await TokService.Instance.GetOpportunitiesAsync(new TokQueryValues() { 
          text = searchText,
          startswith = false
          }, "tokket");
            //var classgroupResult = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //{
            //    partitionkeybase = "classgroups",
            //    startswith = false,
            //    text = searchText
            //});

            foreach (var item in result.Results.Where(x => !string.IsNullOrEmpty(x.OpportunityType) && string.IsNullOrEmpty(x.Type)).ToList())
            {
                OpportunityCollection.Add(item);
            }

            hideBottomDialog();
            SetRecycler();
        }

        void HideHeaderButtons() { 
        
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
                var datas = JsonConvert.DeserializeObject<OpportunityTok>(data.GetStringExtra("OppData"));
                AddOpportunity(datas);
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                TextNothingFound.Visibility = ViewStates.Gone;
            }
        }
        public ShimmerLayout shimmerLayout => FindViewById<ShimmerLayout>(Resource.Id.shimmerLayout);
        public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgress);
        public FloatingActionButton AddOpportunityButton => FindViewById<FloatingActionButton>(Resource.Id.fab_addOpportunity);
        public RecyclerView RecyclerOpportunityList => FindViewById<RecyclerView>(Resource.Id.RecyclerOpportunityList);
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbar);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideText);
        public SwipeRefreshLayout swipeRefreshRecycler => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecycler);
        public Button btnJob => FindViewById<Button>(Resource.Id.btnJob);
        public Button btnSholarship => FindViewById<Button>(Resource.Id.btnScholarship);
        public Button btnInternship => FindViewById<Button>(Resource.Id.btnIntership);

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