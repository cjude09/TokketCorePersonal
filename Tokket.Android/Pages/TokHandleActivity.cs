using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Android.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Newtonsoft.Json;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;

namespace Tokket.Android
{
    [Activity(Label = "Tok Handle", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TokHandleActivity : BaseActivity
    {
        TokketUser user;
        ObservableCollection<TokHandle> TokHandleCollection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_tok_handle);

            user = Settings.GetTokketUser();
            TokHandleCollection = new ObservableCollection<TokHandle>();
            var mLayoutManager = new GridLayoutManager(this, 1);
            RecyclerTokHandles.SetLayoutManager(mLayoutManager);

            btnBack.Click += delegate
            {
                this.Finish();
            };

            btnSettings.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(NameplateSettingsUIActivity));
                var item = TokHandleCollection.FirstOrDefault(a => a.Id == user.CurrentHandle);
                if (item != null)
                {
                    nextActivity.PutExtra("CurrentTokHandle", JsonConvert.SerializeObject(item));
                }
                this.StartActivity(nextActivity);
            };

            RunOnUiThread(async () => await LoadTokHandles());
        }

        private async Task LoadTokHandles()
        {
            txtCurrentTokHandle.Text = "Current Tok Handle: " + user.CurrentHandle ?? "";

            showBlueLoading(this);
            var result = await TokHandleService.Instance.GetTokHandlesByUserAsync(user.Id);
            hideBlueLoading(this);

            foreach (var item in result.Results)
            {
                TokHandleCollection.Add(item);
            }

            var adapterTokhandles = TokHandleCollection.GetRecyclerAdapter(BindTokHandleViewHolder, Resource.Layout.row_tok_handle);
            RecyclerTokHandles.SetAdapter(adapterTokhandles);
        }

        private void BindTokHandleViewHolder(CachingViewHolder holder, TokHandle tokhandle, int position)
        {
            var txtTokHandleText = holder.FindCachedViewById<TextView>(Resource.Id.txtTokHandleText);
            txtTokHandleText.Text = tokhandle.Id;
        }

        public Button btnBack => FindViewById<Button>(Resource.Id.btnBack);
        public ImageButton btnSettings => FindViewById<ImageButton>(Resource.Id.btnSettings);
        public RecyclerView RecyclerTokHandles => FindViewById<RecyclerView>(Resource.Id.recyclerTokHandles);
        public TextView txtCurrentTokHandle => FindViewById<TextView>(Resource.Id.txtCurrentTokHandle);
    }
}