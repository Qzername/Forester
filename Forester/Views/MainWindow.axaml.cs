using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Tools;
using Forester.ViewModels;
using ReactiveUI;
using Splat;

namespace Forester.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    double OldExtendClientAreaTitleBarHeightHint;

    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        PropertyChanged += MainWindow_PropertyChanged;

        var windowConfigurationService = Locator.Current.GetService<WindowConfigurationService>();
        windowConfigurationService.OnChangeConfiguration += WindowConfigurationService_OnChangeConfiguration;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void WindowConfigurationService_OnChangeConfiguration(WindowConfiguration configuration)
    {
        if (configuration.TitleBarHeight != OldExtendClientAreaTitleBarHeightHint)
            OldExtendClientAreaTitleBarHeightHint = configuration.TitleBarHeight;
    }

    private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        //hacky approach to avalonia's bug
        //see: https://github.com/AvaloniaUI/Avalonia/issues/9042 for more info
        if (e.Property.PropertyType == typeof(WindowState))
        {
            if ((WindowState)e.NewValue == WindowState.Maximized)
            {
                Padding = new Thickness(8);
                ExtendClientAreaTitleBarHeightHint = OldExtendClientAreaTitleBarHeightHint + 8;
            }
            else
            {
                Padding = new Thickness(0);
                ExtendClientAreaTitleBarHeightHint = OldExtendClientAreaTitleBarHeightHint;
            }
        }
    }
}
