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
        string name;
        string version;
        string pathToFolder;
        string error;
        float percent;

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
        public string Version
        {
            get => version;
            set => this.RaiseAndSetIfChanged(ref version, value);
        }
        public string PathToFolder
        {
            get => pathToFolder;
            set => this.RaiseAndSetIfChanged(ref pathToFolder, value);
        }
        public string Error
        {
            get => error;
            set => this.RaiseAndSetIfChanged(ref error, value);
        }
        public float Percent
        {
            get => percent;
            set => this.RaiseAndSetIfChanged(ref percent, value);
        }

        DeveloperViewModel developerVM;

        public CreateAppViewModel(DeveloperViewModel developerVM)
        {
            this.developerVM = developerVM;
            PathToFolder = "None";
        }

        public void Publish()
        {
            Error = "";
            Percent = 0f;

            if (string.IsNullOrEmpty(PathToFolder) || string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(Name))
            {
                Error = "All credentials needs to be fullfield";
                return;
            }

            if(PathToFolder == "None")
            {
                Error = "Path cannot be empty";
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
                name = Name,
                version = Version,
                isPrivate = "True",
                description = "",
                quickDescription = "",
                downloadNumber = 0,
                mainDeveloper = 1,
            };

            var result = ServerConnection.Post("/api/Applications/New", app);

            if (!result.IsSuccessStatusCode)
            {
                Error = "Something went wrong. Probably name is taken";
                return;
            }

            //Utworzono nową aplikacje, przeysłam pierwszą wersje
            Error = "Preparing app...";
            string path = AppFileManager.PrepareApp(PathToFolder);

            Error = "Uploading...";

            await ServerConnection.Upload(path, "/api/Download/Upload?name=" + app.name);
            Client_UploadFileCompleted(null, null);

           /* WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            client.UploadFileCompleted += Client_UploadFileCompleted;

            await ServerConnection.Upload(ref client, "/api/Download/Upload?name=" + app.name, path);*/
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            Error = "";
            Percent = 0f;

            AppFileManager.Clear();
            PathToFolder = "None";

            developerVM.RefreshList();
            developerVM.ChangeView(Name);

            Name = string.Empty;
            Version = string.Empty;
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            float progress = (float)e.BytesSent/ new FileInfo("./tempApp.zip").Length;

            if (progress * 100 < Percent)
                return;

            Percent = (float)Math.Round(progress *100,2);
        }

        public async void SelectFolder()
        {
            var dialog = new OpenFolderDialog();
            
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);
                
                if (result is null)
                    return;

                PathToFolder = result;
            }
        }
    }
}
