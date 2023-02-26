using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Preference;
using AndroidX.ViewPager.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.Tabs;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Shared.ViewModels;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using XFragment = AndroidX.Fragment.App.Fragment;
using XFragmentManager = AndroidX.Fragment.App.FragmentManager;
using Result = Android.App.Result;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class MyClassSetsViewActivity : BaseActivity
    {
        internal static MyClassSetsViewActivity Instance { get; private set; }
        string continuationToken;
        Intent nextActivity;
        public Set SetModel; 
        ClassSetModel classSetModel; ClassSetViewModel ClassSetVM;
        string groupId = ""; bool isAddToSet;
        int uiOptions, REQUEST_ADD_REMOVE_TOKS = 1000, fromActivity = -1;
        List<XFragment> fragments = new List<XFragment>();
        List<string> fragmentTitles = new List<string>();
        private int REQUEST_ADD_CLASS_SET = 1001;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mysetsview_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            
            /*var NavService = (NavigationService)SimpleIoc.Default.GetInstance<INavigationService>();
            var setdata = NavService.GetAndRemoveParameter<string>(Intent); //If Called from MySetsView after adding toks
            if (setdata == null)
            {
                classSetModel = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("classsetModel"));
                SetModel = classSetModel;

                ClassSetVM = new ClassSetViewModel();
                RunOnUiThread(async () => await GetClassToksAsync());
            }
            else
            {
                SetModel = JsonConvert.DeserializeObject<Set>(setdata);
            }*/
            var setdata = Intent.GetStringExtra("set"); //If Called from MySetsView after adding toks
            fromActivity = Settings.ActivityInt; //originally called from this activity
            if (setdata == null)
            {
                classSetModel = JsonConvert.DeserializeObject<ClassSetModel>(Intent.GetStringExtra("classsetModel"));
                SetModel = classSetModel;
                groupId = classSetModel.GroupId;

                if (classSetModel.UserId != Settings.GetUserModel().UserId) {
                    btnMySetsViewAdd.Visibility = ViewStates.Gone;
                    btnMySetsViewRemove.Visibility = ViewStates.Gone;
                }

                ClassSetVM = new ClassSetViewModel();
                RunOnUiThread(async () => await LoadToksAsync());
            }
            else
            {
                groupId = "";
                SetModel = JsonConvert.DeserializeObject<Set>(setdata);

                setupViewPager(viewpager);
                tabLayout.SetupWithViewPager(viewpager);
            }

            loadSetInfo();

            imgSet.Click += delegate
            {
                if (!string.IsNullOrEmpty(SetModel.Image))
                {
                    Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
                    Settings.byteImageViewer = SetModel.Image;
                    this.StartActivity(nextActivity);
                }
            };

            if (string.IsNullOrEmpty(lblMySetViewDescription.Text.Trim()))
            {
                lblMySetViewDescription.Visibility = ViewStates.Gone;
            }
            else
            {
                lblMySetViewDescription.Visibility = ViewStates.Visible;
            }

            lblMySetViewToksCnt.Text = SetModel.TokIds.Count().ToString();

            imgbtnSetsTokChoice.Visibility = ViewStates.Visible;

            //Add Toks
            btnMySetsViewAdd.Tag = 0;
            btnMySetsViewAdd.Text = "Add Class Toks to Set";
            btnMySetsViewAdd.Click -= AddRemoveToks;
            btnMySetsViewAdd.Click += AddRemoveToks;

            //Remove Toks
            btnMySetsViewRemove.Tag = 1;
            btnMySetsViewRemove.Text = "Remove Class Toks";
            btnMySetsViewRemove.Click -= AddRemoveToks;
            btnMySetsViewRemove.Click += AddRemoveToks;

            imgbtnSetsTokCards.Click += async(s, e) =>
            {
                if (ClassSetVM.ClassToks.Count > 0)
                {
                    if (ClassSetVM.ClassToks.Count != SetModel.TokIds.Count())
                    {
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                        txtProgress.Text = "Get toks " + ClassSetVM.ClassToks.Count().ToString() + " / " + SetModel.TokIds.Count().ToString();
                        constraintLoading.Visibility = ViewStates.Visible;
                        await GetRemainingToks();
                        constraintLoading.Visibility = ViewStates.Gone;
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                    }

                    if (ClassSetVM.ClassToks.Count == SetModel.TokIds.Count())
                    {
                        nextActivity = new Intent(MainActivity.Instance, typeof(TokCardsMiniGameActivity));
                        nextActivity.PutExtra("classTokModel", JsonConvert.SerializeObject(ClassSetVM.ClassToks));
                        nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(SetModel));
                        MainActivity.Instance.StartActivity(nextActivity);
                    }
                }
                else
                {
                    DialougePopUp("", "You need at least 1 tok to play.");
                }
            };

            imgbtnSetsTokMatch.Click += async(s, e) =>
            {
                if (ClassSetVM.ClassToks != null)
                {
                    if (ClassSetVM.ClassToks.Count > 2)
                    {
                        if (ClassSetVM.ClassToks.Count != SetModel.TokIds.Count())
                        {
                            this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                            txtProgress.Text = "Get toks " + ClassSetVM.ClassToks.Count().ToString() + " / " + SetModel.TokIds.Count().ToString();
                            constraintLoading.Visibility = ViewStates.Visible;
                            await GetRemainingToks();
                            constraintLoading.Visibility = ViewStates.Gone;
                            this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        }

                        if (ClassSetVM.ClassToks.Count == SetModel.TokIds.Count())
                        {
                            nextActivity = new Intent(MainActivity.Instance, typeof(TokMatchActivity));
                            nextActivity.PutExtra("classTokModel", JsonConvert.SerializeObject(ClassSetVM.ClassToks));
                            nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(SetModel));
                            MainActivity.Instance.StartActivity(nextActivity);
                        }
                    }
                    else
                    {
                        DialougePopUp("", "You need at least 3 tok to play.");
                    }
                }
            };

            imgbtnSetsTokChoice.Click += async(s, e) =>
            {
                if (ClassSetVM.ClassToks.Count > 3)
                {
                    if (ClassSetVM.ClassToks.Count != SetModel.TokIds.Count())
                    {
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);
                        txtProgress.Text = "Get toks " + ClassSetVM.ClassToks.Count().ToString() + " / " + SetModel.TokIds.Count().ToString();
                        constraintLoading.Visibility = ViewStates.Visible;
                        await GetRemainingToks();
                        constraintLoading.Visibility = ViewStates.Gone;
                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                    }

                    if (ClassSetVM.ClassToks.Count == SetModel.TokIds.Count())
                    {
                        nextActivity = new Intent(MainActivity.Instance, typeof(TokChoiceActivity));
                        nextActivity.PutExtra("classTokModel", JsonConvert.SerializeObject(ClassSetVM.ClassToks));
                        nextActivity.PutExtra("setModel", JsonConvert.SerializeObject(SetModel));
                        MainActivity.Instance.StartActivity(nextActivity);
                    }
                }
                else
                {
                    DialougePopUp("", "You need at least 4 tok to play.");
                }
            };
        }

        private void loadSetInfo()
        {
            this.Title = SetModel.Name;
            lblMySetViewTitle.Text = SetModel.Name;
            lblMySetViewTokGroup.Text = SetModel.TokGroup;
            lblMySetViewTokType.Text = SetModel.TokType;
            lblMySetViewDescription.Text = SetModel.Description;

            if (!string.IsNullOrEmpty(SetModel.Image))
            {
                Glide.With(this).Load(SetModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Animation.loader_animation).FitCenter()).Into(imgSet);
            }
        }
        
        private async Task LoadToksAsync()
        {
            try {
                showBlueLoading(this);
                var classtokRes = await GetClassToksAsync();
                continuationToken = classtokRes.ContinuationToken;
                ClassSetVM.ClassToks = classtokRes.Results.ToList();

                ClassSetVM.ClassSet = classSetModel;
            } catch (Exception ex) { }

            hideBlueLoading(this);

            setupViewPager(viewpager);
            tabLayout.SetupWithViewPager(viewpager);
        }
        public async Task<ResultData<ClassTokModel>> GetClassToksAsync(string paginationId = "")
        {
            var result = await ClassService.Instance.GetClassToksAsync(
                       new GetClassToksRequest
                       {
                           QueryValues = new ClassTokQueryValues()
                           {
                               partitionkeybase = $"{classSetModel.Id}-classtoks",
                               publicfeed = false,
                               paginationid = paginationId
                           }
                       }
                    );

            return result;
        }
        private void AddRemoveToks(object sender, EventArgs e)
        {
            isAddToSet = true;
            if ((int)(sender as Button).Tag == 1)
            {
                isAddToSet = false; // set false if button clicked is remove
            }

            Settings.ActivityInt = Convert.ToInt16(ActivityType.MySetsView);
            nextActivity = new Intent(this, typeof(AddToksToSetActivity));
            nextActivity.PutExtra("isAddToSet", isAddToSet);
            nextActivity.PutExtra("TokTypeId", SetModel.TokTypeId);
            nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(SetModel));
            nextActivity.PutExtra("isHideAddClassTokToGroup", true);
            this.StartActivityForResult(nextActivity, REQUEST_ADD_REMOVE_TOKS);

            /*nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsActivity));
            nextActivity.PutExtra("isAddToSet", isAddToSet);
            nextActivity.PutExtra("TokTypeId", SetModel.TokTypeId);
            nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(SetModel));
            this.StartActivity(nextActivity);*/
        }
        private async Task GetRemainingToks()
        {
            //Get all remaining toks
            var classtokRes = await GetClassToksAsync(continuationToken);
            continuationToken = classtokRes.ContinuationToken;
            ClassSetVM.ClassToks.AddRange(classtokRes.Results.ToList());

            txtProgress.Text = "Get toks " + ClassSetVM.ClassToks.Count().ToString() + " / " + SetModel.TokIds.Count().ToString();

            if (!string.IsNullOrEmpty(continuationToken))
            {
                await GetRemainingToks();
            }
        }

        private void DialougePopUp(string title = "", string message="")
        {
            var mssgDialog = new AlertDialog.Builder(Instance);
            var alertMssg = mssgDialog.Create();
            alertMssg.SetTitle(title);
            alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
            alertMssg.SetMessage(message);
            alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            alertMssg.Show();

        }
        void setupViewPager(ViewPager viewPager)
        {
            Settings.FilterTag = (int)FilterType.Set;

            fragments = new List<XFragment>();
            fragmentTitles = new List<string>();
            
            fragments.Add(new ClassToksFragment(groupId, _classSetId: classSetModel, _ClassSetVM: JsonConvert.SerializeObject(ClassSetVM), _isClassGroup: false) { ParentIntent = Intent }); //ClassSetVM.ClassSet.GroupId
            //fragments.Add(new myclasssets_tokcards_fragment());
            
            fragmentTitles.Add("Class Toks");
            //fragmentTitles.Add("Tok Cards");

            var adapter = new AdapterFragmentX(this.SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            viewpager.Adapter.NotifyDataSetChanged();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.superset_info_menu, menu);

            var itemEdit = menu.FindItem(Resource.Id.itemEdit);
            var itemDelete = menu.FindItem(Resource.Id.itemDelete);
            var itemReport = menu.FindItem(Resource.Id.itemReport);

            if (SetModel.UserId == Settings.GetTokketUser().Id)
            {
                itemEdit.SetVisible(true);
                itemDelete.SetVisible(true);
                itemReport.SetVisible(false);
            }
            else
            {
                itemEdit.SetVisible(false);
                itemDelete.SetVisible(false);
                itemReport.SetVisible(true);
            }

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();

                    if (Settings.ActivityInt != Convert.ToInt16(ActivityType.ClassGroupActivity))
                    {
                        /*//Go back to where the Left Menu is calling from originally
                        Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                        nextActivity = new Intent(this, typeof(MyClassSetsActivity));
                        nextActivity.PutExtra("isAddToSet", false);
                        nextActivity.PutExtra("TokTypeId", "");
                        nextActivity.SetFlags(ActivityFlags.ClearTop);
                        this.StartActivity(nextActivity);*/
                    }
                    
                    break;
                case Resource.Id.itemEdit:
                    var nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
                    var modelConvert = JsonConvert.SerializeObject(classSetModel);
                    nextActivity.PutExtra("ClassTokSetsModel", modelConvert);
                    nextActivity.PutExtra("isSave", false);
                    this.StartActivityForResult(nextActivity, REQUEST_ADD_CLASS_SET);

                    break;
                case Resource.Id.itemDelete:
                    RunOnUiThread(async () => {
                        showBlueLoading(this);

                        bool? result = null;
                        result = await ClassService.Instance.DeleteClassSetAsync(SetModel.Id, SetModel.PartitionKey);

                        hideBlueLoading(this);

                        if (result != null)
                        {
                            MyClassSetsActivity.Instance.tokFragmentPassItemClassSetsFromAddClassSet(classSetModel, false, true);
                            this.Finish();
                        }
                    });
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnResume()
        {
            base.OnResume();
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        protected override void OnDestroy()
        {
            Settings.ActivityInt = fromActivity;
            base.OnDestroy();
        }
        /*public override void OnBackPressed()
        {
            Console.WriteLine("");
        }*/
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((Settings.ActivityInt == (int)ActivityType.TokInfo) && (resultCode == Result.Ok))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    var tokModelstring = data.GetStringExtra("classtokModel");
                    if (tokModelstring != null)
                    {
                        var remaining = int.Parse(lblMySetViewToksCnt.Text)-1;
                        lblMySetViewToksCnt.Text = remaining.ToString();
                        //     lblMySetViewToksCnt.Text =
                    }
                }
            }
            else if ((requestCode == REQUEST_ADD_REMOVE_TOKS) && (resultCode == Result.Ok))
            {
                var tokidlist = data.GetStringExtra("tokidlist");
                if (!string.IsNullOrEmpty(tokidlist))
                {
                    var listIdToks = JsonConvert.DeserializeObject<List<ClassTokModel>>(tokidlist);
                    if (isAddToSet)
                    {
                        ClassSetVM.ClassToks.InsertRange(0, listIdToks);
                        lblMySetViewToksCnt.Text = ClassSetVM.ClassToks.Count().ToString();
                    }
                    else
                    {
                        foreach (var item in listIdToks)
                        {
                            var tokItem = ClassSetVM.ClassToks.FirstOrDefault(c => c.Id == item.Id);
                            if (tokItem != null)
                            {
                                int ndx = ClassSetVM.ClassToks.IndexOf(tokItem);
                                ClassSetVM.ClassToks.RemoveAt(ndx);
                            }
                        }
                    }

                    ClassToksFragment.Instance.refreshClassSetVMToks(ClassSetVM);
                }
            }
            else if((requestCode == REQUEST_ADD_CLASS_SET) && (resultCode == Result.Ok))
            {
                var dataValue = data.GetStringExtra("classsetModel");
                if (dataValue != null)
                {
                    classSetModel = JsonConvert.DeserializeObject<ClassSetModel>(dataValue);
                    SetModel = classSetModel;
                    loadSetInfo();

                    MyClassSetsActivity.Instance.tokFragmentPassItemClassSetsFromAddClassSet(classSetModel, false, false);
                }
            }
        }
     
        public TextView lblMySetViewTitle => FindViewById<TextView>(Resource.Id.lblMySetViewTitle);
        public TextView lblMySetViewTokGroup => FindViewById<TextView>(Resource.Id.lblMySetViewTokGroup);
        public TextView lblMySetViewTokType => FindViewById<TextView>(Resource.Id.lblMySetViewTokType);
        public TextView lblMySetViewDescription => FindViewById<TextView>(Resource.Id.lblMySetViewDescription);
        public TextView lblMySetViewToksCnt => FindViewById<TextView>(Resource.Id.lblMySetViewToksCnt);
        public Button btnMySetsViewAdd => this.FindViewById<Button>(Resource.Id.btnMySetsViewAdd);
        public Button btnMySetsViewRemove => this.FindViewById<Button>(Resource.Id.btnMySetsViewRemove);
        public ImageButton imgbtnSetsTokCards => this.FindViewById<ImageButton>(Resource.Id.imgbtnSetsTokCards);
        public ImageButton imgbtnSetsTokMatch => this.FindViewById<ImageButton>(Resource.Id.imgbtnSetsTokMatch); 
        public ImageButton imgbtnSetsTokChoice => this.FindViewById<ImageButton>(Resource.Id.imgbtnSetsTokChoice);
        public TabLayout tabLayout => this.FindViewById<TabLayout>(Resource.Id.tabLayoutMySetsViewToks);
        public ViewPager viewpager => this.FindViewById<ViewPager>(Resource.Id.viewpagerMySetsViewToks);
        public ConstraintLayout constraintLoading => this.FindViewById<ConstraintLayout>(Resource.Id.constraintLoading);
        public TextView txtProgress => this.FindViewById<TextView>(Resource.Id.txtProgress);
        public ImageView imgSet => this.FindViewById<ImageView>(Resource.Id.imgSet);
    }
}