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
using Android.Text.Style;

namespace Tokket.Android.Adapters
{
    internal class AddRemoveSetToFromSuperSetAdapter : RecyclerView.Adapter
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public event EventHandler<View.TouchEventArgs> TouchHandler;
        public List<ClassSetModel> items;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => items.Count;
        Context context;
        View itemView;
        #endregion


        #region Constructor
        public AddRemoveSetToFromSuperSetAdapter(Context context)
        {
            this.context = context;
        }

        public AddRemoveSetToFromSuperSetAdapter(List<ClassSetModel> _items, EventHandler<View.TouchEventArgs> OnGridBackgroundTouched) //If caller is Activity or Fragment
        {
            items = _items;
            TouchHandler = OnGridBackgroundTouched;
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<ClassSetModel> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeChanged(position, listUpdate.Count);
        }

        public void AddItem(ClassSetModel item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(ClassSetModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion
        #region Override Events/Methods/Delegates
        public MySetsViewHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        public int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as MySetsViewHolder;

            //Set LinearColor
            string colorHex = "";
            int ndx = position % Colors.Count;

            //MySetsVm.lblMySetPopUp = vh.lblMySetPopUp;

            //Hide views since they are not showing in the UI
            vh.txtSetsReferenceId.Visibility = ViewStates.Gone;
            vh.SetUserImage.Visibility = ViewStates.Gone;
            vh.SetUserName.Visibility = ViewStates.Gone;

            if (items != null)
            {
                colorHex = items[position].ColorMainHex;
                //get the complete url of the tokmoji
                /*foreach (var item in ListTokmojiModel)
                {
                    items[position].Name = items[position].Name.Replace(":" + item.Id + ":", "<img src='" + item.Image + "' style='width: 150px;' />");
                }*/

                // vh.txtSetsTokUpper.SetText(HtmlCompat.FromHtml(items[position].Name, HtmlCompat.FromHtmlModeLegacy), TextView.BufferType.Spannable);

                //vh.txtSetsTokUpper.SetText(Html.FromHtml(items[position].Name, new ImageGetter(), null), TextView.BufferType.Spannable) ;

               // SpannableHelper.ListTokMoji = ListTokmojiModel;
                SpannableStringBuilder ssbName = new SpannableStringBuilder(items[position].Name);

                var result = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);

                vh.txtSetsTokUpper.SetText(result, TextView.BufferType.Spannable);

                vh.txtSetsTokBottom.Text = items[position].TokType + " • " + items[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + items[position].TokIds.Count + " Toks";

                if (!string.IsNullOrEmpty(items[position].Image))
                {
                    if (URLUtil.IsValidUrl(items[position].Image))
                    {
                        Glide.With(itemView).Load(items[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.ImgMySetsRow);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(items[position].Image);
                        //vh.ImgMySetsRow.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                        Glide.With(itemView).AsBitmap().Load(imageByte).Into(vh.ImgMySetsRow);
                    }
                }
                else
                {
                    vh.ImgMySetsRow.SetImageBitmap(null);
                }
            }
            else if (items != null)
            {
                colorHex = items[position].ColorMainHex;
                if (items[position].Label != "superset")
                    vh.txtSetsTokUpper.Text = "Set name: " + items[position].Name; //ListClassSetModel[position].ClassSet.Name;
                else
                    vh.txtSetsTokUpper.Text = "Super Set name: " + items[position].Name;
                vh.txtClassDescription.Visibility = ViewStates.Visible;
                vh.txtClassDescription.Text = items[position].Description;

                vh.txtSetsTokBottom.Text = items[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + items[position].TokIds.Count + " Toks";
                //vh.txtSetsTokBottom.Text = ListClassSetModel[position].ClassSet.CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassSetModel[position].ClassSet.TokIds.Count + " Toks";

                if (!string.IsNullOrEmpty(items[position].Image))
                {
                    if (URLUtil.IsValidUrl(items[position].Image))
                    {
                        Glide.With(itemView).Load(items[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.ImgMySetsRow);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(items[position].Image.Replace("data:image/jpeg;base64,", ""));
                        vh.ImgMySetsRow.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }
                else
                {
                    vh.ImgMySetsRow.SetImageBitmap(null);
                }

                vh.SetUserName.Text = items[position].UserDisplayName;
                if (!string.IsNullOrEmpty(items[position].UserPhoto))
                {
                    //Bitmap bitmap = GetBitmapFromUrl(model.image);
                    // Glide.With(itemView).Load(ListClassItems[position].UserPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.SetUserImage);

                    if (URLUtil.IsValidUrl(items[position].UserPhoto))
                    {
                        Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.SetUserImage);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(items[position].UserPhoto.Replace("data:image/jpeg;base64,", ""));
                        vh.SetUserImage.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }
                else
                {
                    vh.SetUserImage.SetImageResource(Resource.Drawable.Man3);
                }

                if (items[position].UserId == Settings.GetTokketUser().Id)
                {
                    if (!string.IsNullOrEmpty(Settings.GetTokketUser().UserPhoto))
                    {
                        if (URLUtil.IsValidUrl(Settings.GetTokketUser().UserPhoto))
                        {
                            Glide.With(itemView).Load(Settings.GetTokketUser().UserPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.SetUserImage);
                        }
                        else
                        {
                            byte[] imageByte = Convert.FromBase64String(Settings.GetTokketUser().UserPhoto.Replace("data:image/jpeg;base64,", ""));
                            vh.SetUserImage.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                        }
                    }
                    else
                    {
                        vh.SetUserImage.SetImageBitmap(null);
                    }
                }

            }

            vh.lblMySetPopUp.Tag = position;


            //if (Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo))
            //{
            //    vh.lblMySetPopUp.Visibility = ViewStates.Gone;
            //    if (selectedPosition == position)
            //    {
            //        if (items != null)
            //        {
            //            MySetsVm.SetModel = items[position];
            //        }
            //        else if (ListClassItems != null)
            //        {
            //            MySetsVm.SetModel = ListClassItems[position];
            //        }

            //        vh.ItemView.SetBackgroundColor(Color.LightBlue);
            //    }
            //    else if (selectedPosition != position)
            //    {
            //        vh.ItemView.SetBackgroundColor(Color.Transparent);
            //    }

            //    vh.ItemView.Click += (sender, e) =>
            //    {
            //        selectedPosition = position;
            //        NotifyDataSetChanged();
            //    };

            //    //changed the color not random, because everytime a row is clicked, the color will also change because of NotifyDataSetChanged();
            //    if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.ToList();
            //}
            //else
            //{
            //    if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            //}
            randomcolors = Colors.Shuffle().ToList();
            if (string.IsNullOrEmpty(colorHex))
            {
                vh.linearMySetsColor.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));
            }
            else
            {
                try
                {
                    vh.linearMySetsColor.SetBackgroundColor(Color.ParseColor(colorHex));
                }
                catch (Exception)
                {
                    //in case failed due to unknown color
                    vh.linearMySetsColor.SetBackgroundColor(Color.ParseColor(randomcolors[ndx]));
                }
            }

            vh.ItemView.Tag = position;
            vh.ItemView.Touch -= TouchHandler;
            vh.ItemView.Touch += TouchHandler;
            
            if (items[position].isCheck)
            {
                vh.ItemView.SetBackgroundColor(Color.Yellow);
            }
            else if (selectedPosition != position)
            {
                vh.ItemView.SetBackgroundColor(Color.Transparent);
            }
        }




        //private void onImageUsernameClick(object sender, EventArgs e)
        //{
        //    var sendertype = sender.GetType().Name;
        //    int position = 0;
        //    if (sendertype == "AppCompatImageView")
        //    {
        //        position = Convert.ToInt32((sender as ImageView).ContentDescription);
        //    }
        //    else if (sendertype == "AppCompatTextView")
        //    {
        //        position = Convert.ToInt32((sender as TextView).ContentDescription);
        //    }

        //    //if (Settings.GetUserModel().UserId == items[position].UserId)
        //    //{
        //    //    MainActivity.Instance.viewpager.SetCurrentItem(4, true);
        //    //}
        //    //else
        //    //{
        //    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
        //    nextActivity.PutExtra("userid", items[position].UserId);
        //    MainActivity.Instance.StartActivity(nextActivity);
        //    //}

        //}
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.mytoksets_row, parent, false);

            vh = new MySetsViewHolder(itemView, OnClick);
            return vh;
        }
        #endregion


        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        #endregion
    }

    internal class AddRemoveSetToFromSuperSetViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}