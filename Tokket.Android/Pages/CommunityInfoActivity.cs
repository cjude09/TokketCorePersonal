using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net.Wifi.Aware;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Lifecycle.ViewModels;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;

namespace Tokket.Android.Pages
{
    [Activity(Label = "TokChannel", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CommunityInfoActivity : BaseActivity
    {
        public ClassTokDataAdapter ClassTokDataAdapter;
        public List<ClassTokModel> ClassTokCollection;
        List<Tokmoji> ListTokmojiModel;
        string callername = "CommunityInfoActivity";
        private ClassGroup ModelClassGroup { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                this.SetTheme(Resource.Style.AppThemeDark);
            }
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_community_info);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ListTokmojiModel = new List<Tokmoji>();
            ClassTokCollection = new List<ClassTokModel>();

            ModelClassGroup = JsonConvert.DeserializeObject<ClassGroup>(Intent.GetStringExtra("classgroup"));

            if (ModelClassGroup != null)
            {
                this.Title = ModelClassGroup.Name;
                TxtGroupHeader.Text = ModelClassGroup.Description;

                RunOnUiThread(async () => await InitializeData());
            }

        }

        public async Task InitializeData()
        {
            var resultToks = await GetClassToksData();
            ClassTokCollection.AddRange(resultToks);
            SetClassTokRecyclerAdapter();
        }

        private async Task<List<ClassTokModel>> GetClassToksData()
        {   
            var taskCompletionSource = new TaskCompletionSource<List<ClassTokModel>>();
            List<ClassTokModel> classTokModelsResult = new List<ClassTokModel>();
            bool isPublicFeed = false;
            if (Settings.FilterFeed == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            var ownerId = Settings.GetUserModel().UserId;

            var tokResult = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues()
            {
                partitionkeybase = isPublicFeed ? "classtoks" : (!string.IsNullOrEmpty(ModelClassGroup.Id) ? $"{ModelClassGroup.Id}-classtoks" : $"{ownerId}-classtoks"),
                groupid = ModelClassGroup.Id,
                userid = isPublicFeed || !string.IsNullOrEmpty(ModelClassGroup.Id) ? "" : ownerId,
                text = "",
                startswith = false,
                publicfeed = isPublicFeed,
                FilterBy = FilterBy.None,
                FilterItems = new List<string>(),
                paginationid = "",
                level1 = "",
                level2 = "",
                level3 = "",
                descending = Settings.SortByTitleAscending,
                orderby = Settings.SortByReferenceAscending,
                classtokmode = true
            });


            if (tokResult == null)
            {
                MainActivity.Instance.ShowLottieMessageDialog(this, "Failed to load toks.", false);
            }
            else
            {
                if (tokResult.ContinuationToken == "cancelled")
                {
                    MainActivity.Instance.ShowLottieMessageDialog(this, "Task was cancelled.", false);
                }
                else
                {
                    //Cache the result so that it will load automatically once app is opened
                    ClassService.Instance.SetCacheClassToksAsync(callername, tokResult.Results.ToList());

                    classTokModelsResult = tokResult.Results.ToList();
                }
            }

            var Itemwithvalue = new List<ClassTokModel>();
            foreach (var item in classTokModelsResult)
            {
                var reactionValue = await ReactionService.Instance.GetReactionsValueAsync(item.Id);
                if (reactionValue != null)
                {
                    item.ViewsModel = reactionValue.ViewsModel;
                    Itemwithvalue.Add(item);
                }
            }

            return Itemwithvalue;
        }

        private void SetClassTokRecyclerAdapter()
        {
            ClassTokDataAdapter = new ClassTokDataAdapter(this, ClassTokCollection, ListTokmojiModel);

            ClassTokDataAdapter.ItemClick -= OnGridBackgroundClick;
            ClassTokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToks.SetAdapter(ClassTokDataAdapter);
        }

        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;

            Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));

            var classtokModel = ClassTokCollection[position];
            if (ClassTokCollection[position].ViewsModel != null)
            {
                if (Settings.GetTokketUser().Id == ClassTokCollection[position].UserId)
                {
                    ClassTokCollection[position].ViewsModel.TileTapViewsPersonal += 1;
                }
                else
                {
                    ClassTokCollection[position].ViewsModel.TileTapViews += 1;
                }
            }


            var modelConvert = JsonConvert.SerializeObject(classtokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);

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
        public AndroidX.AppCompat.Widget.Toolbar HeaderToolBar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.includeHeaderLayout);
        public TextView TxtGroupHeader => FindViewById<TextView>(Resource.Id.txtGroupHeader);
        public RecyclerView RecyclerToks => FindViewById<RecyclerView>(Resource.Id.recyclerToks);
    }
}
       