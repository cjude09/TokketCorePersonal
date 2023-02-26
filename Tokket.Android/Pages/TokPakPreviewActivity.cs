using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Tokket.Android.ViewModels;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using TokketHelpers = Tokket.Shared.Helpers;
using Android.Text;
using Tokket.Android.Custom;
using Tokket.Core;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Preview", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Preview", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class TokPakPreviewActivity : BaseActivity
    {
        ClassGroupModel classGroupModel;
        ObservableCollection<ClassTokModel> ClassTokCollection { get; set; }
        TokPak tokPakItem; int tokPakPosition = -1;
        private int REQUEST_ACTIVITY_ADDTOKPAK = 1001;
        ReportDialouge Report = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tok_pak_preview);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ClassTokCollection = new ObservableCollection<ClassTokModel>();
            recyclerPages.SetLayoutManager(new GridLayoutManager(this, 1));

            var groupModelString = Intent.GetStringExtra("classGroupModel");
            if (!string.IsNullOrEmpty(groupModelString))
            {
                classGroupModel = JsonConvert.DeserializeObject<ClassGroupModel>(groupModelString);
            }
            tokPakPosition = Intent.GetIntExtra("tokPakPosition", -1);
            var tokPakString = Intent.GetStringExtra("tokPak");
            var classtokModelString = Intent.GetStringExtra("classtokModel");
            Report = new ReportDialouge(this);
            tokPakItem = JsonConvert.DeserializeObject<TokPak>(tokPakString);

            if (!string.IsNullOrEmpty(classtokModelString))
            {
                var classTokModelList = JsonConvert.DeserializeObject<List<ClassTokModel>>(classtokModelString);
                foreach (var item in classTokModelList)
                {
                    if (item != null)
                    {
                        ClassTokCollection.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in tokPakItem.ClassToks)
                {
                    if (item != null)
                    {
                        ClassTokCollection.Add(item);
                    }
                }
            }

            if (tokPakItem.Type == Shared.Helpers.TokPakType.PracticeTest)
            {
                btnPresentationMode.Text = "Practice Test";
            }


            setRecyclerPagesAdapter();

            btnPresentationMode.Click += delegate
            {
                if (tokPakItem.Type == Shared.Helpers.TokPakType.Paper)
                {
                    Intent nextActivity = new Intent(this, typeof(TokPakPaperModeActivity));
                    var modelConvert = JsonConvert.SerializeObject(ClassTokCollection);
                    nextActivity.PutExtra("isPresentationMode", false);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    nextActivity.PutExtra("TitleActivity", tokPakItem.Type +": "+tokPakItem.Name);
                    this.StartActivity(nextActivity);
                }
                else if(tokPakItem.Type == Shared.Helpers.TokPakType.PracticeTest)
                {
                    Intent nextActivity = new Intent(this, typeof(TokPakPracticeTestActivity));
                    var modelConvert = JsonConvert.SerializeObject(ClassTokCollection);
                    nextActivity.PutExtra("isPresentationMode", false);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    nextActivity.PutExtra("TitleActivity", tokPakItem.Type + ": " + tokPakItem.Name);
                    this.StartActivity(nextActivity);
                }
                else
                {
                    Intent nextActivity = new Intent(this, typeof(TokPakPresentationActivity));
                    var modelConvert = JsonConvert.SerializeObject(ClassTokCollection);
                    nextActivity.PutExtra("classtokModel", modelConvert);
                    nextActivity.PutExtra("TitleActivity", tokPakItem.Type + ": " + tokPakItem.Name);
                    this.StartActivity(nextActivity);
                }
            };
        }

        private void setRecyclerPagesAdapter()
        {
            var adapterDetail = ClassTokCollection.GetRecyclerAdapter(BindClassTok, Resource.Layout.activity_tok_pak_preview_row);
            recyclerPages.SetAdapter(adapterDetail);
        }

        private void BindClassTok(CachingViewHolder holder, ClassTokModel model, int position)
        {
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = holder.FindCachedViewById<TextView>(Resource.Id.txtItem);

            txtHeader.Text = model.PrimaryFieldText;

            if (model.IsDetailBased)
            {
                var cleanDetail = model.Details.ToList().Where(x => !string.IsNullOrEmpty(x)).ToList();
                for (int i = 0; i < cleanDetail.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(model.Details[i]))
                    {
                        txtItem.Text = "• " + model.Details[i];
                    }
                    else
                    {
                        txtItem.Text += "\n• " + model.Details[i];
                    }
                }
            }
            else
            {
                txtItem.Text = model.SecondaryFieldText;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Bundle bundle = new Bundle();
            Intent nextActivity;
            switch (item.ItemId)
            {
                case HomeId:
                    if (tokPakItem.ClassToks != null)
                    {
                        setIntentResult();
                    }
                    Finish();
                    break;
                case Resource.Id.item_TokPakPreview_Edit:
                    if (tokPakItem.ClassToks != null)
                    {
                        if (tokPakItem.UserId == TokketHelpers.Settings.GetUserModel().UserId)
                        {
                            bundle.PutBoolean("isSave", false);
                            bundle.PutString("tokPak", JsonConvert.SerializeObject(tokPakItem));

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
                case Resource.Id.item_TokPakPreview_Delete:
                    if (tokPakItem.ClassToks != null) 
                    {
                        if (tokPakItem.UserId == TokketHelpers.Settings.GetUserModel().UserId)
                        {
                            DeleteTokPak();
                        }
                        else
                        {
                            if (classGroupModel != null)
                            {
                                if (!string.IsNullOrEmpty(tokPakItem.GroupId) && classGroupModel.UserId == TokketHelpers.Settings.GetUserModel().UserId)
                                {
                                    DeleteTokPak();
                                }
                                else
                                {
                                    showAlertDialogs("You are not allowed to perform this action!");
                                }
                            }
                            else
                            {
                                showAlertDialogs("You are not allowed to perform this action!");
                            }
                        }
                            
                    }
                    break;
                case Resource.Id.item_TokPakPreview_Report:
                    Report.Show();
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
                r.ItemId = tokPakItem.Id;
                r.ItemLabel = tokPakItem.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = tokPakItem.UserId;
                r.UserId = TokketHelpers.Settings.GetTokketUser()?.Id;

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

        private async void DeleteTokPak() {
          var status =  await TokPakService.Instance.DeleteTokPakAsync(tokPakItem.Id, tokPakItem.PartitionKey);
            if (status) {
                if (!string.IsNullOrEmpty(tokPakItem.GroupId))
                {
                    ClassGroupTokPakFragment.Instance.LoadTokPaK();
                }

                Bundle bundle = new Bundle();
                bundle.PutBoolean("isDelete", true);
                bundle.PutString("tokPakItem", JsonConvert.SerializeObject(tokPakItem));
                bundle.PutInt("tokPakPosition", tokPakPosition);

                Intent = new Intent();
                Intent.PutExtras(bundle);
                SetResult(Result.Ok, Intent);
                Finish();
            }
        }
        public override void OnBackPressed()
        {
            if (tokPakItem.ClassToks != null)
            {
                setIntentResult();
            }

            base.OnBackPressed();
        }
        private void setIntentResult()
        {
            var tokPakData = JsonConvert.SerializeObject(tokPakItem);

            Intent intentResult = new Intent();
            intentResult.PutExtra("tokPakItem", tokPakData);
            SetResult(Result.Ok, intentResult);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.tok_pak_preview_menu, menu);

            var edit = menu.FindItem(Resource.Id.item_TokPakPreview_Edit);
            var delete = menu.FindItem(Resource.Id.item_TokPakPreview_Delete);
            var report = menu.FindItem(Resource.Id.item_TokPakPreview_Report);
            if (tokPakItem.UserId == TokketHelpers.Settings.GetUserModel().UserId)
            {
                edit.SetVisible(true);
                delete.SetVisible(true);
                report.SetVisible(false);
            }
            else {
                edit.SetVisible(false);
                delete.SetVisible(false);
                report.SetVisible(true);
            }
            return base.OnCreateOptionsMenu(menu);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_ACTIVITY_ADDTOKPAK &&  resultCode == Result.Ok)
            {
                ClassTokCollection.Clear();
                var resultData = data.GetStringExtra("tokpak");
                tokPakItem = JsonConvert.DeserializeObject<TokPak>(resultData);
                foreach (var item in tokPakItem.ClassToks)
                {
                    if (item != null)
                    {
                        ClassTokCollection.Add(item);
                    }
                }

                setRecyclerPagesAdapter();
            }
        }
        public RecyclerView recyclerPages => FindViewById<RecyclerView>(Resource.Id.recyclerPages);
        public Button btnPresentationMode => FindViewById<Button>(Resource.Id.btnPresentationMode);
    }
}