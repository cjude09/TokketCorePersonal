using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services;
using Tokket.Android.ViewHolders;
using Xamarin.Essentials;
using Result = Tokket.Shared.Helpers.Result;
using Android.Widget;

namespace Tokket.Android
{
    [Activity(Label = "Peerbook" ,Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class YearbookActivity : BaseActivity
    {
        internal static YearbookActivity Instance;
        public event EventHandler<int> ItemClick;
        ObservableCollection<YearbookTok> YearbookCollection = new ObservableCollection<YearbookTok>();
        YearbookHolder vh;
        string Token = string.Empty;
        YearbookAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            SetContentView(Resource.Layout.yearbook_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            AddYearbookFAB.Click += (obj, _event) => {
                var intent = new Intent(this, typeof(AddYearbookActivity));
                StartActivity(intent);
            };

            YearbookSwipeLayout.Refresh += delegate {
                Init();
                YearbookSwipeLayout.Refreshing = false;
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
                await DeleteStart();
            };

            Init();
           
        }
        private async Task DeleteStart()
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
                    txtProgress.Text = $"Deleting {i + 1} of {itemList.Length}...";
                    var result = await TokService.Instance.DeleteTokAsync(item.Id, item.PartitionKey);

                    if (result.ResultEnum == Result.Success)
                    {

                        YearbookCollection.Remove(item);

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
        private void CancelDelete()
        {
            btnRemoveOpportunity.Visibility = ViewStates.Visible;
            btnDelete.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;
            adapter.itemsCheckedList.Clear();
        }

        public void AddYearbook(YearbookTok yearbook) {
            YearbookCollection.Insert(0,yearbook);
            SetRecycler();
        }

        public void RemoveYearbook(YearbookTok yearbook) {
           
            YearbookCollection.Remove(yearbook);
            SetRecycler();
        }

        private async void Init(string search = "") {
            Shimmerlayout.StartShimmerAnimation();
            YearbookCollection.Clear();
            var queryValues = new TokQueryValues();
            queryValues.token = string.Empty;
            queryValues.loadmore = "yes";
            queryValues.userid = Settings.GetTokketUser().Id;
            queryValues.yearbook_tiletype = "";
            queryValues.yearbook_type = "";
            queryValues.text = search;
            //queryValues.yearbook_timing = search;
            //queryValues.yearbook_type = search;
            //queryValues.yearbook_tiletype = search;
            //queryValues.yearbook_schoolname = search;
            //queryValues.yearbook_grouptype = search;
            
            queryValues.startswith = false;
            var results = await TokService.Instance.GetYearbooksAsync(queryValues,"tokket");
            if (results != null) {
                Token = string.IsNullOrEmpty(results.ContinuationToken) ? "" : results.ContinuationToken;
                foreach (var books in results.Results)
                {
                    var json = JsonConvert.SerializeObject(books);
                    var hasDup = YearbookCollection.Where(s => s.Id == books.Id).FirstOrDefault() == null;
                    if (hasDup) {
                        YearbookCollection.Add(books);
                    }
                  
                }
            }
            SetRecycler();
            Shimmerlayout.Visibility = ViewStates.Gone;
        }

     
        void SetRecycler() {
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            YearbookRecycler.SetLayoutManager(new GridLayoutManager(this, numcol));
            adapter = new YearbookAdapter(YearbookCollection.ToList()) { context = this };
            adapter.ItemClick -= OnGridBackgroundClick;
            adapter.ItemClick += OnGridBackgroundClick;
            YearbookRecycler.SetAdapter(adapter);
        }

        private void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;

            Intent nextActivity = new Intent(MainActivity.Instance, typeof(YearbookInfoActivity));

          var  classtokModel = YearbookCollection[position];
            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("yearbookTok", modelConvert);
          
            this.StartActivityForResult(nextActivity, (int)ActivityType.YearbookActivity);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_group, menu);

            SearchManager searchManager = (SearchManager)GetSystemService(Context.SearchService);
            SearchView = (AndroidX.AppCompat.Widget.SearchView)menu.FindItem(Resource.Id.menu_search).ActionView;
            SearchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            SearchView.QueryHint = "Search...";
            SearchView.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
               this.RunOnUiThread( () =>  Init(search: e.NewText));
                e.Handled = true;
            };

            return base.OnCreateOptionsMenu(menu);
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

        #region Properties
        FloatingActionButton AddYearbookFAB => FindViewById<FloatingActionButton>(Resource.Id.fab_addyearbook);

        ShimmerLayout Shimmerlayout => FindViewById<ShimmerLayout>(Resource.Id.yearbook_shimmer_view_container);

        SwipeRefreshLayout YearbookSwipeLayout => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh_yearbook);

        RecyclerView YearbookRecycler => FindViewById<RecyclerView>(Resource.Id.recyclerView_yearbook);
        AndroidX.AppCompat.Widget.SearchView SearchView;

        public TextView btnRemoveOpportunity => FindViewById<TextView>(Resource.Id.btnRemoveOpportunity);
        public TextView btnDelete => FindViewById<TextView>(Resource.Id.btnDelete);
        public TextView btnCancelDelete => FindViewById<TextView>(Resource.Id.btnCancelDelete);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView txtProgress => FindViewById<TextView>(Resource.Id.txtProgress);
        #endregion
    }
}