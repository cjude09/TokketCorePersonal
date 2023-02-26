using System;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Xamarin.Essentials;
using Tokket.Shared.Helpers;
using Tokket.Android.Setups;
using Tokket.Core;
using Newtonsoft.Json;
using Tokket.Shared.IoC;
using Google.Android.Material.TextField;
using AuthorizationTokenModel = Tokket.Shared.Models.AuthorizationTokenModel;
using IAccountService = Tokket.Shared.Services.Interfaces.IAccountService;
using AndroidX.AppCompat.App;
using Android.App;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace Tokket.Android.ViewModels
{
    public class LoginPageViewModel : ObservableObject
    {
        #region Properties
        public Activity Instance { get; set; }
        public LoginModel Credentials { get; set; }
        public bool IsFacebook { get; set; }
        public bool IsGoogle { get; set; }

        private bool _isLogin = false;
        /// <summary>
        /// Sets and gets the IsSaving property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return _isLogin;
            }
            set
            {
                if (Set(() => IsLogin, ref _isLogin, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand LoginCommand { get; set; }
        public TextInputLayout EmailInputLayout { get; set; }
        public TextInputLayout PasswordInputLayout { get; set; }
        public LinearLayout linearProgress { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Constructors will be called during the Registration in ViewModelLocator (Applying Dependency Injection or Inversion of Controls)
        /// </summary>
        public LoginPageViewModel()
        {
            Credentials = new LoginModel(); // Initialized Model to avoid nullreference exception
            // Initialize Commands here...
            LoginCommand = new RelayCommand(async () => await Login(), IsLogin);

        }
        #endregion

        #region Methods/Events
        public async Task Login()
        {
            IsLogin = true;
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                if (!String.IsNullOrEmpty(Credentials.Username) && !String.IsNullOrEmpty(Credentials.Password))
                {
                    linearProgress.Visibility = ViewStates.Visible;
                    var result = await AccountService.Instance.Login(Credentials);
                    try
                    {
                        //Add try catch due to crash error that shows "Not Implemented"
                        var resultDb = await AppContainer.Resolve<IAccountService>().Login(Credentials); //await AccountServiceDB.Instance.Login(Credentials);
                    }
                    catch (Exception ex)
                    {
                    }
                    //TokketUser = AuthenticatedUser
                    //AuthorizationTokenModel = AuthorizationTokenModel
                    if (result.ResultEnum == Shared.Helpers.Result.Success && result != null)
                    {
                        //TODO remove cache toks / Monkey Cache
                        //removes all data
                        Barrel.Current.EmptyAll();

                        //Clear cache when user login
                        Settings.ProfileTabContinuationToken = "";
                        Settings.HomeContinuationToken = "";
                        Settings.ContinuationToken = "";

                        EmailInputLayout.Error = null;
                        PasswordInputLayout.Error = null;

                        var resultObject = result.ResultObject.ToString();
                        var userAccount = JsonConvert.DeserializeObject<AuthorizationTokenModel>(resultObject);
                        Settings.UserId = userAccount.UserId;
                        Settings.UserAccount = JsonConvert.SerializeObject(userAccount);

                        TokketUser tokketUser = await AccountService.Instance.GetUserAsync(userAccount.UserId);//userAccount.UserId
                        Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                      
                        userAccount.UserPhoto = tokketUser.UserPhoto;
                        Settings.UserCoins = tokketUser.Coins.Value;

                        var tokens = Settings.GetUserModel();
                        await SecureStorage.SetAsync("idtoken", tokens.IdToken);
                        await SecureStorage.SetAsync("refreshtoken", tokens.RefreshToken);
                        await SecureStorage.SetAsync("userid", tokens.UserId);
                        await SecureStorage.SetAsync("accounttype", tokketUser.AccountType);

                        if (!tokketUser.EmailVerified)
                        {
                            LoginActivity.Instance.ShowEmailVerfy(tokketUser.Email, tokens.IdToken);
                            linearProgress.Visibility = ViewStates.Invisible;
                            //var check = await SharedAccount.AccountService.Instance.SendEmailVerificationAsync(SharedHelpers.Settings.GetTokketUser().Email, SharedHelpers.Settings.GetUserModel().IdToken);
                        }
                        else
                        {
                            if (tokketUser.AccountType == "group")
                            {
                                (Instance as LoginActivity).GoToSubAccountPage();
                               /*var _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
                                _navigationService.NavigateTo(ViewModelLocator.SubAccountPage); // The second parameter of NavigateTo is the model or values to be passed by to the next page
                                //LoginActivity.Instance.Finish();*/
                            }
                            else
                            {
                                (Instance as LoginActivity).GoToMainPage();
                                /*var _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
                                _navigationService.NavigateTo(ViewModelLocator.MainPageKey); // The second parameter of NavigateTo is the model or values to be passed by to the next page
                                //LoginActivity.Instance.Finish();*/
                            }
                        }
                     
                    }
                    else
                    {
                        // Handle error when saving
                        EmailInputLayout.Error = " ";
                        PasswordInputLayout.Error = " ";

                        var builder = new AlertDialog.Builder(Instance);
                        builder.SetMessage("Incorrect email and password!");
                        builder.SetTitle("Error");
                        var dialog = (AlertDialog)null;
                        builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                        {

                        });
                        dialog = builder.Create();
                        dialog.Show();
                        dialog.SetCanceledOnTouchOutside(false);


                        /* var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                         await
                             dialog.ShowError(
                                 "Incorrect email and password!",
                                 "Error",
                                 "OK",
                                 null);*/

                        linearProgress.Visibility = ViewStates.Invisible;
                    }
                }
            }
            else
            {
                var builder = new AlertDialog.Builder(Instance);
                builder.SetMessage("No internet access!");
                builder.SetTitle("Failed to connect!");
                var dialog = (AlertDialog)null;
                builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
                {

                });
                dialog = builder.Create();
                dialog.Show();
                dialog.SetCanceledOnTouchOutside(false);

                /*var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                await
                    dialog.ShowError(
                        "No internet access!",
                        "Failed to connect!",
                        "OK",
                        null);*/
            }
            IsLogin = false;
        }
        #endregion
    }
}