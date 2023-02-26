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
using System.Threading.Tasks;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Resource.Drawable;
using AndroidX.Core.Content;
using System.Net;
using Android.Util;
using Android.Text.Util;
using static Android.App.ActionBar;
using AndroidX.CoordinatorLayout.Widget;

namespace Tokket.Android.Adapters
{
    public class ClassTokDataAdapter : RecyclerView.Adapter, View.IOnTouchListener, View.IOnLongClickListener
    {
        #region Members/Properties
        Context context;
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public List<ClassTokModel> items;
        List<string> Colors = new List<string>() {
               "#d32f2f", "#C2185B", "#7B1FA2", "#512DA8",
               "#303F9F", "#1976D2", "#0288D1", "#0097A7",
               "#00796B", "#388E3C", "#689F38", "#AFB42B",
               "#FBC02D", "#FFA000", "#F57C00", "#E64A19"
               };
        List<string> randomcolors = new List<string>();
        bool isBackgroundColored = false;
        public override int ItemCount => items.Count;
        View itemView;
        #endregion

        #region Constructor
        public ClassTokDataAdapter(Context cntxt, List<ClassTokModel> _items, List<Tokmoji> _listTokMoji) //If caller is Activity or Fragment
        {
            context = cntxt;
            items = _items;

            if (_listTokMoji == null)
            {
                _listTokMoji = new List<Tokmoji>();
            }

            if (_listTokMoji.Count > 0)
            {
                SpannableHelper.ListTokMoji = _listTokMoji;
            }
            randomcolors = Colors.Shuffle().ToList();
        }
        #endregion

        #region Abstract Methods
        public void UpdateItems(List<ClassTokModel> listUpdate, int position)
        {
            items.AddRange(listUpdate);
            NotifyItemRangeInserted(position, items.Count - 1);
            //NotifyItemRangeChanged(position, items.Count);//listUpdate.Count);
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
        bool isCountry = true;
        bool _isPointSymbol = false;
        Bitmap bitmapFlag;
        string flagImg = "";
        public TokViewHolder vh;
        AssetManager assetManager = Application.Context.Assets;
        string viewMoreExt = "";
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as TokViewHolder;

            items[position].IsMasterCopy = !items[position].HasReactions && !items[position].HasComments;

            viewMoreExt = "";
            if (items[position].IsMasterCopy)
            {
                viewMoreExt = " (Master Copy)";
            }

            if (items[position].TokGroup.ToLower() == "pic")
            {
                vh.gridTokGroupPic.Visibility = ViewStates.Visible;
                vh.gridTokImage.Visibility = ViewStates.Invisible;
                vh.gridBackground.Visibility = ViewStates.Invisible;
                loadTokGroupPic(position);
                return;
            }
            else
            {
                vh.gridTokGroupPic.Visibility = ViewStates.Invisible;
            }

            Resources res = Application.Context.Resources;

            var font = Typeface.CreateFromAsset(assetManager, "fa_solid_900.otf");

            bool isTileImage = false;
            bool isWithFlag = false;
            bool isPointSymbol = false;
            isCountry = true;
            flagImg = "";
            Stream sr = null;

            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                isBackgroundColored = true;
            }

            if (items[position].UserId == Settings.GetTokketUser().Id)
            {
                if (items[position].UserId == Settings.GetTokketUser().Id)
                {
                    items[position].UserCountry = Settings.GetTokketUser().Country;
                    items[position].UserState = Settings.GetTokketUser().State;
                }

                if (Settings.GetTokketUser().IsPointsSymbolEnabled == false)
                {
                    long? userpoints;
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        userpoints = Settings.GetTokketSubaccount().Points;
                    }
                    else
                    {
                        userpoints = Settings.GetTokketUser().Points;
                    }

                    PointsSymbolModel pointResult = PointsSymbolsHelper.GetPatchExactResult(userpoints);

                    flagImg = pointResult.Image;
                    isCountry = false;
                    isWithFlag = true;
                    _isPointSymbol = false;
                }
                else {

                    long? userpoints;
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        userpoints = Settings.GetTokketSubaccount().Points;
                    }
                    else
                    {
                        userpoints = Settings.GetTokketUser().Points;
                    }

                    PointsSymbolModel pointResult = PointsSymbolsHelper.GetPatchExactResult(userpoints);

                    flagImg = pointResult.Image;
                    isCountry = false;
                    isWithFlag = true;
                    isPointSymbol = true;
                    _isPointSymbol = true;
                }
            }

            if (!isWithFlag)
            {
                try
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
            } else if (!isPointSymbol) {
                try
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
            }

            bitmapFlag = BitmapFactory.DecodeStream(sr);
            string concat = items[position].PrimaryFieldText;
            if (items[position].PrimaryFieldText.Length >= 25) {
                concat = items[position].PrimaryFieldText.Substring(0, 24) +"...";
            };

            var ssbName = new SpannableStringBuilder(concat);
            var spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
            
            if (string.IsNullOrEmpty(items[position].TokSharePk))
            {
                vh.SharedMark.Visibility = ViewStates.Gone;
                vh.SharedMarkTxt.Visibility = ViewStates.Gone;
            }

            ResetTileBackToOrigHeight();

            if (!string.IsNullOrEmpty(items[position].Image) || !string.IsNullOrEmpty(items[position].SecondaryImage))
            {
                isTileImage = true;
                vh.imgTokGroupImage.SetImageDrawable(GetTokGroupDrawable(items[position].TokGroup));

                vh.ItemView.Tag = vh.TokImgPrimaryFieldText;
                string tokimg = items[position].ThumbnailImage;

                if (!string.IsNullOrEmpty(items[position].ThumbnailImage))
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(items[position].ThumbnailImage);
                    request.Method = "HEAD";

                    bool exists;
                    try
                    {
                        request.GetResponse();
                        exists = true;
                    }
                    catch
                    {
                        exists = false;
                        tokimg = items[position].Image; //If thumbnail image failed then use the primary image
                    }
                }

                string secondaryImage = "";
                if (!string.IsNullOrEmpty(items[position].SecondaryImage) && string.IsNullOrEmpty(tokimg)) {
                    secondaryImage = items[position].SecondaryImage;
                    var GListener = new ClassTokImgListener();
                    GListener.position = position;
                    GListener.classTokDataAdapter = this;
                    Glide.With(itemView).Load(items[position].SecondaryImage).Listener(GListener).Into(vh.TokImgMain);
                }

                if (URLUtil.IsValidUrl(tokimg))
                {
                    if (tokimg.EndsWith(".png") || tokimg.EndsWith(".jpg"))
                    {

                    }
                    else
                    {
                        tokimg = tokimg + ".jpg";
                    }

                    //Glide.With(itemView).Load(tokimg).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image).FitCenter()).Into(vh.TokImgMain);

                    var GListener = new ClassTokImgListener();
                    GListener.position = position;
                    GListener.classTokDataAdapter = this;
                    Glide.With(itemView).Load(tokimg).Listener(GListener).Into(vh.TokImgMain);
                }
                else
                {
                    tokimg = tokimg.Replace("data:image/jpeg;base64,", "");
                    byte[] imageDetailBytes = Convert.FromBase64String(tokimg);
                    vh.TokImgMain.SetImageBitmap((BitmapFactory.DecodeByteArray(imageDetailBytes, 0, imageDetailBytes.Length)));
                }

                var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
                if (!string.IsNullOrEmpty(cacheUserPhoto) && items[position].UserId == Settings.GetTokketUser().Id) //load the cached image
                {
                    vh.TokImgUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                    /* var userPhotoByte = Convert.FromBase64String(cacheUserPhoto);
                     vh.TokImgUserPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(userPhotoByte, 0, userPhotoByte.Length)));*/
                }
                else
                {
                    Glide.With(itemView).Load(items[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.TokImgUserPhoto);
                }
                
                vh.TokImgUserPhoto.ContentDescription = position.ToString();
                vh.TokImgUserPhoto.SetOnTouchListener(this);
                
                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        vh.TokUserTitleImg.Text = items[position].SubaccountName; //Settings.GetTokketUser().SubaccountName;
                    }
                }

                if (string.IsNullOrEmpty(vh.TokUserTitleImg.Text))
                {
                    vh.TokUserTitleImg.Visibility = ViewStates.Gone;
                }

                if (isCountry )
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
                tokimagedrawable.SetStroke(GetStroke(items[position].TokGroup), Color.ParseColor(randomcolors[ndx]));
                vh.gridTokImage.SetPadding(GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup));

                if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
                {
                    tokimagedrawable.SetColor(ContextCompat.GetColor(context, Resource.Color.DIM_GREY));
                }
                else
                {
                    tokimagedrawable.SetColor(Color.White);
                }

                vh.TokImgMain.ContentDescription = position.ToString();

                vh.TokImgCategory.Tag = 0;
                vh.TokImgCategory.ContentDescription = items[position].CategoryId;
                vh.TokImgCategory.Click += OnTokButtonClick;

                vh.lblTokImgReferenceId.Text = items[position].ReferenceId;

                //vh.TokImgTokGroup.Text = items[position].TokGroup;
                //vh.TokImgTokGroup.Tag = 1;
                //vh.TokImgTokGroup.ContentDescription = items[position].TokGroup;
                //vh.TokImgTokGroup.Click += OnTokButtonClick;

                vh.TokImgTokType.Tag = 2;
                vh.TokImgTokType.ContentDescription = items[position].TokTypeId;
                vh.TokImgTokType.Click += OnTokButtonClick;

                vh.gridTokImage.Visibility = ViewStates.Visible;
                vh.gridBackground.Visibility = ViewStates.Gone;
                AddDetailViewImage(items, position, isBackgroundColored);
            }
            else
            {
                loadNonImageClassToks(position);
            }

            string dateCreated = items[position].RelativeTime;
            if (!dateCreated.Contains("h") || !dateCreated.Contains("m")) {
                dateCreated = items[position].CreatedTime.ToShortDateString();
            }
            if (string.IsNullOrEmpty(items[position].RelativeTime))
                dateCreated = items[position].CreatedTime.ToString();

            vh.txtDateCreated.Text = dateCreated;
            vh.txtDateCreatedImg.Text = dateCreated;

            if (isBackgroundColored) {
                vh.txtDateCreated.SetTextColor(Color.White);
                vh.txtDateCreatedImg.SetTextColor(Color.White);
                setImageTextColor(Color.White);
            }
            else
            {
                vh.txtDateCreated.SetTextColor(Color.Black);
                vh.txtDateCreatedImg.SetTextColor(Color.Black);
            }

            if (dateCreated.ToString().Length > 4)
            {
                vh.txtDateCreated.SetTextSize(ComplexUnitType.Sp,8.5f);
                vh.txtDateCreatedImg.SetTextSize(ComplexUnitType.Sp,8.5f);
            }
            vh.ItemView.ContentDescription = position + "";

            vh.txtLinkImage.Typeface = font;
            vh.txtLink.Typeface = font;

            vh.txtLinkImage.Tag = position;
            vh.txtLinkImage.ContentDescription = "link_image";
            vh.txtLinkImage.SetOnTouchListener(this);

            vh.txtLink.Tag = position;
            vh.txtLink.ContentDescription = "link";
            vh.txtLink.SetOnTouchListener(this);
            if (isBackgroundColored)
            {
                setTextViewDrawableColor(vh.txtLinkImage, Color.White);
                setTextViewDrawableColor(vh.txtLink, Color.White);
                vh.txtLink.SetTextColor(Color.White);
                vh.txtLinkImage.SetTextColor(Color.White);
            }
            else
            {
                if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
                {
                    setTextViewDrawableColor(vh.txtLinkImage, Color.White);
                    setTextViewDrawableColor(vh.txtLink, Color.White);
                    vh.txtLink.SetTextColor(Color.White);
                    vh.txtLinkImage.SetTextColor(Color.White);
                }
            }
            LoadReactionViewModel(items[position]);
            if (!string.IsNullOrEmpty(items[position].TokShare) || !string.IsNullOrEmpty(items[position].TokSharePk)) {
                InitShareData(items[position].PrimaryFieldText,items[position].SharedTok);
            }
            TestDBClientApiCall(items[position].Id, items[position].PartitionKey);
            //  MainThread.BeginInvokeOnMainThread(async () => {await LoadReactionViewModel(items[position]);});
            // Task.WaitAll(LoadReactionViewModel(items[position]));

            // vh.BtnTokInfoEyeIcon.Typeface = font;

            SetHandle(position);

            vh.ItemView.SetOnLongClickListener(this);
        }

        private void SetHandle(int position)
        {
            if (!string.IsNullOrEmpty(items[position].HandleImage))
            {
                Glide.With(itemView).Load(items[position].HandleImage).Into(vh.imageViewHandleImage);
                Glide.With(itemView).Load(items[position].HandleImage).Into(vh.imageViewHandle);
                vh.TokUserTitleImg.Text = "";
                vh.TokUserTitle.Text = "";
            }
            else if (!string.IsNullOrEmpty(items[position].HandleColor))
            {
                //Set handle color
                vh.linearRowUsernameImg.SetBackgroundColor(Color.ParseColor(items[position].HandleColor));
                vh.linearRow1.SetBackgroundColor(Color.ParseColor(items[position].HandleColor));
                vh.TokUserTitleImg.Text = "";
                vh.TokUserTitle.Text = "";
            }

            //Set handle
            //TODO need to get sample data
            if (items[position].HandlePosition == HandlePosition.OptionA) //Let's say user selected option A
            {
                vh.TokUserTitleImg.Visibility = ViewStates.Visible;
                vh.TokUserTitleImg.Text = items[position].CurrentHandle;

                vh.TokUserTitle.Visibility = ViewStates.Visible;
                vh.TokUserTitle.Text = items[position].CurrentHandle;
            }
            else if (items[position].HandlePosition == HandlePosition.OptionB) //Let's say user selected option B
            {
                vh.ImgUserDisplayName.Visibility = ViewStates.Visible;
                vh.ImgUserDisplayName.Text = items[position].CurrentHandle;

                vh.UserDisplayName.Visibility = ViewStates.Visible;
                vh.UserDisplayName.Text = items[position].CurrentHandle;
            }
            else if (items[position].HandlePosition == HandlePosition.OptionC) //Let's say user selected option C
            {
                vh.ImgUserDisplayName.Visibility = ViewStates.Invisible;
                vh.UserDisplayName.Visibility = ViewStates.Invisible;

                vh.TokUserTitleImg.Visibility = ViewStates.Visible;
                vh.TokUserTitleImg.Text = items[position].CurrentHandle;

                vh.TokUserTitle.Visibility = ViewStates.Visible;
                vh.TokUserTitle.Text = items[position].CurrentHandle;
            }
            else if (items[position].HandlePosition == HandlePosition.OptionD) //Let's say user selected option D
            {
                vh.TokUserTitleImg.Visibility = ViewStates.Gone;
                vh.TokUserTitle.Visibility = ViewStates.Gone;

                //Set layout parameters to matchparent to display it at the center
                vh.ImgUserDisplayName.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                vh.ImgUserDisplayName.Text = items[position].CurrentHandle;

                vh.UserDisplayName.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                vh.UserDisplayName.Text = items[position].CurrentHandle;
            }
        }

        private void setTextViewDrawableColor(TextView textView, Color color)
        {
            foreach (Drawable drawable in textView.GetCompoundDrawables())
            {
                if (drawable != null)
                {
                    drawable.SetColorFilter(new PorterDuffColorFilter(color, PorterDuff.Mode.SrcIn));
                }
            }
        }

        private void LoadReactionViewModel(ClassTokModel classTok) {
          //  var reactionValue = await ReactionService.Instance.GetReactionsValueAsync(classTok.Id);
            vh.LblTokInfoViewCpunt.Text = "0";
            if (classTok.ViewsModel != null)
            {
                vh.LblTokInfoViewCpunt.Text = (classTok.ViewsModel.TileTapViews + classTok.ViewsModel.TileTapViewsPersonal + classTok.ViewsModel.PageVisitViews).ToString();
            }
            //else {
            
            //}
            //await Task.Delay(500);
        }

        private async void TestDBClientApiCall(string id, string pk) {
         // var test = await Tokket.Shared.IoC.AppContainer.Resolve<Shared.Services.Interfaces.IClassTokService>().GetClassTokAsync<Dictionary<string,object>>(id, pk);

        }

        private void loadTokGroupPic(int position)
        {
            vh.gridTokGroupPic.Tag = position;

            vh.gridTokGroupPic.SetBackgroundResource(Resource.Drawable.tileview_layout);
            vh.gridTokGroupPic.Tag = position;

            int ndx = position % Colors.Count;
            if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            GradientDrawable tokimagedrawable = (GradientDrawable)vh.TokdrawableGroupPic.Background;
            tokimagedrawable.SetStroke(GetStroke(items[position].TokGroup), Color.ParseColor(randomcolors[ndx]));
            vh.TokdrawableGroupPic.SetPadding(GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup));

            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                tokimagedrawable.SetColor(ContextCompat.GetColor(context, Resource.Color.DIM_GREY));
            }
            else
            {
                tokimagedrawable.SetColor(Color.White);
            }

            RequestOptions options = new RequestOptions()
                            .CenterCrop()
                            .Placeholder(Resource.Animation.loader_animation)
                            .Error(Resource.Drawable.no_image)
                            //.InvokeDiskCacheStrategy(DiskCacheStrategy.All)
                            .Override(500);

            var GListener = new ClassTokImgListener();
            GListener.position = position;
            GListener.classTokDataAdapter = this;
            Glide.With(itemView).Load(items[position].Image).Thumbnail(0.05f).Apply(options).Listener(GListener).Into(vh.imageTokGroupPic);
        }

        private int GetStroke(string tokGroup)
        {
            var stroke = context.Resources.GetDimension(Resource.Dimension._2sdp);
            if (tokGroup.ToLower() == "basic")
            {
                context.Resources.GetDimension(Resource.Dimension._2sdp);
            }
            else if (tokGroup.ToLower() == "detail" || tokGroup.ToLower() == "detailed" || tokGroup.ToLower() == "list")
            {
                stroke = context.Resources.GetDimension(Resource.Dimension._6sdp);
            }
            else
            {
                stroke = context.Resources.GetDimension(Resource.Dimension._11sdp);
            }
            return Convert.ToInt32(stroke);
        }

        private int GetPadding(string tokGroup)
        {
            float padding = 0;
            if (tokGroup.ToLower() == "basic")
            {
                padding = context.Resources.GetDimension(Resource.Dimension._5sdp);
            }
            else if (tokGroup.ToLower() == "detail" || tokGroup.ToLower() == "detailed" || tokGroup.ToLower() == "list")
            {
                padding = context.Resources.GetDimension(Resource.Dimension._7sdp);
            }
            else
            {
                padding = context.Resources.GetDimension(Resource.Dimension._13sdp);
            }
            return Convert.ToInt32(padding);
        }

        private Drawable GetTokGroupDrawable(string tokGroup)
        {
            if (tokGroup.ToLower() == "basic")
            {
                return ContextCompat.GetDrawable(context, Resource.Drawable.tile_basic);
            }
            else if (tokGroup.ToLower() == "detail" || tokGroup.ToLower() == "detailed")
            {
                return ContextCompat.GetDrawable(context, Resource.Drawable.tile_detailed);
            }
            else if (tokGroup.ToLower() == "list")
            {
                return ContextCompat.GetDrawable(context, Resource.Drawable.tile_list);
            }
            else if (tokGroup.ToLower() == "q&a")
            {
                return ContextCompat.GetDrawable(context, Resource.Drawable.tile_qa);
            }
            else
            {
                return ContextCompat.GetDrawable(context, Resource.Drawable.tile_mega);
            }
        }

        public void loadNonImageClassToks(int position)
        {
            vh.imgTokGroup.SetImageDrawable(GetTokGroupDrawable(items[position].TokGroup));

            vh.ItemView.Tag = vh.PrimaryFieldText;
            vh.lblTokReferenceId.Text = items[position].ReferenceId;
            //Purple Gem
            if (items.Count > 0)
            {
                if (items[position].HasGemReaction)
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.ImgPurpleGem.Visibility = ViewStates.Invisible;
                }
            }            

            var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
            if (!string.IsNullOrEmpty(cacheUserPhoto) && items[position].UserId == Settings.GetTokketUser().Id) //load the cached image
            {
                vh.UserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                /*var userPhotoByte = Convert.FromBase64String(cacheUserPhoto);
                vh.UserPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(userPhotoByte, 0, userPhotoByte.Length)));*/
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
            }

            if (string.IsNullOrEmpty(vh.TokUserTitle.Text))
            {
                vh.TokUserTitle.Visibility = ViewStates.Gone;
            }

            if (isCountry )
            {
                vh.UserFlag.SetImageBitmap(bitmapFlag);
            }
            else
            {
                Glide.With(itemView).Load(flagImg).Into(vh.UserFlag);
            }

            if (items[position].UserId == Settings.GetTokketUser().Id)
            {
                if (isCountry || !_isPointSymbol)
                {
                    vh.UserFlag.SetImageBitmap(bitmapFlag);
                }
                else
                {
                    Glide.With(itemView).Load(flagImg).Into(vh.UserFlag);
                }
            }

            Glide.With(itemView).Load(items[position].StickerImage).Into(vh.TileSticker);
            if (string.IsNullOrEmpty(items[position].StickerImage))
            {
                vh.TileSticker.Visibility = ViewStates.Gone;
            }

            vh.UserDisplayName.Text = items[position].UserDisplayName;
            string concat = items[position].PrimaryFieldText; //string.Empty;
            /*if (items[position].PrimaryFieldText.Length >= 30)
            {
                concat = items[position].PrimaryFieldText.Substring(0, 30).TrimEnd() + "...";
            }
            else {
                concat = items[position].PrimaryFieldText;
            }*/
            vh.PrimaryFieldText.Text = concat;
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
            //if (ndx == 0 || randomcolors.Count == 0) randomcolors = Colors.Shuffle().ToList();
            GradientDrawable Tokdrawable = (GradientDrawable)vh.Tokdrawable.Background;
            //Tokdrawable.SetColor(Color.ParseColor(randomcolors[ndx]));

            if (items[position].ColorMainHex == "#FFFFFF" || string.IsNullOrEmpty(items[position].ColorMainHex))
            {
                if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
                {
                    Tokdrawable.SetColor(ContextCompat.GetColor(context, Resource.Color.DIM_GREY));
                    Tokdrawable.SetStroke(GetStroke(items[position].TokGroup), Color.ParseColor(randomcolors[ndx]));
                    setTextColor(Color.White);
                    isBackgroundColored = true;
                }
                else
                {
                    Tokdrawable.SetColor(Color.White);
                    Tokdrawable.SetStroke(GetStroke(items[position].TokGroup), Color.ParseColor(randomcolors[ndx]));
                    setTextColor(Color.Black);
                    isBackgroundColored = false;
                }
            }
            else
            {
                Tokdrawable.SetStroke(GetStroke(items[position].TokGroup), Color.ParseColor(items[position].ColorMainHex));
                Tokdrawable.SetColor(Color.ParseColor(items[position].ColorMainHex));
                setTextColor(Color.White);
                isBackgroundColored = true;
            }

            vh.gridBackground.SetPadding(GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup), GetPadding(items[position].TokGroup));

            if (items[position].IsDetailBased || (items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega") || items[position].TokGroup.ToLower() == "q&a")
            {
                if (items[position].IsDetailBased)
                {

                    // AddDetailLoop(items, position); //This is the old code

                    AddDetailView(items,position,isBackgroundColored);

                }
                else
                {
                    vh.DetailLinearContainer.RemoveAllViews();
                    var section = items[position].Sections;
                    if (IsDoubleTile(position))
                    {
                        if ((items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega") || items[position].TokGroup.ToLower() == "q&a")
                        {
                            if (items[position].Sections != null)
                            {
                                for (int i = 0; i < section.Count(); i++)
                                {
                                    string indentedText = string.Empty;
                                        if (vh.DetailLinearContainer.ChildCount > 11)
                                            break;

                                    if (!string.IsNullOrEmpty(section[i].Title))
                                    {
                                        indentedText = "• " + section[i].Title;

                                        vh.DetailLinearContainer.AddView(ItemText(indentedText, isBackgroundColored));
                                    }
                                }
                            }
                        }
                    }

                    if (vh.DetailLinearContainer.ChildCount > 0)
                    {
                        vh.DetailLinearContainer.Visibility = ViewStates.Visible;
                    }
                }


                vh.lblTokViewMore.Visibility = ViewStates.Visible;
               if (items[position].IsDetailBased)
                {
                    int cnt = 0;

                    if (string.IsNullOrEmpty(vh.SecondaryFieldText.Text))
                        vh.SecondaryFieldText.Visibility = ViewStates.Gone;

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
                        vh.lblTokViewMore.Text = "View More and Comments" + viewMoreExt;

                        /*if (items[position].TokGroup.ToLower() == "list")
                        {
                            vh.lblTokViewMore.Text = "View List with " + cnt + " items";
                        }
                        else
                        {
                            vh.lblTokViewMore.Text = "View " + cnt + " Details";
                        }*/
                    }

                }
                else if (items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega")
                {
                    if (items[position].Sections != null)
                    {
                        vh.lblTokViewMore.Text = "View More and Comments" + viewMoreExt;
                        //vh.lblTokViewMore.Text = "View " + items[position].Sections.Count() + " Sections";
                    }
                    else
                    {
                        vh.lblTokViewMore.Visibility = ViewStates.Gone;
                    }
                }
                else if (items[position].TokGroup.ToLower() == "q&a")
                {
                    if (items[position].Sections != null)
                    {
                        vh.lblTokViewMore.Text = "View More and Comments" + viewMoreExt;

                        //vh.lblTokViewMore.Text = "View " + items[position].Sections.Count() + " Q & A";
                    }
                    else
                    {
                        vh.lblTokViewMore.Visibility = ViewStates.Gone;
                    }
                }
                else
                {                    
                    vh.SecondaryFieldText.SetMaxLines(3);
                    if (vh.SecondaryFieldText.Text.Length > 30)
                    {
                        vh.lblTokViewMore.Text = "View More" + viewMoreExt;
                    }
                    else
                    {
                        vh.lblTokViewMore.Text = "View Tok Info" + viewMoreExt;
                    }
                }
            }
            else
            {
                IsDoubleTile(position);

                if (!string.IsNullOrEmpty(vh.SecondaryFieldText.Text) && string.IsNullOrEmpty(items[position].EnglishPrimaryFieldText))
                {
                    vh.SecondaryFieldText.Visibility = ViewStates.Visible;
                    vh.DetailLinearContainer.Visibility = ViewStates.Gone;
                }

                vh.SecondaryFieldText.SetMaxLines(3);

                if (vh.SecondaryFieldText.Text.Length > 30)
                {
                    vh.lblTokViewMore.Visibility = ViewStates.Visible;

                    vh.lblTokViewMore.Text = "View More" + viewMoreExt;
                }
                else
                {
                    vh.lblTokViewMore.Text = "View Tok Info" + viewMoreExt;
                    vh.lblTokViewMore.Visibility = ViewStates.Visible;
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

        private TextView ItemText(string text,bool isBackgroundColored = false)
        {
            var view = new TextView(context) { LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent) };
            //var param = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            if(isBackgroundColored)
                view.SetTextColor(Color.White);
            else
                view.SetTextColor(Color.Black);
            view.TextSize = 13;
            IInputFilter[] fArray = new IInputFilter[1];
            fArray[0] = new InputFilterLengthFilter(40);
            view.Ellipsize = TextUtils.TruncateAt.End;
            view.SetPadding(25, 0,25, 0);
            view.SetMaxLines(1);
            view.AutoLinkMask = MatchOptions.WebUrls;
            view.LinksClickable = true;
            view.Gravity = GravityFlags.Left;
            view.TextAlignment = TextAlignment.Gravity;

            // view.SetFilters(fArray);
            view.Text = text;
            return view;
        }

        private void AddDetailLoop(List<ClassTokModel> items, int position) {
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
                                //else
                                //{
                                //    break;
                                //}
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

        private void AddDetailView(List<ClassTokModel> items, int position,bool isBackgroundColored = false) {
            vh.DetailLinearContainer.RemoveAllViews();
            vh.SecondaryFieldText.Visibility = ViewStates.Gone;
            vh.DetailLinearContainer.Visibility = ViewStates.Visible;
            var hasIndents = items[position].IsIndent != null;
            var listdetails = items[position]?.Details?.ToList();
            if (listdetails != null) {
                listdetails = listdetails.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (items[position].Details != null) {
                    bool isDoubleTile = IsDoubleTile(position);
                   
                    for (int i = 0; i < listdetails.Count(); i++) {
                        string indentedText = string.Empty;
                        if (isDoubleTile)
                        {
                            if (vh.DetailLinearContainer.ChildCount > 11)
                                break;
                        } else
                        {
                            if (vh.DetailLinearContainer.ChildCount > 3)
                                break;
                        }

                        if (!string.IsNullOrEmpty(items[position].Details[i]) && !hasIndents)
                        {
                            indentedText = "• " + items[position].Details[i];
                           
                            //if (items[position].Details[i].Length > 11)
                            //{
                            //    indentedText = "• " + items[position].Details[i].Substring(0, 10) + "...";
                            //}
                            //else
                            //{
                            //    indentedText = "• " + items[position].Details[i];

                            //}
                            vh.DetailLinearContainer.AddView(ItemText(indentedText, isBackgroundColored));
                        }
                        else if (!string.IsNullOrEmpty(items[position].Details[i]) && hasIndents)
                        {
                            indentedText = "◦ " + items[position].Details[i];
                          
                        
                            //if (items[position].Details[i].Length > 11)
                            //{
                            //    indentedText = "◦ " + items[position].Details[i].Substring(0, 10) + "...";
                            //}
                            //else
                            //{
                            //    indentedText = "◦ " + items[position].Details[i];
                            //}
                            vh.DetailLinearContainer.AddView(ItemText(indentedText, isBackgroundColored));
                        }
                        else {
                            //if (items[position].Details[i].Length > 11)
                            //{
                            //    indentedText += "• " + items[position].Details[i].Substring(0, 7) + "...";
                            //}
                            //else
                            //{
                            //    indentedText += "• " + items[position].Details[i];

                            //}
                            //vh.DetailLinearContainer.AddView(ItemText(indentedText));
                        }
                    }
                }

            }
        }

        private void AddDetailViewImage(List<ClassTokModel> items, int position, bool isBackgroundColored = false)
        {
            vh.linearDetailImage.RemoveAllViews();
            vh.linearDetailImage.Visibility = ViewStates.Visible;

            bool isDoubleTile = IsDoubleTile(position, true);
            if (!isDoubleTile)
            {
                return;
            }

            var tokGroup = items[position].TokGroup.ToLower();
            if (tokGroup == "detailed" || tokGroup == "detail" || tokGroup == "list")
            {
                var hasIndents = items[position].IsIndent != null;
                var listdetails = items[position]?.Details?.ToList();
                if (listdetails != null)
                {
                    listdetails = listdetails.Where(s => !string.IsNullOrEmpty(s)).ToList();
                    if (items[position].Details != null)
                    {
                        for (int i = 0; i < listdetails.Count(); i++)
                        {
                            string indentedText = string.Empty;
                            if (isDoubleTile)
                            {
                                if (vh.linearDetailImage.ChildCount > 11)
                                    break;
                            }

                            if (!string.IsNullOrEmpty(items[position].Details[i]) && !hasIndents)
                            {
                                indentedText = "• " + items[position].Details[i];

                                vh.linearDetailImage.AddView(ItemText(indentedText, isBackgroundColored));
                            }
                            else if (!string.IsNullOrEmpty(items[position].Details[i]) && hasIndents)
                            {
                                indentedText = "◦ " + items[position].Details[i];
                                vh.linearDetailImage.AddView(ItemText(indentedText, isBackgroundColored));
                            }
                        }
                    }
                }
            }
            else if ((items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega") || items[position].TokGroup.ToLower() == "q&a")
            {
                if (items[position].Sections != null)
                {
                    for (int i = 0; i < items[position].Sections.Count(); i++)
                    {
                        string indentedText = string.Empty;
                        if (isDoubleTile)
                        {
                            if (vh.linearDetailImage.ChildCount > 11)
                                break;
                        }

                        if (!string.IsNullOrEmpty(items[position].Sections[i].Title))
                        {
                            indentedText = "• " + items[position].Sections[i].Title;

                            vh.linearDetailImage.AddView(ItemText(indentedText, isBackgroundColored));
                        }
                    }
                }
            }
        }

        private bool IsDoubleTile(int position, bool isImage = false)
        {
            bool isDoubleTile = false;
            if (items[position].PrimaryFieldText.Length > 105)
            {
                isDoubleTile = true;
            }
            else
            {
                if (items[position].IsDetailBased)
                {
                    var listdetails = items[position]?.Details?.ToList();
                    if (listdetails != null)
                    {
                        if (isImage)
                        {
                            //Always true if tok is image
                            isDoubleTile = true;
                        }
                        else
                        {
                            listdetails = listdetails.Where(s => !string.IsNullOrEmpty(s)).ToList();
                            if (listdetails.Count > 6)
                            {
                                isDoubleTile = true;
                            }
                            else if (vh.PrimaryFieldText.LineCount > 1)
                            {
                                //Detail with primary field text line count greater than 1
                                isDoubleTile = true;
                            }
                        }
                    }
                }
                else if ((items[position].IsMegaTok == true && items[position].TokGroup.ToLower() == "mega") || items[position].TokGroup.ToLower() == "q&a")
                {
                    if (items[position].Sections != null)
                    {
                        if (items[position].Sections.Length > 6)
                        {
                            isDoubleTile = true;
                        }
                    }
                }
            }

            //Set height does not matter if image or not
            if (isDoubleTile)
            {
                vh.gridBackground.LayoutParameters = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension._360sdp)));
                vh.gridTokImage.LayoutParameters = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension._360sdp)));
            }
            else
            {
                ResetTileBackToOrigHeight();
            }

            return isDoubleTile;
        }

        private void ResetTileBackToOrigHeight()
        {
            vh.gridBackground.LayoutParameters = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension._192sdp)));
            vh.gridTokImage.LayoutParameters = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension._192sdp)));
        }
        private void InitShareData(string primary,string shared)
        {
            try
            {
                var getoriginalData = JsonConvert.DeserializeObject<ClassTokModel>(shared); // classtoks_fragment.Instance.ClassTokCollection.Where(item => item.Id == classTokModel.TokShare).FirstOrDefault(); //
                var ssbName = new SpannableStringBuilder($"{getoriginalData.PrimaryFieldText}");
                var spannableResText = SpannableHelper.AddStickersSpannable(MainActivity.Instance, ssbName);
                vh.PrimaryFieldText.SetMaxLines(int.MaxValue);
                vh.TokImgPrimaryFieldText.SetText(spannableResText, TextView.BufferType.Spannable);
                vh.PrimaryFieldText.Text = ssbName.ToString();
                vh.PrimaryFieldText.SetTypeface(null, TypefaceStyle.Italic);
                vh.PrimaryFieldText.SetTextColor(Color.Blue);
                var listed = new List<string>() { primary + "\n", vh.SecondaryFieldText.Text };
                vh.SecondaryFieldText.Text = string.Join("",listed.ToArray());
                vh.SharedMark.Visibility = ViewStates.Visible;
                vh.SharedMarkTxt.Visibility = ViewStates.Visible;
            }
            catch (Exception ex) { }

        }
     
        private void setTextColor(Color color)
        {
            vh.TokUserTitle.SetTextColor(color);
            vh.UserDisplayName.SetTextColor(color);
            vh.PrimaryFieldText.SetTextColor(color);
            vh.Category.SetTextColor(color);
            //vh.TokGroup.SetTextColor(color);
            vh.TokType.SetTextColor(color);
            vh.EnglishPrimaryFieldText.SetTextColor(color);
            vh.lblTokViewMore.SetTextColor(color);
            vh.SecondaryFieldText.SetTextColor(color);
            vh.LblTokInfoViewCpunt.SetTextColor(color);
            vh.lblTokImgReferenceId.SetTextColor(color);
            vh.lblTokReferenceId.SetTextColor(color);
            vh.BtnTokInfoEyeIcon.BackgroundTintList = ColorStateList.ValueOf(color);
        }

        private void setImageTextColor(Color color)
        {
            vh.ImgUserDisplayName.SetTextColor(color);
            vh.txtLinkImage.SetTextColor(color);
            vh.txtDateCreatedImg.SetTextColor(color);
            vh.SharedMark.SetTextColor(color);
            vh.TokImgPrimaryFieldText.SetTextColor(color);
            vh.TokImgSecondaryFieldText.SetTextColor(color);
            vh.TokImgTokType.SetTextColor(color);
            vh.TokImgCategory.SetTextColor(color);
            vh.lblTokImgReferenceId.SetTextColor(color);
            vh.lblTokReferenceId.SetTextColor(color);
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

            if (!MainActivity.Instance.isOnProfile && ProfileUserActivity.Instance == null)
            {
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
                nextActivity.PutExtra("userid", items[position].UserId);
                MainActivity.Instance.StartActivity(nextActivity);
            }
        
            //}

        }
        void OnTokButtonClick(object sender, EventArgs e)
        {
            string titlepage = "";
            string filter = "";
            string headerpage = headerpage = (sender as TextView).Text;
            int filterTag = Settings.FilterTag;

            if ((int)(sender as TextView).Tag == (int)Toks.Category)
            {
                filterTag = 3;
                titlepage = "Category";
                filter = (sender as TextView).ContentDescription;
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokGroup)
            {
                filterTag = 6;
                titlepage = "Tok Group";
                filter = (sender as TextView).ContentDescription.ToLower();
            }
            else if ((int)(sender as TextView).Tag == (int)Toks.TokType)
            {
                filterTag = 1;
                titlepage = "Tok Type";
                filter = (sender as TextView).ContentDescription;
            }
            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ClassToksActivity));
            nextActivity.PutExtra("titlepage", titlepage);
            nextActivity.PutExtra("filter", filter);
            nextActivity.PutExtra("headerpage", headerpage);
            nextActivity.PutExtra("filterTag", filterTag);
            nextActivity.SetFlags(ActivityFlags.ReorderToFront);
            MainActivity.Instance.StartActivity(nextActivity);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.classtok_tile_row, parent, false);

            vh = new TokViewHolder(itemView, OnClick);
            return vh;
        }
        #endregion


        //Helps in aligning sentences on details
        private string WhiteSpaceExtender(int longStringCOunt,string stringToExtend, string firstDetail) {
            string whites = string.Empty;
            if (firstDetail.Length > stringToExtend.Length) {
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
        private  SpannableString createIndentedText(string text,int startInt)
        {
            SpannableString result = new SpannableString(text);
            result.SetSpan(new BulletSpan(40),startInt, text.Length, SpanTypes.InclusiveExclusive) ;
            return result;
        }

        private ISpannable ConvertToBulletList(List<string> detail, List<bool> isindent) {
            string bulletList = string.Empty;
            var StringBuilder = new SpannableStringBuilder(detail[0]+"\n");
            for (int i = 1; i < detail.Count; i++) {
                if (isindent[i])
                {
                    var spannableString = new SpannableString("◦ " + detail[i]+"\n");
                    spannableString.SetSpan(new BulletSpan(40), 0, spannableString.Length(), SpanTypes.InclusiveExclusive);
                    StringBuilder.Append(spannableString);
                }
                else {
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
                    var parentView = (ViewGroup)(v.Parent).Parent;
                    var txtToast = parentView.FindViewById<TextView>(Resource.Id.txtToastLinkCopied);
                    txtToast.Visibility = ViewStates.Visible;

                    var handler = new Handler();
                    handler.PostDelayed(() =>
                    {
                        v.SetBackgroundColor(Color.Transparent);
                        txtToast.Visibility = ViewStates.Gone;
                    }, 1000);

                    Clipboard.SetTextAsync("").GetAwaiter().GetResult();
                    MainActivity.Instance.RunOnUiThread(async () => await Clipboard.SetTextAsync(JsonConvert.SerializeObject(items[ndx])));
                }
                else if (v.ContentDescription == "link_image")
                {
                    int ndx = 0;
                    try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }
                    var parentView = (ViewGroup)(v.Parent).Parent;
                    var txtToast = parentView.FindViewById<TextView>(Resource.Id.txtToastLinkCopiedImg);
                    txtToast.Visibility = ViewStates.Visible;

                    var handler = new Handler();
                    handler.PostDelayed(() =>
                    {
                        v.SetBackgroundColor(Color.Transparent);
                        txtToast.Visibility = ViewStates.Gone;
                    }, 1000);

                    Clipboard.SetTextAsync("").GetAwaiter().GetResult();
                    MainActivity.Instance.RunOnUiThread(async () => await Clipboard.SetTextAsync(JsonConvert.SerializeObject(items[ndx]))); //items[ndx].Id
                }
                else
                {
                    //When Image of User Photo is clicked.
                    if(!MainActivity.Instance.isOnProfile && ProfileUserActivity.Instance == null)
                    {
                        //Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
                        //nextActivity.PutExtra("userid", items[position].UserId);
                        //MainActivity.Instance.StartActivity(nextActivity);
                              int position = Convert.ToInt32(v.ContentDescription);
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileUserActivity));
                    nextActivity.PutExtra("userid", items[position].UserId);
                    MainActivity.Instance.StartActivity(nextActivity);
                    }

              
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
                            ClassTokModel model = items[position];
                            model.GroupId = "";
                            ClassGroupActivity.Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);
                            ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Visible;

                            var result = await ClassService.Instance.UpdateClassToksAsync(model);

                            ClassGroupActivity.Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                            ClassGroupActivity.Instance.LinearProgress.Visibility = ViewStates.Gone;

                            var objBuilder = new AlertDialog.Builder(ClassGroupActivity.Instance);
                            objBuilder.SetTitle("");
                            objBuilder.SetMessage(result.ResultEnum.ToString());
                            objBuilder.SetCancelable(false);

                            AlertDialog objDialog = objBuilder.Create();
                            objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                            {
                                if (result.ResultEnum == Shared.Helpers.Result.Success)
                                {
                                    items.RemoveAt(position);
                                    NotifyDataSetChanged();
                                }
                            });
                            objDialog.Show();
                            objDialog.SetCanceledOnTouchOutside(false);
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

        class ClassTokImgListener : Java.Lang.Object, IRequestListener
        {
            public int position { get; set; }
            public ClassTokDataAdapter classTokDataAdapter { get; set; }
            public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
            {
                classTokDataAdapter.loadNonImageClassToks(position);
                return false;
            }


            public bool OnResourceReady(Java.Lang.Object resource, Java.Lang.Object model, ITarget target, DataSource dataSource, bool isFirstResource)
            {                
                return false;
            }
        }
    }
}