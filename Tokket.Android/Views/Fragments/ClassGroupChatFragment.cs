using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Tokket.Shared.Models;
using Tokket.Shared.Helpers;
using Android.Graphics;
using Tokket.Core;
using Tokket.Shared.Services;
using Tokket.Shared.Models.Chat;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Tokket.Core.Tools;
using System.Threading.Tasks;
using Tokket.Android.Custom;
using System.Net;
using Com.Airbnb.Lottie;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using NetUri = Android.Net.Uri;
using Tokket.Android.Helpers;

namespace Tokket.Android.Fragments
{
    public class ClassGroupChatFragment : AndroidX.Fragment.App.Fragment
    {
        List<string> commentList;
        ClassGroupModel classGroup;
        View v;
        ObservableCollection<TokChatMessage> TokModelCollection { get; set; }
        private ObservableRecyclerAdapter<TokChatMessage, CachingViewHolder> adapterTokModel;

        private ObservableCollection<FileCommentModel> FileCollections { get; set; }
        private ObservableCollection<string> fileCommentAdapter { get; set; }
        internal static ClassGroupChatFragment Instance { get; private set; }
        ReportDialouge Report;
        TokketUser tokketUser = new TokketUser();
        Context GroupContext;
        bool isdelete = false,isnewcomment=false;
        public static TokChatMessage NewMessageBackUp = new TokChatMessage();

        
        public ClassGroupChatFragment(ClassGroupModel _classGroup,Context context)
        {
            classGroup = _classGroup;
            GroupContext = context;
        }

        HubConnection hubConnection;
        TokChatMessage NewChatMessage;
        public  override  View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.fragment_classgroup_chat, container, false);
            try {
                Instance = this;
                //Use this fragment_classgroup_chat_row
             
                var mLayoutManager = new GridLayoutManager(Context, 1);
                recyclerViewChat.SetLayoutManager(mLayoutManager);
                tokketUser = Settings.GetTokketUser();
            }
            catch (Exception ex) {
                ShowAlerDialog();
            }


            //setAdapter();
            Initialize();

            return v;
        }
        private async void Initialize()
        {
            lottieAnimationView.Visibility = ViewStates.Visible;
            await ChatService.Instance.InitChatHub(Settings.GetTokketUser().Id);
            await ChatService.Instance.AddToClassGroupRoomChat(Settings.GetTokketUser().Id, classGroup.Id);

            try
            {  // messages on first load (gives all the messages in the group chat)
                ChatService.Instance.Connections.On<ResultData<TokChatMessage>>("TokChatFirstLoad", model =>
                {

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            TokModelCollection = new ObservableCollection<TokChatMessage>();
                            commentList = new List<string>();
                            var check = JsonConvert.SerializeObject(model);
                            foreach (var items in model.Results)
                            {
                                TokModelCollection.Add(items);
                                commentList.Add(items.message);
                            }
                            setAdapter();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Found error tokchatfirstload: " + ex.ToString());
                        }

                    });
                });
                // new message receiver
                ChatService.Instance.Connections.On<TokChatMessage>("tokchat", model =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {

                            var check = JsonConvert.SerializeObject(model);
                                //will just receive one message at a time
                                TokModelCollection.Insert(0,model);
                                commentList.Insert(0,model.message);
                            setAdapter();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Found error tokchatfirstload: " + ex.ToString());
                        }

                    });
                });


                ChatService.Instance.Connections.On<TokChatMessage>("tokchatDelete", model =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                                //will just receive one message at a time
                                //TokModelCollection.Remove(model);
                                //commentList.Remove(model.message);

                            for (int i=0; i< TokModelCollection.Count();i++) {
                                if (TokModelCollection[i].id == model.id) {
                                    TokModelCollection.RemoveAt(i);
                                    commentList.RemoveAt(i);
                                    break;
                                }
                            }

                            //TokModelCollection.OrderByDescending(x=> x.created_time);
                            setAdapter();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Found error tokchatfirstload: " + ex.ToString());
                        }

                    });
                });

            }
            catch (Exception err)
            {

                Console.WriteLine(err.Message);
            }
            lottieAnimationView.Visibility = ViewStates.Gone;
        }
        public void addNewComment(TokChatMessage comment)
        {
            //TokModelCollection.Add(comment);
            //commentList.Add(comment.message);
      
            //    setAdapter();
        }
        private void setAdapter()
        {
            adapterTokModel = TokModelCollection.GetRecyclerAdapter(BindTokModelCollection, Resource.Layout.fragment_classgroup_chat_row);
            recyclerViewChat.SetAdapter(adapterTokModel);
        }

        private void setCommentFileAdapter(RecyclerView recyclerView) {
            var adapterDetail = FileCollections.GetRecyclerAdapter(BindFileCommentCollection, Resource.Layout.filecomment_row);
            recyclerView.SetLayoutManager(new GridLayoutManager(this.Context,3));
            recyclerView.SetAdapter(null);
            recyclerView.SetAdapter(adapterDetail);
        }

        private void BindFileCommentCollection(RecyclerView.ViewHolder view, FileCommentModel file, int position)
        {
            
            var TileButton = view.ItemView.FindViewById<ImageView>(Resource.Id.viewFileComments);
            var TileLayout = view.ItemView.FindViewById<RelativeLayout>(Resource.Id.FileCommentLayout);
            var TileFileName = view.ItemView.FindViewById<TextView>(Resource.Id.txtFileCommentName);

            TileFileName.Text = " "+ file.Name;
           
            TileLayout.Click += async (obj, _eve) =>
            {
                //var pdfViewer = new PDFViewerDialouge(this.Context);
                //pdfViewer.WebView.Settings.JavaScriptEnabled = true;
                //pdfViewer.WebView.LoadUrl("https://tokketcontent.blob.core.windows.net/files/file-52af53317f104da49e03757489d7ef2epdf");
                //pdfViewer.FileName.Text = file.Name;
                //pdfViewer.Show();
                string format = "https://docs.google.com/viewerng/viewer?url=";
                string fullPath = format + file.Uri;
                Intent browserIntent = new Intent(action: Intent.ActionView,uri: NetUri.Parse(fullPath));
                StartActivity(browserIntent);
                // await FileService.OpenFileWebBrowser(file.Uri);
            };
        }

        private void ShowAlerDialog(string message = "An error occured on Chat") {
            AlertDialog.Builder alertDiag = new AlertDialog.Builder(Context);
            alertDiag.SetTitle("");
            alertDiag.SetIcon(Resource.Drawable.alert_icon_blue);
            alertDiag.SetMessage(message);
            alertDiag.SetPositiveButton("Cancel", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
            diag.SetCanceledOnTouchOutside(false);
        }

        private void BindTokModelCollection(CachingViewHolder holder, TokChatMessage model, int position)
        {
            try {
                ClassGroupActivity.Instance.SelectedTokChatMessage = model;
                //if (NewMessageBackUp.files?.Count > 0) {
                //    model.files = NewChatMessage.files;
                //    model.filesextension = NewChatMessage.filesextension;
                //    model.filesname = NewChatMessage.filesname;
                //}
                var imgUserDetailPhoto = holder.FindCachedViewById<CircleImageView>(Resource.Id.imgcomment_userphoto);
                var txtUserDisplayName = holder.FindCachedViewById<TextView>(Resource.Id.txtUserDisplayName);
                var txtTimeStamp = holder.FindCachedViewById<TextView>(Resource.Id.txtTimeStamp);
                var txtCommentMessage = holder.FindCachedViewById<TextView>(Resource.Id.txtCommentMessage);
                var layoutToast = holder.FindCachedViewById<LinearLayout>(Resource.Id.linearToast);
                var txtToast = holder.FindCachedViewById<TextView>(Resource.Id.txtToast);
                var fileRecycle = holder.FindCachedViewById<RecyclerView>(Resource.Id.filecommentRecycler);
                var fileCount = model.files?.Count > 0 && model?.files != null;
                var btnMenu = holder.FindCachedViewById<Button>(Resource.Id.btn_menu);
                if (fileCount) {

                    fileRecycle.Visibility = ViewStates.Visible;
               
                        FileCollections = new ObservableCollection<FileCommentModel>();
                    FileCollections.Clear();                        
                    for (int i = 0; i < model.files.Count; i++) {
                        string name = $"Untitled {i}", ext = string.Empty, uri = string.Empty;
                        if (!string.IsNullOrEmpty(model.files[i]))
                            uri = model.files[i];
                        if (!string.IsNullOrEmpty(model.filesname?[i]))
                            name = model.filesname[i];
                        if (!string.IsNullOrEmpty(model.filesextension?[i]))
                            ext = model.filesextension[i];
                        FileCollections.Add(new FileCommentModel() { Uri =uri, Name = name, Extension = ext });
                    }

                    setCommentFileAdapter(fileRecycle);
                }

                //txtCommentMessage.Text = commentList[position];
                txtCommentMessage.Text = TokModelCollection[position].message;
                txtUserDisplayName.Text = model.display_name;
                txtTimeStamp.Text = model.created_time.ToString("MMMM dd, yyyy hh:mm tt");
                txtToast.Text = model.created_time.ToString("MMMM dd, yyyy hh:mm tt");

                if (!string.IsNullOrEmpty(model.image))
                {
                    //Bitmap bitmap = GetBitmapFromUrl(model.image);
                  
                    imgUserDetailPhoto.SetImageURI(NetUri.Parse(model.image));
                }
                else {
                    imgUserDetailPhoto.SetImageResource(Resource.Drawable.no_image);
                }

                txtTimeStamp.Click += delegate
                {
                    layoutToast.Visibility = ViewStates.Visible;

                    var handler = new Handler();
                    handler.PostDelayed(() =>
                    {
                        v.SetBackgroundColor(Color.Transparent);
                        layoutToast.Visibility = ViewStates.Gone;
                    }, 1000);
                };
                btnMenu.Click += (obj, _event) => {
                 var menu = new PopupMenu(this.Context, btnMenu);

                    // Call inflate directly on the menu:
                    menu.Inflate(Resource.Menu.chatrow_menu);
                    var edit = menu.Menu.FindItem(Resource.Id.chatrowEdit);
                    var delete = menu.Menu.FindItem(Resource.Id.chatrowDelete);
                    var report = menu.Menu.FindItem(Resource.Id.chatrowReport);

                    if (model.sender_id == Settings.GetTokketUser().Id)
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

                    if (classGroup.UserId == Settings.GetTokketUser().Id)
                        delete.SetVisible(true);

                    menu.MenuItemClick += async (obj, _event) => {
                        switch (_event.Item.TitleFormatted.ToString().ToLower()) {
                            case "edit":
                                ClassGroupActivity.Instance.LoadEditChat(model);
                                break;
                            case "delete":
                                isdelete = true;
                                TokModelCollection.Remove(model);
                                model.group_pk = classGroup.UserId + "-classgroups0";
                                     await ChatService.Instance.DeleteMessage(classGroup.Id,model);
                              
                                break;
                            case "report":
                                ClassGroupActivity.Instance.Report = new ReportDialouge(GroupContext);
                                ClassGroupActivity.Instance.Report.Show();
                                break;
                        }
                    };

                    menu.DismissEvent += (s1, _event) =>{ 
                    
                    };
    
                    menu.Show();             
                };
                Glide.With(v).Load(model.image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter().CircleCrop()).Into(imgUserDetailPhoto);

            }
            catch (Exception ex) {
                ShowAlerDialog("Error on BindTokModelCollection:"+ex.ToString());
            }
        }


        //get the image from url 
        private Bitmap GetBitmapFromUrl(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] bytes = webClient.DownloadData(url);
                if (bytes != null && bytes.Length > 0)
                {
                    return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                }
            }
            return null;
        }
        public RecyclerView recyclerViewChat => v.FindViewById<RecyclerView>(Resource.Id.recyclerView_Chat);
        public TextView TextNothingFound => v.FindViewById<TextView>(Resource.Id.TextNothingFound);
        public LottieAnimationView lottieAnimationView => v.FindViewById<LottieAnimationView>(Resource.Id.lottieAnimationView);
    }
}