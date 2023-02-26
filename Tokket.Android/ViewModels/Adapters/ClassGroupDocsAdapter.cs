using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Extensions;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Helpers;
using Android.Text;
using Tokket.Shared.Services;
using Tokket.Android.Fragments;
using Tokket.Android.Custom;
using System.IO;
using System.Threading;
using NetUri = Android.Net.Uri;

namespace Tokket.Android.Adapters
{
    public class ClassGroupDocsAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        Context context;
        ClassGroupModel classGroupModel;
        int totalItems = 0;
        public event EventHandler<int> ItemClick;
        public List<TokModel> items;
       
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => totalItems;
        View itemView;
        #region Constructor
        public ClassGroupDocsAdapter(Context cntxt, List<TokModel> _items, ClassGroupModel _classGroupModel)
        {
            context = cntxt;
            items = _items;
            classGroupModel = _classGroupModel;
            totalItems = items.Count();
        }
        #endregion

        #region Override Events/Methods/Delegates
        ClassGroupDocsViewHolder vh;
        int selectedPosition = -1;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as ClassGroupDocsViewHolder;

            //Set LinearColor
            string colorHex = "";
            int ndx = position % Colors.Count;

            vh.textDocTitle.Text = items[position].PrimaryFieldText;
            vh.lblDocMenu.Tag = position;
            vh.lblDocMenu.Click -= itemMenuClick;
            vh.lblDocMenu.Click += itemMenuClick;

            if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();

            if (string.IsNullOrEmpty(colorHex))
            {
                vh.linearDocsColor.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));
            }
            else
            {
                vh.linearDocsColor.SetBackgroundColor(Color.ParseColor(colorHex));
            }
        }

        private void itemMenuClick(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as View).Tag; } catch { position = int.Parse((string)(sender as View).Tag); }
            PopupMenu menu = new PopupMenu(context, (sender as View));

            // Call inflate directly on the menu:
            menu.Inflate(Resource.Menu.classgroup_docs_menu);
            var edit = menu.Menu.FindItem(Resource.Id.itemEdit);
            var delete = menu.Menu.FindItem(Resource.Id.itemRemoveDoc);
            var report = menu.Menu.FindItem(Resource.Id.itemReportDoc);

            if (classGroupModel.UserId == Settings.GetTokketUser().Id)
            {
                edit.SetVisible(true);
                delete.SetVisible(true);
                report.SetVisible(false);
            } else if (items[position].UserId == Settings.GetTokketUser().Id) {
                delete.SetVisible(true);
                edit.SetVisible(true);
            }
            else {
                edit.SetVisible(false);
                delete.SetVisible(false);
                report.SetVisible(true);
            }
            // A menu item was clicked:
            menu.MenuItemClick += async (s1, arg1) => {
                switch (arg1.Item.TitleFormatted.ToString().ToLower())
                {
                    case "edit":
                        Xamarin.Essentials.PickOptions pick = new Xamarin.Essentials.PickOptions();
                        var findfiles = await FileUploadService.Instance.SelectFile(pick);

                        byte[] bytes = File.ReadAllBytes(findfiles.FullPath);
                        string base64 = await FileService.ConverFileToBase64(findfiles.FullPath);
                        var fileSize = new FileInfo(findfiles.FullPath).Length;

                        var fileNameSplit = findfiles.FileName.Split(".");
                        var ext = string.Empty;
                        if (fileNameSplit.Length > 1)
                            ext = fileNameSplit[fileNameSplit.Length - 1];

                        FileModel fileModel = new FileModel()
                        {
                            FileId = Guid.NewGuid().ToString().Replace("-", ""),
                            Title = " " + findfiles.FileName,
                            FullPath = findfiles.FullPath,
                            Extension = ext,
                            FileName = findfiles.FileName,
                            FileType = findfiles.ContentType,
                            Size = fileSize,
                            Base64 = base64
                        };


                        showAlertDialogs($"Do you want to replace the file to {findfiles.FileName}?","Accept",
                           async (d,fv)=> {
                               ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;
                               var token = new CancellationTokenSource();
                               var  fileUploadResult = await FileUploadService.Instance.UploadFileAsync(fileModel,token.Token);

                               var updatedDoc = items[position];
                               updatedDoc.Image = fileUploadResult.FileLink;
                               updatedDoc.SourceLink = fileUploadResult.FileLink;
                               updatedDoc.PrimaryFieldText = fileModel.FileName;
                            var result =     await TokService.Instance.UpdateTokAsync(updatedDoc);
                               if (result.ResultEnum == Shared.Helpers.Result.Success)
                               {
                                   ClassGroupDocsFragment.Instance.Init();
                                   showAlertDialogs("File sucessfully updated!");
                                   
                               }
                               else {
                                   showAlertDialogs("File failed to update!");
                               }
                               ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;
                           },2);

                        break;
                    case "remove doc from group":
                        var dialogDelete = new AlertDialog.Builder(context);
                        var alertD = dialogDelete.Create();
                        alertD.SetTitle("");
                        alertD.SetIcon(Resource.Drawable.alert_icon_blue);
                        alertD.SetMessage($"Do you want to delete {items[position].PrimaryFieldText}?");
                        alertD.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), async (d, fv) =>
                        {
                           // ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;
                          var result =  await TokService.Instance.DeleteTokAsync(items[position].Id,items[position].PartitionKey);
                            if (result.ResultEnum == Shared.Helpers.Result.Success)
                            {
                                showAlertDialogs("Document deleted!");
                                ClassGroupDocsFragment.Instance.Init();
                            }
                            else {
                                showAlertDialogs("Failed deleting document!");
                            }
                         //   ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;
                        });
                        alertD.SetButton2(Html.FromHtml("<font color='#007bff'>Cancel</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                        {

                        });
                        alertD.Show();
                        alertD.SetCanceledOnTouchOutside(false);
                        break;
                    case "report":
                        ClassGroupActivity.Instance.SelectedDoc = items[position];
                        ClassGroupActivity.Instance.Report = new ReportDialouge(context);
                        ClassGroupActivity.Instance.Report.Show();
                        break;
                }
            };

            // Menu was dismissed:
            menu.DismissEvent += (s2, arg2) => {
                //Console.WriteLine("menu dismissed");
            };

            menu.Show();
        }

        private void showAlertDialogs(string message,string Okbutton="OK",EventHandler<DialogClickEventArgs> handler = null, int button = 1)
        {
            if (handler == null)
                handler = (d, fv) =>
                {

                };
            var dialogDelete = new AlertDialog.Builder(context);
            var alertD = dialogDelete.Create();
            alertD.SetTitle("");
            alertD.SetIcon(Resource.Drawable.alert_icon_blue);
            alertD.SetMessage(message);
            if(button == 1)
            alertD.SetButton(Html.FromHtml($"<font color='#007bff'>{Okbutton}</font>", FromHtmlOptions.ModeLegacy), handler);
            if (button == 2) {
                alertD.SetButton(Html.FromHtml($"<font color='#007bff'>{Okbutton}</font>", FromHtmlOptions.ModeLegacy), handler);
                alertD.SetButton2(Html.FromHtml($"<font color='#007bff'>Cancel</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            }

            alertD.Show();
            alertD.SetCanceledOnTouchOutside(false);
        }

        public void OnItemRowClick(object sender, int position)
        {
            string format = "https://docs.google.com/viewerng/viewer?url=";
            string source = string.IsNullOrEmpty(items[position].SourceLink) ? items[position].Image : items[position].SourceLink;
            string fullPath = string.Empty;
            if (FileExtensionHelpers.IsImageUri(source))
            {
                fullPath = source;
            }
            else {
                fullPath = format + source;
            }
            Intent browserIntent = new Intent(action: Intent.ActionView, uri: NetUri.Parse(fullPath));
             context.StartActivity(browserIntent);
            //TODO go to the page once tapped
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.classgroup_docs_row, parent, false);

            vh = new ClassGroupDocsViewHolder(itemView, OnClick);
            return vh;
        }
        #endregion

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {

            }
            return true;
        }
        #endregion
    }
}