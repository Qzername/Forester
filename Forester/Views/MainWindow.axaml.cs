using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Forester.ViewModels;
using ReactiveUI;

namespace Forester.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    double OldExtendClientAreaTitleBarHeightHint;

    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        PropertyChanged += MainWindow_PropertyChanged;

        OldExtendClientAreaTitleBarHeightHint = ExtendClientAreaTitleBarHeightHint;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void MainWindow_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        //hacky approach to avalonia's bug
        //see: https://github.com/AvaloniaUI/Avalonia/issues/9042 for more info
        if (e.Property.PropertyType == typeof(WindowState))
        {
            if ((WindowState)e.NewValue == WindowState.Maximized)
            {
                Padding = new Avalonia.Thickness(8);
                ExtendClientAreaTitleBarHeightHint = OldExtendClientAreaTitleBarHeightHint + 8;
            }
            else
            {
                Padding = new Avalonia.Thickness(0);
                ExtendClientAreaTitleBarHeightHint = OldExtendClientAreaTitleBarHeightHint;
            }
        }
    }
}
