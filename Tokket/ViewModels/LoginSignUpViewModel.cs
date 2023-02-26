using Newtonsoft.Json;
using System.Diagnostics;
using Tokket.Core;
using Tokket.Shared.Helpers;
using models = Tokket.Shared.Models;
using Tokket.Shared.Services;
using svc = Tokket.Shared.Services.Interfaces;
using Tokket.Models;
using Tokket.Views.Control;

namespace Tokket.ViewModels.Account
{
    public class LoginSignUpViewModel : BaseViewModel
    {
        readonly svc.IAccountService _accountService;
        public Command LoginCommand { get; }
        public Command SignUpCommand { get; }

        public Command GoToLoginCommand { get; }
        public Command GoToSignUpCommand { get; }

        public Command TermsCommand { get; }
        public WaitingView Loader { get; set; }

        List<TokketSubaccount> SubAccntList;

        List<string> Colors = new List<string>() {
               "#4472C7", "#732FA0", "#05adf4", "#73AD46",
               "#E23DB5", "#BE0400", "#195B28", "#E88030",
               "#873B09", "#FFC100"
               };
        List<string> ColorItems = new List<string>();
        int lastrows = 0, lastcolor = 0;
        public string currentuser = "";

        private string email = "bonquit@gmail.com";
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private string password = "bonquit";
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string confirmpassword = "";
        public string ConfirmPassword
        {
            get { return confirmpassword; }
            set { SetProperty(ref confirmpassword, value); }
        }

        private bool _canProceed = false;
        public bool CanProceed
        {
            get { return _canProceed; }
            set { SetProperty(ref _canProceed, value); }
        }

        public TokketUser User { get; set; } = new TokketUser();

        private bool isSignUpMode = false;
        public bool IsSignUpMode
        {
            get { return isSignUpMode; }
            set { SetProperty(ref isSignUpMode, value); }
        }

        private bool isLoginMode = true;
        public bool IsLoginMode
        {
            get { return isLoginMode; }
            set { SetProperty(ref isLoginMode, value); }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }


        public LoginSignUpViewModel(Page page, svc.IAccountService accountService) : base(page)
        {
            _accountService = accountService;

            LoginCommand = new Command(OnLoginClicked);
            SignUpCommand = new Command(OnSignUpClicked);
            GoToLoginCommand = new Command(OnGoToLoginClicked);
            //GoToSignUpCommand = new Command(OnGoToSignUpClicked);
            TermsCommand = new Command(OnTermsClicked);

        }

        private async void OnLoginClicked(object obj)
        {
            try
            {
                IsLoading = false;
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                {
                    //Loader.IsVisible = true;
                    IsLoading = true;

                    models.LoginModel Credentials = new models.LoginModel() { Username = Email, Password = Password };
                    var result = await AccountService.Instance.Login(Credentials);
                    if (result.ResultEnum == Result.Success)
                    {
                        var resultObject = result.ResultObject.ToString();
                        var userAccount = JsonConvert.DeserializeObject<models.AuthorizationTokenModel>(resultObject);
                        TokketUser tokketUser = await AccountService.Instance.GetUserAsync(userAccount.UserId);
                        App.TokFilter.publicfeed = false;

                        //show the new page for loggin in subaccount 
                        if (tokketUser.AccountType.ToLower() == "group")
                        {
                            App.tokketUser = tokketUser;
                            App.SelectedUser = JsonConvert.DeserializeObject<TokketUserModel>(JsonConvert.SerializeObject(tokketUser));
                            userAccount.UserPhoto = tokketUser.UserPhoto;
                            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                            Settings.UserAccount = JsonConvert.SerializeObject(userAccount);
                            Settings.UserCoins = tokketUser.Coins.Value;

                            var tokens = Settings.GetUserModel();
                            await SecureStorage.SetAsync("idtoken", tokens.IdToken);
                            await SecureStorage.SetAsync("refreshtoken", tokens.RefreshToken);
                            await SecureStorage.SetAsync("token_expiry", tokens.TokenExpiration.ToString());
                            await SecureStorage.SetAsync("userid", tokens.UserId);
                            await SecureStorage.SetAsync("accounttype", tokketUser.AccountType);



                            var result1 = await AccountService.Instance.GetSubaccountsAsync(tokketUser.Id);
                            SubAccntList = new List<TokketSubaccount>();
                            SubAccntList.AddRange(result1.Results.ToList());

                            //Application.Current.MainPage =
                            //    new SubAccountPage(SubAccntList, tokketUser.DisplayName, tokketUser.GroupAccountType);

                        }
                        else
                        {
                            App.tokketUser = tokketUser;
                            App.SelectedUser = JsonConvert.DeserializeObject<TokketUserModel>(JsonConvert.SerializeObject(tokketUser));
                            userAccount.UserPhoto = tokketUser.UserPhoto;
                            Settings.TokketUser = JsonConvert.SerializeObject(tokketUser);
                            Settings.UserAccount = JsonConvert.SerializeObject(userAccount);
                            Settings.UserCoins = tokketUser.Coins.Value;

                            //#region App Shell UI
                            //Application.Current.MainPage = App.AppShellInstance = new AppShell();
                            //Shell.SetTabBarIsVisible(Shell.Current, true);

                            //if (string.IsNullOrEmpty(tokketUser.CoverPhoto))
                            //{
                            //    App.AppShellInstance.FlyoutCoverPhoto.IsVisible = false;
                            //}
                            //else
                            //{
                            //    App.AppShellInstance.FlyoutCoverPhoto.IsVisible = true;
                            //    App.AppShellInstance.FlyoutCoverPhoto.Source = tokketUser.CoverPhoto;
                            //}

                            //App.AppShellInstance.FlyoutUserPhoto.Source = tokketUser.UserPhoto;
                            //App.AppShellInstance.FlyoutUserDisplayName.Text = tokketUser.DisplayName;
                            //App.AppShellInstance.FlyoutUserTitleSubaccount.Text = tokketUser?.TitleId ?? "";
                            //App.AppShellInstance.FlyoutUserCoinsLbl.Text = tokketUser.Coins.ToString();

                            ////await Shell.Current.GoToAsync($"..");
                            //#endregion

                            //App.AppShellInstance.FlyoutUserArrow.IsVisible = false;
                            var tokens = Settings.GetUserModel();
                            await SecureStorage.SetAsync("idtoken", tokens.IdToken);
                            await SecureStorage.SetAsync("refreshtoken", tokens.RefreshToken);
                            await SecureStorage.SetAsync("token_expiry", tokens.TokenExpiration.ToString());
                            await SecureStorage.SetAsync("userid", tokens.UserId);
                            await SecureStorage.SetAsync("accounttype", tokketUser.AccountType);

                            await PageInstance.DisplayAlert("Welcome!", $"Glad your here {tokketUser.DisplayName}", "OK");
                        }
                      
                       
                       
                    }
                    else
                    {
                        // Handle error when saving
                        await Application.Current.MainPage.DisplayAlert("Error", "Incorrect email and password!", "Ok");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", "Please enter your email and password.", "OK");
                }
                IsLoading = false;
                //Loader.IsVisible = false;


                //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            }
            catch (Exception ex)
            {
            //    Crashes.TrackError(ex, new Dictionary<string, string> { { "user_id", App.tokketUser?.Id }, { "Login Error", ex.ToString() } });
            }

        }


        private async void OnSignUpClicked(object obj)
        {
            IsLoading = true;

            var account = await AccountService.Instance.GetUserByEmailAsync(Email);
            if (account != null)
            {
                IsLoading = false;
                await PageInstance.DisplayAlert("Failed", "Email account taken already!", "OK");

            }
            else
            {
                var res = await _accountService.SignUpAsync(Email, Password, User.DisplayName, User.Country, User.Birthday, User.UserPhoto, User.AccountType, User.GroupAccountType, User.DisplayName);
                if (res.ResultEnum == Result.Success)
                {
                    OnLoginClicked(null);
                    await PageInstance.DisplayAlert("Success", res.ResultMessage, "OK");
                }
                else
                {
                    IsLoading = false;
                    await PageInstance.DisplayAlert("Failed", res.ResultMessage, "OK");
                }

            }


        }

        private async void OnGoToLoginClicked(object obj)
        {
            IsSignUpMode = false;
            IsLoginMode = true;
        }

        //private async void OnGoToSignUpClicked(object obj)
        //{
        //    App.Current.MainPage = new SignUpPage();
        //}

        private async void OnTermsClicked(object obj)
        {
            IsSignUpMode = false;
            IsLoginMode = true;
        }

        //private async Task<List<TokketSubaccount>> GetSubAccounts(string userid)
        //{
        //    try
        //    {
        //        var result = await AccountService.Instance.GetSubaccountsAsync(userid);
        //        SubAccntList.AddRange(result.Results.ToList());
        //        var getter = result.Results.ToList();

        //        return result.Results.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        return null;
        //    }


        //}



    }
}
