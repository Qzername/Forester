using Forester.Data;
using Forester.Data.Connection;
using Forester.Services;
using Forester.Services.App;
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
            services.RegisterLazySingleton(() => new ThemeService(GetService<SettingsFile>(resolver)), typeof(ThemeService));

            // --- dialogs ---
            services.RegisterLazySingleton(() => new DialogService(), typeof(DialogService));
            services.RegisterLazySingleton(() => new ErrorMessageService(GetService<DialogService>(resolver)), typeof(ErrorMessageService));

            // --- data ---

            //managers
            services.RegisterLazySingleton(() => new RequestManager(), typeof(RequestManager));
            services.RegisterLazySingleton(() => new FileTransferManager(), typeof(FileTransferManager));

            //databases
            var requestManager = GetService<RequestManager>(resolver);
            var fileTransferManager = GetService<FileTransferManager>(resolver);
            var errorMessage = GetService<ErrorMessageService>(resolver);

            services.RegisterLazySingleton(() => new AccountDatabase(fileTransferManager, requestManager, errorMessage), typeof(AccountDatabase));
            services.RegisterLazySingleton(() => new ApplicationDatabase(requestManager, errorMessage), typeof(ApplicationDatabase));
            services.RegisterLazySingleton(() => new PictureDatabase(fileTransferManager, errorMessage), typeof(PictureDatabase));

            services.RegisterLazySingleton(() => new ApplicationFileDatabase(fileTransferManager, errorMessage), typeof(ApplicationFileDatabase));

            // --- app ---
            services.RegisterLazySingleton(()=> new DeveloperService(), typeof(DeveloperService));
            services.RegisterLazySingleton(() => new LibraryService(), typeof(LibraryService));

            // --- other ---
            services.RegisterLazySingleton(() => new WindowConfigurationService(), typeof(WindowConfigurationService));
            services.RegisterLazySingleton(() => new UserDataService(GetService<AccountDatabase>(resolver)), typeof(UserDataService));
            services.RegisterLazySingleton(() => new PictureService(GetService<PictureDatabase>(resolver), GetService<ThemeService>(resolver)), typeof(PictureService));
        }

        static T GetService<T>(IReadonlyDependencyResolver resolver)
        {
            return resolver.GetService<T>()!;
        }
    }
}
