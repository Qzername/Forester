using Forester.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class SettingsDialogViewModel : ViewModelBase
    {
        //dependency injection
        ThemeService theme { get; }

        public SettingsDialogViewModel() 
        { 
            theme = GetService<ThemeService>();
        }
    }
}
