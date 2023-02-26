using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Tokket.Core;
using Xamarin.Essentials;
using Bumptech.Glide.Request;
using Tokket.Android.Adapters;
using Tokket.Android.ViewModels;
using DE.Hdodenhof.CircleImageViewLib;
using System.IO;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Android.Graphics;
using ServiceAccount = Tokket.Shared.Services;
using Android.Content.Res;
using Newtonsoft.Json;
using Tokket.Core.Tools;
using System.Threading.Tasks;
using Tokket.Shared.Services;
using Supercharge;
using Tokket.Shared.Extensions;
using Android.Views.Animations;
using System.ComponentModel;
using System.Threading;
using Tokket.Android.Helpers;
using Android.Text.Util;
using ImageViews.Photo;
using Android.Animation;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.AppBar;
using Color = Android.Graphics.Color;
using Bumptech.Glide.Load.Resource.Drawable;
using Android.Graphics.Drawables;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services.ServicesDB;
using AndroidX.Core.Widget;
using AndroidX.SwipeRefreshLayout.Widget;

namespace Tokket.Android.Fragments
{
    public class ProfileFragment : AndroidX.Fragment.App.Fragment, View.IOnTouchListener
    {
        internal static ProfileFragment Instance{ get; private set; }
        private FragmentActivity myContext;
        public View v;
        List<TokModel> tokResult;
        public List<TokModel> TokDataList; public  List<ClassTokModel> ClassTokList;
        List<Tokmoji> ListTokmojiModel;
        TokDataAdapter tokDataAdapter; TokCardDataAdapter tokcardDataAdapter;
        public ClassTokDataAdapter classtokDataAdapter; ClassTokCardDataAdapter classtokcardDataAdapter;
        TokModel tokModel; ClassTokModel classTokModel;
        GridLayoutManager mLayoutManager;
        private int EDIT_PROFILE_REQUEST_CODE = 1004;
        public ProfilePageViewModel ProfileVm => App.Locator.ProfilePageVM;
        Stream sr = null; AssetManager assetManager;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.profile_page, container, false);
        
            Instance = this;
            myContext = MainActivity.Instance;

            var tokketUser = Settings.GetTokketUser();
            ProfileVm.UserId = tokketUser.Id;
            ProfileVm.activity = myContext;
            ProfileVm.ProfileCoverPhoto = ProfileCoverPhoto;
            ProfileVm.ProfileUserPhoto = ProfileUserPhoto;
            myContext.RunOnUiThread(async () => await ProfileVm.Initialize(tokketUser));

            ProfileUserPhoto.Click += delegate
            {
                Bitmap imgBitmap = ((BitmapDrawable)ProfileUserPhoto.Drawable).Bitmap;
                MemoryStream byteArrayOutputStream = new MemoryStream();
                imgBitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                string image = JsonConvert.SerializeObject(byteArray);
                gotoImageViewer(image);
            };

            ProfileCoverPhoto.Click += delegate
            {
                gotoImageViewer(ProfileVm.tokketUser.CoverPhoto);
            };

            ProfileCoverPhoto.LayoutChange += delegate
            {
                //set color action bar
                //MainActivity.Instance.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(ManipulateColor.manipulateColor(ProfileVm.GListenerCover.mColorPalette, 0.62f)));
                
                ////set color status bar
                //if (ProfileVm.GListenerCover.mColorPalette == 0)
                //{
                //    MainActivity.Instance.Window.SetStatusBarColor(Color.Transparent);
                //    ParentImageViewer.SetBackgroundColor(Color.Black);
                //}
                //else
                //{
                //    MainActivity.Instance.Window.SetStatusBarColor(ManipulateColor.manipulateColor(ProfileVm.GListenerCover.mColorPalette, 0.32f));
                //}
            };

            UrlEditIcon.Visibility = ViewStates.Gone;

            btnEditProfile.Click += delegate
            {
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(EditProfileActivity));
                nextActivity.PutExtra("bio", ProfileVm.tokketUser.Bio);
                nextActivity.PutExtra("web", ProfileVm.tokketUser.Website);
                nextActivity.PutExtra("displayname", ProfileVm.tokketUser.DisplayName);
                this.StartActivityForResult(nextActivity, EDIT_PROFILE_REQUEST_CODE);
            };

            ProfileUsername.Text = ProfileVm.UserDisplayName;

            if (ProfileVm.tokketUser == null)
            {
                ProfileVm.tokketUser = Settings.GetTokketUser();
            }

            if (!string.IsNullOrEmpty(ProfileVm.tokketUser.CurrentHandle))
            {
                UserTitle.Visibility = ViewStates.Visible;
                UserTitle.Text = ProfileVm.tokketUser.CurrentHandle;
            }
           

            UserDescription.Text = ProfileVm.tokketUser.Bio;
            LinkProfileUrl.Text = ProfileVm.tokketUser.Website;

            if (string.IsNullOrEmpty(UserDescription.Text))
            {
                UserDescription.Visibility = ViewStates.Gone;
            }

            if (string.IsNullOrEmpty(LinkProfileUrl.Text))
            {
                LinkProfileUrl.Visibility = ViewStates.Gone;
                UrlEditIcon.Visibility = ViewStates.Gone;
            }

            long longcoins = Settings.UserCoins;
            TextProfileCoins.Text = longcoins.ToKMB();

            PointsSymbolModel pointResult = PointsSymbolsHelper.GetPatchExactResult(ProfileVm.userpoints);
            TextLevelRank.Text = pointResult.Level;
            PointsSymbolsHelper.UserCurrentRank = pointResult.Name;
            Glide.With(myContext).Load(pointResult.Image).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image)).Into(ImgLevel);
            TextLevelPoints.Text = ProfileVm.userpoints.ToString() + " points";
            var id = Settings.UserId;
            if (ProfileVm.tokketUser.Id == id)
            {
                CoverPhotoIcon.Visibility = ViewStates.Visible;
                UserPhotoIcon.Visibility = ViewStates.Visible;
                AvatarsButton.Visibility = ViewStates.Visible;

                if (Settings.GetTokketUser().AccountType.ToLower() == "group")
                {
                    BtnCreateSub.Visibility = ViewStates.Visible;
                    BtnSubAccnts.Visibility = ViewStates.Visible;
                    BtnSubAccnts.Text = Settings.GetTokketUser().GroupAccountType.Substring(0, 1).ToUpper() + Settings.GetTokketUser().GroupAccountType.Substring(1) + " Subaccounts";
                }

                loadNameBioWebsite(ProfileVm.UserDisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else
            {
                BtnSubAccnts.Visibility = ViewStates.Gone;
                AvatarsButton.Visibility = ViewStates.Gone;
                CoverPhotoIcon.Visibility = ViewStates.Gone;
                UserPhotoIcon.Visibility = ViewStates.Gone;
                BtnCreateSub.Visibility = ViewStates.Gone;

                BetterLinkMovementMethod
                .Linkify(MatchOptions.WebUrls, new List<TextView> { LinkProfileUrl });
                UrlEditIcon.Visibility = ViewStates.Gone;
            }

            //UserDescription.SetOnTouchListener(this);
            //LinkProfileUrl.SetOnTouchListener(this);
            //UrlEditIcon.SetOnTouchListener(this);
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

            var scrollRange = -1;
            AppBarProfile.OffsetChanged += (sender, args) =>
            {
                //if (scrollRange == -1)
                
                scrollRange = AppBarProfile.TotalScrollRange;

                Console.WriteLine(scrollRange);

                if (scrollRange + args.VerticalOffset == 0)
                {
                    if (NestedScroll.ScrollY == 0)
                    {
                        if (ProfileUserPhoto.Visibility == ViewStates.Visible)
                        {
                            Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.fab_scaledown);
                            ProfileUserPhoto.StartAnimation(myAnim);
                            ProfileUserPhoto.Visibility = ViewStates.Invisible;
                        }
                    }

                    UserPhotoIcon.Visibility = ViewStates.Gone;
                    SwipeRefreshProfile.Enabled = false;

                    if (args.VerticalOffset == 0)
                    {
                        
                    }
                }

                if (args.VerticalOffset == 0) //Expanded
                {
                    SwipeRefreshProfile.Enabled = true;

                    ProfileUserPhoto.Visibility = ViewStates.Visible;
                    UserPhotoIcon.Visibility = ViewStates.Visible;

                   /* if (!isCointapped)
                    {
                        Animation myAnim = AnimationUtils.LoadAnimation(MainActivity.Instance, Resource.Animation.view_scaleup);
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
                        if (!string.IsNullOrEmpty(Settings.ProfileTabContinuationToken))
                        {
                            await LoadMoreToks();
                        }
                    }

                  /*  if (NestedScroll.ScrollY == 0)
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

            //GIF Animation
            Glide.With(MainActivity.Instance)
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
                    else
                    {
                        abbr = CountryHelper.GetCountryAbbreviation(ProfileVm.tokketUser.Country);
                    }
                    sr = assetManager.Open("Flags/" + abbr + ".jpg");
                }
                catch (Exception)
                {
                }
            }


            loadStateCountry(tokketUser);


            AvatarsButton.Click += delegate
            {
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(AvatarsActivity));
                this.StartActivityForResult(nextActivity, (int)ActivityType.AvatarsActivity);
            };
            BtnSubAccnts.Click += delegate
            {
                Intent nextActivity = new Intent(MainActivity.Instance, typeof(SubAccountActivity));
                this.StartActivity(nextActivity);
            };

#if (_TOKKEPEDIA)
            SwipeRefreshProfile.SetColorSchemeColors(Color.DarkViolet, Color.Violet, Color.Lavender, Color.LavenderBlush);
#endif

#if (_CLASSTOKS)
            SwipeRefreshProfile.SetColorSchemeColors(Color.ParseColor("#3498db"), Color.ParseColor("#4ea4de"), Color.ParseColor("#68aede"), Color.ParseColor("#88bee3"));
#endif
            SwipeRefreshProfile.Refresh += RefreshLayout_Refresh;

            BtnCreateSub.Click += async (o, e) => {
                LinearProgress.Visibility = ViewStates.Visible;
                var result = await AccountService.Instance.GetSubaccountsAsync(Settings.UserId);

                if (result.Results.ToList().Count > 10)
                {
                    LinearProgress.Visibility = ViewStates.Gone;
                    var objBuilder = new AlertDialog.Builder(this.Context);
                    objBuilder.SetTitle("Limit reached!");
                    objBuilder.SetMessage("You can only create 10 subaccounts");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", (s, ev) =>
                    {



                    });
                    objDialog.Show();
                }
                else {
                    LinearProgress.Visibility = ViewStates.Gone;
                    var intent = new Intent(Context, typeof(CreateSubaccountActivity));
                    StartActivity(intent);
                }

               
            };

            ClassTokList = new List<ClassTokModel>();

            if (IsAdded)
            {
                RequireActivity().RunOnUiThread(async () => await LoadToks());
            }

            return v;
        }

        public void loadUserImage(TokketUser tokketUser) {

            ProfileVm.tokketUser = tokketUser;
            Glide.With(myContext).Load(tokketUser.UserPhoto).Into(ImgFlag);
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
                    Glide.With(myContext).Load(flagImg).Into(ImgFlag);
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
            Intent nextActivity = new Intent(Context, typeof(DialogImageViewerActivity));
            Settings.byteImageViewer = image;
            this.StartActivity(nextActivity);
        }
        public void ShowCurrentRank()
        {
            string oldcolor = ProfileVm.tokketUser.PointsSymbolColor;
            ProfileVm.tokketUser = Settings.GetTokketUser();

            foreach (var item in PointsSymbolsHelper.PointsSymbols)
            {
                item.Image = item.Image.Replace(oldcolor, Settings.GetTokketUser().PointsSymbolColor);
            }

            PointsSymbolModel pointResult = PointsSymbolsHelper.GetPatchExactResult(ProfileVm.userpoints);
            PointsSymbolsHelper.UserCurrentRank = pointResult.Name;
            TextLevelRank.Text = pointResult.Level;
            Glide.With(myContext).Load(pointResult.Image).Into(ImgLevel);
            TextLevelPoints.Text = ProfileVm.userpoints.ToString() + " points";
        }
        public async Task LoadToks(string filter = "", bool isRefresh = false)
        {
            TokDataList = new List<TokModel>();

            if (!isRefresh)
            {
                RecyclerToksList.SetAdapter(null);
                ShimmerToksList.StartShimmerAnimation();
                ShimmerToksList.Visibility = ViewStates.Visible;
            }

            if (!isRefresh)
            {
                //get tokmojis
                var tokmojiResult = TokMojiService.Instance.GetCacheTokmojisAsync();
                if (tokmojiResult.Results == null)
                {
                    tokmojiResult = await TokMojiService.Instance.GetTokmojisAsync(null);
                }
                if (tokmojiResult != null)
                    ListTokmojiModel = tokmojiResult.Results.ToList();
                else
                    ListTokmojiModel = new List<Tokmoji>();
            }

#if (_TOKKEPEDIA)
            var result = await GetToksData();
            TokDataList = result;
            SwipeRefreshProfile.Refreshing = false;

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
            var result = await GetClassToksData(filter, isRefresh);
            SwipeRefreshProfile.Refreshing = false;

            if (isRefresh)
            {
                ClassTokList.Clear();
            }

            ClassTokList.AddRange(result);

            SetClassRecyclerAdapter(result);
#endif
            ShimmerToksList.Visibility = ViewStates.Gone;
        }

        public void SetClassRecyclerAdapter(List<ClassTokModel> classTokList)
        {
            if (Settings.FilterToksProfile == (int)FilterToks.Toks)
            {
                SetClassTokRecyclerAdapter(classTokList);
            }
            else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
            {
                SetClassCardsRecyclerAdapter(classTokList);
            }
        }
        public async Task<List<ClassTokModel>> GetClassToksData(string filter = "", bool isRefresh = false)
        {
            bool isPublicFeed = false;
            if (Settings.FilterTag == (int)FilterType.All)
            {
                isPublicFeed = true;
            }

            ResultData<ClassTokModel> tokResult = new ResultData<ClassTokModel>();
            tokResult.Results = new List<ClassTokModel>();

            var _fromCaller = "profile_fragment";

            //Load cache data first
            if (isRefresh)
            {
                tokResult.Results = null;

                //Empty the barrel to allow to load a new one
                if (Barrel.Current.Exists(_fromCaller))
                {
                    Barrel.Current.Empty(_fromCaller);
                }
            }
            else
            {
                var cachedClassToks = ClassService.Instance.GetCacheClassToksAsync(_fromCaller);
                tokResult = cachedClassToks;
            }

            if (tokResult != null)
            {
                if (tokResult.Results == null)
                {
                    //If no cache data, load new toks
                    var tokQueryModel = new ClassTokQueryValues()
                    {
                        partitionkeybase = $"{Settings.UserId}-classtoks",
                        groupid = "",
                        userid = Settings.UserId,
                        text = filter,
                        startswith = false,
                        publicfeed = isPublicFeed,
                        FilterBy = FilterBy.None,
                        FilterItems = new List<string>()
                    };

                    tokResult = await ClassService.Instance.GetClassToksAsync(tokQueryModel, fromCaller: _fromCaller);
                }

                if (tokResult != null)
                {
                    if (!string.IsNullOrEmpty(tokResult.ContinuationToken))
                    {
                        Settings.ProfileTabContinuationToken = tokResult.ContinuationToken;
                    }
                }
                else
                {
                    tokResult = new ResultData<ClassTokModel>();
                    tokResult.Results = new List<ClassTokModel>();
                }
            }
            else
            {
                tokResult = new ResultData<ClassTokModel>();
                tokResult.Results = new List<ClassTokModel>();
            }

            /*foreach (var item in tokResult.Results)
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
            }*/

            return tokResult.Results.ToList();
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
                partitionkeybase = $"{Settings.UserId}-classtoks",
                groupid = "",
                userid = Settings.UserId,
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

            SetClassRecyclerAdapter(ClassTokList);
#endif
        }
        private void SetRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokDataAdapter = new TokDataAdapter(tokModelRes, ListTokmojiModel);
            tokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(tokDataAdapter);
        }
        private void SetClassTokRecyclerAdapter(List<ClassTokModel> classTokList)
        {
            classtokDataAdapter = new ClassTokDataAdapter(MainActivity.Instance, classTokList, ListTokmojiModel);
            classtokDataAdapter.ItemClick -= OnGridBackgroundClick;
            classtokDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(classtokDataAdapter);
        }
        private void SetCardsRecyclerAdapter(List<TokModel> tokModelRes)
        {
            tokcardDataAdapter = new TokCardDataAdapter(tokModelRes, ListTokmojiModel);
            tokcardDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(tokcardDataAdapter);
        }
        private void SetClassCardsRecyclerAdapter(List<ClassTokModel> classTokList)
        {
            classtokcardDataAdapter = new ClassTokCardDataAdapter(classTokList, ListTokmojiModel);
            classtokcardDataAdapter.ItemClick += OnGridBackgroundClick;
            RecyclerToksList.SetAdapter(classtokcardDataAdapter);
        }
        public async Task<List<TokModel>> GetToksData()
        {
            FilterType type = (FilterType)Settings.FilterTag;
            tokResult = new List<Shared.Models.TokModel>();
            string strtoken = ProfileVm.tokketUser.StreamToken;

            LinearProgress.Visibility = ViewStates.Visible;
            tokResult = await TokService.Instance.GetAllToks(new TokQueryValues() { userid = Settings.UserId, streamtoken = strtoken });
            LinearProgress.Visibility = ViewStates.Gone;

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
            Intent nextActivity = new Intent(myContext, typeof(TokInfoActivity));
            tokModel = TokDataList[position];
            var modelConvert = JsonConvert.SerializeObject(tokModel);
            nextActivity.PutExtra("tokModel", modelConvert);
            this.StartActivityForResult(nextActivity, 20001);
#endif

#if (_CLASSTOKS)
            Intent nextActivity = new Intent(myContext, typeof(TokInfoActivity));
            classTokModel = ClassTokList[position];
            var modelConvert = JsonConvert.SerializeObject(classTokModel);
            nextActivity.PutExtra("classtokModel", modelConvert);
            this.StartActivityForResult(nextActivity, (int)ActivityType.HomePage);
#endif
        }
        public bool OnLongClick(View v)
        {
            ImgUserImageView.Visibility = ViewStates.Invisible;
            var data = ClipData.NewPlainText("", "");
            View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(v);
            v.StartDragAndDrop(data, shadowBuilder, v, 0);
            return true;
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
                    Intent nextActivity = new Intent(MainActivity.Instance, typeof(PatchesActivity));
                    this.StartActivity(nextActivity);
                }
                else
                {
                    if (v.Tag != null)
                    {
                        if (ProfileVm.tokketUser.Id == Settings.UserId)
                        {
                            //When Image of User Photo is clicked.
                            int position = Convert.ToInt32(v.ContentDescription);
                            Intent nextActivity = new Intent(MainActivity.Instance, typeof(ProfileInputBox));

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
                    MainActivity.Instance.ShowLottieMessageDialog(RequireActivity(), string.Format("{0:#,##0.##}", Settings.UserCoins), true, handlerOkClick: (s, e) =>
                    {
                    }, _animation: "dollar_coin_spinning.json", header: "Coins",isImage:true);
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
       
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == 1001) && (resultCode == -1) && (data != null))
            {
                UserDescription.Text = data.GetStringExtra("inputbox");
                UserDescription.Text = ProfileVm.tokketUser.Bio;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == 1002) && (resultCode == -1) && (data != null))
            {
                ProfileVm.tokketUser.Website = data.GetStringExtra("inputbox");
                LinkProfileUrl.Text = ProfileVm.tokketUser.Website;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == 1003) && (resultCode == -1) && (data != null))
            {
                ProfileUsername.Text = data.GetStringExtra("inputbox");
                ProfileVm.tokketUser.DisplayName = ProfileUsername.Text;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == EDIT_PROFILE_REQUEST_CODE) && (data != null))
            {
                ProfileVm.tokketUser.Bio = data.GetStringExtra("bio");
                ProfileVm.tokketUser.Website = data.GetStringExtra("web");
                ProfileVm.tokketUser.DisplayName = data.GetStringExtra("displayname");

                UserDescription.Text = ProfileVm.tokketUser.Bio;
                LinkProfileUrl.Text = ProfileVm.tokketUser.Website;
                ProfileUsername.Text = ProfileVm.tokketUser.DisplayName;
                loadNameBioWebsite(ProfileVm.tokketUser.DisplayName, ProfileVm.tokketUser.Bio, ProfileVm.tokketUser.Website);
            }
            else if ((requestCode == (int)ActivityType.HomePage) && (resultCode == -1))
            {
                var isDeleted = data.GetBooleanExtra("isDeleted", false);
                if (isDeleted)
                {
#if (_CLASSTOKS)
                    var tokModelstring = data.GetStringExtra("classtokModel");
                    if (tokModelstring != null)
                    {
                        classTokModel = JsonConvert.DeserializeObject<ClassTokModel>(tokModelstring);
                        ClassToksFragment.Instance.RemoveClassTokCollection(classTokModel);
                    }
#endif

#if (_TOKKEPEDIA)
                    var tokModelstring = data.GetStringExtra("tokModel");
                    if (tokModelstring != null)
                    {
                        tokModel = JsonConvert.DeserializeObject<TokModel>(tokModelstring);
                        home_fragment.HFInstance.RemoveTokCollection(tokModel);
                    }
#endif
                    /*TokDataList.Remove(tokModel);

                    if (Settings.FilterToksProfile == (int)FilterToks.Toks)
                    {
                        SetRecyclerAdapter(TokDataList);
                    }
                    else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
                    {
                        SetCardsRecyclerAdapter(TokDataList);
                    }*/
                }
            }
            else if ((requestCode == (int)ActivityType.AvatarsActivity) && (resultCode == -1))
            {
                var avatarString = data.GetStringExtra("Avatar");
                var avatarModel = JsonConvert.DeserializeObject<Avatar>(avatarString);
                Glide.With(myContext).Load(avatarModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileUserPhoto);
            }
            else if ((requestCode == 40011) && (resultCode == -1))
            {
                var badgeString = data.GetStringExtra("Badge");
                var badgeModel = JsonConvert.DeserializeObject<Avatar>(badgeString);
                Glide.With(myContext).Load(badgeModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ProfileUserPhoto);
            }
        }
        public void displayImageBrowse()
        {
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                ProfileUserPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                ProfileCoverPhoto.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            }

            myContext.RunOnUiThread(async () => await SaveUserCoverPhoto(Settings.ImageBrowseCrop));
            Settings.ImageBrowseCrop = null;
        }
        private async Task SaveUserCoverPhoto(string base64img)
        {
            base64img = "data:image/jpeg;base64," + base64img;
            if (Settings.BrowsedImgTag == 0) //UserPhoto
            {
                await ServiceAccount.AccountService.Instance.UploadProfilePictureAsync(base64img);
            }
            else if (Settings.BrowsedImgTag == 1) //Cover Photo
            {
                await ServiceAccount.AccountService.Instance.UploadProfileCoverAsync(base64img);
            }
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
        private void loadNameBioWebsite(string displayname, string userbio, string userwebsite)
        {
            //string DisplayIcon = "";
            //SpannableString spannableString;
            //Typeface typeface;
            //Display Name
            /*DisplayIcon = displayname + " edit";
            spannableString = new SpannableString(DisplayIcon);
            typeface = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            spannableString.SetSpan(new RelativeSizeSpan(1.0f), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            spannableString.SetSpan(new TypefaceSpan(typeface), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            ProfileUsername.SetText(spannableString, TextView.BufferType.Spannable);*/
            ProfileUsername.Text = displayname;
            //Bio
            /*DisplayIcon = userbio + " edit";
            spannableString = new SpannableString(DisplayIcon);
            typeface = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            spannableString.SetSpan(new RelativeSizeSpan(1.2f), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            spannableString.SetSpan(new TypefaceSpan(typeface), DisplayIcon.Length - 4, DisplayIcon.Length, SpanTypes.ExclusiveExclusive);
            UserDescription.SetText(spannableString, TextView.BufferType.Spannable);*/
            UserDescription.Text = userbio;
            if (string.IsNullOrEmpty(UserDescription.Text))
            {
                UserDescription.Visibility = ViewStates.Gone;
            }

            /*var spannableEdit = new SpannableString("edit");
            typeface = Typeface.CreateFromAsset(MainActivity.Instance.Application.Assets, "fa_solid_900.otf");
            spannableEdit.SetSpan(new RelativeSizeSpan(1.2f), 0, 4, SpanTypes.ExclusiveExclusive);
            spannableEdit.SetSpan(new TypefaceSpan(typeface), 0, 4, SpanTypes.ExclusiveExclusive);
            UrlEditIcon.SetText(spannableEdit, TextView.BufferType.Spannable);*/

            LinkProfileUrl.Text = userwebsite;
            BetterLinkMovementMethod
                .Linkify(MatchOptions.WebUrls, new List<TextView> { LinkProfileUrl });
            //.SetOnLinkClickListener(LinkProfileUrl.Click);


            if (string.IsNullOrEmpty(LinkProfileUrl.Text))
            {
                LinkProfileUrl.Visibility = ViewStates.Gone;
                UrlEditIcon.Visibility = ViewStates.Gone;
            }
        }
        public void RemoveToksCollection(string tokId)
        {
#if (_CLASSTOKS)
            var collection = ClassTokList.FirstOrDefault(a => a.Id == tokId);
            if (collection != null) //If item exist
            {
                int ndx = ClassTokList.IndexOf(collection); //Get index
                ClassTokList.Remove(collection);
                SetClassTokRecyclerAdapter(ClassTokList);
            }
#endif

#if (_TOKKEPEDIA)
            var collection = TokDataList.FirstOrDefault(a => a.Id == tokId);
            if (collection != null) //If item exist
            {
                int ndx = TokDataList.IndexOf(collection); //Get index
                TokDataList.Remove(collection);
                SetRecyclerAdapter(TokDataList);
            }
#endif
        }

        public void AddTokCollection(ClassTokModel classTokItem, TokModel tokItem)
        {
            int ndx = 0;
            bool isEdit = false;
#if (_TOKKEPEDIA)
            var collection = TokDataList.FirstOrDefault(a => a.Id == tokItem.Id);
            if (collection != null) //If item exist
            {
                ndx = TokDataList.IndexOf(collection); //Get index
                TokDataList.Remove(collection);
                TokDataList.Insert(ndx, tokItem);
                isEdit = true;
            }
            else
            {
                TokDataList.Insert(0, tokItem);
            }

            if (Settings.FilterToksProfile == (int)FilterToks.Toks)
            {
                SetRecyclerAdapter(TokDataList);
            }
            else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
            {
                SetCardsRecyclerAdapter(TokDataList);
            }
#endif

#if (_CLASSTOKS)

            var collection = ClassTokList.FirstOrDefault(a => a.Id == classTokItem.Id);
            if (collection != null) //If item exist
            {
                ndx = ClassTokList.IndexOf(collection); //Get index
                ClassTokList.Remove(collection);
                ClassTokList.Insert(ndx, classTokItem);
                isEdit = true;
            }
            else
            {
                ClassTokList.Insert(0, classTokItem);
            }

            if (Settings.FilterToksProfile == (int)FilterToks.Toks)
            {
                SetClassTokRecyclerAdapter(ClassTokList);
            }
            else if (Settings.FilterToksProfile == (int)FilterToks.Cards)
            {
                SetClassCardsRecyclerAdapter(ClassTokList);
            }
#endif

            if (isEdit)
            {
                RecyclerToksList.ScrollToPosition(ndx);
            }
        }

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

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            MainActivity.Instance.RunOnUiThread(async () => await LoadToks(isRefresh: true));
            //Thread.Sleep(1000);
        }

        public ImageView GifCoinIcon => v.FindViewById<ImageView>(Resource.Id.gif_profileCoins);
        public CircleImageView ProfileUserPhoto => v.FindViewById<CircleImageView>(Resource.Id.img_profileUserPhoto);
        public ImageView ImgFlag => v.FindViewById<ImageView>(Resource.Id.imageProfileFlag);
        public ImageView ProfileCoverPhoto => v.FindViewById<ImageView>(Resource.Id.img_profileCoverPhoto);
        public Button btnEditProfile => v.FindViewById<Button>(Resource.Id.btnEditProfile);
        public TextView LinkProfileUrl => v.FindViewById<TextView>(Resource.Id.lblProfileUrl);
        public TextView ProfileUsername => v.FindViewById<TextView>(Resource.Id.lblProfileUserName);
        public TextView UserTitle => v.FindViewById<TextView>(Resource.Id.lblUserTitle);
        public TextView UserDescription => v.FindViewById<TextView>(Resource.Id.lblProfileUserDescription);
        public ImageView CoverPhotoIcon => v.FindViewById<ImageView>(Resource.Id.ProfileCoverCameraIcon);
        public CircleImageView UserPhotoIcon => v.FindViewById<CircleImageView>(Resource.Id.ProfileUserCameraIcon);
        public RecyclerView RecyclerToksList => v.FindViewById<RecyclerView>(Resource.Id.RecyclerProfilePageToks);
        public ShimmerLayout ShimmerToksList => v.FindViewById<ShimmerLayout>(Resource.Id.ShimmerProfilePageToks);
        public NestedScrollView NestedScroll => v.FindViewById<NestedScrollView>(Resource.Id.NestedProfilePage);
        public TextView TxtProfileWordCountry => v.FindViewById<TextView>(Resource.Id.TxtProfileWordCountry);
        public TextView TxtProfileCountryOrState => v.FindViewById<TextView>(Resource.Id.TxtProfileCountrystate);
        public Button AvatarsButton => v.FindViewById<Button>(Resource.Id.btnProfileAvatars);
        public TextView TextLevelRank => v.FindViewById<TextView>(Resource.Id.TextLevelRank);
        public TextView TextLevelPoints => v.FindViewById<TextView>(Resource.Id.TextLevelPoints);
        public ImageView ImgLevel => v.FindViewById<ImageView>(Resource.Id.ImgLevel);
        public TextView TextProfileCoins => v.FindViewById<TextView>(Resource.Id.TextProfileCoins);
        public PhotoView ImgUserImageView => v.FindViewById<PhotoView>(Resource.Id.ImgProfileImageView);
        public Button BtnSubAccnts => v.FindViewById<Button>(Resource.Id.btnProfileSubAccnts);
        public AppBarLayout AppBarProfile => v.FindViewById<AppBarLayout>(Resource.Id.AppBarProfile);
        public SwipeRefreshLayout SwipeRefreshProfile => v.FindViewById<SwipeRefreshLayout>(Resource.Id.SwipeRefreshProfile);
        public TextView UrlEditIcon => v.FindViewById<TextView>(Resource.Id.lblProfileUrlEditIcon);
        public View ViewDummyForTouch => v.FindViewById<View>(Resource.Id.ViewDummyForTouch);
        public LinearLayout LinearProgress => v.FindViewById<LinearLayout>(Resource.Id.LinearProgress);
        public TextView TextNothingFound => v.FindViewById<TextView>(Resource.Id.TextNothingFound);

        public Button BtnCreateSub => v.FindViewById<Button>(Resource.Id.btncreateSubacc);
    }
}