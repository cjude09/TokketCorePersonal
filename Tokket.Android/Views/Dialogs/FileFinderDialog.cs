using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Fragments;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using static Android.App.ActionBar;
using Tokket.Android.Helpers;

namespace Tokket.Android.Custom
{
    public class FileFinderDialog : Dialog
    {
        #region Properties
        public Context ParentContext { get; set; }

        public ClassGroupModel ClassGroupModel { get; set; }

        ObservableCollection<FileModel> FileCollection { get; set; }

        Button closeBTN => FindViewById<Button>(Resource.Id.closeBTN);

        RecyclerView FileRecycler => FindViewById<RecyclerView>(Resource.Id.recyclerDocs);

        Button FileFindBTN => FindViewById<Button>(Resource.Id.findFileBTN);

        Button UploadBTN => FindViewById<Button>(Resource.Id.uploadBTN);

        Button CancelBTN => FindViewById<Button>(Resource.Id.cancelBTN);

        LinearLayout LinearProgress => FindViewById<LinearLayout>(Resource.Id.uploadLinearProgress);
        #endregion
        #region Constructor
        public FileFinderDialog(Context context) : base(context)
        {
            SetContentView(Resource.Layout.file_upload_view);
            FileCollection = new ObservableCollection<FileModel>();
            closeBTN.Click += (obj, _event) => { Dismiss(); };
            CancelBTN.Click += (obj, _event) => { Dismiss(); };
            FileFindBTN.Click += FindFiles_Click;
            UploadBTN.Click += FileUpload_Click;
            showFileUpdaloaded();
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
        }


        public FileFinderDialog(Context context, int themeResId) : base(context, themeResId)
        {
            SetContentView(Resource.Layout.file_upload_view);
            FileCollection = new ObservableCollection<FileModel>();
            closeBTN.Click += (obj, _event) => { Dismiss(); };
            CancelBTN.Click += (obj, _event) => { Dismiss(); };
            FileFindBTN.Click += FindFiles_Click;
            UploadBTN.Click += FileUpload_Click;
            showFileUpdaloaded();
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
        }

        protected FileFinderDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetContentView(Resource.Layout.file_upload_view);
            FileCollection = new ObservableCollection<FileModel>();
            closeBTN.Click += (obj, _event) => { Dismiss(); };
            CancelBTN.Click += (obj, _event) => { Dismiss(); };
            FileFindBTN.Click += FindFiles_Click;
            UploadBTN.Click += FileUpload_Click;
            showFileUpdaloaded();
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
        }

        protected FileFinderDialog(Context context, bool cancelable, EventHandler cancelHandler) : base(context, cancelable, cancelHandler)
        {
            SetContentView(Resource.Layout.file_upload_view);
            FileCollection = new ObservableCollection<FileModel>();
            closeBTN.Click += (obj, _event) => { Dismiss(); };
            CancelBTN.Click += (obj, _event) => { Dismiss(); };
            FileFindBTN.Click += FindFiles_Click;
            UploadBTN.Click += FileUpload_Click;
            showFileUpdaloaded();
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
        }

        protected FileFinderDialog(Context context, bool cancelable, IDialogInterfaceOnCancelListener cancelListener) : base(context, cancelable, cancelListener)
        {
            SetContentView(Resource.Layout.file_upload_view);
            FileCollection = new ObservableCollection<FileModel>();
            closeBTN.Click += (obj, _event) => { Dismiss(); };
            CancelBTN.Click += (obj, _event) => { Dismiss(); };
            FileFindBTN.Click += FindFiles_Click;
            UploadBTN.Click += FileUpload_Click;
            showFileUpdaloaded();
            Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
        }
        #endregion

        private void showFileUpdaloaded()
        {
            FileRecycler.SetLayoutManager(new LinearLayoutManager(ParentContext));
            setFileRecyclerAdapter(FileRecycler);
        }

        private void setFileRecyclerAdapter(RecyclerView recyclerView)
        {
            var adapterDetail = FileCollection.GetRecyclerAdapter(BindFileCollection, Resource.Layout.filedata_row);
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
            fileProgress.SetProgress(0, false);
            MainThread.BeginInvokeOnMainThread(async () => {

                fileUploadResult = await FileUploadService.Instance.UploadFileAsync(model, token.Token);
                if (fileUploadResult != null)
                {
                    model.FileUrl = fileUploadResult.FileLink;
                    fileProgress.SetProgress(100, true);
                    await Task.Delay(500);
                    removeBtn.Text = "Remove";
                    fileProgress.Visibility = ViewStates.Gone;
                    iconComplete.Visibility = ViewStates.Visible;
                }

            });


          
            fileName.Text = model.Title;
            removeBtn.Click += (s, e) => {
                FileCollection.Remove(model);
               
                if (token != null)
                    token.Cancel();
            };
        }

        private async void FindFiles_Click(object sender, EventArgs e)
        {
            try
            {
                string fileExtension = "";
                var isFoundInvalidFile = false;
                Xamarin.Essentials.PickOptions pick = new Xamarin.Essentials.PickOptions();
                var findfiles = await FileUploadService.Instance.SelectFilesWithValidExtension(pick);
                foreach (var file in findfiles)
                {
                    if (file.FullPath.ToLower() != "error")
                    {
                        byte[] bytes = File.ReadAllBytes(file.FullPath);
                        string base64 = await FileService.ConverFileToBase64(file.FullPath);
                        var fileSize = new FileInfo(file.FullPath).Length;

                        var fileNameSplit = file.FileName.Split(".");
                        var ext = string.Empty;
                        if (fileNameSplit.Length > 1)
                            ext = fileNameSplit[fileNameSplit.Length - 1];
                        FileModel fileModel = new FileModel()
                        {
                            FileId = Guid.NewGuid().ToString().Replace("-", ""),
                            Title = " " + file.FileName,
                            FullPath = file.FullPath,
                            Extension = ext,
                            FileName = file.FileName,
                            FileType = file.ContentType,
                            Size = fileSize,
                            Base64 = base64
                        };

                        FileCollection.Add(fileModel);
                    }
                    else
                    {
                        isFoundInvalidFile = true;
                        fileExtension = file.ContentType.Split("/")[1];
                    }
                }

                if (isFoundInvalidFile)
                {
                    ShowAlertLottieDialog($"File type <{fileExtension}> not allowed.");
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error in FindFiles_Click " + ex.ToString());
            }

        }
        private async void FileUpload_Click(object sender, EventArgs e)
        {
            LinearProgress.Visibility = ViewStates.Visible;

           await ClassService.Instance.UploadDocsToGroup(ClassGroupModel,FileCollection.ToList());
            ClassGroupDocsFragment.Instance.Init();
            Dismiss();
            LinearProgress.Visibility = ViewStates.Gone;
        }

        private void ShowAlertLottieDialog(string message)
        {
            var customView = LayoutInflater.Inflate(Resource.Layout.dialog_message_lottie, null);

            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(ParentContext);

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
    }
}