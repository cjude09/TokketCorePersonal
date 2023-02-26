using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Java.Util.Logging;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Helpers;
using Tokket.Core.Tools;
using Tokket.Shared.Extensions;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;
using Tokket.Android.Pages;

namespace Tokket.Android.Views.Fragments
{
    public class TokChannelCommunitiesFragment : AndroidX.Fragment.App.Fragment
    {
        internal static TokChannelCommunitiesFragment Instance { get; private set; }
        View page;
        public ObservableCollection<ClassGroup> ClassGroupCollection;
        List<string> randomcolors = new List<string>();
        public TokChannelCommunitiesFragment()
        {
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (page == null)
                page = inflater.Inflate(Resource.Layout.fragment_tok_channel_toks, container, false);

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            ClassGroupCollection = new ObservableCollection<ClassGroup>();

            var mLayoutManager = new GridLayoutManager(page.Context, numcol);
            RecyclerToks.SetLayoutManager(mLayoutManager);

            swipeRefreshContainer.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            swipeRefreshContainer.Refresh -= RefreshLayout_Refresh;
            swipeRefreshContainer.Refresh += RefreshLayout_Refresh;

            randomcolors = MainActivity.Instance.Colors.Shuffle().ToList();

            return page;
        }

        public async Task GetCommunities(string level0, string level1, string level2, string level3, bool isSearch = false, string searchText = "")
        {
            RecyclerToks.SetAdapter(null);
            ClassGroupCollection.Clear(); //TODO should not clear if there's a pagination
            shimmerContainer.StartShimmerAnimation();
            shimmerContainer.Visibility = ViewStates.Visible;

            var queryValues = new ClassGroupQueryValues()
            {
                startswith = false,
                level0 = level0,
                level1 = level1,
                level2 = level2,
                level3 = level3,
                text = searchText
            };

            ResultData<ClassGroup> tokResult = new ResultData<ClassGroup>();
            tokResult.Results = new List<ClassGroup>();
            tokResult = await ClassService.Instance.GetCommunitiesAsync(queryValues);

            var tokResultList = tokResult.Results.ToList();
            foreach (var item in tokResultList)
            {
                ClassGroupCollection.Add(item);
            }
            
            RecyclerToks.ContentDescription = tokResult.ContinuationToken;

            var adapterCommunities = ClassGroupCollection.GetRecyclerAdapter(BindCommunities, Resource.Layout.row_communities);
            RecyclerToks.SetAdapter(adapterCommunities);

            if (isSearch)
            {
                txtSearchResult.Visibility = ViewStates.Visible;
                txtSearchResult.Text = "Search results for \"" + searchText + "\": " + tokResultList.Count();
            }

            shimmerContainer.Visibility = ViewStates.Gone;
        }
        public void HideSearchResult()
        {
            txtSearchResult.Visibility = ViewStates.Gone;
            txtSearchResult.Text = "";
        }

        public void AddClassGroupCollection(ClassGroup item, bool isSave = true)
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
            var adapterCommunities = ClassGroupCollection.GetRecyclerAdapter(BindCommunities, Resource.Layout.row_communities);
            RecyclerToks.SetAdapter(adapterCommunities);
        }

        private void BindCommunities(CachingViewHolder holder, ClassGroup classGroup, int position)
        {
            var parentView = holder.FindCachedViewById<LinearLayout>(Resource.Id.parentView);
            var txtName = holder.FindCachedViewById<AppCompatTextView>(Resource.Id.txtName);
            txtName.Text = classGroup.Name;

            parentView.SetBackgroundResource(Resource.Drawable.tileview_layout);
            GradientDrawable DrawableBG = (GradientDrawable)parentView.Background;

            int ndx = position % MainActivity.Instance.Colors.Count;
            if (ndx == 0 || randomcolors.Count == 0) randomcolors = MainActivity.Instance.Colors.Shuffle().ToList();
            DrawableBG.SetStroke(Convert.ToInt32(Resources.GetDimension(Resource.Dimension._2sdp)), Color.ParseColor(randomcolors[position]));
            DrawableBG.SetColor(Color.ParseColor(randomcolors[position]));

            holder.ItemView.Tag = position;
            holder.ItemView.Click -= CommynityInfoClicked;
            holder.ItemView.Click += CommynityInfoClicked;
        }

        private void CommynityInfoClicked(object sender, EventArgs e)
        {
            var view = sender as View;
            int ndx = 0;
            try { ndx = (int)view.Tag; } catch { ndx = int.Parse((string)view.Tag); }
            var nextActivity = new Intent(view.Context, typeof(CommunityInfoActivity));
            nextActivity.PutExtra("classgroup", JsonConvert.SerializeObject(ClassGroupCollection[ndx]));
            StartActivity(nextActivity);
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
                swipeRefreshContainer.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipeRefreshContainer.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        public RecyclerView RecyclerToks => page.FindViewById<RecyclerView>(Resource.Id.recycyclerToks);
        public SwipeRefreshLayout swipeRefreshContainer => page.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshContainer);
        public ShimmerLayout shimmerContainer => page.FindViewById<ShimmerLayout>(Resource.Id.shimmerContainer);
        public AppCompatTextView txtSearchResult => page.FindViewById<AppCompatTextView>(Resource.Id.txtSearchResult);
    }
}
