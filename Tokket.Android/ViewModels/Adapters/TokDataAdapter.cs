using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Extensions;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Tokket.Android.Helpers;
using Android.Text;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;

namespace Tokket.Android.Adapters
{
    public class TokDataAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        SpannableStringBuilder ssbName;
        ISpannable spannableResText;
        public List<TokModel> items;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => items.Count;
        View itemView;
        #endregion

        #region Constructor
        public TokDataAdapter(List<TokModel> _items, List<Tokmoji> _listTokMoji) //If caller is Activity or Fragment
        {
            items = _items;
            SpannableHelper.ListTokMoji = _listTokMoji;
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<TokModel> listUpdate)
        {
            items.AddRange(listUpdate);
            NotifyDataSetChanged();
        }

        public void AddItem(TokModel item) 
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(TokModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        TokViewHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as TokViewHolder;
            var res = Application.Context.Resources;

            Stream sr = null;
            if (!string.IsNullOrEmpty(items[position].UserCountry))
            {
                try
                {
                    sr = assetManager.Open("Flags/" + items[position].UserCountry + ".jpg");
                }
                catch (Exception ex)
                {
                }
            }

            Bitmap bitmap = BitmapFactory.DecodeStream(sr);

            ssbName = new SpannableStringBuilder(items[position].PrimaryFieldText);
            spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);

            if (!string.IsNullOrEmpty(items[position].ThumbnailImage)) //If tile is image
            {
                string tokimg = "";
                if (items[position].ThumbnailImage.EndsWith(".png") || items[position].ThumbnailImage.EndsWith(".jpg"))
                {
                    tokimg = items[position].ThumbnailImage;
                }
                else
                {
                    tokimg = items[position].ThumbnailImage + ".jpg";
                }

                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        vh.TokUserTitleImg.Text = items[position].SubaccountName; //Settings.GetTokketUser().SubaccountName;
                    }
                    else
                    {
                        if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().TitleId != null)
                        {
                            vh.TokUserTitleImg.Text = items[position].TitleId; //Settings.GetTokketUser().TitleId;
                        }
                    }
                }
                else
                {
                    if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().TitleId != null)
                    {
                        vh.TokUserTitleImg.Text = items[position].TitleId; //Settings.GetTokketUser().TitleId;
                    }
                }

                if (string.IsNullOrEmpty(vh.TokUserTitleImg.Text))
                {
                    vh.TokUserTitleImg.Visibility = ViewStates.Gone;
                }

                Glide.With(itemView).Load(items[position].StickerImage).Into(vh.TileStickerImg);
                if (string.IsNullOrEmpty(items[position].StickerImage))
                {
                    vh.TileStickerImg.Visibility = ViewStates.Gone;


                    //Setting the new height of the RelativeLayout in order to display a fix view so that when the Sticker is gone,  -->
                    //<-- the bottom texts will not go up
                   /* float scale = MainActivity.Instance.Resources.DisplayMetrics.Density;
                    int pixels = (int)(110 * scale + 0.5f);

                    var parentParams = vh.RelativeImgAndBottom.LayoutParameters;
                    parentParams.Height = pixels;

                    vh.RelativeImgAndBottom.LayoutParameters = parentParams;*/
                }

                Glide.With(itemView).Load(tokimg).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(vh.TokImgMain);
                Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.TokImgUserPhoto);
                vh.TokImgUserPhoto.ContentDescription = position.ToString();
                vh.TokImgUserPhoto.SetOnTouchListener(this);
                vh.TokImgUserFlag.SetImageBitmap(bitmap);
                vh.ImgUserDisplayName.Text = items[position].UserDisplayName;
                vh.ImgUserDisplayName.ContentDescription = position.ToString();
                vh.ImgUserDisplayName.Click -= onImageUsernameClick;
                vh.ImgUserDisplayName.Click += onImageUsernameClick;

                vh.TokImgPrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);
                if(items[position].TokGroup == "Quote")
                {
                    vh.TokImgSecondaryFieldText.SetText(items[position].SecondaryFieldText, TextView.BufferType.Spannable);
                    vh.TokImgSecondaryFieldText.Visibility = ViewStates.Visible;
                }
            

                //vh.TokImgPrimaryFieldText.Text = items[position].PrimaryFieldText;
                vh.TokImgCategory.Text = items[position].Category;
                vh.TokImgTokGroup.Text = items[position].TokGroup;
                vh.TokImgTokType.Text = items[position].TokType;

                vh.gridTokImage.SetBackgroundResource(Resource.Drawable.tileview_layout);
                vh.gridTokImage.Tag = position;
                GradientDrawable tokimagedrawable = (GradientDrawable)vh.tokimgdrawable.Background;
                if (Settings.CurrentTheme == (int)ThemeStyle.Light)
                {
                    tokimagedrawable.SetColor(Color.White);
                }
                else
                {

                    tokimagedrawable.SetColor(Color.Black);
                }

                //Purple Gem
                if (items[position].HasGemReaction)
                {
                    vh.ImgPurpleGemTokImg.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.ImgPurpleGemTokImg.Visibility = ViewStates.Gone;
                }

                vh.TokImgMain.ContentDescription = position.ToString();

                vh.TokImgCategory.Tag = 0;
                vh.TokImgCategory.ContentDescription = items[position].CategoryId;
                vh.TokImgCategory.Click -= OnTokButtonClick;
                vh.TokImgCategory.Click += OnTokButtonClick;

                vh.TokImgTokGroup.Tag = 1;
                vh.TokImgTokGroup.ContentDescription = items[position].TokGroup;
                vh.TokImgTokGroup.Click -= OnTokButtonClick;
                vh.TokImgTokGroup.Click += OnTokButtonClick;

                vh.TokImgTokType.Tag = 2;
                vh.TokImgTokType.ContentDescription = items[position].TokTypeId;
                vh.TokImgTokType.Click -= OnTokButtonClick;
                vh.TokImgTokType.Click += OnTokButtonClick;

                vh.gridTokImage.Visibility = ViewStates.Visible;
                vh.gridBackground.Visibility = ViewStates.Gone;
            }
            else
            {
                //Purple Gem
                if (items[position].HasGemReaction)
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Gone;
                }

                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        vh.TokUserTitle.Text = items[position].SubaccountName; //Settings.GetTokketUser().SubaccountName;
                    }
                    else
                    {
                        if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().TitleId != null)
                        {
                            vh.TokUserTitle.Text = items[position].TitleId; //Settings.GetTokketUser().TitleId;
                        }
                    }
                }
                else
                {
                    if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().TitleId != null)
                    {
                        vh.TokUserTitle.Text = items[position].TitleId; //Settings.GetTokketUser().TitleId;
                    }
                }

                if (string.IsNullOrEmpty(vh.TokUserTitle.Text))
                {
                    vh.TokUserTitle.Visibility = ViewStates.Gone;
                }

                Glide.With(itemView).Load(items[position].StickerImage).Into(vh.TileSticker);
                if (string.IsNullOrEmpty(items[position].StickerImage))
                {
                    vh.TileSticker.Visibility = ViewStates.Gone;
                }

                vh.UserPhoto.ContentDescription = position.ToString();
                Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.UserPhoto);
                vh.UserFlag.SetImageBitmap(bitmap);

                vh.UserDisplayName.Text = items[position].UserDisplayName;

                vh.PrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);
                if (items[position].TokGroup == "Quote")
                {
                    vh.SecondaryFieldText.SetText(items[position].SecondaryFieldText, TextView.BufferType.Spannable);
                    vh.SecondaryFieldText.Visibility = ViewStates.Visible;
                }

                //vh.PrimaryFieldText.Text = items[position].PrimaryFieldText;

                vh.Category.Text = items[position].Category;
                vh.TokGroup.Text = items[position].TokGroup;
                vh.TokType.Text = items[position].TokType;
                if (string.IsNullOrEmpty(items[position].EnglishPrimaryFieldText))
                {
                    vh.EnglishPrimaryFieldText.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.EnglishPrimaryFieldText.Visibility = ViewStates.Visible;
                }
                vh.EnglishPrimaryFieldText.Text = items[position].EnglishPrimaryFieldText;
                vh.gridBackground.SetBackgroundResource(Resource.Drawable.tileview_layout);
                vh.gridBackground.Tag = position;
                int ndx = position % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                GradientDrawable Tokdrawable = (GradientDrawable)vh.Tokdrawable.Background;
                Tokdrawable.SetColor(Color.ParseColor(randomcolors[ndx]));
                vh.UserPhoto.ContentDescription = position.ToString();

                //vh.UserPhoto.Click -= onImageUsernameClick;
                //vh.UserPhoto.Click += onImageUsernameClick;
                vh.UserPhoto.SetOnTouchListener(this);


                vh.UserDisplayName.ContentDescription = position.ToString();
                vh.UserDisplayName.Click -= onImageUsernameClick;
                vh.UserDisplayName.Click += onImageUsernameClick;

                vh.Category.Tag = 0;
                vh.Category.ContentDescription = items[position].CategoryId;
                vh.Category.Click -= OnTokButtonClick;
                vh.Category.Click += OnTokButtonClick;
                vh.TokGroup.Tag = 1;
                vh.TokGroup.ContentDescription = items[position].TokGroup;
                vh.TokGroup.Click -= OnTokButtonClick;
                vh.TokGroup.Click += OnTokButtonClick;

                vh.TokType.Tag = 2;
                vh.TokType.ContentDescription = items[position].TokTypeId;
                vh.TokType.Click -= OnTokButtonClick;
                vh.TokType.Click += OnTokButtonClick;

                vh.gridBackground.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Gone;
            }
        }
        
        void onImageUsernameClick(object sender, EventArgs e)
        {
            if (Settings.ActivityInt != (int)ActivityType.ProfileTabActivity && Settings.ActivityInt != (int)ActivityType.ProfileActivity)
            {
                var sendertype = sender.GetType().Name;
                int position = 0;
                if (sendertype == "AppCompatImageView")
                {
                    position = Convert.ToInt32((sender as ImageView).ContentDescription);
                }
                else if (sendertype == "AppCompatTextView")
                {
                    position = Convert.ToInt32((sender as TextView).ContentDescription);
                }

                //if (Settings.GetUserModel().UserId == items[position].UserId)
                //{
                //    MainActivity.Instance.viewpager.SetCurrentItem(4, true);
                //}
                //else
                //{
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", items[position].UserId);
                MainActivity.Instance.StartActivity(nextActivity);
                //}
            }
        }
        void OnTokButtonClick(object sender, EventArgs e)
        {
            string titlepage = "";
            string filter = "";
            string headerpage = (sender as TextView).Text;
            bool gotonextpage = true;

            if ((int)(sender as TextView).Tag == (int)Toks.Category)
            {
                if (Settings.FilterTag == 3)
                {
                    gotonextpage = false;
                }
                Settings.FilterTag = 3;
                titlepage = "Category";
                filter = (sender as TextView).ContentDescription;
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokGroup)
            {
                if (Settings.FilterTag == 6)
                {
                    gotonextpage = false;
                }

                Settings.FilterTag = 6;
                titlepage = "Tok Group";
                filter = (sender as TextView).ContentDescription.ToLower();
            }
            else if ((int)(sender as TextView).Tag ==(int)Toks.TokType)
            {
                if (Settings.FilterTag == 1)
                {
                    gotonextpage = false;
                }

                Settings.FilterTag = 1;
                titlepage = "Tok Type";
                filter = (sender as TextView).ContentDescription;
            }

            if (gotonextpage)
            {
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(ToksActivity));
                nextActivity.PutExtra("titlepage", titlepage);
                nextActivity.PutExtra("filter", filter);
                nextActivity.PutExtra("headerpage", headerpage);
                nextActivity.SetFlags(ActivityFlags.ReorderToFront);
                nextActivity.SetFlags(ActivityFlags.NewTask);
                MainActivity.Instance.StartActivity(nextActivity);
            }
        }
        //public void OnGridBackgroundClick(object sender, int position)
        //{
        //    int photoNum = position + 1;
        //    Intent nextActivity = new Intent(MainActivity.Instance, typeof(TokInfoActivity));
        //    var tokModel = items[position];
        //    var modelConvert = JsonConvert.SerializeObject(tokModel);
        //    nextActivity.PutExtra("tokModel", modelConvert);
        //    MainActivity.Instance.StartActivity(nextActivity);
        //}
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.listview_row, parent, false);

            vh = new TokViewHolder(itemView, OnClick);
            return vh;
        }
        #endregion

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
                if (Settings.ActivityInt != (int)ActivityType.ProfileTabActivity && Settings.ActivityInt != (int)ActivityType.ProfileActivity)
                {
                    //When Image of User Photo is clicked.
                    int position = Convert.ToInt32(v.ContentDescription);
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
                    nextActivity.PutExtra("userid", items[position].UserId);
                    MainActivity.Instance.StartActivity(nextActivity);
                }
            }
            return true;
        }
        #endregion
    }
}