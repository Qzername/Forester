using Forester.ViewModels;
using Forester.ViewModels.App;
using Forester.ViewModels.Dialogs.SettingsDialog;
using Forester.Views;
using Forester.Views.App;
using Forester.Views.Dialogs.SettingsDialog;
using ReactiveUI;
using System;

namespace Forester
{
    public class NavigatorViewLocator : IViewLocator
    {
        IViewFor IViewLocator.ResolveView<T>(T viewModel, string contract)
        {
            switch (viewModel)
            {
                //mainwindow
                case LoginViewModel context:
                    return new LoginView()
                    {
                        DataContext = context,
                    };
                case AppViewModel context:
                    return new AppView()
                    {
                        DataContext = context
                    };
                //app
                case StoreViewModel context:
                    return new StoreView()
                    {
                        DataContext = context
                    };
                case LibraryViewModel context:
                    return new LibraryView()
                    {
                        DataContext = context
                    };
                case DeveloperViewModel context:
                    return new DeveloperView()
                    {
                        DataContext = context
                    };
                //settingsdialog
                case AboutViewModel context:
                    return new AboutView()
                    {
                        DataContext = context
                    };
                case ThemeViewModel context:
                    return new ThemeView()
                    {
                        DataContext = context
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewModel));
            }
        }
    }
}
