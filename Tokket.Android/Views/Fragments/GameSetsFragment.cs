using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Adapters;
using Tokket.Shared.Models;
using Tokket.Shared.ViewModels;

namespace Tokket.Android.Fragments
{
    public class GameSetsFragment : AndroidX.Fragment.App.Fragment
    {
        View v;
        GridLayoutManager mLayoutManager;
        //List<GameSetModel> ListGames; 
        public GameSetDataAdapter GameDataAdapter;
        List<GameSetViewModel> ListGameSetViewModel;
        internal static GameSetsFragment Instance { get; private set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.gameset_fragment_page, container, false);
            Instance = this;

            mLayoutManager = new GridLayoutManager(MySetsActivity.Instance, 2);
            RecyclerGameSets.SetLayoutManager(mLayoutManager);

            LoadGames();

            return v;
        }
        private void LoadGames()
        {
            //ListGames = new List<GameSetModel>();
            //ListGames.Add(new GameSetModel() { IdImageGame = Resource.Drawable.tokblitz, GameTitle = "Tok Blitz", GameDescription = GetString(Resource.String.tokblitz_description) });
            //ListGames.Add(new GameSetModel() { IdImageGame = Resource.Drawable.tokblast, GameTitle = "Tok Blast", GameDescription = GetString(Resource.String.tokblast_description) });
            //ListGames.Add(new GameSetModel() { IdImageGame = Resource.Drawable.alpha_guess, GameTitle = "Alpha Guess", GameDescription = GetString(Resource.String.alphaguess_description) });

            ListGameSetViewModel = new List<GameSetViewModel>();
            ListGameSetViewModel.Add(new GameSetViewModel()
            {
               GameScheme = Shared.Helpers.GameScheme.TokBlitz,
               GameSet = new GameSetModel() { IdImageGame = Resource.Drawable.tokblitz, GameTitle = "Tok Blitz", GameDescription = GetString(Resource.String.tokblitz_description) } 
            });
            ListGameSetViewModel.Add(new GameSetViewModel()
            {
                GameScheme = Shared.Helpers.GameScheme.TokBlast,
                GameSet = new GameSetModel() { IdImageGame = Resource.Drawable.tokblast, GameTitle = "Tok Blast", GameDescription = GetString(Resource.String.tokblast_description) }
            });
            ListGameSetViewModel.Add(new GameSetViewModel()
            {
                GameScheme = Shared.Helpers.GameScheme.AlphaGuess,
                GameSet = new GameSetModel() { IdImageGame = Resource.Drawable.alpha_guess, GameTitle = "Alpha Guess", GameDescription = GetString(Resource.String.alphaguess_description) }
            });

            GameDataAdapter = new GameSetDataAdapter(MySetsActivity.Instance, ListGameSetViewModel);
            RecyclerGameSets.SetAdapter(GameDataAdapter);
        }
        public RecyclerView RecyclerGameSets => v.FindViewById<RecyclerView>(Resource.Id.recyclerGameSets);
        public LinearLayout LinearProgress => v.FindViewById<LinearLayout>(Resource.Id.LinearProgress);
    }
}