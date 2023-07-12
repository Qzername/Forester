using Forester.Data;
using Forester.Models.API;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.Manage
{
    internal class UploadNewVersionViewModel : ViewModelBase
    {
        Application currentApplication;

        [Reactive] string newVersion { get; set; }
        [Reactive] string newVersionCurrentPath { get; set; }
        [Reactive] string error { get; set; }

        //dependency injection
        DialogService dialogService;
        DeveloperService developerService;
        ApplicationDatabase applicationDatabase;

        public UploadNewVersionViewModel(Application application)
        {
            currentApplication = application;

            dialogService = GetService<DialogService>();
            developerService = GetService<DeveloperService>();
            applicationDatabase = GetService<ApplicationDatabase>();

            newVersionCurrentPath = "None";
        }

        public async void SelectPath()
        {
            newVersionCurrentPath = await dialogService.OpenDirectoryDialog(new Avalonia.Platform.Storage.FolderPickerOpenOptions());

            if (newVersionCurrentPath == string.Empty)
                newVersionCurrentPath = "None";
        }

        public void Submit()
        {
            if(newVersionCurrentPath == "None")
            {
                error = "Path cannot be empty";
                return;
            }

            if(newVersion == string.Empty)
            {
                error = "New version cannot be empty";
                return;
            }

            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Content = new FileTransferViewModel(FileTransferViewModel.TransferAction.Upload, currentApplication.Name, newVersionCurrentPath, UploadCompleted),
                Height = FileTransferViewModel.PreferredHeight, 
                Width = FileTransferViewModel.PreferredWidth
            });

            dialogService.ChangeVisibility(true);
        }

        async void UploadCompleted()
        {
            await applicationDatabase.Update(currentApplication.Name, new Application()
            {
                Version = newVersion
            });

            await developerService.RefreshApplicationList();
            developerService.MoveToApp(currentApplication.Name);
        }
    }
}
