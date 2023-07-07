using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.ViewModels;
using Splat;

namespace Forester.Controls
{
    public partial class Dialog : UserControl
    {
        //dependency injection
        WindowConfigurationService windowConfigurationService { get; }

        int currentTitlebarHeight;

        public Dialog()
        {
            InitializeComponent();

            var dialogService = GetService<DialogService>();
            dialogService.OnConfigurationChange += SetConfiguration;
            dialogService.OnVisibilityChange += SwitchVisibility;

            windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.OnChangeConfiguration += WindowConfigurationService_OnChangeConfiguration;

            SwitchVisibility(false);
        }

        //maybe its hacky workaround but it works :thumbsup:
        private void WindowConfigurationService_OnChangeConfiguration(WindowConfiguration configuration)
        {
            currentTitlebarHeight = configuration.IsChromeOn ? configuration.TitleBarHeight : 0;
        }

        void SetConfiguration(DialogConfiguration configuration)
        {
            Frame.Width = configuration.Width;
            Frame.Height = configuration.Height;
            DialogContentControl.Content = configuration.Content;

            MainPanel.Margin = new Thickness(0, currentTitlebarHeight, 0, 0);
        }

        void SwitchVisibility(bool isVisible) => MainPanel.IsVisible = isVisible;

        T GetService<T>() => Locator.Current.GetService<T>()!;
    }
}
