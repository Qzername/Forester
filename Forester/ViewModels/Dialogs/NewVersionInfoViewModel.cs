using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.Data;
using Forester.Services;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    /// <summary>
    /// Dialog that is being shown whenever there is new version available 
    /// </summary>
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
                string updaterPath = PathHelper.GetPath("Updater/ForesterUpdater");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = updaterPath
                    }
                };

                if(!File.Exists(updaterPath))
                {
                    GetService<ErrorMessageService>().SendErrorMessage(new Models.Configurations.ErrorMessage()
                    {
                        FriendlyMessage = "No updater found! That means that Forester doesn't have necessary files to update itself to newest version. Please reinstall Forester.",
                        TechnicalMessage = "Forester.ViewModels.Dialogs.NewVersionInfoViewModel(Start)",
                    });
                    return;
                }

                process.Start();

                desktop.Shutdown();
            }
        }
    }
}
