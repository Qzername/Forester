using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Forester.ViewModels;
using Forester.Views;

namespace Forester
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new LoginPanel
                {
                    DataContext = new ViewModels.LoginPanelViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
