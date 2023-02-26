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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Android.Listener;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Tokket.Android.TokQuest;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;
using AndroidX.ConstraintLayout.Widget;

namespace Tokket.Android
{
    [Activity(Label = "Groups", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CreateGameSetClassGroup : BaseActivity
    {
        internal static CreateGameSetClassGroup Instance { get; private set; }
        ObservableCollection<ClassGroupModel> ClassGroupCollection;
        GridLayoutManager mLayoutManager;
        AlertDialog.Builder dialog;
        AlertDialog alert;

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
            SetContentView(Resource.Layout.tokquest_classgroup_create_game);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Settings.ActivityInt = (int)ActivityType.CreateGameSetClassGroup;

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerClassGroupList.SetLayoutManager(mLayoutManager);

            ClassGroupCollection = new ObservableCollection<ClassGroupModel>();

            RunOnUiThread(async () => await Initialize());


            swipeRefreshRecycler.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
            swipeRefreshRecycler.Refresh += RefreshLayout_Refresh;

            if (RecyclerClassGroupList != null)
            {
                RecyclerClassGroupList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrEmpty(RecyclerClassGroupList.ContentDescription))
                    {
                        await Initialize(RecyclerClassGroupList.ContentDescription);
                    }
                };

                RecyclerClassGroupList.AddOnScrollListener(onScrollListener);

                RecyclerClassGroupList.SetLayoutManager(mLayoutManager);
            }

        }
        private void showBottomDialog()
        {
            bottomProgress.Visibility = ViewStates.Visible;
        }
        private void hideBottomDialog()
        {
            bottomProgress.Visibility = ViewStates.Gone;
        }
        private async Task Initialize(string pagination_id = "")
        {
            GroupFilter filter = (GroupFilter)Settings.FilterGroup;
            ResultData<ClassGroupModel> results = new ResultData<ClassGroupModel>();

            int lastposition = 0;
            if (!string.IsNullOrEmpty(pagination_id))
            {
                lastposition = RecyclerClassGroupList.ChildCount - 1;
                showBottomDialog();
            }
            switch (filter)
            {
                case GroupFilter.OwnGroup:
                    results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        startswith = false,
                        joined = false
                    });
                    break;
                case GroupFilter.JoinedGroup:
                    results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false
                    });
                    break;
                case GroupFilter.MyGroup:
                    var myGroups = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = false,
                        startswith = false
                    });

                    var joined = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false
                    });

                    var combined = myGroups.Results.ToList();
                    combined.AddRange(joined.Results);

                    results.Results = combined;

                    break;
            }
            if (!string.IsNullOrEmpty(pagination_id))
            {
                hideBottomDialog();
            }

            RecyclerClassGroupList.ContentDescription = results.ContinuationToken;
            var classgroupResult = results.Results.ToList();

            ClassGroupCollection.Clear();
            foreach (var item in classgroupResult)
            {
                ClassGroupCollection.Add(item);
            }

            SetRecyclerAdapter();

            if (!string.IsNullOrEmpty(pagination_id))
            {
                RecyclerClassGroupList.ScrollToPosition(lastposition);
            }
        }
        private async Task SearchGroup(string searchText)
        {
            showBottomDialog();
            ClassGroupCollection.Clear();
            var classgroupResult = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
            {
                partitionkeybase = "classgroups",
                startswith = false,
                text = searchText
            });

            foreach (var item in classgroupResult.Results.ToList())
            {
                ClassGroupCollection.Add(item);
            }

            hideBottomDialog();
            SetRecyclerAdapter();
        }
        public void AddClassGroupCollection(ClassGroupModel item, bool isSave = true)
        {
            if (isSave == false)
            {
                var result = ClassGroupCollection.FirstOrDefault(c => c.Id == item.Id);
                if (result != null) //If Edit
                {
                    int ndx = ClassGroupCollection.IndexOf(result);
                    ClassGroupCollection.Remove(result);

                    ClassGroupCollection.Insert(ndx, item);
                }
            }
            else
            {
                ClassGroupCollection.Insert(0, item);
            }
            SetRecyclerAdapter();
        }
        public void RemoveClassGroupCollection(ClassGroupModel item)
        {
            var collection = ClassGroupCollection.FirstOrDefault(a => a.Id == item.Id);
            if (collection != null) //If item exist
            {
                int ndx = ClassGroupCollection.IndexOf(collection); //Get index
                ClassGroupCollection.Remove(collection);

                SetRecyclerAdapter();
            }
        }
        private void SetRecyclerAdapter()
        {
            var adapterClassGroup = ClassGroupCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.classgrouplist_row);
            RecyclerClassGroupList.SetAdapter(adapterClassGroup);

            linearProgress.Visibility = ViewStates.Invisible;
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, ClassGroupModel model, int position)
        {
            var ClassGroupHeader = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupHeader);
            var ClassGroupBody = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupBody);
            var ClassGroupFooter = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupFooter);
            var constraintParent = holder.FindCachedViewById<ConstraintLayout>(Resource.Id.constraintParent);
            var imgClassGroupListImg = holder.FindCachedViewById<ImageView>(Resource.Id.imgClassGroupListImg);

            constraintParent.Tag = position;

            try
            {
                constraintParent.SetBackgroundColor(Color.ParseColor(Colors[position]));
            }
            catch (Exception)
            {
                ClassGroupBody.SetBackgroundColor(Color.White);

            }

            ClassGroupHeader.Text = model.Name;
            if (!string.IsNullOrEmpty(model.Image))
            {
                ClassGroupBody.Visibility = ViewStates.Gone;
                imgClassGroupListImg.Visibility = ViewStates.Visible;
                Glide.With(this).Load(model.Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imgClassGroupListImg);
            }
            else
            {
                ClassGroupBody.Visibility = ViewStates.Visible;
                imgClassGroupListImg.Visibility = ViewStates.Gone;
                ClassGroupBody.Text = model.Description;
                ClassGroupFooter.Text = "Last updated " + DateConvert.ConvertToRelative(model.CreatedTime);
            }
        }

        [Java.Interop.Export("ItemRowClicked")]
        public async void ItemRowClicked(View v)
        {
            linearProgress.Visibility = ViewStates.Visible;

            GameDetails = JsonConvert.DeserializeObject<CreateGameDetailsViewModel>(Intent.GetStringExtra("GameDetailsBeforeClassGroup"));

            var pk = GameDetails.ClassSetId + "-classtoks0";
            var data = await TokquestService.Instance.GetClassSetAsyncTokquest(GameDetails.ClassSetId, GameDetails.OwnerId);
            List<ClassTok> classtokRes = new List<ClassTok>();
            foreach (var t in data.TokIds)
            {
                var get1 = await TokquestService.Instance.GetClassTokAsyncTokques(t, pk);
                classtokRes.Add(get1);

            }

            GameDetails.classTokModels = classtokRes;
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }
            Intent nextActivity = new Intent(this, typeof(CreateGameSetToks));
            GameDetails.ClassGroupId = ClassGroupCollection[position].Id;
            var modelConvert = JsonConvert.SerializeObject(GameDetails);
            nextActivity.PutExtra("GameDetailsAfterClassGroup", modelConvert);
            this.StartActivity(nextActivity);
            linearProgress.Visibility = ViewStates.Gone;



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
                swipeRefreshRecycler.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipeRefreshRecycler.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            FilterType type = (FilterType)Settings.FilterTag;
            this.RunOnUiThread(async () => await Initialize());
            Thread.Sleep(1000);
        }

        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgressCreate);
        public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgressCreate);
        public RecyclerView RecyclerClassGroupList => FindViewById<RecyclerView>(Resource.Id.RecyclerClassGroupListCreate);
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbarCreate);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideTextCreate);
        public SwipeRefreshLayout swipeRefreshRecycler => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerCreate);
        public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFoundCreate);
    }
}