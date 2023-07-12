using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Forester.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services
{

    public class DialogService
    {
        public delegate void ChangeDialogConfiguration(DialogConfiguration configuration);
        public delegate void SwitchDialogVisibility(bool isVisible);

        public event ChangeDialogConfiguration OnConfigurationChange;
        public event SwitchDialogVisibility OnVisibilityChange;

        public void ChangeConfiguration(DialogConfiguration configuration)
        {
            OnConfigurationChange?.Invoke(configuration);
        }

        public void ChangeVisibility(bool isVisible)
        {
            OnVisibilityChange?.Invoke(isVisible);
        }

        public async Task<string> OpenFileDialog(FilePickerOpenOptions options)
        {
            string result = string.Empty;

            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var files = await desktop.MainWindow!.StorageProvider.OpenFilePickerAsync(options);

                if (files.Count == 0)
                    return "";

                string? tempResult = files[0].Path.LocalPath;

                if (result is null)
                    return "";

                result = tempResult;
            }

            return result;
        }

        public async Task<string> OpenDirectoryDialog(FolderPickerOpenOptions options)
        {
            string result = string.Empty;

            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var files = await desktop.MainWindow!.StorageProvider.OpenFolderPickerAsync(options);

                if (files.Count == 0)
                    return "";

                string? tempResult = files[0].Path.LocalPath;

                if (result is null)
                    return "";

                result = tempResult;
            }

            return result;
        }
    }
}
