using Tokket.IoC;
using Tokket.ViewModels.Account;

namespace Tokket.Views;

public partial class LoginSignUpPage : ContentPage
{
    LoginSignUpViewModel ViewModel { get; set; }
    public LoginSignUpPage()
	{
		InitializeComponent();
        ViewModel = AppContainer.Resolve<LoginSignUpViewModel>(new Autofac.NamedParameter(AppContainer.PAGE, this));
        ViewModel.IsLoading = false;
        this.BindingContext = ViewModel;
    }
}