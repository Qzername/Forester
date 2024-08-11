using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.Data;
using Forester.Services;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;

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

            Version = GetService<SettingsFile>().ForesterData.Version;
        }

        public void Github()
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/Qzername/Forester", UseShellExecute = true });
        }
    }
}
