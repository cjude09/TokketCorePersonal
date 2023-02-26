using Autofac;
using Tokket.Shared.Services;

namespace Tokket.Shared.IoC
{
    public class AppContainer
    {
        private static IContainer? _container;

        public static void RegisterAndroidDependencies() {

            var builder = new ContainerBuilder();

            //== Services
            //builder.RegisterType<ClassService>().As<IClassService>();
            //builder.RegisterType<TokPakService>().As<ITokPakService>();
            //builder.RegisterType<TokService>().As<ITokService>();
            //builder.RegisterType<AccountService>().As<IAccountService>();
            //builder.RegisterType<CommonService>().As<ICommonService>();
            //builder.RegisterType<BadgeService>().As<IBadgeService>();
           // builder.RegisterType<AccountServiceDB>().As<IAccountService>();
            builder.RegisterType<AccountServiceDB>().As<Tokket.Shared.Services.Interfaces.IAccountService>();
            builder.RegisterType<Shared.Services.ServicesDB.ClassTokServiceDB>().As<Shared.Services.Interfaces.IClassTokService>();
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

    }
}
