using Avalonia.Platform;
using Forester.Models;
using Forester.Services;
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
            windowConfigurationService.OnChangeChromeHints += WindowConfigurationService_OnChangeChromeHints;

            Router = new RoutingState();

            Router.Navigate.Execute(new LoginViewModel(this));
        }

        private void WindowConfigurationService_OnChangeChromeHints(WindowConfiguration configuration)
        {
            ClientAreaChromeHints = configuration.IsChromeOn ? ExtendClientAreaChromeHints.Default : ExtendClientAreaChromeHints.NoChrome;
            TitlebarHeight = configuration.TitleBarHeight;
        }
    }
}
