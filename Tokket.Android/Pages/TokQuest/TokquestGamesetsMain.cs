using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Game sets", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokquestGamesetsMain : BaseActivity
    {
        internal static TokquestGamesetsMain Instance { get; private set; }
        ObservableCollection<gameObject> GameSetsCollection;
        GridLayoutManager mLayoutManager;
        private enum ActivityName
        {
            Filter = 1001
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.tokquest_gamesets);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Settings.ActivityInt = (int)ActivityType.TokquestGamesetsMain;

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerGamesetList.SetLayoutManager(mLayoutManager);

            GameSetsCollection = new ObservableCollection<gameObject>();
            RunOnUiThread(async () => await Initialize());

            //swipeRefreshRecyclerGameset.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            //swipeRefreshRecyclerGameset.Refresh += RefreshLayout_Refresh;

            //if (RecyclerGamesetList != null)
            //{
            //    RecyclerGamesetList.HasFixedSize = true;

            //    var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
            //    onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
            //    {
            //        if (!string.IsNullOrEmpty(RecyclerGamesetList.ContentDescription))
            //        {
            //            await Initialize(RecyclerGamesetList.ContentDescription);
            //        }
            //    };

            //    RecyclerGamesetList.AddOnScrollListener(onScrollListener);

            //    RecyclerGamesetList.SetLayoutManager(mLayoutManager);
            //}

            
        }

        private void TextGamesetPlay_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            Intent nextActivity = new Intent(this, typeof(GameCategoryMain));
            this.StartActivity(nextActivity);
            Console.WriteLine(btn.ContentDescription);

        }


        private void showBottomDialog()
        {
            bottomProgressGameset.Visibility = ViewStates.Visible;
        }
        private void hideBottomDialog()
        {
            bottomProgressGameset.Visibility = ViewStates.Gone;
        }
        private async Task Initialize(string pagination_id = "")
        {
            ResultData<ClassGroupModel> results = new ResultData<ClassGroupModel>();
            int lastposition = 0;
            if (!string.IsNullOrEmpty(pagination_id))
            {
                lastposition = RecyclerGamesetList.ChildCount - 1;
                showBottomDialog();
            }
            
            if (!string.IsNullOrEmpty(pagination_id))
            {
                hideBottomDialog();
            }


            var gameObject = JsonConvert.DeserializeObject<List<gameObject>>(Intent.GetStringExtra("Gamesets"));
            //RecyclerGamesetList.ContentDescription = results.ContinuationToken;
            var classgroupResult = gameObject;

            GameSetsCollection.Clear();
            foreach (var item in classgroupResult)
            {
                GameSetsCollection.Add(item);
            }

            SetRecyclerAdapter();

            if (!string.IsNullOrEmpty(pagination_id))
            {
                RecyclerGamesetList.ScrollToPosition(lastposition);
            }
        }
        
        private void SetRecyclerAdapter()
        {
            var adapterClassGroup = GameSetsCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.gameset_list_row);
            RecyclerGamesetList.SetAdapter(adapterClassGroup);

            linearProgress.Visibility = ViewStates.Invisible;
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, gameObject model, int position)
        {
            var ClassGroupBody = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupBody);
            var ClassGroupFooter = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupFooter);
            var btnPlay = holder.FindCachedViewById<Button>(Resource.Id.TextGamesetPlay);
            btnPlay.Click += BtnPlay_Click;

            ClassGroupBody.Text = model.GameName;
            ClassGroupFooter.Text = "Last updated " + DateConvert.ConvertToRelative(model.CreatedTime);
            btnPlay.ContentDescription = model.Id + "==" + model.PartitionKey + "===" + model.GameListObject.Count();
         
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            var btnPlay = sender as Button;
            Intent nextActivity = new Intent(this, typeof(GameCategoryMain));
            //var modelConvert = JsonConvert.SerializeObject(ClassGroupCollection[position]);
            nextActivity.PutExtra("gameplayOptions", btnPlay.ContentDescription);
            this.StartActivity(nextActivity);
            Console.WriteLine(btnPlay.ContentDescription);

        }

        [Java.Interop.Export("ItemRowClicked")]
        public async void ItemRowClicked(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }
        
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Clear();
            return base.OnCreateOptionsMenu(menu);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.btnFilter:
                    var nextActivity = new Intent(this, typeof(FilterActivity));
                    nextActivity.PutExtra("activitycaller", "Home");
                    StartActivityForResult(nextActivity, (int)ActivityName.Filter);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        //private void RefreshLayout_Refresh(object sender, EventArgs e)
        //{
        //    var current = Connectivity.NetworkAccess;

        //    if (current == NetworkAccess.Internet)
        //    {
        //        //Data Refresh Place  
        //        BackgroundWorker work = new BackgroundWorker();
        //        work.DoWork += Work_DoWork;
        //        work.RunWorkerCompleted += Work_RunWorkerCompleted;
        //        work.RunWorkerAsync();
        //    }
        //    else
        //    {
        //        swipeRefreshRecyclerGameset.Refreshing = false;
        //    }
        //}
        //private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    swipeRefreshRecyclerGameset.Refreshing = false;
        //}
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            this.RunOnUiThread(async () => await Initialize());
            Thread.Sleep(1000);
        }

        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);

        public ProgressBar bottomProgressGameset => FindViewById<ProgressBar>(Resource.Id.bottomProgressGameset1);

        public RecyclerView RecyclerGamesetList => FindViewById<RecyclerView>(Resource.Id.RecyclerGamesetList);
        public ProgressBar progressbargameset => FindViewById<ProgressBar>(Resource.Id.progressbargameset);
        public TextView progressBarinsideTextGameset => FindViewById<TextView>(Resource.Id.progressBarinsideTextGameset);
       
        //public SwipeRefreshLayout swipeRefreshRecyclerGameset => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerGameset);
        
        public TextView TextNothingFoundGameset => FindViewById<TextView>(Resource.Id.TextNothingFoundGameset);
      

    }
}



//gameObject = JsonConvert.DeserializeObject<List<GameObject>>(Intent.GetStringExtra("Gamesets"));
