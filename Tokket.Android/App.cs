using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Tokket.Android.Setups;

namespace Tokket.Android
{
    public partial class App
    {
        private static ViewModelLocator _locator;
        public static ViewModelLocator Locator
        {
            get
            {
                if (_locator == null)
                {
                    // First time initialization
                    var nav = new NavigationService();
                    nav.Configure(
                        ViewModelLocator.LoginPageKey,
                        typeof(LoginActivity));
                    nav.Configure(
                        ViewModelLocator.MainPageKey,
                        typeof(MainActivity));
                    nav.Configure(
                        ViewModelLocator.SubAccountPage,
                        typeof(SubAccountActivity));
                    nav.Configure(
                        ViewModelLocator.MySetPageKey,
                        typeof(MySetsActivity));
                    nav.Configure(
                        ViewModelLocator.MySetsViewPageKey,
                        typeof(MySetsViewActivity));
                    nav.Configure(
                        ViewModelLocator.MyClassSetsViewPageKey,
                        typeof(MyClassSetsViewActivity));
                    nav.Configure(
                        ViewModelLocator.AddSetPageKey,
                        typeof(AddSetActivity));
                    nav.Configure(
                        ViewModelLocator.AddClassSetPageKey,
                        typeof(AddClassSetActivity));
                    nav.Configure(
                        ViewModelLocator.SignupPageKey,
                        typeof(SignupActivity));

                    SimpleIoc.Default.Register<INavigationService>(() => nav);
                    SimpleIoc.Default.Register<IDialogService, DialogService>();

                    _locator = new ViewModelLocator();
                }

                return _locator;
            }
        }
    }
}