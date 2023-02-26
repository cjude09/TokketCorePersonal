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
using AndroidX.AppCompat.Widget;

namespace Tokket.Android.Adapters
{
    public class MySetsAdapter : RecyclerView.Adapter, View.IOnTouchListener
    {
        int calledFromActivity = Settings.ActivityInt;
        int totalItems = 0;
        bool isShowCheckBox = false, isDelete = false;
        ClassGroupModel classGroupModel;
        public event EventHandler<int> ItemClick;
        public List<Set> items;
        public List<ClassSetModel> ListClassItems;

        public List<Set> itemsChecked;
        public List<ClassSetModel> ListClassItemsChecked;
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
        public MySetsAdapter(List<Set> _items, List<ClassSetModel> _ListClassItems, List<Tokmoji> _listTokmojiModel) //, List<ClassSetViewModel> _listClassSetModel
        {
            itemsChecked = new List<Set>();
            ListClassItemsChecked = new List<ClassSetModel>();

            isShowCheckBox = false;
            items = _items;
            ListClassItems = _ListClassItems;
            //ListClassSetModel = _listClassSetModel;
            ListTokmojiModel  = _listTokmojiModel;
            if (items != null)
            {
                totalItems = items.Count();
            }
            else if (ListClassItems != null)
            {
                totalItems = ListClassItems.Count();
            }
        }

        public void isShowCheckBoxDelete(bool _isShowCheckBox)
        {
            isShowCheckBox = _isShowCheckBox;
            NotifyDataSetChanged();
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

            MySetsVm.lblMySetPopUp = vh.lblMySetPopUp;

            vh.txtSetsReferenceId.Visibility = ViewStates.Gone;
            if (isShowCheckBox)
            {
                vh.chkBox.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.chkBox.Visibility = ViewStates.Gone;
            }

            vh.chkBox.Tag = position;
            vh.chkBox.CheckedChange -= chkChange;
            vh.chkBox.CheckedChange += chkChange;

            vh.SetUserImage.Click -= userIsClicked;
            vh.SetUserImage.Click += userIsClicked;

            vh.SetUserName.Click -= userIsClicked;
            vh.SetUserName.Click += userIsClicked;

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

                if (!string.IsNullOrEmpty(items[position].ReferenceId))
                {
                    vh.txtSetsReferenceId.Visibility = ViewStates.Visible;
                    vh.txtSetsReferenceId.Text = items[position].ReferenceId;
                }

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
            else if(ListClassItems != null)
            {
                colorHex = ListClassItems[position].ColorMainHex;
                if (ListClassItems[position].Label != "superset")
                {
                    vh.txtSetsTokUpper.Text = "Set name: " + ListClassItems[position].Name; //ListClassSetModel[position].ClassSet.Name;
                    vh.txtSetsTokBottom.Text = ListClassItems[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassItems[position].TokIds.Count + " Toks";

                }
                else {
                    vh.txtSetsTokUpper.Text = "Super Set name: " + ListClassItems[position].Name;
                    vh.txtSetsTokBottom.Text = ListClassItems[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassItems[position].Sets.Count + " Sets";
                }

                if (!string.IsNullOrEmpty(ListClassItems[position].ReferenceId))
                {
                    vh.txtSetsReferenceId.Visibility = ViewStates.Visible;
                    vh.txtSetsReferenceId.Text = ListClassItems[position].ReferenceId;
                }

                vh.txtClassDescription.Visibility = ViewStates.Visible;
                vh.txtClassDescription.Text = ListClassItems[position].Description;

               // vh.txtSetsTokBottom.Text = ListClassItems[position].CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassItems[position].TokIds.Count + " Toks";
                //vh.txtSetsTokBottom.Text = ListClassSetModel[position].ClassSet.CreatedTime.ToString("MM/dd/yyyy") + " • " + ListClassSetModel[position].ClassSet.TokIds.Count + " Toks";

                if (!string.IsNullOrEmpty(ListClassItems[position].Image))
                {
                    if (URLUtil.IsValidUrl(ListClassItems[position].Image))
                    {
                        Glide.With(itemView).Load(ListClassItems[position].Image).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.ImgMySetsRow);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(ListClassItems[position].Image.Replace("data:image/jpeg;base64,",""));
                        vh.ImgMySetsRow.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }
                else
                {
                    vh.ImgMySetsRow.SetImageBitmap(null);
                }

                vh.SetUserImage.ContentDescription = ListClassItems[position].UserId;
                vh.SetUserName.ContentDescription = ListClassItems[position].UserId;

                vh.SetUserName.Text = ListClassItems[position].UserDisplayName;
                if (!string.IsNullOrEmpty(ListClassItems[position].UserPhoto))
                {
                    //Bitmap bitmap = GetBitmapFromUrl(model.image);
                   // Glide.With(itemView).Load(ListClassItems[position].UserPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.SetUserImage);

                    if (URLUtil.IsValidUrl(ListClassItems[position].UserPhoto))
                    {
                        Glide.With(itemView).Load(ListClassItems[position].UserPhoto).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation)).Into(vh.SetUserImage);
                    }
                    else
                    {
                        byte[] imageByte = Convert.FromBase64String(ListClassItems[position].UserPhoto.Replace("data:image/jpeg;base64,", ""));
                        vh.SetUserImage.SetImageBitmap((BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length)));
                    }
                }
                else
                {
                    vh.SetUserImage.SetImageResource(Resource.Drawable.Man3);
                }

                if (ListClassItems[position].UserId == Settings.GetTokketUser().Id) {
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
        }
        private void userIsClicked(object sender, EventArgs e)
        {
            var v = sender as View;
            var UserId = v.ContentDescription;
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
            nextActivity.PutExtra("userid", UserId);
            MainActivity.Instance.StartActivity(nextActivity);
        }
        private void chkChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var checkBox = sender as AppCompatCheckBox;
            int position = 0;
            try { position = (int)checkBox.Tag; } catch { position = int.Parse((string)checkBox.Tag); }

            if (items != null)
            {
                items[position].isCheck = checkBox.Checked;

                var result = itemsChecked.FirstOrDefault(c => c.Id == items[position].Id);
                if (result != null) //If found
                {
                    int ndx = itemsChecked.IndexOf(result);
                    itemsChecked.Remove(result);
                }
                else
                {
                    if (checkBox.Checked) //Add checker to be sure
                    {
                        itemsChecked.Add(items[position]);
                    }
                }
            }
            else if (ListClassItems != null)
            {
                ListClassItems[position].isCheck = checkBox.Checked;

                var result = ListClassItemsChecked.FirstOrDefault(c => c.Id == ListClassItems[position].Id);
                if (result != null) //If found
                {
                    int ndx = ListClassItemsChecked.IndexOf(result);
                    ListClassItemsChecked.Remove(result);
                }
                else //if save
                {
                    if (checkBox.Checked) //Add checker to be sure
                    {
                        ListClassItemsChecked.Add(ListClassItems[position]);
                    }
                }
            }
        }
        public void OnItemRowClick(object sender, int position)
        {
            if (calledFromActivity == Convert.ToInt16(ActivityType.LeftMenuSets) || calledFromActivity == Convert.ToInt16(ActivityType.ClassGroupActivity))
            {
                Intent nextActivity;

                if (items != null)
                {
                    nextActivity = new Intent(MainActivity.Instance, typeof(MySetsViewActivity));
                    var modelConvert = JsonConvert.SerializeObject(items[position]);
                    nextActivity.PutExtra("setModel", modelConvert);
                    MainActivity.Instance.StartActivity(nextActivity);
                }
                else if (ListClassItems != null)
                {
                    if (ListClassItems[position].Label != "superset")
                    {
                        nextActivity = new Intent(MainActivity.Instance, typeof(MyClassSetsViewActivity));
                        var modelConvert = JsonConvert.SerializeObject(ListClassItems[position]);
                        nextActivity.PutExtra("classsetModel", modelConvert);
                        MainActivity.Instance.StartActivity(nextActivity);
                    }
                    else {
                        nextActivity = new Intent(MainActivity.Instance, typeof(SuperSetInfoActivity));
                        var modelConvert = JsonConvert.SerializeObject(ListClassItems[position]);
                        nextActivity.PutExtra("supersetModel", modelConvert);
                        MainActivity.Instance.StartActivity(nextActivity);
                    }
                
                }
            }
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