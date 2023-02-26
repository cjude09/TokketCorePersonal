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
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Extensions;
using Tokket.Shared.Helpers;
using Android.Webkit;
using Tokket.Shared.Services;
using AndroidX.RecyclerView.Widget;
using Tokket.Core;
using Tokket.Android.Helpers;
using Android.Text;
using Color = Android.Graphics.Color;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Android.OS;
using Newtonsoft.Json;

namespace Tokket.Android.Adapters
{
    public class TokquestAdapter : RecyclerView.Adapter, View.IOnTouchListener// View.IOnLongClickListener
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public List<Shared.Models.ClassGroupModel> items;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => items.Count;
        View itemView;
        int ViewPosition;
        #endregion
        #region Constructor
        public TokquestAdapter(List<Shared.Models.ClassGroupModel> _items, List<Tokmoji> _listTokMoji) //If caller is Activity or Fragment
        {
            items = _items;
            SpannableHelper.ListTokMoji = _listTokMoji;
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<Shared.Models.ClassGroupModel> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeChanged(position, listUpdate.Count);
        }

        public void AddItem(Shared.Models.ClassGroupModel item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(Shared.Models.ClassGroupModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        AbbreviationViewHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as AbbreviationViewHolder;
            Resources res = Application.Context.Resources;

            var font = Typeface.CreateFromAsset(assetManager, "fa_solid_900.otf");

            Stream sr = null;
            ViewPosition = position;

            Bitmap bitmapFlag = BitmapFactory.DecodeStream(sr);
            
            if (!string.IsNullOrEmpty(items[position].Image))
            {
                vh.ItemView.Tag = vh.TokImgPrimaryFieldText;
                string tokimg = tokimg = items[position].Image;

                if (URLUtil.IsValidUrl(tokimg))
                {
                    if (items[position].Image.EndsWith(".png") || items[position].Image.EndsWith(".jpg"))
                    {
                        tokimg = items[position].Image;
                    }
                    else
                    {
                        tokimg = items[position].Image + ".jpg";
                    }

                    Glide.With(itemView).Load(tokimg).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(vh.TokImgMain);
                }
                else
                {
                    tokimg = tokimg.Replace("data:image/jpeg;base64,", "");
                    byte[] imageDetailBytes = Convert.FromBase64String(tokimg);
                    vh.TokImgMain.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }

                Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.TokImgUserPhoto);

                vh.TokImgUserPhoto.ContentDescription = position.ToString();
                vh.TokImgUserPhoto.SetOnTouchListener(this);

              

                if (string.IsNullOrEmpty(vh.TokUserTitleImg.Text))
                {
                    vh.TokUserTitleImg.Visibility = ViewStates.Gone;
                }

                vh.ImgUserDisplayName.Text = items[position].UserDisplayName;
                vh.ImgUserDisplayName.ContentDescription = position.ToString();
                vh.ImgUserDisplayName.Click -= onImageUsernameClick;
                vh.ImgUserDisplayName.Click += onImageUsernameClick;

                
              
                vh.gridTokImage.SetBackgroundResource(Resource.Drawable.tileview_layout);
                vh.gridTokImage.Tag = position;

                int ndx = position % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                GradientDrawable tokimagedrawable = (GradientDrawable)vh.tokimgdrawable.Background;
                //tokimagedrawable.SetStroke(10, Color.ParseColor(randomcolors[ndx]));
                tokimagedrawable.SetColor(Color.White);

                vh.TokImgMain.ContentDescription = position.ToString();

                vh.TokImgCategory.Tag = 0;
                //vh.TokImgCategory.ContentDescription = items[position].CategoryId;
                vh.TokImgCategory.Click += OnTokButtonClick;

                vh.TokImgTokType.Tag = 2;
                //vh.TokImgTokType.ContentDescription = items[position].TokTypeId;
                vh.TokImgTokType.Click += OnTokButtonClick;

                vh.gridTokImage.Visibility = ViewStates.Visible;
                vh.gridBackground.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.ItemView.Tag = vh.PrimaryFieldText;
               
                Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.UserPhoto);

                if (string.IsNullOrEmpty(vh.TokUserTitle.Text))
                {
                    vh.TokUserTitle.Visibility = ViewStates.Gone;
                }



                vh.UserDisplayName.Text = items[position].UserDisplayName;
                //vh.TokType.Text = items[position].TokType;
                vh.gridBackground.SetBackgroundResource(Resource.Drawable.tileview_layout);
                vh.gridBackground.Tag = position;
                int ndx = position % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                GradientDrawable Tokdrawable = (GradientDrawable)vh.Tokdrawable.Background;
                //Tokdrawable.SetColor(Color.ParseColor(randomcolors[ndx]));

                if (items[position].ColorMainHex == "#FFFFFF" || string.IsNullOrEmpty(items[position].ColorMainHex))
                {
                    Tokdrawable.SetColor(Color.White);
                    Tokdrawable.SetStroke(10, Color.ParseColor(randomcolors[ndx]));
                    setTextColor(Color.Black);
                }
                else
                {
                    Tokdrawable.SetStroke(10, Color.ParseColor(items[position].ColorMainHex));
                    Tokdrawable.SetColor(Color.ParseColor(items[position].ColorMainHex));
                    setTextColor(Color.White);
                }

                vh.UserPhoto.ContentDescription = position.ToString();
                vh.UserPhoto.SetOnTouchListener(this);


                vh.UserDisplayName.ContentDescription = position.ToString();
                vh.UserDisplayName.Click -= onImageUsernameClick;
                vh.UserDisplayName.Click += onImageUsernameClick;

                vh.Category.Tag = 0;
                vh.Category.Click += OnTokButtonClick;

                vh.gridBackground.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Gone;
            }

         
        }

        private void setTextColor(Color color)
        {
            vh.UserDisplayName.SetTextColor(color);
            vh.PrimaryFieldText.SetTextColor(color);
            vh.Category.SetTextColor(color);
            //vh.TokGroup.SetTextColor(color);
            //vh.TokType.SetTextColor(color);
            vh.EnglishPrimaryFieldText.SetTextColor(color);
            vh.lblTokViewMore.SetTextColor(color);
            vh.SecondaryFieldText.SetTextColor(color);
        }

        private void onImageUsernameClick(object sender, EventArgs e)
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
        void OnTokButtonClick(object sender, EventArgs e)
        {
            string titlepage = "";
            string filter = "";
            string headerpage = headerpage = (sender as TextView).Text;

            if ((int)(sender as TextView).Tag == (int)Toks.Category)
            {
                Settings.FilterTag = 3;
                titlepage = "Category";
                filter = (sender as TextView).ContentDescription;
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokGroup)
            {
                Settings.FilterTag = 6;
                titlepage = "Tok Group";
                filter = (sender as TextView).ContentDescription.ToLower();
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokType)
            {
                Settings.FilterTag = 1;
                titlepage = "Tok Type";
                filter = (sender as TextView).ContentDescription;
            }
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ClassToksActivity));
            nextActivity.PutExtra("titlepage", titlepage);
            nextActivity.PutExtra("filter", filter);
            nextActivity.PutExtra("headerpage", headerpage);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            MainActivity.Instance.StartActivity(nextActivity);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.alphatok_tile_row, parent, false);

            vh = new AbbreviationViewHolder(itemView,OnClick);
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
            if (e.Action == MotionEventActions.Down)
            {
                return false;
            }
            else if (e.Action == MotionEventActions.Up)
            {
                if (v.ContentDescription == "link")
                {
                    int ndx = 0;
                    try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }
                   
                    Clipboard.SetTextAsync("").GetAwaiter().GetResult();
                    MainActivity.Instance.RunOnUiThread(async () => await Clipboard.SetTextAsync(JsonConvert.SerializeObject(items[ndx]))); //items[ndx].Id
                }
                else
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