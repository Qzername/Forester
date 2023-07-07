using Forester.Services;
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
    public class AboutViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public IScreen HostScreen { get; }

        //standardize this
        [Reactive] string Version { get; set; }
       
        //dependency injection
        ThemeService theme { get; }

        public AboutViewModel(IScreen screen)
        {
            HostScreen = screen;

            theme = GetService<ThemeService>();

            Version = "3.0v";
        }

        public void Discord()
        {
            Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/***REMOVED***", UseShellExecute = true });
        }
    }
}
