using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using Newtonsoft.Json;
using Tokket.Android.Listener;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Tokket.Android.TokQuest;
using Tokket.Core.Tools;
using Xamarin.Essentials;

namespace Tokket.Android
{
    [Activity(Label = "Game Categories", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameCategoryMain : BaseActivity
    {
        internal static GameCategoryMain Instance { get; private set; }
        private enum ActivityName
        {
            Filter = 1001
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_category);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Settings.ActivityInt = (int)ActivityType.GameCategoryMain;

            Instance = this;

            btnRace.Click += BtnRace_Click;
        }

        private void BtnRace_Click(object sender, EventArgs e)
        {

            var setter = Intent.GetStringExtra("gameplayOptions");
            var setGameId = setter.Split("==")[0];
            var setGroupId = setter.Split("==")[1];
            var setGroupId2 = setGroupId.Split("-gamesets-0")[0];
            var gamelent = Convert.ToInt32(setter.Split("===")[1]);

            var gameplayOptions = new TokquestGamePlayOptions(); //JsonConvert.DeserializeObject<TokquestGamePlayOptions>(Intent.GetStringExtra("gameplayOptions"));
            gameplayOptions.GameType = "Race";
            gameplayOptions.GameId = setGameId;
            gameplayOptions.GroupId = setGroupId2;
            gameplayOptions.GameLent = gamelent;
            
            Console.WriteLine(gameplayOptions.GameId);
            Console.WriteLine(gameplayOptions.GroupId);

            var gameplayOptionsSet = JsonConvert.SerializeObject(gameplayOptions);
            Intent nextActivity = new Intent(this, typeof(TeacherAttendanceMain));
            nextActivity.PutExtra("gameplayOptionsSet", gameplayOptionsSet);
            this.StartActivity(nextActivity);
       
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

        public Button btnRace => FindViewById<Button>(Resource.Id.btnRace);
        public Button btnLeader => FindViewById<Button>(Resource.Id.btnLeader);

        public Button btnGauntlet => FindViewById<Button>(Resource.Id.btnGauntlet);

    }
}