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
using Xamarin.Essentials;
using Android.OS;
using Newtonsoft.Json;
using Android.Text.Style;
using Tokket.Shared.Models.Tok;
using Bumptech.Glide.Load.Engine;
using Tokket.Core.Tools;
using AndroidX.AppCompat.Widget;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;

namespace Tokket.Android.Adapters
{
    class CustomTileAdapter : RecyclerView.Adapter, View.IOnTouchListener, View.IOnLongClickListener
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public List<OpportunityTok> items;
        public List<OpportunityTok> itemsCheckedList;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        bool isShowCheckBox = false;
        public override int ItemCount => items.Count;
        View itemView;
        #endregion
        public Context context;
        #region Constructor
        public CustomTileAdapter(List<OpportunityTok> _items) //If caller is Activity or Fragment
        {
            itemsCheckedList = new List<OpportunityTok>();
            isShowCheckBox = false;
            items = _items;
        }
        #endregion

        public CustomTileAdapter(Context context)
        {
            this.context = context;
        }
        public void isShowCheckBoxDelete(bool _isShowCheckBox)
        {
            isShowCheckBox = _isShowCheckBox;
            NotifyDataSetChanged();
        }
        #region Abstract Methods
        public void UpdateItems(List<OpportunityTok> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeChanged(position, listUpdate.Count);
        }

        public void AddItem(OpportunityTok item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(OpportunityTok item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        CustomHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as CustomHolder;
            Resources res = Application.Context.Resources;

            var font = Typeface.CreateFromAsset(assetManager, "fa_solid_900.otf");

            bool isCountry = true;
            string flagImg = "";
            Stream sr = null;
          
             //   vh.ImageYearbook.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Gone;
                vh.gridBackground.Visibility = ViewStates.Gone;

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

            try
            {
                //if (items[position].UserId == Settings.GetTokketUser().Id)
                //{
                //    items[position].UserCountry = Settings.GetTokketUser().Country;
                //    items[position].UserState = Settings.GetTokketUser().State;
                //}
                if (string.IsNullOrEmpty(items[position].Type) && !string.IsNullOrEmpty(items[position].OpportunityType))
                {
                    if (string.IsNullOrEmpty(items[position].UserCountry))
                    {
                        isCountry = false;
                        flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
                    }
                    else if (items[position].UserCountry.ToLower() == "us")
                    {
                        if (!string.IsNullOrEmpty(items[position].UserState))
                        {
                            isCountry = false;
                            try
                            {
                                var stateModel = CountryHelper.GetCountryStates("us").Find(x => x.Id == items[position].UserState);
                                flagImg = stateModel.Image;
                            }
                            catch (Exception)
                            {

                                flagImg = Tokket.Core.Tools.CountryTool.GetCountryFlagJPG1x1(items[position].UserState);
                            }
                        }
                        else
                        {
                            sr = assetManager.Open("Flags/" + items[position].UserCountry + ".jpg");
                        }
                    }
                    else
                    {
                        sr = assetManager.Open("Flags/" + items[position].UserCountry + ".jpg");
                    }
                }
                else {
                    if (string.IsNullOrEmpty(items[position].ItemCountry))
                    {
                        isCountry = false;
                        flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
                    }
                    else if (CountryHelper.GetCountryAbbreviation(items[position].ItemCountry).ToLower() == "us")
                    {
                        if (!string.IsNullOrEmpty(items[position].ItemState))
                        {
                            isCountry = false;
                            try
                            {
                                var stateModel = CountryHelper.GetCountryStates("us").Find(x => x.Id == items[position].ItemState);
                                flagImg = stateModel.Image;
                            }
                            catch (Exception)
                            {

                                flagImg = Tokket.Core.Tools.CountryTool.GetCountryFlagJPG1x1(items[position].ItemState);
                            }
                        }
                        else
                        {
                            sr = assetManager.Open("Flags/" + CountryHelper.GetCountryAbbreviation(items[position].ItemCountry) + ".jpg");
                        }
                    }
                    else
                    {
                        sr = assetManager.Open("Flags/" + CountryHelper.GetCountryAbbreviation(items[position].ItemCountry) + ".jpg");
                    }
                }

              
            }
            catch (Exception)
            {

                isCountry = false;
                flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
            }


            Bitmap bitmapFlag = BitmapFactory.DecodeStream(sr);
            var primary = string.IsNullOrEmpty(items[position].PrimaryFieldText) ? string.Empty : items[position].PrimaryFieldText;
            var ssbName = new SpannableStringBuilder();
            var spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);

            //  vh.txtLinkImage.Typeface = font;
            // vh.txtLink.Typeface = font;

            //    vh.txtLinkImage.Tag = position;
            //    vh.txtLinkImage.ContentDescription = "link";
            //     vh.txtLinkImage.SetOnTouchListener(this);

            //vh.txtLink.Tag = position;
            //vh.txtLink.ContentDescription = "link";
            //vh.txtLink.SetOnTouchListener(this);

            if (!string.IsNullOrEmpty(items[position].Image))
            {
                vh.ItemView.Tag = vh.TokImgPrimaryFieldText;
                string tokimg = tokimg = items[position].ThumbnailImage;

                if (URLUtil.IsValidUrl(tokimg))
                {
                    if (items[position].ThumbnailImage.EndsWith(".png") || items[position].ThumbnailImage.EndsWith(".jpg"))
                    {
                        tokimg = items[position].ThumbnailImage;
                    }
                    else
                    {
                        tokimg = items[position].ThumbnailImage + ".jpg";
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

                if (isCountry)
                {
                    vh.TokImgUserFlag.SetImageBitmap(bitmapFlag);
                }
                else
                {
                    Glide.With(itemView).Load(flagImg).Into(vh.TokImgUserFlag);
                }

                vh.ImgUserDisplayName.Text = items[position].UserDisplayName;
                vh.ImgUserDisplayName.ContentDescription = position.ToString();
                vh.ImgUserDisplayName.Click -= onImageUsernameClick;
                vh.ImgUserDisplayName.Click += onImageUsernameClick;

                //vh.TokImgPrimaryFieldText.Text = items[position].PrimaryFieldText;
                vh.TokImgPrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);

                vh.TokImgCategory.Text = items[position].Category;
                if (!string.IsNullOrEmpty(items[position].OpportunityType))
                {
                    vh.TokImgTokType.Text = items[position].OpportunityType.ToUpperCaseFirstLetter();
                }
                else
                {
                    vh.TokImgTokType.Text = items[position].Type.ToUpperCaseFirstLetter();
                }

                vh.gridTokImage.SetBackgroundResource(Resource.Drawable.tileview_layout);
                vh.gridTokImage.Tag = position;

                int ndx = position % Colors.Count;
                if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
                GradientDrawable tokimagedrawable = (GradientDrawable)vh.tokimgdrawable.Background;
                //tokimagedrawable.SetStroke(10, Color.ParseColor(randomcolors[ndx]));
                tokimagedrawable.SetColor(Color.White);

                vh.TokImgMain.ContentDescription = position.ToString();

                vh.TokImgCategory.Tag = 0;
                vh.TokImgCategory.ContentDescription = items[position].CategoryId;
                vh.TokImgCategory.Click += OnTokButtonClick;

                //vh.TokImgTokGroup.Text = items[position].TokGroup;
                //vh.TokImgTokGroup.Tag = 1;
                //vh.TokImgTokGroup.ContentDescription = items[position].TokGroup;
                //vh.TokImgTokGroup.Click += OnTokButtonClick;

                vh.TokImgTokType.Tag = 2;
                vh.TokImgTokType.ContentDescription = items[position].TokTypeId;
                vh.TokImgTokType.Click += OnTokButtonClick;

                vh.gridTokImage.Visibility = ViewStates.Visible;
                vh.gridBackground.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.ItemView.Tag = vh.PrimaryFieldText;
                //Purple Gem
                if (items[position].HasGemReaction)
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Gone;
                }

                Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.UserPhoto);

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

                if (isCountry)
                {
                    vh.UserFlag.SetImageBitmap(bitmapFlag);
                }
                else
                {
                    Glide.With(itemView).Load(flagImg).Into(vh.UserFlag);
                }

                Glide.With(itemView).Load(items[position].StickerImage).Into(vh.TileSticker);
                if (string.IsNullOrEmpty(items[position].StickerImage))
                {
                    vh.TileSticker.Visibility = ViewStates.Gone;
                }

                vh.UserDisplayName.Text = items[position].UserDisplayName;
                vh.PrimaryFieldText.Text = items[position].PrimaryFieldText;
                vh.SecondaryFieldText.Text = items[position].SecondaryFieldText;
                vh.Category.Text = items[position].Category;
                if (!string.IsNullOrEmpty(items[position].OpportunityType))
                    vh.TokType.Text = items[position].OpportunityType.ToUpperCaseFirstLetter();
                else
                    vh.TokType.Text = items[position].Type.ToUpperCaseFirstLetter();
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

                if (items[position].IsDetailBased || (items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega"))
                {
                    if (items[position].IsDetailBased)
                    {
                        vh.SecondaryFieldText.Visibility = ViewStates.Visible;
                        int cnt = 0;
                        var hasIndents = items[position].IsIndent != null;
                        var listdetails = items[position].Details.ToList();
                        listdetails = listdetails.Where(s => !string.IsNullOrEmpty(s)).ToList();
                        if (items[position].Details != null)
                        {

                            var longestStringcount = listdetails.Aggregate("", (max, cur) => max?.Length > cur?.Length ? max : cur).Length;
                            var indentedText = string.Empty;


                            for (int i = 0; i < items[position].Details.Count(); i++)
                            {
                                if (!string.IsNullOrEmpty(items[position].Details[i]) && !hasIndents)
                                {
                                    if (cnt < 3)
                                    {
                                        cnt += 1;
                                        if (i == 0)
                                        {
                                            vh.SecondaryFieldText.Text = "• " + items[position].Details[i];
                                        }
                                        else
                                        {
                                            vh.SecondaryFieldText.Text += "\n• " + items[position].Details[i];
                                            //if (items[position].Details[i].Length > 11)
                                            //{
                                            //    indentedText += "\n• " + items[position].Details[i].Substring(0, 7) + "...";
                                            //}
                                            //else
                                            //{
                                            //    indentedText += "\n• " + items[position].Details[i] + WhiteSpaceExtender(10, items[position].Details[i], items[position].Details[1]);

                                            //}
                                            //vh.SecondaryFieldText.Text = indentedText;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (!string.IsNullOrEmpty(items[position].Details[i]) && hasIndents)
                                {
                                    if (items[position].IsIndent[i])
                                    {
                                        if (cnt < 3)
                                        {
                                            cnt += 1;
                                            if (i == 0)
                                            {
                                                //vh.SecondaryFieldText.Text = createIndentedText("◦ " + items[position].Details[i], 0).ToString();
                                                //vh.SecondaryFieldText.Text = (indentedText).ToString();

                                            }
                                            else
                                            {
                                                //vh.SecondaryFieldText.Text += (createIndentedText("\n◦ " + items[position].Details[i], 0).ToString());
                                                if (items[position].Details[i].Length > 11)
                                                {
                                                    indentedText += "\n\t\t\t\t◦ " + items[position].Details[i].Substring(0, 7) + "...";
                                                }
                                                else
                                                {
                                                    indentedText += "\n\t\t\t\t◦ " + items[position].Details[i] + WhiteSpaceExtender(10, items[position].Details[i], items[position].Details[1]);

                                                }
                                                vh.SecondaryFieldText.Text = indentedText;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (cnt < 3)
                                        {
                                            cnt += 1;
                                            if (i == 0)
                                            {
                                                // vh.SecondaryFieldText.Text = "• " + items[position].Details[i];
                                                indentedText = "• " + items[position].Details[i];
                                                vh.SecondaryFieldText.Text = indentedText;
                                            }
                                            else
                                            {
                                                vh.SecondaryFieldText.Text += "\n• " + items[position].Details[i];
                                                //    indentedText += "\n• " + items[position].Details[i] + WhiteSpaceExtender(longestStringcount, items[position].Details[i], items[position].Details[0]);
                                                //vh.SecondaryFieldText.Text = indentedText;
                                                //if (items[position].Details[i].Length > 11)
                                                //{
                                                //    indentedText += "\n• " + items[position].Details[i].Substring(0, 7) + "...";
                                                //}
                                                //else
                                                //{
                                                //    indentedText += "\n• " + items[position].Details[i] + WhiteSpaceExtender(10, items[position].Details[i], items[position].Details[1]);

                                                //}
                                                //vh.SecondaryFieldText.Text = indentedText;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }
                                }
                            }

                            //else {
                            //    vh.SecondaryFieldText.TextFormatted = ConvertToBulletList(listdetails, items[position].IsIndent);
                            //}
                        }
                    }
                    else
                    {
                        var section = items[position].Sections;
                    }


                    vh.lblTokViewMore.Visibility = ViewStates.Visible;
                    if (vh.SecondaryFieldText.Text.Length > 30)
                    {
                        vh.lblTokViewMore.Text = "View More";
                    }
                    else if (items[position].IsDetailBased)
                    {
                        int cnt = 0;

                        if (items[position].Details != null)
                        {
                            cnt = items[position].Details.Where(x => (!string.IsNullOrEmpty(x))).ToList().Count();
                        }

                        if (cnt == 0)
                        {
                            vh.lblTokViewMore.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            vh.lblTokViewMore.Text = "View " + cnt + " Details";
                        }

                    }
                    else if (items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega")
                    {
                        if (items[position].Sections != null)
                        {
                            vh.lblTokViewMore.Text = "View " + items[position].Sections.Count() + " Sections";
                        }
                        else
                        {
                            vh.lblTokViewMore.Visibility = ViewStates.Gone;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(vh.SecondaryFieldText.Text) && string.IsNullOrEmpty(items[position].EnglishPrimaryFieldText))
                    {
                        vh.SecondaryFieldText.Visibility = ViewStates.Visible;
                    }

                    if (vh.SecondaryFieldText.Text.Length > 30)
                    {
                        vh.lblTokViewMore.Visibility = ViewStates.Visible;
                        vh.lblTokViewMore.Text = "View More";
                    }
                    else
                    {
                        vh.lblTokViewMore.Visibility = ViewStates.Gone;
                    }
                }

                //vh.UserPhoto.Click -= onImageUsernameClick;
                //vh.UserPhoto.Click += onImageUsernameClick;
                vh.UserPhoto.ContentDescription = position.ToString();
                vh.UserPhoto.SetOnTouchListener(this);


                vh.UserDisplayName.ContentDescription = position.ToString();
                vh.UserDisplayName.Click -= onImageUsernameClick;
                vh.UserDisplayName.Click += onImageUsernameClick;

                vh.Category.Tag = 0;
                vh.Category.ContentDescription = items[position].CategoryId;
                vh.Category.Click += OnTokButtonClick;

                //vh.TokGroup.Text = items[position].TokGroup;
                //vh.TokGroup.Tag = 1;
                //vh.TokGroup.ContentDescription = items[position].TokGroup;
                //vh.TokGroup.Click += OnTokButtonClick;

                vh.TokType.Tag = 2;
                vh.TokType.ContentDescription = items[position].TokTypeId;
                vh.TokType.Click += OnTokButtonClick;

                vh.gridBackground.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Gone;
            }

            vh.ItemView.ContentDescription = position + "";
            vh.ItemView.SetOnLongClickListener(this);
        }
        private void chkChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var checkBox = sender as AppCompatCheckBox;
            int position = 0;
            try { position = (int)checkBox.Tag; } catch { position = int.Parse((string)checkBox.Tag); }

            if (items != null)
            {
                if (items[position].UserId == Settings.GetUserModel().UserId)
                {
                    items[position].isCheck = checkBox.Checked;

                    var result = itemsCheckedList.FirstOrDefault(c => c.Id == items[position].Id);
                    if (result != null) //If found
                    {
                        int ndx = itemsCheckedList.IndexOf(result);
                        itemsCheckedList.Remove(result);
                    }
                    else
                    {
                        if (checkBox.Checked) //Add checker to be sure
                        {
                            itemsCheckedList.Add(items[position]);
                        }
                    }
                }
                else
                {
                    (sender as AppCompatCheckBox).Checked = false;
                    Toast.MakeText(context, Resource.String.created_by_different_user, ToastLength.Long).Show();
                }
            }
        }
        private void setTextColor(Color color)
        {
            vh.UserDisplayName.SetTextColor(color);
            vh.PrimaryFieldText.SetTextColor(color);
            vh.Category.SetTextColor(color);
            //vh.TokGroup.SetTextColor(color);
            vh.TokType.SetTextColor(color);
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
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.customtile_row, parent, false);

            vh = new CustomHolder(itemView, OnClick);
            return vh;
        }
        #endregion
        //Helps in aligning sentences on details
        private string WhiteSpaceExtender(int longStringCOunt, string stringToExtend, string firstDetail)
        {
            string whites = string.Empty;
            if (firstDetail.Length > stringToExtend.Length)
            {
                var lengthtobeadded = longStringCOunt - stringToExtend.Length;
                for (int i = 0; i < lengthtobeadded; i++)
                {
                    whites += "  ";
                }
            }
            else
            {
                var lengthtobeadded = longStringCOunt - stringToExtend.Length;
                for (int i = 0; i < lengthtobeadded; i++)
                {
                    whites += "   ";
                }
            }

            return whites;
        }


        private SpannableString createIndentedText(string text, int startInt)
        {
            SpannableString result = new SpannableString(text);
            result.SetSpan(new BulletSpan(40), startInt, text.Length, SpanTypes.InclusiveExclusive);
            return result;
        }

        private ISpannable ConvertToBulletList(List<string> detail, List<bool> isindent)
        {
            string bulletList = string.Empty;
            var StringBuilder = new SpannableStringBuilder(detail[0] + "\n");
            for (int i = 1; i < detail.Count; i++)
            {
                if (isindent[i])
                {
                    var spannableString = new SpannableString("◦ " + detail[i] + "\n");
                    spannableString.SetSpan(new BulletSpan(40), 0, spannableString.Length(), SpanTypes.InclusiveExclusive);
                    StringBuilder.Append(spannableString);
                }
                else
                {
                    var spannableString = new SpannableString("• " + detail[i] + "\n");
                    spannableString.SetSpan(new BulletSpan(0), 0, spannableString.Length(), SpanTypes.InclusiveExclusive);
                    StringBuilder.Append(spannableString);
                }
            }

            return StringBuilder;
        }
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
                    var parentView = (ViewGroup)((v.Parent).Parent).Parent;
                    var layoutToast = parentView.FindViewById<LinearLayout>(Resource.Id.linearToast);
                    layoutToast.Visibility = ViewStates.Visible;


                    var handler = new Handler();
                    handler.PostDelayed(() =>
                    {
                        v.SetBackgroundColor(Color.Transparent);
                        layoutToast.Visibility = ViewStates.Gone;
                    }, 1000);

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

        public bool OnLongClick(View v)
        {
            int position = int.Parse((string)v.ContentDescription);
            //try { position = (int)v.Tag; } catch { position = int.Parse((string)v.Tag); }

            if (!string.IsNullOrEmpty(items[position].GroupId))
            {
                View labelview = (v.Tag as View); //This is use in order to show the menu in this View's location
                PopupMenu menu = new PopupMenu(ClassGroupActivity.Instance, labelview);
                // Call inflate directly on the menu:
                menu.Inflate(Resource.Menu.delete_menu);

                // A menu item was clicked:
                menu.MenuItemClick += async (s1, arg1) => {
                    switch (arg1.Item.TitleFormatted.ToString().ToLower())
                    {
                        case "delete":
                            //YearbookTok model = items[position];
                            //model.GroupId = "";
                            //ClassGroupActivity.Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);
                            //ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;

                            //var result = await ClassService.Instance.UpdateClassToksAsync(model);

                            //ClassGroupActivity.Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                            //ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;

                            //var objBuilder = new AlertDialog.Builder(ClassGroupActivity.Instance);
                            //objBuilder.SetTitle("");
                            //objBuilder.SetMessage(result.ResultEnum.ToString());
                            //objBuilder.SetCancelable(false);

                            //AlertDialog objDialog = objBuilder.Create();
                            //objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                            //{
                            //    if (result.ResultEnum == Shared.Helpers.Result.Success)
                            //    {
                            //        items.RemoveAt(position);
                            //        NotifyDataSetChanged();
                            //    }
                            //});
                            //objDialog.Show();
                            //objDialog.SetCanceledOnTouchOutside(false);
                            break;
                    }
                };

                // Menu was dismissed:
                menu.DismissEvent += (s2, arg2) => {
                    //Console.WriteLine("menu dismissed");
                };

                menu.Show();
            }
            return true;
        }

        #endregion
    }

}