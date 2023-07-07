using Forester.Data;
using Forester.Data.Connection;
using Forester.Services;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    internal static class Bootstraper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            // --- avalonia ---
            services.RegisterLazySingleton(() => new SettingsFile(), typeof(SettingsFile));
            services.RegisterLazySingleton(() => new ThemeService(resolver.GetService<SettingsFile>()!), typeof(ThemeService));

            // --- data ---

            //managers
            services.RegisterLazySingleton(() => new RequestManager(), typeof(RequestManager));

            //databases
            services.RegisterLazySingleton(() => new AccountDatabase(resolver.GetService<RequestManager>()!), typeof(AccountDatabase));

            // --- other ---
            services.RegisterLazySingleton(() => new WindowConfigurationService(), typeof(WindowConfigurationService));
            services.RegisterLazySingleton(() => new DialogService(), typeof(DialogService));
        }
    }
}
