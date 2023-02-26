using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Tokket.Shared.Models;
using Tokket.Shared.ViewModels;
using Tokket.Android.ViewHolders;

namespace Tokket.Android.Adapters
{
    public class GameSetDataAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        Activity context;
        //List<GameSetModel> ListGames;        
        List<GameSetViewModel> ListGames;
        View itemView;
        public GameSetDataAdapter(Activity context, List<GameSetViewModel> listGames)
        {
            this.context = context;
            this.ListGames = listGames;
        }

        public override int ItemCount => ListGames.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        GameSetViewHolder vh;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh.ImgGame.SetImageDrawable(ContextCompat.GetDrawable(context, ListGames[position].GameSet.IdImageGame));
            vh.TextTitle.Text = ListGames[position].GameSet.GameTitle;

            vh.BtnAddGameSet.Click -= AddGameSet_Click;
            vh.BtnAddGameSet.Click += AddGameSet_Click;

            vh.LinearGame.Tag = position;
            vh.LinearGame.Click -= UnderConstruction_Click;
            vh.LinearGame.Click += UnderConstruction_Click;

            vh.BtnViewGameSet.Tag = position;
            vh.BtnViewGameSet.Click -= BtnViewGameSet_Click;
            vh.BtnViewGameSet.Click += BtnViewGameSet_Click;
            vh.BtnAddGameSet.ContentDescription = position.ToString();
        }
        private void AddGameSet_Click(Object sender, EventArgs e)
        {
            int position = Convert.ToInt32((sender as Button).ContentDescription);
            Intent nextActivity = new Intent(context, typeof(AddGameSetActivity));
            nextActivity.PutExtra("buttonTapped", $"Add Game Set ({ListGames[position].GameSet.GameTitle})");
            nextActivity.PutExtra("gamename", ListGames[position].GameSet.GameTitle);
            //nextActivity.PutExtra("gamedescription", ListGames[position].GameSet.GameDescription);
            context.StartActivity(nextActivity);
        }

        private void BtnViewGameSet_Click(Object sender, EventArgs e)
        {
            int position = Convert.ToInt32((sender as Button).Tag);
            Intent nextActivity = new Intent(MySetsActivity.Instance, typeof(ViewGameSetActivity));
            nextActivity.PutExtra("buttonTapped", $"Game Sets ({ListGames[position].GameSet.GameTitle})");
            nextActivity.PutExtra("gameScheme", (int)ListGames[position].GameScheme);
            MySetsActivity.Instance.StartActivity(nextActivity);
        }

        private void UnderConstruction_Click(Object sender, EventArgs e)
        {
            int position = Convert.ToInt32((sender as LinearLayout).Tag);
            if (ListGames[position].GameScheme == Shared.Helpers.GameScheme.TokBlast || ListGames[position].GameScheme == Shared.Helpers.GameScheme.AlphaGuess)
            {
                var dialogDelete = new AlertDialog.Builder(MySetsActivity.Instance);
                var alertDelete = dialogDelete.Create();
                alertDelete.SetTitle("");
                alertDelete.SetIcon(Resource.Drawable.alert_icon_blue);
                alertDelete.SetMessage("Under Construction");
                alertDelete.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) =>
                {

                });
                alertDelete.Show();
                alertDelete.SetCanceledOnTouchOutside(false);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.gameset_row, parent, false);

            vh = new GameSetViewHolder(itemView, OnClick);
            return vh;
        }
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}