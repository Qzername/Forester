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
        bool _useDefaultPath;
        public bool useDefaultPath
        {
            get => _useDefaultPath;
            set => this.RaiseAndSetIfChanged(ref _useDefaultPath, value);
        }

        string _path;
        public string path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }
       
        bool _deleteNotNecessary;
        public bool deleteNotNecessary
        {
            get => _deleteNotNecessary;
            set => this.RaiseAndSetIfChanged(ref _deleteNotNecessary, value);
        }

        public async void SelectPath()
        {
            var dialog = new OpenFolderDialog();

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);

                if (result is null)
                    return;

                path = result;
            }
        }

        public void Close()
        {
            MainWindowViewModel.Current.ClosePopup();
        }
        
        public void Approve()
        {
            AppViewModel.Current.ConfirmedDownload(useDefaultPath, path, deleteNotNecessary);
            Close();
        }
    }
}
