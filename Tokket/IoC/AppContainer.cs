using Autofac;
using Tokket.Shared.Services;
using Tokket.Shared.Services.Interfaces;
using Tokket.ViewModels.Account;
using Tokket.ViewModels.Config;
using IContainer = Autofac.IContainer;
//using Tokket.ViewModels.Training;

namespace Tokket.IoC
{
    public class AppContainer
    {
        private static IContainer _container;

        public static string PAGE = "page";
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //== ViewModels
            builder.RegisterType<LoginSignUpViewModel>().WithParameter(PAGE, ViewModelConfig.LOGIN_SIGNUP);

            //== Services
            builder.RegisterType<ClassService>().As<IClassService>();
            builder.RegisterType<TokPakService>().As<ITokPakService>();
            builder.RegisterType<TokService>().As<ITokService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<CommonService>().As<ICommonService>();
            builder.RegisterType<BadgeService>().As<IBadgeService>();
           

            _container = builder.Build();
        }
        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>(NamedParameter param_)
        {
            return _container.Resolve<T>(param_);
        }

        public static T Resolve<T>(NamedParameter param, NamedParameter param2)
        {
            return _container.Resolve<T>(param, param2);
        }

    }
}
