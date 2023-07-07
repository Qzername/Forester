using Forester.ViewModels;
using Forester.ViewModels.Dialogs.SettingsDialog;
using Forester.Views;
using Forester.Views.Dialogs.SettingsDialog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
