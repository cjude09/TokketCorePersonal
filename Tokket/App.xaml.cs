using Tokket.Core;
using Tokket.IoC;
using Tokket.Models;
using Tokket.Shared.Models;
using Tokket.Views;

namespace Tokket
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppContainer.RegisterDependencies();
            MainPage = new LoginSignUpPage(); 
        }
        public static AppShell AppShellInstance { get; set; }
        public static ClassTokQueryValues TokFilter { get; set; } = new ClassTokQueryValues();

        public static TokketUser tokketUser { get; set; }


        public static TokketUserModel SelectedUser { get; set; }
    }
}