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
    public class AddExistingClassTokDataAdapter : RecyclerView.Adapter
    {
        #region Members/Properties
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public event EventHandler<View.TouchEventArgs> TouchHandler;
        public List<ClassTokModel> items;
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
        public AddExistingClassTokDataAdapter(List<ClassTokModel> _items, EventHandler<View.TouchEventArgs> OnGridBackgroundTouched) //If caller is Activity or Fragment
        {
            items = _items;
            TouchHandler = OnGridBackgroundTouched;
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<ClassTokModel> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeChanged(position, listUpdate.Count);
        }

        public void AddItem(ClassTokModel item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void RemoveItem(ClassTokModel item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        public TokViewHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        public int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as TokViewHolder;
            Resources res = Application.Context.Resources;

            var font = Typeface.CreateFromAsset(assetManager, "fa_solid_900.otf");

            bool isCountry = true;
            string flagImg = "";
            Stream sr = null;

            try
            {
                if (items[position].UserId == Settings.GetTokketUser().Id)
                {
                    items[position].UserCountry = Settings.GetTokketUser().Country;
                    items[position].UserState = Settings.GetTokketUser().State;
                }

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

                            flagImg = CountryTool.GetCountryFlagJPG1x1(items[position].UserState);
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
            catch (Exception)
            {

                isCountry = false;
                flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
            }


            Bitmap bitmapFlag = BitmapFactory.DecodeStream(sr);
            var ssbName = new SpannableStringBuilder(items[position].PrimaryFieldText);
            var spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);

            vh.txtLinkImage.Typeface = font;
            vh.txtLink.Typeface = font;

            vh.txtLinkImage.Tag = position;
            vh.txtLinkImage.ContentDescription = "link";

            vh.txtLink.Tag = position;
            vh.txtLink.ContentDescription = "link";

            if (string.IsNullOrEmpty(items[position].TokSharePk))
            {
                vh.SharedMark.Visibility = ViewStates.Gone;
                vh.SharedMarkTxt.Visibility = ViewStates.Gone;
            }

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

                var userPhotoCache = MainActivity.Instance.cacheUserPhoto;
                if (!string.IsNullOrEmpty(userPhotoCache) && items[position].UserId == Settings.GetTokketUser().Id) //load the cached image
                {
                    vh.TokImgUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                }
                else
                {
                    Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.TokImgUserPhoto);
                }

                vh.TokImgUserPhoto.ContentDescription = position.ToString();

                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        vh.TokUserTitleImg.Text = items[position].SubaccountName; //Settings.GetTokketUser().SubaccountName;
                    }
                    else
                    {
                        if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().CurrentHandle != null)
                        {
                            vh.TokUserTitleImg.Text = items[position].CurrentHandle; //Settings.GetTokketUser().TitleId;
                        }
                    }
                }
                else
                {
                    if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().CurrentHandle != null)
                    {
                        vh.TokUserTitleImg.Text = items[position].CurrentHandle; //Settings.GetTokketUser().TitleId;
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

                Glide.With(itemView).Load(items[position].StickerImage).Into(vh.TileStickerImg);
                if (string.IsNullOrEmpty(items[position].StickerImage))
                {
                    vh.TileStickerImg.Visibility = ViewStates.Gone;
                }

                vh.ImgUserDisplayName.Text = items[position].UserDisplayName;
                vh.ImgUserDisplayName.ContentDescription = position.ToString();
                vh.ImgUserDisplayName.Click -= onImageUsernameClick;
                vh.ImgUserDisplayName.Click += onImageUsernameClick;

                //vh.TokImgPrimaryFieldText.Text = items[position].PrimaryFieldText;
                vh.TokImgPrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);

                vh.TokImgCategory.Text = items[position].Category;
                vh.TokImgTokType.Text = items[position].TokType;

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

                //vh.TokImgTokGroup.Text = items[position].TokGroup;
                //vh.TokImgTokGroup.Tag = 1;
                //vh.TokImgTokGroup.ContentDescription = items[position].TokGroup;
                //vh.TokImgTokGroup.Click += OnTokButtonClick;

                vh.TokImgTokType.Tag = 2;
                vh.TokImgTokType.ContentDescription = items[position].TokTypeId;

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

                var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
                if (!string.IsNullOrEmpty(cacheUserPhoto) && items[position].UserId == Settings.GetTokketUser().Id) //load the cached image
                {
                    vh.UserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                }
                else
                {
                    Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.UserPhoto);
                }

                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        vh.TokUserTitle.Text = items[position].SubaccountName; //Settings.GetTokketUser().SubaccountName;
                    }
                    else
                    {
                        if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().CurrentHandle != null)
                        {
                            vh.TokUserTitle.Text = items[position].CurrentHandle; //Settings.GetTokketUser().TitleId;
                        }
                    }
                }
                else
                {
                    if (Settings.GetTokketUser().TitleEnabled == true && Settings.GetTokketUser().CurrentHandle != null)
                    {
                        vh.TokUserTitle.Text = items[position].CurrentHandle; //Settings.GetTokketUser().TitleId;
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
                        var listdetails = items[position]?.Details?.ToList();
                        if (listdetails != null)
                        {
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


                vh.UserDisplayName.ContentDescription = position.ToString();
                vh.UserDisplayName.Click -= onImageUsernameClick;
                vh.UserDisplayName.Click += onImageUsernameClick;

                vh.Category.Tag = 0;
                vh.Category.ContentDescription = items[position].CategoryId;

                //vh.TokGroup.Text = items[position].TokGroup;
                //vh.TokGroup.Tag = 1;
                //vh.TokGroup.ContentDescription = items[position].TokGroup;
                //vh.TokGroup.Click += OnTokButtonClick;

                vh.TokType.Tag = 2;
                vh.TokType.ContentDescription = items[position].TokTypeId;

                vh.gridBackground.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Gone;
            }

            vh.ItemView.ContentDescription = position + "";
            if (!string.IsNullOrEmpty(items[position].TokShare))
            {
                InitShareData(items[position].PrimaryFieldText, items[position].TokShare, items[position].TokSharePk);
            }

            vh.ItemView.Tag = position;
            vh.ItemView.Touch -= TouchHandler;
            vh.ItemView.Touch += TouchHandler;

            if (selectedPosition == position)
            {
                vh.ItemView.SetBackgroundColor(Color.Yellow);
            }
            else if (selectedPosition != position)
            {
                vh.ItemView.SetBackgroundColor(Color.Transparent);
            }
        }

        private async void InitShareData(string primary, string id, string pk)
        {
            try
            {
                var getoriginalData = await ClassService.Instance.GetClassTokAsync(id, pk);// classtoks_fragment.Instance.ClassTokCollection.Where(item => item.Id == classTokModel.TokShare).FirstOrDefault(); //
               // var test = await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.ServicesDB.ClassTokServiceDB>().GetClassTok(id, pk);

                var ssbName = new SpannableStringBuilder($"{getoriginalData.PrimaryFieldText}");
                var spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
                vh.PrimaryFieldText.SetMaxLines(int.MaxValue);
                vh.TokImgPrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);
                vh.PrimaryFieldText.Text = ssbName.ToString();
                vh.PrimaryFieldText.SetTypeface(null, TypefaceStyle.Italic);
                vh.PrimaryFieldText.SetTextColor(Color.Blue);
                var listed = new List<string>() { primary + "\n", vh.SecondaryFieldText.Text };
                vh.SecondaryFieldText.Text = string.Join("", listed.ToArray());
            }
            catch (Exception ex) { }

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
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.classtok_tile_row, parent, false);

            vh = new TokViewHolder(itemView, OnClick);
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

        #endregion
    }
}