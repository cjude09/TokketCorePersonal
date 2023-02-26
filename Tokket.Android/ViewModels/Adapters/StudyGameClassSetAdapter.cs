using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Tokket.Android.ViewHolders;
using Tokket.Core;
using Tokket.Shared.Extensions;
using Newtonsoft.Json;
using Tokket.Shared.Helpers;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Android.Webkit;
using Tokket.Shared.Models;
using Android.Text;
using Tokket.Android.Helpers;
using AndroidX.RecyclerView.Widget;
using Color = Android.Graphics.Color;
using Tokket.Android.Custom;

namespace Tokket.Android.Adapters
{

    public class StudyGameClassSetAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        int totalItems = 0;
        ClassGroupModel classGroupModel;
        public event EventHandler<int> ItemClick;
        public List<Set> items;
        public List<ClassSetModel> ListClassItems;
        //public List<ClassSetViewModel> ListClassSetModel;
        private List<Tokmoji> ListTokmojiModel = new List<Tokmoji>();
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        public override int ItemCount => totalItems;
        View itemView;
        #region Constructor
        public StudyGameClassSetAdapter(List<Set> _items, List<ClassSetModel> _ListClassItems, List<Tokmoji> _listTokmojiModel) //, List<ClassSetViewModel> _listClassSetModel
        {
            items = _items;
            ListClassItems = _ListClassItems;
            //ListClassSetModel = _listClassSetModel;
            ListTokmojiModel = _listTokmojiModel;
            if (items != null)
            {
                totalItems = items.Count();
            }
            else if (ListClassItems != null)
            {
                totalItems = ListClassItems.Count();
            }

        }

        public void updateItems(List<Set> _items, List<ClassSetModel> _ListClassItems)
        {
            if (items != null)
            {
                items = _items;
            }
            else if (ListClassItems != null)
            {
                ListClassItems = _ListClassItems;
            }
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        MySetsViewHolder vh;
        public ViewModels.MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        AssetManager assetManager = Application.Context.Assets;
        int selectedPosition = -1;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as MySetsViewHolder;

            //Set LinearColor
            string colorHex = "";
            int ndx = position % Colors.Count;

            vh.lblMySetPopUp.Visibility = ViewStates.Gone;//Hide for this adapter.
            MySetsVm.lblMySetPopUp = vh.lblMySetPopUp;

            if (items != null)
            {
                colorHex = items[position].ColorMainHex;
                SpannableHelper.ListTokMoji = ListTokmojiModel;
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
            else if (ListClassItems != null)
            {
                vh.linearUserInfo.Visibility = ViewStates.Gone;
                colorHex = ListClassItems[position].ColorMainHex;
                vh.txtSetsTokUpper.Text = ListClassItems[position].Name; //ListClassSetModel[position].ClassSet.Name;

                vh.txtClassDescription.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(ListClassItems[position].Description))
                {
                    vh.txtClassDescription.Text = ListClassItems[position].Description;
                }

                if (!string.IsNullOrEmpty(ListClassItems[position].ReferenceId))
                {
                    vh.txtSetsReferenceId.Visibility = ViewStates.Visible;
                    vh.txtSetsReferenceId.Text = ListClassItems[position].ReferenceId;
                }
                else
                {
                    vh.txtSetsReferenceId.Visibility = ViewStates.Gone;
                }

                vh.txtSetsTokBottom.Text = ListClassItems[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassItems[position].TokIds.Count + " Toks";
                //vh.txtSetsTokBottom.Text = ListClassSetModel[position].ClassSet.CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassSetModel[position].ClassSet.TokIds.Count + " Toks";

                if (!string.IsNullOrEmpty(ListClassItems[position].Image))
                {
                    vh.ImgMySetsRow.Visibility = ViewStates.Visible;
                    if (URLUtil.IsValidUrl(ListClassItems[position].Image))
                    {
                        Glide.With(itemView).Load(ListClassItems[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.ImgMySetsRow);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(ListClassItems[position].Image.Replace("data:image/jpeg;base64,", ""));
                        vh.ImgMySetsRow.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }
                else
                {
                    vh.ImgMySetsRow.Visibility = ViewStates.Gone;
                    vh.ImgMySetsRow.SetImageBitmap(null);
                }
            }

            vh.lblMySetPopUp.Tag = position;

            if (Settings.ActivityInt == Convert.ToInt16(ActivityType.TokInfo))
            {
                vh.lblMySetPopUp.Visibility = ViewStates.Gone;
                if (selectedPosition == position)
                {
                    if (items != null)
                    {
                        MySetsVm.SetModel = items[position];
                    }
                    else if (ListClassItems != null)
                    {
                        MySetsVm.SetModel = ListClassItems[position];
                    }

                    vh.ItemView.SetBackgroundColor(Color.LightBlue);
                }
                else if (selectedPosition != position)
                {
                    vh.ItemView.SetBackgroundColor(Color.Transparent);
                }

                //changed the color not random, because everytime a row is clicked, the color will also change because of NotifyDataSetChanged();
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.ToList();
            }
            else
            {
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            }

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
            vh.ItemView.Click -= itemViewIsClicked;
            vh.ItemView.Click += itemViewIsClicked;
            if (selectedPosition == position)
            {
                vh.ItemView.ContentDescription = "selected";
                vh.ItemView.SetBackgroundColor(Color.LightBlue);

                var modelConvert = JsonConvert.SerializeObject(ListClassItems[position]);
                StudyGamesClassSetDialog.Instance.btnOk.ContentDescription = modelConvert;
            }
            else
            {
                vh.ItemView.ContentDescription = "";
                vh.ItemView.SetBackgroundColor(Color.Transparent);
            }
        }
        public override int GetItemViewType(int position)
        {
            return position;

            //This was added due to the reason that when clicked, there will be 2 rows that will highlight
            //For Reference, this one have similar issue with the link below
            //https://stackoverflow.com/questions/32065267/recyclerview-changing-items-during-scroll
        }

        public void itemViewIsClicked(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as View).Tag; } catch { position = int.Parse((string)(sender as View).Tag); }

            selectedPosition = position;
            NotifyDataSetChanged();
        }
        
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

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up)
            {
            }
            return true;
        }
        #endregion
    }
}