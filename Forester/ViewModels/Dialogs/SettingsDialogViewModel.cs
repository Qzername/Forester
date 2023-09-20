using Forester.Services;
using Forester.ViewModels.Dialogs.SettingsDialog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class SettingsDialogViewModel : DialogBase, IScreen
    {
        //dependency injection
        ThemeService theme { get; }
        public RoutingState Router { get; }

        public SettingsDialogViewModel() 
        { 
            Router = new RoutingState();

            theme = GetService<ThemeService>();

            About();
        }

        //yes, i should standardize this
        //yes, i am lazy
        public void About() => Router.Navigate.Execute(new AboutViewModel(this));
        public void Theme() => Router.Navigate.Execute(new ThemeViewModel(this));
        public void Account() => Router.Navigate.Execute(new AccountViewModel(this));

        public void LogOut()
        {
            GetService<WindowConfigurationService>().LogOut();
            CloseDialog();
        }
    }
}
