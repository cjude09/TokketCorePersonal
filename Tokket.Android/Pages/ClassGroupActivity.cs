using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;
using AndroidX.ViewPager.Widget;
using Com.Airbnb.Lottie;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Tabs;
using ImageViews.Photo;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.CallBack;
using Tokket.Android.Custom;
using Tokket.Android.Fragments;
using Tokket.Android.Helpers;
using Tokket.Android.Interface;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Chat;
using Tokket.Shared.Services;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using static Android.App.ActionBar;
using AlertDialog = Android.App.AlertDialog;
using Result = Android.App.Result;
using NetUri = Android.Net.Uri;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using GalaSoft.MvvmLight.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Class Group", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ClassGroupActivity : BaseActivity, IOnStartDragListener
    {
        private int REQUEST_ACTIVITY = 1001;
        private int REQUEST_ADD_EXISTING_CLASS_TOK = 1002;
        Intent nextActivity;
        GlideImgListener GListener; float imgScale = 0;
        internal static ClassGroupActivity Instance { get; private set; }
        public AdapterFragmentX fragment { get; private set; }
        public ClassGroupModel model; Typeface font;
        public IMenuItem requesttojoin;
        public ReportDialouge Report = null;
        public ObservableCollection<ScheduleNotifViewModel> ScheduleNotifCollection { get; set; }
        ObservableCollection<FileModel> FileCollection { get; set; }
        RecyclerView recyclerSchedule;
        List<Fragment> fragments = new List<Fragment>();
        List<string> fragmentTitles = new List<string>();
        List<string> files = new List<string>();
        List<string> fileNames = new List<string>();
        List<string> fileExt = new List<string>();
        public ClassTokQueryValues CurrentClassToksQuery;
        public TokChatMessage SelectedTokChatMessage, EditedTokChat;
        public TokModel SelectedDoc;
        bool editChatMode = false;
        int cntDocAttached = 0;
        private ItemTouchHelper _mItemTouchHelper;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.classgroup_page);

            SetSupportActionBar(toolBar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            txtScheduleNotif.Visibility = ViewStates.Gone;
            Instance = this;
            Settings.ActivityInt = (int)ActivityType.ClassGroupActivity;
            model = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("ClassGroupModel"));

            setupViewPager(viewpager);
            tabLayout.SetupWithViewPager(viewpager);
            tabLayout.TabMode = TabLayout.ModeScrollable;

            imageBtnPreviousTab.Enabled = false;
            imageBtnPreviousTab.Click += delegate
            {
                setPagerAdapter(viewpager, viewpager.CurrentItem - 1);
            };

            imageBtnNextTab.Click += delegate
            {
                setPagerAdapter(viewpager, viewpager.CurrentItem + 1);
            };

            Report = new ReportDialouge(this);
            Initialize();

            font = Typeface.CreateFromAsset(this.Application.Assets, "fa_solid_900.otf");
            TextCheckRequestSent.SetTypeface(font, TypefaceStyle.Bold);

            viewpager.PageSelected -= Viewpager_PageSelected;
            viewpager.PageSelected += Viewpager_PageSelected;

            FloatingBtn.Click += (object sender, EventArgs e) =>
            {
                if (tabLayout.SelectedTabPosition == 0) //ClassToks
                {
                    nextActivity = new Intent(this, typeof(AddClassTokActivity));
                    var modelConvert = JsonConvert.SerializeObject(model);
                    nextActivity.PutExtra("ClassGroupModel", modelConvert);
                    this.StartActivity(nextActivity);
                }
                else if (tabLayout.SelectedTabPosition == 1) //Class Set
                {
                    nextActivity = new Intent(this, typeof(AddClassSetActivity));
                    var modelConvert = JsonConvert.SerializeObject(model);
                    nextActivity.PutExtra("ClassGroupModel", modelConvert);
                    this.StartActivity(nextActivity);
                }
                else if (tabLayout.SelectedTabPosition == 2) //Chat
                {
                }
                else if (tabLayout.SelectedTabPosition == 3) //Pics
                {
                    showAddPicDialog();
                }
                else if (tabLayout.SelectedTabPosition == 4) //Docs
                {
                    var dialog = new FileFinderDialog(this) { ParentContext = this, ClassGroupModel = model };
                    dialog.Show();
                }
                else if (tabLayout.SelectedTabPosition == 5)//Members
                {
                  
                }
                else if (tabLayout.SelectedTabPosition == 6) //Paks
                {
                    var intent = new Intent(this, typeof(AddTokPakActivity));
                    var group = JsonConvert.SerializeObject(model);
                    intent.PutExtra("GroupId", group);
                    StartActivity(intent);
                }
            };

            btnShowSchedule.Click += delegate
            {
                expandScheduleNotification("Schedule");
                GroupInfoLayout.Visibility = ViewStates.Gone;
                txtScheduleNotif.Visibility = ViewStates.Visible;
                btnShowSchedule.SetBackgroundColor(Color.Black);
                btnShowSchedule.SetTextColor(Color.White) ;

                btnShowNotif.SetBackgroundColor(Color.Transparent);
                btnShowNotif.SetTextColor(Color.Black);

                btnShowGroupInfo.SetBackgroundColor(Color.Transparent);
                btnShowGroupInfo.SetTextColor(Color.Black);
            };
            btnShowNotif.Click += delegate
            {
                expandScheduleNotification("Reminders");
                GroupInfoLayout.Visibility = ViewStates.Gone;
                txtScheduleNotif.Visibility = ViewStates.Visible;

                btnShowSchedule.SetBackgroundColor(Color.Transparent);
                btnShowSchedule.SetTextColor(Color.Black);

                btnShowNotif.SetBackgroundColor(Color.Black);
                btnShowNotif.SetTextColor(Color.White);

                btnShowGroupInfo.SetBackgroundColor(Color.Transparent);
                btnShowGroupInfo.SetTextColor(Color.Black);
            };

            btnShowGroupInfo.Click += delegate {
                GroupInfoLayout.Visibility = ViewStates.Visible;
                txtScheduleNotif.Visibility = ViewStates.Gone;

                btnShowSchedule.SetBackgroundColor(Color.Transparent);
                btnShowSchedule.SetTextColor(Color.Black);

                btnShowNotif.SetBackgroundColor(Color.Transparent);
                btnShowNotif.SetTextColor(Color.Black);

                btnShowGroupInfo.SetBackgroundColor(Color.Black);
                btnShowGroupInfo.SetTextColor(Color.White);
            };

            btnShowGroupInfo.SetBackgroundColor(Color.Black);
            btnShowGroupInfo.SetTextColor(Color.White);
            //ChatBox
            loadChatBox();
            chatButtonSend.Click += delegate
            {

                if (FileCollection.Count != cntDocAttached)
                {
                    Toast.MakeText(this, "Document attachment is still in progress...", ToastLength.Short).Show();
                    return;
                }

                var user = Settings.GetTokketUser();
                if (!editChatMode)
                {
                    var message = new TokChatMessage() { sender_id = user.Id, display_name = user.DisplayName, image = user.UserPhoto, message = chatTextComment.Text, group_id = model.Id, files = files, filesname = fileNames, filesextension = fileExt };
                    Task.WhenAll(ChatService.Instance.SendMessageTokChat(model.Id, message));
                    ClassGroupChatFragment.NewMessageBackUp = message;
                    ClassGroupChatFragment.Instance.addNewComment(message);
                }
                else {
                    EditedTokChat.message = chatTextComment.Text;
                    EditedTokChat.files = files;
                    EditedTokChat.filesname = fileNames;
                    EditedTokChat.filesextension = fileExt;
                    ClassGroupChatFragment.NewMessageBackUp = EditedTokChat;
                    Task.WhenAll(ChatService.Instance.UpdateMessage(model.Id,EditedTokChat));
                }
             
              
                hideKeyboard(chatTextComment);
                FileCollection.Clear();
                files.Clear();
                fileNames.Clear();
                fileExt.Clear();
                chatTextComment.Text = "";
                editChatMode = false;
            };

            chatAttach.Click += OnClickAttach;

            ImgGroup.Click += delegate
            {
                showImageViewer();
            };

            btnAddExistingClassTok.Click += delegate
            {
                var groupSerialized = JsonConvert.SerializeObject(model);
                var nextActivity = new Intent(this, typeof(AddExistingClassTokDialogActivity));
                nextActivity.PutExtra("GroupModel", groupSerialized);
                StartActivityForResult(nextActivity, REQUEST_ADD_EXISTING_CLASS_TOK);
            };
        }

        private async void OnClickAttach(object sender, EventArgs e)
        {
            Xamarin.Essentials.PickOptions pick = new Xamarin.Essentials.PickOptions();
          
            var file = await FileUploadService.Instance.FindPDFWithResult(pick);

            if (file == null)
            {
                ShowAlertLottieDialog("Invalid File Extension");
            }
            else if (file.FullPath.ToLower() == "error")
            {
                try
                {
                    var fileExtension = file.ContentType.Split("/")[1];
                    ShowAlertLottieDialog($"File type <{fileExtension}> not allowed.");
                }
                catch (Exception)
                {
                    ShowAlertLottieDialog("Invalid File Extension");
                }
            }
            else if (FileCollection.Count > 5) {
                ShowAlertLottieDialog("Reached limit for file upload");
            }
            else
            {
                LinearProgress.Visibility = ViewStates.Visible;
                byte[] bytes = File.ReadAllBytes(file.FullPath);
                string base64 = await FileService.ConverFileToBase64(file.FullPath);
                var fileSize = new FileInfo(file.FullPath).Length;
                //nFile.FileId = Guid.NewGuid().ToString().Replace("-", "");
                //nFile.Title = file.Name;
                //nFile.FileName = file.FileName;
                //nFile.FullPath = file.FileName;
                //nFile.FileType = file.ContentType;
                //nFile.Extension = ext;
                //nFile.Size = file.Length;
                //fileList.Add(nFile);

                var fileNameSplit = file.FileName.Split(".");
                var ext = string.Empty;
                if (fileNameSplit.Length > 1)
                    ext = fileNameSplit[fileNameSplit.Length - 1];

                FileModel fileModel = new FileModel()
                {
                    FileId = Guid.NewGuid().ToString().Replace("-", ""),
                    Title = " "+file.FileName,
                    FullPath = file.FullPath,
                    Extension = ext,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    Size = fileSize,
                    Base64 = base64
                };

                FileCollection.Add(fileModel);
            }

            LinearProgress.Visibility = ViewStates.Gone;
        }

        private void hideKeyboard(View v)
        {
            var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);
        }
        private void loadChatBox()
        {
            var tokketUser = Settings.GetTokketUser();
            var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
            if (!string.IsNullOrEmpty(cacheUserPhoto))
            {
                chatImgUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
            }
            else
            {
               // Glide.With(this).Load(tokketUser.UserPhoto).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(chatImgUserPhoto);
                Glide.With(this).Load(tokketUser.UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Animation.loader_animation).CircleCrop().FitCenter()).Into(chatImgUserPhoto);
            }
        }

        private void expandScheduleNotification(string type)
        {
            if (txtScheduleNotif.ContentDescription == type && txtScheduleNotif.Visibility == ViewStates.Visible)
            {
                txtScheduleNotif.Visibility = ViewStates.Gone;
                return;
            }
            else
            {
                txtScheduleNotif.Visibility = ViewStates.Visible;
            }

            txtScheduleNotif.ContentDescription = type;

            txtScheduleNotif.Text = "";
            switch (type.ToLower())
            {
                case "schedule":
                    if (model.Schedule != null)
                    {
                        for (int i = 0; i < model.Schedule.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(model.Schedule[i]))
                            {
                                if (i == 0)
                                {
                                    txtScheduleNotif.Text = "• " + model.Schedule[i];
                                }
                                else
                                {
                                    txtScheduleNotif.Text += "\n• " + model.Schedule[i];
                                }
                            }
                        }
                    }
                    else {
                        showAlertDialogs("No Schedule");
                    }
                    break;
                case "reminders":
                    if (model.Notifications != null)
                    {
                        for (int i = 0; i < model.Notifications.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(model.Notifications[i]))
                            {
                                if (i == 0)
                                {
                                    txtScheduleNotif.Text = "• " + model.Notifications[i];
                                }
                                else
                                {
                                    txtScheduleNotif.Text += "\n• " + model.Notifications[i];
                                }
                            }
                        }
                    }
                    else {
                        showAlertDialogs("No Notifcation");
                    }
                  
                    break;
            }
        }
        public void Initialize()
        {
            setActivityTitle(model.Name);

            TextName.Text =  model.Name;
            TextSchool.Text = model.School;
            TextDescription.Text = model.Description;

            GListener = new GlideImgListener();
            GListener.ParentActivity = this;

            ImgGroup.Visibility = ViewStates.Gone;
            txtGroupName.Visibility = ViewStates.Gone;
            if (string.IsNullOrEmpty(model.ThumbnailImage))
            {
                txtGroupName.Visibility = ViewStates.Visible;
                txtGroupName.Text = model.Name.Substring(0, 3);

                int ndx = 0;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.OrderBy(x => Guid.NewGuid() + "-" + new Random().Next(100000, 999999).ToString()).ToList();
                txtGroupName.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));
            }
            else
            {
                ImgGroup.Visibility = ViewStates.Visible;
                Glide.With(this).Load(model.ThumbnailImage).Listener(GListener).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(ImgGroup);
            }

        }
        void setupViewPager(ViewPager viewPager)
        {
            //var modelConvert = JsonConvert.SerializeObject(model);
            
            fragments.Add(new ClassToksFragment(model.Id, _isClassGroup: true) { ParentIntent = Intent });
            fragments.Add(new MyClassTokSetsFragment(model.Id));
            fragments.Add(new ClassGroupChatFragment(model,this));
            fragments.Add(new classgroup_pics_fragmentt(model));
            fragments.Add(new ClassGroupDocsFragment(model));

            fragmentTitles.Add("Toks");
            fragmentTitles.Add("Sets");
            fragmentTitles.Add("Chat");
            fragmentTitles.Add("Pics");
            fragmentTitles.Add("Docs");


            //if (model.UserId == Settings.GetUserModel().UserId)
            //{
            fragments.Add(new ClassGroupMemberFragment(model));
                fragmentTitles.Add("Members");
            //}
           
            fragments.Add(new ClassGroupTokPakFragment(model,Intent,this));
            fragmentTitles.Add("Paks");

            setPagerAdapter(viewPager);
        }
        private void setPagerAdapter(ViewPager viewPager, int position = 0)
        {
            var adapter = new AdapterFragmentX(SupportFragmentManager, fragments, fragmentTitles);
            viewPager.Adapter = adapter;
            viewPager.SetCurrentItem(position, true);
            viewpager.Adapter.NotifyDataSetChanged();
            fragment = adapter;

            if (viewpager.CurrentItem == 0)
            {
                imageBtnPreviousTab.Enabled = false;
            }
            else
            {
                imageBtnPreviousTab.Enabled = true;
            }

            if (viewpager.CurrentItem == fragments.Count - 1)
            {
                imageBtnNextTab.Enabled = false;
            }
            else
            {
                imageBtnNextTab.Enabled = true;
            }
        }
        [Java.Interop.Export("OnClickPopUpMenuST")]
        public void OnClickPopUpMenuST(View v)
        {
            int position = 0;
            try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }
            PopupMenu menu = new PopupMenu(this, v);

            int tokCount = 0;
            try
            {
                tokCount = MyClassTokSetsFragment.Instance.ListClassTokSets[position].TokIds.Count;
            }
            catch { }

            // Call inflate directly on the menu:
            menu.Inflate(Resource.Menu.myclasssets_popmenu);

            var viewTokInfo = menu.Menu.FindItem(Resource.Id.itemViewTokInfo);
            var addClassSetToGroup = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
            var replicate = menu.Menu.FindItem(Resource.Id.itemReplicate);
            var edit = menu.Menu.FindItem(Resource.Id.itemEdit);
            var delete = menu.Menu.FindItem(Resource.Id.itemDelete);
            var playTokCards = menu.Menu.FindItem(Resource.Id.itemPlayTokCards);
            var playTokChoice = menu.Menu.FindItem(Resource.Id.itemPlayTokChoice);
            var tokMatch = menu.Menu.FindItem(Resource.Id.itemTokMatch);
            var itemViewClassTok = menu.Menu.FindItem(Resource.Id.itemAddClassSetToGroup);
            var itemManageClasstok = menu.Menu.FindItem(Resource.Id.itemManageClassSetToGroup);
            var viewClassToks = menu.Menu.FindItem(Resource.Id.viewClassToks);

            viewTokInfo.SetVisible(false);
            addClassSetToGroup.SetVisible(false);
            replicate.SetVisible(false);
            playTokCards.SetVisible(false);
            playTokChoice.SetVisible(false);
            tokMatch.SetVisible(false);
            edit.SetVisible(false);
            delete.SetVisible(false);
            viewClassToks.SetVisible(false);

            if (MyClassTokSetsFragment.Instance.ListClassTokSets[position].UserId == Settings.GetUserModel().UserId)
            {
                edit.SetVisible(true);
                delete.SetVisible(true);
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

            }
            else if (!string.IsNullOrEmpty(MyClassTokSetsFragment.Instance.ListClassTokSets[position].GroupId) && model.UserId == Settings.GetUserModel().UserId)
            {
                delete.SetVisible(true);
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
            }
            if (model.UserId != Settings.GetTokketUser().Id) {
                itemViewClassTok.SetVisible(false);
                itemManageClasstok.SetVisible(false);
                viewClassToks.SetVisible(true);
            }

            // A menu item was clicked:
            menu.MenuItemClick += (s1, arg1) =>
            {
                Intent nextActivity; string modelConvert;
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "edit":
                        nextActivity = new Intent(this, typeof(AddClassSetActivity));
                        modelConvert = JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("ClassTokSetsModel", modelConvert);
                        nextActivity.PutExtra("isSave", false);
                        this.StartActivity(nextActivity);
                        break;
                    case "schedule":
                        break;
                    case "reminders":
                        break;
                    case "delete":
                        var alertDiag = new AlertDialog.Builder(Instance);
                        Dialog diag;
                        alertDiag.SetTitle("Confirm");
                        alertDiag.SetMessage("Are you sure you want to continue?");
                        alertDiag.SetPositiveButton("Cancel", (senderAlert, args) =>
                        {
                            alertDiag.Dispose();
                        });
                        alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                        {
                            TextProgressStatus.Text = "Deleting set...";
                            LinearProgress.Visibility = ViewStates.Visible;
                            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

                            var result = await ClassService.Instance.DeleteClassSetAsync(MyClassTokSetsFragment.Instance.ListClassTokSets[position].Id, MyClassTokSetsFragment.Instance.ListClassTokSets[position].PartitionKey);

                            LinearProgress.Visibility = ViewStates.Gone;
                            Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);

                            string message = "Deleted successfully!";
                            if (result)
                                message = "Failed to delete!";

                            var dialogDelete = new AlertDialog.Builder(Instance);
                            var alertDelete = dialogDelete.Create();
                            alertDelete.SetTitle("");
                            alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                            alertDelete.SetMessage(message);
                            alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                            {
                                if (result)
                                {
                                    MyClassTokSetsFragment.Instance.deleteItemClassSet(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                                }
                            });
                            alertDelete.Show();
                            alertDelete.SetCanceledOnTouchOutside(false);
                        });
                        diag = alertDiag.Create();
                        diag.Show();
                        break;
                    case "manage class toks":
                        nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                        modelConvert = JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("classsetModel", modelConvert);
                        this.StartActivity(nextActivity);
                        break;
                    case "add class toks":
                        nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                        modelConvert = JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("classsetModel", modelConvert);
                        this.StartActivity(nextActivity);
                        break;
                    case "view class toks":
                        nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                        modelConvert = JsonConvert.SerializeObject(MyClassTokSetsFragment.Instance.ListClassTokSets[position]);
                        nextActivity.PutExtra("classsetModel", modelConvert);
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

        private void showAlertDialogs(string message) {

            var dialogDelete = new AlertDialog.Builder(this);
            var alertD = dialogDelete.Create();
            alertD.SetTitle("");
            alertD.SetIcon(Resource.Drawable.alert_icon_blue);
            alertD.SetMessage(message);
            alertD.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
            {

            });
            alertD.Show();
            alertD.SetCanceledOnTouchOutside(false);
        }

        private void showScheduleNotificationDialog(int typeSelected = 0)
        {
            var popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.dialog_classgroup_schedule_notification);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog.Show();

            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            popupDialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            // Access Popup layout fields like below
            var btnUpdate = popupDialog.FindViewById<Button>(Resource.Id.btnUpdate);
            var btnClose = popupDialog.FindViewById<Button>(Resource.Id.btnClose);
            var spinnerType = popupDialog.FindViewById<Spinner>(Resource.Id.spinnerType);
            var progress = popupDialog.FindViewById<LinearLayout>(Resource.Id.linearProgress_dialogue);
            recyclerSchedule = popupDialog.FindViewById<RecyclerView>(Resource.Id.recyclerSchedule);

            // Events for that popup layout  
            btnClose.Click += delegate
            {
                popupDialog.Dismiss();
            };

            btnUpdate.Click += async(s, e) =>
            {
                var listItem = new List<string>();

                progress.Visibility = ViewStates.Visible;
                foreach (var item in ScheduleNotifCollection)
                {
                    if (!string.IsNullOrEmpty(item.stringText))
                    {
                        listItem.Add(item.stringText);
                    }
                }

                if (listItem.Count > 0)
                {
                    if (spinnerType.SelectedItemPosition == 0) //Schedule
                    {
                        model.Schedule = listItem.ToArray();
                    }
                    else if (spinnerType.SelectedItemPosition == 1) //Notification
                    {
                        model.Notifications = listItem.ToArray();
                    }

                    //TODO: Need to create a dialogProgress and put it in BaseActivity to be accessed by all
                    //LinearProgress.Visibility = ViewStates.Visible;
                    //this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                    var result = await ClassService.Instance.UpdateClassGroupAsync(model);
                    //this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                    //LinearProgress.Visibility = ViewStates.Gone;

                    string alertmssg = "Failed to update.";
                    if (result)
                    {
                        alertmssg = "Updated Successfully.";
                    }
                    progress.Visibility = ViewStates.Gone;
                    var dialog = new AlertDialog.Builder(popupDialog.Context);
                    var alertDialog = dialog.Create();
                    alertDialog.SetTitle("");
                    alertDialog.SetIcon(Resource.Drawable.alert_icon_blue);
                    alertDialog.SetMessage(alertmssg);
                    alertDialog.SetButton((int)(DialogButtonType.Positive), Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                    {
                        if (result)
                        {
                            txtScheduleNotif.Visibility = ViewStates.Gone;
                            alertDialog.Dismiss();
                            popupDialog.Dismiss();
                        
                        }
                    });
                    alertDialog.Show();

                    alertDialog.SetCanceledOnTouchOutside(false);
                }
                else {
                    progress.Visibility = ViewStates.Gone;
                     showAlertDialogs("Fill up box");
                }
            };

            loadSpinnerType(spinnerType);
            spinnerType.SetSelection(typeSelected);
           // InitializeScheduleNotif(spinnerType.GetItemAtPosition(typeSelected).ToString());
            recyclerSchedule.SetLayoutManager(new LinearLayoutManager(this));
            //setRecyclerScheduleNotifAdapter(recyclerSchedule);
        }

        private void showFileUpdaloaded() {
            FileRecycle.SetLayoutManager(new LinearLayoutManager(this));
            setFileRecyclerAdapter(FileRecycle);
        }

        private void InitializeScheduleNotif(String headerText)
        {
            ScheduleNotifCollection = new ObservableCollection<ScheduleNotifViewModel>();

            for (int i = 0; i < 10; i++)
            {
                var stringItem = new ScheduleNotifViewModel();
                stringItem.stringHeader = headerText + " " + (i + 1);

                ScheduleNotifCollection.Add(stringItem);
            }

            setRecyclerScheduleNotifAdapter(recyclerSchedule);
        }
        private void setRecyclerScheduleNotifAdapter(RecyclerView recyclerView)
        {
            var adapterDetail = new AddSchedNotifAdapter(this,ScheduleNotifCollection,this); //ScheduleNotifCollection.GetRecyclerAdapter(BindScheduleNotif, Resource.Layout.string_notification_row);

            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(adapterDetail);
            _mItemTouchHelper = new ItemTouchHelper(callback);
            _mItemTouchHelper.AttachToRecyclerView(recyclerView);
            recyclerView.SetAdapter(adapterDetail);
        }

        public void RefreshRecyclerSchedNotif() {
            var adapterDetail = new AddSchedNotifAdapter(this, ScheduleNotifCollection, this); //ScheduleNotifCollection.GetRecyclerAdapter(BindScheduleNotif, Resource.Layout.string_notification_row);

            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(adapterDetail);
            _mItemTouchHelper = new ItemTouchHelper(callback);
            _mItemTouchHelper.AttachToRecyclerView(recyclerSchedule);
            recyclerSchedule.SetAdapter(adapterDetail);
        }

        private void setFileRecyclerAdapter(RecyclerView recyclerView) {
            var adapterDetail = FileCollection.GetRecyclerAdapter(BindFileCollection,Resource.Layout.filedata_row);
            recyclerView.SetAdapter(adapterDetail);
        }

        private void BindFileCollection(RecyclerView.ViewHolder holder, FileModel model, int position)
        {
            var fileName = holder.ItemView.FindViewById<TextView>(Resource.Id.lblFileName);
            var removeBtn = holder.ItemView.FindViewById<Button>(Resource.Id.removeItem);
            var fileProgress = holder.ItemView.FindViewById<ProgressBar>(Resource.Id.fileUploadProgress);
            var iconComplete = holder.ItemView.FindViewById<ImageView>(Resource.Id.iconComplete);
            var fileUploadResult = new FileUploadResult();
            var token = new CancellationTokenSource();
            removeBtn.Text = "Cancel";
            cntDocAttached = 0;
            fileProgress.SetProgress(0,false);
            MainThread.BeginInvokeOnMainThread(async()=> {
              
                fileUploadResult = await FileUploadService.Instance.UploadFileAsync(model, token.Token);
                if (fileUploadResult != null) {
                    if(!editChatMode)
                        files.Add(fileUploadResult.FileLink);
                    fileProgress.SetProgress(100, true);
                    await Task.Delay(500);
                    removeBtn.Text = "Remove";
                    cntDocAttached += 1;
                    fileProgress.Visibility = ViewStates.Gone;
                    iconComplete.Visibility = ViewStates.Visible;
                }
             
            });

            if (!editChatMode)
            {
                fileNames.Add(model.FileName);
                fileExt.Add(model.Extension);
            }
          
          
            fileName.Text = model.Title;
            removeBtn.Click += (s, e) => {
                FileCollection.Remove(model);
                if (!editChatMode)
                    files.Remove(fileUploadResult.FileLink);
                else
                    files.Remove(model.FileUrl);
                fileNames.Remove(model.FileName);
                fileExt.Remove(model.Extension);
                if (token != null)
                    token.Cancel();
            };
        }

        private void BindScheduleNotif(CachingViewHolder holder, ScheduleNotifViewModel model, int position)
        {
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = holder.FindCachedViewById<EditText>(Resource.Id.txtItem);
           

            txtItem.Hint = model.stringHeader;
            txtHeader.Text = model.stringHeader;
            //txtItem.Text = model.stringText;

            var contentBinding = new Binding<string, string>(model,
                                                  () => model.stringText,
                                                  txtItem,
                                                  () => txtItem.Text,
                                                  BindingMode.TwoWay);
        }
        public RecyclerView.ViewHolder getRecyclerSchedNotifViewHolder(int position)
        {
            return recyclerSchedule.FindViewHolderForAdapterPosition(position);
        }
        public void loadSpinnerType(View v)
        {
            Spinner spinner = (Spinner)v;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerType_ItemSelected);
            List<string> spinnerTypeList = new List<string>();

            spinnerTypeList.Add("Schedule");
            spinnerTypeList.Add("Reminders");
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, spinnerTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinner.Adapter = Aadapter;
        }
        private void spinnerType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var typeSelected = spinner.GetItemAtPosition(e.Position).ToString();
            switch (typeSelected.ToLower())
            {
                case "schedule":
                    break;
                case "reminders":
                    break;
            }

            InitializeScheduleNotif(typeSelected);
        }

        public void showImageViewer()
        {
            Bitmap imgTokMojiBitmap = ((BitmapDrawable)ImgGroup.Drawable).Bitmap;
            MemoryStream byteArrayOutputStream = new MemoryStream();
            imgTokMojiBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
            byte[] byteArray = byteArrayOutputStream.ToArray();

            Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
            Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
            this.StartActivity(nextActivity);
        }
        private void Viewpager_PageSelected(object sender,  ViewPager.PageSelectedEventArgs e)
        {
            appBarLayout.SetExpanded(true);
            layoutChatBox.Visibility = ViewStates.Gone;
            FloatingBtn.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.colorAccent);
            btnAddExistingClassTok.Visibility = ViewStates.Gone;
            switch (e.Position)
            {
                case 0://Class Tok
                    FloatingBtn.Visibility = ViewStates.Visible;
                    btnAddExistingClassTok.Visibility = ViewStates.Visible;
                    break;
                case 1://Class Set
                    FloatingBtn.Visibility = ViewStates.Visible;
                    break;
                case 2://Chat
                    try {
                        FileCollection = new ObservableCollection<FileModel>();
                        layoutChatBox.Visibility = ViewStates.Visible;
                        FloatingBtn.Visibility = ViewStates.Gone;
                        showFileUpdaloaded();
                    } catch (Exception ex) { }
                    
                    break;
                case 3://Pics
                    FloatingBtn.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.limegreen);
                    FloatingBtn.Visibility = ViewStates.Visible;
                    break;
                case 4://Docs
                    FloatingBtn.Visibility = ViewStates.Visible;
                 
                    break;
                case 5:// Tok Members
                    FloatingBtn.Visibility = ViewStates.Gone;
                    break;
                case 6: //Paks
                    FloatingBtn.Visibility = ViewStates.Visible;
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.classgroup_menu, menu);

            var item_menu = menu.FindItem(Resource.Id.item_menu);
            var inviteUsers = menu.FindItem(Resource.Id.menu_InviteUsers);
            requesttojoin = menu.FindItem(Resource.Id.menu_requesttojoin);
            var delete = menu.FindItem(Resource.Id.menu_delete);
            var edit = menu.FindItem(Resource.Id.menu_edit);
            var report = menu.FindItem(Resource.Id.menu_report);
            var requests = menu.FindItem(Resource.Id.menu_requests);
            var removeUsers = menu.FindItem(Resource.Id.menu_RemoveUsers);         
            if (model.HasPendingRequest)
            {
                requesttojoin.SetTitle("Waiting for approval");
            }
            else if (model.IsMember)
            {
                requesttojoin.SetTitle("Leave Group");
                //BtnRequestToJoin.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.ParseColor("#dc3545"));
            }
            else
            {
                //BtnRequestToJoin.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.ParseColor("#007bff"));
                requesttojoin.SetTitle("Request to Join");
            }
            removeUsers.SetVisible(false);
            if (model.UserId == Settings.GetUserModel().UserId)
            {
                item_menu.SetTitle("MANAGE");
                menu.SetGroupVisible(Resource.Id.group1, true);
                inviteUsers.SetVisible(true);
                delete.SetVisible(true);
                edit.SetVisible(true);
                requests.SetVisible(true);
             //   removeUsers.SetVisible(true);
                report.SetVisible(false);
                requesttojoin.SetVisible(false);
            }
            else
            {
                item_menu.SetTitle("ACTIONS");
                menu.SetGroupVisible(Resource.Id.group1, false);
                inviteUsers.SetVisible(false);
                delete.SetVisible(false);
              
                edit.SetVisible(false);
                report.SetVisible(true);
                requests.SetVisible(false);
            }

            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            AlertDialog.Builder alertDiag;
            Dialog diag;
            string modelConvert;
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
                case Resource.Id.menu_requesttojoin:
                    if (model.IsMember)
                    {
                        alertDiag = new AlertDialog.Builder(this);
                        alertDiag.SetTitle("Confirm");
                        alertDiag.SetMessage("Are you sure you want to leave from this group?");
                        alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                            alertDiag.Dispose();
                        });
                        alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Leave</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                        {
                            TextProgressStatus.Text = "Leaving...";
                            LinearProgress.Visibility = ViewStates.Visible;

                            var result = await ClassService.Instance.LeaveClassGroupAsync(model.Id, model.PartitionKey);
                            LinearProgress.Visibility = ViewStates.Gone;
                            TextProgressStatus.Text = "Loading...";

                            if (result)
                            {
                                item.SetTitle("Request to Join");
                            }
                        });
                        diag = alertDiag.Create();
                        diag.Show();
                        diag.SetCanceledOnTouchOutside(false);
                    }
                    else if (model.HasPendingRequest == false)
                    {
                        alertDiag = new AlertDialog.Builder(this);
                        alertDiag.SetTitle("Confirm");
                        alertDiag.SetMessage("Are you sure you want to join in this group?");
                        alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                            alertDiag.Dispose();
                        });
                        alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Join</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) =>
                        {
                            TextProgressStatus.Text = "Requesting...";
                            LinearProgress.Visibility = ViewStates.Visible;

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
                            LinearProgress.Visibility = ViewStates.Gone;
                            TextProgressStatus.Text = "Loading...";

                            if (result != null)
                            {
                                model.HasPendingRequest = true;
                                TextRequestStatus.Text = "Request Sent";
                                LinearRequestSent.Visibility = ViewStates.Visible;
                                item.SetTitle("Request Sent");
                            }
                        });
                        diag = alertDiag.Create();
                        diag.Show();
                        diag.SetCanceledOnTouchOutside(false);
                    }
                    break;
                case Resource.Id.menu_report:
                    Report.Show();
                    break;
                case Resource.Id.menu_InviteUsers:
                    nextActivity = new Intent(this, typeof(InviteUsersActivity));
                    modelConvert = JsonConvert.SerializeObject(model);
                    nextActivity.PutExtra("ClassGroupModel", modelConvert);
                    this.StartActivity(nextActivity);
                    break;
                case Resource.Id.menu_RemoveUsers:
                    
                    break;
                case Resource.Id.menu_delete:
                    alertDiag = new AlertDialog.Builder(this);
                    alertDiag.SetTitle("Confirm");
                    alertDiag.SetMessage("Are you sure you want to delete this group?");
                    alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                        alertDiag.Dispose();
                    });
                    alertDiag.SetNegativeButton(Html.FromHtml("<font color='#dc3545'>Delete</font>", FromHtmlOptions.ModeLegacy), async (senderAlert, args) => {
                        string message = "";
                        LinearProgress.Visibility = ViewStates.Visible;
                        this.Window.AddFlags(WindowManagerFlags.NotTouchable);

                        var issuccess = await ClassService.Instance.DeleteClassGroupAsync(model.Id, model.PartitionKey); //API

                        this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        LinearProgress.Visibility = ViewStates.Gone;

                        if (issuccess)
                        {
                            message = "Deleted the group successfully!";
                        }
                        else
                        {
                            message = "Failed to delete group!";
                        }

                        var dialogDelete = new AlertDialog.Builder(this);
                        var alertDelete = dialogDelete.Create();
                        alertDelete.SetTitle("");
                        alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertDelete.SetMessage(message);
                        alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                        {
                            ClassGroupListActivity.Instance.RemoveClassGroupCollection(model);
                            this.Finish();
                        });
                        alertDelete.Show();
                        alertDelete.SetCanceledOnTouchOutside(false);
                    });
                    diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                    break;
                case Resource.Id.menu_edit:
                    nextActivity = new Intent(this, typeof(AddClassGroupActivity));
                    modelConvert = JsonConvert.SerializeObject(model);
                    nextActivity.PutExtra("ClassGroupModel", modelConvert);
                    nextActivity.PutExtra("isSaving", false);
                    this.StartActivity(nextActivity);

                    break;
                case Resource.Id.menu_requests:
                    modelConvert = JsonConvert.SerializeObject(model);
                    nextActivity = new Intent(this, typeof(RequestsDialogActivity));
                    nextActivity.PutExtra("classGroupModel", modelConvert);
                    this.StartActivity(nextActivity);
                    break;
                case Resource.Id.menu_schedule:
                    showScheduleNotificationDialog(0);
                    break;
                case Resource.Id.menu_notification:
                    showScheduleNotificationDialog(1);
                    break;
            }
            return base.OnOptionsItemSelected(item);
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
                if (tabLayout.SelectedTabPosition == 0)//Toks
                {
                    r.ItemId = model.Id;
                    r.ItemLabel = model.Label;
                    r.Message = Report.SelectedReportMessage;
                    r.OwnerId = model.UserId;
                    r.UserId = Settings.GetTokketUser()?.Id;
                }
                else if (tabLayout.SelectedTabPosition == 1) { // Sets 
                
                }
                else if (tabLayout.SelectedTabPosition == 2) //Chat
                {
                    r.ItemId = SelectedTokChatMessage.id;
                    r.ItemLabel = SelectedTokChatMessage.label;
                    r.Message = Report.SelectedReportMessage;
                    r.OwnerId = SelectedTokChatMessage.id;
                    r.UserId = Settings.GetTokketUser()?.Id;
                }
                else if (tabLayout.SelectedTabPosition == 3) //Pics
                {

                }
                else if (tabLayout.SelectedTabPosition == 4) // Docs
                {
                    r.ItemId = SelectedDoc.Id;
                    r.ItemLabel = SelectedDoc.Label;
                    r.Message = Report.SelectedReportMessage;
                    r.OwnerId = SelectedDoc.Id;
                    r.UserId = Settings.GetTokketUser()?.Id;
                }
                else if (tabLayout.SelectedTabPosition == 5) //Members
                {

                }
                else if (tabLayout.SelectedTabPosition == 6) //Paks
                {

                }
              

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

        Dialog addPicDialog;
        TokModel tokPicModel = new TokModel();
        public void setTokPicModel(TokModel _tokPicModel)
        {
            tokPicModel = _tokPicModel;
        }
        public void showAddPicDialog(bool isSave = true, Drawable drawable = null,TokModel picModel = null)
          {
            addPicDialog = new Dialog(this);
            addPicDialog.SetContentView(Resource.Layout.dialog_addtok_pic);
            addPicDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            addPicDialog.Show();

            var txtAddPicLoading = addPicDialog.FindViewById<TextView>(Resource.Id.txtAddPicLoading);
            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            double widthD = getLayoutWidth();
            addPicDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
            addPicDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);
           

            if (isSave)
            {
                tokPicModel = new TokModel();
                btnAddPicSave.Text = "+ Add Pic";
                txtAddPicLoading.Text = "Saving...";
            }
            else
            {
                txtAddPicLoading.Text = "Updating...";
                btnAddPicSave.Text = "Update Pic";
                txtAddPicPrimaryText.Text = tokPicModel.PrimaryFieldText;
                imageAddPic.ContentDescription = tokPicModel.Image;
                imageAddPic.SetImageDrawable(drawable);
            }
            
            // Events for that popup layout  
            btnAddPicBrowse.Click += delegate
            {
                Settings.ActivityInt = (int)ActivityType.AddPicClassGroup;
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), REQUEST_ACTIVITY);

            };

            btnAddPicSave.Click += async (sender, e) =>
            {
                if (string.IsNullOrEmpty(imageAddPic.ContentDescription))
                {
                    ShowAlertLottieDialog("Upload error! No image selected.");
                    return;
                }
                else
                {
                    linearTokPicProgress.Visibility = ViewStates.Visible;
                    this.Window.AddFlags(WindowManagerFlags.NotTouchable);


                    ResultModel result = new ResultModel();
                    if (isSave)
                    {
                        var user = Settings.GetTokketUser();
                        //ClassGroupModel classGroup = await ClassService.Instance.GetClassGroupAsync(groupid);

                        tokPicModel.Image = imageAddPic.ContentDescription;
                        tokPicModel.PrimaryFieldText = txtAddPicPrimaryText.Text;
                        tokPicModel.SecondaryFieldText = user.DisplayName;
                        tokPicModel.TokGroup = "Basic";
                        tokPicModel.IsDetailBased = false;
                        tokPicModel.IsMegaTok = false;
                        tokPicModel.Category = "Image";
                        tokPicModel.CategoryId = "category-" + tokPicModel.Category?.ToIdFormat();
                        tokPicModel.TokType = model.Name;
                        tokPicModel.GroupId = model.Id;
                        tokPicModel.UserPhoto = user.UserPhoto;
                        tokPicModel.UserId = user.Id;
                        tokPicModel.UserDisplayName = user.DisplayName;
                        tokPicModel.TokTypeId = $"toktype-{tokPicModel.TokGroup.ToIdFormat()}-{tokPicModel.TokType.ToIdFormat()}";
                        tokPicModel.UserCountry = user.Country;
                        tokPicModel.UserState = user.State;

                        if (tokPicModel.PrimaryFieldText == null || tokPicModel.PrimaryFieldText == string.Empty)
                        {
                            tokPicModel.PrimaryFieldText = " ";
                        }
                        Console.WriteLine("Partition Key:" + tokPicModel.PartitionKey);
                        result = await TokService.Instance.CreateTokAsync(tokPicModel, item: "tokpics");
                    }
                    else
                    {
                        if (picModel.PrimaryFieldText == null || picModel.PrimaryFieldText == string.Empty)
                        {
                            picModel.PrimaryFieldText = " ";
                        }
                        picModel.Image = imageAddPic.ContentDescription;

                        result = await TokService.Instance.UpdateTokAsync(picModel);
                    }


                    this.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                    linearTokPicProgress.Visibility = ViewStates.Gone;

                    if (result.ResultEnum == Shared.Helpers.Result.Success)
                    {
                        TokModel resultTok = new TokModel();

                        if (isSave)
                        {
                            resultTok = result.ResultObject as TokModel;

                            if (string.IsNullOrEmpty(resultTok.Image))
                            {
                                resultTok.Image = imageAddPic.ContentDescription;
                            }
                        }
                        else
                        {
                            resultTok = picModel;
                            try
                            {
                                DialogImageViewerActivity.Instance.closeActivity();
                            }
                            catch (Exception)
                            {
                            }
                        }
                        classgroup_pics_fragmentt.Instance.insertTokAdded(resultTok);

                        addPicDialog.Dismiss();
                    }
                }
            };
        }

        public  void LoadEditChat(TokChatMessage message) {
            FileCollection.Clear();
            files.Clear();
            fileNames.Clear();
            fileExt.Clear();

            editChatMode = true;
            chatTextComment.Text = message.message;
            files = message.files ??= new List<string>();
            fileNames = message.filesname ??= new List<string>();
            fileExt = message.filesextension ??= new List<string>();

            if (files != null && files.Count > 0) {
                for (int i = 0; i < message.files.Count; i++)
                {
                    FileModel fileModel = new FileModel()
                    {
                        FileId = Guid.NewGuid().ToString().Replace("-", ""),
                        Title = " " + message.filesname[i],
                        Extension = message.filesextension[i],
                        FileName = message.filesname[i],
                        FileUrl = message.files[i]
                    };

                    FileCollection.Add(fileModel);
                }
            }
          
            EditedTokChat = message;


        }

        private void ShowAlertLottieDialog(string message)
        {
            var customView = LayoutInflater.Inflate(Resource.Layout.dialog_message_lottie, null);

            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);

            customView.FindViewById<TextView>(Resource.Id.labelNote).Text = message;

            builder.SetView(customView);

            var dialog = builder.Create();
            dialog.Show();

            dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);

            dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            // Access Popup layout fields like below  
            var btnPopupCancel = customView.FindViewById<Button>(Resource.Id.btnCancel);
            var btnPopOk = customView.FindViewById<Button>(Resource.Id.btnOk);
            var lottieAnimationView = customView.FindViewById<LottieAnimationView>(Resource.Id.lottieAnimationView);
            btnPopupCancel.Visibility = ViewStates.Gone;

            lottieAnimationView.SetAnimation("lottie_exclamation.json");
            lottieAnimationView.PlayAnimation();

            // Events for that popup layout  
            btnPopupCancel.Click += delegate
            {
                dialog.Dismiss();
            };

            btnPopOk.Click += delegate
            {
                dialog.Dismiss();
            };
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == REQUEST_ACTIVITY) && (resultCode == Result.Ok) && (data != null))
            {
                NetUri uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }
            else if ((requestCode == REQUEST_ADD_EXISTING_CLASS_TOK) && (resultCode == Result.Ok) && (data != null))
            {
                var classTokModelString = data.GetStringExtra("classtokModel");
            }
        }
        public void displayImageBrowse(Bitmap bitmap, string image)
        {
            imageAddPic.ContentDescription = "data:image/jpeg;base64," + image;
            imageAddPic.SetImageBitmap(bitmap);
        }

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            _mItemTouchHelper.StartDrag(viewHolder);
        }

        public Button btnAddExistingClassTok => this.FindViewById<Button>(Resource.Id.btnAddExistingClassTok);
        public AppBarLayout appBarLayout => this.FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
        public CollapsingToolbarLayout collapsingToolbar => this.FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
        public TextView TextProgressStatus => this.FindViewById<TextView>(Resource.Id.TextProgressStatus);
        public TextView TextName => this.FindViewById<TextView>(Resource.Id.TextName);
        public TextView TextSchool => this.FindViewById<TextView>(Resource.Id.TextSchool);
        public TextView TextDescription => this.FindViewById<TextView>(Resource.Id.TextDescription);
        public TextView TextCheckRequestSent => this.FindViewById<TextView>(Resource.Id.TextCheckRequestSent);
        public TextView TextRequestStatus => this.FindViewById<TextView>(Resource.Id.TextRequestStatus);
        public LinearLayout LinearProgress => this.FindViewById<LinearLayout>(Resource.Id.LinearProgress_ClassGroup);
        public LinearLayout LinearRequestSent => this.FindViewById<LinearLayout>(Resource.Id.LinearRequestSent);

        public LinearLayout GroupInfoLayout => this.FindViewById<LinearLayout>(Resource.Id.GroupInfoLinear);
        public TabLayout tabLayout => this.FindViewById<TabLayout>(Resource.Id.tabLayout);
        public FloatingActionButton FloatingBtn => this.FindViewById<FloatingActionButton>(Resource.Id.fab_AddClassGrouppage);
        public ViewPager viewpager => this.FindViewById<ViewPager>(Resource.Id.viewpagerClassGroup);
        public ImageView ImgGroup => FindViewById<ImageView>(Resource.Id.ImgGroup);
        public TextView txtGroupName => FindViewById<TextView>(Resource.Id.txtGroupName);
        public Button btnShowSchedule => FindViewById<Button>(Resource.Id.btnShowSchedule);
        public Button btnShowNotif => FindViewById<Button>(Resource.Id.btnShowNotif);
        public Button btnShowGroupInfo => FindViewById<Button>(Resource.Id.btnShowGroupInfo);
        public TextView txtScheduleNotif => FindViewById<TextView>(Resource.Id.txtScheduleNotif);
        public ImageButton imageBtnPreviousTab => FindViewById<ImageButton>(Resource.Id.imageBtnPreviousTab);
        public ImageButton imageBtnNextTab => FindViewById<ImageButton>(Resource.Id.imageBtnNextTab);

        //Add Pic Properties
        public ImageView imageAddPic => addPicDialog.FindViewById<ImageView>(Resource.Id.imageViewPic);
        public Button btnAddPicBrowse => addPicDialog.FindViewById<Button>(Resource.Id.btnBrowse);
        public TextView txtAddPicPrimaryText => addPicDialog.FindViewById<TextView>(Resource.Id.txtPrimaryFieldText);
        public Button btnAddPicSave => addPicDialog.FindViewById<Button>(Resource.Id.btnAddPic);
        public LinearLayout linearTokPicProgress => addPicDialog.FindViewById<LinearLayout>(Resource.Id.linearProgress);

        //ChatBox
        public View layoutChatBox => this.FindViewById<View>(Resource.Id.layoutChatBox);
        public CircleImageView chatImgUserPhoto => layoutChatBox.FindViewById<CircleImageView>(Resource.Id.imgcomment_userphoto);
        public TextView chatTextComment => layoutChatBox.FindViewById<TextView>(Resource.Id.tokinfo_txtComment);
        public ImageButton chatButtonSend => layoutChatBox.FindViewById<ImageButton>(Resource.Id.btnSend);
        public ImageButton chatAttach => layoutChatBox.FindViewById<ImageButton>(Resource.Id.btnAttach);

        public RecyclerView FileRecycle => layoutChatBox.FindViewById<RecyclerView>(Resource.Id.fileRecycle);

    }
}