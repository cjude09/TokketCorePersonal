using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Shared.Models;
using Tokket.Shared.Services;

namespace Tokket.Android.Fragments
{
    public class ClassGroupDocsFragment : AndroidX.Fragment.App.Fragment
    {
        ClassGroupModel classGroup;
        View v;
        internal static ClassGroupDocsFragment Instance;

        public ClassGroupDocsFragment(ClassGroupModel _classGroup)
        {
            Instance = this;
            classGroup = _classGroup;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.fragment_classgroup_docs, container, false);

            var mLayoutManager = new GridLayoutManager(Application.Context, 1);
            RecyclerMainList.SetLayoutManager(mLayoutManager);
            SwipeRefreshMain.Refresh += (obj, _event) => {
                Init();
              
            };
            Init();
            return v;
        }

        public async void Init()
        {
            var list = new List<TokModel>();
            var getdocs = await ClassService.Instance.GetClassGroupDocs(classGroup);

            if (getdocs != null)
            {
                foreach (var docs in getdocs)
                {
                    list.Add(docs);
                }

                var docsAdapter = new ClassGroupDocsAdapter(this.Context, list, classGroup);
                docsAdapter.ItemClick += docsAdapter.OnItemRowClick;
                RecyclerMainList.SetAdapter(docsAdapter);
                SwipeRefreshMain.Refreshing = false;
            }
        }
        public RecyclerView RecyclerMainList => v.FindViewById<RecyclerView>(Resource.Id.recyclerView_class_group_docs);
        public SwipeRefreshLayout SwipeRefreshMain => v.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh_classgroup_docs);
    }
}