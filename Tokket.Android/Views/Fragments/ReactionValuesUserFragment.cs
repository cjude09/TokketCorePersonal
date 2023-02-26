using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Listener;
using Tokket.Shared.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using System.Threading.Tasks;
using Supercharge;
using AndroidX.RecyclerView.Widget;
using AndroidX.Preference;

namespace Tokket.Android.Fragments
{
    public class ReactionValuesUserFragment : AndroidX.Fragment.App.Fragment
    {
        TokketUserReactionsAdapter ReactionsAdapter;
        View view; ReactionQueryValues reactionQueryValues;
        //SwipeRefreshLayout refreshLayout = null;
        GridLayoutManager mLayoutManager; string tokId;
        private string titlepage = "";
        bool isDetailed = false; int DetailNumber = -1;
        public ReactionValuesUserFragment (string title)
        {
            titlepage = title;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.reactionvalues_users_page, container, false);
            view = v;

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            tokId = prefs.GetString("tokId", "");
            try
            {
                isDetailed = bool.Parse(prefs.GetString("isDetailed",""));
                DetailNumber = int.Parse(prefs.GetString("detailNumber",""));
            }
            catch (Exception ex)
            {
                isDetailed = false;
                DetailNumber = -1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerUsers.SetLayoutManager(mLayoutManager);

            Settings.ContinuationToken = null;
            MainActivity.Instance.RunOnUiThread(async () => await LoadData());

            if (RecyclerUsers != null)
            {
                RecyclerUsers.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async(object sender, EventArgs e) =>
                {
                    //Load More
                    if (!string.IsNullOrEmpty(Settings.ContinuationToken))
                    {
                        reactionQueryValues = new ReactionQueryValues();
                        reactionQueryValues.item_id = tokId;
                        reactionQueryValues.detail_number = 0;
                        reactionQueryValues.kind = titlepage.ToLower();
                        reactionQueryValues.pagination_id = Settings.ContinuationToken;

                        var result = await ReactionService.Instance.GetReactionsUsersAsync(reactionQueryValues); //GetApplicationUsers(Id, Convert.ToInt32(Index), reaction).Skip(skip).Take(takeCnt).ToList();
                        ReactionsAdapter.UpdateItems(result);
                    }
                };


                RecyclerUsers.AddOnScrollListener(onScrollListener);

                RecyclerUsers.SetLayoutManager(mLayoutManager);
            }

            return v;
        }
        private async Task LoadData()
        {
            ShimmerContainer.StartShimmerAnimation();
            ShimmerContainer.Visibility = ViewStates.Visible;

            reactionQueryValues = new ReactionQueryValues();
            reactionQueryValues.item_id = tokId;
            reactionQueryValues.detail_number = DetailNumber;
            reactionQueryValues.kind = titlepage.ToLower();
            reactionQueryValues.pagination_id = "";

            var result = await ReactionService.Instance.GetReactionsUsersAsync(reactionQueryValues); 
            //reactionValueVM.User = users; //users.Results.ToList();            
            
            ReactionsAdapter = new TokketUserReactionsAdapter(result);
            RecyclerUsers.SetAdapter(ReactionsAdapter);
            ShimmerContainer.Visibility = ViewStates.Gone;
        }

        public RecyclerView RecyclerUsers => view.FindViewById<RecyclerView>(Resource.Id.reactionvalues_users_recyclerview);
        public ShimmerLayout ShimmerContainer => view.FindViewById<ShimmerLayout>(Resource.Id.reactionvalues_shimmerlayout);
    }
}