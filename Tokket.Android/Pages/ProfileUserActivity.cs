using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Core.Widget;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.AppBar;
using ImageViews.Photo;
using Newtonsoft.Json;
using Supercharge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Android.Adapters;
using Tokket.Android.Custom;
using Tokket.Android.Fragments;
using Tokket.Android.Helpers;
using Tokket.Shared.Extensions;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Android.ViewModels;
using Tokket.Core;
using Tokket.Core.Tools;
using Xamarin.Essentials;
using Color = Android.Graphics.Color;
using ServiceAccount = Tokket.Shared.Services;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = " ", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProfileUserActivity : BaseActivity, View.IOnTouchListener
    {
        private int REQUEST_PROFILE_MEMBERSHIP = 10001;
        private float startY;
        float imgScale;
        internal static ProfileUserActivity Instance { get; private set; }
        List<TokModel> tokResult, TokDataList;
        List<Tokmoji> ListTokmojiModel; public List<ClassTokModel> ClassTokList;
        TokDataAdapter tokDataAdapter; TokCardDataAdapter tokcardDataAdapter; string UserId;
        TokModel tokModel; ClassTokModel classTokModel;
        public ClassTokDataAdapter classtokDataAdapter; ClassTokCardDataAdapter classtokcardDataAdapter; 
        GridLayoutManager mLayoutManager;
        public ProfilePageViewModel ProfileVm => App.Locator.ProfilePageVM;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        CancellationTokenSource source;
        private int EDIT_PROFILE_REQUEST_CODE = 1004;
        AssetManager assetManager;
        ReportDialouge Report = null;
        Stream sr = null;
        public bool isOnProfile { get; private set; } = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile_page);

            ProfileToolBar.Visibility = ViewStates.Visible;

            SetSupportActionBar(ProfileToolBar);

            if (SupportActionBar != null)
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();

            UserId = Intent.GetStringExtra("userid");
            ProfileVm.UserId = UserId;
            Instance = this;
            Settings.ActivityInt = (int)ActivityType.ProfileActivity;

            ProfileVm.activity = this;
            ProfileVm.ProfileCoverPhoto = ProfileCoverPhoto;
            ProfileVm.ProfileUserPhoto = ProfileUserPhoto;
            Report = new ReportDialouge(this);
            this.RunOnUiThread(async () => await ProfileVm.Initialize());
            isOnProfile = true;
            ProfileCoverPhoto.LayoutChange += delegate
            {
                //set color status bar
                Window.SetStatusBarColor(ManipulateColor.manipulateColor(ProfileVm.GListenerCover.mColorPalette, 0.32f));

                if (Window.StatusBarColor == 0)
                {
                    Window.SetStatusBarColor(Color.Black);
                }
                
            };

            if (UserId != Settings.GetUserModel().UserId)
            {
                btnEditProfile.Visibility = ViewStates.Gone;
            }
            UrlEditIcon.Visibility = ViewStates.Gone;

            btnEditProfile.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(EditProfileActivity));
                nextActivity.PutExtra("tokketUser", JsonConvert.SerializeObject(ProfileVm.tokketUser));
                nextActivity.PutExtra("bio", ProfileVm.tokketUser.Bio);
                nextActivity.PutExtra("web", ProfileVm.tokketUser.Website);
                nextActivity.PutExtra("displayname", ProfileVm.tokketUser.DisplayName);
                this.StartActivityForResult(nextActivity, EDIT_PROFILE_REQUEST_CODE);
            };

            ProfileUsername.Text = ProfileVm.UserDisplayName;

            if (!string.IsNullOrEmpty(ProfileVm.tokketUser.CurrentHandle))
            {
                UserTitle.Visibility = ViewStates.Visible;
                UserTitle.Text = ProfileVm.tokketUser.CurrentHandle;
            }

            UserDescription.Text = ProfileVm.tokketUser.Bio;
            LinkProfileUrl.Text = ProfileVm.tokketUser.Website;

            if (UserId != Settings.GetUserModel().UserId)
            {
                if (string.IsNullOrEmpty(ProfileVm.tokketUser.Bio))
                {
                    UserDescription.Visibility = ViewStates.Gone;
                }
            }

            if (UserId != Settings.GetUserModel().UserId)
            {
                if (string.IsNullOrEmpty(ProfileVm.tokketUser.Website))
                {
                    LinkProfileUrl.Visibility = ViewStates.Gone;
                    UrlEditIcon.Visibility = ViewStates.Gone;
                }
            }

            if (UserId == Settings.GetUserModel().UserId)
            {
                loadNameBioWebsite(ProfileVm.UserDisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);

                CoverPhotoIcon.Visibility = ViewStates.Visible;
                UserPhotoIcon.Visibility = ViewStates.Visible;
                AvatarsButton.Visibility = ViewStates.Visible;

                if (!string.IsNullOrEmpty(Settings.GetTokketUser().AccountType))
                {
                    if (Settings.GetTokketUser().AccountType == "group")
                    {
                        BtnSubAccnts.Visibility = ViewStates.Visible;
                        BtnSubAccnts.Text = Settings.GetTokketUser().GroupAccountType.Substring(0, 1).ToUpper() + Settings.GetTokketUser().GroupAccountType.Substring(1) + " Subaccounts";
                    }
                }
            }
            else
            {
                BtnSubAccnts.Visibility = ViewStates.Gone;
                CoverPhotoIcon.Visibility = ViewStates.Gone;
                UserPhotoIcon.Visibility = ViewStates.Gone;
                AvatarsButton.Visibility = ViewStates.Gone;

                BetterLinkMovementMethod
                .Linkify(MatchOptions.WebUrls, new List<TextView> { LinkProfileUrl });

                UrlEditIcon.Visibility = ViewStates.Gone;
            }

            long longcoins = 0;
            if (ProfileVm.Coins != null)
            {
                longcoins = ProfileVm.Coins.Value;
            }

            //string stringcoins = ProfileVm.Coins.ToString();
            //if (string.IsNullOrEmpty(stringcoins))
            //{
            //    longcoins = 0;
            //}
            //else
            //{
            //    longcoins = long.Parse(stringcoins);
            //}

            TextProfileCoins.Text = longcoins.ToKMB();

            ShowCurrentRank();

            //UserDescription.SetOnTouchListener(this);
            //UrlEditIcon.SetOnTouchListener(this);
            //ProfileUsername.SetOnTouchListener(this);
            GifCoinIcon.SetOnTouchListener(this);
            TextProfileCoins.SetOnTouchListener(this);
            ImgLevel.SetOnTouchListener(this);

            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }
            tokModel = new TokModel();
            classTokModel = new ClassTokModel();
            mLayoutManager = new GridLayoutManager(Application.Context, numcol);
            RecyclerToksList.SetLayoutManager(mLayoutManager);

            this.RunOnUiThread(async () => await LoadToks());

            var scrollRange = -1;

            AppBarProfile.OffsetChanged += (sender, args) =>
            {
                //if (scrollRange == -1)
                scrollRange = AppBarProfile.TotalScrollRange;

                if (scrollRange + args.VerticalOffset == 0)
                {
                    //set color action bar
                    //SupportActionBar.SetBackgroundDrawable(new ColorDrawable(ManipulateColor.manipulateColor(ProfileVm.GListenerCover.mColorPalette, 0.62f)));

                    if (NestedScroll.ScrollY == 0)
                    {
                        if (ProfileUserPhoto.Visibility == ViewStates.Visible)
                        {
                            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_scaledown);
                            ProfileUserPhoto.StartAnimation(myAnim);
                            ProfileUserPhoto.Visibility = ViewStates.Invisible;
                        }
                    }

                    UserPhotoIcon.Visibility = ViewStates.Gone;
                    CollapsingToolbar.Title = ProfileVm.UserDisplayName;
                    SwipeRefreshProfile.Enabled = false;
                }

                if (args.VerticalOffset == 0) //Expanded
                {
                    SwipeRefreshProfile.Enabled = true;

                    CollapsingToolbar.Title = " ";

                    ProfileUserPhoto.Visibility = ViewStates.Visible;

                    if (UserId == Settings.GetTokketUser().Id)
                    {
                        UserPhotoIcon.Visibility = ViewStates.Visible;
                    }

                   /* if (!isCointapped)
                    {
                        Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.view_scaleup);
                        ProfileUserPhoto.StartAnimation(myAnim);
                    }*/
                }
            };

            //LoadMore
            if (RecyclerToksList != null)
            {
                RecyclerToksList.HasFixedSize = true;
                RecyclerToksList.NestedScrollingEnabled = false;

                NestedScroll.ScrollChange += async (object sender, NestedScrollView.ScrollChangeEventArgs e) =>
                {
                    View view = (View)NestedScroll.GetChildAt(NestedScroll.ChildCount - 1);

                    int diff = (view.Bottom - (NestedScroll.Height + NestedScroll.ScrollY));

                    if (diff == 0)
                    {
                        if (!string.IsNullOrEmpty(Settings.ContinuationToken))
                        {
                            await LoadMoreToks();
                        }
                    }

                    /*if (NestedScroll.ScrollY == 0)
                    {
                        //SwipeRefreshProfile.Enabled = true;
                        isCointapped = false;
                    }
                    else
                    {
                        SwipeRefreshProfile.Enabled = false;
                        isCointapped = true;
                    }*/
                };
            }

            Glide.With(this)
                .Load(Resource.Drawable.gold)
                .Into(GifCoinIcon);

            /*Stream input = Resources.OpenRawResource(Resource.Drawable.gold);
            byte[] bytes = ConvertByteArray(input);
            GifCoinIcon.SetBytes(bytes);
            GifCoinIcon.StartAnimation();*/

            assetManager = Application.Context.Assets;
            if (!string.IsNullOrEmpty(ProfileVm.tokketUser.Country))
            {
                try
                {
                    string abbr = string.Empty;
                    if (ProfileVm.tokketUser.Country.Length == 2)
                    {
                        abbr = ProfileVm.tokketUser.Country;
                    }
                    else {
                        abbr = CountryHelper.GetCountryAbbreviation(ProfileVm.tokketUser.Country);
                    }
                   
                    sr = assetManager.Open("Flags/" + abbr + ".jpg");
                }
                catch (Exception ex)
                {
                   
                }
            }

            loadStateCountry(ProfileVm.tokketUser);

            BtnSubAccnts.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(SubAccountActivity));
                this.StartActivity(nextActivity);
            };

#if (_TOKKEPEDIA)
            SwipeRefreshProfile.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
#endif

#if (_CLASSTOKS)
            SwipeRefreshProfile.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
#endif
            SwipeRefreshProfile.Refresh += RefreshLayout_Refresh;

            ProfileCoverPhoto.Click += delegate
            {
                gotoImageViewer(ProfileVm.tokketUser.CoverPhoto);
            };

            ProfileUserPhoto.Click += delegate
            {
                if (ProfileUserPhoto.Drawable != null) {
                    Bitmap imgBitmap = ((BitmapDrawable)ProfileUserPhoto.Drawable).Bitmap;
                    MemoryStream byteArrayOutputStream = new MemoryStream();
                    imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                    byte[] byteArray = byteArrayOutputStream.ToArray();
                    string image = JsonConvert.SerializeObject(byteArray);
                    gotoImageViewer(image);
                }
              
            };
        }

        public void loadStateCountry(TokketUser tokketUser)
        {
            ProfileVm.tokketUser = tokketUser;
            if (ProfileVm.tokketUser.Country == "us")
            {
                TxtProfileWordCountry.Text = "State";
                TxtProfileCountryOrState.Visibility = ViewStates.Visible;
                //If Country ="us" but null/empty State, show the "us" Country flag
                if (String.IsNullOrEmpty(ProfileVm.tokketUser.State))
                {
                    sr = assetManager.Open("Flags/us.jpg");
                    Bitmap bitmapFlag = BitmapFactory.DecodeStream(sr);
                    ImgFlag.SetImageBitmap(bitmapFlag);
                    ImgFlag.Visibility = ViewStates.Visible;
                    TxtProfileCountryOrState.Text = "None";
                }
                else
                {
                    string flagImg = "";
                    try
                    {
                        var stateModel = CountryHelper.GetCountryStates("us").Find(x => x.Id == ProfileVm.tokketUser.State);
                        flagImg = stateModel.Image;
                    }
                    catch (Exception)
                    {

                        flagImg = CountryTool.GetCountryFlagJPG1x1(ProfileVm.tokketUser.State);
                    }

                    Glide.With(this).Load(flagImg).Into(ImgFlag);
                    TxtProfileCountryOrState.Text = ProfileVm.tokketUser.State;
                }
            }
            else
            {
                //If null/empty Country, show an empty space the size of the flag
                TxtProfileCountryOrState.Visibility = ViewStates.Visible;
                TxtProfileCountryOrState.Text = ProfileVm.tokketUser.Country;
                Bitmap bitmapFlag = BitmapFactory.DecodeStream(sr);
                ImgFlag.SetImageBitmap(bitmapFlag);
                ImgFlag.Visibility = ViewStates.Visible;
            }
        }

        private void gotoImageViewer(string image)
        {
            Intent nextActivity = new Intent(this, typeof(DialogImageViewerActivity));
            Settings.byteImageViewer = image;
            this.StartActivity(nextActivity);
        }
        private void loadNameBioWebsite(string displayname, string userbio, string userwebsite)
        {
            /*string DisplayIcon = "";
            SpannableString spannableString;
            Typeface typeface;
            //Display Name
            DisplayIcon = displayname + " edit";
            spannableString = new SpannableString(DisplayIcon);
            typeface = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            spannableString.SetSpan(new RelativeSizeSpan(1.0f), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            spannableString.SetSpan(new TypefaceSpan(typeface), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            ProfileUsername.SetText(spannableString, TextView.BufferType.Spannable);*/
            ProfileUsername.Text = displayname;

            //Bio
            /*DisplayIcon = userbio + " edit";
            spannableString = new SpannableString(DisplayIcon);
            typeface = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            spannableString.SetSpan(new RelativeSizeSpan(1.2f), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            spannableString.SetSpan(new TypefaceSpan(typeface), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            UserDescription.SetText(spannableString, TextView.BufferType.Spannable);*/
            UserDescription.Text = userbio;

            if (UserId != Settings.GetUserModel().UserId)
            {
                if (string.IsNullOrEmpty(UserDescription.Text))
                {
                    UserDescription.Visibility = ViewStates.Gone;
                }
            }

            /*var spannableEdit = new SpannableString("edit");
            typeface = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            spannableEdit.SetSpan(new RelativeSizeSpan(1.2f), 0, 4, SpanTypes.ExclusiveExclusive);
            spannableEdit.SetSpan(new TypefaceSpan(typeface), 0, 4, SpanTypes.ExclusiveExclusive);
            UrlEditIcon.SetText(spannableEdit, TextView.BufferType.Spannable);*/
            UrlEditIcon.Visibility = ViewStates.Gone;

            LinkProfileUrl.Text = userwebsite;
            BetterLinkMovementMethod
                .Linkify(MatchOptions.WebUrls, new List<TextView> { LinkProfileUrl });
            //.SetOnLinkClickListener(LinkProfileUrl.Click);


            if (UserId != Settings.GetUserModel().UserId)
            {
                if (string.IsNullOrEmpty(LinkProfileUrl.Text))
                {
                    LinkProfileUrl.Visibility = ViewStates.Gone;
                    UrlEditIcon.Visibility = ViewStates.Gone;
                }
            }
        }
        
        public async Task LoadToks()
        {
            TokDataList = new List<TokModel>();
            RecyclerToksList.SetAdapter(null);
            ShimmerToksList.StartShimmerAnimation();
            ShimmerToksList.Visibility = ViewStates.Visible;

            var tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
            ListTokmojiModel = tokmojiResult?.Results?.ToList();
            if (ListTokmojiModel == null)
                ListTokmojiModel = new List<Tokmoji>();

#if (_TOKKEPEDIA)
            var result = await GetToksData();
            TokDataList = result;

            SetClassRecyclerAdapter();
#endif

#if (_CLASSTOKS)
            var result = await GetClassToksData();
            ClassTokList = new List<ClassTokModel>();
            foreach (var item in result)
            {
                if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                {
                    if (Settings.FilterToksProfile == (int)FilterToks.Cards)
                    {
                        var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                        var getTokSections = getTokSectionsResult.Results;
                        item.Sections = getTokSections.ToArray();
                    }
                }
                ClassTokList.Add(item);
            }

            SetClassRecyclerAdapter();
#endif
            if (ClassTokList.Count < 1) {
                TextTokNothingFound.Visibility = ViewStates.Visible;
                TextTokNothingFound.Text = "No Toks";
            }else
                TextTokNothingFound.Visibility = ViewStates.Gone;

            ShimmerToksList.Visibility = ViewStates.Gone;
        }

        public void SetClassRecyclerAdapter()
        {
#if (_TOKKEPEDIA)
            if (Settings.FilterToksProfile == (int) FilterToks.Toks)
            {
                SetRecyclerAdapter(result);
            }
            else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
            {
                SetCardsRecyclerAdapter(result);
            }
#endif

#if (_CLASSTOKS)
            if (Settings.FilterToksProfile == (int)FilterToks.Toks)
            {
                SetClassTokRecyclerAdapter();
            }
            else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
            {
                SetClassCardsRecyclerAdapter();
            }
#endif
        }
        public async Task<List<ClassTokModel>> GetClassToksData(string filter = "")
        {
            bool isPublicFeed = false;
            if (Settings.FilterTag == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            List<ClassTokModel> classTokModelsResult = new List<ClassTokModel>();
            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();

            source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            source.CancelAfter(TimeSpan.FromSeconds(30));
            tokResult = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues()
            {
                partitionkeybase = $"{UserId}-classtoks",
                groupid = UserId,
                userid = UserId,
                text = filter,
                startswith = false,
                publicfeed = isPublicFeed,
                FilterBy = FilterBy.None,
                FilterItems = new List<string>()
            }, source.Token);

            token.Register(() =>
            {
                Console.WriteLine("Task is canceled");
            });

            if (tokResult == null)
            {
                //Close this Activity and go back to Login
                Intent logoutActivity = new Intent(MainActivity.Instance, typeof(LoginActivity));
                logoutActivity.AddFlags(ActivityFlags.ClearTop);
                SecureStorage.Remove("idtoken");
                SecureStorage.Remove("refreshtoken");
                SecureStorage.Remove("userid");

                Settings.UserAccount = string.Empty;

                StartActivity(logoutActivity);
                this.Finish();
                MainActivity.Instance.Finish();
            }
            else
            {
                classTokModelsResult = tokResult.Results.ToList();
                RecyclerToksList.ContentDescription = tokResult.ContinuationToken;
            }
            return classTokModelsResult;
        }

        public async Task LoadMoreToks()
        {
#if (_TOKKEPEDIA)
            TokQueryValues tokQueryModel = new TokQueryValues() { userid = Settings.GetUserModel().UserId, streamtoken = null };
            tokQueryModel.token = Settings.ProfileTabContinuationToken;
            tokQueryModel.loadmore = "yes";
            var result = await TokService.Instance.GetAllToks(tokQueryModel);
            if (result != null)
            {                
                if (Settings.FilterToksProfile == (int)FilterToks.Toks)
                {
                    tokDataAdapter.UpdateItems(result);
                }
                else if (Settings.FilterToksProfile == (int)FilterToks.Toks)
                {
                    foreach (var item in result)
                    {
                        if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                        {
                            if (Settings.FilterToksProfile == (int)FilterToks.Cards)
                            {
                                var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                                var getTokSections = getTokSectionsResult.Results;
                                item.Sections = getTokSections.ToArray();
                            }
                        }
                    }
                    tokcardDataAdapter.UpdateItems(result);
                }

                TokDataList.AddRange(result);
            }
#endif

#if (_CLASSTOKS)
            bool isPublicFeed = false;
            if (Settings.FilterTag == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            var tokResult = await ClassService.Instance.GetClassToksAsync(new ClassTokQueryValues()
            {
                partitionkeybase = $"{UserId}-classtoks",
                groupid = "",
                userid = Settings.GetUserModel().UserId,
                text = "",
                startswith = false,
                publicfeed = isPublicFeed,
                FilterBy = FilterBy.None,
                FilterItems = new List<string>(),
                paginationid = Settings.ProfileTabContinuationToken
            }, fromCaller: "profile_fragment");
            var classTokModelsResult = tokResult.Results.ToList();
            Settings.ProfileTabContinuationToken = tokResult.ContinuationToken;
            foreach (var item in classTokModelsResult)
            {
                if (item.IsMegaTok == true || item.TokGroup.ToLower() == "mega")
                {
                    if (Settings.FilterToksProfile == (int)FilterToks.Cards)
                    {
                        var getTokSectionsResult = await TokService.Instance.GetTokSectionsAsync(item.Id);
                        var getTokSections = getTokSectionsResult.Results;
                        item.Sections = getTokSections.ToArray();
                    }
                }

                ClassTokList.Add(item);
            }

            SetClassRecyclerAdapter();
#endif
        }
        private void SetRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokDataAdapter = new TokDataAdapter(tokModelRes, ListTokmojiModel);
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(tokDataAdapter);
        }
        private void SetCardsRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokcardDataAdapter = new TokCardDataAdapter(tokModelRes, ListTokmojiModel);
            tokcardDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(tokcardDataAdapter);
        }
        private void SetClassTokRecyclerAdapter()
        {
            classtokDataAdapter = new ClassTokDataAdapter(this, ClassTokList, ListTokmojiModel);
            classtokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(classtokDataAdapter);
        }
        private void SetClassCardsRecyclerAdapter()
        {
            classtokcardDataAdapter = new ClassTokCardDataAdapter(ClassTokList, ListTokmojiModel);
            classtokcardDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(classtokcardDataAdapter);
        }

        public void ShowCurrentRank()
        {
            string oldcolor = ProfileVm.tokketUser.PointsSymbolColor;
            //ProfileVm.tokketUser = Settings.GetTokketUser();

            foreach (var item in PointsSymbolsHelper.PointsSymbols)
            {
                if (!string.IsNullOrEmpty(oldcolor))
                {
                    item.Image = item.Image.Replace(oldcolor, Settings.GetTokketUser().PointsSymbolColor);
                }
            }

            PointsSymbolModel pointResult = PointsSymbolsHelper.GetPatchExactResult(ProfileVm.userpoints);
            TextLevelRank.Text = pointResult.Level;

            Glide.With(this).Load(pointResult.Image).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image)).Into(ImgLevel);

            TextLevelPoints.Text = ProfileVm.userpoints.ToString() + " points";
        }
        public async Task<List<Shared.Models.TokModel>> GetToksData()
        {
            FilterType type = (FilterType)Settings.FilterTag;
            tokResult = new List<Shared.Models.TokModel>();
            string strtoken = ProfileVm.tokketUser.StreamToken;
            tokResult = await TokService.Instance.GetAllToks(new TokQueryValues() { userid = UserId, streamtoken = strtoken });

            var toksWithSticker = new List<TokModel>();
            tokResult = tokResult.OrderByDescending(x => x.DateCreated.Value).ToList() ?? new List<TokModel>();
            var cnt = 0;
            foreach (var tok in tokResult)
            {
                var sticker = StickersTool.Stickers.FirstOrDefault(x => x.Id == (string.IsNullOrEmpty(tok.Sticker) ? tok.Sticker : tok.Sticker.Split("-")[0]));
                tok.StickerImage = sticker?.Image ?? string.Empty;
                tok.IndexCounter = cnt;
                toksWithSticker.Add(tok);
                cnt += 1;
            }
            toksWithSticker = toksWithSticker.ToList();
            return tokResult;
        }
        public void OnGridBackgroundClick(object sender, int position)
        {
            int photoNum = position + 1;
#if (_TOKKEPEDIA)
            Intent nextActivity = new Intent(this, typeof(TokInfoActivity));
            tokModel = TokDataList[position];
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            nextActivity.PutExtra("tokModel", modelConvert);
            this.StartActivityForResult(nextActivity, 20001);
#endif

#if (_CLASSTOKS)
            Intent nextActivity = new Intent(this, typeof(TokInfoActivity));
            classTokModel = ClassTokList[position];
            var modelConvert = JsonConvert.SerializeObject(classTokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
#endif

        }
        public bool OnTouch(View v, MotionEvent e)
        {
            int ndx = 0;
            if (v.ContentDescription != "patches")
            {
                try
                {
                    try { ndx = (int)v.Tag; } catch { ndx = int.Parse((string)v.Tag); }
                }
                catch { }
            }


            if (e.Action == MotionEventActions.Up)
            {
                if (v.ContentDescription == "coins")
                {

                }
                else if (v.ContentDescription == "patches")
                {
                    Intent nextActivity = new Intent(this, typeof(PatchesActivity));
                    var tokketuser = JsonConvert.SerializeObject(ProfileVm.tokketUser);
                    nextActivity.PutExtra("tokketuser",tokketuser);
                    this.StartActivity(nextActivity);
                }
                else
                {
                    if (v.Tag != null)
                    {
                        if (UserId == Settings.GetUserModel().UserId)
                        {
                            //When Image of User Photo is clicked.
                            int position = Convert.ToInt32(v.ContentDescription);
                            Intent nextActivity = new Intent(this, typeof(ProfileInputBox));

                            if (ndx == 1002)
                            {
                                nextActivity.PutExtra("inputbox", ProfileVm.tokketUser.Website); //used a substring to remove the "edit" word at the end of every text
                                nextActivity.PutExtra("inputtype", ndx);
                            }
                            else
                            {
                                nextActivity.PutExtra("inputbox", (v as TextView).Text.Substring(0, (v as TextView).Text.Length - 4)); //used a substring to remove the "edit" word at the end of every text
                                nextActivity.PutExtra("inputtype", ndx);
                            }

                            this.StartActivityForResult(nextActivity, ndx);
                        }
                    }
                }
            }
            else if (e.Action == MotionEventActions.Down)
            {
                if (v.ContentDescription == "coins")
                {
                    long longcoins = ProfileVm.Coins.Value;
                    ShowLottieMessageDialog(this, string.Format("{0:#,##0.##}", longcoins.ToKMB()), true, handlerOkClick: (s, e) =>
                    {
                    }, _animation: "dollar_coin_spinning.json", header: "Coins");
                }
            }
            else if (e.Action == MotionEventActions.Cancel)
            {
                if (v.ContentDescription == "coins")
                {

                }
            }
            return true;
        }
        
        [Java.Interop.Export("OnClickProfileImage")]
        public void OnClickProfileImage(View v)
        {
            if (UserId == Settings.GetUserModel().UserId)
            {
                Settings.BrowsedImgTag = Convert.ToInt32(v.ContentDescription);
                BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this,ProfileUserPhoto);
                bottomsheet.Show(this.SupportFragmentManager, "tag");
            }
        }

        [Java.Interop.Export("OnClickCoverPhoto")]
        public void OnClickCoverPhoto(View v)
        {
            if (UserId == Settings.GetUserModel().UserId)
            {
                Settings.BrowsedImgTag = Convert.ToInt32(v.ContentDescription);
                BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this, ProfileCoverPhoto);
                bottomsheet.Show(this.SupportFragmentManager, "tag");

                /*Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.ProfileActivity);*/
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.ProfileActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;

                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }
            else if ((requestCode == 1001) && (resultCode == Result.Ok) && (data != null))
            {
                ProfileVm.tokketUser.Bio = data.GetStringExtra("inputbox");
                UserDescription.Text = ProfileVm.tokketUser.Bio;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == 1002) && (resultCode == Result.Ok) && (data != null))
            {
                ProfileVm.tokketUser.Website = data.GetStringExtra("inputbox");
                LinkProfileUrl.Text = ProfileVm.tokketUser.Website;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == 1003) && (resultCode == Result.Ok) && (data != null))
            {
                ProfileUsername.Text = data.GetStringExtra("inputbox");
                ProfileVm.tokketUser.DisplayName = ProfileUsername.Text;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == EDIT_PROFILE_REQUEST_CODE) && (resultCode == Result.Ok) && (data != null))
            {
                ProfileVm.tokketUser.Bio = data.GetStringExtra("bio");
                ProfileVm.tokketUser.Website = data.GetStringExtra("web");
                ProfileVm.tokketUser.DisplayName = data.GetStringExtra("displayname");

                UserDescription.Text = ProfileVm.tokketUser.Bio;
                LinkProfileUrl.Text = ProfileVm.tokketUser.Website;
                ProfileUsername.Text = ProfileVm.tokketUser.DisplayName;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == 20001) && (resultCode == Result.Ok))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
                    TokDataList.Remove(tokModel);
                    SetRecyclerAdapter(TokDataList);
                }
            }
            else if ((requestCode == (int)ActivityType.AvatarsActivity) && (resultCode == Result.Ok)) //Avatar
            {
                var avatarString = data.GetStringExtra("Avatar");
                var avatarModel = JsonConvert.DeserializeObject<Avatar>(avatarString);
                Glide.With(this).Load(avatarModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileUserPhoto);
            }
            else if ((requestCode == 40011) && (resultCode == Result.Ok)) //Badge
            {
                var badgeString = data.GetStringExtra("Badge");
                var badgeModel = JsonConvert.DeserializeObject<BadgeOwned>(badgeString);
                Glide.With(this).Load(badgeModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileUserPhoto);
            }
            else if ((requestCode == REQUEST_PROFILE_MEMBERSHIP) && (resultCode == Result.Ok))
            {
                long longcoins = Settings.UserCoins;
                TextProfileCoins.Text = longcoins.ToKMB();
            }
        }
        public void displayImageBrowse()
        {
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                var bitmap = BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length);
                MainActivity.Instance.bitmapUserPhoto = SpannableHelper.scaleDown(bitmap, 300, true);

                ProfileUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);

                classtokDataAdapter.NotifyDataSetChanged();

                ProfileFragment.Instance.ProfileUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
                MainActivity.Instance.userphoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);

                ClassToksFragment.Instance.ClassTokDataAdapter.NotifyDataSetChanged();

                if (ProfileFragment.Instance.classtokDataAdapter != null)
                {
                    ProfileFragment.Instance.classtokDataAdapter.NotifyDataSetChanged();
                }

                //Recreate activity due to issue that image not showing
                Finish();
                OverridePendingTransition(0, 0);
                StartActivity(this.Intent);
                OverridePendingTransition(0, 0);
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                ProfileCoverPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            }

            this.RunOnUiThread(async () => await SaveUserCoverPhoto(Settings.ImageBrowseCrop));
            Settings.ImageBrowseCrop = null;
        }
        private async Task SaveUserCoverPhoto(string base64img)
        {
            var saveBase64img = "data:image/jpeg;base64," + base64img;
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                var tokketUser = Settings.GetTokketUser();
                //Refresh image
                if (Barrel.Current.Exists(tokketUser.Id))
                {
                    Barrel.Current.Empty(tokketUser.Id);
                }

                MainActivity.Instance.cacheUserPhoto = base64img;

                SetCachedAsync<string>(tokketUser.Id, base64img, 365);

                var uri = await ServiceAccount.AccountService.Instance.UploadProfilePictureAsync(saveBase64img);

                if (uri)
                {
                    tokketUser = await ServiceAccount.AccountService.Instance.GetUserAsync(Settings.GetUserModel().UserId);
                    Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                }
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                await ServiceAccount.AccountService.Instance.UploadProfileCoverAsync(saveBase64img);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.profile_menu, menu);

            //IMenuItem Android.Resource.Id
            var itemReport = menu.FindItem(Resource.Id.item_Report);
            //var itemTitle = menu.FindItem(Resource.Id.item_titles);
            var item_avatar = menu.FindItem(Resource.Id.item_avatar);
            var itemMembership = menu.FindItem(Resource.Id.item_Membership);

            if (UserId == Settings.GetUserModel().UserId)
            {
                itemReport.SetVisible(false);
                //itemTitle.SetVisible(true);
                item_avatar.SetVisible(true);
                itemMembership.SetVisible(true);
            }
            else
            {
                itemReport.SetVisible(true);
                //itemTitle.SetVisible(false);
                item_avatar.SetVisible(false);
                itemMembership.SetVisible(false);
            }

            return true;
        }

        public override void Finish()
        {
            Instance = null;
            isOnProfile = false;
            base.Finish();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent nextActivity;
            switch (item.ItemId)
            {
                case HomeId:
                    Settings.ActivityInt = (int)ActivityType.HomePage;
                    Finish();
                    break;
                case Resource.Id.item_filter:
                    nextActivity = new Intent(this, typeof(FilterActivity));
                    nextActivity.PutExtra("activitycaller", "Profile");
                    nextActivity.PutExtra("SubTitle", "User: " + ProfileVm.UserDisplayName);
                    //nextActivity.PutExtra("TokList",JsonConvert.SerializeObject(TokDataList));

                    editor.PutString("TokModelList", JsonConvert.SerializeObject(TokDataList));
                    editor.Apply();

                    StartActivity(nextActivity);
                    break;
                /*case Resource.Id.item_titles:
                    nextActivity = new Intent(this, typeof(ProfileTitleActivity));
                    StartActivity(nextActivity);
                    break;*/
                case Resource.Id.item_avatar:
                    nextActivity = new Intent(this, typeof(AvatarsActivity));
                    this.StartActivityForResult(nextActivity, (int)ActivityType.AvatarsActivity);
                    break;
                case Resource.Id.item_badges:
                    nextActivity = new Intent(this, typeof(BadgesActivity));
                    this.StartActivityForResult(nextActivity, 40011);
                    break;
                case Resource.Id.item_sets:
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.ProfileSetsView);
                    nextActivity = new Intent(this, typeof(MySetsActivity));
                    nextActivity.PutExtra("isAddToSet", false);
                    nextActivity.PutExtra("TokTypeId", "");
                    nextActivity.PutExtra("userId", UserId);
                    nextActivity.SetFlags(ActivityFlags.NewTask);
                    StartActivity(nextActivity);
                    break;
                case Resource.Id.item_Share:
                    RunOnUiThread(async () => await Share.RequestAsync(new ShareTextRequest
                    {
                        Uri = Shared.Config.Configurations.Url + "user/" + ProfileVm.UserId,
                        Title = ProfileVm.UserDisplayName
                    }));

                    break;

                case Resource.Id.item_Membership:
                    nextActivity = new Intent(this, typeof(ProfileMembershipActivity));
                    StartActivityForResult(nextActivity, REQUEST_PROFILE_MEMBERSHIP);
                    break;
                case Resource.Id.item_Report:
                    Report.Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        [Java.Interop.Export("onRadioButtonClicked")]
        public void onRadioButtonClicked(View view)
        {
            Report.ItemSelected(view);
        }

        [Java.Interop.Export("OnReport")]
        public async void OnReport(View view)
        {
            var alert = new AlertDialog.Builder(this);
            Report.ReportProgress.Visibility = ViewStates.Visible;
            if (!string.IsNullOrEmpty(Report.SelectedReportMessage) || Report.SelectedReportMessage != "Choose One")
            {
                Report r = new Report();
                r.ItemId = classTokModel.Id;
                r.ItemLabel = classTokModel.Label;
                r.Message = Report.SelectedReportMessage;
                r.OwnerId = classTokModel.UserId;
                r.UserId = Settings.GetTokketUser()?.Id;

                var result = await ReactionService.Instance.AddReport(r);

                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    alert.SetTitle("Report Successful!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                else
                {
                    alert.SetTitle("Report Failed!");
                    alert.SetPositiveButton("OK", (obj, eve) => {
                        Report.Dismiss();
                    });
                }
                alert.Show();
            }
            else
            {
                alert.SetTitle("Select a reason for the report!");
                alert.Show();
            }
            Report.ReportProgress.Visibility = ViewStates.Gone;
        }
        public override void OnBackPressed()
        {
            Settings.ActivityInt = (int)ActivityType.HomePage;
            base.OnBackPressed();
        }
        /*private byte[] ConvertByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }*/
        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Transparent);
                TextNothingFound.Visibility = ViewStates.Gone;

                //Data Refresh Place  
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            }
            else
            {
                TextNothingFound.Text = "No Internet Connection!";
                TextNothingFound.SetTextColor(Color.White);
                TextNothingFound.SetBackgroundColor(Color.Black);
                TextNothingFound.Visibility = ViewStates.Visible;
                SwipeRefreshProfile.Refreshing = false;
            }
        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SwipeRefreshProfile.Refreshing = false;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            this.RunOnUiThread(async () => await LoadToks());
            Thread.Sleep(3000);
        }

        public Button btnEditProfile => FindViewById<Button>(Resource.Id.btnEditProfile);
        public ImageView GifCoinIcon => FindViewById<ImageView>(Resource.Id.gif_profileCoins);
        public CircleImageView ProfileUserPhoto => FindViewById<CircleImageView>(Resource.Id.img_profileUserPhoto);
        public ImageView ImgFlag => FindViewById<ImageView>(Resource.Id.imageProfileFlag);
        public ImageView ProfileCoverPhoto => FindViewById<ImageView>(Resource.Id.img_profileCoverPhoto);
        public TextView LinkProfileUrl => FindViewById<TextView>(Resource.Id.lblProfileUrl);
        public TextView ProfileUsername => FindViewById<TextView>(Resource.Id.lblProfileUserName);
        public TextView UserTitle => FindViewById<TextView>(Resource.Id.lblUserTitle);
        public TextView UserDescription => FindViewById<TextView>(Resource.Id.lblProfileUserDescription);
        public ImageView CoverPhotoIcon => FindViewById<ImageView>(Resource.Id.ProfileCoverCameraIcon);
        public CircleImageView UserPhotoIcon => FindViewById<CircleImageView>(Resource.Id.ProfileUserCameraIcon);
        public RecyclerView RecyclerToksList => FindViewById<RecyclerView>(Resource.Id.RecyclerProfilePageToks);
        public ShimmerLayout ShimmerToksList => FindViewById<ShimmerLayout>(Resource.Id.ShimmerProfilePageToks);
        public NestedScrollView NestedScroll => FindViewById<NestedScrollView>(Resource.Id.NestedProfilePage);
        public TextView TxtProfileWordCountry => FindViewById<TextView>(Resource.Id.TxtProfileWordCountry);
        public TextView TxtProfileCountryOrState => FindViewById<TextView>(Resource.Id.TxtProfileCountrystate);
        public Button AvatarsButton => FindViewById<Button>(Resource.Id.btnProfileAvatars);
        public TextView TextLevelRank => FindViewById<TextView>(Resource.Id.TextLevelRank);
        public TextView TextLevelPoints => FindViewById<TextView>(Resource.Id.TextLevelPoints);
        public ImageView ImgLevel => FindViewById<ImageView>(Resource.Id.ImgLevel);
        public TextView TextProfileCoins => FindViewById<TextView>(Resource.Id.TextProfileCoins);
        public PhotoView ImgUserImageView => FindViewById<PhotoView>(Resource.Id.ImgProfileImageView);
        public Button BtnSubAccnts => FindViewById<Button>(Resource.Id.btnProfileSubAccnts);
        public AppBarLayout AppBarProfile => FindViewById<AppBarLayout>(Resource.Id.AppBarProfile);
        public AndroidX.AppCompat.Widget.Toolbar ProfileToolBar => FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.profile_toolbar);
        public CollapsingToolbarLayout CollapsingToolbar => FindViewById<CollapsingToolbarLayout>(Resource.Id.CollapsingToolbar);
        public SwipeRefreshLayout SwipeRefreshProfile => FindViewById<SwipeRefreshLayout>(Resource.Id.SwipeRefreshProfile);
        public TextView UrlEditIcon => FindViewById<TextView>(Resource.Id.lblProfileUrlEditIcon);
        public View ViewDummyForTouch => FindViewById<View>(Resource.Id.ViewDummyForTouch);
        public TextView TextNothingFound => FindViewById<TextView>(Resource.Id.TextNothingFound);

        public TextView TextTokNothingFound => FindViewById<TextView>(Resource.Id.TextTokNothingFound);
    }
}