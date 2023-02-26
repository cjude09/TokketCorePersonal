using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Supercharge;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Tokket.Android.ViewHolders;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;

namespace Tokket.Android.Fragments
{
    public class classgroup_pics_fragmentt : AndroidX.Fragment.App.Fragment
    {
        ClassGroupModel classGroup;
        View v;
        private ObservableRecyclerAdapter<TokModel, CachingViewHolder> adapterTokModel;
        internal static classgroup_pics_fragmentt Instance { get; private set; }
        ObservableCollection<TokModel> TokModelCollection { get; set; }
        public classgroup_pics_fragmentt(ClassGroupModel _classGroup)
        {
            classGroup = _classGroup;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.fragment_classgroup_pics, container, false);

            Instance = this;

            TokModelCollection = new ObservableCollection<TokModel>();
            var mLayoutManager = new GridLayoutManager(Context, 3);
            recyclerViewPics.SetLayoutManager(mLayoutManager);

            MainActivity.Instance.RunOnUiThread(async () => await InitializeData());
            return v;
        }
        public async Task InitializeData(string paginationtoken = "")
        {
            shimmerPics.StartShimmerAnimation();
            shimmerPics.Visibility = ViewStates.Visible;

            /*ClassGroupModel classGroup = await ClassService.Instance.GetClassGroupAsync(groupId);*/
            TokQueryValues queryValues = new TokQueryValues();
            queryValues.tokgroup = "Basic";
            queryValues.category = "Image";
            queryValues.token = paginationtoken;
            queryValues.toktype = classGroup.Name;
            queryValues.groupid = classGroup.Id;
            var result = await TokService.Instance.GetToksAsync(queryValues, "tokpics");
            recyclerViewPics.ContentDescription = Settings.ContinuationToken;

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.Image))
                {
                    TokModelCollection.Add(item);
                }
            }

            setRecyclerAdapter();

            shimmerPics.Visibility = ViewStates.Gone;
        }
        private void setRecyclerAdapter()
        {
            adapterTokModel = TokModelCollection.GetRecyclerAdapter(BindTokModelCollection, Resource.Layout.tokpic_row);
            recyclerViewPics.SetAdapter(adapterTokModel);

            TextNothingFound.Visibility = ViewStates.Gone;
            if (TokModelCollection.Count == 0)
            {
                TextNothingFound.Text = "No pics.";
                TextNothingFound.Visibility = ViewStates.Visible;
            }
        }
        public void insertTokAdded(TokModel tokModel)
        {
            var result = TokModelCollection.FirstOrDefault(c => c.Id == tokModel.Id);
            if (result != null) //If Edit
            {
                int ndx = TokModelCollection.IndexOf(result);
                TokModelCollection.Remove(result);

                TokModelCollection.Insert(ndx, tokModel);
            }
            else //if save
            {
                TokModelCollection.Insert(0, tokModel); //Make it in the first
            }

            setRecyclerAdapter();
        }

        public void removeTok(TokModel tok)
        {
            //TokModelCollection.Remove(tok);

            var result = TokModelCollection.FirstOrDefault(c => c.Id == tok.Id);
            if (result != null)
            {
                int ndx = TokModelCollection.IndexOf(result);
                TokModelCollection.Remove(result);
            }

            setRecyclerAdapter();
        }

        private void BindTokModelCollection(CachingViewHolder holder, TokModel model, int position)
        {
            var imgPic = holder.FindCachedViewById<SquareImageView>(Resource.Id.imgPic);

            if (!string.IsNullOrEmpty(model.Image))
            {
                if (model.Image.Contains("data:image/jpeg;base64,"))
                {
                    byte[] imageDetailBytes = Convert.FromBase64String(model.Image.Replace("data:image/jpeg;base64,", ""));
                    imgPic.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }
                else
                {
                    RequestOptions options = new RequestOptions()
                        .CenterCrop()
                        .Placeholder(Resource.Animation.loader_animation)
                        .Error(Resource.Drawable.no_image);

                    Glide.With(Context).Load(model.Image).Thumbnail(0.05f).Apply(options).Into(imgPic);
                }
            }


            imgPic.Click -= delegate { };  //Add to avoid showing multiple popup
            imgPic.Click += delegate
            {
                //Add try catch to avoid a possible crash
                try
                {
                    Bitmap imgTokMojiBitmap = ((BitmapDrawable)imgPic.Drawable).Bitmap;
                    MemoryStream byteArrayOutputStream = new MemoryStream();
                    imgTokMojiBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                    byte[] byteArray = byteArrayOutputStream.ToArray();

                    Intent nextActivity = new Intent(Context, typeof(DialogImageViewerActivity));
                    Settings.byteImageViewer = JsonConvert.SerializeObject(byteArray);
                    nextActivity.PutExtra("tokPicModel", JsonConvert.SerializeObject(model));
                    nextActivity.PutExtra("classGroupModel", JsonConvert.SerializeObject(classGroup));
                    this.StartActivity(nextActivity);
                }
                catch (Exception ex) { }
            };
        }

        public SwipeRefreshLayout swipeRefreshPics => v.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshPics);
        public RecyclerView recyclerViewPics => v.FindViewById<RecyclerView>(Resource.Id.recyclerView_Pics);
        public ShimmerLayout shimmerPics => v.FindViewById<ShimmerLayout>(Resource.Id.shimmer_pics);
        public TextView TextNothingFound => v.FindViewById<TextView>(Resource.Id.TextNothingFound);
    }
}