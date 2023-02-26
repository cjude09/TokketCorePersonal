using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Tokket.Core;
using Xamarin.Essentials;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Game", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewGameSetActivity : BaseActivity
    {
        GameScheme gameScheme;
        ObservableRecyclerAdapter<GameSet, CachingViewHolder> adapterGameSet;
        ObservableCollection<GameSet> gameSetsCollection { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here                     
            SetContentView(Resource.Layout.activity_viewgameset);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            this.Title = Intent.GetStringExtra("buttonTapped");
            //gameScheme = (GameScheme)Enum.Parse(typeof(GameScheme), Intent.GetStringExtra("gameScheme"));
            gameScheme = GameScheme.TokBlitz;

            gameSetsCollection = new ObservableCollection<GameSet>();
            RunOnUiThread(async () => await LoadGameSets());
        }
      

        private async Task LoadGameSets()
        {
            var ctoken = await SecureStorage.GetAsync("idtoken");
            var qry = new SetQueryValues() { text = "", userid = "", token = ctoken, loadmore = !string.IsNullOrEmpty(ctoken) ? "yes" : "", offset = 12 };

            switch (gameScheme)
            {
                case GameScheme.TokBlast:
                    qry.gamename = "tokblast";
                    break;
                case GameScheme.TokBoom:
                    qry.gamename = "tokboom";
                    break;
                case GameScheme.AlphaGuess:
                    qry.gamename = "alphaguess";
                    break;
                case GameScheme.TokBlitz:
                default:
                    qry.gamename = "tokblitz";
                    break;
            }

            var result = await SetService.Instance.GetGameSetsAsync(qry);
            foreach (var gameset in result.Results)
            {
                gameSetsCollection.Add(gameset);
            }
            SetGameAdapter();
        }

        private void SetGameAdapter()
        {
            adapterGameSet = gameSetsCollection.GetRecyclerAdapter(BindGameSetsViewHolder, Resource.Layout.mytoksets_row);
            RecyclerList.SetAdapter(adapterGameSet);
        }

        private void BindGameSetsViewHolder(CachingViewHolder holder, GameSet gameSet, int position)
        {
            var txtSetsTokUpper = holder.FindCachedViewById<TextView>(Resource.Id.txtSetsTokUpper);
            var txtClassDescription = holder.FindCachedViewById<TextView>(Resource.Id.txtClassDescription);
            //var txtSetsTokBottom = holder.FindCachedViewById<TextView>(Resource.Id.txtSetsTokBottom);
            //var lblMySetPopUp = holder.FindCachedViewById<TextView>(Resource.Id.lblMySetPopUp);
            //var linearMySetsColor = holder.FindCachedViewById<LinearLayout>(Resource.Id.linearMySetsColor);
            //var ImgMySetsRow = holder.FindCachedViewById<ImageView>(Resource.Id.ImgMySetsRow);

            txtSetsTokUpper.Text = gameSet.Name;
            txtClassDescription.Text = gameSet.Description;
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

        public RecyclerView RecyclerList => FindViewById<RecyclerView>(Resource.Id.RecyclerContainer);
    }
}