using Forester.Services;
using Forester.Tools;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs.SettingsDialog
{
    public class AccountViewModel : RoutableBase
    {
        //dependency injection
        ThemeService theme { get; }
        UserDataService userDataService { get; set; }

        public AccountViewModel(IScreen screen) : base(screen)
        {
            theme = GetService<ThemeService>();
            userDataService = GetService<UserDataService>();
        }
    }
}
