using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Custom;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Android.ViewModels;
using Tokket.Core;
using Xamarin.Essentials;
using Color = Android.Graphics.Color;
using XFragment = AndroidX.Fragment.App.Fragment;
using Result = Android.App.Result;
using Android.Graphics;

namespace Tokket.Android
{
    [Activity(Label = "Class Sets", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MyClassSetsActivity : BaseActivity
    {
        private AdapterFragmentX adapterFragment;
        private int REQUEST_TOKINFO_RESULT = 1001;
        internal static MyClassSetsActivity Instance { get; private set; }
        string tabheader = "My Class Sets", titleHeader = "";
        bool isAddToSet, isPublicClassSets; string toktypeid = "";
        Set SetModel; ClassTokModel ClassTokMode;
        int uiOptions, mainViewPagerPos = 0;
        Bundle bundle;
        ReportDialouge Report = null;
        ClassSetModel SelectedModel;
        List<XFragment> fragments = new List<XFragment>();
        List<string> fragmentTitles = new List<string>();
        public MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mysets_page);
            
            bundle = new Bundle();
            Instance = this;
            MySetsVm.Instance = Instance;
            Report = new ReportDialouge(this);
            toktypeid = Intent.GetStringExtra("TokTypeId").ToString();
            MySetsVm.TokTypeID = toktypeid;

            MySetsVm.LinearProgress = LinearProgress;
            MySetsVm.ProgressCircle = ProgressCircle;
            MySetsVm.ProgressText = ProgressText;

            isAddToSet = Intent.GetBooleanExtra("isAddToSet", true);
            MySetsVm.IsAddToksToSet = isAddToSet;

            isPublicClassSets = Intent.GetBooleanExtra("isPublicClassSets", false);
            MySetsVm.isPublicClassSets = isPublicClassSets;

            if (isPublicClassSets)
                tabheader = "Public Class Sets";

            BtnCancel.Click += delegate
            {
                MySetsVm.CancelSet();
            };

            //BtnAddClassSet.Click += delegate
            //{
            //    var nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
            //    this.StartActivity(nextActivity);
            //};
            
            LinearToolbar.SetBackgroundColor(Color.ParseColor("#3498db"));
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                LinearToolbar.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorPrimaryDark)));
            }

            setupViewPager(viewpager,tabheader);
            tabLayout.SetupWithViewPager(viewpager);

            btnSearch.Visibility = ViewStates.Gone;
            txtSearch.Visibility = ViewStates.Gone;

            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
            {
                //If opened from MySetsViewActivity
                if (isAddToSet)
                {
                    BtnAddClassSet.Text = "Add to Set";
                }
                else
                {
                    BtnAddClassSet.Text = "Remove from Set";
                }

                BtnAddClassSet.Click += async (s, e) =>
                {
                    if (isAddToSet)
                    {
                        await MySetsVm.AddSet();
                    }
                    else
                    {
                        await MySetsVm.RemoveToksFromSet();
                    }
                };

                tabheader = "Select a tok:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                SetModel = JsonConvert.DeserializeObject<Set>(Intent.GetStringExtra("classsetModel"));
                MySetsVm.SetModel = SetModel;
                txtMySetsPageTitle.Text = SetModel.Name;
            }
            else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets))
            {
                btnSearch.Visibility = ViewStates.Visible;
                txtSearch.Visibility = ViewStates.Visible;
                //BtnAddClassSet.Text = "+ Add Class Set";

                BtnAddClassSet.Click += async (s, e) =>
                {
                    await MySetsVm.AddSet();
                };

                if (isPublicClassSets)
                {
                    btnRemoveClassSet.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnRemoveClassSet.Visibility = ViewStates.Visible;
                }
            }
            else if (Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo))
            {
                ClassTokMode = JsonConvert.DeserializeObject<ClassTokModel>(Intent.GetStringExtra("classtokModel"));
                MySetsVm.tokList = ClassTokMode;
                txtMySetsPageTitle.Text = ClassTokMode.PrimaryFieldText;
                tabheader = "Select a class tok:";
                txtMySetsPageTitle.Visibility = ViewStates.Visible;

                BtnAddClassSet.Text = "Add to Class Set";
                BtnAddClassSet.Click += async (s, e) =>
                {
                    await MySetsVm.AddSet();
                };
            }

            btnSearch.Click += (s, e) =>
            {
                viewpager.Visibility = ViewStates.Gone;
                viewpagerSearchSets.Visibility = ViewStates.Visible;
                setupSearchViewPager(viewpagerSearchSets, titleHeader);
                tabLayout.SetupWithViewPager(viewpagerSearchSets);
            };

            InitFilterViewButtons();
        }
        
        void setupSearchViewPager(ViewPager viewPager, string title = "MY CLASS TOKS SETS")
        {
            //If search was only click for the first time
            if (btnSearch.Tag == null)
            {
                fragments[0] = new SetsSearchFragment("", ispublicsets: isPublicClassSets, issupersets: false);
                fragments[1] = new SetsSearchFragment("", ispublicsets: false, issupersets: true);
            }
            else
            {
                fragments.RemoveAt(mainViewPagerPos);
                fragments.Insert(mainViewPagerPos, new SetsSearchFragment("", ispublicsets: isPublicClassSets, issupersets: false));
            }
            
            setupSearchPager(viewPager);
            tabLayout.GetTabAt(mainViewPagerPos).Select();
        }

        private void setupSearchPager(ViewPager viewPager)
        {
            adapterFragment = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapterFragment;
            viewpagerSearchSets.Adapter.NotifyDataSetChanged();
            viewpagerSearchSets.PageSelected += (obj, e) => {
                mainViewPagerPos = e.Position;
                if (e.Position == 0)
                {
                    BtnAddClassSet.Text = "+ Add Class Set";
                }
                else if (e.Position == 1)
                {
                    BtnAddClassSet.Text = "+ Add Super Set";
                }
            };
        }
        
        void setupViewPager(ViewPager viewPager, string title = "MY CLASS TOKS SETS")
        {
            titleHeader = title;
            fragments = new List<XFragment>();
            fragmentTitles = new List<string>();

            fragments.Add(new MyClassTokSetsFragment("", ispublicsets: isPublicClassSets));
            fragments.Add(new MyClassTokSetsFragment("", ispublicsets: isPublicClassSets, issupersets: true));//Supersets
            //fragments.Add(new studysets_fragment(""));
            //fragments.Add(new convertible_fragment(""));

            fragmentTitles.Add(title);
            fragmentTitles.Add("Supersets");
            //fragmentTitles.Add("Study Sets");
            //fragmentTitles.Add("Convertible");

            setupViewPagerFragment(viewPager);

            btnTab1.Text = title;
            btnTab2.Text = "Supersets";
        }

        private void setupViewPagerFragment(ViewPager viewPager)
        {
            adapterFragment = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapterFragment;
            viewpager.Adapter.NotifyDataSetChanged();
            viewpager.PageSelected += (obj, e) => {
                mainViewPagerPos = e.Position;
                if (e.Position == 0)
                {
                    BtnAddClassSet.Text = "+ Add Class Set";
                }
                else if (e.Position == 1)
                {
                    BtnAddClassSet.Text = "+ Add Super Set";
                }
            };
        }

        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);
            var alertDiag = new AlertDialog.Builder(Instance);
            Dialog diag;
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }

            bundle.PutInt("position", position);
            int tokCount = 0;
            try {
                 tokCount = ((MyClassTokSetsFragment)page).ListClassTokSets[position].TokIds.Count;
            } catch { }
            PopupMenu menu = new PopupMenu(this, v);

            var itemTab = viewpager.CurrentItem;
            // Call inflate directly on the menu:
            if (itemTab != 2 && itemTab != 1)
            {
                menu.Inflate(Resource.Menu.myclasssets_popmenu);
                var itemViewTokInfo = menu.Menu.FindItem(Resource.Id.itemViewTokInfo);
                var itemViewClassTok = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
                var itemManageClasstok = menu.Menu.FindItem(Resource.Id.itemManageClassSetToGroup);
                var itemReplicate = menu.Menu.FindItem(Resource.Id.itemReplicate);
                var itemEdit = menu.Menu.FindItem(Resource.Id.itemEdit);
                var itemDelete = menu.Menu.FindItem(Resource.Id.itemDelete);
                var itemPlayTokCards = menu.Menu.FindItem(Resource.Id.itemPlayTokCards);
                var itemPlayTokChoice = menu.Menu.FindItem(Resource.Id.itemPlayTokChoice);
                var itemTokMatch = menu.Menu.FindItem(Resource.Id.itemTokMatch);
                var itemReport = menu.Menu.FindItem(Resource.Id.itemReport);

                if (Settings.ActivityInt == Convert.ToInt16(ActivityType.MySetsView))
                {
                    itemViewTokInfo.SetVisible(true);
                    itemViewClassTok.SetVisible(false);
                    itemReplicate.SetVisible(false);
                    itemEdit.SetVisible(false);
                    itemDelete.SetVisible(false);
                    itemPlayTokCards.SetVisible(false);
                    itemPlayTokChoice.SetVisible(false);
                    itemTokMatch.SetVisible(false);
                }
                else
                {
                    itemViewTokInfo.SetVisible(false);
                    itemViewClassTok.SetVisible(true);
                    itemReplicate.SetVisible(true);
                    itemEdit.SetVisible(true);
                    itemDelete.SetVisible(true);
                    itemPlayTokCards.SetVisible(true);
                    itemPlayTokChoice.SetVisible(true);
                    itemTokMatch.SetVisible(true);
                }

                if (page != null)
                {
                    if (((MyClassTokSetsFragment)page).ListClassTokSets[position].Id == Settings.GetTokketUser().Id)
                    {
                        itemReport.SetVisible(false);
                        itemReplicate.SetVisible(true);
                        itemEdit.SetVisible(true);
                        itemDelete.SetVisible(true);
                    }
                    else
                    {
                        itemReport.SetVisible(true);
                        itemReplicate.SetVisible(false);
                        itemEdit.SetVisible(false);
                        itemDelete.SetVisible(false);
                    }
                }
                else
                {
                    itemReport.SetVisible(true);
                    itemReplicate.SetVisible(false);
                    itemEdit.SetVisible(false);
                    itemDelete.SetVisible(false);
                }

                if (tokCount > 0)
                {
                    itemViewClassTok.SetVisible(false);
                    itemManageClasstok.SetVisible(true);
                }
                else
                {
                    itemViewClassTok.SetVisible(true);
                    itemManageClasstok.SetVisible(false);
                }
                // A menu item was clicked:
                menu.MenuItemClick += async (s1, arg1) =>
                {
                    Intent nextActivity; string modelConvert;
                    switch (arg1.Item.TitleFormatted.ToString().ToLower())
                    {
                        case "edit":
                            nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
                            modelConvert = JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
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

                                await ClassService.Instance.DeleteClassSetAsync(((MyClassTokSetsFragment)page).ListClassTokSets[position].Id, ((MyClassTokSetsFragment)page).ListClassTokSets[position].PartitionKey);

                                LinearProgress.Visibility = ViewStates.Gone;
                                Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                                var dialogDelete = new AlertDialog.Builder(Instance);
                                var alertDelete = dialogDelete.Create();
                                alertDelete.SetTitle("");
                                alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                                alertDelete.SetMessage("Set deleted!");
                                alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                                {
                                    ((MyClassTokSetsFragment)page).deleteItemClassSet(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
                                });
                                alertDelete.Show();
                                alertDelete.SetCanceledOnTouchOutside(false);
                            });
                            diag = alertDiag.Create();
                            diag.Show();
                            break;
                        case "manage class toks":
                            nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                            modelConvert = JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
                            nextActivity.PutExtra("classsetModel", modelConvert);
                            this.StartActivity(nextActivity);
                            break;
                        case "add class toks":
                            nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                            modelConvert = JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
                            nextActivity.PutExtra("classsetModel", modelConvert);
                            this.StartActivity(nextActivity);
                            break;
                        case "play tok cards":
                            if (((MyClassTokSetsFragment)page).ListClassTokSets[position].TokIds.Count > 0)
                            {
                                nextActivity = new Intent(this, typeof(TokCardsMiniGameActivity));
                                modelConvert = JsonConvert.SerializeObject(SetModel);
                                nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]));
                                this.StartActivity(nextActivity);
                            }
                            else
                            {
                                var mssgDialog = new AlertDialog.Builder(Instance);
                                var alertMssg = mssgDialog.Create();
                                alertMssg.SetTitle("");
                                alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                                alertMssg.SetMessage("Tok Cards requires at least 1 toks in the set. Add more toks to play.");
                                alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                                alertMssg.Show();
                            }

                            break;
                        case "play tok choice":
                            if (((MyClassTokSetsFragment)page).ListClassTokSets[position].TokIds.Count > 3)
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
                                    nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]));
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
                            if (((MyClassTokSetsFragment)page).ListClassTokSets[position].TokIds.Count > 2)
                            {
                                nextActivity = new Intent(this, typeof(TokMatchActivity));
                                modelConvert = JsonConvert.SerializeObject(SetModel);
                                nextActivity.PutExtra("classsetModel", JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]));
                                nextActivity.PutExtra("isSet", true);
                                this.StartActivity(nextActivity);
                            }
                            else
                            {
                                var mssgDialog = new AlertDialog.Builder(Instance);
                                var alertMssg = mssgDialog.Create();
                                alertMssg.SetTitle("");
                                alertMssg.SetIcon(Resource.Drawable.alert_icon_blue);
                                alertMssg.SetMessage("Tok Match requires at least 3 toks in the set. Add more toks to play.");
                                alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
                                alertMssg.Show();
                            }
                            break;

                        case "report":
                            SelectedModel = ((MyClassTokSetsFragment)page).ListClassTokSets[position];
                            Report.Show();
                            break;
                        case "view tok info":
                            nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
#if (_CLASSTOKS)
                            modelConvert = JsonConvert.SerializeObject(MySetsVm.ClassTokResult[position]);
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
            }
            else if (itemTab == 1) {
                menu.Inflate(Resource.Menu.myclasssets_popmenu);
                var itemViewTokInfo = menu.Menu.FindItem(Resource.Id.itemViewTokInfo);
                var itemViewClassTok = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
                var viewclasstoktItem = menu.Menu.FindItem(Resource.Id.viewClassToks);
                var itemManageClasstok = menu.Menu.FindItem(Resource.Id.itemManageClassSetToGroup);
                var itemReplicate = menu.Menu.FindItem(Resource.Id.itemReplicate);
                var itemEdit = menu.Menu.FindItem(Resource.Id.itemEdit);
                var itemDelete = menu.Menu.FindItem(Resource.Id.itemDelete);
                var itemPlayTokCards = menu.Menu.FindItem(Resource.Id.itemPlayTokCards);
                var itemPlayTokChoice = menu.Menu.FindItem(Resource.Id.itemPlayTokChoice);
                var itemTokMatch = menu.Menu.FindItem(Resource.Id.itemTokMatch);
                var itemReport = menu.Menu.FindItem(Resource.Id.itemReport);
                var itemAddClassSetToSuperSet = menu.Menu.FindItem(Resource.Id.AddClassSetToSuperSet);
                itemViewTokInfo.SetVisible(false);
                itemViewClassTok.SetVisible(false);
                itemManageClasstok.SetVisible(false);
                itemReplicate.SetVisible(false); 
                itemEdit.SetVisible(false);
                itemPlayTokCards.SetVisible(false); 
                itemPlayTokChoice.SetVisible(false);
                itemTokMatch.SetVisible(false);
                viewclasstoktItem.SetVisible(false);

                if (((MyClassTokSetsFragment)page).ListClassTokSets[position].UserId == Settings.GetTokketUser().Id)
                {
                    itemAddClassSetToSuperSet.SetVisible(true);
                    itemDelete.SetVisible(true);
                    itemEdit.SetVisible(true);
                    itemReport.SetVisible(false);
                }
                else {
                    itemAddClassSetToSuperSet.SetVisible(false);
                    itemReport.SetVisible(true);
                    itemDelete.SetVisible(false);
                }

                // A menu item was clicked:
                menu.MenuItemClick += async (s1, arg1) =>
                {
                    Intent nextActivity; string modelConvert;
                    switch (arg1.Item.TitleFormatted.ToString().ToLower())
                    {
                  
                        case "report":
                            SelectedModel = ((MyClassTokSetsFragment)page).ListClassTokSets[position];
                            Report.Show();
                            break;
                        case "add class set to superset":
                            
                            break;
                        case "edit":
                            nextActivity = new Intent(MainActivity.Instance, typeof(AddClassSetActivity));
                            modelConvert = JsonConvert.SerializeObject(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
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

                                await ClassService.Instance.DeleteClassSetAsync(((MyClassTokSetsFragment)page).ListClassTokSets[position].Id, ((MyClassTokSetsFragment)page).ListClassTokSets[position].PartitionKey);

                                LinearProgress.Visibility = ViewStates.Gone;
                                Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                                var dialogDelete = new AlertDialog.Builder(Instance);
                                var alertDelete = dialogDelete.Create();
                                alertDelete.SetTitle("");
                                alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                                alertDelete.SetMessage("Set deleted!");
                                alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                                {
                                    ((MyClassTokSetsFragment)page).deleteItemClassSet(((MyClassTokSetsFragment)page).ListClassTokSets[position]);
                                });
                                alertDelete.Show();
                                alertDelete.SetCanceledOnTouchOutside(false);
                            });
                            diag = alertDiag.Create();
                            diag.Show();
                            break;
                    }
                };

                // Menu was dismissed:
                menu.DismissEvent += (s2, arg2) =>
                {
                    //Console.WriteLine("menu dismissed");
                };
            }
            else
            {
                menu.Inflate(Resource.Menu.convertible_popupmenu);
                var itemCreateGameSet = menu.Menu.FindItem(Resource.Id.itemCreateGameSet);
                itemCreateGameSet.SetVisible(true);

                // A menu item was clicked:
                menu.MenuItemClick += (s1, arg1) =>
                {
                    Intent nextActivity; string modelConvert;
                    switch (arg1.Item.TitleFormatted.ToString().ToLower())
                    {
                        case "create game set":

                            break;

                    }
                };

                // Menu was dismissed:
                menu.DismissEvent += (s2, arg2) =>
                {
                    //Console.WriteLine("menu dismissed");
                };
            }
            menu.Show();
        }


        private async Task<List<ClassTokModel>> GetClassToksAsync(ClassSetModel classSetModel)
        {
            var list = new List<ClassTokModel>();
            try
            {
                var classtokRes = await ClassService.Instance.GetClassToksAsync(
                      new GetClassToksRequest
                      {
                          QueryValues =  new ClassTokQueryValues() { partitionkeybase = $"{classSetModel.Id}-classtoks", publicfeed = false }

                      });
              list = classtokRes.Results.ToList();
                //ClassSetVM.ClassSet = classSetModel;
            }
            catch (Exception ex) { }

            return list;
          
        }
        [Java.Interop.Export("onRadioButtonClicked")]
        public void onRadioButtonClicked(View view)
        {
            Report.ItemSelected(view);
        }

        [Java.Interop.Export("OnReport")]
        public async void OnReport(View view)
        {
            var alert = new AlertDialog.Builder(this);
            Report.ReportProgress.Visibility = ViewStates.Visible;
           
            if (!string.IsNullOrEmpty(Report.SelectedReportMessage) || Report.SelectedReportMessage != "Choose One")
            {
                Report r = new Report();
                r.ItemId = SelectedModel.Id;
                r.ItemLabel = SelectedModel.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = SelectedModel.UserId;
                r.UserId = Settings.GetTokketUser()?.Id;

                var result = await ReactionService.Instance.AddReport(r);

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    alert.SetTitle("Report Successful!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                else
                {
                    alert.SetTitle("Report Failed!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                alert.Show();
            }
            else
            {
                alert.SetTitle("Select a reason for the report!");
                alert.Show();
            }
            Report.ReportProgress.Visibility = ViewStates.Gone;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == REQUEST_TOKINFO_RESULT) && (resultCode == Result.Ok))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    MySetsVm.ClassTokResult.RemoveAt(bundle.GetInt("position"));
                    MySetsVm.AssignRecyclerToksAdapter(null, MySetsVm.ClassTokResult);
                }
            }
        }

       
        protected override void OnResume()
        {
            base.OnResume();
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }
        public override void OnBackPressed()
        {
            Console.WriteLine("");
        }
        bool OptionFilter=true; string TypeFilter = "";
        private void InitFilterViewButtons() {
            ShowFilterView.Click += delegate { 
                FilterView.Visibility = ViewStates.Visible; ShowFilterView.Visibility = ViewStates.Gone;
                txtSearch.Visibility = ViewStates.Gone;
                btnSearch.Visibility = ViewStates.Gone;
                btnTab1.Visibility = ViewStates.Gone;
                btnTab2.Visibility = ViewStates.Gone;
            };
            CloseFilter.Click += delegate { FilterView.Visibility = ViewStates.Gone; ShowFilterView.Visibility = ViewStates.Visible;
                if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets))
                {
                    txtSearch.Visibility = ViewStates.Visible;
                    btnSearch.Visibility = ViewStates.Visible;
                }

                btnTab1.Visibility = ViewStates.Visible;
                btnTab2.Visibility = ViewStates.Visible;
            };
            BtnPublic.SetTextColor(Color.Purple);
            BtnAll.SetTextColor(Color.Purple);
            BtnPublic.Click += delegate {
                OptionFilter = false;
                BtnPublic.SetTextColor(Color.Purple);
                BtnMyClassSets.SetTextColor(Color.Black);
                FindFilteredSets(OptionFilter,TypeFilter);
                setupViewPager(viewpager, "PUBLIC CLASS TOKS SETS");
                tabLayout.SetupWithViewPager(viewpager);
            };
            BtnMyClassSets.Click += delegate {
                OptionFilter = true;
                BtnPublic.SetTextColor(Color.Black);
                BtnMyClassSets.SetTextColor(Color.Purple);
                FindFilteredSets(OptionFilter, TypeFilter);

                setupViewPager(viewpager);
                tabLayout.SetupWithViewPager(viewpager);
            };

            BtnAll.Click += delegate {
                TypeFilter = string.Empty;
                BtnAll.SetTextColor(Color.Purple);
                btnPlayable.SetTextColor(Color.Black);
                btnNonPlayable.SetTextColor(Color.Black);
                FindFilteredSets(OptionFilter, TypeFilter);
            };
            
            btnPlayable.Click += delegate {
                TypeFilter = "Playable";
                BtnAll.SetTextColor(Color.Black);
                btnPlayable.SetTextColor(Color.Purple);
                btnNonPlayable.SetTextColor(Color.Black);
                FindFilteredSets(OptionFilter, TypeFilter);
            };
            btnNonPlayable.Click += delegate {
                TypeFilter = "Non-Playable";
                BtnAll.SetTextColor(Color.Black);
                btnPlayable.SetTextColor(Color.Black);
                btnNonPlayable.SetTextColor(Color.Purple);
                FindFilteredSets(OptionFilter, TypeFilter);
            };

            checkSetDeleteShow = false;
            checkSuperSetDeleteShow = false;
            btnRemoveClassSet.Click += delegate
            {
                var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);

                if (page != null)
                {
                    btnRemoveClassSet.Visibility = ViewStates.Gone;
                    BtnAddClassSet.Visibility = ViewStates.Gone;

                    btnDeleteSets.Visibility = ViewStates.Visible;
                    btnCancelDelete.Visibility = ViewStates.Visible;


                    if (viewpager.CurrentItem == 0)
                    {
                        checkSetDeleteShow = !checkSetDeleteShow;

                        ((MyClassTokSetsFragment)page).showCheckBoxView(checkSetDeleteShow);
                    }
                    else
                    {
                        checkSuperSetDeleteShow = !checkSuperSetDeleteShow;

                        ((MyClassTokSetsFragment)page).showCheckBoxView(checkSuperSetDeleteShow);
                    }
                }
            };

            btnCancelDelete.Click += delegate
            {
                CancelDeleteSets();
            };

            btnDeleteSets.Click += async(o, s) =>
            {
                var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);
                if (page != null)
                {
                    if (viewpager.CurrentItem == 0)
                    {
                        await ((MyClassTokSetsFragment)page).deleteClassSetStart();
                    }
                    else
                    {
                        await ((MyClassTokSetsFragment)page).deleteClassSetStart();
                    }
                }
            };

            btnTab1.Click += delegate
            {
                tabLayout.GetTabAt(0).Select();
            };

            btnTab2.Click += delegate
            {
                tabLayout.GetTabAt(1).Select();
            };
        }

        bool checkSetDeleteShow = false;
        bool checkSuperSetDeleteShow = false;
        public void CancelDeleteSets()
        {
            btnRemoveClassSet.Visibility = ViewStates.Visible;
            BtnAddClassSet.Visibility = ViewStates.Visible;

            btnDeleteSets.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;

            var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);
            if (page != null)
            {
                if (viewpager.CurrentItem == 0)
                {
                    checkSetDeleteShow = false;

                    ((MyClassTokSetsFragment)page).showCheckBoxView(false); //setBack to false
                }
                else
                {
                    checkSuperSetDeleteShow = false;

                    ((MyClassTokSetsFragment)page).showCheckBoxView(false); //setBack to false
                }
            }
        }
        private void FindFilteredSets(bool option, string type)
        {
            var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await ((MyClassTokSetsFragment)page).Initialize(string.Empty, option,type);

            });
        }

        public void tokFragmentPassItemClassSetsFromAddClassSet(ClassSetModel model, bool isSave = true, bool isDelete = false)
        {
            var page = SupportFragmentManager.FindFragmentByTag("android:switcher:" + viewpager.Id + ":" + viewpager.CurrentItem);

            if (page != null)
            {
                ((MyClassTokSetsFragment)page).PassItemClassSetsFromAddClassSet(model, isSave, isDelete);
            }
        }
        public TextView txtMySetsPageTitle => this.FindViewById<TextView>(Resource.Id.txtMySetsPageTitle);
        public TextView BtnCancel => FindViewById<TextView>(Resource.Id.btnMySetsCancel);
        public TextView BtnAddClassSet => FindViewById<TextView>(Resource.Id.btnMySetsAdd);
        public TextView btnRemoveClassSet => FindViewById<TextView>(Resource.Id.btnRemoveClassSet);
        public TextView btnDeleteSets => FindViewById<TextView>(Resource.Id.btnDeleteSets);
        public TextView btnCancelDelete => FindViewById<TextView>(Resource.Id.btnCancelDelete);
        public LinearLayout LinearToolbar => this.FindViewById<LinearLayout>(Resource.Id.LinearSetsToolbar);
        public LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.linear_mysetsprogress);
        public ProgressBar ProgressCircle => FindViewById<ProgressBar>(Resource.Id.progressbarMySets);
        public TextView ProgressText => FindViewById<TextView>(Resource.Id.progressBarTextMySets);
        public TextView MySetsPageTitle => this.FindViewById<TextView>(Resource.Id.txtMySetsPageTitle);
        public TabLayout tabLayout => this.FindViewById<TabLayout>(Resource.Id.tabMySets);
        public TextView TextNoSetsInfo => FindViewById<TextView>(Resource.Id.txtMySetsPageNoSets);
        public ViewPager viewpagerSearchSets => this.FindViewById<ViewPager>(Resource.Id.viewpagerSearchSets);
        public ViewPager viewpager => this.FindViewById<ViewPager>(Resource.Id.viewpagerMySets);
        public TextView txtTotalToksSelected => this.FindViewById<TextView>(Resource.Id.txtTotalToksSelected);
        public Button ShowFilterView => this.FindViewById<Button>(Resource.Id.btnShowFilters);
        public EditText txtSearch => this.FindViewById<EditText>(Resource.Id.txtSearch);
        public ImageButton btnSearch => this.FindViewById<ImageButton>(Resource.Id.btnSearch);
        public Button btnTab1 => FindViewById<Button>(Resource.Id.btnTab1);
        public Button btnTab2 => FindViewById<Button>(Resource.Id.btnTab2);
        //Filter 
        public View FilterView => this.FindViewById<View>(Resource.Id.filterLyout);
        public Button CloseFilter => FilterView.FindViewById<Button>(Resource.Id.btnclose);
        public Button BtnPublic => FilterView.FindViewById<Button>(Resource.Id.btnPublic);
        public Button BtnAll => FilterView.FindViewById<Button>(Resource.Id.btnAll);
        public Button BtnMyClassSets => FilterView.FindViewById<Button>(Resource.Id.btnMyclasssets);
        public Button btnPlayable => FilterView.FindViewById<Button>(Resource.Id.btnPlayable);
        public Button btnNonPlayable => FilterView.FindViewById<Button>(Resource.Id.btnNonPlayable);
    }
}