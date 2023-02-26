using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Tokket.Android.Adapters;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Listener;
using Newtonsoft.Json;

namespace Tokket.Android.Custom
{
    public class StudyGamesClassSetDialog : Dialog
    {
        internal static StudyGamesClassSetDialog Instance { get; private set; }
        private StudyGameClassSetAdapter mySetsAdapter;
        private Context cntxt;
        public Button btnOk, btnCancel;
        private ProgressBar bottomProgress;
        List<ClassSetModel> ListClassTokSets;
        private ShimmerLayout shimmerLayout;
        private RecyclerView recyclerView;
        public StudyGamesClassSetDialog(Context context, string subMenu) : base(context)
        {
            SetContentView(Resource.Layout.dialog_study_game_class_set);

            cntxt = context;
            Instance = this;

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            btnOk = FindViewById<Button>(Resource.Id.btnOK);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            bottomProgress = FindViewById<ProgressBar>(Resource.Id.bottomProgress);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            shimmerLayout = FindViewById<ShimmerLayout>(Resource.Id.mysets_shimmer_view_container);

            var mLayoutManager = new GridLayoutManager(context, 1);
            recyclerView.SetLayoutManager(mLayoutManager);

            MainActivity.Instance.RunOnUiThread(async () => await Initialize());

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //Load more stuff here
                    if (!string.IsNullOrEmpty(recyclerView.ContentDescription))
                    {
                        showBottomProgress();
                        await Initialize(recyclerView.ContentDescription);
                        hideBottomProgress();
                    }
                };


                recyclerView.AddOnScrollListener(onScrollListener);

                recyclerView.SetLayoutManager(mLayoutManager);
            }

            btnCancel.Click += delegate
            {
                Dismiss();
            };

            btnOk.Click += delegate
            {
                if (!string.IsNullOrEmpty(btnOk.ContentDescription))
                {
                    var model = JsonConvert.DeserializeObject<ClassSetModel>(btnOk.ContentDescription);
                    if (model.TokIds.Count == 0)
                    {
                        //Show alert message
                        showAlertMessage("Unable to proceed. No toks under this set!");
                    }
                    else
                    {
                        MainActivity.Instance.loadStudyGamesOptions(btnOk.ContentDescription, subMenu);
                        Dismiss();
                    }
                }
            };
        }

        private async Task Initialize(string paginationId = "")
        {
            int lastposition = recyclerView.ChildCount - 1;
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;

            recyclerView.SetAdapter(null);
            shimmerLayout.StartShimmerAnimation();
            shimmerLayout.Visibility = ViewStates.Visible;

            //ListClassSetModel = new List<ClassSetViewModel>();
            ListClassTokSets = new List<ClassSetModel>();

            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;

                var setQueryValues = new ClassSetQueryValues()
                {
                    userid = Settings.GetUserModel().UserId,
                    groupid = "",
                    partitionkeybase = "classsets",
                    startswith = false,
                    paginationid = paginationId
                };

                if (!string.IsNullOrEmpty(paginationId))
                {
                    showBottomProgress();
                }

                var result = await ClassService.Instance.GetClassSetAsync(setQueryValues, cancellationToken);

                if (!string.IsNullOrEmpty(paginationId))
                {
                    hideBottomProgress();
                }

                if (result.ContinuationToken == "cancelled")
                {
                    shimmerLayout.Visibility = ViewStates.Gone;
                    MainActivity.Instance.ShowLottieMessageDialog(cntxt, "Task was cancelled.", false);
                }
                else
                {
                    recyclerView.ContentDescription = result.ContinuationToken;

                    var newToks = result.Results.ToList().Where(s => s.TokIds.Count > 0); //Only display sets that have toks
                    ListClassTokSets.AddRange(newToks);
                }
            }

            shimmerLayout.Visibility = ViewStates.Gone;
            AssignRecyclerAdapter(); //ListClassSetModel
            if (!string.IsNullOrEmpty(paginationId))
            {
                recyclerView.ScrollToPosition(lastposition);
            }

            if (ListClassTokSets.Count == 0)
            {
                showAlertMessage("No sets found!", (s, e) => {
                    Dismiss();
                });
            }
        }

        public void AssignRecyclerAdapter()
        {
            mySetsAdapter = new StudyGameClassSetAdapter(null, ListClassTokSets, null);
            recyclerView.SetAdapter(mySetsAdapter);
        }

        private void showBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Visible;
        }
        private void hideBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Gone;
        }

        private void showAlertMessage(string message, EventHandler<DialogClickEventArgs> handler = null)
        {
            var alertDiag = new AlertDialog.Builder(cntxt);
            alertDiag.SetTitle("");
            alertDiag.SetMessage(message);
            if (handler == null)
            {
                handler = (senderAlert, args) =>
                {
                    alertDiag.Dispose();
                };
            }
            else
            {
                alertDiag.SetPositiveButton("OK", handler);
            }

            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }
    }
}