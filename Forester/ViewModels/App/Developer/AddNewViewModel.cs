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
            if (Ava.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = (await desktop.MainWindow!.StorageProvider.OpenFolderPickerAsync(new Ava.Platform.Storage.FolderPickerOpenOptions()))[0].Path.LocalPath;

                Debug.Log(result);

                if (result is null)
                    return;

                pathToFolder = result;
            }
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

            dialogService.ChangeConfiguration(new Models.Configurations.DialogConfiguration()
            {
                Content = new FileTransferViewModel(FileTransferViewModel.Action.Upload, name, pathToFolder),
                Width = 800,
                Height = 500,
            });

            dialogService.ChangeVisibility(true);
        }
    }
}
