using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Android.Helpers;
using Tokket.Android.ViewModels;
using Tokket.Shared.Models;

namespace Tokket.Android.Adapters
{
    /*class TokPakPresentationPagerAdapter : LoopingPagerAdapter
    {

        Context context;
        List<ClassTokModel> addPageModelList;
        private ObservableCollection<DetailImageItem> DetailImageCollections { get; set; }
        bool isInfinite;
        public Activity Parent { get; set; }
        public TokPakPresentationPagerAdapter(Context context, List<ClassTokModel> addPageModelList, bool _isInfinite) : base(context, addPageModelList, _isInfinite)
        {
            Init(context, addPageModelList, _isInfinite);
        }

        private void Init(Context ctx, List<ClassTokModel> addPageModelList, bool _isInfinite)
        {
            this.context = ctx;
            this.addPageModelList = addPageModelList;
            this.isInfinite = _isInfinite;
        }

        protected override void BindView(View convertView, int position, int viewType)
        {
            var linearBackground = convertView.FindViewById<LinearLayout>(Resource.Id.linearBackground);
            var txtHeader = convertView.FindViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = convertView.FindViewById<TextView>(Resource.Id.txtItem);
            var recycler = convertView.FindViewById<RecyclerView>(Resource.Id.RecyclerItem);
            var mainImage = convertView.FindViewById<ImageView>(Resource.Id.mainItemImage);
            linearBackground.Background = null;
            DetailImageCollections = new ObservableCollection<DetailImageItem>();
            txtHeader.Text = addPageModelList[position].PrimaryFieldText;
            if (!string.IsNullOrEmpty(addPageModelList[position].Image))
            {
                Glide.With(Parent).Load(addPageModelList[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(mainImage);
            }
            else
                mainImage.Visibility = ViewStates.Gone;

            if (addPageModelList[position].IsDetailBased)
            {
                recycler.Visibility = ViewStates.Visible;
                txtItem.Visibility = ViewStates.Gone;
                for (int i = 0; i < addPageModelList[position].Details.Count(); i++)
                {
                    string image = string.Empty;
                    if (!string.IsNullOrEmpty(addPageModelList[position].Details[i])) {
                        if(addPageModelList[position].ImagesIsTokPakVisible[i+1])
                           image = addPageModelList[position].DetailImages?[i] == null ? string.Empty: addPageModelList[position].DetailImages[i];
                      
                        var detailImage = new DetailImageItem()
                        {
                            Text = addPageModelList[position].Details[i],
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
                txtItem.Text = addPageModelList[position].SecondaryFieldText;
            }
        }


        private void setItemDetailAdapter(RecyclerView recyclerView) {
            var adapterDetail = DetailImageCollections.GetRecyclerAdapter(BindDetailImageCollections, Resource.Layout.tok_pak_presentation_item_row);
            recyclerView.SetLayoutManager(new GridLayoutManager(this.Context, 1));
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
                Glide.With(Parent).Load(item.ImageUri).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(imageDetail);
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
            else {
                visibilityButton.Visibility = ViewStates.Gone;
                imageDetail.Visibility = ViewStates.Gone;
            }
          
        }

        protected override View InflateView(int viewType, ViewGroup container, int listPosition)
        {
            return LayoutInflater.From(context).Inflate(Resource.Layout.tok_pak_presentation_row, container, false);
        }
    }*/

    class DetailImageItem {
       public string Text { get; set; }

        public string ImageUri { get; set; }
    }
    class TokPakPresentationPagerAdapter : PagerAdapter
    {
        private ObservableCollection<DetailImageItem> DetailImageCollections { get; set; }
        Context context;
        List<ClassTokModel> addPageModelList;

        public TokPakPresentationPagerAdapter(Context context, List<ClassTokModel> addPageModelList)
        {
            this.context = context;
            this.addPageModelList = addPageModelList;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            var view = LayoutInflater.From(context).Inflate(Resource.Layout.activity_tok_pak_preview_row, null);
            var linearBackground = view.FindViewById<LinearLayout>(Resource.Id.linearBackground);
            var txtHeader = view.FindViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = view.FindViewById<TextView>(Resource.Id.txtItem);
            var recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerItem);
            var mainImage = view.FindViewById<ImageView>(Resource.Id.mainItemImage);
            linearBackground.Background = null;
            DetailImageCollections = new ObservableCollection<DetailImageItem>();
            txtHeader.Text = addPageModelList[position].PrimaryFieldText;
            if (!string.IsNullOrEmpty(addPageModelList[position].Image))
            {
                Glide.With(context).Load(addPageModelList[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(mainImage);
            }
            else
                mainImage.Visibility = ViewStates.Gone;

            if (addPageModelList[position].IsDetailBased)
            {
                recycler.Visibility = ViewStates.Visible;
                txtItem.Visibility = ViewStates.Gone;
                for (int i = 0; i < addPageModelList[position].Details.Count(); i++)
                {
                    string image = string.Empty;
                    if (!string.IsNullOrEmpty(addPageModelList[position].Details[i]))
                    {
                        if (addPageModelList[position].ImagesIsTokPakVisible[i + 1])
                            image = addPageModelList[position].DetailImages?[i] == null ? string.Empty : addPageModelList[position].DetailImages[i];

                        var detailImage = new DetailImageItem()
                        {
                            Text = addPageModelList[position].Details[i],
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
                txtItem.Text = addPageModelList[position].SecondaryFieldText;
            }

            container.AddView(view);
            return view;
        }
        private void setItemDetailAdapter(RecyclerView recyclerView)
        {
            var adapterDetail = DetailImageCollections.GetRecyclerAdapter(BindDetailImageCollections, Resource.Layout.tok_pak_presentation_item_row);
            recyclerView.SetLayoutManager(new GridLayoutManager(context, 1));
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
                Glide.With(context).Load(item.ImageUri).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).FitCenter()).Into(imageDetail);
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

        //Fill in cound here, currently 0
        public override int Count => addPageModelList.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == ((LinearLayout)@object);
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView(@object as View);
        }
    }
}