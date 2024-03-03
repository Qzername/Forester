using Avalonia.Platform;
using Forester.Data;
using Forester.Data.Connection;
using Forester.Models.API;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Tools;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace Forester.ViewModels
{
    public class MainViewModel : ViewModelBase, IScreen
    {
        public RoutingState Router { get; }

        [Reactive] ExtendClientAreaChromeHints ClientAreaChromeHints { get; set; }
        [Reactive] int TitlebarHeight { get; set; }

        //Dependecy injection
        WindowConfigurationService windowConfigurationService;
        

        public MainViewModel()
        {
            AutoUpdateCheck();

            windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.OnChangeConfiguration += WindowConfigurationService_OnChangeChromeHints;
            windowConfigurationService.OnLogOutCalled += WindowConfigurationService_OnLogOutCalled;

            Router = new RoutingState();

            MoveToLogin();
        }

        async void AutoUpdateCheck()
        {
            string version = GetService<SettingsFile>().ForesterData.Version;

            string newestVersion = await GetService<VersionDatabase>().GetVersion();

            if (version ==newestVersion)
                return;

            var dialogService = GetService<DialogService>();
            
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Content = new NewVersionInfoViewModel(),
                Width = 500,
                Height = 153,
            });
            dialogService.ChangeVisibility(true);
        }
        
        private void WindowConfigurationService_OnLogOutCalled()
        {
            var settingsFile = GetService<SettingsFile>();
            settingsFile.SetAutoLogin(new Account());
            settingsFile.SaveSettings();

            MoveToLogin();
        }

        private void WindowConfigurationService_OnChangeChromeHints(WindowConfiguration configuration)
        {
            ClientAreaChromeHints = configuration.IsChromeOn ? ExtendClientAreaChromeHints.Default : ExtendClientAreaChromeHints.NoChrome;
            TitlebarHeight = configuration.TitleBarHeight;
        }

        async void MoveToLogin()
        {
            var settingsFile = GetService<SettingsFile>();
            var requestManager = GetService<AccountDatabase>();

            //autologin turned off or login and password is incorrect
            if (string.IsNullOrEmpty(settingsFile.Settings.AutoLogin.Login) ||
               string.IsNullOrEmpty(settingsFile.Settings.AutoLogin.Password) ||
               !await requestManager.Login(new Account()
               {
                   Login = settingsFile.Settings.AutoLogin.Login,
                   Username = settingsFile.Settings.AutoLogin.Username,
                   Password = settingsFile.Settings.AutoLogin.Password,
               }))
            {
                Router.Navigate.Execute(new LoginViewModel(this));
                return;
            }

            //autologin succeed
            await GetService<UserDataService>().SetAccount(settingsFile.Settings.AutoLogin.Login);
            Router.Navigate.Execute(new AppViewModel(this));
        }
    }
}
