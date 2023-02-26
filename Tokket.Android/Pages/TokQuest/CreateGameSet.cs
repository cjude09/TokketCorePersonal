using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Listener;
using Tokket.Android.ViewModels;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Tokket.Android.TokQuest;
using Tokket.Core.Tools;

namespace Tokket.Android.TokQuest
{
    [Activity(Label = "", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    public class CreateGameSet : BaseActivity
    {

        public CreateGameDetailsViewModel GameDetails;

        internal static CreateGameSet Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.GameSetForm);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                Instance = this;
                GameDetails = JsonConvert.DeserializeObject<CreateGameDetailsViewModel>(Intent.GetStringExtra("details"));

                chosenName.Text = $"Study Set Name: {GameDetails.ChosenName}";

              
                BtnSaveGameName.Click += BtnSaveGameName_Click;
                btnCancelCreateGame.Click += BtnCancelCreateGame_Click;
            }
            catch (Exception err)
            {

                Console.WriteLine("++" + err.Message);
            }
         
            // Create your application here
        }

        private void BtnCancelCreateGame_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnSaveGameName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ChosenGameName.Text)) {
                
                //var data = await TokquestService.Instance.GetGamesets(ClassGroupCollection[position].Id);
                Intent nextActivity = new Intent(this, typeof(CreateGameSetClassGroup));
                var modelConvert = JsonConvert.SerializeObject(GameDetails);
                nextActivity.PutExtra("GameDetailsBeforeClassGroup", modelConvert);
                this.StartActivity(nextActivity);

            }

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
            }
            return base.OnOptionsItemSelected(item);
        }


        public TextView chosenName => FindViewById<TextView>(Resource.Id.ChosenStudySetName);

        public Button BtnSaveGameName => FindViewById<Button>(Resource.Id.btnSaveGameName);

        public Button btnCancelCreateGame => FindViewById<Button>(Resource.Id.btnCancelCreateGame);

        public EditText ChosenGameName => FindViewById<EditText>(Resource.Id.ChosenGameName);

    }
}