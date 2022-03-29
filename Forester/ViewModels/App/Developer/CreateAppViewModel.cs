using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.Code;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Forester.ViewModels.App.Developer
{
    public class CreateAppViewModel : ViewModelBase
    {
        string _name;
        string _version;
        string _pathToFolder;
        string _error;
        float _percent;

        public string name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string version
        {
            get => _version;
            set => this.RaiseAndSetIfChanged(ref _version, value);
        }
        public string pathToFolder
        {
            get => _pathToFolder;
            set => this.RaiseAndSetIfChanged(ref _pathToFolder, value);
        }
        public string error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }
        public float percent
        {
            get => _percent;
            set => this.RaiseAndSetIfChanged(ref _percent, value);
        }

        DeveloperViewModel developerVM;

        public CreateAppViewModel(DeveloperViewModel developerVM)
        {
            this.developerVM = developerVM;
            pathToFolder = "None";
        }

        public void Publish()
        {
            error = "";
            percent = 0f;

            if (string.IsNullOrEmpty(pathToFolder) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(name))
            {
                error = "All credentials needs to be fullfield";
                return;
            }

            if(pathToFolder == "None")
            {
                error = "Path cannot be empty";
                return;
            }

            Thread uploadThread = new Thread(UploadThread);
            uploadThread.Start();
        }

        async void UploadThread()
        {
            Application app = new Application()
            {
                ID = 0,
                name = name,
                version = version,
                isPrivate = "True",
                description = "",
                quickDescription = "",
                downloadNumber = 0,
                mainDeveloper = 1,
            };

            var result = ServerConnection.Post("/api/Applications/New", app);

            if (!result.IsSuccessStatusCode)
            {
                error = "Something went wrong. Probably name is taken";
                return;
            }

            //Utworzono nową aplikacje, przeysłam pierwszą wersje
            error = "Preparing app...";
            string path = AppFileManager.PrepareApp(pathToFolder);

            WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            client.UploadFileCompleted += Client_UploadFileCompleted;

            error = "Uploading...";

            await ServerConnection.Upload(ref client, "/api/Download/Upload?name=" + app.name, path);
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            error = "";
            percent = 0f;

            AppFileManager.Clear();
            pathToFolder = "None";

            developerVM.RefreshList();
            developerVM.ChangeView(name);

            name = string.Empty;
            version = string.Empty;
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            float progress = (float)e.BytesSent/ new FileInfo("./tempApp.zip").Length;

            if (progress * 100 < percent)
                return;

            percent = (float)Math.Round(progress *100,2);
        }

        public async void SelectFolder()
        {
            var dialog = new OpenFolderDialog();
            
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);
                
                if (result is null)
                    return;

                pathToFolder = result;
            }
        }
    }
}
