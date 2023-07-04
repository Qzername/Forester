using Forester.Data;
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
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsFile(), typeof(SettingsFile));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ThemeService(resolver.GetService<SettingsFile>()!), typeof(ThemeService));
        }
    }
}
