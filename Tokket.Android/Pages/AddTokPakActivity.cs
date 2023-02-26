using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.CallBack;
using Tokket.Android.Fragments;
using Tokket.Android.Interface;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using Result = Android.App.Result;
using AndroidX.RecyclerView.Widget;
using AndroidX.AppCompat.Widget;

namespace Tokket.Android
{
    [Activity(Label = "Create Tok Paks", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddTokPakActivity : BaseActivity, IOnStartDragListener
    {
        internal static AddTokPakActivity Instance { get; private set; }
        int SEARCH_TOKS_REQUEST_CODE = 1001;
        int ADD_CLASSTOK_REQUEST_CODE = 1002;
        Bundle bundle = new Bundle();
        TokPak tokPakItem; bool isSave = true, fromGroupModel = true;
        public ObservableCollection<TokPakDetailViewModel> PagesCollection { get; set; }
        private ItemTouchHelper _mItemTouchHelper;
        private AddTokPakDataAdapter recyclerAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_tok_pak);

            var customToolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.custom_toolbar);
/*#if (_CLASSTOKS)
            defaultToolbarColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.colorAccent));
#endif
#if (_TOKKEPEDIA)
            defaultToolbarColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.primary));
#endif
            customToolbar.SetBackgroundColor(defaultToolbarColor);*/
            SetSupportActionBar(customToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            recyclerPages.SetLayoutManager(new LinearLayoutManager(this));

            Instance = this;

            Initialization();
            loadSpinnerType();

            isSave = Intent.GetBooleanExtra("isSave", true);
            fromGroupModel = Intent.GetBooleanExtra("fromGroupModel", true);
            if (!isSave)
            {
                string intenttokPak = Intent.GetStringExtra("tokPak");
                if (!string.IsNullOrEmpty(intenttokPak))
                {
                    tokPakItem = JsonConvert.DeserializeObject<TokPak>(intenttokPak);
                    txtAddTokPakName.Text = tokPakItem.Name;
                    spinnerType.SetSelection((int)tokPakItem.Type + 1);

                    for (int i = 0; i < tokPakItem.ClassToks.Count; i++)
                    {
                        if (i < PagesCollection.Count)
                        {
                            PagesCollection[i].classTokModel = tokPakItem.ClassToks[i];
                        }
                        else
                        {
                            TokPakDetailViewModel classTokItem = new TokPakDetailViewModel();
                            PagesCollection.Add(classTokItem);
                            PagesCollection[i].classTokModel = tokPakItem.ClassToks[i];
                        }
                    }

                    for (int i = PagesCollection.Count; i < 5; i++)
                    {
                        onClickAddPage();
                    }
                    setRecyclerPagesAdapter();
                }
            }
            else
            {
                tokPakItem.UserId = Settings.GetUserModel().UserId;
                tokPakItem.Type = TokPakType.Presentation; //Set default
            }

            btnAddPage.Click += delegate
            {
                onClickAddPage();
            };

            btnPreview.Click += delegate
            {
                List<ClassTokModel> classTokList = new List<ClassTokModel>();
                foreach (var item in PagesCollection)
                {
                    if (item != null)
                    {
                        if (item.classTokModel.PrimaryFieldText != null)
                        {
                            classTokList.Add(item.classTokModel);
                        }
                    }
                }

                if (classTokList.Count > 0)
                {
                    Intent nextActivity = new Intent(this, typeof(TokPakPreviewActivity));
                    var modelConvert = JsonConvert.SerializeObject(classTokList);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    nextActivity.PutExtra("tokPak", JsonConvert.SerializeObject(tokPakItem));
                    this.StartActivity(nextActivity);
                }
            };

            btnSave.Click += async(s, e) =>
            {
                if (spinnerType.FirstVisiblePosition == 0)
                {
                    showalertDialog("Select a type.");
                    return;
                }
                else if (string.IsNullOrEmpty(txtAddTokPakName.Text))
                {
                    showalertDialog("Please enter a Name for identification.");
                    return;
                }
                else if (string.IsNullOrEmpty(txtCategory.Text))
                {
                    showalertDialog("Please enter a Category.");
                    return;
                }

                showProgress();
                tokPakItem.ClassToks = new List<ClassTokModel>();

                foreach (var item in PagesCollection)
                {
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.classTokModel.PrimaryFieldText))
                        {
                            tokPakItem.ClassToks.Add(item.classTokModel);
                        }
                    }
                }

                TokPak resultTokPak = new TokPak();
                if (tokPakItem.ClassToks.Count > 0)
                {
                    tokPakItem.Name = txtAddTokPakName.Text;
                    tokPakItem.Category = txtCategory.Text;

                    if (isSave)
                    {
                        var groupId = Intent.GetStringExtra("GroupId");
                        ClassGroupModel model = new ClassGroupModel();
                        if (!string.IsNullOrEmpty(groupId))
                        {
                            model = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("GroupId"));
                            tokPakItem.GroupId = model.Id;
                        }
                        txtProgress.Text = "Saving...";
                        resultTokPak = await TokPakService.Instance.AddTokPakAsync(tokPakItem);
                    }
                    else
                    {
                        txtProgress.Text = "Updating...";
                        resultTokPak = await TokPakService.Instance.UpdateTokPakAsync(tokPakItem);
                    }
                }

                if (resultTokPak != null)
                {
                    tokPakItem = resultTokPak;
                }

                hideProgress();

                Intent intent = new Intent();
                intent.PutExtra("tokpak", JsonConvert.SerializeObject(tokPakItem));
                SetResult(Result.Ok, intent);

                if (!string.IsNullOrEmpty(tokPakItem.GroupId)) {
                    ClassGroupTokPakFragment.Instance.LoadTokPaK();
                }

                if (isSave && string.IsNullOrEmpty(tokPakItem.GroupId))
                {
                    //Once saving is done, go directly to viewtokpackactivity
                    var nextActivity = new Intent(this, typeof(ViewTokPakActivity));
                    this.StartActivity(nextActivity);
                }

                this.Finish();
            };
        }
        private void Initialization()
        {
            isSave = true;
            tokPakItem = new TokPak();
            bundle = new Bundle();
            PagesCollection = new ObservableCollection<TokPakDetailViewModel>();
            TokPakDetailViewModel pageItem = new TokPakDetailViewModel();
            PagesCollection.Add(pageItem);

            pageItem = new TokPakDetailViewModel();
            PagesCollection.Add(pageItem);

            pageItem = new TokPakDetailViewModel();
            PagesCollection.Add(pageItem);

            pageItem = new TokPakDetailViewModel();
            PagesCollection.Add(pageItem);

            pageItem = new TokPakDetailViewModel();
            PagesCollection.Add(pageItem);

            setRecyclerPagesAdapter();
        }
        public void setRecyclerPagesAdapter()
        {
            recyclerAdapter = new AddTokPakDataAdapter(this, PagesCollection, this);
            recyclerPages.SetAdapter(recyclerAdapter);
            
            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(recyclerAdapter);
            _mItemTouchHelper = new ItemTouchHelper(callback);
            _mItemTouchHelper.AttachToRecyclerView(recyclerPages);

            /*var adapterDetail = PagesCollection.GetRecyclerAdapter(BindPagesDetail, Resource.Layout.add_tok_pak_row);
            recyclerPages.SetAdapter(adapterDetail);*/
        }
       /* private void BindPagesDetail(CachingViewHolder holder, TokPakDetailModel model, int position)
        {
            var recyclerTok = holder.FindCachedViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.recyclerTok);
            var txtPrimaryNumber = holder.FindCachedViewById<TextView>(Resource.Id.txtPrimaryNumber);
            var txtPagePrimary = holder.FindCachedViewById<TextView>(Resource.Id.txtPagePrimary);
            var btnPasteTok = holder.FindCachedViewById<Button>(Resource.Id.btnPasteTok);
            var btnFindTok = holder.FindCachedViewById<Button>(Resource.Id.btnFindTok);
            var btnRemovePaste = holder.FindCachedViewById<Button>(Resource.Id.btnRemovePaste);
            var btnAddClassTok = holder.FindCachedViewById<Button>(Resource.Id.btnAddClassTok);
            txtPrimaryNumber.Text = "Page " + (position + 1).ToString();

            btnRemovePaste.Tag = position;
            btnFindTok.Tag = position;
            btnPasteTok.Tag = position;

            if (position > 4)
            {
                btnRemovePaste.Visibility = ViewStates.Visible;
            }

            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.classTokModel.PrimaryFieldText))
                {
                    txtPagePrimary.Text = model.classTokModel.PrimaryFieldText;
                    btnRemovePaste.Visibility = ViewStates.Visible;
                }
            }

            btnRemovePaste.Click -= btnRemove_Click;
            btnRemovePaste.Click += btnRemove_Click;

            btnFindTok.Click -= btnFindTok_Click;
            btnFindTok.Click += btnFindTok_Click;

            btnPasteTok.Click -= btnPasteTok_Click;
            btnPasteTok.Click += btnPasteTok_Click;

            btnAddClassTok.Click -= btnAddClassTok_Click;
            btnAddClassTok.Click += btnAddClassTok_Click;

            //Load the copied or paste tok
            recyclerTok.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(this));

            var tokAdapter = new ClassTokDataAdapter(model.tokItemList, new List<Tokmoji>());
            recyclerTok.SetAdapter(tokAdapter);

            if (model.tokItemList.Count == 0)
            {
                recyclerTok.Visibility = ViewStates.Gone;
            }
            else
            {
                recyclerTok.Visibility = ViewStates.Visible;
            }
        }*/

        public void btnAddClassTok_Click(object sender, EventArgs e)
        {
            bundle.PutInt("position", (int)(sender as Button).Tag);
            Intent nextActivity = new Intent(this, typeof(AddClassTokActivity));
            this.StartActivityForResult(nextActivity, ADD_CLASSTOK_REQUEST_CODE);
        }

        public void btnRemove_Click(object sender, EventArgs e)
        {
            int position = (int)(sender as Button).Tag;
            if (PagesCollection.Count > 5)
            {
                PagesCollection.RemoveAt(position);
            }
            else
            {
                PagesCollection[position].classTokModel = new ClassTokModel();
               // PagesCollection[position].tokItemList = new List<ClassTokModel>();
            }
            setRecyclerPagesAdapter();
        }

        public void UpdateItemPosition() {
            int idx = 0;
            var views = recyclerPages.GetLayoutManager().ItemCount;
            foreach (var page in PagesCollection) {
                if (idx < views-1) {
                    View v = recyclerPages.GetLayoutManager().GetChildAt(idx);

                    v.Tag = idx;
                    idx++;
                }
              
            }
        }

        public RecyclerView.ViewHolder getRecyclerViewHolder(int position)
        {
            return recyclerPages.FindViewHolderForAdapterPosition(position);
        }
        public void btnFindTok_Click(object sender, EventArgs e)
        {
            bundle.PutInt("position", (int)(sender as Button).Tag);
            Intent nextActivity = new Intent(this, typeof(SearchToksDialog));
            nextActivity.PutExtra("fromGroupModel", fromGroupModel);
            this.StartActivityForResult(nextActivity, SEARCH_TOKS_REQUEST_CODE);
        }

        private void onClickAddPage()
        {
            if (PagesCollection.Count < 26) {
                TokPakDetailViewModel classTokItem = new TokPakDetailViewModel();
                PagesCollection.Add(classTokItem);
                setRecyclerPagesAdapter();
                if (PagesCollection.Count == 25) {
                    btnAddPage.Visibility = ViewStates.Gone;
                    LimitReminder.Visibility = ViewStates.Visible;
                }
            }
        
        }

        public ObservableCollection<TokPakDetailViewModel> InsertPage(int insertPos) {
            TokPakDetailViewModel pageItem = new TokPakDetailViewModel();
            PagesCollection.Insert(insertPos,pageItem);
            return PagesCollection;
        }
        public void btnPasteTok_Click(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); };
            var classTokString = Clipboard.GetTextAsync().GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(classTokString))
            {
                try
                {
                    var classTokPaste = JsonConvert.DeserializeObject<ClassTokModel>(classTokString);
                    if (classTokPaste == null)
                    {
                        showalertDialog("Pasted Tok is invalid!");
                    }
                    else
                    {
                        PagesCollection[position].classTokModel = classTokPaste;

                        var list = new List<ClassTokModel>();
                        list.Add(classTokPaste);

                       // PagesCollection[position].tokItemList = list;

                        Clipboard.SetTextAsync("").GetAwaiter().GetResult();
                        setRecyclerPagesAdapter();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    showalertDialog("Pasted Tok is invalid!");
                }
            }
            else
            {
                showalertDialog("Pasted Tok is invalid!");
            }
        }
        private void showalertDialog(string message)
        {
            var alertDiag = new AlertDialog.Builder(this);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage(message);
            alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == SEARCH_TOKS_REQUEST_CODE) && (resultCode == Result.Ok) && (data != null))
            {
                var position = bundle.GetInt("position", -1);

                var classtokData = data.GetStringExtra("classtokModel");
                if (classtokData != null)
                {
                    var classTokPaste = JsonConvert.DeserializeObject<ClassTokModel>(classtokData);

                    if (position >= 0)
                    {
                        PagesCollection[position].classTokModel = classTokPaste;

                        var list = new List<ClassTokModel>();
                        list.Add(classTokPaste);

                        //PagesCollection[position].tokItemList = list;

                        setRecyclerPagesAdapter();
                    }
                }
                
            } else if ((requestCode == ADD_CLASSTOK_REQUEST_CODE) && (resultCode == Result.Ok) && (data != null)) {
                var position = bundle.GetInt("position", -1);

                var classtokData = data.GetStringExtra("tokModel");
                var classTokPaste = JsonConvert.DeserializeObject<ClassTokModel>(classtokData);

                if (position >= 0)
                {
                    PagesCollection[position].classTokModel = classTokPaste;

                    var list = new List<ClassTokModel>();
                    list.Add(classTokPaste);

                  //  PagesCollection[position].tokItemList = list;

                    setRecyclerPagesAdapter();
                }
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

        public void loadSpinnerType()
        {
            spinnerType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerType_ItemSelected);
            List<string> spinnerTypeList = new List<string>();

            spinnerTypeList.Add("Choose...");
            spinnerTypeList.Add("Paper");
            spinnerTypeList.Add("Presentation");
            spinnerTypeList.Add("Practice Test");
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, spinnerTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerType.Adapter = Aadapter;
        }
        private void spinnerType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var typeSelected = spinnerType.GetItemAtPosition(e.Position).ToString();
            switch (typeSelected.ToLower())
            {
                case "paper":
                    tokPakItem.Type = TokPakType.Paper;
                    break;
                case "presentation":
                    tokPakItem.Type = TokPakType.Presentation;
                    break;
                case "practice test":
                    tokPakItem.Type = TokPakType.PracticeTest;
                    ShowLottieMessageDialog(this, messageContent: "Minimum of 4 toks required. Do you want to continue?", handlerOKText: "Continue", handlerCancelClick: (s, e) => {
                        this.Finish();
                    }, _animation: "lottie_exclamation.json");
                    break;
                default:
                    tokPakItem.Type = TokPakType.Paper;
                    break;
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

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            _mItemTouchHelper.StartDrag(viewHolder);
        }

        public Spinner spinnerType => FindViewById<Spinner>(Resource.Id.spinnerType);
        public RecyclerView recyclerPages => FindViewById<RecyclerView>(Resource.Id.recyclerPages);
        public Button btnAddPage => FindViewById<Button>(Resource.Id.btnAddPage);
        public Button btnSave => FindViewById<Button>(Resource.Id.btnSave);
        public Button btnPreview => FindViewById<Button>(Resource.Id.btnPreview);
        public LinearLayout linearProgress => FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public ProgressBar progressBarCircle => FindViewById<ProgressBar>(Resource.Id.progressBarCircle);
        public TextView txtProgress => FindViewById<TextView>(Resource.Id.txtProgress);
        public AppCompatEditText txtAddTokPakName => FindViewById<AppCompatEditText>(Resource.Id.txtAddTokPakName);

        public RelativeLayout LimitReminder => FindViewById<RelativeLayout>(Resource.Id.limitStar);
        public AppCompatEditText txtCategory => FindViewById<AppCompatEditText>(Resource.Id.txtCategory);
    }
}