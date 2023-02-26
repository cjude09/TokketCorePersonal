using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Tokket.Android.ViewModels;

namespace Tokket.Android.Setups
{
    public class ViewModelLocator
    {
        public const string LoginPageKey = "LoginPage";
        public const string MainPageKey = "MainPage";
        public const string SubAccountPage = "SubAccountPage";
        public const string MySetPageKey = "MySetPage";
        public const string MySetsViewPageKey = "MySetsViewPageKey";
        public const string MyClassSetsViewPageKey = "MyClassSetsViewPageKey";
        public const string AddSetPageKey = "AddSetPage";
        public const string AddClassSetPageKey = "AddClassSetPageKey";
        public const string SignupPageKey = "SignupPage";
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<HomePageViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<TokCardsPageViewModel>();
            SimpleIoc.Default.Register<AddTokViewModel>();
            SimpleIoc.Default.Register<MySetsViewModel>();
            SimpleIoc.Default.Register<AddSetPageViewModel>();
            SimpleIoc.Default.Register<TokInfoViewModel>();
        }
        public LoginPageViewModel LoginPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<LoginPageViewModel>();
            }
        }
        public MySetsViewModel MySetsPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MySetsViewModel>();
            }
        }

        public AddTokViewModel AddTokPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<AddTokViewModel>();
            }
        }
        public AddSetPageViewModel AddSetPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<AddSetPageViewModel>();
            }
        }
        public ProfilePageViewModel ProfilePageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ProfilePageViewModel>();
            }
        }
        public HomePageViewModel HomePageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<HomePageViewModel>();
            }
        }
        public TokInfoViewModel TokInfoPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<TokInfoViewModel>();
            }
        }
        public TokCardsPageViewModel TokCardsPageVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<TokCardsPageViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}