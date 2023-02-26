using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Adapters;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Android.Graphics;
using Tokket.Android.Listener;
using System.Threading;
using Supercharge;
using System.ComponentModel;
using Tokket.Shared.Services;
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Android.Text;
using AndroidX.Core.Content;
using Result = Android.App.Result;
using AndroidX.SwipeRefreshLayout.Widget;

namespace Tokket.Android
{
    [Activity(Label = "My Class Tok Sets",Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddToksToSetActivity : Activity
    {
        private int REQUEST_TOKINFO_RESULT = 1001;
        private int REQUEST_FILTER_RESULT = 1002;
        internal static AddToksToSetActivity Instance { get; private set; }
        bool isAddToSet; string toktypeid = "";
        Set SetModel; ClassTokModel ClassTokMode;
        Bundle bundle;

        bool isPublicFeed = true; int filterBy = 0; string filterByItems = "";

        View page; string UserId; public List<ClassSetModel> ListClassTokSets; //public List<ClassSetViewModel> ListClassSetModel;
        MySetsAdapter MySetsAdapter; string groupId = "";
        GridLayoutManager mLayoutManager; SwipeRefreshLayout refreshLayout = null;
        public ViewModels.MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_toks_to_set);
            
            bundle = new Bundle();
            Instance = this;
            MySetsVm.Instance = Instance;

            toktypeid = Intent.GetStringExtra("TokTypeId")?.ToString();

            MySetsVm.TokTypeID = toktypeid;

            MySetsVm.LinearProgress = LinearProgress;
            MySetsVm.ProgressCircle = ProgressCircle;
            MySetsVm.ProgressText = ProgressText;

            isAddToSet = Intent.GetBooleanExtra("isAddToSet", true);

            MySetsVm.IsAddToksToSet = isAddToSet;

            BtnCancel.Click += delegate
            {
                MySetsVm.CancelSet();
            };

            LinearToolbar.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.navy_blue)));
             
            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
            {
                //If opened from MySetsViewActivity
                if (isAddToSet)
                {
                    BtnAddClassSet.Text = "Add to Set";
                    BtnAddClassSet.Click += async (s, e) =>
                    {
                        await MySetsVm.AddSet();
                    };
                }
                else
                {
                    BtnAddClassSet.Text = "Remove from Set";
                    BtnAddClassSet.Click += async (s, e) =>
                    {
                        await MySetsVm.RemoveToksFromSet();
                    };
                   // BtnAddToksToGroup.Text ="Remove Class Toks";
                }

                txtHeader.Text = "Select a tok:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                SetModel = JsonConvert.DeserializeObject<Set>(Intent.GetStringExtra("classsetModel"));

                MySetsVm.SetModel = SetModel;
                groupId = SetModel.GroupId;
                txtMySetsPageTitle.Text = SetModel.Name;
            }
            else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo))
            {
                ClassTokMode = JsonConvert.DeserializeObject<ClassTokModel>(Intent.GetStringExtra("classtokModel"));

                MySetsVm.tokList = ClassTokMode;
                txtMySetsPageTitle.Text = ClassTokMode.PrimaryFieldText;
                txtHeader.Text = "Select a class set:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                BtnAddClassSet.Text = "Add to Class Set";
                BtnAddClassSet.Click += async (s, e) =>
                {
                    await MySetsVm.AddSet();
                };
            }

            UserId = Settings.GetUserModel().UserId;
            ListClassTokSets = new List<ClassSetModel>();

            MySetsVm.RecyclerMainList = RecyclerMainList;
            MySetsVm.ShimmerLayout = ShimmerLayout;

            mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerMainList.SetLayoutManager(mLayoutManager);

            if (!string.IsNullOrEmpty(groupId))
            {
                TextNothingFound.Visibility = ViewStates.Visible;
            }

            if (isAddToSet)
            {
                this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                    isPublicFeed: isPublicFeed,
                    filterBy: filterBy,
                    filterByItems: filterByItems,
                    isAddtoktoSet: true,
                    groupId: groupId,
                    ownerId: UserId
                    )) ;
            }
            else
            {
                if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity))
                {
                    this.RunOnUiThread(async () => await Initialize());
                }
                else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                {
                    this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                            isPublicFeed: isPublicFeed,
                            filterBy: filterBy,
                            filterByItems: filterByItems,
                            isAddtoktoSet: isAddToSet
                            ));
                }

                imageButtonFilterToks.Visibility = ViewStates.Gone;
            }

            refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.mytoksets_swiperefresh);
            refreshLayout.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
            refreshLayout.Refresh += RefreshLayout_Refresh;

            if (RecyclerMainList != null)
            {
                RecyclerMainList.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    //Load more stuff here
                    if (!string.IsNullOrEmpty(RecyclerMainList.ContentDescription))
                    {
                        showBottomProgress();
                        if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView) || isAddToSet)
                        {
                            this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                                paginationId: RecyclerMainList.ContentDescription,
                                isPublicFeed: isPublicFeed,
                                filterBy: filterBy,
                                filterByItems: filterByItems,
                                isAddtoktoSet: true
                                ));
                        }
                        else
                        {
                            await Initialize(RecyclerMainList.ContentDescription);
                        }
                        hideBottomProgress();
                    }
                };


                RecyclerMainList.AddOnScrollListener(onScrollListener);

                RecyclerMainList.SetLayoutManager(mLayoutManager);
            }

            imageButtonFilterToks.Click += delegate
            {
                var nextActivity = new Intent(this, typeof(DialogFilterToksActivity));
                nextActivity.PutExtra("isPublicFeed", isPublicFeed);
                nextActivity.PutExtra("filterBy", filterBy);
                nextActivity.PutExtra("filterItems", filterByItems);
                StartActivityForResult(nextActivity, REQUEST_FILTER_RESULT);
            };
            
            InitAddToGroup();

            var isHideAddClassTokToGroup = Intent.GetBooleanExtra("isHideAddClassTokToGroup", false);
            if (isHideAddClassTokToGroup)
            {
                BtnAddToksToGroup.Visibility = ViewStates.Gone;
            };
        }
        private void showBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Visible;
        }
        private void hideBottomProgress()
        {
            bottomProgress.Visibility = ViewStates.Gone;
        }

        private async Task Initialize(string paginationId = "")
        {
            int lastposition = RecyclerMainList.ChildCount - 1;
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            CancellationToken cancellationToken;

            RecyclerMainList.SetAdapter(null);
            ShimmerLayout.StartShimmerAnimation();
            ShimmerLayout.Visibility = ViewStates.Visible;

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
                    userid = UserId,
                    groupid = groupId,
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
                    ShimmerLayout.Visibility = ViewStates.Gone;
                    showRetryDialog("Task was cancelled.");
                }
                else
                {
                    RecyclerMainList.ContentDescription = result.ContinuationToken;

                    ListClassTokSets.AddRange(result.Results.ToList());
                }
            }

            if (ListClassTokSets.Count == 0)
            {
                TextNothingFound.Text = "No class sets.";
            }

            ShimmerLayout.Visibility = ViewStates.Gone;
            AssignRecyclerAdapter(); //ListClassSetModel
            if (!string.IsNullOrEmpty(paginationId))
            {
                RecyclerMainList.ScrollToPosition(lastposition);
            }
        }

        private void showRetryDialog(string message)
        {
            var builder = new AlertDialog.Builder(MyClassSetsActivity.Instance)
                            .SetMessage(message)
                            .SetPositiveButton("Cancel", (_, args) =>
                            {

                            })
                            .SetNegativeButton("Retry", async (_, args) =>
                            {
                                if (isAddToSet)
                                {
                                    this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                                            isPublicFeed: isPublicFeed,
                                            filterBy: filterBy,
                                            filterByItems: filterByItems,
                                            isAddtoktoSet: true
                                            ));
                                }
                                else
                                {
                                    if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity))
                                    {
                                        this.RunOnUiThread(async () => await Initialize());
                                    }
                                    else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                                    {
                                        this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                                                isPublicFeed: isPublicFeed,
                                                filterBy: filterBy,
                                                filterByItems: filterByItems,
                                                isAddtoktoSet: true
                                                ));
                                    }
                                }
                            })
                            .SetCancelable(false)
                            .Show();
        }
        public void AssignRecyclerAdapter()
        {
            MySetsAdapter = new MySetsAdapter(null, ListClassTokSets, null);
            MySetsAdapter.ItemClick += MySetsAdapter.OnItemRowClick;
            RecyclerMainList.SetAdapter(MySetsAdapter);
        }

        public void PassItemClassSetsFromAddClassSet(ClassSetModel model, bool isSave = true)
        {
            if (isSave)
            {
                ListClassTokSets.Insert(0, model);

                AssignRecyclerAdapter();
            }
            else
            {
                //If Update
                var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
                if (result != null) //If Edit
                {
                    int ndx = ListClassTokSets.IndexOf(result);
                    ListClassTokSets.Remove(result);
                    ListClassTokSets.Insert(ndx, model);
                }
                AssignRecyclerAdapter();
            }
        }

        public void deleteItemClassSet(ClassSetModel model)
        {
            var result = ListClassTokSets.FirstOrDefault(c => c.Id == model.Id);
            if (result != null) //If Edit
            {
                int ndx = ListClassTokSets.IndexOf(result);
                ListClassTokSets.Remove(result);
            }
            AssignRecyclerAdapter();
        }
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            //Data Refresh Place  
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.RunWorkerAsync();
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            RecyclerMainList.ContentDescription = null;
            if (isAddToSet)
            {
                this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                                isPublicFeed: isPublicFeed,
                                filterBy: filterBy,
                                filterByItems: filterByItems,
                                isAddtoktoSet: true
                                ));
            }
            else
            {
                if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity))
                {
                    this.RunOnUiThread(async () => await Initialize());
                }
                else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                {
                    this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                                isPublicFeed: isPublicFeed,
                                filterBy: filterBy,
                                filterByItems: filterByItems,
                                isAddtoktoSet: true
                                ));
                }
            }
            Thread.Sleep(1000);
        }


        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            var alertDiag = new AlertDialog.Builder(Instance);
            Dialog diag;
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }

            bundle.PutInt("position", position);

            PopupMenu menu = new PopupMenu(this, v);
            menu.Inflate(Resource.Menu.myclasssets_popmenu);
            var itemViewTokInfo = menu.Menu.FindItem(Resource.Id.itemViewTokInfo);
            var itemAddClassSetToGroup = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
            var itemViewClassToks = menu.Menu.FindItem(Resource.Id.viewClassToks);
            var itemReplicate = menu.Menu.FindItem(Resource.Id.itemReplicate);
            var itemEdit = menu.Menu.FindItem(Resource.Id.itemEdit);
            var itemDelete = menu.Menu.FindItem(Resource.Id.itemDelete);
            var itemPlayTokCards = menu.Menu.FindItem(Resource.Id.itemPlayTokCards);
            var itemPlayTokChoice = menu.Menu.FindItem(Resource.Id.itemPlayTokChoice);
            var itemTokMatch = menu.Menu.FindItem(Resource.Id.itemTokMatch);
            var itemReport = menu.Menu.FindItem(Resource.Id.itemReport);
            var itemManageClassToks = menu.Menu.FindItem(Resource.Id.itemManageClassSetToGroup);
            itemReport.SetVisible(false);

            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
            {
                itemViewTokInfo.SetVisible(true);
                itemAddClassSetToGroup.SetVisible(false);
                itemReplicate.SetVisible(false);
                itemEdit.SetVisible(false);
                itemDelete.SetVisible(false);
                itemPlayTokCards.SetVisible(false);
                itemPlayTokChoice.SetVisible(false);
                itemTokMatch.SetVisible(false);
                itemManageClassToks.SetVisible(false);
                itemViewClassToks.SetVisible(false);
            }
            else
            {
                itemViewTokInfo.SetVisible(false);
                itemAddClassSetToGroup.SetVisible(true);
                itemReplicate.SetVisible(true);
                itemEdit.SetVisible(true);
                itemDelete.SetVisible(true);
                itemPlayTokCards.SetVisible(true);
                itemPlayTokChoice.SetVisible(true);
                itemTokMatch.SetVisible(true);
            }

            // A menu item was clicked:
            menu.MenuItemClick += (s1, arg1) =>
            {
                Intent nextActivity; string modelConvert;
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "edit":
                        nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
                        modelConvert = JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("ClassTokSetsModel", modelConvert);
                        nextActivity.PutExtra("isSave", false);
                        this.StartActivity(nextActivity);
                        break;
                    case "delete":
                        alertDiag.SetTitle("Confirm");
                        alertDiag.SetMessage("Are you sure you want to continue?");
                        alertDiag.SetPositiveButton("Cancel", (senderAlert, args) =>
                        {
                            alertDiag.Dispose();
                        });
                        alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                        {
                            ProgressCircle.IndeterminateDrawable.SetColorFilter(Color.LightBlue, PorterDuff.Mode.Multiply);
                            ProgressText.Text = "Deleting set...";
                            LinearProgress.Visibility = ViewStates.Visible;
                            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

                            await ClassService.Instance.DeleteClassSetAsync(MyClassTokSetsFragment.Instance.ListClassTokSets[position].Id, MyClassTokSetsFragment.Instance.ListClassTokSets[position].PartitionKey);

                            LinearProgress.Visibility = ViewStates.Gone;
                            Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                            var dialogDelete = new AlertDialog.Builder(Instance);
                            var alertDelete = dialogDelete.Create();
                            alertDelete.SetTitle("");
                            alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                            alertDelete.SetMessage("Set deleted!");
                            alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                            {
                                MyClassTokSetsFragment.Instance.deleteItemClassSet(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                            });
                            alertDelete.Show();
                            alertDelete.SetCanceledOnTouchOutside(false);
                        });
                        diag = alertDiag.Create();
                        diag.Show();
                        break;
                    case "view class toks":
                        nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                        modelConvert = JsonConvert.SerializeObject(MySetsVm.MyToksAdapter.items[position]); //JsonConvert.SerializeObject(myclasstoksets_fragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("classsetModel", modelConvert);
                        this.StartActivity(nextActivity);
                        break;
                    case "play tok cards":
                        nextActivity = new Intent(this, typeof(TokCardsMiniGameActivity));
                        modelConvert = JsonConvert.SerializeObject(SetModel);
                        nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]));
                        this.StartActivity(nextActivity);
                        break;
                    case "play tok choice":
                        if (MyClassTokSetsFragment.Instance.ListClassTokSets[position].TokIds.Count > 3)
                        {
                            alertDiag = new AlertDialog.Builder(Instance);
                            alertDiag.SetTitle("Tok Choice");
                            alertDiag.SetMessage("Continue to Play Set?");
                            alertDiag.SetPositiveButton(Html.FromHtml("<font color='#dc3545'>Return</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                            {
                                alertDiag.Dispose();
                            });
                            alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Play</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                            {
                                nextActivity = new Intent(this, typeof(TokChoiceActivity));
                                modelConvert = JsonConvert.SerializeObject(SetModel);
                                nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]));
                                this.StartActivity(nextActivity);
                            });
                            diag = alertDiag.Create();
                            diag.Show();
                        }
                        else
                        {
                            var mssgDialog = new AlertDialog.Builder(Instance);
                            var alertMssg = mssgDialog.Create();
                            alertMssg.SetTitle("");
                            alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                            alertMssg.SetMessage("Tok Choice requires at least 4 toks in the set. Add more toks to play.");
                            alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                            alertMssg.Show();
                        }

                        break;
                    case "play tok match":
                        nextActivity = new Intent(this, typeof(TokMatchActivity));
                        modelConvert = JsonConvert.SerializeObject(SetModel);
                        nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]));
                        nextActivity.PutExtra("isSet", true);
                        this.StartActivity(nextActivity);
                        break;
                    case "view tok info":
                        nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
                        nextActivity.PutExtra("view_tok_info", true);
#if (_CLASSTOKS)
                        modelConvert = JsonConvert.SerializeObject(MySetsVm.MyToksAdapter.items[position]); //ClassTokResult
                        nextActivity.PutExtra("classtokModel", modelConvert);
#endif
#if (_TOKKEPEDIA)
                        modelConvert = JsonConvert.SerializeObject(MySetsVm.TokResult[position]);
                        nextActivity.PutExtra("tokModel", modelConvert);
#endif

                        this.StartActivityForResult(nextActivity, REQUEST_TOKINFO_RESULT);
                        break;
                }
            };
            
            // Menu was dismissed:
            menu.DismissEvent += (s2, arg2) =>
            {
                //Console.WriteLine("menu dismissed");
            };
            menu.Show();
        }

        private void InitAddToGroup() {
            if (isAddToSet)
                BtnAddToksToGroup.Visibility = ViewStates.Gone;

            if (!string.IsNullOrEmpty(groupId))
                BtnAddToksToGroup.Visibility = ViewStates.Visible;
            else
                BtnAddToksToGroup.Visibility = ViewStates.Gone;

            BtnAddToksToGroup.Click += delegate {
                if (isAddToSet)
                {

                }
                else { 
                
                }
            };
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == REQUEST_FILTER_RESULT) && (resultCode == Result.Ok) && (data != null))
            {
                isPublicFeed = data.GetBooleanExtra("isPublicFeed", false);
                filterBy = data.GetIntExtra("filterBy", 0);
                filterByItems = data.GetStringExtra("filterByList");
                bool setDesceding = data.GetBooleanExtra("sortTitle", true); ;
                string setReferenceIdSort = data.GetStringExtra("sortReferenceID");
                this.RunOnUiThread(async () => await MySetsVm.InitializeData(
                    isPublicFeed: isPublicFeed,
                    filterBy: filterBy,
                    filterByItems: filterByItems,
                    isAddtoktoSet: true,
                    sortByReference: setReferenceIdSort,
                    isDescending: setDesceding
                    ));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                MyClassSetsActivity.Instance.txtTotalToksSelected.Text = "";
            }
            catch (Exception ex) { }
        }

        public TextView txtHeader => this.FindViewById<TextView>(Resource.Id.txtHeader);
        public TextView txtMySetsPageTitle => this.FindViewById<TextView>(Resource.Id.txtMySetsPageTitle);
        public TextView BtnCancel => FindViewById<TextView>(Resource.Id.btnMySetsCancel);
        public TextView BtnAddClassSet => FindViewById<TextView>(Resource.Id.btnMySetsAdd);
        public LinearLayout LinearToolbar => this.FindViewById<LinearLayout>(Resource.Id.LinearSetsToolbar);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_mysetsprogress);
        public ProgressBar ProgressCircle => FindViewById<ProgressBar>(Resource.Id.progressbarMySets);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextMySets);
        public TextView MySetsPageTitle => this.FindViewById<TextView>(Resource.Id.txtMySetsPageTitle);
        public TextView TextNoSetsInfo => FindViewById<TextView>(Resource.Id.txtMySetsPageNoSets);
        public TextView txtTotalToksSelected => this.FindViewById<TextView>(Resource.Id.txtTotalToksSelected);

        public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgress);
        public RecyclerView RecyclerMainList => FindViewById<RecyclerView>(Resource.Id.recyclerView_mytoksets);
        public ShimmerLayout ShimmerLayout => FindViewById<ShimmerLayout>(Resource.Id.mysets_shimmer_view_container);
        public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFound);
        public ImageButton imageButtonFilterToks => FindViewById<ImageButton>(Resource.Id.imageButtonFilterToks);

        public Button BtnAddToksToGroup => FindViewById<Button>(Resource.Id.BtnAddToGroup);
    }
}