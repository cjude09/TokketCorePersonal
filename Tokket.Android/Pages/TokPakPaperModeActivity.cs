using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tokket.Android.Adapters;
using Tokket.Android.Helpers;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Models;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Paper", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullSensor)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Paper", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullSensor)]
#endif
    public class TokPakPaperModeActivity : BaseActivity
    {
        ObservableCollection<ClassTokModel> ClassTokCollection { get; set; }
        internal static TokPakPaperModeActivity Instance { get; private set; }

        private ObservableCollection<DetailImageItem> DetailImageCollections { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tok_pak_paper_mode);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;

            var title = Intent.GetStringExtra("TitleActivity");
            Title = title;

            recyclerPages.SetLayoutManager(new GridLayoutManager(this, 1));
            ClassTokCollection = new ObservableCollection<ClassTokModel>();

            var classtokModelString = Intent.GetStringExtra("classtokModel");
            ClassTokCollection = JsonConvert.DeserializeObject<ObservableCollection<ClassTokModel>>(classtokModelString);
          
            setRecyclerPagesAdapter();
        }

        private void setRecyclerPagesAdapter()
        {
            var adapterDetail = ClassTokCollection.GetRecyclerAdapter(BindClassTokViewHolder, Resource.Layout.activity_tok_pak_paper_row);
            recyclerPages.SetAdapter(adapterDetail);
        }

        private void BindClassTokViewHolder(CachingViewHolder holder, ClassTokModel model, int position)
        {
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = holder.FindCachedViewById<TextView>(Resource.Id.txtItem);
            var recycler = holder.FindCachedViewById<RecyclerView>(Resource.Id.RecyclerItem);
            txtHeader.Text = model.PrimaryFieldText;

            recycler.Visibility = ViewStates.Visible;
            txtItem.Visibility = ViewStates.Gone;
            DetailImageCollections = new ObservableCollection<DetailImageItem>();
            if (model.IsDetailBased)
            {
                recycler.Visibility = ViewStates.Visible;
                txtItem.Visibility = ViewStates.Gone;
                for (int i = 0; i < model.Details.Count(); i++)
                {
                    string image = string.Empty;
                    if (!string.IsNullOrEmpty(model.Details[i]))
                    {
                        if(model.ImagesIsTokPakVisible == null)
                            model.ImagesIsTokPakVisible = new List<bool> { true, true, true, true, true, true, true, true, true, true };

                        if (model.ImagesIsTokPakVisible[i + 1])
                            image = model.DetailImages?[i] == null ? string.Empty : model.DetailImages[i];

                        var detailImage = new DetailImageItem()
                        {
                            Text = model.Details[i],
                            ImageUri = image
                        };
                        DetailImageCollections.Add(detailImage);
                    }

                    //if (!string.IsNullOrEmpty(addPageModelList[position].Details[i]))
                    //{
                    //    if (i == 0)
                    //    {
                    //        txtItem.Text = "• " + addPageModelList[position].Details[i];
                    //    }
                    //    else
                    //    {
                    //        txtItem.Text += "\n• " + addPageModelList[position].Details[i];
                    //    }
                    //}

                }

                setItemDetailAdapter(recycler);
            }
            else
            {
                txtItem.Text = model.SecondaryFieldText;
            }
        }

        private void setItemDetailAdapter(RecyclerView recyclerView)
        {
            var adapterDetail = DetailImageCollections.GetRecyclerAdapter(BindDetailImageCollections, Resource.Layout.tok_pak_presentation_item_row);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 1));
            recyclerView.SetAdapter(adapterDetail);
        }

        private void BindDetailImageCollections(RecyclerView.ViewHolder view, DetailImageItem item, int position)
        {
            var txtHeader = view.ItemView.FindViewById<TextView>(Resource.Id.txtItemName);
            var imageDetail = view.ItemView.FindViewById<ImageView>(Resource.Id.detailItemImage);
            var visibilityButton = view.ItemView.FindViewById<Button>(Resource.Id.btn_imagePresent);

            txtHeader.Text = $"• {item.Text}";
            if (!string.IsNullOrEmpty(item.ImageUri))
            {
                Glide.With(this).Load(item.ImageUri).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(imageDetail);
                visibilityButton.Click += (obj, _event) =>
                {
                    if (visibilityButton.Text.ToLower() == "show")
                    {
                        imageDetail.Visibility = ViewStates.Visible;
                        visibilityButton.Text = "Hide";
                        visibilityButton.SetCompoundDrawablesRelativeWithIntrinsicBounds(Resource.Drawable.caret_up, 0, 0, 0);
                    }
                    else
                    {
                        imageDetail.Visibility = ViewStates.Gone;
                        visibilityButton.Text = "Show";
                        visibilityButton.SetCompoundDrawablesRelativeWithIntrinsicBounds(Resource.Drawable.caret_down, 0, 0, 0);
                    }
                };
            }
            else
            {
                visibilityButton.Visibility = ViewStates.Gone;
                imageDetail.Visibility = ViewStates.Gone;
            }

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

        public RecyclerView recyclerPages => FindViewById<RecyclerView>(Resource.Id.recyclerPages);
    }
}