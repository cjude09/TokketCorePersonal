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
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Tokket.Android.TokQuest;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using static Android.Resource;
using Android.Views.Animations;
using AlertDialog = Android.App.AlertDialog;
using Animation = Android.Views.Animations.Animation;
using AnimationUtils = Android.Views.Animations.AnimationUtils;
using Color = Android.Graphics.Color;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Students", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StudentAttendanceMain : BaseActivity
    {
        public HubConnection _Connections;
        public TokQuestMultiplayer tokquestMultiplayer;
        public TokquestPlayer tokquestPlayer;
        ObservableCollection<TokquestPlayer> PlayersSetsCollection;
        ObservableCollection<string> BtnSetsCollection;

        GridLayoutManager mLayoutManager;
        GridLayoutManager mLayoutManagerGameplay;
        private AlertDialog.Builder dialog;
        private AlertDialog alert;
        
        private GamePlayHolder gamePlayHolder;

        private static string[] places = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th" };

        System.Threading.Timer TheTimer = null;
        int TimeCounter = 0;
        int MaxTimeCounter = 0;

        internal static StudentAttendanceMain Instance { get; private set; }
        private enum ActivityName
        {
            Filter = 1001
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.students_attendance_layout);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Instance = this;
            Settings.ActivityInt = (int)ActivityType.StudentAttendanceMain;

            Instance = this;

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerAttendList.SetLayoutManager(mLayoutManager);
            PlayersSetsCollection = new ObservableCollection<TokquestPlayer>();
            RunOnUiThread(async () => await Initialize());

            mLayoutManagerGameplay = new GridLayoutManager(Application.Context, numcol);
            RecyclerAttendListChoices.SetLayoutManager(mLayoutManagerGameplay);
            BtnSetsCollection = new ObservableCollection<string>();

            TokquestGamePlayExit.Click += TokquestGamePlayExit_Click;
            gamePlayHolder = new GamePlayHolder();
            dialog = new AlertDialog.Builder(this);
            alert = dialog.Create();
            alert.SetTitle("Alert");
            alert.SetIcon(Resource.Drawable.tokquesticon);
            checkOrExContinue.Click += CheckOrExContinue_Click;
            checkOrExContinueOver.Click += CheckOrExContinueOver_Click;




        }

        private void CheckOrExContinueOver_Click(object sender, EventArgs e)
        {
           
                Finish();
                TokquestGameService.Instance.Connections.StopAsync();
        }

        private async void CheckOrExContinue_Click(object sender, EventArgs e)
        {
            linearGameAlert.Visibility = ViewStates.Gone;
            linearGameAlert.Alpha = 0.0f;
            var gamemessage = new GameMessage
            {

                id = Settings.GetTokketUser().Id + "-tokquestplayer",
                point = gamePlayHolder.Score,
                round = gamePlayHolder.Round,
                correctanswers = gamePlayHolder.TotalCorrect,
                Type = "message"

            };

            await TokquestGameService.Instance.SendMessage(tokquestMultiplayer.id, gamemessage);
            GamesSetter();
           
        }

        private void RunTimer(int MaxValue) {

            TimeCounter = MaxValue;
            MaxTimeCounter = MaxValue;
            TheTimer = new System.Threading.Timer(
                this.Tick, null, 0, 1000);
        }

        public void Tick(object info)
        {

            if (TokquestGameService.Instance.Connections.State == HubConnectionState.Connected)
            {

                TimeCounter--;
                // times up
                if (TimeCounter <= 0)
                {

                    RunOnUiThread(()=> {
                        dialog = new AlertDialog.Builder(this);
                        alert = dialog.Create();
                        alert.SetTitle("Alert");
                        alert.SetIcon(Resource.Drawable.tokquesticon);
                        StopTimer();
                        gamePlayHolder.TotalWrong++;
                        alert.SetMessage("Time's Up!");
                        alert.SetButton("Ok", async (c, ev) =>
                        {
                            alert.Dismiss();
                            alert.Hide();
                            var gamemessage = new GameMessage
                            {

                                id = Settings.GetTokketUser().Id + "-tokquestplayer",
                                point = gamePlayHolder.Score,
                                round = gamePlayHolder.Round,
                                correctanswers = gamePlayHolder.TotalCorrect,
                                Type = "message"

                            };

                            await TokquestGameService.Instance.SendMessage(tokquestMultiplayer.id, gamemessage);
                            GamesSetter();
                        });
                        alert.Show();


                    });
                   
                }
                TimerTokquest.Text = TimeCounter.ToString();

            }
            else {

                RunOnUiThread(() => {
                    dialog = new AlertDialog.Builder(this);
                    alert = dialog.Create();
                    alert.SetTitle("Alert");
                    alert.SetIcon(Resource.Drawable.tokquesticon);
                    StopTimer();
                    gamePlayHolder.TotalWrong++;
                    alert.SetMessage("Disconnected!!");
                    alert.SetButton("Ok", (c, ev) =>
                    {
                        alert.Dismiss();
                        alert.Hide();
                        Finish();
                    });
                    alert.Show();

                });
                
            }

        }

        private void StopTimer()
        {
            TimeCounter = 0;
            TimerTokquest.Text = TimeCounter.ToString();
            TheTimer.Dispose();
        }





        private void TokquestGamePlayExit_Click(object sender, EventArgs e)
        {
            
            alert.SetMessage("Are you sure you want to exit?");
            alert.SetButton("Confirm", (c, ev) =>
            {

                try
                {
                    TokquestGameService.Instance.Connections.StopAsync();

                }
                catch (Exception err)
                {

                    Console.WriteLine(err.Message);
                }
                Finish();
            });
            
            alert.SetButton2("CANCEL", (c, ev) => {
                alert.Dismiss();
                alert.Hide();
            });

            alert.Show();
        }

        private async Task Initialize()
        {


            try
            {
                var class_group = JsonConvert.DeserializeObject<ClassGroupModel>(Intent.GetStringExtra("ClassGroup"));
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
                                tokquestMultiplayer.leaderId = class_group.UserId;
                                tokquestMultiplayer.label = "tokquestmultiplayer";
                                tokquestMultiplayer.pk = class_group.Id;
                                tokquestMultiplayer.id = class_group.Id;
                                tokquestMultiplayer.isActive = false;

                                var res = await TokquestGameService.Instance.AddToClassGroupRoom(userId, false, tokquestMultiplayer, tokquestPlayer);
                                TokquestGameService.Instance.Connections.On<IEnumerable<TokquestPlayer>>("student_waiting", model =>
                                {
                                    //Console.WriteLine(model.Count());

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {

                                        RecyclerAttendList.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();
                                        foreach (var item in model.Where(x => x.is_teacher == false).ToList())
                                        {
                                            PlayersSetsCollection.Add(item);
                                        }

                                        SetRecyclerAdapter();


                                    });
                                });


                                TokquestGameService.Instance.Connections.On<IEnumerable<TokquestPlayer>>("teacher_waiting", model =>
                                {
                                    //Console.WriteLine(model.Count());

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {

                                        RecyclerAttendList.ContentDescription = "none";
                                        PlayersSetsCollection.Clear();
                                        foreach (var item in model.Where(x=> x.is_teacher == false).ToList())
                                        {
                                            PlayersSetsCollection.Add(item);
                                        }

                                        SetRecyclerAdapter();


                                    });


                                });


                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("Game_Over_Trigger", model =>
                                {
                                    
                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        linearProgressGameOver.Visibility = ViewStates.Gone;
                                        var tokquestUserId = Settings.GetTokketUser().Id + "-tokquestplayer";
                                        var me = model.tokquestPlayers.Where(x => x.Id == tokquestUserId).FirstOrDefault();
                                        var place = model.tokquestPlayers.IndexOf(me);
                                        var gameMessage = $"You finished in {places[place]} place and scored {me.total_point} points!";

                                        var wrong_answers = (model.gameLength - me.correct_answers);
                                        AlertShowGameOver(gameMessage,me.total_point.ToString()
                                           ,me.correct_answers.ToString(),wrong_answers.ToString());


                                    });


                                });



                                TokquestGameService.Instance.Connections.On<TokQuestMultiplayer>("start_game", model =>
                                {

                                    //Console.WriteLine(model.label);
                                    MainThread.BeginInvokeOnMainThread( async () =>
                                    {

                                        try
                                        {
                                            linearProgressAttend.Visibility = ViewStates.Visible;
                                            var gameObject = await TokquestService.Instance.GetGameset(model.gameId, model.pk + "-gamesets-0");

                                            if (gameObject != null)
                                            {
                                                gamePlayHolder.CurrentGameObject = gameObject;
                                                gamePlayHolder.Round = 1;
                                                gamePlayHolder.Score = 0;
                                                gamePlayHolder.TotalCorrect = 0;
                                                gamePlayHolder.TotalWrong = 0;
                                                //swipeRefreshRecyclerAttend.Visibility = ViewStates.Gone;
                                                RecyclerAttendList.Visibility = ViewStates.Gone;
                                                TextNothingFoundAttend.Visibility = ViewStates.Gone;
                                                GamePlayDiv.Visibility = ViewStates.Visible;
                                                SupportActionBar.Hide();

                                                var getter = gameObject.GameListObject[gamePlayHolder.Round -1].choices.ToList();
                                                gameplayTextQuestion.Text = gameObject.GameListObject[gamePlayHolder.Round - 1].question;
                                                TimerTokquest.Text = gameObject.GameListObject[gamePlayHolder.Round - 1].Time;
                                                BtnSetsCollection.Clear();
                                                foreach (var t in getter)
                                                {
                                                    BtnSetsCollection.Add(t);

                                                }
                                                SetRecyclerAdapterBtn();

                                                RunTimer(Convert.ToInt32(TimerTokquest.Text));

                                            }



                                        }
                                        catch (Exception err)
                                        {

                                            Console.WriteLine(err.Message + "error in start game");
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

        //game setter 
        private void GamesSetter()
        {
            StopTimer();
            gamePlayHolder.Round++;

            if (gamePlayHolder.Round > gamePlayHolder.CurrentGameObject.GameListObject.Count())
            {
                // game over
                RunOnUiThread(() => {

                    gameplayTextQuestion.Visibility = ViewStates.Gone;
                    RecyclerAttendListChoices.Visibility = ViewStates.Gone;
                    linearProgressGameOver.Visibility = ViewStates.Visible;

                });

            }
            else {
                var getter = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].choices.ToList();
                gameplayTextQuestion.Text = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].question;
                TimerTokquest.Text = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].Time;
                BtnSetsCollection.Clear();
                foreach (var t in getter)
                {
                    BtnSetsCollection.Add(t);

                }

                SetRecyclerAdapterBtn();
                RunTimer(Convert.ToInt32(TimerTokquest.Text));
            }


        }

        private void SetRecyclerAdapterBtn()
        {
            var adapterClassGroup = BtnSetsCollection.GetRecyclerAdapter(BindClassGroupViewHolderBtn, Resource.Layout.choices_buttons);
            RecyclerAttendListChoices.SetAdapter(adapterClassGroup);

            linearProgressAttend.Visibility = ViewStates.Invisible;
            
            gameplayTextQuestion.Alpha = 0.0f;
            gameplayTextQuestion.TranslationX = -500.0f;
            gameplayTextQuestion.Animate().Alpha(1.0f).SetDuration(1000).Start();
            gameplayTextQuestion.Animate().TranslationX(0.0f).SetDuration(700).Start();



            RecyclerAttendListChoices.Alpha = 0.0f;
            RecyclerAttendListChoices.TranslationX = -500.0f;
            RecyclerAttendListChoices.Animate().Alpha(1.0f).SetDuration(1000).Start();
            RecyclerAttendListChoices.Animate().TranslationX(0.0f).SetDuration(700).Start();

        }


        private void BindClassGroupViewHolderBtn(CachingViewHolder holder, string model, int position)
        {   var textName = holder.FindCachedViewById<Button>(Resource.Id.PlayersChoices);

            string[] colors = { "#ffc000", "#cccc00", "#09caf7", "#6a0bf5", "black", "#f711ec" };
            textName.SetBackgroundColor(Color.ParseColor(colors[position]));
            textName.Text = model;
            textName.Click +=  TextName_ClickAsync; 
            
        }

        //checker
        private void TextName_ClickAsync(object sender, EventArgs e)
        {

            var me = (Button)sender;
            var test = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].answer.Contains(me.Text);
            var question_kind = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].QuestionKind;
          
            var current_Q = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].question;

            var current_Ans = gamePlayHolder.CurrentGameObject.GameListObject[gamePlayHolder.Round - 1].answer[0];

            if (test == true)
            {
                var total_score = 0;
                if (question_kind == "1") {

                    total_score = (TimeCounter + 2);

                }
                else if(question_kind == "2") {
                    
                    total_score = (TimeCounter + 5);

                }
                gamePlayHolder.TotalCorrect++;
                gamePlayHolder.Score += total_score;

                string textMessage = current_Q + "? :" + current_Ans;
                AlertShow(true,textMessage);



            }
            else if(test == false)
            {

                gamePlayHolder.TotalWrong++;
                string textMessage = current_Q + "? :" + current_Ans;
                AlertShow(false, textMessage);

            }


            StopTimer();
        }


        private void AlertShow(bool isCorrect, string textMessage) {

            if (isCorrect)
            {
                checkOrExImg.SetImageResource(Resource.Drawable.check_green);
                checkOrExText.Text = "Correct!";

            }
            else {
                checkOrExImg.SetImageResource(Resource.Drawable.x_red);
                checkOrExText.Text = "Wrong!";

            }

            checkOrExTextAnswer.Text = textMessage;
            linearGameAlert.Visibility = ViewStates.Visible;
            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.zoom_in);
            linearGameAlert.StartAnimation(myAnim);
            linearGameAlert.Alpha = 0.0f;
            linearGameAlert.Animate().Alpha(1.0f).SetDuration(400).Start();
        
        }

        private void AlertShowGameOver(string textMessage, string score , string total_correct, string total_wrong)
        {

            final_score.Text = score;
            final_correct_answer.Text = total_correct;
            final_wrong_answer.Text = total_wrong;

            checkOrExTextAnswerOver.Text = textMessage;
            linearGameAlertOver.Visibility = ViewStates.Visible;
            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.zoom_in);
            linearGameAlertOver.StartAnimation(myAnim);
            linearGameAlertOver.Alpha = 0.0f;
            linearGameAlertOver.Animate().Alpha(1.0f).SetDuration(400).Start();

        }

        private void SetRecyclerAdapter()
        {
            var adapterClassGroup = PlayersSetsCollection.GetRecyclerAdapter(BindClassGroupViewHolder, Resource.Layout.student_attendace_row);
            RecyclerAttendList.SetAdapter(adapterClassGroup);

            linearProgressAttend.Visibility = ViewStates.Invisible;
        }

        private void BindClassGroupViewHolder(CachingViewHolder holder, TokquestPlayer model, int position)
        {
            ImageView imgPlayer = holder.FindCachedViewById<ImageView>(Resource.Id.tokquestPLayerImage);
            var textName = holder.FindCachedViewById<Button>(Resource.Id.tokquestPlayerText);
            textName.Text = model.FullName;


            //var imageBitmap = GetImageBitmapFromUrl(model.user_photo);

            //if (imageBitmap == null)
            //{
            //    imgPlayer.SetImageResource(Resource.Drawable.tokquestLogo);
            //}
            //else {
            //    imgPlayer.SetImageBitmap(imageBitmap);

            //}

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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    try
                    {
                        TokquestGameService.Instance.Connections.StopAsync();

                    }
                    catch (Exception err)
                    {

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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            try
            {
                TokquestGameService.Instance.Connections.StopAsync();

            }
            catch (Exception err)
            {

                Console.WriteLine(err.Message);
            }
            Finish();

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
                Console.WriteLine(err.Message  + ": error in image fetch");
                imageBitmap = null;
            }
           

            return imageBitmap;
        }

       

        public RecyclerView RecyclerAttendList => FindViewById<RecyclerView>(Resource.Id.RecyclerAttendList);
        public LinearLayout linearProgressAttend => FindViewById<LinearLayout>(Resource.Id.linearProgressAttend);
    
        public LinearLayout GamePlayDiv => FindViewById<LinearLayout>(Resource.Id.GamePlayDiv);
        public TextView TextNothingFoundAttend => FindViewById<TextView>(Resource.Id.TextNothingFoundAttend);

        //public SwipeRefreshLayout swipeRefreshRecyclerAttend => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerAttend);

        //public SwipeRefreshLayout swipeRefreshRecyclerAttendChoices => FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshRecyclerAttendChoices);
        
        public RecyclerView RecyclerAttendListChoices => FindViewById<RecyclerView>(Resource.Id.RecyclerAttendListChoices);

        public TextView gameplayTextQuestion => FindViewById<TextView>(Resource.Id.gameplayTextQuestion);

        public Button TimerTokquest => FindViewById<Button>(Resource.Id.TimerTokquest);
        public Button TokquestGamePlayExit => FindViewById<Button>(Resource.Id.TokquestGamePlayExit);
        public LinearLayout linearProgressGameOver => FindViewById<LinearLayout>(Resource.Id.linearProgressGameOver);

        public LinearLayout linearGameAlert => FindViewById<LinearLayout>(Resource.Id.linearGameAlert);
        public ImageView checkOrExImg => FindViewById<ImageView>(Resource.Id.checkOrExImg);

        public Button checkOrExContinue => FindViewById<Button>(Resource.Id.checkOrExContinue);
        public TextView checkOrExText => FindViewById<TextView>(Resource.Id.checkOrExText);

        public TextView checkOrExTextAnswer => FindViewById<TextView>(Resource.Id.checkOrExTextAnswer);


        public LinearLayout linearGameAlertOver => FindViewById<LinearLayout>(Resource.Id.linearGameAlertOver);


        public TextView checkOrExTextAnswerOver => FindViewById<TextView>(Resource.Id.checkOrExTextAnswerOver);

        public TextView final_wrong_answer => FindViewById<TextView>(Resource.Id.final_wrong_answer);

        public TextView final_correct_answer => FindViewById<TextView>(Resource.Id.final_correct_answer);

        public TextView final_score => FindViewById<TextView>(Resource.Id.final_score);
        
         public Button checkOrExContinueOver => FindViewById<Button>(Resource.Id.checkOrExContinueOver);


    }
}