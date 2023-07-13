using Avalonia.Platform.Storage;
using Forester.Models.App;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Runtime.Intrinsics.X86;

namespace Forester.ViewModels.Dialogs
{
    public class DownloadSettingsViewModel : DialogBase
    {
        [Reactive] bool useDefaultPath { get; set; }
        [Reactive] string path { get; set; }
        [Reactive] bool deleteNotNecessary { get; set; }

        //dependency injection
        DialogService dialogService;
        LibraryService libraryService;

        string applicationName;

        public DownloadSettingsViewModel(string applicationName)
        {
            this.applicationName = applicationName;

            path = "None";

            dialogService = GetService<DialogService>();
            libraryService = GetService<LibraryService>();
        }

        public async void SelectFolder()
        {
            path = await dialogService.OpenDirectoryDialog(new FolderPickerOpenOptions());

            if (path == string.Empty)
                path = "None";
        }

        public void Approve()
        {   
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Content = new FileTransferViewModel(FileTransferViewModel.TransferAction.Download, applicationName, GetPath(), Downloaded),
                Width = FileTransferViewModel.PreferredWidth,
                Height = FileTransferViewModel.PreferredHeight
            }) ;
        }

        void Downloaded()
        {
            DownloadSettings downloadSettings = new DownloadSettings()
            {
                DeleteNotNecessary = deleteNotNecessary,
                IsDownloaded = true,
                UseDefaultPath = useDefaultPath,
                Path = GetPath()
            };

            libraryService.AddDownloadSettings(applicationName, downloadSettings);
            libraryService.ClearView();
        }
         
        string GetPath()
        {
            if (useDefaultPath)
                return $"./Apps/{applicationName}/";
            else
                return path;
        }
    }
}
