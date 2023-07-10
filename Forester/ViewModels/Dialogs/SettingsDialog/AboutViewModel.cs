using Forester.Services;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs.SettingsDialog
{
    public class AboutViewModel : RoutableBase
    {
        //standardize this
        [Reactive] string Version { get; set; }
       
        //dependency injection
        ThemeService theme { get; }

        public AboutViewModel(IScreen screen) : base(screen)
        {
            theme = GetService<ThemeService>();

            Version = "3.0v";
        }

        public void Discord()
        {
            Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/***REMOVED***", UseShellExecute = true });
        }
    }
}
