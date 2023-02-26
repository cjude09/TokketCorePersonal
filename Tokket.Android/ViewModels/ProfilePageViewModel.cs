using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Android.Adapters;
using Tokket.Android.Helpers;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Tokket.Core;
using GalaSoft.MvvmLight;

namespace Tokket.Android.ViewModels
{
    public class ProfilePageViewModel : ObservableObject
    {
        #region Properties
        public string UserId { get; set; }
        public TokketUser tokketUser { get; set; }
        public long? userpoints, Coins;
        public string UserDisplayName;
        public AdapterFragment fragment { get; private set; }
        #endregion

        #region Commands
        public ImageView ProfileCoverPhoto { get; set; }
        public ImageView ProfileUserPhoto { get; set; }
        public Activity activity { get; set; }
        public GlideImgListener GListenerCover {get;set;}
        public GlideImgListener GListenerUserPhoto { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Constructors will be called during the Registration in ViewModelLocator (Applying Dependency Injection or Inversion of Controls)
        /// </summary>
        public ProfilePageViewModel()
        {
            tokketUser = new TokketUser(); // Initialized Model to avoid nullreference exception
        }
        #endregion

        #region Methods/Events
        public async Task Initialize(TokketUser _tokketUser = null)
        {
            if (Settings.ActivityInt == (int)ActivityType.ProfileTabActivity)
            {
                tokketUser = _tokketUser;
            }
            else
            {
                tokketUser = AccountService.Instance.GetUser(UserId);
            }

            var cacheUserPhoto = MainActivity.Instance.cacheUserPhoto;
            string userprofilepic = "", selectedavatar = "";
            bool? isAvatarProfilePic = false;

            if (UserId == Settings.GetTokketUser().Id)
            {
                if (Settings.GetTokketUser().AccountType == "group")
                {
                    selectedavatar = ""; // Settings.GetTokketSubaccount().SelectedAvatar;
                    isAvatarProfilePic = false; // Settings.GetTokketSubaccount().IsAvatarProfilePicture;
                    userpoints = Settings.GetTokketSubaccount().Points;
                    UserDisplayName = Settings.GetTokketSubaccount().SubaccountName;
                    Coins = Settings.GetTokketSubaccount().Coins;
                }
                else
                {
                    selectedavatar = tokketUser?.SelectedAvatar;
                    isAvatarProfilePic = tokketUser.IsAvatarProfilePicture;
                    userpoints = tokketUser.Points;
                    UserDisplayName = tokketUser.DisplayName;
                    Coins = tokketUser.Coins;
                }
            }
            else
            {
                isAvatarProfilePic = tokketUser.IsAvatarProfilePicture;
                userpoints = tokketUser.Points;
                UserDisplayName = tokketUser.DisplayName;
                Coins = tokketUser.Coins;
            }

            bool isCachedUserPhotoUsed = false;
            if (isAvatarProfilePic == true)
            {
                var listavatar = new List<string>();

                listavatar.Add(selectedavatar);
                var result = await AvatarsService.Instance.AvatarsByIdsAsync(listavatar);
                var resultlist = result.Results.ToList();
                userprofilepic = resultlist[0].Image;
            }
            else
            {
                if (!string.IsNullOrEmpty(cacheUserPhoto) && UserId == Settings.GetTokketUser().Id)
                {
                    isCachedUserPhotoUsed = true;
                }
                else
                {
                    userprofilepic = tokketUser.UserPhoto;
                }
            }

            if (!isCachedUserPhotoUsed)
            {
                GListenerUserPhoto = new GlideImgListener();
                GListenerUserPhoto.ParentActivity = activity;
                Glide.With(activity).AsBitmap().Load(userprofilepic).Listener(GListenerUserPhoto).Apply(RequestOptions.NoTransformation().Placeholder(Resource.Drawable.Man3)).Into(ProfileUserPhoto);
            }
            else
            {
                ProfileUserPhoto.SetImageBitmap(MainActivity.Instance.bitmapUserPhoto);
            }
            
            //Glide.With(activity).Load(tokketUser.CoverPhoto).Into(ProfileCoverPhoto);

            GListenerCover = new GlideImgListener();
            GListenerCover.ParentActivity = activity;

            Glide.With(activity)
                .Load(tokketUser.CoverPhoto)
                .Listener(GListenerCover)
                .Into(ProfileCoverPhoto);
        }
        #endregion
    }
}