using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using AppResult = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "AddRemoveSuperSetActivity")]
    public class AddRemoveSuperSetActivity : BaseActivity
    {
        internal static AddRemoveSuperSetActivity Instance { get; private set; }
        public List<ClassSetModel> ListClassTokSets;
        ClassSetModel superSetModel;
        List<ClassSetModel> selectedClassSetList;
        AddRemoveSetToFromSuperSetAdapter AddRemoveSetToFromSuperSetAdapter;
        MySetsAdapter MySetsAdapter;
        GestureDetector gesturedetector;
        Bundle bundle;
        bool isAddSets = true;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dialog_add_remove_setsToSuperSet);
            int numcol = 1;
            bundle = new Bundle();
            Instance = this;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            selectedClassSetList = new List<ClassSetModel>();
            gesturedetector = new GestureDetector(this, new MyGestureListener(this));
            var mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            recyclerView.SetLayoutManager(mLayoutManager);

            ListClassTokSets = new List<ClassSetModel>();

            btnClose.Click += delegate {
                Finish();
            };
            superSetModel = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("supersetModel"));
            isAddSets = Intent.GetBooleanExtra("isAddSets",true);
                InitSets();
            BtnAddSets.Enabled = false;
            BtnAddSets.Click += AddRemoveSets_Click;
            // Create your application here
        }

        private void AddRemoveSets_Click(object sender, EventArgs e)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            CancellationToken cancellationToken;
/*
            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });*/

            if (isAddSets)
            {
                foreach (var item in selectedClassSetList)
                {
                    superSetModel.Sets.Add(item);
                    superSetModel.SetIds.Add(item.Id);
                    superSetModel.SetPartitionKeys.Add(item.PartitionKey);
                }

                var builder = new AlertDialog.Builder(this)
                          .SetMessage($"Do you want to add this set to {superSetModel.Name}")
                          .SetPositiveButton("Okay", async (_, args) =>
                          {
                              bool result = true;
                              using (var cancellationTokenSource = new CancellationTokenSource())
                              {
                                  cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                                  cancellationToken = cancellationTokenSource.Token;

                                  result = await ClassService.Instance.UpdateClassSetAsync(superSetModel, cancellationToken);
                              }

                              this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                              LinearProgress.Visibility = ViewStates.Gone;

                              if (!result)
                              {
                                  var builder = new AlertDialog.Builder(this)
                          .SetMessage("Failed to add set")
                          .SetPositiveButton("Cancel", (_, args) =>
                          {

                          })
                          .SetCancelable(false)
                          .Show();
                              }
                              else
                              {
                                  string alertmessage = "";
                                  if (result)
                                  {
                                      alertmessage = "Class set Added.";
                                  }
                                  else
                                  {
                                      alertmessage = "Failed to add set.";
                                  }

                                  var builder = new AlertDialog.Builder(this);
                                  builder.SetMessage(alertmessage);
                                  builder.SetTitle("");
                                  var dialog = (AlertDialog)null;
                                  builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                                  {
                                      if (result)
                                      {
                                         // myclasstoksets_fragment.Instance.PassItemClassSetsFromAddClassSet(superSetModel, false);
                                          //Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                                          var resultData = JsonConvert.SerializeObject(selectedClassSetList);
                                          var updateSuperSet = JsonConvert.SerializeObject(superSetModel);
                                          Intent intent = new Intent();
                                          intent.PutExtra("classSetData", resultData);
                                          intent.PutExtra("updateSuperSet",updateSuperSet);
                                          SetResult(AppResult.Ok, intent);
                                          this.Finish();
                                      }
                                  });
                                  dialog = builder.Create();
                                  dialog.Show();
                                  if (result)
                                  {
                                      dialog.SetCanceledOnTouchOutside(false);
                                  }
                              }
                          })
                          .SetNegativeButton("Cancel", async (_, args) =>
                          {
                            
                          })
                          .SetCancelable(false)
                          .Show();

            }
            else {
                foreach (var item in selectedClassSetList)
                {
                    superSetModel.Sets.Remove(item);
                    superSetModel.SetIds.Remove(item.Id);
                    superSetModel.SetPartitionKeys.Remove(item.PartitionKey);
                }

                var builder = new AlertDialog.Builder(this)
                         .SetMessage($"Do you want to remove this set from {superSetModel.Name}")
                         .SetPositiveButton("Okay", async (_, args) =>
                         {
                             bool result = true;
                             using (var cancellationTokenSource = new CancellationTokenSource())
                             {
                                 cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                                 cancellationToken = cancellationTokenSource.Token;

                                 result = await ClassService.Instance.UpdateClassSetAsync(superSetModel, cancellationToken);
                             }

                             this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                             LinearProgress.Visibility = ViewStates.Gone;

                             if (!result)
                             {
                                 var builder = new AlertDialog.Builder(this)
                         .SetMessage("Failed to remove set")
                         .SetPositiveButton("Cancel", (_, args) =>
                         {

                         })
                         .SetCancelable(false)
                         .Show();
                             }
                             else
                             {
                                 string alertmessage = "";
                                 if (result)
                                 {
                                     alertmessage = "Class set removed.";
                                 }
                                 else
                                 {
                                     alertmessage = "Failed to remove set.";
                                 }

                                 var builder = new AlertDialog.Builder(this);
                                 builder.SetMessage(alertmessage);
                                 builder.SetTitle("");
                                 var dialog = (AlertDialog)null;
                                 builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                                 {
                                     if (result)
                                     {
                                          // myclasstoksets_fragment.Instance.PassItemClassSetsFromAddClassSet(superSetModel, false);
                                          //Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                                          var resultData = JsonConvert.SerializeObject(selectedClassSetList);
                                         var updateSuperSet = JsonConvert.SerializeObject(superSetModel);
                                         Intent intent = new Intent();
                                         intent.PutExtra("classSetData", resultData);
                                         intent.PutExtra("updateSuperSet", updateSuperSet);
                                         SetResult(AppResult.Ok, intent);
                                         this.Finish();
                                     }
                                 });
                                 dialog = builder.Create();
                                 dialog.Show();
                                 if (result)
                                 {
                                     dialog.SetCanceledOnTouchOutside(false);
                                 }
                             }
                         })
                         .SetNegativeButton("Cancel", async (_, args) =>
                         {

                         })
                         .SetCancelable(false)
                         .Show();
            }
        }

        void setRecycler(List<ClassSetModel> loadmore = null) {
            if (loadmore == null) {
                AddRemoveSetToFromSuperSetAdapter = new AddRemoveSetToFromSuperSetAdapter(ListClassTokSets, OnGridBackgroundTouched);
                recyclerView.SetAdapter(AddRemoveSetToFromSuperSetAdapter);
            } else {

                AddRemoveSetToFromSuperSetAdapter.UpdateItems(loadmore, recyclerView.ChildCount);
                recyclerView.SetAdapter(AddRemoveSetToFromSuperSetAdapter);
                recyclerView.ScrollToPosition(AddRemoveSetToFromSuperSetAdapter.ItemCount - loadmore.Count);
            }
          
        }

        async void InitSets() {
            if (isAddSets)
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

                    var setQueryValues = new ClassSetQueryValues()
                    {
                        userid = "",
                        groupid = "",
                        partitionkeybase = "classsets",
                        startswith = false,
                        paginationid = ""
                    };

                    //if (!string.IsNullOrEmpty(paginationId))
                    //{
                    //   showBottomProgress();
                    //}
                    //if (isSuperSets)
                    //    setQueryValues.label = "superset";
                    var result = await ClassService.Instance.GetClassSetAsync(setQueryValues, cancellationToken);

                    if (result == null)
                        result = new Tokket.Core.Tools.ResultData<ClassSetModel>();



                    //if (!string.IsNullOrEmpty(paginationId))
                    //{
                    //    hideBottomProgress();
                    //}

                    if (result.ContinuationToken == "cancelled")
                    {
                        shimmer_view_container.Visibility = ViewStates.Gone;
                        //showRetryDialog("Task was cancelled.");
                    }
                    else
                    {
                        foreach (var item in result.Results)
                        {
                            ListClassTokSets.Add(item);
                        }

                        //RecyclerMainList.ContentDescription = result.ContinuationToken;
                        //if (string.IsNullOrEmpty(type))
                        //{
                        //    ListClassTokSets.AddRange(result.Results.ToList());
                        //}
                        //else
                        //{
                        //    var filtersets = new List<ClassSetModel>();
                        //    foreach (var sets in result.Results.ToList())
                        //    {
                        //        if (sets.TokGroup == type)
                        //        {
                        //            filtersets.Add(sets);
                        //        }
                        //    }
                        //    ListClassTokSets.AddRange(filtersets);
                        //}

                        /*for (int i = 0; i < ListClassTokSets.Count; i++)
                        {
                            ClassSetViewModel ModelItem = new ClassSetViewModel();

                            var classtokRes = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues() { partitionkeybase = $"{ListClassTokSets[i].Id}-classtoks" }, cancellationToken);
                            if (classtokRes.ContinuationToken == "cancelled")
                            {
                                ShimmerLayout.Visibility = ViewStates.Gone;
                                showRetryDialog("Task was cancelled.");
                                return;
                            }
                            ModelItem.ClassToks = classtokRes.Results.ToList();
                            ModelItem.ClassSet = ListClassTokSets[i];
                            ListClassSetModel.Add(ModelItem);
                        }*/
                    }
                }

                shimmer_view_container.Visibility = ViewStates.Gone;
                setRecycler();
            }
            else {
                ListClassTokSets = superSetModel.Sets;
                shimmer_view_container.Visibility = ViewStates.Gone;
                BtnAddSets.Text = "Remove Sets";
                setRecycler();
            }
         
        }

        int selectedIndex = 0;
        private void OnGridBackgroundTouched(object sender, View.TouchEventArgs e)
        {
            selectedIndex = 0;
            var view = sender as View;
            try { selectedIndex = (int)view.Tag; } catch { selectedIndex = int.Parse((string)view.Tag); }
            //selectedClassSet = ListClassTokSets[selectedIndex];
            BtnAddSets.Enabled = true;
            gesturedetector.OnTouchEvent(e.Event);
        }

        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }

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

            itemViewClassToks.SetVisible(true);
            itemViewTokInfo.SetVisible(false);
            itemAddClassSetToGroup.SetVisible(false);
            itemReplicate.SetVisible(false);
            itemEdit.SetVisible(false);
            itemDelete.SetVisible(false);
            itemPlayTokCards.SetVisible(false);
            itemPlayTokChoice.SetVisible(false);
            itemTokMatch.SetVisible(false);
            itemManageClassToks.SetVisible(false);

            // A menu item was clicked:
            menu.MenuItemClick += (s1, arg1) =>
            {
                Intent nextActivity; string modelConvert;
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "view class toks":
                        nextActivity = new Intent(this, typeof(SuperSetInfoActivity));
                        modelConvert = JsonConvert.SerializeObject(AddRemoveSetToFromSuperSetAdapter.items[position]);
                        nextActivity.PutExtra("supersetModel", modelConvert);
                        MainActivity.Instance.StartActivity(nextActivity);
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

        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private AddRemoveSuperSetActivity activity;
            public MyGestureListener(AddRemoveSuperSetActivity Activity)
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
                activity.AddRemoveSetToFromSuperSetAdapter.selectedPosition = activity.selectedIndex;
                activity.AddRemoveSetToFromSuperSetAdapter.items[activity.selectedIndex].isCheck = !activity.AddRemoveSetToFromSuperSetAdapter.items[activity.selectedIndex].isCheck;

                if (activity.AddRemoveSetToFromSuperSetAdapter.items[activity.selectedIndex].isCheck == true)
                {
                    activity.selectedClassSetList.Add(activity.AddRemoveSetToFromSuperSetAdapter.items[activity.selectedIndex]);
                }
                else
                {
                    //If item is from true then it was set to false, remove it from list

                    var result = activity.selectedClassSetList.FirstOrDefault(c => c.Id == activity.AddRemoveSetToFromSuperSetAdapter.items[activity.selectedIndex].Id);
                    if (result != null)
                    {
                        int ndx = activity.selectedClassSetList.IndexOf(result);
                        activity.selectedClassSetList.Remove(result);
                    }
                }

                activity.recyclerView.Post(() =>
                {
                    activity.AddRemoveSetToFromSuperSetAdapter.NotifyDataSetChanged();
                });

               // End

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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //...

                //Handle sandwich menu icon click
                case HomeId:
                    //If menu is open, close it. Else, open it.
                    Finish();
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        public Button btnClose => FindViewById<Button>(Resource.Id.btnClose);
        //public Button btnFilter => FindViewById<Button>(Resource.Id.btnFilter);
        public ShimmerLayout shimmer_view_container => FindViewById<ShimmerLayout>(Resource.Id.shimmer_view_container);

        public Button BtnAddSets => FindViewById<Button>(Resource.Id.btnAddRemove);
        public RecyclerView recyclerView => FindViewById<RecyclerView>(Resource.Id.recyclerView);

        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public TextView TxtProgressText => FindViewById<TextView>(Resource.Id.txtProgressText);
     
    }
}