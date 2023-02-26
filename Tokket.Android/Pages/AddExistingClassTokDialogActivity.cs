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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Result = Tokket.Shared.Helpers.Result;
using ServiceAccount = Tokket.Shared.Services;
using SharedHelpers = Tokket.Shared.Helpers;
using AuthorizationTokenModel = Tokket.Shared.Models.AuthorizationTokenModel;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "AddExistingClassTokDialogActivity", Theme = "@style/Theme.Transparent", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddExistingClassTokDialogActivity : BaseActivity
    {
        GestureDetector gesturedetector;
        private string TAG = Java.Lang.Class.FromType(typeof(AddExistingClassTokDialogActivity)).Name;
        private ClassGroupModel groupModel;
        ResultData<Tokmoji> tokmojiResult;
        public AddExistingClassTokDataAdapter classTokDataAdapter;
        public List<ClassTokModel> ClassTokCollection, newClassTokCollection;
        private ClassTokModel classtokModel;
        internal static AddExistingClassTokDialogActivity Instance { get; private set; }
        bool isPublicFeed = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.dialog_add_existing_class_tok);

            setWindowSize(0.9, 0.9);

            Instance = this;

            gesturedetector = new GestureDetector(this, new MyGestureListener(this));
            groupModel = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("GroupModel"));
            BtnAddExistingToks.Enabled = false;
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            var mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            recyclerView.SetLayoutManager(mLayoutManager);

            RunOnUiThread(async () => await InitializeData());

            btnClose.Click += delegate
            {
                this.Finish();
            };

            BtnAddExistingToks.Click += async (obj, eve) => {
                await updateClassTok();
            };

            btnFilter.Click += (s, e) =>
            {
                PopupMenu menu = new PopupMenu(Instance, btnFilter);

                // Call inflate directly on the menu:
                menu.Inflate(Resource.Menu.menu_add_existing_class_tok);
                var itemPublic = menu.Menu.FindItem(Resource.Id.itemPublic);
                var itemMyClassTok = menu.Menu.FindItem(Resource.Id.btnMyClassTok);

                // A menu item was clicked:
                menu.MenuItemClick += (s1, arg1) => {
                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.itemPublic:
                            isPublicFeed = true;
                            RunOnUiThread(async () => await InitializeData());
                            break;
                        case Resource.Id.btnMyClassTok:
                            isPublicFeed = false;
                            RunOnUiThread(async () => await InitializeData());
                            break;
                    }
                    //Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                };

                // Menu was dismissed:
                menu.DismissEvent += (s2, arg2) => {
                    //Console.WriteLine("menu dismissed");
                };

                menu.Show();
            };

            //gesturedetector.DoubleTap += async(object sender, GestureDetector.DoubleTapEventArgs e) =>
            //{
            //    await updateClassTok();
            //};
        }

        private async Task updateClassTok()
        {
            showBlueLoading(this);

            var taskCompletionSource = new TaskCompletionSource<ResultModel>();
            CancellationToken cancellationToken;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
            cancellationToken = cancellationTokenSource.Token;

            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });

            classtokModel.GroupId = groupModel.Id;
            classtokModel.IsGroup = true;
            classtokModel.GroupName = groupModel.Name;
            classtokModel.IsPublic =false;
            classtokModel.IsPrivate = true; //chkPrivate.Checked;
            classtokModel.IsGroup = true;
  

            var result = await ClassService.Instance.AddClassToksAsync(classtokModel, cancellationToken);

            hideBlueLoading(this);

            if (result.ResultMessage == "cancelled")
            {
                ShowLottieMessageDialog(this, "Task was cancelled.", false);
                //showRetryDialog("Task was cancelled.");
            }
            else
            {
                if (result.ResultMessage != "cancelled")
                {
                    var isSuccess = false;
                    if (result.ResultEnum == Result.Success)
                        isSuccess = true;

                    ShowLottieMessageDialog(this, result.ResultEnum.ToString(), isSuccess, handlerOkClick: (s, e) =>
                    {
                        if (result.ResultEnum == Result.Success)
                        {
                            ClassToksFragment.Instance.AddClassTokCollection(classtokModel);

                            var serializedObject = JsonConvert.SerializeObject(classtokModel);
                            Intent = new Intent();
                            Intent.PutExtra("classtokModel", serializedObject);
                            SetResult(AppResult.Ok, Intent);
                            Finish();
                        }
                    });
                }
            }
        }

        int selectedIndex = 0;
        private void OnGridBackgroundTouched(object sender, View.TouchEventArgs e)
        {
            selectedIndex = 0;
            var view = sender as View;
            try { selectedIndex = (int)view.Tag; } catch { selectedIndex = int.Parse((string)view.Tag); }
            classtokModel = ClassTokCollection[selectedIndex];
            BtnAddExistingToks.Enabled = true;
            gesturedetector.OnTouchEvent(e.Event);
        }

        public async Task InitializeData(bool isFromFilter = false, bool isSwipeRefresh = false)
        {
            newClassTokCollection = new List<ClassTokModel>();
            ClassTokCollection = new List<ClassTokModel>();
            recyclerView.SetAdapter(null);
            shimmer_view_container.StartShimmerAnimation();
            shimmer_view_container.Visibility = ViewStates.Visible;
            await CheckToken();

            //If no cache data found then load new toks

            await LoadNewToks(true);

            if (newClassTokCollection.Count > 0)
            {
                ClassTokCollection.AddRange(newClassTokCollection);
            }

            SetClassTokRecyclerAdapter();

            shimmer_view_container.Visibility = ViewStates.Gone;
        }

        public async Task LoadNewToks(bool isLoadTokMoji)
        {
            List<Task> tasksList = new List<Task>();

            tasksList.Add(GetNewToks());

            await Task.WhenAll(tasksList);
        }

        private async Task GetNewToks()
        {
            List<ClassTokModel> resultToksData = new List<ClassTokModel>();
            resultToksData = await GetClassToksData();

            if (resultToksData != null)
            {
                ClassTokCollection.AddRange(resultToksData);
            }
        }

        public async Task<List<ClassTokModel>> GetClassToksData()
        {
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;
            List<ClassTokModel> classTokModelsResult = new List<ClassTokModel>();

            /*cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken = cancellationTokenSource.Token;

                ClassTokQueryValues queryValues;
                if (isPublicFeed)
                {
                    var groupId = groupModel == null ? "" : groupModel.Id;
                    var ownerId = Settings.GetUserModel().UserId;
                    queryValues = new ClassTokQueryValues()
                    {
                        partitionkeybase = isPublicFeed ? "classtoks" : (!string.IsNullOrEmpty(groupId) ? $"{groupId}-classtoks" : $"{ownerId}-classtoks"),
                        groupid = groupId,
                        userid = isPublicFeed || !string.IsNullOrEmpty(groupId) ? "" : ownerId,
                        text = "",
                        startswith = false,
                        publicfeed = isPublicFeed,
                        FilterBy = FilterBy.None,
                        FilterItems = new List<string>(),
                        paginationid = "",
                        classtokmode = true
                    };
                }
                else
                {
                    queryValues = new ClassTokQueryValues()
                    {
                        partitionkeybase = $"{Settings.GetUserModel().UserId}-classtoks",
                        groupid = "",//isPublicFeed ? groupId : "",
                        userid = isPublicFeed ? "" : Settings.GetUserModel().UserId,
                        text = "",
                        startswith = false,
                        publicfeed = isPublicFeed,
                        FilterBy = FilterBy.None,
                        FilterItems = new List<string>(),
                        classtokmode = true
                    };
                }

                ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
                tokResult.Results = new List<ClassTokModel>();


                GetClassToksRequest request = new GetClassToksRequest()
                {
                    QueryValues = queryValues
                };

                tokResult = await ClassService.Instance.GetClassToksAsync(request);



                //tokResult = await ClassService.Instance.GetClassToksAsync(queryValues, cancellationToken, fromCaller: TAG);

                if (tokResult.ContinuationToken == "cancelled")
                {
                    shimmer_view_container.Visibility = ViewStates.Gone;
                    ShowLottieMessageDialog(this, "Task was cancelled.", false);
                }
                else
                {
                    classTokModelsResult = tokResult.Results.ToList();
                    recyclerView.ContentDescription = tokResult.ContinuationToken;
                }
            }

            return classTokModelsResult;
        }

        private void SetClassTokRecyclerAdapter(List<ClassTokModel> loadMoreItems = null)
        {
            if (loadMoreItems == null)
            {
                classTokDataAdapter = new AddExistingClassTokDataAdapter(ClassTokCollection, OnGridBackgroundTouched);
                recyclerView.SetAdapter(classTokDataAdapter);
            }
            else
            {
                classTokDataAdapter.UpdateItems(loadMoreItems, recyclerView.ChildCount);
                recyclerView.SetAdapter(classTokDataAdapter);
                recyclerView.ScrollToPosition(classTokDataAdapter.ItemCount - loadMoreItems.Count);
            }
        }

        private async Task CheckToken()
        {
            bool resultbool = false;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            var refreshtoken = await SecureStorage.GetAsync("refreshtoken");
            var result = ServiceAccount.AccountService.Instance.VerifyToken(idtoken, refreshtoken);
            if (result.ResultMessage.Contains("refreshed"))
            {
                SecureStorage.Remove("idtoken");
                SecureStorage.Remove("refreshtoken");
                var obj = JsonConvert.DeserializeObject<AuthorizationTokenModel>(result.ResultObject.ToString());

                await SecureStorage.SetAsync("idtoken", obj.IdToken);
                await SecureStorage.SetAsync("refreshtoken", obj.RefreshToken);
            }
        }

        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private AddExistingClassTokDialogActivity activity;
            public MyGestureListener(AddExistingClassTokDialogActivity Activity)
            {
                activity = Activity;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                return true;
            }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                //Change background color based on this
                activity.classTokDataAdapter.selectedPosition = activity.selectedIndex;
                activity.recyclerView.Post(() =>
                {
                    activity.classTokDataAdapter.NotifyDataSetChanged();
                });

                //End

                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {

            }
            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                return base.OnFling(e1, e2, velocityX, velocityY);
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                return base.OnScroll(e1, e2, distanceX, distanceY);
            }
        }

        public Button btnClose => FindViewById<Button>(Resource.Id.btnClose);
        public Button btnFilter => FindViewById<Button>(Resource.Id.btnFilter);
        public ShimmerLayout shimmer_view_container => FindViewById<ShimmerLayout>(Resource.Id.shimmer_view_container);

        public Button BtnAddExistingToks => FindViewById<Button>(Resource.Id.btnAddEC);
        public RecyclerView recyclerView => FindViewById<RecyclerView>(Resource.Id.recyclerView);
    }
}