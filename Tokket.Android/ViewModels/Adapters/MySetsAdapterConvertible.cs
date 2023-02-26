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
using Tokket.Android.TokQuest;
using Tokket.Android.ViewModels;

namespace Tokket.Android.Adapters
{

    public class MySetsAdapterConvertible : RecyclerView.Adapter, View.IOnTouchListener
    {
        int totalItems = 0;
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

        AlertDialog.Builder dialog;
        AlertDialog alert;

        List<string> randomcolors = new List<string>();
        public override int ItemCount => totalItems;
        View itemView;
        #region Constructor
        public MySetsAdapterConvertible(List<Set> _items, List<ClassSetModel> _ListClassItems, List<Tokmoji> _listTokmojiModel) //, List<ClassSetViewModel> _listClassSetModel
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
        #endregion

        #region Override Events/Methods/Delegates
        MySetsViewHolderConvertible vh;
        public ViewModels.MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        AssetManager assetManager = Application.Context.Assets;
        int selectedPosition = -1;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as MySetsViewHolderConvertible;

            //Set LinearColor
            string colorHex = "";
            int ndx = position % Colors.Count;

            MySetsVm.lblMySetPopUp = vh.lblMySetPopUp;

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
                vh.lblMySetPopUp.ContentDescription = position.ToString();
                vh.lblMySetPopUp.Click += LblMySetPopUp_Click; 

            }
            else if (ListClassItems != null)
            {
                colorHex = ListClassItems[position].ColorMainHex;
                vh.txtSetsTokUpper.Text = ListClassItems[position].Name; //ListClassSetModel[position].ClassSet.Name;

                vh.txtClassDescription.Visibility = ViewStates.Visible;
                vh.txtClassDescription.Text = ListClassItems[position].Description;

                vh.txtSetsTokBottom.Text = ListClassItems[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassItems[position].TokIds.Count + " Toks";
                //vh.txtSetsTokBottom.Text = ListClassSetModel[position].ClassSet.CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassSetModel[position].ClassSet.TokIds.Count + " Toks";

                if (!string.IsNullOrEmpty(ListClassItems[position].Image))
                {
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

                vh.ItemView.Click += (sender, e) =>
                {
                    selectedPosition = position;
                    NotifyDataSetChanged();
                };

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
                vh.linearMySetsColor.SetBackgroundColor(Color.ParseColor(colorHex));
            }
        }

        private void LblMySetPopUp_Click(object sender, EventArgs e )
        {
            var pos = (TextView)sender;
            Intent nextActivity;

            int num = Convert.ToInt32(pos.ContentDescription);
            var tokcounts = ListClassItems[num].TokIds.Count;
            // display alert
            if (tokcounts <= 0)
            {
                dialog = new AlertDialog.Builder(itemView.Context);
                alert = dialog.Create();
                alert.SetTitle("Alert");
                alert.SetIcon(Resource.Drawable.tokquesticon);
                alert.SetMessage("Must have at least 1 tok!");
                alert.SetButton("OK", (c, ev) =>
                {
                    alert.Dismiss();
                    alert.Hide();
                });
                alert.SetButton2("CANCEL", (c, ev) => {
                    alert.Dismiss();
                    alert.Hide();
                });
                alert.Show();
            }
            // continu to the form
            else
            {

                nextActivity = new Intent(MainActivity.Instance, typeof(CreateGameSet));
                var classId = ListClassItems[num].Id;
                var ownerId = Settings.GetTokketUser().Id + "-classsets0";
                var details = new CreateGameDetailsViewModel();
                details.ClassSetId = classId;
                details.OwnerId = ownerId;
                details.ChosenName = ListClassItems[num].Name;
                var modelConvert = JsonConvert.SerializeObject(details);

                nextActivity.PutExtra("details", modelConvert);
                
                MainActivity.Instance.StartActivity(nextActivity);
            }
        }

        public void OnItemRowClick(object sender, int position)
        {
            //if (Settings.ActivityInt == Convert.ToInt16(ActivityType.LeftMenuSets) || Settings.ActivityInt == Convert.ToInt16(ActivityType.ClassGroupActivity))
            //{
                Intent nextActivity;

               var tokcounts =  ListClassItems[position].TokIds.Count;
                // display alert
                if (tokcounts <= 0)
                {
                    dialog = new AlertDialog.Builder(itemView.Context);
                    alert = dialog.Create();
                    alert.SetTitle("Alert");
                    alert.SetIcon(Resource.Drawable.tokquesticon);
                    alert.SetMessage("Must have at least 1 tok!");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        alert.Dismiss();
                        alert.Hide();
                    });
                    alert.SetButton2("CANCEL", (c, ev) => {
                        alert.Dismiss();
                        alert.Hide();
                    });
                    alert.Show();
                }
                // continu to the form
                else {

                  
                    nextActivity = new Intent(MainActivity.Instance, typeof(CreateGameSet));
                    var classId = ListClassItems[position].Id;
                    var ownerId = Settings.GetTokketUser().Id + "-classsets0";
                    var details = new CreateGameDetailsViewModel();
                    details.ClassSetId = classId;
                    details.OwnerId = ownerId;
                    details.ChosenName = ListClassItems[position].Name;
                    var modelConvert = JsonConvert.SerializeObject(details);

                    nextActivity.PutExtra("details", modelConvert);

                    MainActivity.Instance.StartActivity(nextActivity);
                }

            
            //}
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.convertible_toksets_row, parent, false);

            vh = new MySetsViewHolderConvertible(itemView, OnClick);
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