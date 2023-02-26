using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Tokquest;
using Xamarin.Essentials;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Game Toks", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CreateGameSetToks : BaseActivity
    {
        internal static CreateGameSetToks Instance { get; private set; }
        ObservableCollection<ClassTok> ClassGroupCollection;
        public CreateGameDetailsViewModel GameDetails;

        private enum ActivityName
        {
            Filter = 1001
        }


        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.GameSetTokList);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Settings.ActivityInt = (int)ActivityType.CreateGameSetToks;

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            var mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerClassGroupList.SetLayoutManager(mLayoutManager);

            ClassGroupCollection = new ObservableCollection<ClassTok>();

            RunOnUiThread(async () => await Initialize());
            btnCancelCreateGameTokList.Click += BtnCancelCreateGameTokList_Click;
            btnSaveGameToksContinue.Click += BtnSaveGameToksContinue_Click;
            //swipeRefreshRecycler.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            //swipeRefreshRecycler.Refresh += RefreshLayout_Refresh;

            //if (RecyclerClassGroupList != null)
            //{
            //    RecyclerClassGroupList.HasFixedSize = true;

            //    var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
            //    onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
            //    {
            //        if (!string.IsNullOrEmpty(RecyclerClassGroupList.ContentDescription))
            //        {
            //            await Initialize(RecyclerClassGroupList.ContentDescription);
            //        }
            //    };

            //    RecyclerClassGroupList.AddOnScrollListener(onScrollListener);

            //    RecyclerClassGroupList.SetLayoutManager(mLayoutManager);
            //}

        }

        private void BtnSaveGameToksContinue_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnCancelCreateGameTokList_Click(object sender, EventArgs e)
        {
            Finish();
        }

        //private void showBottomDialog()
        //{
        //    bottomProgress.Visibility = ViewStates.Visible;
        //}
        //private void hideBottomDialog()
        //{
        //    bottomProgress.Visibility = ViewStates.Gone;
        //}
        private async Task Initialize(string pagination_id = "")
        {
        
            int lastposition = 0;
            //if (!string.IsNullOrEmpty(pagination_id))
            //{
            //    lastposition = RecyclerClassGroupList.ChildCount - 1;
            //    showBottomDialog();
            //}
            //switch (filter)
            //{
            //    case GroupFilter.OwnGroup:
            //        results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //        {
            //            paginationid = pagination_id,
            //            partitionkeybase = "classgroups",
            //            userid = Settings.GetTokketUser().Id,
            //            startswith = false,
            //            joined = false
            //        });
            //        break;
            //    case GroupFilter.JoinedGroup:
            //        results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //        {
            //            paginationid = pagination_id,
            //            partitionkeybase = "classgroups",
            //            userid = Settings.GetTokketUser().Id,
            //            joined = true,
            //            startswith = false
            //        });
            //        break;
            //    case GroupFilter.MyGroup:
            //        var myGroups = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //        {
            //            paginationid = pagination_id,
            //            partitionkeybase = "classgroups",
            //            userid = Settings.GetTokketUser().Id,
            //            joined = false,
            //            startswith = false
            //        });

            //        var joined = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            //        {
            //            paginationid = pagination_id,
            //            partitionkeybase = "classgroups",
            //            userid = Settings.GetTokketUser().Id,
            //            joined = true,
            //            startswith = false
            //        });

            //        var combined = myGroups.Results.ToList();
            //        combined.AddRange(joined.Results);

            //        results.Results = combined;

            //        break;
            //}
            //if (!string.IsNullOrEmpty(pagination_id))
            //{
            //    hideBottomDialog();
            //}

            //RecyclerClassGroupList.ContentDescription = results.ContinuationToken;
            //var classgroupResult = results.Results.ToList();

            //ClassGroupCollection.Clear();
            //foreach (var item in classgroupResult)
            //{
            //    ClassGroupCollection.Add(item);
            //}

            //SetRecyclerAdapter();

            //if (!string.IsNullOrEmpty(pagination_id))
            //{
            //    RecyclerClassGroupList.ScrollToPosition(lastposition);
            //}

            // for (int i = 0; i < 5; i++) {
            //    ClassGroupCollection.Add("item" + i);
            //}

            GameDetails = JsonConvert.DeserializeObject<CreateGameDetailsViewModel>(Intent.GetStringExtra("GameDetailsAfterClassGroup"));
            foreach (var tt in GameDetails.classTokModels) {

                ClassGroupCollection.Add(tt);
            }
            
            SetRecyclerAdapter();


        }


        private void SetRecyclerAdapter()
        {
            var adapterClassGroup = ClassGroupCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.GameSetTokListRow);
            RecyclerClassGroupList.SetAdapter(adapterClassGroup);
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, ClassTok model, int position)
        {
            var btnCheck = holder.FindCachedViewById<CheckBox>(Resource.Id.checkboxGame);
            var PrimaryText = holder.FindCachedViewById<TextView>(Resource.Id.PrimaryTextGame);
            var Edit = holder.FindCachedViewById<TextView>(Resource.Id.PrimaryTextEdit);
            var btnYesOrNo = holder.FindCachedViewById<Button>(Resource.Id.BtnGameIsPlayable);

            try
            {
                if (!model.IsDetailBased)
                {

                    btnYesOrNo.Text = "No";
                    btnYesOrNo.SetBackgroundColor(Color.Red);
                }

            }
            catch (Exception err)
            {
                Console.WriteLine("---" + err.Message);
                
            }

            try
            {
                PrimaryText.Text = (model.PrimaryFieldText == null) ? model.SecondaryFieldText : model.PrimaryFieldText;

            }
            catch (Exception)
            {

                PrimaryText.Text = "None";

            }
        }

        [Java.Interop.Export("ItemRowClicked")]
        public async void ItemRowClicked(View v)
        {
           // linearProgress.Visibility = ViewStates.Visible;

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
        //        swipeRefreshRecycler.Refreshing = false;
        //    }
        //}
        //private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    swipeRefreshRecycler.Refreshing = false;
        //}
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            this.RunOnUiThread(async () => await Initialize());
            Thread.Sleep(1000);
        }

        //public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgressCreate);
        //public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgressCreate);
        public RecyclerView RecyclerClassGroupList => FindViewById<RecyclerView>(Resource.Id.RecyclerGameToksList);

        public Button btnCancelCreateGameTokList => FindViewById<Button>(Resource.Id.btnCancelCreateGameTokList);

        public Button btnSaveGameToksContinue => FindViewById<Button>(Resource.Id.btnSaveGameToksContinue);

        //public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbarCreate);
        //public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideTextCreate);
        //public SwipeRefreshLayout swipeRefreshRecycler => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerCreate);
        //public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFoundCreate);
    }
}