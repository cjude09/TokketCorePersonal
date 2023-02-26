using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Newtonsoft.Json;
using Tokket.Core.Tools;
using Tokket.Android.Listener;
using Tokket.Shared.Services.ServicesDB;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Android.Helpers;

namespace Tokket.Android.Fragments
{
    public class CategoriesFragment : AndroidX.Fragment.App.Fragment, View.IOnTouchListener
    {

        View v;
        public string filterText { get; set; } = "";

        public bool isSearchedClicked { get; set; } = false;
        string pageToken = "";
        private List<CategoryModel> CategoryList;
        public RecyclerView RecyclerCategoriesContainer => v.FindViewById<RecyclerView>(Resource.Id.RecyclerContainer);
        private ProgressBar progressBar;
        private List<CategoryModel> categoryResultList;
        private System.Threading.CancellationTokenSource CancelToken;

        internal static CategoriesFragment Instance { get; private set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            CategoryList = new List<CategoryModel>();
            categoryResultList = new List<CategoryModel>();
            v = inflater.Inflate(Resource.Layout.container, container, false);

            Instance = this;

            var mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerCategoriesContainer.SetLayoutManager(mLayoutManager);

            progressBar = v.FindViewById<ProgressBar>(Resource.Id.progressbar);
            progressBar.Visibility = ViewStates.Visible;

            //Only Activate search if button is clicked
            ((Activity)Context).RunOnUiThread(async () => await Task.WhenAll(InitializeCategories()));

            Settings.ActivityInt = 0;

            if (RecyclerCategoriesContainer != null)
            {
                RecyclerCategoriesContainer.HasFixedSize = true;

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                onScrollListener.LoadMoreEvent += async (object sender, EventArgs e) =>
                {
                    if(!string.IsNullOrEmpty(pageToken))
                         await InitializeCategories(pageToken);
                };

                RecyclerCategoriesContainer.AddOnScrollListener(onScrollListener);

                RecyclerCategoriesContainer.SetLayoutManager(mLayoutManager);
            }

            return v;
        }

        private async Task InitializeCategories(string pagination = "")
        {
            CancelToken = new System.Threading.CancellationTokenSource();
           var resultCategories = await CommonService.Instance.SearchCategoriesAsync(filterText.ToLower(),pagination);
           categoryResultList = resultCategories.Results.ToList();
            var filter = resultCategories;
            pageToken = filter.ContinuationToken;
           //var filter = await ClassService.Instance.GetMoreFilterOptions(new ClassTokQueryValues()
           // {
           //     text = filterText.ToLower(),
           //     FilterBy = FilterBy.Category,
           //     RecentOnly = true,
           //     paginationid = pagination
           // }) ;

            var result = new List<CategoryModel>();

            if (filter.Results == null)
            {
                TextNothingFound.Text = "No categories found.";
                TextNothingFound.Visibility = ViewStates.Visible;
                return;
            }

            if (filter.Results.Count() == 0)//if (filter.Count == 0)
            {
                TextNothingFound.Text = "No categories found.";
                TextNothingFound.Visibility = ViewStates.Visible;
            }
            else
            {
                TextNothingFound.Visibility = ViewStates.Gone;
                #region Filter ver1
                //if (string.IsNullOrEmpty(filterText))
                //{
                //    foreach (var model in filter.Results)
                //    {
                //        var data = JsonConvert.DeserializeObject<CategoryModel>(model.JsonData);
                //        result.Add(data);
                //    }
                //}
                //else
                //{
                //    foreach (var model in filter.Results)
                //    {

                //        var data = JsonConvert.DeserializeObject<CategoryModel>(model.JsonData);
                //        if (data.Name.Contains(filterText))
                //        {
                //            result.Add(data);
                //        }

                //    }
                //}
                //categoryResultList.AddRange(result);
                #endregion

                if (!string.IsNullOrEmpty(filterText))
                {
                    var filtering = resultCategories.Results.Where(t => t.Name.Contains(filterText));
                    resultCategories.Results = filtering;
                    result = resultCategories.Results.ToList();
                }
                else
                    result = filter.Results.ToList();
            }

            var tasks = new List<Task>();
            for (int i = 0; i < result.Count(); i++)
            {
                result[i].filterCategory = result[i].Id;
                var filterItems = new List<string>();
                filterItems.Add(result[i].filterCategory);

                result[i].categoryList = filterItems;

                tasks.Add(GetTokCounts(i));
            }

            await Task.WhenAll(tasks);

            var newCategoryList = new List<CategoryModel>();
            foreach (var item in result)
            {
                //if (item.toks_count > 0)
                //{
                //    newCategoryList.Add(item);
                //}
                newCategoryList.Add(item);
            }

            CategoryList.AddRange(newCategoryList);
       
            CategoryList.OrderBy(x => x.filterCategory).ToList();
            //CategoryList.AddRange(result);
            SetCategoriesAdapter();
            progressBar.Visibility = ViewStates.Gone;
        }

        public void CallCancelSearchCategory() {
            if (CancelToken != null) {
                CancelToken.Cancel();
            }
        }

        private async Task GetTokCounts(int position)
        {
            var queryValues = new ClassTokQueryValues()
            {
                partitionkeybase = $"{Settings.GetUserModel().UserId}-classtoks",
                groupid = "",
                userid = "",
                text = string.Empty,
                startswith = false,
                publicfeed = true,
                FilterBy = FilterBy.Category,
                FilterItems = categoryResultList[position].categoryList,
                classtokmode = true
            };
            int totalcount = 0;
            string CT = string.Empty;
            //ResultData<ClassTokModel> tokResult = await ClassService.Instance.GetClassToksAsync(queryValues);
            //while (tokResult != null && !string.IsNullOrEmpty(tokResult.ContinuationToken)) {
            //    //queryValues.paginationid = CT;
            //    tokResult = await ClassService.Instance.GetClassToksAsync(queryValues);
            //    CT = tokResult.ContinuationToken;
            //    totalcount += tokResult.Results.Count();
            //}
            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();


            //tokResult = await ClassService.Instance.GetClassToksAsync(queryValues, cancellationToken, fromCaller: TAG);

            try
            {
                //ClasstokserviceDB GetClasstoks function
                //Add try catch due to error 400 crash
                GetClassToksRequest request = new GetClassToksRequest()
                {
                    QueryValues = queryValues
                };
                var getclasstoksDB = await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.Interfaces.IClassTokService>().GetClassToksAsync<ClassTokModel>(request);
                tokResult.ContinuationToken = getclasstoksDB.ContinuationToken;
                var resultString = JsonConvert.SerializeObject(getclasstoksDB.Results);
                tokResult.Results = JsonConvert.DeserializeObject<List<ClassTokModel>>(resultString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            if (tokResult?.Results != null)
            {
                categoryResultList[position].toks_count = tokResult.Results.Count();
            }
        }

        private void SetCategoriesAdapter()
        {
            var adapterCategories = CategoryList.GetRecyclerAdapter(BindCategoriesViewHolder, Resource.Layout.categoryrow);
            RecyclerCategoriesContainer.SetAdapter(adapterCategories);
        }

        private void BindCategoriesViewHolder(CachingViewHolder holder, CategoryModel model, int position)
        {
            var Category = holder.FindCachedViewById<TextView>(Resource.Id.TextCategory);
            var TokCounter = holder.FindCachedViewById<TextView>(Resource.Id.TokCounter);

            Category.Text = model.Name;
            TokCounter.Text = model.toks_count + " toks";

            Category.ContentDescription = position.ToString();
            Category.SetOnTouchListener(this);

        }

        public bool OnTouch(View v, MotionEvent e)
        {
            int ndx = int.Parse((string)v.ContentDescription);
            if (e.Action == MotionEventActions.Up)
            {
                bool gotonextpage = true;

                string titlepage = "";
                string filter = "";
                string headerpage = (v as TextView).Text;

                /*if (Settings.FilterTag == 3)
                {
                    gotonextpage = false;
                }*/

                if (gotonextpage)
                {
                    Settings.FilterTag = 3;
                    titlepage = "Category";
                    filter = CategoryList[ndx].Name;

                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ClassToksActivity));
                    nextActivity.PutExtra("titlepage", titlepage);
                    nextActivity.PutExtra("filter", filter);
                    nextActivity.PutExtra("headerpage", headerpage);
                    nextActivity.PutExtra("filterBy", (int)FilterBy.Category);
                    nextActivity.PutExtra("filterItems", JsonConvert.SerializeObject(CategoryList[ndx].categoryList));
                    //nextActivity.PutExtra("classtokModel");
                    nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    MainActivity.Instance.StartActivity(nextActivity);
                }
            }
            return true;
        }
        public TextView TextNothingFound => v.FindViewById<TextView>(Resource.Id.TextNothingFound);
    }
}