using Android.App;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.ObjectModel;

namespace Tokket.Android.TokQuest
{
    [Activity(Label = "WaitingLobbyActivity")]
    public class WaitingLobbyActivity : BaseActivity
    {
        ObservableCollection<WaitingPlayer> WaitingPlayerCollection = new ObservableCollection<WaitingPlayer>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = "Waiting...";
            SetContentView(Resource.Layout.tokquest_waitingroom_page);
            // Create your application here
        }
        RecyclerView WaitingPlayerRecycler => FindViewById<RecyclerView>(Resource.Id.recycler_waiting);
    }

    class WaitingPlayer
    {
        string ImageUrl { get; set; }

        string DisplayName { get; set; }
    }
}