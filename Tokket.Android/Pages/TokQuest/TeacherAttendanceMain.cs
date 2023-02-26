using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Students", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TeacherAttendanceMain : BaseActivity
    {
        public HubConnection _Connections;
        public TokQuestMultiplayer tokquestMultiplayer;
        public TokquestPlayer tokquestPlayer;
        ObservableCollection<TokquestPlayer> PlayersSetsCollection;
        GridLayoutManager mLayoutManager;
        GridLayoutManager mLayoutManagerAfter;

        private bool isGameStarted = false;

        AlertDialog.Builder dialog;
        AlertDialog alert;
        internal static TeacherAttendanceMain Instance { get; private set; }
        private enum ActivityName
        {
            Filter = 1001
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.teacher_attendance_layout);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            Settings.ActivityInt = (int)ActivityType.TeacherAttendanceMain;

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            mLayoutManagerAfter = new GridLayoutManager(Application.Context, numcol);

            RecyclerAttendListTeacher.SetLayoutManager(mLayoutManager);
            
            RecyclerAttendListTeacherAfter.SetLayoutManager(mLayoutManagerAfter);


            PlayersSetsCollection = new ObservableCollection<TokquestPlayer>();
            RunOnUiThread(async () => await Initialize());
            AttendTeacherBtnStart.Click += AttendTeacherBtnStart_ClickAsync;
            isGameStarted = false;
        }

        private async void AttendTeacherBtnStart_ClickAsync(object sender, EventArgs e)
        {
            if (PlayersSetsCollection.Count >= 2)
            {

                linearProgressAttendTeacher.Visibility = ViewStates.Visible;
                var res = await TokquestGameService.Instance.GameStart(tokquestMultiplayer.pk, tokquestMultiplayer);
                //start game 
                RecyclerAttendListTeacher.Visibility = ViewStates.Gone;
                AttendTeacherBtnStartDiv.Visibility = ViewStates.Gone;
                //swipeRefreshRecyclerAttendTeacher.Visibility = ViewStates.Gone;
                TextNothingFoundAttendTeacher.Visibility = ViewStates.Gone;
                TeacherGamePlayDiv.Visibility = ViewStates.Visible;

                TextNothingFoundAttendTeacher.Visibility = ViewStates.Gone;
                TextNothingFoundAttendTeacher.Text = "";

            }
            else {

                TextNothingFoundAttendTeacher.Visibility = ViewStates.Visible;
                TextNothingFoundAttendTeacher.Text = "2 members needed to continue!";
            
            }


        }

        private async Task Initialize()
        {


            try
            {
                var gameplayOptionsSet = JsonConvert.DeserializeObject<TokquestGamePlayOptions>(Intent.GetStringExtra("gameplayOptionsSet"));
                
                var tokquestUserId = Settings.GetTokketUser().Id + "-tokquestplayer";
                var userId = Settings.GetTokketUser().Id;

                var ConnectionInfo = await TokquestGameService.Instance.GetSignalRConnectionInfoGroup(tokquestUserId, true);

                if (ConnectionInfo.AccessToken == null)
                {
                    await TokquestGameService.Instance.Connections.StopAsync();
                }
                TokquestGameService.Instance.Connections = new HubConnectionBuilder()
                        .WithUrl(ConnectionInfo.Url, options =>
                        {
                            options.AccessTokenProvider = () => Task.FromResult(ConnectionInfo.AccessToken);
                        }).Build();
                await TokquestGameService.Instance.Connections.StartAsync().ContinueWith(x =>
                {

                    if (TokquestGameService.Instance.Connections.State == HubConnectionState.Connected)
                    {
                        try
                        {

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {

                                tokquestPlayer = new TokquestPlayer
                                {
                                    FullName = Settings.GetTokketUser().DisplayName,
                                    Id = Settings.GetTokketUser().Id,
                                    user_photo = Settings.GetTokketUser().UserPhoto
                                };

                                tokquestMultiplayer = new TokQuestMultiplayer();
                                tokquestMultiplayer.leaderId = Settings.GetTokketUser().Id;
                                tokquestMultiplayer.label = "tokquestmultiplayer";
                                tokquestMultiplayer.pk = gameplayOptionsSet.GroupId;
                                tokquestMultiplayer.id = gameplayOptionsSet.GroupId;
                                tokquestMultiplayer.isActive = false;
                                tokquestMultiplayer.gameId = gameplayOptionsSet.GameId;
                                tokquestMultiplayer.gameType = gameplayOptionsSet.GameType;
                                tokquestMultiplayer.gameLength = gameplayOptionsSet.GameLent;

                                var res = await TokquestGameService.Instance.AddToClassGroupRoom(userId, true, tokquestMultiplayer, tokquestPlayer);

                                TokquestGameService.Instance.Connections.On<IEnumerable<TokquestPlayer>>("teacher_waiting", model =>
                                {
                                    //Console.WriteLine(model.Count());

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {

                                        RecyclerAttendListTeacher.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();

                                        var models = model.Where(x => x.is_teacher == false).ToList();
                                        
                                        foreach (var item in models)
                                        {
                                            PlayersSetsCollection.Add(item);
                                        }

                                        SetRecyclerAdapter(true ,models.Count());

                                    });
                                });

                                TokquestGameService.Instance.Connections.On<IEnumerable<TokquestPlayer>>("student_waiting", model =>
                                {
                                    
                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {

                                        RecyclerAttendListTeacher.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();
                                        var models = model.Where(x => x.is_teacher == false).ToList();
                                        foreach (var item in models)
                                        {
                                            PlayersSetsCollection.Add(item);
                                        }
                                        var btnSetter = model.Where(x => x.is_teacher == true).FirstOrDefault();
                                        if (btnSetter != null)
                                        {
                                            SetRecyclerAdapter(true, models.Count());

                                        }
                                        else
                                        {
                                            SetRecyclerAdapter(false, models.Count());

                                        }




                                    });
                                });


                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("start_game", model =>
                                {

                                
                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        isGameStarted = true;
                                        GameResetter();
                                        RecyclerAttendListTeacherAfter.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();
                                        var models = model.tokquestPlayers.Where(x => x.is_teacher == false).ToList(); 
                                        foreach (var item in models)
                                        {
                                       
                                            PlayersSetsCollection.Add(item);
                                        }
                                        linearProgressAttendTeacher.Visibility = ViewStates.Visible;
                                        SetRecyclerAdapterAfter();

                                    });
                                });

                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("message", model =>
                                {


                                    MainThread.BeginInvokeOnMainThread(async () =>
                                    {
                                        RecyclerAttendListTeacherAfter.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();
                                        var models = model.tokquestPlayers.Where(x => x.is_teacher == false).ToList();
                                        foreach (var item in models)
                                        {
                                            PlayersSetsCollection.Add(item);
                                        }
                                        linearProgressAttendTeacher.Visibility = ViewStates.Visible;
                                        SetRecyclerAdapterAfter();
                                       
                                        var checkIfDone = model.tokquestPlayers.Where(x => x.is_finished == true || x.is_active == false).ToList();

                                        // game over trigger
                                        if (checkIfDone.Count  >= (model.tokquestPlayers.Count - 1)) {

                                            var tokquestUserId = Settings.GetTokketUser().Id + "-tokquestplayer";
                                            var game_message = new GameMessage();
                                            game_message.id = tokquestUserId;
                                            game_message.Type = "Game_Over_Trigger";
                                            await TokquestGameService.Instance.SendMessage(model.id,game_message);

                                        }

                                                                                                                
                                    });
                                });


                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("Game_Over_Trigger", model =>
                                {


                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        GameOverText.Visibility = ViewStates.Visible;
                                        TeacherGamePlayDiv.TranslationY = 100.0f;
                                        RecyclerAttendListTeacherAfter.TranslationY = 100.0f;
                                    

                                    });

                                });

                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("quiter", model =>
                                {

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        try
                                        {
                                            if (isGameStarted) {
                                                PlayersSetsCollection.Clear();
                                                var models = model.tokquestPlayers.Where(x => x.is_teacher == false).ToList();
                                                foreach (var item in models)
                                                {
                                                    PlayersSetsCollection.Add(item);
                                                }
                                                linearProgressAttendTeacher.Visibility = ViewStates.Visible;
                                                SetRecyclerAdapterAfter();

                                                var checkIfDone = model.tokquestPlayers.Where(x => x.is_active == false).ToList();
                                                // game over trigger
                                                if (checkIfDone.Count >= (model.tokquestPlayers.Count - 1))
                                                {
                                                    TokquestGameService.Instance.Connections.StopAsync();
                                                    Finish();
                                                }

                                            } 

                                        }
                                        catch (Exception err)
                                        {

                                            Console.WriteLine(err.Message + "error in quiter");
                                        }
                                     
                                    });
                                });

                                TokquestGameService.Instance.Connections.On<TokquestPlayer>("pre_quiter_name", model =>
                                {

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        try
                                        {
                                            
                                            if (isGameStarted == false)
                                            {

                                                
                                                for (int i =0;i< PlayersSetsCollection.Count;i++) {
                                                    if (model.Id == PlayersSetsCollection[i].Id) {
                                                        PlayersSetsCollection.RemoveAt(i);
                                                        break;
                                                    }
                                                }


                                                if (PlayersSetsCollection.Count <= 0)
                                                {
                                                    PlayersSetsCollection.Clear();
                                                    SetRecyclerAdapter(false, 0);
                                                }
                                                else {

                                                    var btnSetter = PlayersSetsCollection.Where(x => x.is_teacher == true).FirstOrDefault();
                                                    if (btnSetter != null)
                                                    {
                                                        SetRecyclerAdapter(true, PlayersSetsCollection.Count());

                                                    }
                                                    else
                                                    {
                                                        SetRecyclerAdapter(false, PlayersSetsCollection.Count());

                                                    }

                                                }


                                            }

                                        }
                                        catch (Exception err)
                                        {
                                            Console.WriteLine(err.Message + "error in pre");
                                        }

                                    });
                                });


                            });

                        }
                        catch (Exception err)
                        {

                            Console.WriteLine(err.Message + ": error in Instance.Connections.State");
                        }


                    }


                });


            }
            catch (Exception err)
            {

                Console.WriteLine(err.Message + "err in MainActivity try");
            }


        }


        private void SetRecyclerAdapter(bool isTeacher , int membersCount)
        {
            var adapterClassGroup = PlayersSetsCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.student_attendace_row);
            RecyclerAttendListTeacher.SetAdapter(adapterClassGroup);

            linearProgressAttendTeacher.Visibility = ViewStates.Invisible;
           
            if(isTeacher)
            AttendTeacherBtnStartDiv.Visibility = ViewStates.Visible;

            if (membersCount <= 0)
            {

                TextNothingFoundAttendTeacher.Visibility = ViewStates.Visible;
                TextNothingFoundAttendTeacher.Text = "No members yet";
            }
            else {

                TextNothingFoundAttendTeacher.Visibility = ViewStates.Gone;
                TextNothingFoundAttendTeacher.Text = "";

            }

        }

        private void SetRecyclerAdapterAfter()
        {
            RecyclerAttendListTeacherAfter.Visibility = ViewStates.Visible;

            var adapterClassGroup = PlayersSetsCollection.GetRecyclerAdapter(BindClassGroupViewHolderAfter, Resource.Layout.gameplay_attendance_row);
            RecyclerAttendListTeacherAfter.SetAdapter(adapterClassGroup);

            linearProgressAttendTeacher.Visibility = ViewStates.Invisible;
         

        }

        private void BindClassGroupViewHolderAfter(CachingViewHolder holder, TokquestPlayer model, int position)
        {
            var textName = holder.FindCachedViewById<TextView>(Resource.Id.TextPlayerName);
            textName.ContentDescription = tokquestPlayer.Id;
            textName.Text = model.FullName;
            

            var textPoints = holder.FindCachedViewById<TextView>(Resource.Id.TextPlayerPoints);
            textPoints.Text =  model.total_point.ToString();

            var textCorrect = holder.FindCachedViewById<TextView>(Resource.Id.TextCorrect);
            textCorrect.Text = model.correct_answers.ToString();

            var textActive = holder.FindCachedViewById<TextView>(Resource.Id.TextActive);
            textActive.Text = model.is_active.ToString();
            if (model.is_active == false)
            {
                textActive.SetBackgroundColor(Color.Red);
                textActive.SetTextColor(Color.White);

            }


            // round 
            var TextQAns = holder.FindCachedViewById<TextView>(Resource.Id.TextQAns);
           

            var textFinished = holder.FindCachedViewById<TextView>(Resource.Id.TextFinished);
            textFinished.Text = model.is_finished.ToString();
            if (model.is_finished)
            {
                textFinished.SetBackgroundColor(Color.Green);
                textFinished.SetTextColor(Color.White);
                int rounder = model.Round;
                TextQAns.Text = rounder.ToString();

            }
            else {
                int rounder = model.Round + 1;
                TextQAns.Text = rounder.ToString();

            }

        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, TokquestPlayer model, int position)
        {
            ImageView imgPlayer = holder.FindCachedViewById<ImageView>(Resource.Id.tokquestPLayerImage);
            var textName = holder.FindCachedViewById<Button>(Resource.Id.tokquestPlayerText);
            textName.Text = model.FullName;
            textName.ContentDescription = model.Id;


            var imageBitmap = GetImageBitmapFromUrl(model.user_photo);

            if (imageBitmap == null)
            {
                imgPlayer.SetImageResource(Resource.Drawable.tokquestLogo);
            }
            else
            {
                imgPlayer.SetImageBitmap(imageBitmap);

            }
        
            imgPlayer.Click += ImgPlayer_Click;

        }

        private void ImgPlayer_Click(object sender, EventArgs e)
        {
            Console.WriteLine("not yet implemented");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            menu.Clear();
            return base.OnCreateOptionsMenu(menu);

        }

        private void GameResetter() {

            GameOverText.Visibility = ViewStates.Gone;
          

        }

        public override void OnBackPressed()
        {
            isGameStarted = false;
            TokquestGameService.Instance.Connections.StopAsync();
            Finish();
            
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    try
                    {
                        isGameStarted = false;
                        TokquestGameService.Instance.Connections.StopAsync();

                    }
                    catch (Exception err)
                    {
                        isGameStarted = false;
                        Console.WriteLine(err.Message);
                    }
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


        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + ": error in image fetch");
                imageBitmap = null;
            }


            return imageBitmap;
        }

        public RecyclerView RecyclerAttendListTeacher => FindViewById<RecyclerView>(Resource.Id.RecyclerAttendListTeacher);
        public LinearLayout linearProgressAttendTeacher => FindViewById<LinearLayout>(Resource.Id.linearProgressAttendTeacher);

        public LinearLayout AttendTeacherBtnStartDiv => FindViewById<LinearLayout>(Resource.Id.AttendTeacherBtnStartDiv);

        public Button AttendTeacherBtnStart => FindViewById<Button>(Resource.Id.AttendTeacherBtnStart);

        public TextView TextNothingFoundAttendTeacher => FindViewById<TextView>(Resource.Id.TextNothingFoundAttendTeacher);

        public LinearLayout TeacherGamePlayDiv => FindViewById<LinearLayout>(Resource.Id.TeacherGamePlayDiv);

        //public SwipeRefreshLayout swipeRefreshRecyclerAttendTeacher => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerAttendTeacher);

        //public SwipeRefreshLayout swipeRefreshRecyclerAttendTeacherAfter => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerAttendTeacherAfter);

        public RecyclerView RecyclerAttendListTeacherAfter => FindViewById<RecyclerView>(Resource.Id.RecyclerAttendListTeacherAfter);
        
        public LinearLayout GameOverText => FindViewById<LinearLayout>(Resource.Id.GameOverText);


    }
}