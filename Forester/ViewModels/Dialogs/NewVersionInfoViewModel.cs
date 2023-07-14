using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.Data;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class NewVersionInfoViewModel : DialogBase
    {
        //dependency injection
        VersionDatabase versionDatabase;

        [Reactive] public int value { get; set; }
        IProgress<int> progress;

        public NewVersionInfoViewModel()
        {
            progress = new Progress<int>(percent => { value = percent; });

            versionDatabase = GetService<VersionDatabase>();

            _ = Start();
        }

        public async Task Start()
        {
            await versionDatabase.Download(progress);

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "./Updater/ForesterUpdater.exe"
                    }
                };

                process.Start();

                desktop.Shutdown();
            }
        }
    }
}
