using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.ViewModels.App.Library;
using ReactiveUI;

namespace Forester.ViewModels.Popup.Library
{
    public class DownloadSettingsViewModel : ViewModelBase
    {
        bool useDefaultPath;
        public bool UseDefaultPath
        {
            get => useDefaultPath;
            set => this.RaiseAndSetIfChanged(ref useDefaultPath, value);
        }

        string path;
        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }
       
        bool deleteNotNecessary;
        public bool DeleteNotNecessary
        {
            get => deleteNotNecessary;
            set => this.RaiseAndSetIfChanged(ref deleteNotNecessary, value);
        }

        AppViewModel aVM;
        
        public DownloadSettingsViewModel(AppViewModel aVM)
        {
            this.aVM = aVM;
        }

        public async void SelectPath()
        {
            var dialog = new OpenFolderDialog();

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);

                if (result is null)
                    return;

                Path = result;
            }
        }

        public void Close()
        {
            MainWindowViewModel.Current.ClosePopup();
        }
        
        public void Approve()
        {
            aVM.ConfirmedDownload(UseDefaultPath, Path, DeleteNotNecessary);
            Close();
        }
    }
}
