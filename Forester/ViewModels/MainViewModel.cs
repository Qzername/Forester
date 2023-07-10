using Avalonia.Platform;
using Forester.Data;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.ViewModels.Bases;
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
            windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.OnChangeConfiguration += WindowConfigurationService_OnChangeChromeHints;
            windowConfigurationService.OnLogOutCalled += WindowConfigurationService_OnLogOutCalled;

            Router = new RoutingState();

            MoveToLogin();
        }

        private void WindowConfigurationService_OnLogOutCalled()
        {
            var settingsFile = GetService<SettingsFile>();
            settingsFile.SetAutoLogin(new Models.Account());
            settingsFile.SaveSettings();

            MoveToLogin();
        }

        private void WindowConfigurationService_OnChangeChromeHints(WindowConfiguration configuration)
        {
            ClientAreaChromeHints = configuration.IsChromeOn ? ExtendClientAreaChromeHints.Default : ExtendClientAreaChromeHints.NoChrome;
            TitlebarHeight = configuration.TitleBarHeight;
        }

        void MoveToLogin()
        {
            Router.Navigate.Execute(new LoginViewModel(this));
        }
    }
}
