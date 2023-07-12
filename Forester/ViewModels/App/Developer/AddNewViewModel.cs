using Avalonia.Controls.ApplicationLifetimes;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Avalonia.Controls;
using Ava = Avalonia;
using Forester.Tools;
using Forester.Data;
using Forester.Models.API;
using Forester.Services;
using Forester.ViewModels.Dialogs;
using Forester.Views.App;
using Forester.Services.App;

namespace Forester.ViewModels.App.Developer
{
    public class AddNewViewModel : RoutableBase
    {
        [Reactive] string name { get; set; }
        [Reactive] string version { get; set; }
        [Reactive] string pathToFolder { get; set; }
        [Reactive] string error { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;
        DialogService dialogService;

        public AddNewViewModel(IScreen screen) : base(screen)
        {
            applicationDatabase = GetService<ApplicationDatabase>();
            dialogService = GetService<DialogService>();

            error = "";
            pathToFolder = "None";
        }

        public async void SelectFolder()
        {
            pathToFolder = await dialogService.OpenDirectoryDialog(new Ava.Platform.Storage.FolderPickerOpenOptions());
        
            if(pathToFolder == string.Empty)
                pathToFolder = "None";
        }

        public void Submit()
        {
            error = "";

            if (string.IsNullOrEmpty(pathToFolder) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(name))
            {
                error = "All credentials needs to be fullfield";
                return;
            }

            if (pathToFolder == "None")
            {
                error = "Path cannot be empty";
                return;
            }

            Publish();
        }

        async void Publish()
        {
            bool isDone =  await applicationDatabase.Create(new Application()
            {
                Name = name,
            });

            if(!isDone)
            {
                error = "Something went horribly wrong";
                return;
            }

            dialogService.ChangeConfiguration(new Models.Configurations.DialogConfiguration()
            {
                Content = new FileTransferViewModel(FileTransferViewModel.TransferAction.Upload, name, pathToFolder, UploadFinished),
                Width = FileTransferViewModel.PreferredWidth,
                Height = FileTransferViewModel.PreferredHeight,
            });
            dialogService.ChangeVisibility(true);
        }

        async void UploadFinished()
        {
            await applicationDatabase.Update(name, new Application()
            {
                Version = version
            });

            var developerService = GetService<DeveloperService>();

            await developerService.RefreshApplicationList();
            developerService.MoveToApp(name);
        }
    }
}
