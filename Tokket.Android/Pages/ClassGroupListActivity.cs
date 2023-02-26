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
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Tokket.Shared.Extensions;
using Tokket.Android.ViewModels;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.AppCompat.Widget;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Result = Android.App.Result;
using AppCompatCheckBox = AndroidX.AppCompat.Widget.AppCompatCheckBox;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using Tokket.Android.Helpers;
using System.Text.RegularExpressions;

namespace Tokket.Android
{
    [Activity(Label = "Tok Groups", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ClassGroupListActivity : BaseActivity
    {
        string activityName = "ClassGroupListActivity", lastGroupKind = "", lastCallerName = "";
        internal static ClassGroupListActivity Instance { get; private set; }
        List<ClassGroupModel> itemsCheckedList;
        ObservableCollection<ClassGroupModel> ClassGroupCollection;
        GridLayoutManager mLayoutManager;
        bool isShareRequest = false, isShowCheckBox = false;
        ObservableRecyclerAdapter<ClassGroupModel, CachingViewHolder> adapterClassGroup;
        private enum ActivityName
        {
            Filter = 1001
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.classgroupslist_page);

            itemsCheckedList = new List<ClassGroupModel>();
            SetSupportActionBar(toolBar);

            setActivityTitle(title: this.GetString(Resource.String.tok_group));

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Title = "Tok Groups";
            Settings.ActivityInt = (int)ActivityType.GameCategoryMain;

            Instance = this;
            if (!string.IsNullOrEmpty(Intent.GetStringExtra("shareRequest")))
                isShareRequest = true;
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerClassGroupList.SetLayoutManager(mLayoutManager);
            RecyclerClassGroupList.ScrollChange += RecyclerScrollEvent;
            ClassGroupCollection = new ObservableCollection<ClassGroupModel>();

            //Remove currently cached data
            ClearCachedDataList();

            this.RunOnUiThread(async () => await Initialize(groupKind: "", callername: activityName + "owned"));

            AddClassGroupButton.Click += delegate
            {
                var nextActivity = new Intent(this, typeof(AddClassGroupActivity));
                this.StartActivity(nextActivity);
            };

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
         //   ButtonIndicator(btnAll);
            btnAll.Click += delegate
            {
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnStudy.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;
                Settings.FilterGroup = (int)GroupFilter.MyGroup;
                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for Groups...";
                
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("all");

                this.RunOnUiThread(async () => await Initialize(groupKind: "", callername: activityName + "all"));
            };

            btnClass.Click += delegate
            {
                //  ButtonIndicator(btnClass);
                btnAll.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnStudy.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;
                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for class...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("class");

                this.RunOnUiThread(async () => await Initialize(groupKind: "class", callername: activityName + "class"));
            };

            btnClubs.Click += delegate
            {
                btnClass.Checked = false;
                btnAll.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnStudy.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;
                updateRadioButtonStyle();

                //  ButtonIndicator(btnClubs);
                SearchView.QueryHint = "Search for clubs...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("club");
                this.RunOnUiThread(async () => await Initialize(groupKind: "club", callername: activityName + "club"));

                //TextNothingFound.Text = "No available clubs.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnTeams.Click += delegate
            {
                //  ButtonIndicator(btnTeams);
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnAll.Checked = false;
                btnChurch.Checked = false;
                btnStudy.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;
                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for teams...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("team");

                this.RunOnUiThread(async () => await Initialize(groupKind: "team", callername: activityName + "team"));

                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };
            btnChurch.Click += delegate
            {
                // ButtonIndicator(btnChurch);
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnAll.Checked = false;
                btnStudy.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;
                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for Church...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("church");

                this.RunOnUiThread(async () => await Initialize(groupKind: "church", callername: activityName + "church"));

                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };
            btnStudy.Click += delegate
            {
                // ButtonIndicator(btnStudy);
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnAll.Checked = false;
                btnOwnedGroups.Checked = false;
                btnFaith.Checked = false;

                Settings.FilterGroup = (int)GroupFilter.MyGroup;

                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for Study...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("study");

                this.RunOnUiThread(async () => await Initialize(groupKind: "study", callername: activityName + "study"));

                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnOwnedGroups.Click += delegate
            {
                // ButtonIndicator(btnStudy);
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnAll.Checked = false;
                btnStudy.Checked = false;
                btnFaith.Checked = false;
                updateRadioButtonStyle();
                Settings.FilterGroup = (int)GroupFilter.OwnGroup;

                SearchView.QueryHint = "Search for Owned Groups...";
                TextNothingFound.Visibility = ViewStates.Gone;

                //Preload UI groups first
                PreLoadUIGroups("owned");

                this.RunOnUiThread(async () => await Initialize(groupKind: "", callername: activityName + "owned"));

                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnFaith.Click += delegate
            {
                // ButtonIndicator(btnStudy);
                btnClass.Checked = false;
                btnClubs.Checked = false;
                btnTeams.Checked = false;
                btnChurch.Checked = false;
                btnAll.Checked = false;
                btnOwnedGroups.Checked = false;
                btnStudy.Checked = false;
                updateRadioButtonStyle();

                SearchView.QueryHint = "Search for Faith...";
                TextNothingFound.Visibility = ViewStates.Gone;

                PreLoadUIGroups("faith");

                this.RunOnUiThread(async () => await Initialize(groupKind: "faith", callername: activityName + "faith"));

                //TextNothingFound.Text = "No available teams.";
                //swipeRefreshRecycler.Visibility = ViewStates.Gone;
            };

            btnRemoveData.Click += delegate
            {
                btnRemoveData.Visibility = ViewStates.Gone;
                btnDelete.Visibility = ViewStates.Visible;
                btnCancelDelete.Visibility = ViewStates.Visible;
                isShowCheckBox = true;

                adapterClassGroup.NotifyDataSetChanged();
            };

            btnCancelDelete.Click += delegate
            {
                CancelDelete();
            };

            btnDelete.Click += async (o, s) =>
            {
                await DeleteStart();
            };
        }

        private void ClearCachedDataList()
        {
            Barrel.Current.Empty(activityName + "owned");
            Barrel.Current.Empty(activityName + "all");
            Barrel.Current.Empty(activityName + "class");
            Barrel.Current.Empty(activityName + "club");
            Barrel.Current.Empty(activityName + "team");
            Barrel.Current.Empty(activityName + "church");
            Barrel.Current.Empty(activityName + "study");
            Barrel.Current.Empty(activityName + "owned");
            Barrel.Current.Empty(activityName + "faith");
        }
        private async Task DeleteStart()
        {
            if (itemsCheckedList.Count() > 0)
            {
                showBlueLoading(this);
            }

            var itemList = itemsCheckedList.ToArray();
            for (int i = 0; i < itemList.Length; i++)
            {
                var item = itemList[i];
                if (item.isCheck)
                {
                    progressBarinsideText.Text = $"Deleting {i + 1} of {itemList.Count()}.";
                    var result = await ClassService.Instance.DeleteClassGroupAsync(item.Id, item.PartitionKey);
                    if (result)
                    {
                        ClassGroupCollection.Remove(item);

                        if (i == itemList.Count() - 1)
                        {
                            SetRecyclerAdapter();
                            CancelDelete();

                            List<ClassGroupModel> listGroups = new List<ClassGroupModel>();
                            foreach (var group in ClassGroupCollection)
                            {
                                listGroups.Add(group);
                            }
                            ClassService.Instance.SetCacheGroupGroupAsync(lastCallerName, listGroups);
                            await Task.Delay(1000);
                            hideBlueLoading(this);
                            progressBarinsideText.Text = "Loading...";
                        };
                    }
                }
            }
        }
        private void CancelDelete()
        {
            isShowCheckBox = false;
            btnRemoveData.Visibility = ViewStates.Visible;
            btnDelete.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;
            itemsCheckedList = new List<ClassGroupModel>();

            adapterClassGroup.NotifyDataSetChanged();
        }

        //private void ButtonIndicator(View view) {
        //    //btnAll.Background = null;;
        //    btnAll.Typeface = Typeface.Default;
        //    //btnClass.Background = null;
        //    btnClass.Typeface = Typeface.Default;
        //   // btnClubs.Background = null;
        //    btnClubs.Typeface = Typeface.Default;
        //   // btnTeams.Background = null;
        //    btnTeams.Typeface = Typeface.Default;
        //   // btnChurch.Background = null;
        //    btnChurch.Typeface = Typeface.Default;
        //   // btnStudy.Background = null;
        //    btnStudy.Typeface = Typeface.Default;

        //    var button = view as Button;
        //    button.SetBackgroundResource(Resource.Drawable.yellowStroke);
        //    button.Typeface = Typeface.DefaultBold;

        //}

        private void updateRadioButtonStyle()
        {
            linearDelete.Visibility = ViewStates.Gone;
            if (btnAll.Checked)
            {
                linearDelete.Visibility = ViewStates.Visible;
                btnAll.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnAll.Typeface = Typeface.Default;
            }

            if (btnClass.Checked)
            {
                btnClass.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnClass.Typeface = Typeface.Default;
            }

            if (btnClubs.Checked)
            {
                btnClubs.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnClubs.Typeface = Typeface.Default;
            }

            if (btnTeams.Checked)
            {
                btnTeams.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnTeams.Typeface = Typeface.Default;
            }

            if (btnChurch.Checked)
            {
                btnChurch.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnChurch.Typeface = Typeface.Default;
            }

            if (btnStudy.Checked)
            {
                btnStudy.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnStudy.Typeface = Typeface.Default;
            }

            if (btnOwnedGroups.Checked)
            {
                linearDelete.Visibility = ViewStates.Visible;
                btnOwnedGroups.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnOwnedGroups.Typeface = Typeface.Default;
            }

            if (btnFaith.Checked)
            {
                btnFaith.Typeface = Typeface.DefaultBold;
            }
            else
            {
                btnFaith.Typeface = Typeface.Default;
            }
        }

        private void RenameAllButton(GroupFilter filter) {
            switch (filter) {
                case GroupFilter.JoinedGroup:
                    btnAll.Text = "Joined Groups";
                    break;
                case GroupFilter.MyGroup:
                    btnAll.Text = "My Groups";
                    break;
                case GroupFilter.OwnGroup:
                    btnAll.Text = "Owned Groups";
                    break;
                case GroupFilter.AllGroup:
                    btnAll.Text = "All Groups";
                    break;
            }
        }

        private void RecyclerScrollEvent(object sender, View.ScrollChangeEventArgs e)
        {
            var layout = (sender as RecyclerView).GetLayoutManager();
            var manager = layout as GridLayoutManager;
            int totalItem = manager.ItemCount;
            int lastVisible = manager.FindLastVisibleItemPosition();
            bool endHasBeenReached = lastVisible +1 >= totalItem;
            if (lastVisible > 0 && endHasBeenReached)
            {
                LimitReminder.Visibility = ViewStates.Visible;
            }
            else {
                LimitReminder.Visibility = ViewStates.Gone;
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
        
        private void PreLoadUIGroups(string ext)
        {
            //Clear first
            ClassGroupCollection.Clear();

            var caller = activityName + ext;

            //Get cached data
            var cachedClassToks = ClassService.Instance.GetCachedClassGroupAsync(caller);
            if (cachedClassToks.Results != null)
            {
                var cacheList = cachedClassToks.Results.ToList();

                foreach (var item in cacheList)
                {
                    ClassGroupCollection.Add(item);
                }

                if (ClassGroupCollection.Count() > 0)
                {
                    SetRecyclerAdapter();
                }
            }

            //this.RunOnUiThread(async () => await Initialize(groupKind: _groupKind));

            /*if (ClassGroupCollection.Count() == 0)
            {
                linearProgress.Visibility = ViewStates.Visible;
                swipeRefreshRecycler.Visibility = ViewStates.Visible;
                RunOnUiThread(async () => await Initialize());
            };*/
        }
        private async Task Initialize(string pagination_id = "",string groupKind = "", string callername = "")
        {
            lastGroupKind = groupKind;
            lastCallerName = callername;

            GroupFilter filter = (GroupFilter)Settings.FilterGroup;

            //RenameAllButton(filter);
            ResultData<ClassGroupModel> results = new ResultData<ClassGroupModel>();
            
            int lastposition = 0;
            if (!string.IsNullOrEmpty(pagination_id))
            {
                lastposition = RecyclerClassGroupList.ChildCount - 1;
                showBottomDialog();
            }
            else
            {
                if (ClassGroupCollection.Count == 0)
                {
                    showBlueLoading(this);
                }
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
                        joined = false,
                        groupkind = groupKind

                    }, callername) ;
                    break;
                case GroupFilter.JoinedGroup:
                    results = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false,
                        groupkind = groupKind
                    }, callername);
                    break;
                case GroupFilter.MyGroup:
                    var myGroups = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = false,
                        startswith = false,
                        groupkind = groupKind
                    }, callername);

                    var joined = await ClassService.Instance.GetClassGroupAsync(new ClassGroupQueryValues()
                    {
                        paginationid = pagination_id,
                        partitionkeybase = "classgroups",
                        userid = Settings.GetTokketUser().Id,
                        joined = true,
                        startswith = false,
                        groupkind = groupKind
                    }, callername);

                    if (myGroups.Results != null)
                    {
                        var combined = myGroups.Results.ToList();
                        combined.AddRange(joined.Results);

                        results.Results = combined;
                    }
                    else
                    {
                        results.Results = new List<ClassGroupModel>();
                    }

                    break;           
            }
            if (!string.IsNullOrEmpty(pagination_id))
            {
                hideBottomDialog();
            }

            RecyclerClassGroupList.ContentDescription = results.ContinuationToken;

            List<ClassGroupModel> classgroupResult = new List<ClassGroupModel>();
            if (results.Results != null)
            {
                classgroupResult = results.Results.ToList();
            }

            if (string.IsNullOrEmpty(groupKind))
            {
                foreach (var item in classgroupResult)
                {
                    var hasitem = ClassGroupCollection.Where(i => i.Id == item.Id).FirstOrDefault() != null;
                    if (!hasitem)
                    {
                        ClassGroupCollection.Add(item);
                    }
                }

                /*foreach (var item in classgroupResult)
                {
                    ClassGroupCollection.Add(item);
                }*/

            }
            else {
                foreach (var item in classgroupResult)
                {
                    var kind = string.IsNullOrEmpty(item.GroupKind) ? "class" : item.GroupKind;
                    if (kind.ToLower() == "clubs")
                    {
                        kind = "club";
                    }
                    else if (kind.ToLower() == "teams")
                    {
                        kind = "team";
                    }
                    var hasitem = ClassGroupCollection.Where(i => i.Id == item.Id).FirstOrDefault() != null;
                    if (groupKind == kind.ToLower() && !hasitem)
                    {
                        ClassGroupCollection.Add(item);
                    }
                }
            }

            SetRecyclerAdapter();

            if (!string.IsNullOrEmpty(pagination_id))
            {
                RecyclerClassGroupList.ScrollToPosition(lastposition);
            }
            if (ClassGroupCollection.Count == 0) {
                switch (groupKind) {
                    case "club":
                        TextNothingFound.Text = "No available clubs.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "team":
                        TextNothingFound.Text = "No available teams.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "class":
                        TextNothingFound.Text = "No available class.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "study":
                        TextNothingFound.Text = "No available class.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    case "church":
                        TextNothingFound.Text = "No available class.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                    default:
                        TextNothingFound.Text = "No available groups.";
                        swipeRefreshRecycler.Visibility = ViewStates.Gone;
                        TextNothingFound.Visibility = ViewStates.Visible;
                        break;
                }
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
                userid = Settings.GetTokketUser().Id,
                text = searchText,
            });

            if (classgroupResult.Results != null)
            {
                foreach (var item in classgroupResult.Results.ToList())
                {
                    ClassGroupCollection.Add(item);
                }
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
            adapterClassGroup = ClassGroupCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.classgrouplist_row);
            adapterClassGroup.NotifyDataSetChanged();
            RecyclerClassGroupList.SetAdapter(adapterClassGroup);

            hideBlueLoading(this);
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, ClassGroupModel model, int position)
        {
            var chkBox = holder.FindCachedViewById<AppCompatCheckBox>(Resource.Id.chkBox);
            var ClassGroupHeader = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupHeader);
            var ClassGroupBody = holder.FindCachedViewById<TextView>(Resource.Id.TextClassGroupBody);
            var txtMembers = holder.FindCachedViewById<TextView>(Resource.Id.txtMembers);
            var txtGroupType = holder.FindCachedViewById<TextView>(Resource.Id.txtGroupType);
            var txtGroupDescription = holder.FindCachedViewById<TextView>(Resource.Id.txtGroupDescription);
            var constraintParent = holder.FindCachedViewById<ConstraintLayout>(Resource.Id.constraintParent);
            var imgClassGroupListImg = holder.FindCachedViewById<ImageView>(Resource.Id.imgClassGroupListImg);
            var btnJoinStatus = holder.FindCachedViewById<Button>(Resource.Id.btn_joinStatus);
            var groupUserProfile = holder.FindCachedViewById<ImageView>(Resource.Id.imageGroupImgUserPhoto);
            var groupUserName = holder.FindCachedViewById<TextView>(Resource.Id.lbl_groupOwnerName);
            var groupSchoolName = holder.FindCachedViewById<TextView>(Resource.Id.lbl_SchoolName);
            //Add + 1 because position alone does not seem to work
            int ndx = (position + 1) % Colors.Count;
            if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            constraintParent.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));

            constraintParent.Tag = position;

            ClassGroupHeader.Text = "Name: " + model.Name;
            txtMembers.Text = "Members: " + model.Members;
            txtGroupType.Text = model.GroupAccountType;
            txtGroupDescription.Text = model.Description;

            if (!string.IsNullOrEmpty(model.Image))
            {
                ClassGroupBody.Visibility = ViewStates.Gone;
                imgClassGroupListImg.Visibility = ViewStates.Visible;
                Glide.With(this).Load(model.ThumbnailImage).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(imgClassGroupListImg);
                
            }
            else
            {
                ClassGroupBody.Visibility = ViewStates.Visible;
                imgClassGroupListImg.Visibility = ViewStates.Gone;

                if (!string.IsNullOrEmpty(model.MiddleText))
                {
                    ClassGroupBody.Text = model.MiddleText;
                }
                else
                {
                    ClassGroupBody.Text = GetFirstThreeLetters(position, model);
                    ClassGroupCollection[position].MiddleText = ClassGroupBody.Text;
                }
            }

            if (model.UserId == Settings.GetUserModel().UserId)
            {
                btnJoinStatus.Visibility = ViewStates.Gone;
            }
            else {
                btnJoinStatus.Visibility = ViewStates.Visible;
                if (model.IsMember)
                {
                    btnJoinStatus.Text = "Joined";
                    btnJoinStatus.Click += delegate { };
                }
                else if (model.HasPendingRequest)
                {
                    btnJoinStatus.Text = "Requested";
                    btnJoinStatus.Click += delegate { };
                }
                else
                {
                    btnJoinStatus.Text = "Request";
                    btnJoinStatus.Click += delegate {
                        showAlertDialog(this, $"Do you want to request to join the group {model.Name}?", async (obj, eve) => {
                            var modelitem = new ClassGroupRequestModel();
                            modelitem.ReceiverId = model.UserId;
                            modelitem.SenderId = Settings.GetUserModel().UserId;
                            modelitem.Name = model.Name;
                            modelitem.Status = "0";
                            modelitem.Remarks = "Pending";
                            modelitem.Message = "Request to join group.";
                            modelitem.GroupId = model.Id;
                            modelitem.GroupPartitionKey = model.PartitionKey;
                            var result = await ClassService.Instance.RequestClassGroupAsync(modelitem);
                            if (result != null)
                            {
                                model.HasPendingRequest = true;
                                btnJoinStatus.Text = "Requested";
                            }
                            else {
                                showAlertDialog(this, "Something went wrong");
                            }
                        });

                    };
                }
            }
            groupUserName.Text = model.UserDisplayName;
            groupSchoolName.Text = model.School;
            try {
                Glide.With(this).Load(model.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(groupUserProfile);
            }
            catch (Exception ex) { }

            if (isShowCheckBox)
            {
                chkBox.Visibility = ViewStates.Visible;
            }
            else
            {
                chkBox.Visibility = ViewStates.Gone;
            }

            chkBox.Tag = position;
            chkBox.CheckedChange -= chkChange;
            chkBox.CheckedChange += chkChange;
        }

        private void chkChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var checkBox = sender as AppCompatCheckBox;
            int position = 0;
            try { position = (int)checkBox.Tag; } catch { position = int.Parse((string)checkBox.Tag); }

            if (ClassGroupCollection != null)
            {
                if (ClassGroupCollection[position].UserId == Settings.GetUserModel().UserId)
                {
                    ClassGroupCollection[position].isCheck = checkBox.Checked;

                    var result = itemsCheckedList.FirstOrDefault(c => c.Id == ClassGroupCollection[position].Id);
                    if (result != null) //If found
                    {
                        int ndx = itemsCheckedList.IndexOf(result);
                        itemsCheckedList.Remove(result);
                    }
                    else
                    {
                        if (checkBox.Checked) //Add checker to be sure
                        {
                            itemsCheckedList.Add(ClassGroupCollection[position]);
                        }
                    }
                }
                else
                {
                    (sender as AppCompatCheckBox).Checked = false;
                    Toast.MakeText(this, Resource.String.created_by_different_user, ToastLength.Long).Show();
                }
            }
        }

        private string GetFirstThreeLetters(int position, ClassGroupModel model, int length = 3)
        {
            string desc = model.Name;

            //Add try catch in case there's an error.
            try
            {
                desc = model.Name.Substring(0, length);
                //Check if item already exist.
                var hasitem = ClassGroupCollection.Where(i => i.MiddleText == desc).FirstOrDefault() != null;
                if (hasitem)
                {
                    //If item exist, increase length and generate a new one
                    GetFirstThreeLetters(position, model, length + 1);
                }
            }
            catch (Exception ex) { }

            //Get name based on length
            return desc.ToUpper();
        }
        [Java.Interop.Export("ItemRowClicked")]
        public void ItemRowClicked(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }
            // var resultGroupItem = await ClassService.Instance.GetClassGroupAsync(ClassGroupCollection[position].Id);
            if (!isShareRequest)
            {
                Intent nextActivity = new Intent(this, typeof(ClassGroupActivity));
                var modelConvert = JsonConvert.SerializeObject(ClassGroupCollection[position]);
                nextActivity.PutExtra("ClassGroupModel", modelConvert);
                this.StartActivity(nextActivity);
            }
            else {
                Intent intent = new Intent();
                var modelConvert = JsonConvert.SerializeObject(ClassGroupCollection[position]);
                intent.PutExtra("ClassGroupModel", modelConvert);
                SetResult(Result.Ok, intent);
                Finish();
            }
        
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_group, menu);

            var searchItem = menu.FindItem(Resource.Id.menu_search);
            var view = LayoutInflater.Inflate(Resource.Layout.custom_searchview, null); ;
            SearchView = view.FindViewById<SearchView>(Resource.Id.searchView);

            searchItem.SetActionView(SearchView);

            SearchManager  searchManager = (SearchManager)GetSystemService(Context.SearchService);
            //SearchView = (SearchView)menu.FindItem(Resource.Id.menu_search).ActionView;

            SearchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            SearchView.QueryHint = "Search for Groups...";
            SearchView.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                this.RunOnUiThread(async () => await SearchGroup(searchText: e.NewText));
                e.Handled = true;
            };
                   
            // change hint color
            TextView searchText = (TextView)SearchView.FindViewById(Resource.Id.search_src_text);
            searchText.SetTextColor(Color.White);
            searchText.SetHintTextColor(Color.LightGray);
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == (int)ActivityName.Filter && resultCode == Result.Ok) //Filter
            {
                ClassGroupCollection.Clear();
                RunOnUiThread(async () => await Initialize());
            }
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
            this.RunOnUiThread(async () => await Initialize(groupKind: lastGroupKind, callername: lastCallerName));
            Thread.Sleep(1000);
        }

        public ProgressBar bottomProgress => FindViewById<ProgressBar>(Resource.Id.bottomProgress);
        public FloatingActionButton AddClassGroupButton => FindViewById<FloatingActionButton>(Resource.Id.fab_AddClassGroup);
        public RecyclerView RecyclerClassGroupList => FindViewById<RecyclerView>(Resource.Id.RecyclerClassGroupList);
        public ProgressBar progressbar => FindViewById<ProgressBar>(Resource.Id.progressbar);
        public TextView progressBarinsideText => FindViewById<TextView>(Resource.Id.progressBarinsideText);
        public SwipeRefreshLayout swipeRefreshRecycler => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecycler);
        public RadioButton btnClass => FindViewById<RadioButton>(Resource.Id.btnClass);
        public RadioButton btnClubs => FindViewById<RadioButton>(Resource.Id.btnClubs);
        public RadioButton btnTeams => FindViewById<RadioButton>(Resource.Id.btnTeams);
  
        public RadioButton btnStudy => FindViewById<RadioButton>(Resource.Id.btnStudy);

        public RadioButton btnChurch => FindViewById<RadioButton>(Resource.Id.btnChurch);

        public RadioButton btnAll => FindViewById<RadioButton>(Resource.Id.btnAll);
        public RadioButton btnOwnedGroups => FindViewById<RadioButton>(Resource.Id.btnOwnedGroups); 
        public RadioButton btnFaith => FindViewById<RadioButton>(Resource.Id.btnFaith);
        public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFound);
        public LinearLayout LimitReminder => FindViewById<LinearLayout>(Resource.Id.limitStar);
        SearchView SearchView;

        public TextView btnRemoveData => FindViewById<TextView>(Resource.Id.btnRemoveOpportunity);
        public TextView btnDelete => FindViewById<TextView>(Resource.Id.btnDelete);
        public TextView btnCancelDelete => FindViewById<TextView>(Resource.Id.btnCancelDelete);
        public LinearLayout linearDelete => FindViewById<LinearLayout>(Resource.Id.linearDelete);

        public Button btnSort => FindViewById<Button>(Resource.Id.btnSort);
    }
}