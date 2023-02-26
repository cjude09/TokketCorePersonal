using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Android.Content.PM;
using Android.Graphics;
using Android.Text;
using Tokket.Android.Fragments;
using Tokket.Android.Custom;
using Tokket.Core;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using AndroidX.AppCompat.Widget;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;
using Result = Android.App.Result;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    /*#if (_TOKKEPEDIA)
        [Activity(Label = "View Tok Pak", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif

    #if (_CLASSTOKS)
        [Activity(Label = "View Tok Pak", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    #endif*/
    [Activity(Label = "View Tok Pak", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewTokPakActivity : BaseActivity
    {
        Bundle bundle = new Bundle();
        private int REQUEST_ACTIVITY_PREVIEW = 1001;
        private int REQUEST_ACTIVITY_ADDTOKPAK = 1002;
        ObservableCollection<TokPak> TokPakCollection { get; set; }
        List<TokPak> TokPakListChecked = new List<TokPak>();
        TokPak SelectedTokPak;
        int SelectedIndex;
        ReportDialouge Report = null;
        bool isPublicData = false;
        bool isShowCheckBox = false;
        string cachecaller = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_view_tok_pak);
            SetSupportActionBar(toolBar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            
            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            bundle = new Bundle();
            TokPakCollection = new ObservableCollection<TokPak>();
            TokPakListChecked = new List<TokPak>();
            recyclerPages.SetLayoutManager(new GridLayoutManager(this, numcol));
            recyclerPages.ScrollChange += Recycler_ScrollChange;

            isPublicData = Intent.GetBooleanExtra("isPublic", false);

            if (isPublicData)
            {
                setActivityTitle("Public Tok Paks");
            }
            else
            {
                setActivityTitle("My Tok Paks");
                btnRemoveTokPak.Visibility = ViewStates.Visible;
            }

            RunOnUiThread(async () => await Initialize(isPublicFeed: isPublicData));
            Report = new ReportDialouge(this);
            btnMyToks.Click += async (s, e) =>
            {
                TokPakCollection.Clear();
                await Initialize(isPublicFeed: isPublicData);
            };

            btnPresentation.Click += async (s, e) =>
            {
                TokPakCollection.Clear();
                await Initialize(isPublicFeed: isPublicData, tokPakType: ((int)TokPakType.Presentation).ToString());
            };

            btnPapers.Click += async (s, e) =>
            {
                TokPakCollection.Clear();
                await Initialize(isPublicFeed: isPublicData, tokPakType: ((int)TokPakType.Paper).ToString());
            };

            btnPracticeTest.Click += async (s, e) =>
            {
                TokPakCollection.Clear();
                await Initialize(isPublicFeed: isPublicData, tokPakType: ((int)TokPakType.PracticeTest).ToString());
            };

            btnRemoveTokPak.Click += delegate
            {
                btnRemoveTokPak.Visibility = ViewStates.Gone;
                btnDeleteTokPak.Visibility = ViewStates.Visible;
                btnCancelDelete.Visibility = ViewStates.Visible;
                isShowCheckBox = true;
                setRecyclerPagesAdapter();
            };
            
            btnCancelDelete.Click += delegate
            {
                CancelDelete();
            };

            btnDeleteTokPak.Click += async (o, s) =>
            {
                await DeleteTokPakStart();
            };
        }
        private void CancelDelete()
        {
            btnRemoveTokPak.Visibility = ViewStates.Visible;
            btnDeleteTokPak.Visibility = ViewStates.Gone;
            btnCancelDelete.Visibility = ViewStates.Gone;
            isShowCheckBox = false;
            TokPakListChecked.Clear();
            setRecyclerPagesAdapter();
        }
        private async Task DeleteTokPakStart()
        {
            var listTokPakToDelete = new List<TokPak>();

            if (TokPakListChecked.Count() > 0)
            {
                showProgress();
            }

            var itemList = TokPakListChecked.ToArray();
            for (int i = 0; i < itemList.Length; i++)
            {
                var item = itemList[i];
                if (item.isCheck)
                {
                    txtProgress.Text = $"Deleting {i + 1} of {itemList.Length}...";
                    bool? result = null;
                    result = await TokPakService.Instance.DeleteTokPakAsync(item.Id, item.PartitionKey);

                    if (result != null)
                    {
                        listTokPakToDelete.Add(item);
                        TokPakCollection.Remove(item);

                        if (i == itemList.Length - 1)
                        {
                            setRecyclerPagesAdapter();
                            CancelDelete();
                            TokPakService.Instance.SetCacheTokPaksAsync(cachecaller, TokPakCollection.ToList());
                            await Task.Delay(1000);
                            hideProgress();
                        };
                    }
                }
            }
        }

        private void Recycler_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            var layout = (sender as RecyclerView).GetLayoutManager();
            var manager = layout as GridLayoutManager;
            int totalItem = manager.ItemCount;
            int lastVisible = manager.FindLastVisibleItemPosition();
            bool endHasBeenReached = lastVisible + 1 >= totalItem;
            if (lastVisible > 0 && endHasBeenReached)
            {
                LimitReminder.Visibility = ViewStates.Visible;
            }
            else
            {
                LimitReminder.Visibility = ViewStates.Gone;
            }
        }
        private void PreloadUIToks(string paginationToken = "", bool isPublicFeed = false, string tokPakType = "")
        {
            TokPakListChecked.Clear();
            showProgress();
            var cachedClassToks = TokPakService.Instance.GetCacheTokPaksAsync(cachecaller);
            if (cachedClassToks.Results != null)
            {
                var cacheList = cachedClassToks.Results.ToList();

                TokPakListChecked.AddRange(cacheList);

                hideProgress();
                setRecyclerPagesAdapter();
            }

            if (TokPakListChecked.Count() == 0)
            {
                RunOnUiThread(async () => await Initialize(paginationToken, isPublicFeed, tokPakType));
            }
        }

        private async Task Initialize(string paginationToken = "", bool isPublicFeed = false, string tokPakType = "")
        {
            if (isPublicFeed)
            {
                cachecaller = "tok_pak_list_public_" + tokPakType;
            }
            else
            {
                cachecaller = "tok_pak_list_my_" + tokPakType;
            }
            TokPakListChecked.Clear();
            showProgress();

            var intentGroupId = Intent.GetStringExtra("GroupId");
            string groupId = "";
            if (!string.IsNullOrEmpty(intentGroupId))
            {
                groupId = string.IsNullOrEmpty(JsonConvert.DeserializeObject<string>(intentGroupId)) ? string.Empty : JsonConvert.DeserializeObject<string>(intentGroupId);
            }

            var result = await TokPakService.Instance.GetTokPaksAsync(new TokPakQueryValues()
            {
                paginationid = paginationToken,
                userid = isPublicFeed ? "" : Settings.GetUserModel().UserId,
                groupid = groupId,
                tokpaktype = tokPakType,
                publicfeed = isPublicFeed
            }, cachecaller) ;
            if (result != null) {
                foreach (var item in result.Results.ToList())
                {
                    TokPakCollection.Add(item);
                }
            }          

            hideProgress();

            setRecyclerPagesAdapter();
        }

        private void setRecyclerPagesAdapter()
        {
            var adapterDetail = TokPakCollection.GetRecyclerAdapter(BindClassTok, Resource.Layout.activity_view_tok_pak_row);
            recyclerPages.SetAdapter(adapterDetail);
        }

        private void BindClassTok(CachingViewHolder holder, TokPak model, int position)
        {
            var chkBox = holder.FindCachedViewById<AppCompatCheckBox>(Resource.Id.chkBox);
            var imageUserPhoto = holder.FindCachedViewById<ImageView>(Resource.Id.imageUserPhoto);
            var txtNoPages = holder.FindCachedViewById<TextView>(Resource.Id.txtNoPages);
            var txtDate = holder.FindCachedViewById<TextView>(Resource.Id.txtDate);
            var txtUserDisplayName = holder.FindCachedViewById<TextView>(Resource.Id.txtUserDisplayName);
            var txtUserTitle = holder.FindCachedViewById<TextView>(Resource.Id.txtUserTitle);
            var viewHeaderBackground = holder.FindCachedViewById<View>(Resource.Id.viewHeaderBackground);
            var constraintParent = holder.FindCachedViewById<ConstraintLayout>(Resource.Id.constraintParent);
            var txtViewMore = holder.FindCachedViewById<TextView>(Resource.Id.txtViewMore);
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = holder.FindCachedViewById<TextView>(Resource.Id.txtItem);
            var btnPresentation = holder.FindCachedViewById<Button>(Resource.Id.btnPageType);
            var txtCategory = holder.FindCachedViewById<TextView>(Resource.Id.txtCategory);
            var btnManage = holder.FindCachedViewById<Button>(Resource.Id.btnManage);

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

            txtCategory.Text = model.Category;
            holder.ItemView.Tag = position;
            txtHeader.SetTextColor(Color.White);
            txtUserDisplayName.Text = model.UserDisplayName;
            txtUserTitle.Text = model.TitleDisplay;
            txtNoPages.Text = "# of pages: " + model.ClassToks.Count().ToString();
            txtDate.Text = model.CreatedTime.ToString("MM/dd/yyyy");

            var userPhoto = isPublicData ? model.UserPhoto : Settings.GetTokketUser().UserPhoto;

            Glide.With(holder.ItemView).Load(userPhoto).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image)).Into(imageUserPhoto);

            switch (model.Type)
            {
                case TokPakType.Paper:
                    txtHeader.Text = "Paper";
                    btnPresentation.Text = "Paper";
                    constraintParent.SetBackgroundResource(Resource.Drawable.rounded_blue_stroke_bg);
                    viewHeaderBackground.SetBackgroundColor(Color.ParseColor("#4472c4"));
                 
                    break;
                case TokPakType.Presentation:
                    txtHeader.Text = "Presentation";
                    btnPresentation.Text = "Presentation";
                    constraintParent.SetBackgroundResource(Resource.Drawable.rounded_pink_stroke_bg);
                    viewHeaderBackground.SetBackgroundColor(Color.ParseColor("#f711ec"));
                    break;
                case TokPakType.PracticeTest:
                    txtHeader.Text = "Practice Test";
                    btnPresentation.Text = "Practice Test";
                    constraintParent.SetBackgroundResource(Resource.Drawable.rounded_yellow_stroke_bg);
                    viewHeaderBackground.SetBackgroundColor(Color.ParseColor("#ffc000"));

                    break;
                default:
                    break;
            }


            txtViewMore.Visibility = ViewStates.Gone;
            for (int i = 0; i < model.ClassToks.Count; i++)
            {
                if (i < 9)
                {
                    if (i == 0)
                    {
                        txtItem.Text = "• " + model.ClassToks[i].PrimaryFieldText.ToString();
                    }
                    else
                    {
                        txtItem.Text += "\n• " + model.ClassToks[i].PrimaryFieldText.ToString();
                    }
                }
                else
                {
                    txtViewMore.Visibility = ViewStates.Visible;
                    break;
                }
            }

            btnPresentation.Tag = position;
            txtViewMore.Tag = position;

            txtViewMore.Click -= onClickPreview;
            btnPresentation.Click -= onClickPreview;
            if (model.Type != TokPakType.PracticeTest)
            {
                btnPresentation.Click += onClickPreview;
                txtViewMore.Click += onClickPreview;
            } else
            {
                btnPresentation.Click += onClickPresentationMode;
                txtViewMore.Click += onClickPreview;
            }
            
            btnManage.Click += (obj, _event) => {
                var menu = new PopupMenu(this, btnManage);

                // Call inflate directly on the menu:
                menu.Inflate(Resource.Menu.chatrow_menu);
                var edit = menu.Menu.FindItem(Resource.Id.chatrowEdit);
                var delete = menu.Menu.FindItem(Resource.Id.chatrowDelete);
                var report = menu.Menu.FindItem(Resource.Id.chatrowReport);

                if (model.UserId == Settings.GetUserModel().UserId)
                {
                    edit.SetVisible(true);
                    delete.SetVisible(true);
                    report.SetVisible(false);
                }
                else
                {

                    edit.SetVisible(false);
                    delete.SetVisible(false);
                    report.SetVisible(true);
                }
                Bundle bundle = new Bundle();
                Intent nextActivity;
                menu.MenuItemClick += async (obj, _event) => {
                    switch (_event.Item.ItemId)
                    {
                        case Resource.Id.chatrowEdit:
                            if (model.ClassToks != null)
                            {
                                if (model.UserId == Settings.GetUserModel().UserId)
                                {
                                    SelectedIndex = position;
                                    bundle.PutBoolean("isSave", false);
                                    bundle.PutString("tokPak", JsonConvert.SerializeObject(model));

                                    nextActivity = new Intent(this, typeof(AddTokPakActivity));
                                    nextActivity.PutExtras(bundle);
                                    this.StartActivityForResult(nextActivity, REQUEST_ACTIVITY_ADDTOKPAK);
                                }
                                else
                                {
                                    showAlertDialogs("You are not allowed to perform this action!");
                                }
                            }

                            break;
                        case Resource.Id.chatrowDelete:
                            if (model.ClassToks != null)
                            {
                                if (model.UserId == Settings.GetUserModel().UserId)
                                {
                                    DeleteTokPak(model, position);
                                }
                                else
                                {
                                    showAlertDialogs("You are not allowed to perform this action!");
                                }
                            }
                            break;
                        case Resource.Id.chatrowReport:
                            SelectedTokPak = model;
                            Report.Show();
                            break;
                    }
                };

                menu.DismissEvent += (s1, _event) => {

                };

                menu.Show();
            };
        }

        private void chkChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var checkBox = sender as AppCompatCheckBox;
            int position = 0;
            try { position = (int)checkBox.Tag; } catch { position = int.Parse((string)checkBox.Tag); }

            TokPakCollection[position].isCheck = checkBox.Checked;

            var result = TokPakListChecked.FirstOrDefault(c => c.Id == TokPakCollection[position].Id);
            if (result != null) //If found
            {
                int ndx = TokPakListChecked.IndexOf(result);
                TokPakListChecked.Remove(result);
            }
            else //if save
            {
                if (checkBox.Checked) //Add checker to be sure
                {
                    TokPakListChecked.Add(TokPakCollection[position]);
                }
            }
        }
        private void showProgress()
        {
            linearProgress.Visibility = ViewStates.Visible;
            Window.AddFlags(WindowManagerFlags.NotTouchable);
        }
        private void hideProgress()
        {
            linearProgress.Visibility = ViewStates.Gone;
            Window.ClearFlags(WindowManagerFlags.NotTouchable);
        }

        private void onClickPreview(object sender, EventArgs e)
        {
            int position = (int)(sender as View).Tag;
            bundle.PutInt("position", position);
            Intent nextActivity = new Intent(this, typeof(TokPakPreviewActivity));
            var modelConvert = JsonConvert.SerializeObject(TokPakCollection[position]);
            nextActivity.PutExtra("tokPak", modelConvert);
            nextActivity.PutExtra("tokPakPosition", position);
            this.StartActivityForResult(nextActivity, REQUEST_ACTIVITY_PREVIEW);
        }

        private void onClickPresentationMode(object sender, EventArgs e)
        {
            int position = (int)(sender as View).Tag;

            Intent nextActivity = new Intent(this, typeof(TokPakPracticeTestActivity));
            var modelConvert = JsonConvert.SerializeObject(TokPakCollection[position].ClassToks);
            nextActivity.PutExtra("isPresentationMode", false);
            nextActivity.PutExtra("classtokModel", modelConvert);
            nextActivity.PutExtra("TitleActivity", TokPakCollection[position].Type + ": " + TokPakCollection[position].Name);
            this.StartActivity(nextActivity);
        }

        private void showAlertDialogs(string message)
        {

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

        private async void DeleteTokPak(TokPak model,int tokPakPosition)
        {
            var status = await TokPakService.Instance.DeleteTokPakAsync(model.Id, model.PartitionKey);
            if (status)
            {
                TokPakCollection.Remove(model);
                setRecyclerPagesAdapter();
            }
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
                r.ItemId = SelectedTokPak.Id;
                r.ItemLabel = SelectedTokPak.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = SelectedTokPak.UserId;
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

            if (requestCode == REQUEST_ACTIVITY_PREVIEW && resultCode == Result.Ok)
            {
                var isDelete = data.GetBooleanExtra("isDelete", false);
                var tokPakData = data.GetStringExtra("tokPakItem");

                if (isDelete)
                {
                    int tokPakPosition = data.GetIntExtra("tokPakPosition", -1);
                    if (tokPakPosition > -1)
                    {
                        TokPakCollection.RemoveAt(tokPakPosition);
                        setRecyclerPagesAdapter();
                    }
                }
                else
                {
                    if (tokPakData != null)
                    {
                        int position = bundle.GetInt("position", -1);
                        var tokPakResult = JsonConvert.DeserializeObject<TokPak>(tokPakData);
                        if (tokPakResult != null)
                        {
                            if (position == -1)
                            {
                                TokPakCollection.Add(tokPakResult);
                            }
                            else
                            {
                                TokPakCollection.RemoveAt(position);

                                TokPakCollection.Insert(position, tokPakResult);
                            }
                            setRecyclerPagesAdapter();
                        }
                    }
                }
            }
            else if (requestCode == REQUEST_ACTIVITY_ADDTOKPAK && resultCode == Result.Ok)
            {
               
                var resultData = data.GetStringExtra("tokpak");
               var  tokPakItem = JsonConvert.DeserializeObject<TokPak>(resultData);
                //   TokPakCollection[SelectedIndex] = tokPakItem;
                TokPakCollection.RemoveAt(SelectedIndex);
                TokPakCollection.Insert(0,tokPakItem);
                setRecyclerPagesAdapter();
            }

        
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public Button btnMyToks => FindViewById<Button>(Resource.Id.btnMyToks);
        public Button btnPresentation => FindViewById<Button>(Resource.Id.btnPresentation);
        public Button btnPapers => FindViewById<Button>(Resource.Id.btnPapers);
        public Button btnPracticeTest => FindViewById<Button>(Resource.Id.btnPracticeTest);
        public RecyclerView recyclerPages => FindViewById<RecyclerView>(Resource.Id.recyclerPages);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public ProgressBar progressBarCircle => FindViewById<ProgressBar>(Resource.Id.progressBarCircle);
        public TextView txtProgress => FindViewById<TextView>(Resource.Id.txtProgress);
        public LinearLayout LimitReminder => FindViewById<LinearLayout>(Resource.Id.limitStar);

        public TextView btnRemoveTokPak => FindViewById<TextView>(Resource.Id.btnRemoveTokPak);
        public TextView btnDeleteTokPak => FindViewById<TextView>(Resource.Id.btnDeleteTokPak);
        public TextView btnCancelDelete => FindViewById<TextView>(Resource.Id.btnCancelDelete);
    }
}